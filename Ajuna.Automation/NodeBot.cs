using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    internal class BalanceNodeBot
    {
        private NodeClient _client;

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        public BalanceNodeBot(NodeClient nodeClient)
        {
            _client = nodeClient;
            _tracker = new Dictionary<string, long[]>();
            _stopwatch = new Stopwatch();
        }

        internal async Task RunAsync(CancellationToken token)
        {
            NodeState nodeState = NodeState.None;

            while (!token.IsCancellationRequested)
            {
                nodeState = await GetNodeStateAsync(nodeState, token);
                await DoNodeAsync(nodeState, token);
            }
        }

        private async Task DoNodeAsync(NodeState nodeState, CancellationToken token)
        {
            switch (nodeState)
            {
                case NodeState.Connect:
                    _ = await _client.ConnectAsync(false, true, token);
                    break;

                case NodeState.Faucet:
                    if (await _client.FaucetAsync(_client.Token(1000), token))
                    {
                        WaitOnExtrinsic();
                    }
                    break;

                case NodeState.Play:
                    if (_ = await _client.SendAsync(Client.Alice, _client.Token(1), 50, token))
                    {
                        Thread.Sleep(250);
                    }
                    Thread.Sleep(250);
                    break;

                case NodeState.Finished:
                    var downTime = 10000;
                    Log.Information("Downtime for {value} sec.", (double)downTime / 1000);
                    Thread.Sleep(downTime);
                    break;

                case NodeState.Disconnect:
                    _ = await _client.DisconnectAsync();
                    break;

                default:
                    break;
            }
        }

        private async Task<NodeState> GetNodeStateAsync(NodeState nodeState, CancellationToken token)
        {
            // Connect
            if (!_client.IsConnected)
            {
                return ChangeState(nodeState, NodeState.Connect);
            }

            var accountInfo = await _client.GetAccountInfoAsync(token);

            // Faucet
            if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value < 100000000)
            {
                return ChangeState(nodeState, NodeState.Faucet);
            }

            // Finish
            if (nodeState != NodeState.Play)
            {
                return ChangeState(nodeState, NodeState.Play, $"balance:{accountInfo.Data.Free.Value}");
            } 

            return ChangeState(nodeState, NodeState.Finished);
        }

        private void WaitOnExtrinsic()
        {
            // wait on extrinsic
            var running = _client.ExtrinsicManger.Running;
            if (running.Any())
            {
                Log.Debug("Waiting on {count} extrinsic proccesed!", running.Count());
                while (running.Any())
                {
                    Thread.Sleep(1000);
                    running = _client.ExtrinsicManger.Running;
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