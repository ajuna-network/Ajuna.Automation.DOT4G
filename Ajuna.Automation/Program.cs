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
        private const string WORKER_URL = "ws://991f-89-210-82-26.ngrok.io";
        private const string MRENCLAVE = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";
        // SHARD IS EQUAL TO MRENCLAVE 
        private const string SHARD = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";

        // Set to true for Stress Testing 
        private static bool IS_STRESS_TESTING = false;

        private const int MAX_NUMBER_OF_CONCURRENT_BOTS = 1000;

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
                    await playService.PlayAsync(Client.RandomAccount(),cts.Token);
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
                    botTasks.Add(ps.PlayAsync(Client.RandomAccount(),cts.Token));
                    
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
                Log.Information("{Text} {Number}","Number of active bots is ", tasks.Count);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }
    }
}
