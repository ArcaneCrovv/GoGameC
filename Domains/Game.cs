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
        
        public bool IsBlackPlayerCurrent = true;
        public Dictionary<BoardColor, int> Score = new Dictionary<BoardColor, int>
        {
            {BoardColor.Black, 0},
            {BoardColor.White, 0}
        };
        public GameBoard Board;
        public bool IsPlayerBeforePassed;
        public GamePhase GamePhase = GamePhase.Game;

        public void SetStone (Point coordinates)
        {
            var currentPlayerColor = IsBlackPlayerCurrent ? BoardColor.Black : BoardColor.White;
            var scoreToAdd = Board.SetStoneNullIfCant(coordinates, currentPlayerColor);
            
            if (scoreToAdd == null)
                return;
            
            Score[currentPlayerColor] += scoreToAdd.Value;
            IsBlackPlayerCurrent = !IsBlackPlayerCurrent;
            IsPlayerBeforePassed = false;
        }

        public void Pass()
        {
            if (IsPlayerBeforePassed)
            {
                GamePhase = GamePhase.GameEnded;
                EndGame();
            }

            IsPlayerBeforePassed = true;
            IsBlackPlayerCurrent = !IsBlackPlayerCurrent;
        }

        private void EndGame()
        {
            var scoreToAdd = Board.CountTerritories();
            Score[BoardColor.Black] += scoreToAdd[BoardColor.Black];
            Score[BoardColor.White] += scoreToAdd[BoardColor.White];
        }

        public BoardColor Winner()
        {
            return Score[BoardColor.Black] > Score[BoardColor.White] ? BoardColor.Black : BoardColor.White;
        }
    }
}