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
using System.IO;

namespace OIG
{
    public partial class Fo_Runin : Form
    {
        //自定義跑合程式的檔案
        private RunSpindleFile file;
        //跑合程式的檔案路徑
        private String filename;
        private bool bStart = false;

        public Fo_Runin()
        {
            InitializeComponent();

            this.LoadLanguageFile(Units.langfile, this.Name);

            //檔案路徑
            filename = Application.StartupPath + "\\run.txt";
            //開檔
            file = new RunSpindleFile(filename);

            //加入listBox顯示
            foreach (RunSpindleData data in file.Datas)
            {
                lb_Runin_List.Items.Add(data.ToString());
            }

            //顯示Chart
            RefleshChart();

            //TB_G3_Rpm.Text = Units.Fo_Main.TB_G3_Rpm.Text;
        }

        private void RefleshChart()//顯示Chart
        {
            //清除
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            //啟始
            chart1.Series[0].Points.AddXY(0, 0);
            chart1.Series[1].Points.AddXY(0, 0);

            //計算目前到第幾分鐘
            int mins = 0;
            foreach (RunSpindleData data in file.Datas)
            {

                //分割成每點/每分鐘
                for (int i = 0; i <= data.Time; i++)
                {

                    chart1.Series[0].Points.AddXY(mins, data.Rpm);
                    if (i != data.Time) mins++;
                }
            }
        }

        


