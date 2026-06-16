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

        public XmlElement xmlOIG_Coordinate;//內圓座標系
        public XmlElement xmlOCD_Coordinate;//外圓座標系

        public XmlElement xmlProcessList;//工序清單

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

        public void LoadFromFile(string filename)
        { 
            
            xmlDoc.Load(filename);

            XmlElement root = xmlDoc.DocumentElement;

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
                    xmlOIG_Param = x;
                    for (int j = 0; j < x.ChildNodes.Count; j++)
                    {
                        if (x.ChildNodes[j].Name == "Coordinate") xmlOIG_Coordinate = (XmlElement)x.ChildNodes[j]; //內圓修砂對點
                    }
                }
                else if (x.Name == "GwType1")
                {
                    xmlOCD_Param = x;
                    for (int j = 0; j < x.ChildNodes.Count; j++)
                    {
                        if (x.ChildNodes[j].Name == "Coordinate") xmlOCD_Coordinate = (XmlElement)x.ChildNodes[j]; //外圓修砂對點
                    }
                }
                else if (x.Name == "ProcessList") xmlProcessList = x;
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
