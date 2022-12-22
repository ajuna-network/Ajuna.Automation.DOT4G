using Ajuna.Automation.AI;
using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Ajuna.NetApi.Model.Types;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    public class Program
    {
        private const string NODE_URL = "ws://127.0.0.1:9944";

        private static Random _random = new Random();

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
                await MainAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // This is the normal way we close.
            }

            // Finally, once just before the application exits...
            Log.CloseAndFlush();
        }

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

                case ModeType.Play:
                    await PlayAsync(account, token);
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


        private static async Task PlayAsync(Account account, CancellationToken token)
        {
            var nodeClient = new NodeClient(account, NODE_URL);

            var client = new PlayBot(nodeClient, new StraightAI());

            await client.RunAsync(token);
        }
    }
}
