using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OIG
{
    public class MachineSetting
    {
        public XmlDocument xmlDoc = new XmlDocument();

        public string Version;
        
        public XmlElement xmlGw1;
        public XmlElement xmlGw2;
        public XmlElement xmlGw3;
        public XmlElement xmlGw4;
        
        public XmlElement xmlOIG_Param;//內圓砂輪參數
        public XmlElement xmlOCD_Param;//外圓砂輪參數

        public XmlElement xmlOCD_PA_Param;//外圓斜頭正角度砂輪參數
        public XmlElement xmlOCD_NA_Param;//外圓斜頭負角度砂輪參數

        public XmlElement xmlOIG_Coordinate;//內圓座標系
        public XmlElement xmlOCD_Coordinate;//外圓座標系
        
        public XmlElement xmlProcessList;//工序清單

        XmlElement root = null;
        public MachineSetting()
        {
        }

        public XmlElement GetGw(int no)
        {
           
            if (no == 1) return xmlGw1;      
            else if (no == 2) return xmlGw2;              
            else if (no == 3) return xmlGw3;
            else if (no == 4) return xmlGw4;
            
            return null;
        }
        public XmlElement GetGw(int no, int gwType)
        {

            if(root != null)
            {
                string tagName = $"GW{no}";

                // 2. 建立 XPath 查詢語句，意思是：尋找標籤為 GWx 且屬性 GwType 等於指定數字的節點
                // 例如：GW1[@GwType='0']
                string xpath = $"{tagName}[@GwType='{gwType}']";

                // 3. 在 root 底下尋找符合條件的第一個節點
                XmlNode targetNode = root.SelectSingleNode(xpath);

                // 4. 如果找到了，轉型成 XmlElement 並回傳
                if (targetNode != null)
                {
                    return (XmlElement)targetNode;
                }
            }

            return null;
        }
        public XmlElement GetGwTypeDressMode(int gwType, int dressMode)
        {

            if (root != null)
            {
                string xpath = $"//GwType{gwType}//DressMode{dressMode}";

                // 3. 在 root 底下尋找符合條件的第一個節點
                XmlNode targetNode = root.SelectSingleNode(xpath);

                // 4. 如果找到了，轉型成 XmlElement 並回傳
                if (targetNode != null)
                {
                    return (XmlElement)targetNode;
                }
            }

            return null;
        }
        //public XmlElement GetGwTypeShapeDef(int gwType, int dressMode)
        //{

        //    if (root != null)
        //    {
        //        string xpath = $"//GwType{gwType}/ShapeDef[@DressMode='{dressMode}']";

        //        // 3. 在 root 底下尋找符合條件的第一個節點
        //        XmlNode targetNode = root.SelectSingleNode(xpath);

        //        // 4. 如果找到了，轉型成 XmlElement 並回傳
        //        if (targetNode != null)
        //        {
        //            return (XmlElement)targetNode;
        //        }
        //    }

        //    return null;
        //}
        public XmlElement GetGwTypeTools(int gwType, int dressMode)
        {
            if (root != null)
            {
                // 將 XPath 後面加上 /Tools，直接定位到 Tools 節點
                string xpath = $"//GwType{gwType}/ShapeDef[@DressMode='{dressMode}']/Tools";

                XmlNode targetNode = root.SelectSingleNode(xpath);

                if (targetNode != null)
                {
                    return (XmlElement)targetNode;
                }
            }

            return null;
        }
        public XmlElement GetGwTypeCoordinate(int gwType)
        {
            if (root != null)
            {
                // 將 XPath 後面加上 /Tools，直接定位到 Tools 節點
                string xpath = $"//GwType{gwType}/Coordinate";

                XmlNode targetNode = root.SelectSingleNode(xpath);

                if (targetNode != null)
                {
                    return (XmlElement)targetNode;
                }
            }

            return null;
        }
        public void LoadFromFile(string filename)
        { 
            
            xmlDoc.Load(filename);

            root = xmlDoc.DocumentElement;

            if(root.Name == "MachineSetting") Version = root.GetAttribute("Version");

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement x = (XmlElement)root.ChildNodes[i];
                if (x.Name == "GW1") xmlGw1 = x;
                else if (x.Name == "GW2") xmlGw2 = x;
                else if (x.Name == "GW3") xmlGw3 = x;
                else if (x.Name == "GW4") xmlGw4 = x;
                else if (x.Name == "GwType0")
                {
                    xmlOCD_Param = x;
                    for (int j = 0; j < x.ChildNodes.Count; j++)
                    {
                        if (x.ChildNodes[j].Name == "Coordinate") xmlOIG_Coordinate = (XmlElement)x.ChildNodes[j]; //外圓修砂對點
                    }
                }
                else if (x.Name == "GwType1")
                {
                    xmlOIG_Param = x;
                    for (int j = 0; j < x.ChildNodes.Count; j++)
                    {
                        if (x.ChildNodes[j].Name == "Coordinate") xmlOCD_Coordinate = (XmlElement)x.ChildNodes[j]; //內圓修砂對點
                    }
                }
                else if (x.Name == "ProcessList") xmlProcessList = x;
                else if(x.Name == "GwType2")
                {
                    xmlOCD_PA_Param = x;
                    
                    
                }
                else if (x.Name == "GwType3")
                {
                    xmlOCD_NA_Param = x;
                    
                }
            }
        }

        public void SaveToFile(string filename)
        {
            xmlDoc.Save(filename);
        }
    }

    public static class MachineSettingLoader
    {
        public static XmlElement GetShape(this XmlElement x, int mode)
        {
            for (int i = 0; i < x.ChildNodes.Count; i++) 
            {
                XmlElement c = (XmlElement)x.ChildNodes[i];
                if (!c.Name.Contains("Shape")) continue;

                if (int.TryParse(c.GetAttribute("DressMode"), out int dress_mode))
                {
                    if (dress_mode == mode)
                    {
                        return c;
                    }
                }                
            }
            return null;
        }

        public static XmlElement GetChildNodeAt(this XmlElement x, int index)
        {
            if (x == null) return null;
            return (index < x.ChildNodes.Count) ? (XmlElement)x.ChildNodes[index] : null;
        }

        public static bool IsOIG(this XmlElement x)
        {
            return x.GetAttribute("GwType") == "0";
        }

        public static bool IsOCD(this XmlElement x)
        {
            return x.GetAttribute("GwType") == "1";
        }
    }


    public enum GW_TYPE //砂輪類型 (0:內圓, 1:外圓)
    { 
        OIG = 0,
        OCD = 1
    }

    
}
