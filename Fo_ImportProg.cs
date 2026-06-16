using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace OIG
{
    public partial class Fo_ImportProg : Form
    {
        public Fo_ImportProg()
        {
            InitializeComponent();
        }

        public void RefreshList()
        {
            listView1.Items.Clear();

            ushort hnd = Units.Fo_Main.focas.FlibHndl;


            Units.Fo_Main.focas.ReadAllDir("//CNC_MEM/USER/PATH1/", out string[] files);
            foreach (string file in files)
            {
                string type = file.Substring(0, 2);
                string name = file.Substring(2);

                if (type == "10") //程式
                {
                    //listBox1.Items.Add(name);
                    StringBuilder sb = new StringBuilder(1024);
                    uint start = 1;
                    uint len = 1024;
                    if (Focas1.cnc_rdpdf_line(hnd, "//CNC_MEM/USER/PATH1/" + name, 0, sb, ref start, ref len) == Focas1.EW_OK)
                    {
                        //textBox1.AppendText(sb.ToString().Trim() + "\r\n");
                        name = sb.ToString();
                        int index = name.IndexOf('%');
                        if (index != -1) name = name.Substring(0, index);
                        index = name.IndexOf('\n');
                        if (index != -1) name = name.Substring(0, index);

                        //listBox1.Items.Add(name);

                        int start_index = name.IndexOf('(');
                        int end_index = name.IndexOf(')');


                        string memo = "";
                        if (start_index >= 0 && end_index >= 0)
                        {
                            memo = name.Substring(start_index + 1, end_index - start_index - 1);
                            name = name.Substring(0, start_index);
                        }


                        var item = new ListViewItem(name);
                        if (memo != "") item.SubItems.Add(memo);
                        item.ImageIndex = 0;
                        listView1.Items.Add(item);


                    }
                }
            }
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            RefreshList();
            RefreshDirectory(la_PCPath.Text);
        }

        private void btn_Import_Click(object sender, EventArgs e)
        {
            var result = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 122, "是否要寫入程式"),
                                    "",
                                    MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes) return;

            //ListView 使用並排顯示時無法使用 "選擇多個"
            //因此 Click 時會改變ImageIndex 這邊使用ImageIndex 來判斷哪些是被選擇的
            //計算被選擇的數量
            int count = 0;
            foreach (ListViewItem item in listView2.Items)
            {
                if (item.ImageIndex == 1) count++;
            }

            progressBar1.Maximum = count;
            progressBar1.Value = 0;

            panel1.Visible = true;
            Application.DoEvents();

            int i = 0;
            foreach (ListViewItem item in listView2.Items)
            {
                //沒有勾選就跳過
                if (item.ImageIndex == 0) continue;

                i++;
                int ret;
                progressBar1.Value = i;
                Application.DoEvents();

                string filename = Path.Combine(la_PCPath.Text, item.Text);
                string progname = Path.GetFileNameWithoutExtension(item.Text);
                string[] lines = File.ReadAllLines(filename);

                //判斷是否已經存在
                bool bWrite = true;
                foreach (ListViewItem item2 in listView1.Items)
                {
                    if (item2.Text != progname) continue;

                    //詢問是否要覆蓋
                    var result2 = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 127, "是否要覆蓋") + " [" + progname + "]",
                                                LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                                MessageBoxButtons.YesNo);
                    if (result2 != DialogResult.Yes)
                    {
                        bWrite = false;
                        break;
                    }

                    ret = Units.Fo_Main.focas.DeleteNcProgram("//CNC_MEM/USER/PATH1/" + progname);
                    if (ret != Focas1.EW_OK)
                    {
                        Console.WriteLine("[" + LanguageManager.LoadMessage(Units.langfile, "Message", 123, "刪除失敗") + "]//CNC_MEM/USER/PATH1/" + progname + "\r\n");
                        Application.DoEvents();
                    }
                    else
                    {
                        Console.WriteLine("[" + LanguageManager.LoadMessage(Units.langfile, "Message", 124, "刪除") + "]//CNC_MEM/USER/PATH1/" + progname + "\r\n");
                        Application.DoEvents();
                    }
                    break;
                }

                if (!bWrite) continue;

                ret = Units.Fo_Main.focas.WriteFile(FileType.NC_Program, lines.ToList(), "//CNC_MEM/USER/PATH1/");
                if (ret != Focas1.EW_OK)
                {
                    Console.WriteLine("[" + LanguageManager.LoadMessage(Units.langfile, "Message", 125, "寫入失敗") + "]" + progname + "\r\n");
                    Application.DoEvents();
                }
                else
                {
                    Console.WriteLine("[" + LanguageManager.LoadMessage(Units.langfile, "Message", 126, "寫入") + "]" + progname + "\r\n");
                    Application.DoEvents();
                }

            }

            panel1.Visible = false;
            Application.DoEvents();
            RefreshList();
            Application.DoEvents();

        }

        private void btn_Export_Click(object sender, EventArgs e)
        {
            var result = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 128, "是否要讀出程式"),
                        "",
                        MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes) return;

            //ListView 使用並排顯示時無法使用 "選擇多個"
            //因此 Click 時會改變ImageIndex 這邊使用ImageIndex 來判斷哪些是被選擇的
            //計算被選擇的數量
            int count = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.ImageIndex == 1) count++;
            }

            progressBar1.Maximum = count;
            progressBar1.Value = 0;

            panel1.Visible = true;
            Application.DoEvents();

            int i = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                //沒有勾選就跳過
                if (item.ImageIndex == 0) continue;

                i++;
                int ret;
                progressBar1.Value = i;
                Application.DoEvents();

                //string[] lines = File.ReadAllLines(filename);

                string name = item.Text;
                name = name.Replace("<", "");
                name = name.Replace(">", "");

                //判斷是否已經存在
                bool bRead = true;
                foreach (ListViewItem item2 in listView2.Items)
                {
                    string progname = Path.GetFileNameWithoutExtension(item2.Text);
                    if (name != progname) continue;

                    //詢問是否要覆蓋
                    var result2 = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 127, "是否要覆蓋") + " [" + progname + "]",
                                                LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                                MessageBoxButtons.YesNo);
                    if (result2 != DialogResult.Yes)
                    {
                        bRead = false;
                    }
                    break;
                }

                if (!bRead) continue;

                ret = Units.Fo_Main.focas.ReadFile(FileType.NC_Program, out string[] lines, "//CNC_MEM/USER/PATH1/" + name);
                if (ret != Focas1.EW_OK)
                {
                    Console.WriteLine("[" + LanguageManager.LoadMessage(Units.langfile, "Message", 125, "讀取失敗") + "]" + item.Text + "\r\n");
                    Application.DoEvents();
                    continue;
                }
                else
                {
                    Console.WriteLine("[" + LanguageManager.LoadMessage(Units.langfile, "Message", 126, "讀取") + "]" + item.Text + "\r\n");
                    Application.DoEvents();
                }

                for (int j = 0; j < lines.Length; j++) lines[j] = lines[j].Trim();
                File.WriteAllLines(Path.Combine(la_PCPath.Text, name + ".txt"), lines);
            }

            panel1.Visible = false;
            Application.DoEvents();
            RefreshDirectory(la_PCPath.Text);
            Application.DoEvents();
        }


        private void Fo_ImportProg_FormClosing(object sender, FormClosingEventArgs e)
        {
            Units.Fo_Main.fo_ImportProg = null;
        }

        private void Fo_ImportProg_Load(object sender, EventArgs e)
        {
            RefreshList();
            this.LoadLanguageFile(Units.langfile, this.Name);

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            string directory = ini.ReadString("System", "LastPCPath", Environment.CurrentDirectory);
            RefreshDirectory(directory);

            usbDetector1.Start();
        }

        private void RefreshDirectory(string directory)
        {
            la_PCPath.Text = directory;

            listView2.Items.Clear();

            if (!Directory.Exists(directory)) return;

            string[] files;
            try
            {
                files = Directory.GetFiles(directory);
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteString("System", "LastPCPath", directory);
                la_PCPath.Text = directory;
                files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    ListViewItem item = listView2.Items.Add(name);
                    item.ImageIndex = 0;
                }
            }
            catch
            {
            }

        }

        private void btn_PCPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteString("System", "LastPCPath", folderBrowserDialog1.SelectedPath);

                RefreshDirectory(folderBrowserDialog1.SelectedPath);
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                item.ImageIndex = (item.ImageIndex + 1) % 2;
            }
            listView1.SelectedItems.Clear();
        }

        private void listView2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                item.ImageIndex = (item.ImageIndex + 1) % 2;
            }
            listView2.SelectedItems.Clear();
        }

        private void usbDetector1_OnInserted(object sender, System.Management.EventArrivedEventArgs e)
        {
            try
            {
                RefreshList();
                RefreshDirectory(la_PCPath.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void usbDetector1_OnRemoved(object sender, System.Management.EventArrivedEventArgs e)
        {
            try
            {
                RefreshList();
                RefreshDirectory(la_PCPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
