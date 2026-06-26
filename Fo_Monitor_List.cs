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
    public partial class Fo_Monitor_List : Form
    {
        public Fo_Monitor_List()
        {
            InitializeComponent();
        }

        private void ch_Absolute_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_MonitorAbs.Visible = ch_Absolute.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_MonitorAbs.Name, ch_Absolute.Checked ? 1 : 0);
        }

        private void ch_Distance_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_MonitorDistToGo.Visible = ch_Distance.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_MonitorDistToGo.Name, ch_Distance.Checked ? 1 : 0);
        }

        private void ch_Machine_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_MonitorMach.Visible = ch_Machine.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_MonitorMach.Name, ch_Machine.Checked ? 1 : 0);
        }

        private void ch_Relative_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_DC.Visible = ch_Relative.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_DC.Name, ch_Relative.Checked ? 1 : 0);
        }

        private void ch_Program_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_Prog.Visible = ch_Program.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_Prog.Name, ch_Program.Checked ? 1 : 0);
        }

        private void ch_WorkInfo_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_Info.Visible = ch_WorkInfo.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_Info.Name, ch_WorkInfo.Checked ? 1 : 0);
        }

        private void ch_GW1_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_GW1.Visible = ch_GW1.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_GW1.Name, ch_GW1.Checked ? 1 : 0);
        }

        private void ch_GW2_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_GW2.Visible = ch_GW2.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_GW2.Name, ch_GW2.Checked ? 1 : 0);
        }

        private void ch_GW3_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_GW3.Visible = ch_GW3.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_GW3.Name, ch_GW3.Checked ? 1 : 0);
        }

        private void ch_GW4_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Monitor_GW4.Visible = ch_GW4.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Monitor_GW4.Name, ch_GW4.Checked ? 1 : 0);
        }

        private void ch_SpindleComm_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Spindle.Visible = ch_SpindleComm.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Spindle.Name, ch_SpindleComm.Checked ? 1 : 0);
        }



        private void Fo_Monitor_List_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        private void ch_SpindleCAxis_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Spindle3.Visible = ch_SpindleCAxis.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Spindle3.Name, ch_SpindleCAxis.Checked ? 1 : 0);
        }

        private void ch_Roller_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Roller.Visible = ch_Roller.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Roller.Name, ch_Roller.Checked ? 1 : 0);
        }

        private void ch_SpindleRate_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_Spindle2.Visible = ch_SpindleRate.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Spindle2.Name, ch_SpindleRate.Checked ? 1 : 0);
        }

        private void ch_GrindMode_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.pa_GrindInfo.Visible = ch_SpindleRate.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", Units.Fo_Main.pa_Spindle2.Name, ch_SpindleRate.Checked ? 1 : 0);
        }

        private void Fo_Monitor_List_Load(object sender, EventArgs e)
        {
            ch_Absolute.Tag = Units.Fo_Main.pa_MonitorAbs;
            ch_Distance.Tag = Units.Fo_Main.pa_MonitorDistToGo;
            ch_Machine.Tag = Units.Fo_Main.pa_MonitorMach;
            ch_Relative.Tag = Units.Fo_Main.pa_Monitor_DC;
            ch_WorkInfo.Tag = Units.Fo_Main.pa_Monitor_Info;
            ch_GrindMode.Tag = Units.Fo_Main.pa_GrindInfo;
            ch_GW1.Tag = Units.Fo_Main.pa_Monitor_GW1;
            ch_GW2.Tag = Units.Fo_Main.pa_Monitor_GW2;
            ch_GW3.Tag = Units.Fo_Main.pa_Monitor_GW3;
            ch_GW4.Tag = Units.Fo_Main.pa_Monitor_GW4;
            ch_Program.Tag = Units.Fo_Main.pa_Monitor_Prog;
            ch_SpindleComm.Tag = Units.Fo_Main.pa_Spindle;
            ch_SpindleRate.Tag = Units.Fo_Main.pa_Spindle2;
            ch_SpindleCAxis.Tag = Units.Fo_Main.pa_Spindle3;
            ch_Roller.Tag = Units.Fo_Main.pa_Roller;

            if (this.Visible)
            {
                foreach (var obj in this.Controls)
                {
                    CheckBox ch = obj as CheckBox;
                    if (ch == null) continue;
                    Panel pa = ch.Tag as Panel;
                    if (pa == null) continue;

                    ch.Checked = pa.Visible;
                }
            }
        }
    }
}
