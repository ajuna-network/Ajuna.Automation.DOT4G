using Ajuna.Automation.AI;
using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Ajuna.NetApi.Model.AjunaCommon;
using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApi.Model.Types.Primitive;
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

    public class PlayBot
    {
        private readonly NodeClient _nodeClient;
        private readonly WorkerClient _workerClient;
        private readonly IBotAI _logic;

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        private (U32?, RunnerState) _currentRunner;
        private Dot4GObj? _gameBoard;

        public PlayBot(NodeClient nodeClient, WorkerClient workerClient, IBotAI logic)
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

            _currentRunner = (null, RunnerState.None);

            NodeState nodeState = NodeState.None;
            WorkerState workerState = WorkerState.None;
            PlayState playState = PlayState.None;

            _stopwatch.Start();

            while (!token.IsCancellationRequested)
            {
                nodeState = await GetNodeStateAsync(nodeState, token);
                await DoNodeAsync(nodeState, token);

                if (nodeState == NodeState.Play)
                {
                    workerState = await GetWorkerStateAsync(workerState, token);
                    await DoWorkerAsync(workerState, token);
                    if (workerState == WorkerState.Game)
                    {
                        _gameBoard = await _workerClient.GetGameBoardAsync();
                        if (_gameBoard != null)
                        {
                            var myTurn = _gameBoard.Next == _nodeClient.Account.Value;
                            var winner = _gameBoard.Winner != null && _gameBoard.Winner.Any();
                            Log.Information("GameBoard[{id}|{phase}] - turn:{bool} [e{empty}|m{moves}] win:{winner}", _gameBoard.Id, _gameBoard.GamePhase, myTurn, _gameBoard.EmptySlots.Count, _gameBoard.PossibleMoves.Count, winner);
                        }
                        playState = GetPlayState(playState, _gameBoard);
                        await DoPlayAsync(playState, _gameBoard);
                    }
                }

                Thread.Sleep(SleepTime);
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

                case NodeState.Finished:
                    var downTime = 60000;
                    Log.Information("Downtime for {value} sec.", (double)downTime / 1000);
                    Thread.Sleep(downTime);
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

        private async Task DoPlayAsync(PlayState playState, Dot4GObj? gameBoard)
        {
            if (gameBoard == null)
            {
                return;
            }

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

        private async Task<NodeState> GetNodeStateAsync(NodeState nodeState, CancellationToken token)
        {
            // Connect
            if (!_nodeClient.IsConnected)
            {
                return ChangeState(nodeState, NodeState.Connect);
            }

            var accountInfo = await _nodeClient.GetBalanceNodeAsync(true, token);

            // Faucet
            if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value < 1000000000000)
            {
                return ChangeState(nodeState, NodeState.Faucet);
            }

            // Queue & Players
            if (_currentRunner.Item1 == null || _currentRunner.Item1.Value == 0)
            {
                var playerQueued = await _nodeClient.GetPlayerQueueAsync(token);
                _currentRunner.Item1 = await _nodeClient.GetRunnerIdAsync(token);
                _currentRunner.Item2 = ChangeState(_currentRunner.Item2, RunnerState.None);

                if (playerQueued != null && playerQueued.Value > 0)
                {
                    return ChangeState(nodeState, NodeState.Players);
                }

                if (_currentRunner.Item1 == null || _currentRunner.Item1.Value == 0)
                {
                    return ChangeState(nodeState, NodeState.Queue);
                } 
            }

            var newRunnerState = await _nodeClient.GetRunnerStateAsync(_currentRunner.Item1, token);

            // Worker, Play & Finished
            switch (newRunnerState?.Value)
            {
                case RunnerState.Queued:
                    _currentRunner.Item2 = ChangeState(_currentRunner.Item2, newRunnerState.Value, $"runner:{_currentRunner.Item1.Value}");
                    return ChangeState(nodeState, NodeState.Worker);

                case RunnerState.Accepted:
                    _currentRunner.Item2 = ChangeState(_currentRunner.Item2, newRunnerState.Value, $"runner:{_currentRunner.Item1.Value}");
                    return ChangeState(nodeState, NodeState.Play);

                case RunnerState.Finished:
                    var oldId = _currentRunner.Item1.Value;
                    Log.Debug("Runner ID {id} is {state}", oldId, newRunnerState?.Value);
                    _currentRunner.Item1 = null;
                    _currentRunner.Item2 = ChangeState(_currentRunner.Item2, RunnerState.None, $"runner:{oldId}");
                    return ChangeState(nodeState, NodeState.Finished);

                default: 
                    return ChangeState(nodeState, NodeState.None);
            };
        }

        private async Task<WorkerState> GetWorkerStateAsync(WorkerState workerState, CancellationToken token)
        {
            // Connect
            if (!_workerClient.IsConnected)
            {
                return ChangeState(workerState, WorkerState.Connect);
            }

            // check for slow mode bug
            if (_workerClient.client.RPCDelayed)
            {
                return ChangeState(workerState, WorkerState.Disconnect);
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

        private PlayState GetPlayState(PlayState playState, Dot4GObj gameBoard)
        {
            if (gameBoard == null)
            {
                return ChangeState(playState, PlayState.NoBoard);
            }

            switch (gameBoard.GamePhase)
            {
                case GamePhase.Bomb:

                    var player = gameBoard.Players.Values.Where(p => p.Address == _workerClient.Account.Value).ToList();
                    
                    if (player.Count == 1 && player[0].Bombs > 0)
                    {
                        return ChangeState(playState, PlayState.Bomb);
                    }

                    return ChangeState(playState, PlayState.OpBomb);

                case GamePhase.Play:

                    if (gameBoard.Winner != null && gameBoard.Winner.Any())
                    {
                        return ChangeState(playState, PlayState.Finished, $"winner:{gameBoard.Winner}");
                    }

                    if (!gameBoard.PossibleMoves.Any())
                    {
                        return ChangeState(playState, PlayState.Finished, $"draw");
                    }

                    if (gameBoard.Next == _workerClient.Account.Value)
                    {
                        return ChangeState(playState, PlayState.Stone, $"moves:{gameBoard.PossibleMoves.Count()}");
                    }

                    return ChangeState(playState, PlayState.OpStone);


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
                Log.Debug("Waiting on {count} extrinsic proccesed!", running.Count());
                while (running.Any())
                {
                    Thread.Sleep(1000);
                    running = _nodeClient.ExtrinsicManger.Running;
                }
                Log.Debug("All extrinsic proccessed!");
            }
        }

        private T ChangeState<T>(T oldState, T newState, string msg = "")
        {
            if (oldState == null || newState == null)
            {
                return oldState;
            }

            if (oldState.Equals(newState))
            {
                return oldState;
            }

            var key = typeof(T).Name + "_" + oldState.ToString();
            var elapsedMs = _stopwatch.ElapsedMilliseconds;
            if (_tracker.TryGetValue(key, out long[] values))
            {
                values[0] = values[0] + 1;
                values[1] = values[1] + elapsedMs;
                _tracker[key] = values;
            }
            else
            {
                _tracker.Add(key, new long[] { 1, elapsedMs });
            }
            _stopwatch.Restart();

            if (msg != null && msg.Any())
            {
                msg = " - " + msg;
            }

            Log.Information("{name} = {state1} from {state2} in {ms} sec.{msg}", typeof(T).Name, newState, oldState, (double)elapsedMs / 1000, msg);

            return newState;
        }

    }
}