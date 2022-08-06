using Ajuna.Automation.AI;
using Ajuna.Automation.Model;
using Ajuna.NetApi;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    partial class Program
    {
        private const string NODE_URL = "ws://127.0.0.1:9944";
        private const string WORKER_URL = "ws://56b4-84-75-48-249.ngrok.io";
        private const string SHARD = "2WTKarArPH1jxUCCDMbLvmDKG9UiPZxfBrb2eQUWyU3K";
        private const string MRENCLAVE = "2WTKarArPH1jxUCCDMbLvmDKG9UiPZxfBrb2eQUWyU3K";

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
            var randomBytes = new byte[16];
            _random.NextBytes(randomBytes);

            var mnemonic = string.Join(' ', Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
            var account = Mnemonic.GetAccountFromMnemonic(mnemonic, "aA1234dd", Ajuna.NetApi.Model.Types.KeyType.Sr25519);

            var nodeClient = new NodeClient(account, NODE_URL);
            var workerClient = new WorkerClient(account, WORKER_URL, SHARD, MRENCLAVE);

            var logic = new RandomAI();

            var client = new Bot(nodeClient, workerClient, logic);

            await client.RunAsync(token);

        }


    }
}
