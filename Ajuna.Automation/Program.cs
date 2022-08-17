using Ajuna.Automation.AI;
using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Ajuna.NetApi;
using Ajuna.NetApi.Model.Types;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    partial class Program
    {
        private const string NODE_URL = "ws://127.0.0.1:9944";
        private const string WORKER_URL = "ws://2d124076029f.ngrok.io";
        private const string SHARD = "2nwiSmLC2aqdZhBt2aAKPSL2kqCkcv9df4UthNixMU64";
        private const string MRENCLAVE = "2nwiSmLC2aqdZhBt2aAKPSL2kqCkcv9df4UthNixMU64";

        private static Random _random = new Random();

        private static async Task Main(string[] args)
        {
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
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
            ModeType modeType = ModeType.Balance;

            Account account = RandomAccount();

            Log.Information("My Account is {address}", account.Value);

            switch (modeType)
            {
                case ModeType.Balance:
                    await BalanceStressAsync(account, token);
                    break;

                case ModeType.Play:
                    await PlayAsync(account, token);
                    break;
            }
        }

        private static Account RandomAccount()
        {
            var randomBytes = new byte[16];
            _random.NextBytes(randomBytes);
            var mnemonic = string.Join(' ', Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
            return Mnemonic.GetAccountFromMnemonic(mnemonic, "aA1234dd", KeyType.Sr25519);
        }

        private static async Task BalanceStressAsync(Account account, CancellationToken token)
        {
            var workerClient = new WorkerClient(account, WORKER_URL, SHARD, MRENCLAVE);

            var client = new WorkerBot(workerClient);
            await client.RunAsync(token);
        }

        private static async Task PlayAsync(Account account, CancellationToken token)
        {
            var nodeClient = new NodeClient(account, NODE_URL);
            var workerClient = new WorkerClient(account, WORKER_URL, SHARD, MRENCLAVE);

            var logic = new StraightAI();

            var client = new PlayBot(nodeClient, workerClient, logic);
            await client.RunAsync(token);
        }
    }
}
