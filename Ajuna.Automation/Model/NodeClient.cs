using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Ajuna.NetApi;
using Ajuna.NetApi.Model.Extrinsics;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using AjunaNET.NetApiExt.Generated.Model.dot4gravity;
using AjunaNET.NetApiExt.Generated.Model.frame_system;
using AjunaNET.NetApiExt.Generated.Model.pallet_ajuna_board.dot4gravity;
using AjunaNET.NetApiExt.Generated.Model.pallet_ajuna_board.types;
using AjunaNET.NetApiExt.Generated.Model.sp_core.crypto;
using AjunaNET.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using AjunaNET.NetApiExt.Generated.Storage;
using AjunaNET.NetApiExt.Generated.Types.Base;
using Newtonsoft.Json.Linq;

namespace Ajuna.Automation.Model
{
    public class NodeClient : Client
    {
        public BigInteger Token(uint amount) =>new(amount * Math.Pow(10, Properties.TokenDecimals));

        public NodeClient(Account account, string url) : base(account, url)
        {
        }

        public async Task<AccountInfo?> GetAccountInfoAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                return null;
            }

            var account32 = new AccountId32();
            account32.Create(Utils.GetPublicKeyFrom(Account.Value));

            var answer = await client.SystemStorage.Account(account32, token);
            return answer.Bytes != null ? answer : null;
        }

        public async Task<bool> FaucetAsync(BigInteger amount,CancellationToken token)
        {
            return await SendAsync(Alice, Account, amount, 1, token);
        }

        public async Task<bool> SendAsync(Account to, BigInteger amount, int maxRunning, CancellationToken token)
        {
            return await SendAsync(Alice, to, amount, maxRunning, token);
        }

        public async Task<bool> SendAsync(Account from, Account to, BigInteger amount, int maxRunning, CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Count() >= maxRunning)
            {
                return false;
            }

            var account = new AccountId32();
            account.Create(Utils.GetPublicKeyFrom(to.Value));

            var multiAddress = new EnumMultiAddress();
            multiAddress.Create(MultiAddress.Id, account);

            var balance = new BaseCom<U128>();
            balance.Create(amount);

            var extrinsic = BalancesCalls.Transfer(multiAddress, balance);
            var charge = new ChargeAssetTxPayment(0, 0);

            var subscription = await client.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsic, from, charge, 64, token);
            if (subscription == null)
            {
                return false;
            }

            ExtrinsicManger.Add(subscription, "Send");
            return true;
        }

        public async Task<U32?> GetNextBoardIdAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            var answer = await client.AjunaBoardStorage.NextBoardId(token);
            return answer.Bytes != null ? answer : null;
        }

        public async Task<BoardGame?> GetBoardGamesAsync(U32 boardGameId, CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            var answer = await client.AjunaBoardStorage.BoardGames(boardGameId, token);
            return answer.Bytes != null ? answer : null;
        }

        public async Task<U32?> GetPlayerBoardsAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return null;
            }

            var account32 = new AccountId32();
            account32.Create(Utils.GetPublicKeyFrom(Account.Value));

            var answer = await client.AjunaBoardStorage.PlayerBoards(account32, token);
            return answer.Bytes != null ? answer : null;
        }


        public async Task<bool> QueueAsync(CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return false;
            }

            var extrinsic = AjunaBoardCalls.Queue();
            var charge = new ChargeAssetTxPayment(0, 0);

            var subscription = await client.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsic, Account, charge, 64, token);
            if (subscription == null)
            {
                return false;
            }

            ExtrinsicManger.Add(subscription, "Queue");
            return true;
        }

        public async Task<bool> BombAsync(U8 col, U8 row, CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return false;
            }

            var enumTurn = new EnumTurn();
            var bombCoords = new Coordinates();
            bombCoords.Col = col;
            bombCoords.Row = row;
            enumTurn.Create(Turn.DropBomb, bombCoords);

            var extrinsic = AjunaBoardCalls.Play(enumTurn);

            var subscription = await client.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsic, Account, ChargeAssetTxPayment.Default(), 64, token);
            if (subscription == null)
            {
                return false;
            }

            ExtrinsicManger.Add(subscription, "Bomb");
            return true;
        }

        public async Task<bool> StoneAsync(Side side, U8 column, CancellationToken token)
        {
            if (!IsConnected || ExtrinsicManger.Running.Any())
            {
                return false;
            }

            var enumTurn = new EnumTurn();
            var stoneCoords = new BaseTuple<EnumSide, U8>();
            var enumSide = new EnumSide();
            enumSide.Create(side);
            stoneCoords.Create(enumSide, column);
            enumTurn.Create(Turn.DropStone, stoneCoords);

            var extrinsic = AjunaBoardCalls.Play(enumTurn);

            var subscription = await client.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsic, Account, ChargeAssetTxPayment.Default(), 64, token);
            if (subscription == null)
            {
                return false;
            }

            ExtrinsicManger.Add(subscription, "Stone");
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

            var answer = await client.AjunaMatchmakerStorage.PlayerQueue(account32, token);
            return answer.Bytes != null ? answer : null;
        }
    }
}