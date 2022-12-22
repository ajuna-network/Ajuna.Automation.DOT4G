using System;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using AjunaNET.NetApiExt.Generated.Model.dot4gravity;

namespace Ajuna.Automation.AI
{
    public class StraightAI : IBotAI
    {
        private readonly Random _random;

        public StraightAI()
        {
            _random = new Random();
        }

        public int[] Bombs(Dot4GObj gameBoard)
        {
            return gameBoard.EmptySlots[_random.Next(gameBoard.EmptySlots.Count)];
        }

        public (Side, int) Play(Dot4GObj gameBoard)
        {
            if (GetWinnerLine(gameBoard, out int best))
            {
                return gameBoard.PossibleMoves[best];
            }

            return gameBoard.PossibleMoves[_random.Next(gameBoard.PossibleMoves.Count)];
        }

        private bool GetWinnerLine(Dot4GObj gameBoard, out int best)
        {
            best = 0;
            var me = gameBoard.Players[gameBoard.Next];

            var bestTagged = 0;
            for (int i = 0; i < gameBoard.PossibleMoves.Count; i++)
            {
                (Side, int) moves = gameBoard.PossibleMoves[i];
                var stroke = gameBoard.GetStroke(moves.Item1, moves.Item2, me.Stone, out int tagged);
                if (stroke.Count > 3 && bestTagged < tagged)
                {
                    best = 1;
                }
            }

            return true;
        }
    }
}