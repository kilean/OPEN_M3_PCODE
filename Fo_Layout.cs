using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OIG
{
    public partial class Fo_Layout : Form
    {
        public Label la_Temp;
        private Thread ThrMain;
        public Fo_Layout(Label label)
        {
            InitializeComponent();
            la_Temp = label;
        }

        public void Execute()
        {
            Thread.Sleep(200);
            while (true)
            {
                this.Invoke(new Action(() =>
                {
                    if (this.Visible)
                    {
                        if (Control.MouseButtons == MouseButtons.Left)
                        {
                            //la_LeftPos.Text = la_Temp.Parent.Left.ToString();
                            //la_TopPos.Text = la_Temp.Parent.Top.ToString();
                            //la_HeightValue.Text = la_Temp.Parent.Height.ToString();
                            //la_WidthValue.Text = la_Temp.Parent.Width.ToString();

                            tb_Left.Text = la_Temp.Parent.Left.ToString();
                            tb_Top.Text = la_Temp.Parent.Top.ToString();
                            tb_Width.Text = la_Temp.Parent.Width.ToString();
                            tb_Height.Text = la_Temp.Parent.Height.ToString();
                        }
                    }
                }));
            }
        }

        public void label1_MouseMove(object sender, MouseEventArgs e)
        {
            //當在Panel1 按下滑鼠左鍵
            if (e.Button == MouseButtons.Left)
            {
                WinApi.ReleaseCapture();
                WinApi.SendMessage(la_Temp.Parent.Handle, 161, 2, IntPtr.Zero);
            }
        }

        private void Fo_Layout_FormClosing(object sender, FormClosingEventArgs e)
        {
            ThrMain.Abort();
            la_Temp.MouseMove -= label1_MouseMove;
            Units.Fo_Main.fo_layout = null;

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            
            int.TryParse(tb_Left.Text, out int left);
            la_Temp.Parent.Left = left;
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Left", left);

            int.TryParse(tb_Top.Text, out int top);
            la_Temp.Parent.Top = top;
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Top", top);

            int.TryParse(tb_Width.Text, out int width);
            la_Temp.Parent.Width = width;
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Width", width);

            int.TryParse(tb_Height.Text, out int height);
            la_Temp.Parent.Height = height;
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Height", height);

        }

        private void tb_Left_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(tb_Left.Text, out int left);
                la_Temp.Parent.Left = left;
                //la_LeftPos.Text = tb_Left.Text;
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteInteger("UI", la_Temp.Parent.Name + "_Left", left);
            }
        }

        private void tb_Top_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(tb_Top.Text, out int top);
                la_Temp.Parent.Top = top;
                //la_TopPos.Text = tb_Top.Text;
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteInteger("UI", la_Temp.Parent.Name + "_Top", top);
            }
        }

        private void tb_Width_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(tb_Width.Text, out int width);
                la_Temp.Parent.Width = width;
                //la_WidthValue.Text = tb_Width.Text;
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteInteger("UI", la_Temp.Parent.Name + "_Width", width);
            }
        }

        private void tb_Height_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int.TryParse(tb_Height.Text, out int height);
                la_Temp.Parent.Height = height;
                //la_HeightValue.Text = tb_Height.Text;
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteInteger("UI", la_Temp.Parent.Name + "_Height", height);
            }
        }

        private void tb_Left_Leave(object sender, EventArgs e)
        {
            int.TryParse(tb_Left.Text, out int left);
            la_Temp.Parent.Left = left;
            //la_LeftPos.Text = tb_Left.Text;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Left", left);
        }

        private void tb_Top_Leave(object sender, EventArgs e)
        {
            int.TryParse(tb_Top.Text, out int top);
            la_Temp.Parent.Top = top;
            //la_TopPos.Text = tb_Top.Text;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Top", top);
        }

        private void tb_Width_Leave(object sender, EventArgs e)
        {
            int.TryParse(tb_Width.Text, out int width);
            la_Temp.Parent.Width = width;
            //la_WidthValue.Text = tb_Width.Text;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Width", width);
        }

        private void tb_Height_Leave(object sender, EventArgs e)
        {
            int.TryParse(tb_Height.Text, out int height);
            la_Temp.Parent.Height = height;
            //la_HeightValue.Text = tb_Height.Text;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Height", height);
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            la_Temp.Parent.Visible = checkBox1.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name, checkBox1.Checked ? 1 : 0);
        }

        private void Fo_Layout_Load(object sender, EventArgs e)
        {
            
            checkBox1.Checked = la_Temp.Visible;
            tb_Left.Text = la_Temp.Parent.Left.ToString();
            tb_Top.Text = la_Temp.Parent.Top.ToString();
            tb_Width.Text = la_Temp.Parent.Width.ToString();
            tb_Height.Text = la_Temp.Parent.Height.ToString();
            ThrMain = new Thread(Execute);
            ThrMain.Start();
        }

        private void ch_Grid_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = ch_Grid.Checked;
            Units.Fo_Main.bGridMode = ch_Grid.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;

            if (Control.MouseButtons != MouseButtons.Left)
            {
                pa.Left = (int)Math.Round(pa.Left / 8.0) * 8;
                pa.Top = (int)Math.Round(pa.Top / 8.0) * 8;
            }
        }

        private void btn_LeftDec_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int left = pa.Left - (ch_Grid.Checked ? 8 : 1);
            pa.Left = left;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Left", left);
            tb_Left.Text = left.ToString();
        }

        private void btn_LeftInc_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int left = pa.Left + (ch_Grid.Checked ? 8 : 1);
            pa.Left = left;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Left", left);
            tb_Left.Text = left.ToString();
        }
        private void btn_TopDec_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int top = pa.Top - (ch_Grid.Checked ? 8 : 1);
            pa.Top = top;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Top", top);
            tb_Top.Text = top.ToString();
        }

        private void btn_TopInc_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int top = pa.Top + (ch_Grid.Checked ? 8 : 1);
            pa.Top = top;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Top", top);
            tb_Top.Text = top.ToString();
        }
        private void btn_WidthDec_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int width = pa.Width - (ch_Grid.Checked ? 8 : 1);
            pa.Width = width;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Width", width);
            tb_Width.Text = width.ToString();
        }
        private void btn_WidthInc_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int width = pa.Width + (ch_Grid.Checked ? 8 : 1);
            pa.Width = width;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Width", width);
            tb_Width.Text = width.ToString();
        }
        private void btn_HeightDec_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int height = pa.Height - (ch_Grid.Checked ? 8 : 1);
            pa.Height = height;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Height", height);
            tb_Height.Text = height.ToString();
        }

        private void btn_HeightInc_Click(object sender, EventArgs e)
        {
            Panel pa = la_Temp.Parent as Panel;
            if (pa == null) return;
            int height = pa.Height + (ch_Grid.Checked ? 8 : 1);
            pa.Height = height;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", la_Temp.Parent.Name + "_Height", height);
            tb_Height.Text = height.ToString();
        }


    }
}
