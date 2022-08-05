using Ajuna.NetApi.Model.AjunaCommon;
using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApi.Model.Types.Primitive;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Ajuna.UnityInterface;
using Dot4GBot.AI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation.Benchmark.Bot
{


    internal class Bot
    {
        public enum NodeState
        {
            None,
            Connect,
            Faucet,
        }

        private readonly BotClient _uClient;


        public Dictionary<string, long[]> Tracker;

        private Stopwatch stopwatch;

        public Bot(Dot4GClient dot4gClient, IBotAI logic, DisplayType displayType)
        {
            _uClient = dot4gClient;
            _logic = logic;
            _displayType = displayType;

            Tracker = new Dictionary<string, long[]>();
            stopwatch = new Stopwatch();
        }

        internal async Task RunAsync(CancellationToken token)
        {

            NodeState nodeState = NodeState.None;
            WorkerState workerState = WorkerState.None;

            stopwatch.Start();

            int count = 0;

            while (!token.IsCancellationRequested)
            {
                count++;

                var SleepTime = 1000;
                Dot4GObj gameBoard = null;
                U32 runnerId = null;

                nodeState = await GetNodeStateAsync(nodeState);

                switch (nodeState)
                {
                    case NodeState.Connect:
                        _ = await _uClient.ConnectNodeAsync();
                        break;

                    case NodeState.Faucet:
                        _ = await _uClient.FaucetAsync();
                        break;

                    case NodeState.Queue:
                        _ = await _uClient.QueueAsync();
                        break;

                    case NodeState.None:
                        break;

                    case NodeState.Players:
                        break;

                    case NodeState.Worker:
                        break;

                    case NodeState.Play:
                        break;
                }

                if (nodeState == NodeState.Play)
                {
                    var balanceWorker = await _uClient.GetBalanceWorkerAsync();
                    if (balanceWorker is null || balanceWorker.Value < 100)
                    {
                        workerState = ChangeWorkerState(workerState, WorkerState.Faucet);
                    }
                    else
                    {
                        gameBoard = await _uClient.GetGameBoardAsync();
                        if (gameBoard is null)
                        {
                            workerState = ChangeWorkerState(workerState, WorkerState.Wait);
                        }
                        else if (gameBoard.GamePhase == GamePhase.Bomb)
                        {
                            var player = gameBoard.Players.Values.Where(p => p.Address == _uClient.Account.Value).ToList();
                            if (player.Count == 1 && player[0].Bombs > 0)
                            {
                                workerState = ChangeWorkerState(workerState, WorkerState.Bomb);
                            }
                            else
                            {
                                workerState = ChangeWorkerState(workerState, WorkerState.OpBomb);
                            }
                        }
                        else if (gameBoard.GamePhase == GamePhase.Play)
                        {
                            if (gameBoard.Winner != null && gameBoard.Winner.Count() > 0)
                            {
                                workerState = ChangeWorkerState(workerState, WorkerState.None);
                            }
                            else if (gameBoard.Players[gameBoard.Next].Address == _uClient.Account.Value)
                            {
                                workerState = ChangeWorkerState(workerState, WorkerState.Play);
                            }
                            else
                            {
                                workerState = ChangeWorkerState(workerState, WorkerState.OpTurn);
                            }
                        }
                    }

                    switch (workerState)
                    {
                        case WorkerState.None:
                            break;

                        case WorkerState.Faucet:
                            var faucet = await _uClient.FaucetWorkerAsync();
                            if (faucet)
                            {
                                SleepTime = 1000;
                            }
                            break;

                        case WorkerState.Wait:
                        case WorkerState.OpBomb:
                        case WorkerState.OpTurn:
                            SleepTime = 300;
                            break;

                        case WorkerState.Bomb:
                            int[] bombPos = _logic.Bombs(gameBoard);
                            var bomb = await _uClient.BombAsync(bombPos[0], bombPos[1]);
                            if (bomb)
                            {
                                SleepTime = 300;
                            }
                            break;

                        case WorkerState.Play:
                            (Side, int) move = _logic.Play(gameBoard);
                            var stone = await _uClient.StoneAsync(move.Item1, move.Item2);
                            if (stone)
                            {
                                SleepTime = 300;
                            }
                            break;
                    }
                }

                // wait on extrinsic
                if (_uClient.HasExtrinsics > 0)
                {
                    while (_uClient.HasExtrinsics > 0)
                    {
                        Print(Name, nodeState, runnerId, workerState, true);
                        Console.WriteLine($"+-------------------------------------{_uClient.HasExtrinsics}-+");
                        Thread.Sleep(500);
                    }
                    continue;
                }

                Print(Name, nodeState, runnerId, workerState);
                // print board here ...
                if (gameBoard != null)
                {
                    gameBoard.Print();
                }
                else
                {
                    Console.WriteLine("+--------------------" + (count % 3 == 0 ? workerState.ToString().PadRight(17, ' ').PadLeft(18, ' ') + "-+" : "-------------------+"));
                }

                if (count == int.MaxValue)
                {
                    count = 0;
                }

                Console.WriteLine((count % 2 == 0 ? "-" : "|").PadLeft(41));

                Thread.Sleep(SleepTime);
            }
        }

        private async Task<NodeState> GetNodeStateAsync(NodeState nodeState)
        {

            // Connect
            if (!_uClient.IsNodeConnected)
            {
                return ChangeNodeState(nodeState, NodeState.Connect);
            }

            var accountInfo = await _uClient.GetBalanceNodeAsync();

            // Faucet
            if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value < 1000000000000)
            {
                return ChangeNodeState(nodeState, NodeState.Faucet);
            }

            var playerQueued = await _uClient.GetPlayerQueueAsync();
            var runnerId = await _uClient.GetRunnerIdAsync();

            // Queue
            if (playerQueued.Value == 0 && runnerId.Value == 0)
            {
                return ChangeNodeState(nodeState, NodeState.Queue);
            }

            // Players
            if (playerQueued.Value > 0 && runnerId.Value == 0)
            {
                return ChangeNodeState(nodeState, NodeState.Players);
            }

            var runnerState = await _uClient.GetRunnerStateAsync(runnerId);

            // Worker
            if (runnerState.Value == RunnerState.Queued)
            {
                return ChangeNodeState(nodeState, NodeState.Worker);
            }

            // Play
            if (runnerState.Value == RunnerState.Accepted)
            {
                await _uClient.ConnectTeeAsync();
                return ChangeNodeState(nodeState, NodeState.Play);
            }

            // Queue
            if (runnerState.Value == RunnerState.Finished)
            {
                await _uClient.DisconnectTeeAsync();
                return ChangeNodeState(nodeState, NodeState.Queue);
            }

            return NodeState.None;
        }

        private NodeState ChangeNodeState(NodeState nodeState, object connect)
        {
            throw new NotImplementedException();
        }

        private NodeState ChangeNodeState(NodeState oldState, NodeState newState)
        {
            if (oldState == newState)
            {
                return oldState;
            }

            var key = "Node" + oldState.ToString();
            if (Tracker.TryGetValue(key, out long[] values))
            {
                values[0] = values[0] + 1;
                values[1] = values[1] + stopwatch.ElapsedMilliseconds;
                Tracker[key] = values;
            }
            else
            {
                Tracker.Add(key, new long[] { 1, stopwatch.ElapsedMilliseconds });
            }
            stopwatch.Restart();

            return newState;
        }

        private WorkerState ChangeWorkerState(WorkerState oldState, WorkerState newState)
        {
            if (oldState == newState)
            {
                return oldState;
            }

            var key = "Worker" + oldState.ToString();
            if (Tracker.TryGetValue(key, out long[] values))
            {
                values[0] = values[0] + 1;
                values[1] = values[1] + stopwatch.ElapsedMilliseconds;
                Tracker[key] = values;
            }
            else
            {
                Tracker.Add(key, new long[] { 1, stopwatch.ElapsedMilliseconds });
            }
            stopwatch.Restart();

            return newState;
        }

        private void Print(string name, NodeState nodeState, U32 runnerId, WorkerState workerState, bool flag = false)
        {
            Console.Clear();
            Console.WriteLine("+---------------------------------------+");
            Console.WriteLine("| " + $"Name: {name}       Node: {nodeState}".PadRight(38) + "|");
            Console.WriteLine("| " + $" R-Id: {(runnerId != null ? runnerId.Value.ToString() : "").PadRight(6)}     Worker: {workerState}".PadRight(38) + "|");
            switch (nodeState)
            {
                case NodeState.None:
                    break;
                case NodeState.Faucet:
                    Console.WriteLine("| " + $"Let's robe Alice faucet! ".PadRight(38) + "|");
                    break;
                case NodeState.Queue:
                    Console.WriteLine("| " + $"Queuing up, now.".PadRight(38) + "|");
                    break;
                case NodeState.Players:
                    Console.WriteLine("| " + $"Waiting for Player.".PadRight(38) + "|");
                    break;
                case NodeState.Play:
                    Console.WriteLine("| " + $"Playtime!".PadRight(38) + "|");
                    break;
                case NodeState.Worker:
                    Console.WriteLine("| " + $"Waiting on TEE.".PadRight(38) + "|");
                    break;
                default:
                    Console.WriteLine("| " + $" ".PadRight(38) + "|");
                    break;
            }

            if (flag)
            {
                Console.WriteLine("| " + $" Waiting on In-Block".PadRight(38) + "|");
            }
            else
            {
                Console.WriteLine("| " + $" ".PadRight(38) + "|");
            }


        }
    }
}