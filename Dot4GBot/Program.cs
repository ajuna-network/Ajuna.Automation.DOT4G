using Ajuna.NetApi;
using Ajuna.UnityInterface;
using Dot4GBot.AI;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dot4GBot
{
    partial class Program
    {
        private static string _nodeUrl = "ws://127.0.0.1:9944";
        private static string _ngrokUrl = "ws://a5bc-84-75-48-249.ngrok.io";
        private static string _mrenclave = "2WTKarArPH1jxUCCDMbLvmDKG9UiPZxfBrb2eQUWyU3K";

        private static Random _random = new Random();

        private static async Task Main(string[] args)
        {
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo
                .Console()
                .CreateLogger();

            //Console.Write("\r\nNODE URL[" + _nodeUrl + "]=: ");
            //var nodeUrl = Console.ReadLine();
            //if (nodeUrl.Count() > 0)
            //{
            //    _nodeUrl = nodeUrl;
            //}
            //Console.WriteLine($"=> '{_nodeUrl}'");

            //Console.Write("\r\nNGROK URL[" + _ngrokUrl + "]=: ");
            //var ngrokUrl = Console.ReadLine();
            //if (ngrokUrl.Count() > 0)
            //{
            //    _ngrokUrl = ngrokUrl;
            //}
            //Console.WriteLine($"=> '{_ngrokUrl}'");

            //Console.Write("\r\nMRENCLAVE[" + _mrenclave + "]=: ");
            //var mrenclave = Console.ReadLine();
            //if (mrenclave.Count() > 0)
            //{
            //    _mrenclave = mrenclave;
            //}
            //Console.WriteLine($"=> '{_mrenclave}'");

            //Console.WriteLine($"Let's play!");
            //Thread.Sleep(1000);

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

            var dot4gClient = new Dot4GClient(account,
                _nodeUrl,
                _ngrokUrl,
                _mrenclave,
                _mrenclave);

            IBotAI logic = new RandomAI();

            var bot = new D4GBot(dot4gClient, logic, DisplayType.UI);
            await bot.RunAsync(token);

            foreach(var track in bot.Tracker)
            {
                Console.WriteLine($"track {track.Key} = {track.Value[0]} @ {track.Value[1]/1000}s => avg. {track.Value[1]/track.Value[0]}ms");
            }
        }


    }
}
