using Ajuna.NetApi.Model.AjunaWorker;
using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApi.Model.Types;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Ajuna.Automation.Model
{
    public class WorkerClient : Client
    {
        private RSAParameters _shieldingKey;

        private string _shardHex;

        private string _mrenclaveHex;

        public bool HasShieldingKey => _shieldingKey.Modulus != null;

        public WorkerClient(Account account, string url, string shardHex, string mrenclaveHex) : base(account, url)
        {
            _shieldingKey = new RSAParameters();
            _shardHex = shardHex;
            _mrenclaveHex = mrenclaveHex;
        }

        public async Task<bool> GetShieldingKeyAsync()
        {
            if (!IsConnected)
            {
                return false;
            }

            _shieldingKey = await client.ShieldingKeyAsync();

            return HasShieldingKey;
        }

        public async Task<bool> FaucetAsync()
        {
            if (!IsConnected || !HasShieldingKey)
            {
                return false;
            }

            var hash = await client.BalanceTransferAsync(Alice, Account, 100000, _shieldingKey, _shardHex, _mrenclaveHex);
            if (hash == null)
            {
                return false;
            }

            return true;
        }

        public async Task<Balance?> GetBalanceAsync()
        {
            if (!IsConnected || !HasShieldingKey)
            {
                return null;
            }

            return await client.GetFreeBalanceAsync(Account, _shieldingKey, _shardHex);
        }

        public async Task<Dot4GObj?> GetGameBoardAsync()
        {
            if (!IsConnected || !HasShieldingKey)
            {
                return null;
            }

            var boardGame = await client.GetBoardGameAsync(Account, _shieldingKey, _shardHex);
            if (boardGame == null)
            {
                return null;
            }

            return new Dot4GObj(boardGame);
        }

        public async Task<bool> BombAsync(int posX, int posY)
        {
            if (!IsConnected || !HasShieldingKey)
            {
                return false;
            }

            var turn = SgxGameTurn.DropBomb(new int[] { posX, posY });
            var hash = await client.PlayTurnAsync(Account, turn, _shieldingKey, _shardHex, _mrenclaveHex);
            if (hash == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> StoneAsync(Side side, int column)
        {
            if (!IsConnected || !HasShieldingKey)
            {
                return false;
            }

            var turn = SgxGameTurn.DropStone(side, (byte)column);
            var hash = await client.PlayTurnAsync(Account, turn, _shieldingKey, _shardHex, _mrenclaveHex);
            if (hash == null)
            {
                return false;
            }

            return true;
        }
    }
}
