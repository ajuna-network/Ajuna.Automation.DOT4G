using Ajuna.Automation.AI;
using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Ajuna.NetApi;
using Ajuna.NetApi.Model.Types;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    partial class Program
    {
        private const string NODE_URL = "ws://127.0.0.1:9944";
        private const string WORKER_URL = "ws://8826-89-210-82-26.ngrok.io";

        private const string MRENCLAVE = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";

        // SHARD IS EQUAL TO MRENCLAVE 
        private const string SHARD = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";

        // Set to true for Stress Testing 
        private static bool IS_STRESS_TESTING = true;
        private const int MAX_NUMBER_OF_CONCURRENT_BOTS = 25;

        private static async Task Main(string[] args)
        {
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo
                .Console()
                .CreateLogger();

            // Add this to your C# console app's Main method to give yourself
            // a CancellationToken that is canceled when the user hits Ctrl+C.
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            try
            {
                Console.WriteLine("Press Ctrl+C to end.");

                // Play as a single bot
                if (!IS_STRESS_TESTING)
                {
                    var playService = new PlayService(NODE_URL, WORKER_URL, MRENCLAVE, SHARD);
                    await playService.PlayAsync(Client.RandomAccount(), cts.Token);
                    return;
                }

                // Fire up different bots for stress testing
                var botTasks = new List<Task>();

                // Show number of active bots every 20 seconds 
                ShowNumberOfActiveBotsAsync(botTasks);

                // Number of bots to fire up at once before taking a short break
                var batchNumber = 12;
                var currentBatchNumber = 0;


                for (int i = 0; i < MAX_NUMBER_OF_CONCURRENT_BOTS; i++)
                {
                    var ps = new PlayService(NODE_URL, WORKER_URL, MRENCLAVE, SHARD);
                    botTasks.Add(ps.PlayAsync(Client.RandomAccount(), cts.Token));

                    await Task.Delay(TimeSpan.FromMilliseconds(300));

                    if (batchNumber == currentBatchNumber)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(15));
                        currentBatchNumber = 0;
                    }

                    currentBatchNumber++;
                    Console.WriteLine(i);
                }

                Task.WaitAll(botTasks.ToArray());
            }
            catch (OperationCanceledException)
            {
                // This is the normal way we close.
            }

            // Finally, once just before the application exits...
            Log.CloseAndFlush();
        }

        private static async Task ShowNumberOfActiveBotsAsync(List<Task> tasks)
        {
            while (true)
            {
                Log.Information("{Text} {Number}", "Number of active bots is ", tasks.Count);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }

        #region More Testing Options
        private static async Task MainAsync(CancellationToken token)
        {
            ModeType modeType = ModeType.Play;

            Account account = Client.RandomAccount();
            Log.Information("My Account is {address}", account.Value);

            switch (modeType)
            {
                case ModeType.RandomAccounts:
                    await NodeRandomAccountsAsync(account, token);
                    break;

                case ModeType.BalanceOnNode:
                    await BalanceNodeStressAsync(account, token);
                    break;

                case ModeType.BalanceOnWorker:
                    await BalanceWorkerStressAsync(account, token);
                    break;

                case ModeType.Play:
                    await PlayAsync(account, token);
                    break;

                case ModeType.WorkerTest:
                    await WorkerTestPlayAsync(account, token);
                    break;
            }
        }

        private static async Task NodeRandomAccountsAsync(Account account, CancellationToken token)
        {
            var nodeClient = new NodeClient(account, NODE_URL);

            var client = new AccountBot(nodeClient);
            await client.RunAsync(token);
        }

        private static async Task BalanceNodeStressAsync(Account account, CancellationToken token)
        {
            var nodeClient = new NodeClient(account, NODE_URL);

            var client = new BalanceNodeBot(nodeClient);
            await client.RunAsync(token);
        }

        private static async Task BalanceWorkerStressAsync(Account account, CancellationToken token)
        {
            var workerClient = new WorkerClient(account, WORKER_URL, SHARD, MRENCLAVE);

            var client = new BalanceWorkerBot(workerClient);
            await client.RunAsync(token);
        }

        private static async Task WorkerTestPlayAsync(Account account, CancellationToken token)
        {
            Log.Information("Connecting to: {0}", WORKER_URL);

            var workerClient = new WorkerClient(account, WORKER_URL, SHARD, MRENCLAVE);

            _ = await workerClient.ConnectAsync(false, false, token);

            Log.Information("Is worker connected? {0}", workerClient.IsConnected);

            _ = await workerClient.client.RPCMethodsAsync();

            _ = await workerClient.DisconnectAsync();
        }

        private static async Task PlayAsync(Account account, CancellationToken token)
        {
            var nodeClient = new NodeClient(account, NODE_URL);
            var workerClient = new WorkerClient(account, WORKER_URL, SHARD, MRENCLAVE);

            var logic = new StraightAI();

            var client = new PlayBot(nodeClient, workerClient, logic);
            await client.RunAsync(token);
        }
        
        #endregion
    }
}