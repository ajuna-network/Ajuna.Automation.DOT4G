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
        private const string SHARD = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";
        private const string MRENCLAVE = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";

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
                var ps1 = new PlayService(NODE_URL, WORKER_URL, MRENCLAVE, SHARD);
                await ps1.PlayAsync(Client.RandomAccount(),cts.Token);
                return;
                
                Console.WriteLine("Press Ctrl+C to end.");
                
                var botTasks = new List<Task>();
                ShowNumberOfActiveBots(botTasks);
                
                var batchNumber = 12;
                var currentBatchNumber = 0;
                
                
                for (int i = 0; i < 350; i++)
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
                
                 
                // await MainAsync(cts.Token);
                // return;
            }
            catch (OperationCanceledException)
            {
                // This is the normal way we close.
            }

            // Finally, once just before the application exits...
            Log.CloseAndFlush();
        }

        private static async Task ShowNumberOfActiveBots(List<Task> tasks)
        {
            while (true)
            {
                Log.Information("{text} {number}","Number of active bots is ", tasks.Count);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }
    }
}
