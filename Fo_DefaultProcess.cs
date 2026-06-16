using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
namespace OIG
{
    public partial class Fo_DefaultProcess : Form
    {
        private List<int> ProcessID = new List<int>();
        XDocument xmlDefaultProcess = new XDocument();//DefaultProcess.xml  

        public Fo_DefaultProcess()
        {
            InitializeComponent();
        }

        private void Fo_DefaultProcess_Load(object sender, EventArgs e)
        {
            this.LoadLanguageFile(Units.langfile, this.Name);

            ProcessID.Clear();
            dataGridView1.Rows.Clear();

            //從檔案讀取原始資料(公制)
            string filename = Application.StartupPath + "\\DefaultProcess.xml";
            if (!File.Exists(filename))
            {
                Fo_Msg.Show("File[" + filename + "] Not Found", "");
                return;
            }
            xmlDefaultProcess = XDocument.Load(filename);//讀取XML檔案
            //讀取語言
            Units.Fo_Main.LoadProcessDbName();

            //工序清單
            foreach (var item in Units.ProcessList)
            {
                listBox1.Items.Add(item.Name);//工序名稱
                ProcessID.Add(item.ID);
            }


            // 設置 ListBox 的第一項為選中項
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }

        }


        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //先回存, 在顯示其他工序資料
            Item_save();

            //例外處理
            int index = lbindex = listBox1.SelectedIndex;//紀錄當前選擇
            if (index < 0) return;

            dataGridView1.Rows.Clear();

            TProcess process = Units.ProcessList[index];
            if (process == null) return;
            if (process.SubPrograms == null) return;
            if (process.SubPrograms.Count <= 0) return;

            foreach (var item in process.SubPrograms[0].Arguments)
            {
                dataGridView1.Rows.Add(
                            item.AddrCode.ToString(),
                            item.Name,
                            item.Type.ToString("0"),
                            item.Value.ToString("0.#####"),
                            item.Min.ToString("0.#####"),
                            item.Max.ToString("0.#####"),
                            item.Code,
                            item.Unit);
            }
        }


        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Item_save();
            this.Close();
        }

        int lbindex = 0;
        private void Item_save()
        {
            int index = lbindex;//執行變換前保存前一個工序
            if (index < 0) return;

            TProcess process = Units.ProcessList[index];
            if (process == null) return;
            if (process.SubPrograms == null) return;
            if (process.SubPrograms.Count <= 0) return;

            int id = ProcessID[index];

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string Pcode = row.Cells[0].Value?.ToString();
                int.TryParse(row.Cells[2].Value?.ToString(), out int Page);
                double.TryParse(row.Cells[3].Value?.ToString(), out double DefaultNum);
                double.TryParse(row.Cells[4].Value?.ToString(), out double Min);
                double.TryParse(row.Cells[5].Value?.ToString(), out double Max);
                string Code = row.Cells[6].Value?.ToString();
                if (Code == null) Code = "";
                string Unit = row.Cells[7].Value?.ToString();
                if (Unit == null) Unit = "";

                //系統中的
                var a = process.SubPrograms[0].GetArgument(Pcode);
                if (a == null) continue;
                a.Value = DefaultNum;
                a.Min = Min;
                a.Max = Max;
                a.Code = Code;
                a.Unit = Unit;
                a.Type = Page;

                //DefaultProcess.xml 的
                var processNode = xmlDefaultProcess.Descendants("Process").FirstOrDefault(x => x.Attribute("ID")?.Value == id.ToString());
                if (processNode == null)
                {
                    Fo_Msg.Show("Process ID[" + id + "] Not Found", "");
                    continue;
                }
                var argNode = processNode.Descendants("PCode").FirstOrDefault(x => x.Attribute("PCode")?.Value == a.AddrCode);
                argNode.SetAttributeValue("pages", Page);
                argNode.SetAttributeValue("Min", Min);
                argNode.SetAttributeValue("Max", Max);
                argNode.SetAttributeValue("Show", DefaultNum);
                argNode.SetAttributeValue("Code", Code);
                argNode.SetAttributeValue("Unit", Unit);

            }

            xmlDefaultProcess.Save(Application.StartupPath + "\\DefaultProcess.xml");
        }

        private void Fo_DefaultProcess_FormClosing(object sender, FormClosingEventArgs e)
        {

            Units.Fo_Main.LoadProcessDbName();//預設工序庫讀取名稱

            //公英制判斷
            Units.Fo_Main.focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);
            if (F2.BIT_0())
            {
                foreach (TProcess p in Units.ProcessList)
                {
                    foreach (TSubProgram sp in p.SubPrograms)
                    {
                        foreach (TArgument a in sp.Arguments)
                        {
                            if (a.Unit.Contains("mm"))
                            {
                                a.Max /= 25;
                                a.Min /= 25;
                                a.Default /= 25;
                                a.Value /= 25.4;
                                a.Unit = a.Unit.Replace("mm", "inch");
                            }

                        }
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            //if (Units.Fo_Main.isCellClickDisabled) return;
            //Units.Fo_Main.isCellClickDisabled = true;//輸入時不切換往復功能
            if (e.ColumnIndex != Col_Default.Index && e.ColumnIndex != Col_Min.Index && e.ColumnIndex != Col_Max.Index)
            {
                Fo_Keyboard form = new Fo_Keyboard();
                form.TB_Input.Text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                DialogResult ret = form.ShowDialog();
                //Units.Fo_Main.disableClickTimer.Start();
                if (ret != DialogResult.OK) return;
                if (e.ColumnIndex == Col_Page.Index)
                {
                    double.TryParse(form.TB_Input.Text, out double val);
                    form.TB_Input.Text = val.ToString("0");
                }
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.TB_Input.Text;

            }
            else
            {
                Fo_Num form = new Fo_Num();
                form.uc_UserNum1.la_Msg.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                form.uc_UserNum1.ShowCancelBtn = true;
                DialogResult ret = form.ShowDialog();
                //Units.Fo_Main.disableClickTimer.Start();
                if (ret != DialogResult.OK) return;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.uc_UserNum1.la_Num.Text;
            }
        }


    }
}

