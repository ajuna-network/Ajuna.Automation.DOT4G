using System;
using System.Threading;
using System.Threading.Tasks;
using Ajuna.NetApi;
using Ajuna.NetApi.Model.Extrinsics;
using Ajuna.NetApi.Model.Rpc;
using Ajuna.NetApi.Model.Types;
using AjunaNET.NetApiExt.Generated;
using Schnorrkel.Keys;

namespace Ajuna.Automation.Model
{
    public abstract class Client
    {
        public static MiniSecret MiniSecretAlice => new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
        public static Account Alice => Account.Build(KeyType.Sr25519, MiniSecretAlice.ExpandToSecret().ToBytes(), MiniSecretAlice.GetPair().Public.Key);

        public Account Account { get; set; }

        public Properties Properties { internal get; set; }

        public ExtrinsicManager ExtrinsicManger { get; }

        internal readonly SubstrateClientExt client;

        public bool IsConnected => client.IsConnected;

        public string Address(bool squezzed = true) => squezzed ? $"{Account.Value.Substring(0, 5)}...{Account.Value.Substring(Account.Value.Length - 5)}" : Account.Value;

        public Client(Account account, string url)
        {
            Account = account;
            client = new SubstrateClientExt(new Uri(url),
                ChargeAssetTxPayment.Default());

            ExtrinsicManger = new ExtrinsicManager(60);
        }

        public async Task<bool> ConnectAsync(bool useMetadata, bool standardSubstrate, CancellationToken token)
        {
            if (!IsConnected)
            {
                await client.ConnectAsync(useMetadata, standardSubstrate, token);
            }

            Properties = await client.System.PropertiesAsync(token);

            return IsConnected;
        }

        public async Task<bool> DisconnectAsync()
        {
            if (!IsConnected)
            {
                return false;
            }

            await client.CloseAsync();
            return true;
        }

        public static Account RandomAccount(int seed = 0)
        {
            var random = seed > 0 ? new Random(seed) : new Random();
            var randomBytes = new byte[16];
            random.NextBytes(randomBytes);
            var mnemonic = string.Join(' ', Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
            return Mnemonic.GetAccountFromMnemonic(mnemonic, "aA1234dd", KeyType.Sr25519);
        }
    }
}