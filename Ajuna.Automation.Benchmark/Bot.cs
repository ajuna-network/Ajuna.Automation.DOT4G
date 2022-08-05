using Ajuna.NetApi.Model.AjunaCommon;
using Ajuna.NetApi.Model.Dot4gravity;
using Serilog;
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

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        public Bot(NodeClient nodeClient, WorkerClient workerClient)
        {
            _nodeClient = nodeClient;
            _workerClient = workerClient;

            _tracker = new Dictionary<string, long[]>();
            _stopwatch = new Stopwatch();
        }

        internal async Task RunAsync(CancellationToken token)
        {
            var SleepTime = 1000;

            NodeState nodeState = NodeState.None;
            WorkerState workerState = WorkerState.None;

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

                    }
                }

                Thread.Sleep(SleepTime);
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
                return ChangeWorkerState(workerState, WorkerState.Connect);
            }

            // ShieldingKey
            if (!_workerClient.HasShieldingKey)
            {
                return ChangeWorkerState(workerState, WorkerState.ShieldingKey);
            }

            var balanceWorker = await _workerClient.GetBalanceAsync();

            // Faucet
            if (balanceWorker is null || balanceWorker.Value < 100)
            {
                return ChangeWorkerState(workerState, WorkerState.Faucet);
            }

            return ChangeWorkerState(workerState, WorkerState.Game);

            //var gameBoard = await _workerClient.GetGameBoardAsync();

            //// Wait
            //if (gameBoard is null)
            //{
            //    return ChangeWorkerState(workerState, WorkerState.Wait);
            //}

            //// Bomb
            //if (gameBoard.GamePhase == GamePhase.Bomb)
            //{
            //    var player = gameBoard.Players.Values.Where(p => p.Address == _workerClient.Account.Value).ToList();
            //    if (player.Count == 1 && player[0].Bombs > 0)
            //    {
            //        return ChangeWorkerState(workerState, WorkerState.Bomb);
            //    }
            //    else
            //    {
            //        return ChangeWorkerState(workerState, WorkerState.OpBomb);
            //    }
            //}

            //// Finished, Play & OpTurn
            //if (gameBoard.GamePhase == GamePhase.Play)
            //{
            //    if (gameBoard.PossibleMoves.Count() == 0 || gameBoard.Winner != null && gameBoard.Winner.Count() > 0)
            //    {
            //        return ChangeWorkerState(workerState, WorkerState.Finished);
            //    }
                
            //    if (gameBoard.Next == _workerClient.Account.Value)
            //    {
            //        return ChangeWorkerState(workerState, WorkerState.Stone);
            //    }
            //    else
            //    {
            //        return ChangeWorkerState(workerState, WorkerState.OpStone);
            //    }
            //}

            //return WorkerState.None;
        }

        private async Task<NodeState> GetNodeStateAsync(NodeState nodeState, CancellationToken token)
        {

            // Connect
            if (!_nodeClient.IsConnected)
            {
                return ChangeNodeState(nodeState, NodeState.Connect);
            }

            var accountInfo = await _nodeClient.GetBalanceNodeAsync(token);

            // Faucet
            if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value < 1000000000000)
            {
                return ChangeNodeState(nodeState, NodeState.Faucet);
            }

            var playerQueued = await _nodeClient.GetPlayerQueueAsync(token);

            var runnerId = await _nodeClient.GetRunnerIdAsync(token);

            // Queue & Players
            if (runnerId == null || runnerId.Value == 0)
            {
                return playerQueued == null || playerQueued.Value == 0
                    ? ChangeNodeState(nodeState, NodeState.Queue)
                    : ChangeNodeState(nodeState, NodeState.Players);
            }          

            var runnerState = await _nodeClient.GetRunnerStateAsync(runnerId, token);

            if (runnerState == null)
            {
                return ChangeNodeState(nodeState, NodeState.Wait);
            }

            // Worker, Play & Finished
            return runnerState.Value switch
            {
                RunnerState.Queued => ChangeNodeState(nodeState, NodeState.Worker),
                RunnerState.Accepted => ChangeNodeState(nodeState, NodeState.Play),
                RunnerState.Finished => ChangeNodeState(nodeState, NodeState.Finished),
                _ => NodeState.None,
            };
        }

        private NodeState ChangeNodeState(NodeState oldState, NodeState newState)
        {
            if (oldState == newState)
            {
                return oldState;
            }

            var key = "Node" + oldState.ToString();
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

        private WorkerState ChangeWorkerState(WorkerState oldState, WorkerState newState)
        {
            if (oldState == newState)
            {
                return oldState;
            }

            var key = "Worker" + oldState.ToString();
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