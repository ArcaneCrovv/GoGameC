using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2.Domains;

namespace WindowsFormsApp2
{
    public class MyForm : Form
    {
        Dictionary<BoardColor, Image> MazeImages = new Dictionary<BoardColor, Image>
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
            
            table.BackgroundImageLayout = ImageLayout.Stretch;
            
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
                    BackgroundImage =  MazeImages[BoardColor.Liberty],
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

            model.Board.StateChanged += (x, y, color) => 
                ((Button)table.GetControlFromPosition(x, y)).BackgroundImage = MazeImages[model.Board.StoneColorOn(x, y)];

            return table;
        }

        public TableLayoutPanel CreateStatPanel(Game model)
        {
            var table = new TableLayoutPanel{Dock = DockStyle.Fill};
            return null;
        }
        
        public MyForm(Game model)
        {
            var mainTable = CreateMainTable();

            var table = CreateGameBoardPanel(model);
            
            mainTable.Controls.Add(table, 0, 0);
            mainTable.Controls.Add(new TableLayoutPanel{Dock = DockStyle.Fill}, 1, 0);

            Controls.Add(mainTable);
        }
    }
}