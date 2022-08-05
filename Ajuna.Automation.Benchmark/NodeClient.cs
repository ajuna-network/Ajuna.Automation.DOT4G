using Ajuna.NetApi;
using Ajuna.NetApi.Model.AjunaCommon;
using Ajuna.NetApi.Model.Extrinsics;
using Ajuna.NetApi.Model.FrameSystem;
using Ajuna.NetApi.Model.PalletBalances;
using Ajuna.NetApi.Model.PalletGameRegistry;
using Ajuna.NetApi.Model.SpCore;
using Ajuna.NetApi.Model.SpRuntime;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using Schnorrkel.Keys;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ajuna.Automation
{
    public class NodeClient : Client
    {
        public NodeClient(Account account, string url) : base(account, url) { }

        public async Task<AccountInfo?> GetBalanceNodeAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            var account32 = new AccountId32();
            account32.Create(Utils.GetPublicKeyFrom(Account.Value));

            return await client.SystemStorage.Account(account32, token);
        }

        public async Task<bool> FaucetAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return false;
            }

            var accountAlice = new AccountId32();
            accountAlice.Create(Utils.GetPublicKeyFrom(Alice.Value));

            var account32 = new AccountId32();
            account32.Create(Utils.GetPublicKeyFrom(Account.Value));

            var multiAddress = new EnumMultiAddress();
            multiAddress.Create(MultiAddress.Id, account32);

            var amount = new BaseCom<U128>();
            amount.Create(1100000000000);

            var extrinsic = BalancesCalls.Transfer(multiAddress, amount);
            var charge = new ChargeAssetTxPayment(0, 0);

            var subscription = await client.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsic, Alice, charge, 64, token);
            if (subscription == null)
            {
                return false;
            }

            ExtrinsicManger.Add(subscription, "Faucet");
            return true;
        }

        public async Task<bool> QueueAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any()) 
            { 
                return false; 
            }

            var extrinsic = GameRegistryCalls.Queue();
            var charge = new ChargeAssetTxPayment(0, 0);

            var subscription = await client.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsic, Account, charge, 64, token);
            if (subscription == null)
            {
                return false;
            }

            ExtrinsicManger.Add(subscription, "Queue");
            return true;
        }

        public async Task<U8?> GetPlayerQueueAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            var account32 = new AccountId32();
            account32.Create(Utils.GetPublicKeyFrom(Account.Value));

            return await client.MatchmakerStorage.PlayerQueue(account32, token);
        }

        public async Task<U32?> GetRunnerIdAsync(CancellationToken token)
        {

            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            var account = new AccountId32();
            account.Create(Utils.GetPublicKeyFrom(Account.Value));

            return await client.GameRegistryStorage.Players(account, token);
        }

        public async Task<EnumRunnerState?> GetRunnerStateAsync(U32 registerId, CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            return await client.RunnerStorage.Runners(registerId, token);
        }

    }
}
