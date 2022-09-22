using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Ajuna.NetApi.Model.AjunaWorker;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    internal class BalanceWorkerBot
    {
        private readonly WorkerClient _workerClient;

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        public BalanceWorkerBot(WorkerClient workerClient)
        {
            _workerClient = workerClient;
            _tracker = new Dictionary<string, long[]>();
            _stopwatch = new Stopwatch();
        }

        internal async Task RunAsync(CancellationToken token)
        {
            WorkerState workerState = WorkerState.None;

            while (!token.IsCancellationRequested)
            {
                workerState = await GetWorkerStateAsync(workerState, token);
                await DoWorkerAsync(workerState, token);
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

                case WorkerState.Disconnect:
                    _ = await _workerClient.DisconnectAsync();
                    break;

                case WorkerState.Faucet:
                    _ = await _workerClient.FaucetAsync();
                    break;

                case WorkerState.Game:
                case WorkerState.None:
                    _ = await _workerClient.SendAsync(Client.Alice, 100);
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
            if (balanceWorker is null || balanceWorker.Value < 1000)
            {
                return ChangeState(workerState, WorkerState.Faucet, $"balance:{balanceWorker?.Value}");
            }

            // Game
            if (workerState == WorkerState.None)
            {
                return ChangeState(workerState, WorkerState.Game, $"balance:{balanceWorker?.Value}");
            }

            return ChangeState(workerState, WorkerState.None, $"balance:{balanceWorker?.Value}");
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

            Log.Debug("{name} = {state1} from {state2} in {ms} sec.{msg}", typeof(T).Name, newState, oldState, (double)elapsedMs / 1000, msg);

            return newState;
        }
    }
}