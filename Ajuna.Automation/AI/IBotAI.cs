using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using AjunaNET.NetApiExt.Generated.Model.dot4gravity;
using AjunaNET.NetApiExt.Generated.Model.pallet_ajuna_board.types;

namespace Ajuna.Automation.AI
{
    public interface IBotAI
    {
        public int[] Bombs(Dot4GObj gameBoard);

        public (Side, int) Play(Dot4GObj gameBoard);
    }
}