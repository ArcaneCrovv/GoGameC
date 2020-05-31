using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2;
using WindowsFormsApp2.Domains;
public class MyForm : Form
{
    public TableLayoutPanel CreateMainTable()
    {
        var mainTable = new TableLayoutPanel();
        mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        mainTable.Dock = DockStyle.Fill;

        return mainTable;
    }

    public MyForm(Game model)
    {
        var mainTable = CreateMainTable();

        var table = BoardPanel.CreateGameBoardPanel(model);
        var statTable = StatTable.CreateStatPanel(model);
        
        mainTable.Controls.Add(table, 0, 0);
        mainTable.Controls.Add(statTable, 1, 0);

        Controls.Add(mainTable);
    }
}
