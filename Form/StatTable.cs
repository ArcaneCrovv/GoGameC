using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp2.Domains;
public static class StatTable
    {
        public static TableLayoutPanel CreateStatPanel(Game model)
        {
            var table = new TableLayoutPanel{Dock = DockStyle.Fill};
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            table.Controls.Add(CreatePassButton(model), 0, 0);
            table.Controls.Add(CreateScorePanel(model), 0, 2);
            table.Controls.Add(new TableLayoutPanel(), 0, 2);
            
            return table;
        }
        
        private static Button CreatePassButton(Game model)
        {
            var passButton = new Button
            {
                Text = $@"Player {(model.IsBlackPlayerCurrent ? "Black" : "White")} current. Want to pass?",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 18, FontStyle.Regular)
            };
            passButton.Click += (sender, args) => model.Pass();
            model.CurrentPlayerChanged += () => passButton.Text 
                = $@"Player {(model.IsBlackPlayerCurrent ? "Black" : "White")} current. Want to pass?";
            return passButton;
        }

        private static TableLayoutPanel CreateScorePanel(Game model)
        {
            var scoreTable = new TableLayoutPanel {Dock = DockStyle.Fill};
            scoreTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            scoreTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            scoreTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            var scoreLabel = new Label
            {
                Text = "Score",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 20, FontStyle.Regular)
            };
            scoreTable.Controls.Add(scoreLabel, 0, 0);
            
            var inScore = new TableLayoutPanel {Dock = DockStyle.Fill};
            inScore.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            inScore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            inScore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            inScore.Controls.Add(BlackScoreLabel(model), 0, 0);
            inScore.Controls.Add(WhiteScoreLabel(model), 1, 0);
            
            scoreTable.Controls.Add(inScore, 0, 1);
            
            return scoreTable;
        }

        private static Label BlackScoreLabel(Game model)
        {
            var blackLabel = new Label
            {
                Text = $@"Black score {model.Score[BoardColor.Black]}",
                BackColor = Color.Black,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 18, FontStyle.Regular)
            };

            model.ScoreChanged += color =>
            {
                if (color == BoardColor.Black)
                    blackLabel.Text = $@"Black score {model.Score[BoardColor.Black]}";
            };
            
            return blackLabel;
        }

        private static Label WhiteScoreLabel(Game model)
        {
            var whiteLabel = new Label
            {
                Text = $@"White score {model.Score[BoardColor.White]}",
                BackColor = Color.Wheat,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 18, FontStyle.Regular)
            };
                        
            model.ScoreChanged += color =>
            {
                if (color == BoardColor.White)
                    whiteLabel.Text = $@"White score {model.Score[BoardColor.White]}";
            };            

            return whiteLabel;
        }
    }
