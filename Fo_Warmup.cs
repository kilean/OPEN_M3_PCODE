using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OCD
{
    public partial class Fo_Warmup : Form
    {
        private Thread ThrWarn = null;
        public Fo_Warmup(MachineType MachType)
        {
            InitializeComponent();
            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void btn_WarnStart_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.Actions.Enqueue(new Action(() =>
            {
                Units.Fo_Main.focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
                Units.Fo_Main.focas.WriteMacro(980, 20);
                Units.Fo_Main.OneKeyCall(8999);
            }));

            if (ThrWarn != null)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 148, "程式仍在執行"));
                return;
            }
            double val;
            double.TryParse(btn_WarnX1.DisplayText, out val);
            double.TryParse(btn_WarnX2.DisplayText, out val);
            double.TryParse(btn_WarnZ1.DisplayText, out val);
            double.TryParse(btn_WarnZ2.DisplayText, out val);
            double.TryParse(btn_WarnTimes.DisplayText, out val);

            Units.Fo_Main.Actions.Enqueue(new Action(() =>
            {
                Units.Fo_Main.focas.WriteMacro(820, val);//暖機位置X1
                Units.Fo_Main.focas.WriteMacro(821, val);//暖機位置X2
                Units.Fo_Main.focas.WriteMacro(822, val);//暖機位置Z1
                Units.Fo_Main.focas.WriteMacro(823, val);//暖機位置Z2
                Units.Fo_Main.focas.WriteMacro(826, val);//往復次數
            }));

            //程式結束要回 O8000
            ThrWarn = new Thread(() =>
            {
                Thread.Sleep(3000);//延遲一段時間再判斷, 一鍵呼叫 啟動程式沒那麼快

                while (Units.Fo_Main.bRun) Thread.Sleep(500);//程式執行中

                Units.Fo_Main.Actions.Enqueue((Action)(() =>
                {
                    Units.Fo_Main.focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
                }));
                ThrWarn = null;
            });
            ThrWarn.Start();
        }

        private void btn_WarnUsePosX1_Click(object sender, EventArgs e)
        {
            btn_WarnX1.DisplayText = la_WarnMach_1.Text;
        }

        private void btn_WarnUsePosX2_Click(object sender, EventArgs e)
        {
            btn_WarnX2.DisplayText = la_WarnMach_1.Text;
        }

        private void btn_WarnUsePosZ1_Click(object sender, EventArgs e)
        {
            btn_WarnZ1.DisplayText = la_WarnMach_2.Text;
        }

        private void btn_WarnUsePosZ2_Click(object sender, EventArgs e)
        {
            btn_WarnZ2.DisplayText = la_WarnMach_2.Text;
        }


        private void InputPos(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = sender as Uc_RoundBtn;
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            if (btn == null) return;

            int.TryParse(btn.Tag.ToString(), out int no);

            Fo_Num form = new Fo_Num();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            if (form.ShowDialog() != DialogResult.OK) return;

            double.TryParse(form.uc_UserNum1.la_Num.Text, out double val);
            btn.DisplayText = val.ToString(Units.DisplayFmt);

        }

        private void btn_WarnTimes_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_WarnTimes.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            if (form.ShowDialog() != DialogResult.OK) return;

            double.TryParse(form.uc_UserNum1.la_Num.Text, out double val);
            if (val < 1) val = 1;
            btn_WarnTimes.DisplayText = val.ToString("0");
        }

        private void Fo_Warmup_Load(object sender, EventArgs e)
        {
            if (!Units.Fo_Main.focas.IsConnected()) return;
            Units.Fo_Main.focas.ReadMacro(820, out double Macro820);
            Units.Fo_Main.focas.ReadMacro(821, out double Macro821);
            Units.Fo_Main.focas.ReadMacro(822, out double Macro822);
            Units.Fo_Main.focas.ReadMacro(823, out double Macro823);
            Units.Fo_Main.focas.ReadMacro(826, out double Macro826);

            btn_WarnX1.DisplayText = Macro820.ToString(Units.DisplayFmt);
            btn_WarnX2.DisplayText = Macro821.ToString(Units.DisplayFmt);
            btn_WarnZ1.DisplayText = Macro822.ToString(Units.DisplayFmt);
            btn_WarnZ2.DisplayText = Macro823.ToString(Units.DisplayFmt);
            btn_WarnTimes.DisplayText = Macro826.ToString("0");
        }
    }
}
