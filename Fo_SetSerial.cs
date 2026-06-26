using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace OCD
{
    public partial class Fo_SetSerial : Form
    {
        public Fo_SetSerial()
        {
            InitializeComponent();
            this.LoadLanguageFile(Units.langfile, this.Name);
            string[] ports = SerialPort.GetPortNames();
            foreach (string s in ports)
            {
                CB_Port422.Items.Add(s);
                CB_Port485.Items.Add(s);
            }

            if (CB_Port422.Items.Count > 0) CB_BaudRate422.SelectedIndex = 0;
            if (CB_Port485.Items.Count > 0) CB_BaudRate485.SelectedIndex = 0;


        }

        private void btn_OK_Click(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void TextBoxClick(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            TextBox box = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2;
            form.Top = Screen.PrimaryScreen.Bounds.Height - form.Height;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", box.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                box.Text = data.ToString("0.##");
                box.Focus();
                SendKeys.Send("{ENTER}");
            }
        }
        private void CB_Dev5_SelectedIndexChanged(object sender, EventArgs e)
        {
            CB_SP_Channel.Visible = CB_SP_Dev.SelectedIndex == 1;
        }


        private void TB_Gw1Hz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(TB_GW1_SetHz.Text, out double Hz))
                {
                    TB_GW1_SetHz.BackColor = Color.Red;
                }
                if (!double.TryParse(TB_GW1_SetRPM.Text, out double Now))
                {
                    TB_GW1_SetRPM.BackColor = Color.Red;
                }
                TB_GW1_SetHz.Text = Hz.ToString("0.##");
                TB_GW1_SetRPM.Text = Now.ToString("0.##");
                TB_GW1_Rate.Text = (Now / Hz).ToString("0.##");
            }
        }

        private void TextBoxColor(object sender, EventArgs e)
        {
            TextBox tb =sender as TextBox;
            if(tb.BackColor == Color.Red)
            {
                tb.BackColor = Color.White;
            }
        }

        private void TB_Gw2Hz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(TB_GW2_SetHz.Text, out double Hz))
                {
                    TB_GW2_SetHz.BackColor = Color.Red;
                }
                if (!double.TryParse(TB_GW2_SetRPM.Text, out double Now))
                {
                    TB_GW2_SetRPM.BackColor = Color.Red;
                }
                TB_GW2_SetHz.Text = Hz.ToString("0.##");
                TB_GW2_SetRPM.Text = Now.ToString("0.##");
                TB_GW2_Rate.Text = (Now / Hz).ToString("0.##");
            }
        }

        private void TB_Gw3Hz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(TB_GW3_SetHz.Text, out double Hz))
                {
                    TB_GW3_SetHz.BackColor = Color.Red;
                }
                if (!double.TryParse(TB_GW3_SetRPM.Text, out double Now))
                {
                    TB_GW3_SetRPM.BackColor = Color.Red;
                }
                TB_GW3_SetHz.Text = Hz.ToString("0.##");
                TB_GW3_SetRPM.Text = Now.ToString("0.##");
                TB_GW3_Rate.Text = (Now / Hz).ToString("0.##");
            }
        }

        private void TB_Gw4Hz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(TB_GW4_SetHz.Text, out double Hz))
                {
                    TB_GW4_SetHz.BackColor = Color.Red;
                }
                if (!double.TryParse(TB_GW4_SetRPM.Text, out double Now))
                {
                    TB_GW4_SetRPM.BackColor = Color.Red;
                }
                TB_GW4_SetHz.Text = Hz.ToString("0.##");
                TB_GW4_SetRPM.Text = Now.ToString("0.##");
                TB_GW4_Rate.Text = (Now / Hz).ToString("0.##");
            }
        }

        private void TB_Dev2Now_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(TB_GW2_SetHz.Text, out double Hz))
                {
                    TB_GW2_SetHz.BackColor = Color.Red;
                }
                if (!double.TryParse(TB_GW2_SetRPM.Text, out double Now))
                {
                    TB_GW2_SetRPM.BackColor = Color.Red;
                }

                TB_GW2_Rate.BackColor = Color.Red;
            }
        }
    }
}
