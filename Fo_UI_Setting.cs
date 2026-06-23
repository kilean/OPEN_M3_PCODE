using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace OIG
{
    public partial class Fo_UI_Setting : Form
    {
        //工序名稱查找用
        public Dictionary<string, string> DicProcessName = new Dictionary<string, string>();

        MachineSetting machineSetting = new MachineSetting();

        public Fo_UI_Setting()
        {
            InitializeComponent();

            //建立快速查找資料
            XmlDocument doc = new XmlDocument();
            string filename = Application.StartupPath + "\\DefaultProcess.xml";
            if (File.Exists(filename))
            {
                doc.Load(filename);

                //取得根元素(DefaultProcess)
                XmlNode root_x = doc.DocumentElement;

                //開始解析XML檔
                for (int i = 0; i < root_x.ChildNodes.Count; i++)
                {
                    //子節點 (Param) 
                    XmlElement x = (XmlElement)root_x.ChildNodes[i];
                    if (x.Name != "Process") continue;

                    string id = x.GetAttribute("ID");
                    string memo = x.GetAttribute("Memo");
                    if (id != "") DicProcessName.Add(id, memo);
                }
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            machineSetting.SaveToFile(Application.StartupPath + "\\MachineSetting.xml");

            Units.Fo_Main.machineSetting.LoadFromFile(Application.StartupPath + "\\MachineSetting.xml");
            bool bFinish = false;
            Units.Fo_Main.Actions.Enqueue(new Action(() =>
            {
                for (int i = 1; i <= 4; i++)
                {
                    XmlElement xmlGw = machineSetting.GetGw(i);
                    int.TryParse(xmlGw.GetAttribute("GwType"), out int type);
                    Units.Fo_Main.focas.WriteMacro(10004 + (i - 1) * 200, type);
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
            this.Close();
        }


        private void Fo_UI_Setting_Load(object sender, EventArgs e)
        {
            //LoadXmlAndBuildTree();
            machineSetting.LoadFromFile(Application.StartupPath + "\\MachineSetting.xml");

            RefreshTreeView();
            //treeView1.ExpandAll();
            if (treeView1.Nodes.Count > 0) treeView1.Nodes[0].Expand();


        }

        private void RefreshGwNode(TreeNode node, int no)
        {
            XmlElement x = machineSetting.GetGw(no);
            int.TryParse(x.GetAttribute("GwType"), out int GwType);
            node.Text = "砂輪" + no + " : " + (GwType == 0 ? "內圓" : "外圓");
            node.Tag = x;
            node.ImageIndex = 1 + GwType * 10;
            node.SelectedImageIndex = 1 + GwType * 10;
            XmlElement xmlParams = GwType == 0 ? machineSetting.xmlOIG_Param : machineSetting.xmlOCD_Param;
            for (int i = 0; i < 7; i++)
            {
                TreeNode n_shape = i < node.Nodes.Count ? node.Nodes[i] : null;
                XmlElement x_shape = x.GetChildNodeAt(i);
                if (n_shape == null || x_shape == null) continue;//例外處理
                n_shape.Tag = x_shape;
                int.TryParse(x_shape.GetAttribute("DressMode"), out int mode);
                XmlElement xmlShapeDef = xmlParams.GetShape(mode);
                n_shape.Text = "位置" + (i + 1) + " : " + (xmlShapeDef == null || mode == 0 ? "<未設定>" : xmlShapeDef.GetAttribute("Memo"));
                n_shape.ImageIndex = GwType * 10 + mode;
                n_shape.SelectedImageIndex = GwType * 10 + mode;

            }
        }

        private void RefreshParamNode(TreeNode node, XmlElement x_param)
        {
            node.Text = x_param.GetAttribute("Memo");
            node.Tag = x_param;
            for (int i = 0; i < x_param.ChildNodes.Count; i++)
            {
                XmlElement x_ch = (XmlElement)x_param.ChildNodes[i];
                TreeNode n_ch = i < node.Nodes.Count ? node.Nodes[i] : null;
                if (n_ch == null) continue; //例外處理
                n_ch.Tag = x_ch;
                if (x_ch.Name == "ShapeDef")
                {
                    //修整模式
                    int.TryParse(x_ch.GetAttribute("DressMode"), out int mode);
                    n_ch.Text = "修整模式 : " + x_ch.GetAttribute("DressMode") + " (" + (mode == 0 ? "<未設定>" : x_ch.GetAttribute("Memo")) + ")";
                    int index = x_param.Name == "GwType0" ? mode : mode + 10;
                    n_ch.ImageIndex = index;
                    n_ch.SelectedImageIndex = index;
                }
                else if (x_ch.Name == "Coordinate")
                {
                    n_ch.Text = "座標系設定";
                }
            }
        }

        private void RefreshProcessListNode(TreeNode node, XmlElement x_list)
        {
            node.Text = "工序清單";
            node.Tag = x_list;
            if (x_list == null) return;//例外處理
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                TreeNode n = node.Nodes[i];
                XmlElement x = x_list.GetChildNodeAt(i);
                n.Tag = x;
                string id = x.GetAttribute("ID");

                n.Text = "工序" + (i + 1).ToString("00") + " : " + (DicProcessName.ContainsKey(id) ? DicProcessName[id] : "<未設定>");
            }

        }

        private void RefreshTreeView()
        {
            if (treeView1.Nodes.Count == 0) return;
            TreeNode root = treeView1.Nodes[0];
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Name == "N_GW1") RefreshGwNode(node, 1);
                else if (node.Name == "N_GW2") RefreshGwNode(node, 2);
                else if (node.Name == "N_GW3") RefreshGwNode(node, 3);
                else if (node.Name == "N_GW4") RefreshGwNode(node, 4);
                else if (node.Name == "N_GwType0") RefreshParamNode(node, machineSetting.xmlOIG_Param);//內圓                
                else if (node.Name == "N_GwType1") RefreshParamNode(node, machineSetting.xmlOCD_Param);//外圓
                else if (node.Name == "N_ProcessList") RefreshProcessListNode(node, machineSetting.xmlProcessList);
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            dataGridView1.Rows.Clear();

            XmlElement xmlNode = e.Node.Tag as XmlElement;
            if (xmlNode == null) return;//例外處理

            // 顯示屬性
            foreach (XmlAttribute attr in xmlNode.Attributes)
            {
                if (attr.Name == "Memo") continue;
                int rowIndex = dataGridView1.Rows.Add(attr.Name, attr.Value, e.Node);
                dataGridView1.Rows[rowIndex].Tag = attr;   // ⭐存 XmlAttribute
            }

            if (dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[Col_Value.Index];

            if (e.Node.IsExpanded) e.Node.Collapse();
            else e.Node.Expand();
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void un_EditUI_OnBtnOkClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;//例外處理

            if (dataGridView1.CurrentCell.ColumnIndex != dataGridView1.Columns["Col_Value"].Index) return; //例外處理


            double.TryParse(un_EditUI.la_Num.Text, out double data);
            dataGridView1.CurrentCell.Value = data;

            DataGridViewRow row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
            XmlAttribute attr = row.Tag as XmlAttribute;
            if (attr == null) return; //例外處理

            // 回寫 XML
            attr.Value = data.ToString();

            RefreshTreeView();//刷新
        }


    }
}
