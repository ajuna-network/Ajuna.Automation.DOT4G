using System;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using AjunaNET.NetApiExt.Generated.Model.dot4gravity;

namespace Ajuna.Automation.AI
{
    public class RandomAI : IBotAI
    {
        private readonly Random _random;

        public RandomAI()
        {
            _random = new Random();
        }

        public int[] Bombs(Dot4GObj gameBoard)
        {
            return gameBoard.EmptySlots[_random.Next(gameBoard.EmptySlots.Count)];
        }

        public (Side, int) Play(Dot4GObj gameBoard)
        {
            return gameBoard.PossibleMoves[_random.Next(gameBoard.PossibleMoves.Count)];
        }
    }
}