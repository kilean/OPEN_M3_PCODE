using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OIG
{
    public partial class Fo_SetSpeed : Form
    {
        public Fo_SetSpeed()
        {
            InitializeComponent();
            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void trackBar1_Scroll(Object sender, EventArgs e)
        {
            la_Speed.Text = trackBar1.Value.ToString(); 
        }

        private void btn_Save_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(Object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_N100_Click(object sender, EventArgs e)
        {
            int val = trackBar1.Value - 10;
            int c = (int)Math.Round(val / 10.0);
            val = c * 10;
            if (val < trackBar1.Minimum) val = trackBar1.Minimum;
            if (val > trackBar1.Maximum) val = trackBar1.Maximum;
            trackBar1.Value = val;
            la_Speed.Text = trackBar1.Value.ToString(); 
        }

        private void btn_N10_Click(object sender, EventArgs e)
        {
            int val = trackBar1.Value - 5;
            int c = (int)Math.Round(val / 5.0);
            val = c * 5;
            if (val < trackBar1.Minimum) val = trackBar1.Minimum;
            if (val > trackBar1.Maximum) val = trackBar1.Maximum;
            trackBar1.Value = val;
            la_Speed.Text = trackBar1.Value.ToString(); 
        }

        private void btn_P10_Click(object sender, EventArgs e)
        {
            int val = trackBar1.Value + 5;
            int c = (int)Math.Round(val / 5.0);
            val = c * 5;
            if (val < trackBar1.Minimum) val = trackBar1.Minimum;
            if (val > trackBar1.Maximum) val = trackBar1.Maximum;
            trackBar1.Value = val;
            la_Speed.Text = trackBar1.Value.ToString(); 
        }

        private void btn_P100_Click(object sender, EventArgs e)
        {
            int val = trackBar1.Value + 10;
            int c = (int)Math.Round(val / 10.0);
            val = c * 10;
            if (val < trackBar1.Minimum) val = trackBar1.Minimum;
            if (val > trackBar1.Maximum) val = trackBar1.Maximum;
            trackBar1.Value = val;
            la_Speed.Text = trackBar1.Value.ToString(); 
        }
    }
}
