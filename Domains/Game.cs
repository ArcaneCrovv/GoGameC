using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindowsFormsApp2.Domains
{
    public class Game
    {
        public Game(int comi, int boardSize)
        {
            Board = new GameBoard(boardSize);
            Score[BoardColor.White] = comi;
        }

        public event Action CurrentPlayerChanged;
        public event Action<BoardColor> ScoreChanged;
        public event Action<int, int, BoardColor> GameEnded;
        
        
        public bool IsBlackPlayerCurrent = true;
        public Dictionary<BoardColor, int> Score = new Dictionary<BoardColor, int>
        {
            {BoardColor.Black, 0},
            {BoardColor.White, 0}
        };
        public GameBoard Board;
        private bool _isPlayerBeforePassed;
        public GamePhase GamePhase = GamePhase.Game;

        public void SetStone (Point coordinates)
        {
            var currentPlayerColor = IsBlackPlayerCurrent ? BoardColor.Black : BoardColor.White;
            var scoreToAdd = Board.SetStoneNullIfCant(coordinates, currentPlayerColor);
            
            if (scoreToAdd == null)
                return;
            
            if (scoreToAdd != 0)
                ChangeScore(currentPlayerColor, scoreToAdd.Value);
            
            ChangeCurrentPlayer();
            _isPlayerBeforePassed = false;
        }

        private void ChangeScore(BoardColor addScoreColor, int scoreToAdd)
        {
            Score[addScoreColor] += scoreToAdd;
            ScoreChanged?.Invoke(addScoreColor);
        }

        private void ChangeCurrentPlayer()
        {
            IsBlackPlayerCurrent = !IsBlackPlayerCurrent;
            CurrentPlayerChanged?.Invoke();
        }

        public void Pass()
        {
            if (_isPlayerBeforePassed)
            {
                GamePhase = GamePhase.GameEnded;
                EndGame();
                return;
            }

            _isPlayerBeforePassed = true;
            ChangeCurrentPlayer();
        }

        private void EndGame()
        {
            var scoreToAdd = Board.CountTerritories();
            Score[BoardColor.Black] += scoreToAdd[BoardColor.Black];
            Score[BoardColor.White] += scoreToAdd[BoardColor.White];
            EndGameInvoke();
            ResetGame();
        }

        private void EndGameInvoke()
        {
            var winner = Score[BoardColor.Black] > Score[BoardColor.White] 
                ? BoardColor.Black : BoardColor.White;
            
            GameEnded?.Invoke(Score[BoardColor.Black], Score[BoardColor.White], winner);
        }

        private void ResetGame()
        {
            IsBlackPlayerCurrent = true;
            _isPlayerBeforePassed = false;
            CurrentPlayerChanged?.Invoke();
            
            Board.EmptyBoard();
            Score = new Dictionary<BoardColor, int>
            {
                {BoardColor.Black, 0},
                {BoardColor.White, 0}
            };
            ScoreChanged?.Invoke(BoardColor.Black);
            ScoreChanged?.Invoke(BoardColor.White);
        }
    }
}