        //1秒 = 幾分鐘
        double t_rate = 1 / 60.0;
        int iCurrentSecond = 0;
        int CurrentRpm = 0;
        private void Execute()
        {
            this.Invoke(new Action(() => {
                btn_Runin_Start.Lamp = true;
            }));

            Thread.Sleep(1000);



            //目前執行時間
            int iExeStart = Environment.TickCount;
            //總計分鐘
            int m_count = 0;
            double no=0;
            this.Invoke(new Action(() => { 
                //目前砂輪號
                Units.Fo_Main.focas.ReadMacro(506, out no);
            }));

            SerialDevice dev = null;
            if (no == 1) dev = Units.Fo_Main.Gw1;
            else if (no == 2) dev = Units.Fo_Main.Gw2;
            if (dev == null)
            {
                Fo_Msg.Show("GW Number Error.");
                return;
            }

            foreach (RunSpindleData d in file.Datas)
            {
                CurrentRpm = (int)d.Rpm;



                double dVal = d.Rpm;
                if (dVal < dev.MinRpm) dVal = dev.MinRpm;
                if (dVal > dev.MaxRpm) dVal = dev.MaxRpm;
                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                dev.CmdSpeed = dVal / dev.Rate;
                //傳送指令到變頻器
                Units.Fo_Main.masterSerialBus1.Add(dev.Slave.ToString("X2") + "061009" + ((int)Math.Round(dev.CmdSpeed / dev.Unit)).ToString("X4"));
                
                //總計分鐘
                m_count += (int)d.Time;

                //目標時間(分鐘轉毫秒)
                int iTragetTime = (int)(m_count * 60 * 1000);

                //等待迴圈
                while (true)
                {
                    //目前已經進行的時間(ms)
                    int iTime = Environment.TickCount - iExeStart;
                    //轉成秒數
                    int iExeSecond = iTime / 1000;
                    if (iCurrentSecond != iExeSecond)
                    {
                        iCurrentSecond = iExeSecond;
                        this.Invoke(new Action(() => {
                            double val = 0;
                            if (no == 1) double.TryParse(Units.Fo_Main.la_Gw1CurrentRpm.Text, out val);
                            else if (no == 2) double.TryParse(Units.Fo_Main.la_Gw2CurrentRpm.Text, out val);
                            chart1.Series[1].Points.AddXY(iCurrentSecond * t_rate, val);
                        }));
                    }
                    //超過此速度需要的時間 - 離開回圈
                    if (iTime >= iTragetTime) break;
                    //繼續等待
                    Thread.Sleep(50);
                    //按下停止按鈕 - 離開回圈
                    if (!bStart) break;
                }

                //按下停止按鈕 - 離開程序
                if (!bStart) break;

            }

            this.Invoke(new Action(() => {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 107, "跑合結束"));
                btn_Runin_Start.Lamp = false;
                btn_Runin_Start.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 105, "啟動");
            }));
        }

        private void TextBoxClick(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                box.Text = data.ToString();
            }
        }

        private void Fo_Runin_Paint(object sender, PaintEventArgs e)    
        {
            e.Graphics.DrawImage(Properties.Resources.background2, new Rectangle(0, 0, this.Width, this.Height));
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (bStart)
            {
                bStart = false;
                btn_Runin_Start.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 105, "啟動");
            }
            else
            {
                bStart = true;
                chart1.Series[1].Points.Clear();
                btn_Runin_Start.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 106, "停止");
                Thread thread = new Thread(Execute);
                thread.Start();
            }
        }

        private void btn_CleaarAll_Click(object sender, EventArgs e)
        {
            //清除所有顯示
            lb_Runin_List.Items.Clear();

            //清除檔案
            file.Datas.Clear();
            //存檔
            file.SaveToFile(filename);

            //清除Chart
            chart1.Series[0].Points.Clear();
            //啟始
            chart1.Series[0].Points.AddXY(0, 0);
        }

        private void uc_RoundBtn1_Click(object sender, EventArgs e)
        {
            //判斷選擇哪一個索引
            int index = lb_Runin_List.SelectedIndex;
            //沒有選擇 - 離開
            if (index < 0) return;

            //顯示移除此筆資料
            lb_Runin_List.Items.RemoveAt(index);

            //從檔案移除此筆資料
            file.Datas.RemoveAt(index);
            //存檔
            file.SaveToFile(filename);

            //重新顯示Chart
            RefleshChart();
        }

        private void uc_RoundBtn1_Click_1(object sender, EventArgs e)
        {
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

            uint time = uint.Parse(tb_Runin_Minute.Text);
            if (time < 1) time = 1;
            if (time > 60) time = 60;

            uint rpm = uint.Parse(tb_Runin_RPM.Text);
            if (rpm < dev.MinRpm) rpm = (uint)dev.MinRpm;
            if (rpm > dev.MaxRpm) rpm = (uint)dev.MaxRpm;

            //建立結構 & 資料
            RunSpindleData data = new RunSpindleData(time, rpm);

            //加到顯示
            lb_Runin_List.Items.Add(data.ToString());

            //先計算先前已經加入到Chart的時間總合
            uint mins = 0;
            foreach (RunSpindleData d in file.Datas)
            {
                mins += d.Time;
            }

            //在Chart最後面加入這比程序
            //並且分割成每分鐘
            for (int i = 0; i <= data.Time; i++)
            {
                chart1.Series[0].Points.AddXY(mins, data.Rpm);
                mins++;
            }

            //資料結構加入到 file
            file.Datas.Add(data);
            //存檔
            file.SaveToFile(filename);
        }

        private void btn_Start_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }

    public class RunSpindleFile
    {
        //程序列表
        public List<RunSpindleData> Datas = new List<RunSpindleData>();

        public RunSpindleFile()
        {
        }

        public RunSpindleFile(String FileName)
        {
            LoadFromFile(FileName);
        }

        //讀檔
        public void LoadFromFile(String filename)
        {
            Datas.Clear();

            //檔案不存在 - 離開
            if (!File.Exists(filename)) return;

            //讀取檔案所有行數
            string[] lines = File.ReadAllLines(filename);
            //轉換到結構中
            foreach (String s in lines)
            {
                //解CommaText
                string[] csv = s.Split(',');
                //結構正確
                if (csv.Length == 2)
                {
                    try
                    {
                        uint time = uint.Parse(csv[0]);
                        uint rpm = uint.Parse(csv[1]);
                        Datas.Add(new RunSpindleData(time, rpm));
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        //存檔
        public void SaveToFile(String filename)
        {
            List<string> list = new List<string>();
            foreach (RunSpindleData data in Datas)
            {
                //轉CommaText
                list.Add(data.Time.ToString() + "," + data.Rpm.ToString());
            }
            //輸出至檔案
            File.WriteAllLines(filename, list.ToArray());
        }
    }

    //跑合程序的資料
    public class RunSpindleData
    {
        //屬性
        public uint Time = 0;//時間(分)
        public uint Rpm = 0;//轉速(RPM)

        //建構子
        public RunSpindleData()
        {
        }

        //建構子(時間, 轉速)
        public RunSpindleData(uint time, uint rpm)
        {
            Time = time;//時間(分)
            Rpm = rpm;//轉速(RPM)
        }

        public override String ToString()
        {
            return Rpm.ToString() + "(rpm) " + Time.ToString() + "(min)";
        }
    }
}
