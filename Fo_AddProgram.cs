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
    public partial class Fo_AddProgram : Form
    {
        public Fo_AddProgram()
        {
            InitializeComponent();

            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void btn_OK_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(Object sender, EventArgs e)
        {
            this.Close();
        }

        private void TextBoxClick(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Msg.Visible = false;
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                box.Text = data.ToString();
            }
        }

        private void BtnClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
                return;

            TB_ProgName.SelectedText = btn.Text;

            TB_ProgName.Focus();
        }

        private void button56_Click(object sender, EventArgs e)
        {
            if (TB_ProgName.SelectionStart <= 0)
                return;

            if (TB_ProgName.Text[TB_ProgName.SelectionStart - 1] == '\n')
            {
                TB_ProgName.SelectionStart = TB_ProgName.SelectionStart - 2;
                TB_ProgName.SelectionLength = 2;
            }
            else
            {
                TB_ProgName.SelectionStart = TB_ProgName.SelectionStart - 1;
                TB_ProgName.SelectionLength = 1;
            }
            TB_ProgName.SelectedText = "";

            TB_ProgName.Focus();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            if (TB_ProgName.SelectionStart <= 0)
                return;

            
            TB_ProgName.SelectionStart = TB_ProgName.SelectionStart - 1;
            TB_ProgName.Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TB_ProgName.SelectionStart = TB_ProgName.SelectionStart + 1;
            TB_ProgName.Focus();
        }
    }
}
