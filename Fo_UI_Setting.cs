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

namespace OCD
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
            //bool bFinish = false;
            //Units.Fo_Main.Actions.Enqueue(new Action(() =>
            //{
            //    for (int i = 1; i <= 4; i++)
            //    {
            //        XmlElement xmlGw = machineSetting.GetGw(i);
            //        int.TryParse(xmlGw.GetAttribute("GwType"), out int type);
            //        Units.Fo_Main.focas.WriteMacro(10004 + (i - 1) * 200, type);
            //    }
            //    bFinish = true;
            //}));
            //int iStart = Environment.TickCount;
            //while (!bFinish)
            //{
            //    int iTime = Environment.TickCount - iStart;
            //    if (iTime > 5000)
            //    {

            //        //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
            //        return;
            //    }
            //    Application.DoEvents();
            //}
            this.Close();
        }


        private void Fo_UI_Setting_Load(object sender, EventArgs e)
        {

            
            //LoadXmlAndBuildTree();
            machineSetting.LoadFromFile(Application.StartupPath + "\\MachineSetting.xml");

            RefreshTreeView();
            ////treeView1.ExpandAll();
            //if (treeView1.Nodes.Count > 0) treeView1.Nodes[0].Expand();
                
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

        private void RefreshTreeView(string savedNodeName = "")
        {
            //if (treeView1.Nodes.Count == 0) return;
            //TreeNode root = treeView1.Nodes[0];
            //foreach (TreeNode node in root.Nodes)
            //{
            //    if (node.Name == "N_GW1") RefreshGwNode(node, 1);
            //    else if (node.Name == "N_GW2") RefreshGwNode(node, 2);
            //    else if (node.Name == "N_GW3") RefreshGwNode(node, 3);
            //    else if (node.Name == "N_GW4") RefreshGwNode(node, 4);
            //    else if (node.Name == "N_GwType0") RefreshParamNode(node, machineSetting.xmlOIG_Param);//內圓                
            //    else if (node.Name == "N_GwType1") RefreshParamNode(node, machineSetting.xmlOCD_Param);//外圓
            //    else if (node.Name == "N_ProcessList") RefreshProcessListNode(node, machineSetting.xmlProcessList);
            //}

            
            // 1. 清空 TreeView 舊有節點
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate(); // 暫停重繪，優化效能
            int iSettingImgIndex = 41;
            int iMachineImgIndex = 42;
            int iEditImgIndex = 43;
            try
            {
                // 1. 初始化 XmlDocument 並載入檔案
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(filePath);

                // 2. 取得根節點 <MachineSetting>
                XmlElement root = machineSetting.xmlDoc.DocumentElement;
                if (root == null) return;

                string version = root.GetAttribute("Version");
                if (string.IsNullOrEmpty(version)) version = "未知";

                // 建立根節點
                TreeNode rootNode = new TreeNode($"MacnineSetting");
                rootNode.ImageIndex = iMachineImgIndex;
                rootNode.SelectedImageIndex = iMachineImgIndex;
                treeView1.Nodes.Add(rootNode);

                // ==========================================
                // 1. 解析 GW 砂輪配置群組 (依 GwType 分組)
                // ==========================================
                TreeNode gwConfigRoot = new TreeNode("砂輪配置設定");
                gwConfigRoot.ImageIndex = iSettingImgIndex;
                gwConfigRoot.SelectedImageIndex = iSettingImgIndex;
                rootNode.Nodes.Add(gwConfigRoot);

                // 使用 XPath 找出所有名稱以 GW 開頭且後面帶有數字的節點 (GW1, GW2, GW3, GW4...)
                XmlNodeList gwNodes = root.SelectNodes("*[starts-with(name(), 'GW') and string-length(name()) = 3]");

                // 由於 XmlDocument 沒有 LINQ 的 GroupBy，我們可以用一個 Dictionary 來手動做 GwType 分組
                var gwGroups = new System.Collections.Generic.Dictionary<string, (string Memo, System.Collections.Generic.List<XmlElement> Elements)>();
                int shapeIndex = 0;
                foreach (XmlNode node in gwNodes)
                {
                    if (node is XmlElement gwElem)
                    {
                        string gwType = gwElem.GetAttribute("GwType") ?? "未知";
                        string memo = gwElem.GetAttribute("Memo") ?? "";

                        if (!gwGroups.ContainsKey(gwType))
                        {
                            gwGroups[gwType] = (memo, new System.Collections.Generic.List<XmlElement>());
                        }
                        gwGroups[gwType].Elements.Add(gwElem);
                    }
                }

                // 將分組好的 GW 填入 TreeView
                foreach (var kp in gwGroups)
                {
                    string typeName = "外圓";
                    if (kp.Key == "1") typeName = "內圓";
                    else if (kp.Key == "2") typeName = "外圓正角度";
                    else if (kp.Key == "3") typeName = "外圓負角度";
                    // TreeNode typeNode = new TreeNode($"GwType {kp.Key} : {kp.Value.Memo}");
                    TreeNode typeNode = new TreeNode($"{typeName}");
                    typeNode.ImageIndex = iSettingImgIndex;
                    typeNode.SelectedImageIndex = iSettingImgIndex;
                    gwConfigRoot.Nodes.Add(typeNode);

                    foreach (XmlElement gwElem in kp.Value.Elements)
                    {
                        TreeNode gwNode = new TreeNode(gwElem.Name); // GW1, GW2...
                        gwNode.ImageIndex = iSettingImgIndex;
                        gwNode.SelectedImageIndex = iSettingImgIndex;
                        typeNode.Nodes.Add(gwNode);

                        // 撈出底下的 Shape 子節點
                        XmlNodeList shapeNodes = gwElem.SelectNodes("Shape");
                        
                        foreach (XmlNode shapeNode in shapeNodes)
                        {
                            if (shapeNode is XmlElement shapeElem)
                            {
                                string mode = shapeElem.GetAttribute("DressMode");
                                int iModeImageIndex = 0;
                                int.TryParse(mode, out int iMode);
                                if (iMode > 0)
                                {
                                    if (kp.Key == "0") iModeImageIndex = 10 + iMode;
                                    else if (kp.Key == "0") iModeImageIndex = 10 + iMode;
                                    else if (kp.Key == "1") iModeImageIndex = iMode;
                                    else if (kp.Key == "2") iModeImageIndex = 20 + iMode;
                                    else if (kp.Key == "3") iModeImageIndex = 30 + iMode;

                                }
                                if (!string.IsNullOrEmpty(mode))
                                {
                                    TreeNode dressModeNode = new TreeNode($"修整模式 (DressMode): {mode}", iModeImageIndex, iModeImageIndex);
                                    dressModeNode.Name = $"Shape_{gwElem.Name}_{shapeIndex}";
                                    dressModeNode.Tag = shapeElem;
                                    gwNode.Nodes.Add(dressModeNode);
                                    shapeIndex++;
                                }
                            }
                        }
                    }
                }

                // ==========================================
                // 2. 解析 GwType0 ~ GwType3 參數定義
                // ==========================================
                TreeNode paramDefRoot = new TreeNode("砂輪參數定義");
                paramDefRoot.ImageIndex = iSettingImgIndex;
                paramDefRoot.SelectedImageIndex = iSettingImgIndex;
                rootNode.Nodes.Add(paramDefRoot);

                // 找出所有以 GwType 開頭且長度大於 6 的節點 (排除剛剛的 GW1~4)
                XmlNodeList gwTypeDefs = root.SelectNodes("*[starts-with(name(), 'GwType') and string-length(name()) > 6]");
                int toolIndex = 0;
                foreach (XmlNode typeDefNode in gwTypeDefs)
                {
                    if (typeDefNode is XmlElement typeDefElem)
                    {
                        string typeName = typeDefElem.Name; // GwType0, GwType1...
                        string memo = typeDefElem.GetAttribute("Memo");
                        //TreeNode defNode = new TreeNode($"{typeName} ({memo})");
                        TreeNode defNode = new TreeNode($"{memo}");
                        defNode.ImageIndex = iSettingImgIndex;
                        defNode.SelectedImageIndex = iSettingImgIndex;
                        paramDefRoot.Nodes.Add(defNode);

                        // 解析 ShapeDef
                        XmlNodeList shapeDefs = typeDefElem.SelectNodes("ShapeDef");
                        foreach (XmlNode shapeDefNode in shapeDefs)
                        {
                            if (shapeDefNode is XmlElement shapeDefElem)
                            {
                                string mode = shapeDefElem.GetAttribute("DressMode");
                                string sMemo = shapeDefElem.GetAttribute("Memo");
                                TreeNode shapeDefTreeNode = new TreeNode($"模式 {mode}: {sMemo}");
                                shapeDefTreeNode.ImageIndex = iSettingImgIndex;
                                shapeDefTreeNode.SelectedImageIndex = iSettingImgIndex;
                                //shapeDefTreeNode.Tag = shapeDefElem;
                                defNode.Nodes.Add(shapeDefTreeNode);

                                // 撈出 Param 參數
                                XmlNodeList paramsList = shapeDefElem.SelectNodes("Param");
                                foreach (XmlNode paramNode in paramsList)
                                {
                                    if (paramNode is XmlElement paramElem)
                                    {
                                        string macro = paramElem.GetAttribute("Macro");
                                        string pMemo = paramElem.GetAttribute("Memo");
                                        shapeDefTreeNode.Nodes.Add(new TreeNode($"[巨集 {macro}] {pMemo}"));
                                    }
                                }

                                // 撈出 Tools 參數並加入 TreeView 節點
                                XmlNodeList toolsNodes = shapeDefElem.SelectNodes("Tools");
                                foreach (XmlNode toolsNode in toolsNodes)
                                {
                                    if (toolsNode is XmlElement toolsElem)
                                    {
                                        // 建立一個 "工具設定" 的中間母節點
                                        TreeNode toolsRootNode = new TreeNode("修刀數 (Tools)");
                                        toolsRootNode.ImageIndex = iSettingImgIndex;
                                        toolsRootNode.SelectedImageIndex = iSettingImgIndex;
                                        shapeDefTreeNode.Nodes.Add(toolsRootNode);

                                        // 撈取 Tool3 與 Tool2 (或者用 * 撈取所有子節點)
                                        XmlNodeList toolTypeList = toolsElem.SelectNodes("*");
                                        foreach (XmlNode toolTypeNode in toolTypeList)
                                        {
                                            if (toolTypeNode is XmlElement toolTypeElem)
                                            {
                                                // 建立 Tool3 或 Tool2 節點
                                                TreeNode toolTypeTreeNode = new TreeNode(toolTypeElem.Name); // "Tool3" 或 "Tool2"
                                                toolTypeTreeNode.ImageIndex = iSettingImgIndex;
                                                toolTypeTreeNode.SelectedImageIndex = iSettingImgIndex;
                                                toolsRootNode.Nodes.Add(toolTypeTreeNode);

                                                // 撈取 Tool3/Tool2 底下的 G55, G57, G58 等節點
                                                XmlNodeList gNodes = toolTypeElem.SelectNodes("*");
                                                foreach (XmlNode gNode in gNodes)
                                                {
                                                    if (gNode is XmlElement gElem)
                                                    {
                                                        string gName = gElem.Name; // G55, G57 或 G58
                                                        string visible = gElem.GetAttribute("Visible");
                                                        string image = gElem.GetAttribute("Image");

                                                        // 建立最底層的 Gxx 節點，並把這個 XmlElement 塞進 Tag 供之後點擊時在 DataGridView 編輯
                                                        TreeNode gTreeNode = new TreeNode($"{gName} [使用: {visible}]");
                                                        gTreeNode.ImageIndex = iEditImgIndex;
                                                        gTreeNode.SelectedImageIndex = iEditImgIndex;
                                                        gTreeNode.Tag = gElem; // 【核心】綁定 Tag，點擊此節點就能直接送去 DataGridView
                                                        gTreeNode.Name = $"Tool_{gName}_{toolIndex}";
                                                        toolTypeTreeNode.Nodes.Add(gTreeNode);
                                                        toolIndex++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // ==========================================
                // 3. 解析 MacroLimit 巨集限制
                // ==========================================
                XmlElement macroLimitElem = root.SelectSingleNode("MacroLimit") as XmlElement;
                if (macroLimitElem != null)
                {
                    TreeNode macroRoot = new TreeNode("巨集數值限制 (MacroLimit)");
                    macroRoot.ImageIndex = iSettingImgIndex;
                    macroRoot.SelectedImageIndex = iSettingImgIndex;
                    rootNode.Nodes.Add(macroRoot);

                    XmlNodeList macroNodes = macroLimitElem.SelectNodes("Macro");
                    foreach (XmlNode macroNode in macroNodes)
                    {
                        if (macroNode is XmlElement macroElem)
                        {
                            string no = macroElem.GetAttribute("No");
                            string min = macroElem.GetAttribute("Min");
                            string max = macroElem.GetAttribute("Max");
                            string unit = macroElem.GetAttribute("Unit");
                            string memo = macroElem.GetAttribute("Memo");

                            macroRoot.Nodes.Add(new TreeNode($"No.{no} ({memo}) -> 範圍: {min} ~ {max} {unit}", iEditImgIndex, iEditImgIndex));
                        }
                    }
                }

                // ==========================================
                // 4. 解析 ProcessList 工序列表
                // ==========================================
                XmlElement processListElem = root.SelectSingleNode("ProcessList") as XmlElement;
                if (processListElem != null)
                {
                    string memo = processListElem.GetAttribute("Memo");
                    TreeNode processRoot = new TreeNode($"工序設定列表 ({memo})");
                    processRoot.Tag = processListElem;
                    processRoot.ImageIndex = iSettingImgIndex;
                    processRoot.SelectedImageIndex = iSettingImgIndex;
                    rootNode.Nodes.Add(processRoot);

                    XmlNodeList processNodes = processListElem.SelectNodes("Process");
                    int processIndex = 0;
                    for (int i = 0; i < processNodes.Count; i++)
                    {
                        if (processNodes[i] is XmlElement processElem)
                        {
                            string id = processElem.GetAttribute("ID");
                            if (id != "0") // 排除 ID="0" 的空工序
                            {
                                //processRoot.Nodes.Add(new TreeNode($"位置 {i + 1} ➔ 工序 ID: {id}", iEditImgIndex, iEditImgIndex));
                                TreeNode pNode = new TreeNode($"位置 {i + 1} ➔ 工序 ID: {id}", iEditImgIndex, iEditImgIndex);
                                // 【核心改進】個別的工序也可以綁定（看你的操作習慣，建議點選母節點一次列出比較好操作）
                                pNode.Tag = processElem;
                                pNode.Name = $"{processElem.Name}_{processIndex}";
                                processRoot.Nodes.Add(pNode);
                                processIndex++;
                            }
                        }
                    }
                }

                // 預設全部展開
                //treeView1.ExpandAll();
                //if (treeView1.Nodes.Count > 0) treeView1.Nodes[0].Expand();
                if (!string.IsNullOrEmpty(savedNodeName))
                {
                    TreeNode foundNode = FindNodeByName(treeView1.Nodes, savedNodeName);
                    if (foundNode != null)
                    {
                        treeView1.SelectedNode = foundNode;
                        foundNode.EnsureVisible(); // 自動滾動到視線內
                    }
                    else
                    {
                        // 如果找不到舊路徑，再使用預設行為
                        if (treeView1.Nodes.Count > 0) treeView1.Nodes[0].Expand();
                    }
                }
                else
                {
                    if (treeView1.Nodes.Count > 0) treeView1.Nodes[0].Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                treeView1.EndUpdate(); // 恢復重繪
            }
        }
        
        private TreeNode FindNodeByName(TreeNodeCollection nodes, string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName)) return null;

            foreach (TreeNode node in nodes)
            {
                // 比對 Name 屬性
                if (node.Name == nodeName)
                {
                    return node;
                }

                // 遞迴搜尋子節點
                if (node.Nodes.Count > 0)
                {
                    TreeNode found = FindNodeByName(node.Nodes, nodeName);
                    if (found != null) return found;
                }
            }
            return null;
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            dataGridView1.Rows.Clear();

            XmlElement xmlNode = e.Node.Tag as XmlElement;
            //if (xmlNode == null) return;//例外處理
            
            if (xmlNode != null)
            {
                // 顯示屬性
                foreach (XmlAttribute attr in xmlNode.Attributes)
                {
                    if (attr.Name == "Memo") continue;
                    if (attr.Name == "Image") continue;
                    int rowIndex = dataGridView1.Rows.Add(attr.Name, attr.Value, e.Node);
                    dataGridView1.Rows[rowIndex].Tag = attr;   // ⭐存 XmlAttribute
                }

                if (dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[Col_Value.Index];
                
            }
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

            string savedNodeName = treeView1.SelectedNode?.Name;
            
            RefreshTreeView(savedNodeName);//刷新
        }
    }
}
