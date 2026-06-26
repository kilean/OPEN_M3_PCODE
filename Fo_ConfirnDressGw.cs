using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCD
{
    public partial class Fo_ConfirnDressGw :Form
    {
        public Fo_ConfirnDressGw()
        {
            InitializeComponent();
            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void button1_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
