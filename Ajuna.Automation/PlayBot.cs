using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Ajuna.Automation.AI;
using Ajuna.Automation.Enums;
using Ajuna.Automation.Model;
using Ajuna.NetApi;
using Ajuna.NetApi.Model.Types.Primitive;
using AjunaNET.NetApiExt.Generated.Model.pallet_ajuna_board.types;
using AjunaNET.NetApiExt.Generated.Model.dot4gravity;
using Serilog;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using AjunaNET.NetApiExt.Generated.Types.Base;

namespace Ajuna.Automation
{
    public class PlayBot
    {
        private readonly NodeClient _client;
        private readonly IBotAI _logic;

        private readonly Dictionary<string, long[]> _tracker;
        private readonly Stopwatch _stopwatch;

        private BoardGame? _boardGame;

        private U32? _playerBoards;

        public PlayBot(NodeClient nodeClient, IBotAI logic)
        {
            _client = nodeClient;
            _logic = logic;

            _tracker = new Dictionary<string, long[]>();
            _stopwatch = new Stopwatch();
        }

        internal async Task RunAsync(CancellationToken token)
        {
            var SleepTime = 1000;

            NodeState nodeState = NodeState.None;
            PlayState playState = PlayState.None;

            _stopwatch.Start();

            while (!token.IsCancellationRequested)
            {
                nodeState = await GetNodeStateAsync(nodeState, token);
                
                await DoNodeAsync(nodeState, token);

                if (nodeState == NodeState.Play)
                {
                    _boardGame = await _client.GetBoardGamesAsync(_playerBoards, token);

                    //    var myTurn = _gameBoard.Next == _nodeClient.Account.Value;
                    //    var winner = _gameBoard.Winner != null && _gameBoard.Winner.Any();
                    //    Log.Information("GameBoard[{id}|{phase}] - turn:{bool} [e{empty}|m{moves}] win:{winner}", _gameBoard.Id, _gameBoard.GamePhase, myTurn, _gameBoard.EmptySlots.Count, _gameBoard.PossibleMoves.Count, winner);

                    playState = GetPlayState(playState, _boardGame);
                    
                    await DoPlayAsync(playState, _boardGame, token);
                }

                Thread.Sleep(SleepTime);
            }
        }

        private async Task<NodeState> GetNodeStateAsync(NodeState nodeState, CancellationToken token)
        {
            // Connect
            if (!_client.IsConnected)
            {
                return ChangeState(nodeState, NodeState.Connect);
            }

            var accountInfo = await _client.GetAccountInfoAsync(token);

            // Faucet
            if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value < _client.Token(10))
            {
                return ChangeState(nodeState, NodeState.Faucet);
            }

            _playerBoards = await _client.GetPlayerBoardsAsync(token);


            var playerQueue = await _client.GetPlayerQueueAsync(token);

            if (playerQueue == null && _playerBoards == null)
            {
                return ChangeState(nodeState, NodeState.Queue);
            }

            if (_playerBoards != null)
            {
                return ChangeState(nodeState, NodeState.Play);
            }

            return ChangeState(nodeState, NodeState.Wait);
        }

        private async Task DoNodeAsync(NodeState nodeState, CancellationToken token)
        {
            switch (nodeState)
            {
                case NodeState.Connect:
                    _ = await _client.ConnectAsync(false, true, token);
                    break;

                case NodeState.Faucet:
                    if (await _client.FaucetAsync(_client.Token(1000), token))
                    {
                        WaitOnExtrinsic();
                    }
                    break;

                case NodeState.Queue:
                    if (await _client.QueueAsync(token))
                    {
                        WaitOnExtrinsic();
                    }
                    break;

                case NodeState.Finished:
                    var downTime = 10000;
                    Log.Information("Downtime for {value} sec.", (double)downTime / 1000);
                    Thread.Sleep(downTime);
                    break;

                case NodeState.Disconnect:
                    _ = await _client.DisconnectAsync();
                    break;

                default:
                    break;
            }
        }

