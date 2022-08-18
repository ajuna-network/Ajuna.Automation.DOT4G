using Ajuna.NetApi;
using Ajuna.NetApi.Model.AjunaCommon;
using Ajuna.NetApi.Model.Extrinsics;
using Ajuna.NetApi.Model.FrameSystem;
using Ajuna.NetApi.Model.PalletBalances;
using Ajuna.NetApi.Model.PalletGameRegistry;
using Ajuna.NetApi.Model.Rpc;
using Ajuna.NetApi.Model.SpCore;
using Ajuna.NetApi.Model.SpRuntime;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using Schnorrkel.Keys;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation.Model
{
    public abstract class Client
    {
        public static MiniSecret MiniSecretAlice => new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
        public static Account Alice => Account.Build(KeyType.Sr25519, MiniSecretAlice.ExpandToSecret().ToBytes(), MiniSecretAlice.GetPair().Public.Key);

        public Account Account { get; set; }

        public ExtrinsicManager ExtrinsicManger { get; }

        internal readonly SubstrateClientExt client;

        public bool IsConnected => client.IsConnected;

        public Client(Account account, string url)
        {
            Account = account;
            client = new SubstrateClientExt(new Uri(url));

            ExtrinsicManger = new ExtrinsicManager(60);
        }

        public async Task<bool> ConnectAsync(bool useMetadata, bool standardSubstrate, CancellationToken token)
        {
            if (!IsConnected)
            {
                client.RPCDelayed = false;
                await client.ConnectAsync(useMetadata, standardSubstrate, token);
            }

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

        public static Account RandomAccount()
        {
            Random random = new Random();
            var randomBytes = new byte[16];
            random.NextBytes(randomBytes);
            var mnemonic = string.Join(' ', Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
            return Mnemonic.GetAccountFromMnemonic(mnemonic, "aA1234dd", KeyType.Sr25519);
        }
    }
}
