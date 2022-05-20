using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentScissors
{
    public partial class StartAndEndPointChooser : Form
    {
        public StartAndEndPointChooser()
        {
            InitializeComponent();
        }

        private void StartAndEndPointChooser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (StartX.Text.Length != 0)
            {
                Program.startX = Convert.ToInt32(StartX.Text);
            }
            if(StartY.Text.Length != 0)
            {
                Program.startY = Convert.ToInt32(StartY.Text);
            }
            if (EndX.Text.Length != 0)
            {
                Program.endX = Convert.ToInt32(EndX.Text);
            }
            if (EndY.Text.Length != 0)
            {
                Program.endY = Convert.ToInt32(EndY.Text);
            }
        }

        private void StartX_TextChanged(object sender, EventArgs e)
        {

        }

        private void StartX_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void EndX_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void StartY_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void EndY_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (StartX.Text.Length == 0 || StartY.Text.Length == 0 || EndX.Text.Length == 0 || EndY.Text.Length == 0)
            {
                MessageBox.Show("Please write down the start and end Points");
                return;
            }
            Program.startX = Convert.ToInt32(StartX.Text);
            Program.startY = Convert.ToInt32(StartY.Text);
            Program.endX = Convert.ToInt32(EndX.Text);
            Program.endY = Convert.ToInt32(EndY.Text);

            this.Close();
            
        }
    }
}
