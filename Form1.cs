using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class MyForm : Form
    {
        public MyForm()
        {
            var label = new Label();
            label.Location = new Point(0, 0);
            label.Size = new Size(ClientSize.Width, 30);
            label.Text = "Тут будет го.";
            Controls.Add(label);
        }
    }
}