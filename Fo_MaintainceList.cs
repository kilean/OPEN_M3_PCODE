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
    public partial class Fo_MaintainceList : Form
    {
        public Fo_MaintainceList()
        {
            InitializeComponent();
        }

        private void OnChecked(object sender, EventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            if (ch == null) return;
            Panel pa = ch.Tag as Panel;
            if (pa == null) return;

            pa.Visible = ch.Checked;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("UI", pa.Name, ch.Checked ? 1 : 0);
        }

        private void Fo_MaintainceList_Load(object sender, EventArgs e)
        {
            ch_Language.Tag = Units.Fo_Main.pa_Language;
            ch_ProcessParam.Tag = Units.Fo_Main.pa_ProcessParam;
            ch_ScreenDisplay.Tag = Units.Fo_Main.pa_ScreenDisplay;
            ch_Balance.Tag = Units.Fo_Main.pa_Balance;
            ch_Position.Tag = Units.Fo_Main.pa_PositionSet;
            ch_Runin.Tag = Units.Fo_Main.pa_RunSpindle;
            ch_NCProg.Tag = Units.Fo_Main.pa_ImportProg;
            ch_Warmup.Tag = Units.Fo_Main.pa_Warmup;
            ch_Door.Tag = Units.Fo_Main.pa_MaintainDoor;
            ch_FuncSwitch.Tag = Units.Fo_Main.pa_FuncSW;
            ch_CNCDataManager.Tag = Units.Fo_Main.pa_CNCDataManager;
            ch_GWRPS.Tag = Units.Fo_Main.pa_GWRPS;
            ch_Rotation_Pos_Setting.Tag = Units.Fo_Main.pa_RotationCenterOffset;
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

        private void Fo_MaintainceList_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
