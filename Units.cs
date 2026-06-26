using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Resources;
using OCD;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace OCD
{
    public static class Units
    {
        public static Fo_Main Fo_Main;

        public static MacroInfo MacroInfo;
        //程式庫
        public static TProgram_DB ProgramDB = new TProgram_DB();
        //預設工序名稱
        public static TProcessNameFile DefProcessNames;
        public static XDocument xmlDefaultProcessLang = new XDocument();//DefaultProcessLang.xml
        public static List<TProcess> ProcessList = new List<TProcess>();//DefaultProcessLang.xml + DefaultProcess.xml

        //預設引數名稱
        public static TArgValueNameFile ArgValueNames = new TArgValueNameFile();

        public static String langfile;
        public static String LangCode;

        public static TAlarmNameFile alarmfile;

        public static String DisplayFmt = "0.0000";

        public static double BalanceAngle1 = 0;
        public static double BalanceAngle2 = 120;
        public static double BalanceAngle3 = 240;
        public static string BalanceVersion = "";
        public static string BalanceStatus = "";
        public static int BalanceError = 0;
        public static int BalanceMode = 0;
        public static double BalanceLock1 = 0;
        public static double BalanceLock2 = 0;
        public static int BalanceDO = 0;
        public static double BalanceVibration_um = 0;
        public static double BalanceVibration_G = 0;
        public static double BalanceRPM = 0;
        public static int BalanceStep = 0;
        public static double BalanceTrialAngle = 0;
        public static double BalanceVibration1_um = 0;
        public static double BalanceVibration2_um = 0;
        public static double BalanceVibration3_um = 0;

        public static int BA_Version = 0x1010;
        public static int BA_Status = 0x1020;
        public static int BA_Error = 0x1030;
        public static int BA_ModeStatus = 0x1040;
        public static int BA_ShockLevel = 0x1110;
        public static int BA_DOStatus = 0x1120;
        public static int BA_Vibration_um = 0x1210;
        public static int BA_Vibration_G = 0x1220;
        public static int BA_RPM = 0x1230;
        public static int BA_BalancingStep = 0x1240;
        public static int BA_InitialAngle = 0x1250;
        public static int BA_TrialAngle = 0x1260;
        public static int BA_Angle = 0x1270;
        public static int BA_NarrowBandVibration = 0x1280;
        public static int BA_InitialRun = 0x1310;
        public static int BA_TrialRun = 0x1320;
        public static int BA_ResidualRun = 0x1330;
        public static int BA_ParameterRun = 0x1340;
        public static int BA_Abort = 0x1350;
    }
}

public class Limit
{
    public int No;
    public double Max;
    public double Min;
    public String Unit;

    public Limit() { }

    public Limit(int no, double min, double max, string unit)
    {
        No = no;
        Max = max;
        Min = min;
        Unit = unit;
    }

    public Limit(XmlElement x)
    {
        if (x == null) return;
        if (x.Name != "Macro") return;

        int.TryParse(x.GetAttribute("No"), out No);
        double.TryParse(x.GetAttribute("Min"), out Min);
        double.TryParse(x.GetAttribute("Max"), out Max);
        Unit = x.GetAttribute("Unit");
    }

    public Limit Clone()
    {
        return (Limit)this.MemberwiseClone();
    }
}

public class MacroInfo
{
    public Dictionary<int, XmlElement> Items = new Dictionary<int, XmlElement>();
    public String FileName;
    public XmlDocument xmlDoc = new XmlDocument();

    public MacroInfo() { }

    public MacroInfo(string filename)
    {
        LoadFromFile(filename);
    }

    public void LoadFromFile(string filename)
    {
        FileName = filename;
        if (!File.Exists(filename)) return;


        xmlDoc.Load(filename);//讀取XML檔案
        XmlElement root = xmlDoc.DocumentElement;

        //從找到的工序中, 取出(所有PCode 標籤).(有Text 標籤的)
        foreach (XmlElement x in root.ChildNodes)
        {
            int.TryParse(x.GetAttribute("No"), out int no); //工序ID 
            //double.TryParse(x.GetAttribute("Min"), out double min); //最小值Min
            //double.TryParse(x.GetAttribute("Max"), out double max); //最大值Max
            //string unit = x.GetAttribute("Unit");
            if (!Items.ContainsKey(no))
            {
                Items.Add(no, x);
            }
        }
    }

    public void LoadLangFile(string filename)
    {
        if (!File.Exists(filename)) return;
        TIniFile ini = new TIniFile(filename);

        foreach (XmlElement x in Items.Values)
        {
            if (x == null) continue;
            if (x.Name != "Macro") continue;

            int.TryParse(x.GetAttribute("No"), out int no);

            string name = ini.ReadString("Macro", no.ToString(), "");
            x.SetAttribute("Name", name);
        }
    }

    public void SaveToFile(string filename)
    {
        FileName = filename;
        xmlDoc.Save(FileName);
    }

    public void Save()
    {
        xmlDoc.Save(FileName);
    }

    public void GetMinMax(int no, out double min, out double max)
    {
        if (!Items.ContainsKey(no))
        {
            min = 0;
            max = 0;
            return;
        }
        XmlElement x = Items[no];
        double.TryParse(x.GetAttribute("Min"), out min);
        double.TryParse(x.GetAttribute("Max"), out max);
    }

    public bool CheckMacroMinMax(int no, ref double val, out Limit limit) //檢查上下限並修正, 沒問題會回傳 true, 0~0不檢查
    {
        if (!Items.ContainsKey(no))//沒有此上下限
        {
            limit = null;
            return true;
        }

        XmlElement x = Items[no];//取得上下限

        limit = new Limit(x);
        if (limit.Min != 0 && limit.Max != 0) //0~0 不檢查
        {
            if (limit.Min > val) //超過下限
            {
                val = limit.Min; //修正數值
                return false;
            }
            if (limit.Max < val) //超過上限
            {
                val = limit.Max; //修正數值
                return false;
            }
        }
        return true;
    }

    public bool CheckMacroMinMax(int no, ref double val) //檢查上下限並修正, 沒問題會回傳 true, 0~0不檢查
    {
        if (!Items.ContainsKey(no))//沒有此上下限
        {
            return true;
        }

        Limit limit = new Limit(Items[no]);//取得上下限
        if (limit.Min != 0 || limit.Max != 0) //0~0 不檢查
        {
            if (limit.Min > val) //超過下限
            {
                val = limit.Min; //修正數值
                return false;
            }
            if (limit.Max < val) //超過上限
            {
                val = limit.Max; //修正數值
                return false;
            }
        }
        return true;
    }
}
