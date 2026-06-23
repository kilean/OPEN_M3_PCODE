using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OIG
{
    public partial class Fo_ToolSelect : Form
    {
        public Fo_ToolSelect()
        {
            InitializeComponent();

            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void Fo_ToolSelect_Load(object sender, EventArgs e)
        {

            pa_ToolSelect.Dock = DockStyle.Fill;

            //if (Units.Fo_Main.MachType == MachineType.OIG_R_M4) this.Height = 336;
            //else this.Height = 168;
            this.Height = 168;
            if(Units.Fo_Main.GwCount >= 4)
            {
                this.Height = 336;
            }
            PictureBox[] pics = { pic_Gw1, pic_Gw2, pic_Gw3, pic_Gw4 };
            double[] modes = new double[4];
            int GwNo = 0;
            bool bFinish = false;
            Units.Fo_Main.Actions.Enqueue(new Action(() => {
                Units.Fo_Main.focas.ReadMacro(506, out double no);
                GwNo = (int)Math.Round(no);

                for(int i=0; i<4; i++) { 
                    Units.Fo_Main.focas.ReadMacro(10005 + i * 200, out double mode);
                    modes[i] = mode;
                }
                bFinish = true;
            }));
            int iStart = Environment.TickCount;
            while (!bFinish)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 5000)
                {

                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                    return;
                }
                Application.DoEvents();
            }


            for (int i = 0; i < Units.Fo_Main.GwCount; i++)
            {
                string machineTypeName = "OCD";
                if (Units.Fo_Main.GWType[i] == MachineType.OIG) machineTypeName = "OIG";
                else if (Units.Fo_Main.GWType[i] == MachineType.OCD2) machineTypeName = "OCD2";
                else if (Units.Fo_Main.GWType[i] == MachineType.OCD3) machineTypeName = "OCD3";
                int DressMode = (int)Math.Round(modes[i]);
                string filename = Application.StartupPath + $"\\image\\{machineTypeName}\\Shape\\150x150\\Shape" + DressMode + ".png";
                pics[i].Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            }

            la_NoGw.BackColor = GwNo == 0 ? Color.Yellow : Color.Transparent;
            la_Gw1.BackColor = GwNo == 1 ? Color.Yellow : Color.Transparent;
            la_Gw2.BackColor = GwNo == 2 ? Color.Yellow : Color.Transparent;
            la_Gw3.BackColor = GwNo == 3 ? Color.Yellow : Color.Transparent;
            la_Gw4.BackColor = GwNo == 4 ? Color.Yellow : Color.Transparent;
            la_Gw3.Visible = pic_Gw3.Visible = Units.Fo_Main.GwCount == 3;
            la_Gw4.Visible = pic_Gw4.Visible = Units.Fo_Main.GwCount == 4;
            la_NoGw.Visible = pic_NoGw.Visible = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pic_ToolSelect_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic == null) return; //例外處理
            int.TryParse(pic.Tag.ToString(), out int no);//取得要換的砂輪號

            
            Units.Fo_Main.Actions.Enqueue(new Action(() =>
            {
                Units.Fo_Main.focas.WriteMacro(507, no);//要換的砂輪號
                Units.Fo_Main.focas.WriteMacro(980, 5);//換砂輪
                Units.Fo_Main.OneKeyCall(8999);//一鍵呼叫執行程式
            }));
            Close();
        }
    }
}
