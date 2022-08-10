using Ajuna.NetApi;
using Ajuna.NetApi.Model.Base;
using Ajuna.NetApi.Model.Dot4gravity;
using Ajuna.NetApi.Model.PalletBoard;
using Ajuna.NetApi.Model.SpCore;
using Ajuna.NetApi.Model.Types.Base;
using Ajuna.NetApi.Model.Types.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ajuna.NetApiExt.Model.AjunaWorker.Dot4G
{

    public class Dot4GObj
    {
        public int Id { get; }

        public int Seed { get; }

        public Dot4GCell[,] Board { get; set; }

        public Dictionary<string, Dot4GPlayer> Players { get; set; } = new Dictionary<string, Dot4GPlayer>();

        public GamePhase GamePhase { get; }

        public Dot4GMove LastTurn { get; }

        public string Next { get; }

        public string Winner { get; }

        public List<(Side, int)> PossibleMoves { get; }

        public List<int[]> EmptySlots { get; }

        public Dot4GObj(BoardGame boardGame)
        {
            var playerAddress = boardGame.State.Players.Value.Select(p => Utils.GetAddressFrom(p.Value.Value.Select(q => q.Value).ToArray())).ToList();
            for (int i = 0; i < playerAddress.Count(); i++)
            {
                Players.Add(playerAddress[i], new Dot4GPlayer(GetPlayername(playerAddress[i]), playerAddress[i], i + 1));
            }
            GamePhase = boardGame.State.Phase.Value;
            Id = (int) boardGame.BoardId.Value;
            Seed = (int)boardGame.State.Seed.Value;
            Winner = boardGame.State.Winner.OptionFlag ? Utils.GetAddressFrom(boardGame.State.Winner.Value.Value.Bytes) : null;

            foreach (var bomb in boardGame.State.Bombs.Value.ToList())
            {
                var player = Players.Values.Where(p => p.Address == Utils.GetAddressFrom(((AccountId32)bomb.Value[0]).Value.Value.Select(q => q.Value).ToArray())).FirstOrDefault();
                if (player != null)
                {
                    player.Bombs = ((U8)bomb.Value[1]).Value;
                }
            }

            Next = Utils.GetAddressFrom(boardGame.State.NextPlayer.Value.Bytes);

            EnumCell[][] array = boardGame.State.Board.Cells.Value.Select(p => p.Value).ToArray();

            Board = new Dot4GCell[array.Length, array[0].Length];

            for (int i = 0; i < array.Length; i++)
            {
                EnumCell[] enumCellArray = array[i];
                for (int j = 0; j < enumCellArray.Length; j++)
                {
                    EnumCell enumCell = enumCellArray[j];
                    switch (enumCell.Value)
                    {
                        case Cell.Empty:
                            Board[i, j] = new Dot4GCell(new int[] { i, j }, enumCell.Value);
                            break;

                        case Cell.Bomb:
                            var playerBombs = ((Arr2Special1)enumCell.Value2).Value.Where(p => p.OptionFlag).Select(p => p.Value.Value).ToArray();
                            Board[i, j] = new Dot4GCell(new int[] { i, j }, enumCell.Value, playerBombs);
                            break;

                        case Cell.Block:
                            Board[i, j] = new Dot4GCell(new int[] { i, j }, enumCell.Value);
                            break;

                        case Cell.Stone:
                            var playerId = ((U8)enumCell.Value2).Value;
                            Board[i, j] = new Dot4GCell(new int[] { i, j }, enumCell.Value, playerId);
                            break;
                    }
                }
            }
            LastTurn = GetLastTurn(boardGame.State.LastMove); 
            PossibleMoves = DropStoneList();
            EmptySlots = GetCoords(Cell.Empty);
        }

        private Dot4GMove GetLastTurn(BaseOpt<LastMove> lastMove)
        {
            if (lastMove.OptionFlag == false)
            {
                return null;
            }

            return new Dot4GMove()
            {
                Id = 0,
                PlayerAddress = Utils.GetAddressFrom(lastMove.Value.Player.Value.Bytes),
                Side = lastMove.Value.Side.Value,
                Column = lastMove.Value.Position.Value,
            };
        }

        public bool ValidateBomb(int posX, int posY)
        {
            return EmptySlots.Where(p => p[0] == posX && p[1] == posY).Any();
        }

        public bool ValidateStone(Side side, int col)
        {
            return PossibleMoves.Where(p => p.Item1 == side && p.Item2 == col).Any();
        }

        private List<(Side, int)> DropStoneList()
        {
            var list = GetCoords(Cell.Empty);
            var moves = new List<(Side, int)>();

            list.Where(p => p[0] == 0).Select(p => p[1]).ToList()
                .ForEach(p => moves.Add((Side.North, p)));
            list.Where(p => p[1] == 9).Select(p => p[0]).ToList()
                .ForEach(p => moves.Add((Side.East, p)));
            list.Where(p => p[0] == 9).Select(p => p[1]).ToList()
                .ForEach(p => moves.Add((Side.South, p)));
            list.Where(p => p[1] == 0).Select(p => p[0]).ToList()
                .ForEach(p => moves.Add((Side.West, p)));

            return moves;
        }

        public List<int[]> GetRay(Side side, int column)
        {
            List<int[]> result = new List<int[]>();

            if (!ValidateStone(side, column))
            {
                return result;
            }

            switch (side)
            {
                case Side.North:
                    for (int x = 0; x < Board.GetLength(0); x++)
                    {
                        result.Add(new int[] { x, column });
                        if (x < Board.GetLength(0) - 1 && Board[x + 1, column].Cell == Cell.Empty)
                        {
                            continue;
                        }
                        break;
                    }
                    break;

                case Side.East:
                    for (int y = Board.GetLength(1) - 1; y >= 0; y--)
                    {
                        result.Add(new int[] { column, y });
                        if (y > 0 && Board[column, y - 1].Cell == Cell.Empty)
                        {
                            continue;
                        }
                        break;
                    }
                    break;

                case Side.South:
                    for (int x = Board.GetLength(0) - 1; x >= 0; x--)
                    {
                        result.Add(new int[] { x, column });
                        if (x > 0 && Board[x - 1, column].Cell == Cell.Empty)
                        {
                            continue;
                        }
                        break;
                    }
                    break;

                case Side.West:
                    for (int y = 0; y < Board.GetLength(1); y++)
                    {
                        result.Add(new int[] { column, y });
                        if (y < Board.GetLength(1) - 1 && Board[column, y + 1].Cell == Cell.Empty)
                        {
                            continue;
                        }
                        break;
                    }
                    break;
            }

            return result;
        }

        public List<int[]> GetStroke(Side side, int column, int stone, out int tagged)
        {

            tagged = 0;
            List<int[]> result = new List<int[]>();

            if (!ValidateStone(side, column))
            {
                return result;
            }

            switch (side)
            {
                case Side.North:

                    for (int x = 0; x < Board.GetLength(0); x++)
                    {
                        result.Add(new int[] { x, column });

                        if (x < Board.GetLength(0) - 1)
                        {
                            var next = Board[x + 1, column];

                            if (next.Cell == Cell.Stone && next.PlayerIds.First() == stone)
                            {
                                tagged = +1;
                                continue;
                            }
                            
                            if (tagged == 0 && next.Cell == Cell.Empty)
                            {
                                continue;
                            }
                        }
                        break;
                    }
                    break;

                case Side.East:
                    for (int y = Board.GetLength(1) - 1; y >= 0; y--)
                    {
                        result.Add(new int[] { column, y });
                        
                        if (y > 0)
                        {
                            var next = Board[column, y - 1];

                            if (next.Cell == Cell.Stone && next.PlayerIds.First() == stone)
                            {
                                tagged = +1;
                                continue;
                            }

                            if (next.Cell == Cell.Empty)
                            {
                                continue;
                            }
                        }
                        break;
                    }
                    break;

                case Side.South:
                    for (int x = Board.GetLength(0) - 1; x >= 0; x--)
                    {
                        result.Add(new int[] { x, column });

                        if (x > 0)
                        {
                            var next = Board[x - 1, column];

                            if (next.Cell == Cell.Stone && next.PlayerIds.First() == stone)
                            {
                                tagged = +1;
                                continue;
                            }

                            if (next.Cell == Cell.Empty)
                            {
                                continue;
                            }
                        }
                        break;
                    }
                    break;

                case Side.West:
                    for (int y = 0; y < Board.GetLength(1); y++)
                    {
                        result.Add(new int[] { column, y });

                        if (y < Board.GetLength(1) - 1)
                        {
                            var next = Board[column, y + 1];

                            if (next.Cell == Cell.Stone && next.PlayerIds.First() == stone)
                            {
                                tagged = +1;
                                continue;
                            }

                            if (next.Cell == Cell.Empty)
                            {
                                continue;
                            }
                        }
                        break;
                    }
                    break;
            }

            return result;
        }

        private string GetPlayername(string address)
        {
            switch (address)
            {
                case "5GrwvaEF5zXb26Fz9rcQpDWS57CtERHpNehXCPcNoHGKutQY":
                    return "Alice";
                case "5FHneW46xGXgs5mUiveU4sbTyGBzmstUspZC92UhjJM694ty":
                    return "Bob";
                default:
                    return address.Substring(0, 7);
            }
        }

        public List<int[]> GetCoords(Cell cell)
        {
            var result = new List<int[]>();
            for (int i = 0; i < Board.GetLength(0); i++)
            {

                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    if (Board[i, j].Cell == cell) {
                        result.Add(new int[] { i, j});
                    }

                }
            }
            return result;
        }

        public void Print()
        {
            Console.WriteLine("+---------------------------------------+");
            Console.WriteLine("| " + $"Board[{Id}|{Seed}] Phase: {GamePhase}".PadRight(38) + "|");
            Console.WriteLine("| " + $"Empty[{EmptySlots.Count}] - Moves[{PossibleMoves.Count}]".PadRight(38) + "|");
            Players.Values.ToList().ForEach(p => Console.WriteLine("| " + p.ToString().PadRight(38) + "|"));
            Console.WriteLine("| " + $"Next: {GetPlayername(Players[Next].Name)}".PadRight(38) + "|");
            Console.WriteLine("| " + $"Winner: {(Winner != null ? GetPlayername(Winner) : "...")}".PadRight(38) + "|");
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                Console.WriteLine("+---+---+---+---+---+---+---+---+---+---+");
                Console.Write($"|");
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    Console.Write(Board[i, j].ToString());
                    Console.Write($"|");
                }
                Console.WriteLine();
            }
            Console.WriteLine("+---+---+---+---+---+---+---+---+---+---+");
        }
    }
}
