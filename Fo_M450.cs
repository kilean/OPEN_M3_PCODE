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
    public partial class Fo_M450 : Form
    {
        public Fo_M450()
        {
            InitializeComponent();
        }

        private void btn_Finish_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.M450Status = 0;
            Units.Fo_Main.bM450Finish = true;
            this.Close();
        }

        private void btn_Setting_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.M450Status = 1;
            Units.Fo_Main.bM450Finish = true;
            this.Close();
        }
    }
}