        private PlayState GetPlayState(PlayState playState, BoardGame? boardGame)
        {
            if (boardGame == null)
            {
                return ChangeState(playState, PlayState.NoBoard);
            }

            var gameBoard = new Dot4GObj(boardGame);

            switch (gameBoard.GamePhase)
            {
                    case GamePhase.Bomb:
                        var player = gameBoard.Players.Values.Where(p => p.Address == _client.Account.Value).ToList();

                        if (player.Count == 1 && player[0].Bombs > 0)
                        {
                            return ChangeState(playState, PlayState.Bomb);
                        }

                        return ChangeState(playState, PlayState.OpBomb);

                    case GamePhase.Play:

                        if (gameBoard.Winner != null && gameBoard.Winner.Any())
                        {
                            return ChangeState(playState, PlayState.Finished, $"winner:{gameBoard.Winner}");
                        }

                        if (!gameBoard.PossibleMoves.Any())
                        {
                            return ChangeState(playState, PlayState.Finished, $"draw");
                        }

                        if (gameBoard.Next == _client.Account.Value)
                        {
                            return ChangeState(playState, PlayState.Stone, $"moves:{gameBoard.PossibleMoves.Count}");
                        }

                        return ChangeState(playState, PlayState.OpStone);

            }

            return ChangeState(playState, PlayState.None);
        }

        private async Task DoPlayAsync(PlayState playState, BoardGame? boardGame, CancellationToken token)
        {
            if (boardGame == null)
            {
                return;
            }

            var gameBoard = new Dot4GObj(boardGame);

            switch (playState)
            {
                case PlayState.Bomb:
                    {
                        int[] bombPos = _logic.Bombs(gameBoard);
                        var col = new U8();
                        col.Create((byte)bombPos[0]);
                        var row = new U8();
                        row.Create((byte)bombPos[1]);
                        if (await _client.BombAsync(col, row, token))
                        {
                            Log.Information("[{0}]: Set a bomb!", _client.Address(true));
                            WaitOnExtrinsic();
                        }
                    }
                    break;

                case PlayState.Stone:
                    {
                        
                        (Side, int) move = _logic.Play(gameBoard);
                        var col = new U8();
                        col.Create((byte)move.Item2);
                        if (await _client.StoneAsync(move.Item1, col, token))
                        {
                            Log.Information("[{0}]: Played stone!", _client.Address(true));
                        }
                    }
                    break;
            }
        }

        private void WaitOnExtrinsic()
        {
            // wait on extrinsic
            var running = _client.ExtrinsicManger.Running;
            if (running.Any())
            {
                Log.Debug("Waiting on {count} extrinsic proccesed!", running.Count());
                while (running.Any())
                {
                    Thread.Sleep(1000);
                    running = _client.ExtrinsicManger.Running;
                }
                Log.Debug("All extrinsic proccessed!");
            }
        }

        private T ChangeState<T>(T oldState, T newState, string msg = "")
        {
            if (oldState == null || newState == null)
            {
                return oldState;
            }

            if (oldState.Equals(newState))
            {
                return oldState;
            }

            var key = typeof(T).Name + "_" + oldState.ToString();
            var elapsedMs = _stopwatch.ElapsedMilliseconds;
            if (_tracker.TryGetValue(key, out long[] values))
            {
                values[0] = values[0] + 1;
                values[1] = values[1] + elapsedMs;
                _tracker[key] = values;
            }
            else
            {
                _tracker.Add(key, new long[] { 1, elapsedMs });
            }
            _stopwatch.Restart();

            if (msg != null && msg.Any())
            {
                msg = " - " + msg;
            }

            Log.Information("{name} = {state1} from {state2} in {ms} sec.{msg}", typeof(T).Name, newState, oldState, (double)elapsedMs / 1000, msg);

            return newState;
        }
    }
}