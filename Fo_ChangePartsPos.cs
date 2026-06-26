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
    public partial class Fo_ChangePartsPos :Form
    {
        public bool bClose;
        public bool bCloseFinish;

        public Fo_ChangePartsPos()
        {
            InitializeComponent();
            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void btn_Save_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_XPos_Click(object sender, EventArgs e)
        {
            TB_PosX.Text = la_PosX.Text;
        }

        private void btn_ZPos_Click(object sender, EventArgs e)
        {
            TB_PosZ.Text = la_PosZ.Text;
        }

        private void Fo_ChangePartsPos_FormClosing(object sender, FormClosingEventArgs e)
        {
            bClose = true;
            while (!bCloseFinish) Application.DoEvents();
        }

        private void TextBoxClick(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            form.uc_UserNum1.la_Msg.Visible = false;
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString();
            }
        }
    }
}
