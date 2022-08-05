using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;

namespace Dot4GBot.AI
{
    public interface IBotAI
    {
        public int[] Bombs(Dot4GObj gameBoard);

        public (Side, int) Play(Dot4GObj gameBoard);
    }
}