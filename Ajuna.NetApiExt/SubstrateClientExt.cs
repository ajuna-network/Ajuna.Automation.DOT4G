//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Ajuna.NetApi.Model.AjunaWorker;
using Ajuna.NetApi.Model.Meta;
using Ajuna.NetApi.Model.PalletBoard;
using Ajuna.NetApi.Model.PalletConnectfour;
using Ajuna.NetApi.Model.PrimitiveTypes;
using Ajuna.NetApi.Model.SpCore;
using Ajuna.NetApi.Model.SpRuntime;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using Ajuna.NetApiExt.Model.AjunaWorker.Helper;
using Newtonsoft.Json;
using Serilog;
using Org.BouncyCastle.Security;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Ajuna.NetApi
{


    public sealed class SubstrateClientExt : Ajuna.NetApi.SubstrateClient
    {
        /// <summary>
        /// StorageKeyDict for key definition informations.
        /// </summary>
        public System.Collections.Generic.Dictionary<System.Tuple<string, string>, System.Tuple<Ajuna.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>> StorageKeyDict;

        /// <summary>
        /// SystemStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.FrameSystem.SystemStorage SystemStorage;

        /// <summary>
        /// TimestampStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletTimestamp.TimestampStorage TimestampStorage;

        /// <summary>
        /// AuraStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletAura.AuraStorage AuraStorage;

        /// <summary>
        /// GrandpaStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletGrandpa.GrandpaStorage GrandpaStorage;

        /// <summary>
        /// BalancesStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletBalances.BalancesStorage BalancesStorage;

        /// <summary>
        /// TransactionPaymentStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletTransactionPayment.TransactionPaymentStorage TransactionPaymentStorage;

        /// <summary>
        /// AssetsStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletAssets.AssetsStorage AssetsStorage;

        /// <summary>
        /// VestingStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletVesting.VestingStorage VestingStorage;

        /// <summary>
        /// CouncilStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletCouncil.CouncilStorage CouncilStorage;

        /// <summary>
        /// CouncilMembershipStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletCouncilMembership.CouncilMembershipStorage CouncilMembershipStorage;

        /// <summary>
        /// TreasuryStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletTreasury.TreasuryStorage TreasuryStorage;

        /// <summary>
        /// DemocracyStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletDemocracy.DemocracyStorage DemocracyStorage;

        /// <summary>
        /// SudoStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletSudo.SudoStorage SudoStorage;

        /// <summary>
        /// SchedulerStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletScheduler.SchedulerStorage SchedulerStorage;

        /// <summary>
        /// MatchmakerStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletMatchmaker.MatchmakerStorage MatchmakerStorage;

        /// <summary>
        /// RunnerStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletRunner.RunnerStorage RunnerStorage;

        /// <summary>
        /// GameRegistryStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletGameRegistry.GameRegistryStorage GameRegistryStorage;

        /// <summary>
        /// ObserversStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletObservers.ObserversStorage ObserversStorage;

        /// <summary>
        /// TeerexStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletTeerex.TeerexStorage TeerexStorage;

        /// <summary>
        /// SidechainStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletSidechain.SidechainStorage SidechainStorage;

        /// <summary>
        /// BoardStorage storage calls.
        /// </summary>
        public Ajuna.NetApi.Model.PalletBoard.BoardStorage BoardStorage;

        public SubstrateClientExt(System.Uri uri) :
                base(uri)
        {
            StorageKeyDict = new System.Collections.Generic.Dictionary<System.Tuple<string, string>, System.Tuple<Ajuna.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>>();
            this.SystemStorage = new Ajuna.NetApi.Model.FrameSystem.SystemStorage(this);
            this.TimestampStorage = new Ajuna.NetApi.Model.PalletTimestamp.TimestampStorage(this);
            this.AuraStorage = new Ajuna.NetApi.Model.PalletAura.AuraStorage(this);
            this.GrandpaStorage = new Ajuna.NetApi.Model.PalletGrandpa.GrandpaStorage(this);
            this.BalancesStorage = new Ajuna.NetApi.Model.PalletBalances.BalancesStorage(this);
            this.TransactionPaymentStorage = new Ajuna.NetApi.Model.PalletTransactionPayment.TransactionPaymentStorage(this);
            this.AssetsStorage = new Ajuna.NetApi.Model.PalletAssets.AssetsStorage(this);
            this.VestingStorage = new Ajuna.NetApi.Model.PalletVesting.VestingStorage(this);
            this.CouncilStorage = new Ajuna.NetApi.Model.PalletCouncil.CouncilStorage(this);
            this.CouncilMembershipStorage = new Ajuna.NetApi.Model.PalletCouncilMembership.CouncilMembershipStorage(this);
            this.TreasuryStorage = new Ajuna.NetApi.Model.PalletTreasury.TreasuryStorage(this);
            this.DemocracyStorage = new Ajuna.NetApi.Model.PalletDemocracy.DemocracyStorage(this);
            this.SudoStorage = new Ajuna.NetApi.Model.PalletSudo.SudoStorage(this);
            this.SchedulerStorage = new Ajuna.NetApi.Model.PalletScheduler.SchedulerStorage(this);
            this.MatchmakerStorage = new Ajuna.NetApi.Model.PalletMatchmaker.MatchmakerStorage(this);
            this.RunnerStorage = new Ajuna.NetApi.Model.PalletRunner.RunnerStorage(this);
            this.GameRegistryStorage = new Ajuna.NetApi.Model.PalletGameRegistry.GameRegistryStorage(this);
            this.ObserversStorage = new Ajuna.NetApi.Model.PalletObservers.ObserversStorage(this);
            this.TeerexStorage = new Ajuna.NetApi.Model.PalletTeerex.TeerexStorage(this);
            this.SidechainStorage = new Ajuna.NetApi.Model.PalletSidechain.SidechainStorage(this);
            this.BoardStorage = new Ajuna.NetApi.Model.PalletBoard.BoardStorage(this);
        }

        public enum Wrapped
        {
            Nonce,
            Balance,
            Board,
            Hash
        }

        public bool Unwrap<T>(Wrapped wrapped, RpcReturnValue returnValue, out T result) where T : IType, new()
        {
            result = new T();

            switch (returnValue.DirectRequestStatus.Value)
            {
                case DirectRequestStatus.Ok:
                    break;

                case DirectRequestStatus.TrustedOperationStatus:

                    var valueBytes = returnValue.Value.Value.Select(p => p.Value).ToArray();

                    switch (wrapped)
                    {
                        case Wrapped.Nonce:
                            var nonceWrapped = new BaseOpt<BaseVec<U8>>();
                            nonceWrapped.Create(valueBytes);
                            if (nonceWrapped.OptionFlag)
                            {
                                var bytes = nonceWrapped.Value.Value.Select(p => p.Value).ToArray();
                                result.Create(bytes);
                                return true;
                            }
                            break;

                        case Wrapped.Balance:
                            var balanceWrapped = new BaseOpt<BaseVec<U8>>();
                            balanceWrapped.Create(valueBytes);
                            if (balanceWrapped.OptionFlag)
                            {
                                var bytes = balanceWrapped.Value.Value.Select(p => p.Value).ToArray();
                                result.Create(bytes);
                                return true;
                            }
                            break;

                        case Wrapped.Hash:
                            result.Create(valueBytes);
                            return true;

                        case Wrapped.Board:
                            var boardWrapped = new BaseOpt<BaseVec<U8>>();
                            boardWrapped.Create(valueBytes);
                            if (boardWrapped.OptionFlag)
                            {
                                var bytes = boardWrapped.Value.Value.Select(p => p.Value).ToArray();
                                result.Create(bytes);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                    }


                    break;

                case DirectRequestStatus.Error:
                    var byteArray = returnValue.Value.Bytes;
                    var converter = new UTF8Encoding();
                    var str = converter.GetString(UnwrapBytes(byteArray));
                    Log.Error("DirectRequestStatus {value}: {string}", returnValue.DirectRequestStatus.Value, str);
                    break;

            }

            return false;
        }

        public async Task<string> BalanceTransferAsync(Account fromAccount, Account toAccount, uint amount, RSAParameters shieldingKey, string shardHex, string mrenclaveHex)
        {
            EnumTrustedOperation tOpNonce = Wrapper.CreateGetter(fromAccount, TrustedGetter.Nonce);
            var nonceValue = await ExecuteTrustedOperationAsync(tOpNonce, shieldingKey, shardHex);
            if (Unwrap(Wrapped.Nonce, nonceValue, out U32 nonce))
            {
                Log.Information("Account[{value}]({nonce}) transfers {amount} to Account[{value}]", fromAccount.Value, nonce.Value, amount, toAccount.Value);
                EnumTrustedOperation tOpTransfer = Wrapper.CreateCallBalanceTransfer(fromAccount, toAccount, amount, nonce.Value, mrenclaveHex, shardHex);
                var returnValue = await ExecuteTrustedOperationAsync(tOpTransfer, shieldingKey, shardHex);
                if (Unwrap(Wrapped.Hash, returnValue, out H256 value))
                {
                    //Logger.Info($"Hash is {Utils.Bytes2HexString(value.Value.Bytes)}");
                    return Utils.Bytes2HexString(value.Value.Bytes);
                }
            }

            return null;
        }

        public async Task<string> PlayTurnAsync(Account account, SgxGameTurn turn, RSAParameters shieldingKey, string shardHex, string mrenclaveHex)
        {
            EnumTrustedOperation tOpNonce = Wrapper.CreateGetter(account, TrustedGetter.Nonce);
            var nonceValue = await ExecuteTrustedOperationAsync(tOpNonce, shieldingKey, shardHex);
            if (Unwrap(Wrapped.Nonce, nonceValue, out U32 nonce))
            {
                Log.Information("Account[{value}]({nonce}) play {name}", account.Value, nonce.Value, turn.GetType().Name);
                EnumTrustedOperation tOpPlayTurn = Wrapper.CreateCallPlayTurn(account, turn, nonce.Value, mrenclaveHex, shardHex);
                var returnValue = await ExecuteTrustedOperationAsync(tOpPlayTurn, shieldingKey, shardHex);
                if (Unwrap(Wrapped.Hash, returnValue, out H256 value))
                {
                    //Logger.Info($"Hash is {Utils.Bytes2HexString(value.Value.Bytes)}");
                    return Utils.Bytes2HexString(value.Value.Bytes);
                }
            }

            return null;
        }

        public async Task<U32> GetNonce(Account account, RSAParameters shieldingKey, string shardHex)
        {
            EnumTrustedOperation tOpNonce = Wrapper.CreateGetter(account, TrustedGetter.Nonce);
            var nonceValue = await ExecuteTrustedOperationAsync(tOpNonce, shieldingKey, shardHex);
            if (Unwrap(Wrapped.Nonce, nonceValue, out U32 nonce))
            {
                return nonce;
            }
            return null;
        }

        public async Task<BoardGame> GetBoardGameAsync(Account account, RSAParameters shieldingKey, string shardHex)
        {
            EnumTrustedOperation tOpBoard = Wrapper.CreateGetter(account, TrustedGetter.Board);
            var boardValue = await ExecuteTrustedOperationAsync(tOpBoard, shieldingKey, shardHex);
            if (Unwrap(Wrapped.Board, boardValue, out BoardGame boardGame))
            {
                return boardGame;
            }

            return null;
        }

        public async Task<Balance> GetFreeBalanceAsync(Account account, RSAParameters shieldingKey, string shardHex)
        {
            EnumTrustedOperation tOpPreBalance = Wrapper.CreateGetter(account, TrustedGetter.FreeBalance);
            var balanceValuePre = await ExecuteTrustedOperationAsync(tOpPreBalance, shieldingKey, shardHex);
            if (Unwrap(Wrapped.Balance, balanceValuePre, out Balance balance))
            {
                return balance;
            }
            return null;
        }

        public async Task<RpcReturnValue> ExecuteTrustedOperationAsync(EnumTrustedOperation trustedOperation, RSAParameters shieldingKey, string shardHex)
        {
            var cypherText = Wrapper.SignTrustedOperation(shieldingKey, trustedOperation);

            // - ShardIdentifier
            var shardId = new H256();
            shardId.Create(Base58.Bitcoin.Decode(shardHex).ToArray());

            Request request = new Request
            {
                Shard = shardId,
                CypherText = Wrapper.VecU8FromBytes(cypherText)
            };

            var parameters = Utils.Bytes2HexString(request.Encode());

            var result = await InvokeAsync<string>("author_submitAndWatchExtrinsic", new object[] { parameters }, CancellationToken.None);

            var returnValue = new RpcReturnValue();
            returnValue.Create(result);

            return returnValue;
        }


        public byte[] UnwrapBytes(byte[] byteArray)
        {
            var baseVec1 = new BaseVec<U8>();
            baseVec1.Create(byteArray);

            var bytes1 = new List<byte>();
            foreach (var by in baseVec1.Value)
            {
                bytes1.Add(by.Value);
            }
            var baseVec2 = new BaseVec<U8>();
            baseVec2.Create(bytes1.ToArray());

            var bytes2 = new List<byte>();
            foreach (var by in baseVec2.Value)
            {
                bytes2.Add(by.Value);
            }
            return bytes2.ToArray();
        }

        public async Task<RSAParameters> ShieldingKeyAsync()
        {
            var result = await InvokeAsync<string>("author_getShieldingKey", null, CancellationToken.None);

            var rpcReturnValue = new RpcReturnValue();
            rpcReturnValue.Create(result);

            // - Create RSAPubKey from ShieldingKey
            var rsaParameters = new RSAParameters();

            if (rpcReturnValue.DirectRequestStatus.Value == DirectRequestStatus.Ok)
            {
                var rsaPubKeyStr = new UTF8Encoding().GetString(UnwrapBytes(rpcReturnValue.Value.Bytes));
                RSAPubKey rsaPubKey = JsonConvert.DeserializeObject<RSAPubKey>(rsaPubKeyStr);

                rsaParameters.Modulus = rsaPubKey.N.ToArray();
                rsaParameters.Exponent = rsaPubKey.E.ToArray();
                Array.Reverse(rsaParameters.Modulus, 0, rsaParameters.Modulus.Length);
                Array.Reverse(rsaParameters.Exponent, 0, rsaParameters.Exponent.Length);
                return rsaParameters;
            }

            return rsaParameters;
        }

        public async Task<byte[]> RPCMethodsAsync()
        {
            var result = await InvokeAsync<string>("rpc_methods", null, CancellationToken.None);
            Console.WriteLine($"-----------> {result}");

            //var rpcReturnValue = new RpcReturnValue();
            //rpcReturnValue.Create(result);

            return null;
        }
    }
}