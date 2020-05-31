using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2.Domains;

namespace WindowsFormsApp2
{
    public class MyForm : Form
    {
        private readonly Dictionary<BoardColor, Image> _mazeImages = new Dictionary<BoardColor, Image>
        {
            {BoardColor.Liberty,Image.FromFile("C:\\Things\\GoGame\\Colors\\liber.bmp")},
            {BoardColor.Black, Image.FromFile("C:\\Things\\GoGame\\Colors\\black.bmp")},
            {BoardColor.White, Image.FromFile("C:\\Things\\GoGame\\Colors\\white.bmp")}
        };

        public TableLayoutPanel CreateMainTable()
        {
            var mainTable = new TableLayoutPanel();
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            mainTable.Dock = DockStyle.Fill;

            return mainTable;
        }

        public TableLayoutPanel CreateGameBoardPanel(Game model)
        {
            var table = new TableLayoutPanel {Dock = DockStyle.Fill};

            for (var i = 0; i < model.Board.Size; i++)
            for (var j = 0; j < model.Board.Size; j++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / model.Board.Size));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / model.Board.Size));
            }
            
            for (var row = 0; row < model.Board.Size; row++)
            for (var colomn = 0; colomn < model.Board.Size; colomn++)
            {
                var iRow = row;
                var iColomn = colomn;
                var button = new Button
                {
                    BackgroundImage =  _mazeImages[BoardColor.Liberty],
                    Dock = DockStyle.Fill,
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Bounds = Rectangle.Empty,
                    Margin = Padding.Empty,
                    FlatStyle = FlatStyle.Flat,
                };
                button.FlatAppearance.BorderSize = 0;
                button.Click += (sender, args) => model.SetStone(new Point(iColomn, iRow));
                table.Controls.Add(button, iColomn, iRow);
            }

            model.Board.BoardStateChanged += (x, y, color) => 
                ((Button)table.GetControlFromPosition(x, y)).BackgroundImage = _mazeImages[model.Board.StoneColorOn(x, y)];

            return table;
        }

        private Button CreatePassButton(Game model)
        {
            var passButton = new Button
            {
                Text = $@"Player {(model.IsBlackPlayerCurrent ? "Black" : "White")} current. Want to pass?",
                Dock = DockStyle.Fill
            };
            passButton.Click += (sender, args) => model.Pass();
            model.CurrentPlayerChanged += () => passButton.Text 
                = $@"Player {(model.IsBlackPlayerCurrent ? "Black" : "White")} current. Want to pass?";
            return passButton;
        }
        
        public TableLayoutPanel CreateStatPanel(Game model)
        {
            var table = new TableLayoutPanel{Dock = DockStyle.Fill};
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            table.Controls.Add(CreatePassButton(model));
            
            table.Controls.Add(CreateScorePanel(model), 0, 2);
            
            return table;
        }

        public Label BlackScoreLabel(Game model)
        {
            var blackLabel = new Label
            {
                Text = $@"Black score {model.Score[BoardColor.Black]}",
                BackColor = Color.Black,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            model.ScoreChanged += color =>
            {
                if (color == BoardColor.Black)
                    blackLabel.Text = $@"Black score {model.Score[BoardColor.Black]}";
            };
            
            return blackLabel;
        }

        public Label WhiteScoreLabel(Game model)
        {
                        var whiteLabel = new Label
            {
                Text = $@"White score {model.Score[BoardColor.White]}",
                BackColor = Color.Wheat,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
                        
            model.ScoreChanged += color =>
            {
                if (color == BoardColor.White)
                    whiteLabel.Text = $@"White score {model.Score[BoardColor.White]}";
            };            

            return whiteLabel;
        }

        public TableLayoutPanel CreateScorePanel(Game model)
        {
            var scoreTable = new TableLayoutPanel {Dock = DockStyle.Fill};
            scoreTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            scoreTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            scoreTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            scoreTable.Controls.Add(new Label
            {
                Text = "Score", 
                TextAlign = ContentAlignment.MiddleCenter
            }, 0, 0);
            
            var inScore = new TableLayoutPanel {Dock = DockStyle.Fill};
            inScore.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            inScore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            inScore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            inScore.Controls.Add(BlackScoreLabel(model), 0, 0);
            inScore.Controls.Add(WhiteScoreLabel(model), 1, 0);
            
            scoreTable.Controls.Add(inScore, 0, 1);
            
            return scoreTable;
        }
        
        public MyForm(Game model)
        {
            var mainTable = CreateMainTable();

            var table = CreateGameBoardPanel(model);
            var statTable = CreateStatPanel(model);
            
            mainTable.Controls.Add(table, 0, 0);
            mainTable.Controls.Add(statTable, 1, 0);

            Controls.Add(mainTable);
        }
    }
}