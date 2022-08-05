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

namespace Ajuna.Automation
{
    public abstract class Client
    {
        public MiniSecret MiniSecretAlice => new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
        public Account Alice => Account.Build(KeyType.Sr25519, MiniSecretAlice.ExpandToSecret().ToBytes(), MiniSecretAlice.GetPair().Public.Key);

        public Account Account { get; }

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

    }
}
