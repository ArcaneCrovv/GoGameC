using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp2.Domains;
using WindowsFormsApp2.Properties;

public class BoardPanel
{
    private static readonly Dictionary<BoardColor, Image> MazeImages = new Dictionary<BoardColor, Image>
    {
        {BoardColor.Liberty,Image.FromFile("C:\\Things\\GoGame\\Colors\\liber.bmp")},
        {BoardColor.Black, Image.FromFile("C:\\Things\\GoGame\\Colors\\black.bmp")},
        {BoardColor.White, Image.FromFile("C:\\Things\\GoGame\\Colors\\white.bmp")}
    };
    
    public static TableLayoutPanel CreateGameBoardPanel(Game model)
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

        model.Board.BoardStateChanged += (x, y, color) => 
            ((Button)table.GetControlFromPosition(x, y)).BackgroundImage = MazeImages[model.Board.StoneColorOn(x, y)];

        return table;
    }
}
