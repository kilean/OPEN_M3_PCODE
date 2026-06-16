using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Security.Cryptography;
using System.IO;

namespace OIG
{
    public partial class Fo_Balance : Form
    {
        Thread ThrShift;
        Thread ThrShift2;

        public bool Reverse = false;

        public Thread ThrMain = null;

        public Fo_Balance()
        {
            InitializeComponent();

            ThrMain = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);

                    this.Invoke(new Action(() =>
                    {
                        Units.Fo_Main.focas.PMC_ReadByte(PmcAddrType.E, 2823, out byte E2823);
                        btn_GW.Lamp = E2823.BIT_0(); //GW
                    }));
                }
            });
            ThrMain.Start();

            btn_GW.MouseDown += new MouseEventHandler(Units.Fo_Main.PB_MouseDown);
            btn_GW.MouseUp += new MouseEventHandler(Units.Fo_Main.PB_MouseUp);

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            Reverse = ini.ReadInteger("Balance", "Reverse", 0) == 1;

            this.LoadLanguageFile(Units.langfile, this.Name);

            tc_Balance.SizeMode = TabSizeMode.Fixed;
            tc_Balance.Appearance = TabAppearance.FlatButtons;
            tc_Balance.ItemSize = new Size(0, 1);

            tc_Balance.SelectedTab = tab_Balance_Step1;

            pa_Msg.Width = 296;
            pa_Msg.Height = 168;
            pa_Msg.Left = 336;
            pa_Msg.Top = 288;
            pa_Msg.Visible = false;

            new Thread(() =>
            {

                Thread.Sleep(1000);
                this.Invoke(new Action(() =>
                {
                    timer1.Enabled = true;
                }));
            }).Start();
        }

        private void Execute()
        {
            Thread.Sleep(1000);

            int RepeatCount = 1;
            for (int j = 0; j < RepeatCount; j++)
            {
                Thread.Sleep(500);

                this.Invoke((Action)(() =>
                {
                    uc_Rotor2.RotorColor1 = Color.Red;
                }));

                Thread.Sleep(500);

                this.Invoke((Action)(() =>
                {
                    if (Reverse)
                    {
                        uc_Rotor2.RotorAngle1 = 360 - 0;
                    }
                    else
                    {
                        uc_Rotor2.RotorAngle1 = 0;
                    }
                }));

                Thread.Sleep(500);
                for (int i = 0; i < 30; i++)
                {
                    Thread.Sleep(10);
                    this.Invoke((Action)(() =>
                    {
                        if (Reverse)
                        {
                            uc_Rotor2.RotorAngle1 = 360 - i;
                        }
                        else
                        {
                            uc_Rotor2.RotorAngle1 = i;
                        }
                    }));

                }

                Thread.Sleep(500);

                this.Invoke((Action)(() =>
                {
                    uc_Rotor2.RotorColor1 = Color.Gray;
                }));

            }
        }

        private void Execute2()
        {

            int a1 = (int)Math.Round(Units.BalanceAngle1);
            int a2 = (int)Math.Round(Units.BalanceAngle2);
            int a3 = (int)Math.Round(Units.BalanceAngle3);

            Thread.Sleep(1000);

            int RepeatCount = 1;
            for (int j = 0; j < RepeatCount; j++)
            {

                this.Invoke((Action)(() =>
                {
                    if (Reverse)
                    {
                        uc_Rotor3.RotorAngle1 = 360 - 30;
                        uc_Rotor3.RotorAngle2 = 360 - 120;
                        uc_Rotor3.RotorAngle3 = 360 - 240;
                    }
                    else
                    {
                        uc_Rotor3.RotorAngle1 = 30;
                        uc_Rotor3.RotorAngle2 = 120;
                        uc_Rotor3.RotorAngle3 = 240;
                    }
                    uc_Rotor3.RotorColor1 = Color.Red;
                }));

                Thread.Sleep(500);

                int step = a1 - 30;
                int end = Math.Abs(step);
                for (int i = 0; i < end; i++)
                {
                    Thread.Sleep(20);
                    this.Invoke((Action)(() =>
                    {
                        uc_Rotor3.RotorAngle1 += step > 0 ? -1 : 1;
                    }));
                }

                this.Invoke((Action)(() =>
                {
                    uc_Rotor3.RotorColor1 = Color.Gray;
                    uc_Rotor3.RotorColor2 = Color.Red;
                }));

                Thread.Sleep(500);
                step = a2 - 120;
                end = Math.Abs(step);
                for (int i = 0; i < end; i++)
                {
                    Thread.Sleep(20);
                    this.Invoke((Action)(() =>
                    {
                        uc_Rotor3.RotorAngle2 += step > 0 ? -1 : 1;
                    }));
                }

                this.Invoke((Action)(() =>
                {
                    uc_Rotor3.RotorColor2 = Color.Gray;
                    uc_Rotor3.RotorColor3 = Color.Red;
                }));

                Thread.Sleep(500);
                step = a3 - 240;
                end = Math.Abs(step);
                for (int i = 0; i < end; i++)
                {
                    Thread.Sleep(20);
                    this.Invoke((Action)(() =>
                    {
                        uc_Rotor3.RotorAngle3 += step > 0 ? -1 : 1;
                    }));
                }
                Thread.Sleep(500);

                this.Invoke((Action)(() =>
                {
                    uc_Rotor3.RotorColor3 = Color.Gray;
                }));

                Thread.Sleep(500);

            }
        }


        private void TextBoxClick(object sender, EventArgs e)
        {

            TextBox box = (TextBox)sender;
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";   
            Fo_Num form = new Fo_Num();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", box.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

                //目前砂輪號
                Units.Fo_Main.focas.ReadMacro(506, out double no);

                SerialDevice dev = null;     

                if (no == 1) dev = Units.Fo_Main.Gw1;
                else if (no == 2) dev = Units.Fo_Main.Gw2;
                

                if (dev == null)
                {
                    Fo_Msg.Show("GW Number Error.");
                    return;
                }


                form.uc_UserNum1.la_Msg.Text += "\r\n" + dev.MinRpm + " ~ " + dev.MaxRpm; //顯示 動平衡參數 的上下限


            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                box.Text = data.ToString();
            }
        }

        private void btn_Prev_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Fo_Balance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ThrShift != null) ThrShift.Abort();
            if (ThrShift2 != null) ThrShift2.Abort();

            if (ThrMain != null) ThrMain.Abort();
        }



        private void btn_Balance_Step1_Next_Click(object sender, EventArgs e)
        {
            if (tb_Balance_Step1_RPM.Text == "")
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 85, "資料不可為空值"));
                return;
            }

            if (!btn_GW.Lamp)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 109, "請先啟動砂輪"));
                return;
            }

            if (Reverse)
            {
                la_Balance_Step2_AngleA.Text = "[330.0] " + LanguageManager.LoadMessage(Units.langfile, "Message", 113, "度");
            }
            else
            {
                la_Balance_Step2_AngleA.Text = "[30.0] " + LanguageManager.LoadMessage(Units.langfile, "Message", 113, "度");
            }

            String str_rpm = ((int)Math.Round(double.Parse(tb_Balance_Step1_RPM.Text) * 10)).ToString("X4");
            Units.BalanceAngle1 = 0.0;
            Units.BalanceAngle2 = 120.0;
            Units.BalanceAngle3 = 240.0;
            int a1 = 0;
            int a2 = 1200;
            int a3 = 2400;

            int Slave = Units.Fo_Main.BalanceSlave;

            //傳送 Initial & 轉速 & 角度 指令
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_InitialRun.ToString("X4") + "00050A" + str_rpm + a1.ToString("X4") + a2.ToString("X4") + a3.ToString("X4") + "0001");

            btn_Balance_Step1_Abort.Visible = false;
            btn_Balance_Step1_Next.Visible = false;

            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
            pa_Msg.Visible = true;

            new Thread(() =>
            {

                Thread.Sleep(1000);
                int index = 0;
                //等待狀態從1 → 2
                int iStart = Environment.TickCount;
                bool bExit = false;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "..";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "...";

                        index = (index + 1) % 5;
                    }));

                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Busy") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Step1_Abort.Visible = true;
                            btn_Balance_Step1_Next.Visible = true;
                            pa_Msg.Visible = false;
                        }));

                        bExit = true;
                    }
                }



                //等待狀態從2 → 1
                iStart = Environment.TickCount;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "....";
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".....";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "......";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".......";

                        index = (index + 1) % 5;
                    }));
                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Ready") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Step1_Abort.Visible = true;
                            btn_Balance_Step1_Next.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }

                Thread.Sleep(1000);

                if (!bExit)
                {
                    this.Invoke(new Action(() =>
                    {
                        pa_Msg.Visible = false;
                        tc_Balance.SelectedTab = tab_Balance_Step2;
                        ThrShift = new Thread(Execute);
                        ThrShift.Start();
                    }));
                }

                this.Invoke(new Action(() =>
                {
                    Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_ModeStatus.ToString("X4") + "0001020001");
                }));

            }).Start();



        }

        private void btn_Balance_Step2_Next_Click(object sender, EventArgs e)
        {
            if (!btn_GW.Lamp)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 109, "請先啟動砂輪"));
                return;
            }

            int Slave = Units.Fo_Main.BalanceSlave;

            //Trial Run & 試重角度
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_TrialRun.ToString("X4") + "000102012C");

            if (Reverse)
            {
                la_Balance_Step3_A.Text = "A : [330.0] " + LanguageManager.LoadMessage(Units.langfile, "Message", 110, "度");
            }
            else
            {
                la_Balance_Step3_A.Text = "A : [30.0] " + LanguageManager.LoadMessage(Units.langfile, "Message", 110, "度");
            }

            btn_Balance_Step2_Abort.Visible = false;
            btn_Balance_Step2_Next.Visible = false;

            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
            pa_Msg.Visible = true;

            new Thread(() =>
            {

                Thread.Sleep(1000);
                int index = 0;
                //等待狀態從1 → 2
                int iStart = Environment.TickCount;
                bool bExit = false;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "..";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "...";

                        index = (index + 1) % 5;
                    }));

                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Busy") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Step2_Abort.Visible = true;
                            btn_Balance_Step2_Next.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }



                //等待狀態從2 → 1
                iStart = Environment.TickCount;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "....";
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".....";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "......";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".......";

                        index = (index + 1) % 5;
                    }));
                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Ready") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Step2_Abort.Visible = true;
                            btn_Balance_Step2_Next.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }

                Thread.Sleep(1000);

                if (!bExit)
                {
                    this.Invoke(new Action(() =>
                    {
                        pa_Msg.Visible = false;
                        if (Reverse)
                        {
                            la_Balance_Step3_AngleA.Text = "A: [" + (360 - Units.BalanceAngle1).ToString("0.0") + "]";
                            la_Balance_Step3_AngleB.Text = "B: [" + (360 - Units.BalanceAngle2).ToString("0.0") + "]";
                            la_Balance_Step3_AngleC.Text = "C: [" + (360 - Units.BalanceAngle3).ToString("0.0") + "]";

                            FinalAngle1 = 360 - Units.BalanceAngle1;
                            FinalAngle2 = 360 - Units.BalanceAngle2;
                            FinalAngle3 = 360 - Units.BalanceAngle3;
                        }
                        else
                        {
                            la_Balance_Step3_AngleA.Text = "A: [" + (Units.BalanceAngle1).ToString("0.0") + "]";
                            la_Balance_Step3_AngleB.Text = "B: [" + (Units.BalanceAngle2).ToString("0.0") + "]";
                            la_Balance_Step3_AngleC.Text = "C: [" + (Units.BalanceAngle3).ToString("0.0") + "]";

                            FinalAngle1 = Units.BalanceAngle1;
                            FinalAngle2 = Units.BalanceAngle2;
                            FinalAngle3 = Units.BalanceAngle3;
                        }
                        tc_Balance.SelectedTab = tab_Balance_Step3;
                        ThrShift2 = new Thread(Execute2);
                        ThrShift2.Start();
                    }));
                }

                this.Invoke(new Action(() =>
                {
                    Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_ModeStatus.ToString("X4") + "0001020001");
                }));


            }).Start();

        }

        private void btn_Balance_Step1_Abort_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.RefleshQueryList();
            this.Close();
        }

        private void btn_Balance_Step2_Abort_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.RefleshQueryList();
            this.Close();
        }

        private void btn_Balance_Step3_Abort_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.RefleshQueryList();
            this.Close();
        }

        double FinalAngle1;
        double FinalAngle2;
        double FinalAngle3;

        private void btn_Balance_Step3_Next_Click(object sender, EventArgs e)
        {
            if (!btn_GW.Lamp)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 109, "請先啟動砂輪"));
                return;
            }

            int Slave = Units.Fo_Main.BalanceSlave;

            //Residual Run
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_ResidualRun.ToString("X4") + "0001020000");

            btn_Balance_Step3_Abort.Visible = false;
            btn_Balance_Step3_Next.Visible = false;

            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
            pa_Msg.Visible = true;




            new Thread(() =>
            {

                Thread.Sleep(1000);
                int index = 0;
                //等待狀態從1 → 2
                int iStart = Environment.TickCount;
                bool bExit = false;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "..";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "...";

                        index = (index + 1) % 5;
                    }));

                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Busy") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Step3_Abort.Visible = true;
                            btn_Balance_Step3_Next.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }



                //等待狀態從2 → 1
                iStart = Environment.TickCount;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "....";
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".....";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "......";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".......";

                        index = (index + 1) % 5;
                    }));
                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Ready") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Step3_Abort.Visible = true;
                            btn_Balance_Step3_Next.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }

                Thread.Sleep(1000);

                if (!bExit)
                {
                    this.Invoke(new Action(() =>
                    {
                        pa_Msg.Visible = false;

                        la_Balance_Offset_A.Text = "A : [" + FinalAngle1 + "]";
                        la_Balance_Offset_B.Text = "B : [" + FinalAngle2 + "]";
                        la_Balance_Offset_C.Text = "C : [" + FinalAngle3 + "]";

                        FinalAngle1 -= Units.BalanceAngle1;
                        if (FinalAngle1 < 0) FinalAngle1 += 360;
                        FinalAngle2 -= Units.BalanceAngle2;
                        if (FinalAngle2 < 0) FinalAngle2 += 360;
                        FinalAngle3 -= Units.BalanceAngle3;
                        if (FinalAngle3 < 0) FinalAngle3 += 360;

                        la_Balance_Offset_AngleA.Text = "A : [" + FinalAngle1.ToString("0.0") + "]";
                        la_Balance_Offset_AngleB.Text = "B : [" + FinalAngle2.ToString("0.0") + "]";
                        la_Balance_Offset_AngleC.Text = "C : [" + FinalAngle3.ToString("0.0") + "]";

                        tc_Balance.SelectedTab = tab_Balance_Offset;
                        //ThrShift2 = new Thread(Execute2);
                        //ThrShift2.Start();
                    }));
                }

                this.Invoke(new Action(() =>
                {
                    Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_ModeStatus.ToString("X4") + "0001020001");
                }));

            }).Start();
        }

        private void btn_Balance_Offset_Finish_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.RefleshQueryList();
            this.Close();
        }

        private void btn_Balance_Offset_Start_Click(object sender, EventArgs e)
        {
            if (!btn_GW.Lamp)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 109, "請先啟動砂輪"));
                return;
            }

            int Slave = Units.Fo_Main.BalanceSlave;

            //Residual Run
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_ResidualRun.ToString("X4") + "0001020000");

            btn_Balance_Offset_Finish.Visible = false;
            btn_Balance_Offset_Start.Visible = false;

            new Thread(() =>
            {

                Thread.Sleep(1000);
                int index = 0;
                //等待狀態從1 → 2
                int iStart = Environment.TickCount;
                bool bExit = false;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中");
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "..";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "...";

                        index = (index + 1) % 5;
                    }));

                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Busy") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Offset_Finish.Visible = true;
                            btn_Balance_Offset_Start.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }



                //等待狀態從2 → 1
                iStart = Environment.TickCount;
                while (!bExit)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (index == 0)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "....";
                        else if (index == 1)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".....";
                        else if (index == 2)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + "......";
                        else if (index == 3)
                            la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 99, "運算中") + ".......";

                        index = (index + 1) % 5;
                    }));
                    Thread.Sleep(500);
                    if (Units.BalanceStatus == "Ready") break;

                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 30000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 100, "運轉超時"));
                            btn_Balance_Offset_Finish.Visible = true;
                            btn_Balance_Offset_Start.Visible = true;
                            pa_Msg.Visible = false;
                        }));
                        bExit = true;
                    }
                }

                Thread.Sleep(1000);

                if (!bExit)
                {
                    this.Invoke(new Action(() =>
                    {
                        pa_Msg.Visible = false;
                        btn_Balance_Offset_Finish.Visible = true;
                        btn_Balance_Offset_Start.Visible = true;

                        la_Balance_Offset_A.Text = "A : [" + FinalAngle1 + "]";
                        la_Balance_Offset_B.Text = "B : [" + FinalAngle2 + "]";
                        la_Balance_Offset_C.Text = "C : [" + FinalAngle3 + "]";

                        FinalAngle1 -= Units.BalanceAngle1;
                        if (FinalAngle1 < 0) FinalAngle1 += 360;
                        FinalAngle2 -= Units.BalanceAngle2;
                        if (FinalAngle2 < 0) FinalAngle2 += 360;
                        FinalAngle3 -= Units.BalanceAngle3;
                        if (FinalAngle3 < 0) FinalAngle3 += 360;

                        la_Balance_Offset_AngleA.Text = "A : [" + FinalAngle1.ToString("0.0") + "]";
                        la_Balance_Offset_AngleB.Text = "B : [" + FinalAngle2.ToString("0.0") + "]";
                        la_Balance_Offset_AngleC.Text = "C : [" + FinalAngle3.ToString("0.0") + "]";
                    }));
                }

            }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            la_StatusVal.Text = Units.BalanceStatus.ToString();
            la_ErrorCodeVal.Text = Units.BalanceError.ToString("X4");
            la_StepVal.Text = Units.BalanceStep.ToString();
            la_AngleVal.Text = "[" + Units.BalanceAngle1.ToString("0.0") + "][" + Units.BalanceAngle2.ToString("0.0") + "][" + Units.BalanceAngle3.ToString("0.0") + "]";
            la_TrialAngleVal.Text = Units.BalanceTrialAngle.ToString();
            la_umVal.Text = Units.BalanceVibration_um.ToString("0.00");
            la_GVal.Text = Units.BalanceVibration_G.ToString("0.00");
            la_ModeVal.Text = Units.BalanceMode.ToString();
            la_RPMVal.Text = Units.BalanceRPM.ToString("0.0");
            la_NarrowVal.Text = (Units.BalanceVibration1_um / 10).ToString("0.0");
        }

        private void Fo_Balance_Shown(object sender, EventArgs e)
        {

        }

        private void btn_ClearError_Click(object sender, EventArgs e)
        {
            int Slave = Units.Fo_Main.BalanceSlave;
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_Error.ToString("X4") + "0001020000");
        }

        private void btn_SetMaster_Click(object sender, EventArgs e)
        {
            int Slave = Units.Fo_Main.BalanceSlave;
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_ModeStatus.ToString("X4") + "0001020001");
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            int Slave = Units.Fo_Main.BalanceSlave;
            Units.Fo_Main.masterSerialBus1.Add(Slave.ToString("X2") + "10" + Units.BA_Abort.ToString("X4") + "0001020000");
        }

        private void tb_Balance_Step1_RPM_TextChanged(object sender, EventArgs e)
        {
            //Fo_Num form = new Fo_Num();


            //目前砂輪號
            Units.Fo_Main.focas.ReadMacro(506, out double no);

            //顯示並等待結果
            //if (form.ShowDialog() == DialogResult.OK)
            //{

            SerialDevice dev = null;

            if (no == 1) dev = Units.Fo_Main.Gw1;
            else if (no == 2) dev = Units.Fo_Main.Gw2;


            if (dev == null)
            {
                Fo_Msg.Show("GW Number Error.");
                return;
            }
            double.TryParse(tb_Balance_Step1_RPM.Text, out double dVal);
            if (dVal < dev.MinRpm) dVal = dev.MinRpm;
            if (dVal > dev.MaxRpm) dVal = dev.MaxRpm;

            //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
            dev.CmdSpeed = dVal / dev.Rate;

            if (no == 1) Units.Fo_Main.btn_Gw1CmdRpm.DisplayText = dVal.ToString("0");
            else if (no == 2) Units.Fo_Main.btn_Gw2CmdRpm.DisplayText = dVal.ToString("0");
            
            tb_Balance_Step1_RPM.Text = dVal.ToString("0");

            //傳送指令到變頻器
            Units.Fo_Main.masterSerialBus1.Add(dev.Slave.ToString("X2") + "061009" + ((int)Math.Round(dev.CmdSpeed / dev.Unit)).ToString("X4"));

            //}
        }
    }
}
