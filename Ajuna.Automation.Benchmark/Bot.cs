using Ajuna.Automation.AI;
using Ajuna.Automation.Enums;
using Ajuna.NetApi.Model.AjunaCommon;
using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{

    public class Bot
    {
        private readonly NodeClient _nodeClient;
        private readonly WorkerClient _workerClient;
        private readonly IBotAI _logic;

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        public Bot(NodeClient nodeClient, WorkerClient workerClient, IBotAI logic)
        {
            _nodeClient = nodeClient;
            _workerClient = workerClient;
            _logic = logic;

            _tracker = new Dictionary<string, long[]>();
            _stopwatch = new Stopwatch();
        }

        internal async Task RunAsync(CancellationToken token)
        {
            var SleepTime = 1000;

            NodeState nodeState = NodeState.None;
            WorkerState workerState = WorkerState.None;
            PlayState playState = PlayState.None;

            _stopwatch.Start();

            while (!token.IsCancellationRequested)
            {
                nodeState = await GetNodeStateAsync(nodeState, token);
                Log.Information("node state is {state}", nodeState);
                await DoNodeAsync(nodeState, token);

                if (nodeState == NodeState.Play)
                {
                    workerState = await GetWorkerStateAsync(workerState, token);
                    Log.Information("worker state is {state}", workerState);
                    await DoWorkerAsync(workerState, token);
                    if (workerState == WorkerState.Game)
                    {
                        var gameBoard = await _workerClient.GetGameBoardAsync();
                        if (gameBoard != null)
                        {
                            Log.Information("GameBoard[{id}|{phase}]:{hash} Empty:{empty},Moves:{moves}", gameBoard.Id, gameBoard.GamePhase, gameBoard.Next, gameBoard.EmptySlots.Count, gameBoard.PossibleMoves.Count);
                            playState = GetPlayState(playState, gameBoard);
                            Log.Information("play state is {state}", playState);
                            await DoPlayAsync(playState, gameBoard);
                        } 
                        else
                        {

                        }
                    }
                }

                Thread.Sleep(SleepTime);
            }
        }

        private async Task DoPlayAsync(PlayState playState, Dot4GObj gameBoard)
        {
            switch (playState)
            {
                case PlayState.Bomb:
                    int[] bombPos = _logic.Bombs(gameBoard);
                    _ = await _workerClient.BombAsync(bombPos[0], bombPos[1]);
                    break;

                case PlayState.Stone:
                    (Side, int) move = _logic.Play(gameBoard);
                    _ = await _workerClient.StoneAsync(move.Item1, move.Item2);
                    break;
            }
        }

        private PlayState GetPlayState(PlayState playState, Dot4GObj gameBoard)
        {
            if (gameBoard == null)
            {
                return ChangeState(playState, PlayState.None);
            }

            switch (gameBoard.GamePhase)
            {
                case GamePhase.Bomb:
                    {
                        var player = gameBoard.Players.Values.Where(p => p.Address == _workerClient.Account.Value).ToList();
                        if (player.Count == 1 && player[0].Bombs > 0)
                        {
                            return ChangeState(playState, PlayState.Bomb);
                        }
                        else
                        {
                            return ChangeState(playState, PlayState.OpBomb);
                        }
                    }

                case GamePhase.Play:
                    if (gameBoard.PossibleMoves.Count() == 0 || gameBoard.Winner != null && gameBoard.Winner.Count() > 0)
                    {
                        return ChangeState(playState, PlayState.Finished);
                    }

                    if (gameBoard.Next == _workerClient.Account.Value)
                    {
                        return ChangeState(playState, PlayState.Stone);
                    }
                    else
                    {
                        return ChangeState(playState, PlayState.OpStone);
                    }

                default:
                    return ChangeState(playState, PlayState.None);
            }
        }

        private void WaitOnExtrinsic()
        {
            // wait on extrinsic
            var running = _nodeClient.ExtrinsicManger.Running;
            if (running.Any())
            {
                Log.Information("Waiting on {count} extrinsic proccesed!", running.Count());
                while (running.Any())
                {
                    Thread.Sleep(1000);
                    running = _nodeClient.ExtrinsicManger.Running;
                }
                Log.Information("All extrinsic proccessed!");
            }
        }

        private async Task DoNodeAsync(NodeState nodeState, CancellationToken token)
        {
            switch (nodeState)
            {
                case NodeState.Connect:
                    _ = await _nodeClient.ConnectAsync(false, true, token);
                    break;

                case NodeState.Faucet:
                    if (await _nodeClient.FaucetAsync(token))
                    {
                        WaitOnExtrinsic();
                    }
                    break;

                case NodeState.Queue:
                    if (await _nodeClient.QueueAsync(token))
                    {
                        WaitOnExtrinsic();
                    }
                    break;

                case NodeState.Disconnect:
                    _ = await _nodeClient.DisconnectAsync();
                    break;

                default:
                    break;
            }
        }

        private async Task DoWorkerAsync(WorkerState workerState, CancellationToken token)
        {
            switch (workerState)
            {
                case WorkerState.Connect:
                    _ = await _workerClient.ConnectAsync(false, false, token);
                    break;

                case WorkerState.ShieldingKey:
                    _ = await _workerClient.GetShieldingKeyAsync();
                    break;

                case WorkerState.Faucet:
                    _ = await _workerClient.FaucetAsync();
                    break;

                case WorkerState.Disconnect:
                    _ = await _workerClient.DisconnectAsync();
                    break;

                default:
                    break;
            }
        }

        private async Task<WorkerState> GetWorkerStateAsync(WorkerState workerState, CancellationToken token)
        {
            // Connect
            if (!_workerClient.IsConnected)
            {
                return ChangeState(workerState, WorkerState.Connect);
            }

            // ShieldingKey
            if (!_workerClient.HasShieldingKey)
            {
                return ChangeState(workerState, WorkerState.ShieldingKey);
            }

            var balanceWorker = await _workerClient.GetBalanceAsync();

            // Faucet
            if (balanceWorker is null || balanceWorker.Value < 100)
            {
                return ChangeState(workerState, WorkerState.Faucet);
            }

            return ChangeState(workerState, WorkerState.Game);
        }

        private async Task<NodeState> GetNodeStateAsync(NodeState nodeState, CancellationToken token)
        {

            // Connect
            if (!_nodeClient.IsConnected)
            {
                return ChangeState(nodeState, NodeState.Connect);
            }

            var accountInfo = await _nodeClient.GetBalanceNodeAsync(token);

            // Faucet
            if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value < 1000000000000)
            {
                return ChangeState(nodeState, NodeState.Faucet);
            }

            var playerQueued = await _nodeClient.GetPlayerQueueAsync(token);

            var runnerId = await _nodeClient.GetRunnerIdAsync(token);

            // Queue & Players
            if (runnerId == null || runnerId.Value == 0)
            {
                return playerQueued == null || playerQueued.Value == 0
                    ? ChangeState(nodeState, NodeState.Queue)
                    : ChangeState(nodeState, NodeState.Players);
            }          

            var runnerState = await _nodeClient.GetRunnerStateAsync(runnerId, token);

            if (runnerState == null)
            {
                return ChangeState(nodeState, NodeState.Wait);
            }

            // Worker, Play & Finished
            return runnerState.Value switch
            {
                RunnerState.Queued => ChangeState(nodeState, NodeState.Worker),
                RunnerState.Accepted => ChangeState(nodeState, NodeState.Play),
                RunnerState.Finished => ChangeState(nodeState, NodeState.Finished),
                _ => NodeState.None,
            };
        }

        private T ChangeState<T>(T oldState, T newState)
        {
            if (oldState is null || newState is null)
            {
                return oldState;
            }

            if (oldState.Equals(newState))
            {
                return oldState;
            }

            var key = oldState.GetType().Name + "_" + oldState.ToString();
            if (_tracker.TryGetValue(key, out long[] values))
            {
                values[0] = values[0] + 1;
                values[1] = values[1] + _stopwatch.ElapsedMilliseconds;
                _tracker[key] = values;
            }
            else
            {
                _tracker.Add(key, new long[] { 1, _stopwatch.ElapsedMilliseconds });
            }
            _stopwatch.Restart();

            return newState;
        }

    }
}