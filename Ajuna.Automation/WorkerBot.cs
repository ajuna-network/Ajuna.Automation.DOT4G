﻿using Ajuna.Automation.Enums;
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
    internal class WorkerBot
    {
        private readonly WorkerClient _workerClient;

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        private Balance? _workerBalance;

        public WorkerBot(WorkerClient workerClient)
        {
            _workerClient = workerClient;
            _tracker = new Dictionary<string, long[]>();
            _stopwatch = new Stopwatch();

            _workerBalance = null;
        }

        internal async Task RunAsync(CancellationToken token)
        {
            var SleepTime = 1000;

            WorkerState workerState = WorkerState.None;

            while (!token.IsCancellationRequested)
            {
                workerState = await GetWorkerStateAsync(workerState, token);
                await DoWorkerAsync(workerState, token);

                //Thread.Sleep(SleepTime);
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
                case WorkerState.None:
                    _ = await _workerClient.FaucetAsync();
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
            if (balanceWorker is null 
             || _workerBalance is null
             || (balanceWorker.Value != _workerBalance.Value 
                && workerState != WorkerState.Faucet))
            {
                _workerBalance = balanceWorker;
                return ChangeState(workerState, WorkerState.Faucet, $"balance:{balanceWorker?.Value}");
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

            Log.Information("{name} = {state1} from {state2} in {ms} sec.{msg}", typeof(T).Name, newState, oldState, (double)elapsedMs / 1000, msg);

            return newState;
        }
    }
}