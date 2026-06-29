using OCD;
using OCD.Properties;
using pmcMessagadll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Lifetime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using System.Xml.Linq;
using UserNumLib;
using static Focas1;

#pragma warning disable IDE1006

namespace OCD
{
    public partial class Fo_Main : Form
    {
        public Queue<Action> Actions = new Queue<Action>();

        public MachineSetting machineSetting = new MachineSetting();

        public int GwCount = 0;
        //public int CurrentGwNo = 0;
        public bool Gw2CantUse = true;
        public int[] Gw2CantUseID = { 55, 56, 57, 59 };

        public PictureBox[] GwRefPosSetPics;
        public Dictionary<int, int> GwRefPosSetStatus = new Dictionary<int, int>();

        public bool bPosSetSave = false;
        public bool bChangePart = false;
        public bool bIDCenter = false;
        public bool bSafePos = false;
        public bool bRotation = false;
        public bool bTowerSafePos = false;
        public bool bIDRevSafePos = false;
        public bool bDressMaxMinValue = false;

        public XmlDocument ParamXmlDoc = new XmlDocument();

        public bool Rolleropen = false; //滾輪功能
        public bool Measopen = false; //量測功能
        public bool Rightopen = false; //右側修整條件
        public bool GapOpen = false; //間隙消除
        //選配會影響軸順序
        public static Dictionary<string, int> AxisNo = new Dictionary<string, int>();

        public List<PmcMessageData> PmcMsgList = new List<PmcMessageData>();

        public Fo_ImportProg fo_ImportProg = null;
        public Fo_Warmup fo_Warmup = null;
        public Process ScreenDisplayProcess = null;
        public Process CNCDataManageProcess = null;

        public Fo_TraverseStep fo_TraverseStep = null;

        //電流防撞
        public bool bClearCrash = false;
        public bool bGW1_GAP_CRASH_Enabled = false;
        public bool bGW1_GAP = false;
        public bool bGW1_CRASH = false;
        public double GW1_Grind_GAP = 0;
        public double GW1_Grind_CRASH = 0;
        public double GW1_Dress_GAP = 0;
        public double GW1_Dress_CRASH = 0;
        public bool bGW2_GAP_CRASH_Enabled = false;
        public bool bGW2_GAP = false;
        public bool bGW2_CRASH = false;
        public double GW2_Grind_GAP = 0;
        public double GW2_Grind_CRASH = 0;
        public double GW2_Dress_GAP = 0;
        public double GW2_Dress_CRASH = 0;

        //恆速(m/s -> RPM)
        public double GW2_GRIND_AT_628 = 0;//研磨中 線速度->轉速(加工程式算好丟到這)
        public double GW2_DRESS_AT_629 = 0;//修整中 線速度->轉速(加工程式算好丟到這)
        public double GW1_GRIND_AT_638 = 0;//研磨中 線速度->轉速(加工程式算好丟到這)
        public double GW1_DRESS_AT_639 = 0;//修整中 線速度->轉速(加工程式算好丟到這)

        //上緣觸發旗標
        public bool bGW2_GRIND_AT = false; //研磨中旗標
        public bool bGW2_DRESS_AT = false; //修整中旗標
        public bool bGW1_GRIND_AT = false; //研磨中旗標
        public bool bGW1_DRESS_AT = false; //修整中旗標

        //電流防撞
        public bool bGW1GRIND_AT = false; //R530.0 研磨中旗標
        public bool bGW1DRESS_AT = false; //R530.1 修整中旗標
        public bool bGW2GRIND_AT = false; //R530.2 研磨中旗標
        public bool bGW2DRESS_AT = false; //R530.3 修整中旗標

        public const int WM_LBUTTONUP = 0x0202;


        Fo_Monitor_List fo_monitor = null;
        Fo_MaintainceList fo_maintan = null;

        public Fo_Layout fo_layout = null;
        public bool bGridMode = false;

        public Panel pa_Main;
        public Panel pa_SoftPanel;


        public int Axis3Type = 0;//0:線性軸, 1:旋轉軸
        public int Axis4Type = 1;//0:線性軸, 1:旋轉軸

        int UserSCode;//使用者設定的工件轉速
        int ProgSCode;//程式運行中的工件轉速
        //int LastSendSCode;//實際寫到驅動器的

        public double OffsetMax;
        public double OffsetMin;

        Fo_Runin fo_Runin;

        //轉英制旗標
        public bool bInchTrans = false;

        public List<int> DressToolTag = new List<int>();

        public List<String> CurrentDGW_Step = new List<string>();

        //目前編輯的砂輪號
        public int CurrentEditGwNo;
        //暫存 #500~#999
        Dictionary<int, double> CurrentMacro = new Dictionary<int, double>();
        //暫存 砂輪資料 #10000 ~ #10399
        Dictionary<int, double> CurrentGwMacro = new Dictionary<int, double>();

        public Dictionary<string, int> Edit_DGV_Index = new Dictionary<string, int>();


        //XML 文件
        private XmlDocument xmlDocument = new XmlDocument();
        private XmlDocument xmlCodeUnit = new XmlDocument();
        private List<SoftPBLamp> SoftPBLamps = new List<SoftPBLamp>();
        private Dictionary<string, byte> PMC_Values = new Dictionary<string, byte>();
        private Uc_RoundBtn btn_ToProbe = null;


        public IntPtr ScreenDisplay;

        public int SUCCESS = Focas1.EW_OK;

        Panel pa = new Panel();
        Label la = new Label();

        Panel pa_Loading = new Panel();
        Label la_Loading = new Label();

        public Fo_Logo Logo = new Fo_Logo();

        //使用設備
        public int Gw1Dev = 0;
        public int Gw2Dev = 0;
        public int Gw3Dev = 0;
        public int Gw4Dev = 0;
        public int RollerDev = 0;
        public int SpindleDev = 0;

        public SerialDevice Gw1 = new SerialDevice() { Slave = 1 };
        public SerialDevice Gw2 = new SerialDevice() { Slave = 2 };
        public SerialDevice Gw3 = new SerialDevice() { Slave = 3 };
        public SerialDevice Gw4 = new SerialDevice() { Slave = 4 };
        public SerialDevice Roller = new SerialDevice() { Slave = 5 };
        public SerialDevice Spindle = new SerialDevice() { Slave = 6 };

        public int SpindleChIndex = 0; //通訊頻道 0:RS485, 1:RS422

        public Focas1 focas = new Focas1();

        private NativeTabControl NativeTabControl1 = new NativeTabControl();//為了讓TabControl 沒有邊框
        private NativeTabControl NativeTabControl2 = new NativeTabControl();//為了讓TabControl 沒有邊框
        private NativeTabControl NativeTabControl3 = new NativeTabControl();//為了讓TabControl 沒有邊框
        private NativeTabControl NativeTabControl4 = new NativeTabControl();//為了讓TabControl 沒有邊框

        bool bReadProcessExe = true;

        //int SetSCode;//使用者設定的工件轉速
        //bool bCycleStart;
        //int GetSCode;//程式運行中的工件轉速
        //int CmdSCode;//實際寫到驅動器的
        //public bool InchMode;

        //量測方向
        //private int MeasureDir = -1;

        //private bool bQueryPosition;
        public AxisPosition Pos;
        public StatusInfo Status;

        private double dManualZeroPoint;
        private double dManualZeroPointZ;
        private double dManualZeroPointY;

        private CreateMode CreateProcessMode;

        //Macro 位址、名稱、數值(FANUC、台達)
        //private Macros macros = new Macros();

        public TSubProgram GrindingDress = new TSubProgram();

        //主程式
        private String MainProgram = "";

        //編輯加工程式時使用
        private ComboBox CB = new ComboBox();
        private DataGridView Edit_DGV = null;
        private PCodeInfo Edit_AVN = null;

        private ComboBox CB2 = new ComboBox();

        private Button BTN = new Button();

        public int ProcessIndex; //要編輯的工序，在Processes[]的索引值

        public int CheckSerialHeart;

        //private bool bWriteTCode = false;
        //private bool bReadTCode = false;
        //private bool bWriteProcessExe = false;
        //private bool bShowG59 = false;

        //各工序是否執行
        //public List<bool> ProcessEnabled = null;
        public List<bool> RedoEnabled = new List<bool>();
        public List<bool> ExecEnabled = new List<bool>();

        //新增程式暫存用
        public TProcess TempProcess = null;
        public TProgram TempProgram = null;
        //目前開啟的程式
        public TProgram CurrentProgram = null;

        //private int iCheckMainProgStart;
        //private bool IsO8999 = false;

        //暫存上一頁
        private Stack<TabPage> PrevPage = new Stack<TabPage>();

        private string IPAddress = "192.168.168.2";
        private int Port = 8193;

        private AlarmFile TroubleShootingFile1;
        private AlarmFile PmcAlarmTable;

        private AlarmFile CurrentAlarm = new AlarmFile();

        //private bool bRPM_Mode = true;
        private double MS_Speed = 0;//米速(加工)
        private double MasterG54Diam = 0;//Master 時的外徑(加工)
        private double MasterG54 = 0;//Master 時的位置(加工)

        private double MS_DGW_Speed = 0;//米速(修整)
        private double MasterG55Diam = 0;//Master 時的外徑(修整)
        private double MasterG55 = 0;//Master 時的位置(修整)

        //private double CurrentHz;

        public int iQueryAlarmTick;

        int DressGwStep;
        int DressPartsStep;

        bool bRefleshSoftPanell = false;
        bool bSoftPanelBuzy = false;
        bool bRefleshMonitor = false;
        bool bMonitorBuzy = false;
        public LinkSQLite database = new LinkSQLite();

        //int ToAndBack = 0;
        //int DressDir = 0;

        //Fo_M450 fo_M450 = null;

        public bool bCycleStart = false;//F0.5
        public bool bRun = false;//F0.7

        //bool bReady_E2402_0 = true; // 軟體開啟後給FANUC READY 訊號
        //bool bReset_E2402_1 = false; // RESET 點 Rewin G8.6
        //bool bM450_Finish_E2402_2 = false; //M450 Finish & 重新啟動程式
        //bool bOneKeyStart_E2402_3 = false; // 畫面呼叫程式，模式:D1595 Byte(0:僅呼叫, 1:呼叫&執行, 2:呼叫&執行&回原程式)、程式號D1596 Word
        //bool bGwSizeTip_E2402_4 = false; //砂輪更換提示
        //bool bGwSizeMin_E2402_5 = false; //砂輪過小
        //bool bM440_E2402_6 = false; //NC 程式暫停，等待PC讀取資料
        double Gw1SizeTipDiameter; //修砂外徑提示值
        double Gw2SizeTipDiameter; //修砂外徑提示值

        bool GW1_Comm_Enabled = false;
        bool GW2_Comm_Enabled = false;
        bool GW3_Comm_Enabled = false;
        bool GW4_Comm_Enabled = false;

        bool Roller_Comm_Enabled = false;

        bool SP_Comm_Enabled = false;

        //成型修整
        public DGWFile dgwFile = new DGWFile();

        double Gw1_DressBase_MaxPos;//修整座最大位置 參數 6934
        double Gw1_DressBase_MinPos;//修整座最小位置 參數 6954

        double Gw2_DressBase_MaxPos;//修整座最大位置 參數 6934
        double Gw2_DressBase_MinPos;//修整座最小位置 參數 6954

        //private object TempMaintanceTab = null;



        //bool GwSetEdit = false;        //登錄砂輪資料編輯
        bool GwDressEdit = false;      //登錄砂輪修砂編輯
        bool GwWorkPiEdit = false;     //登錄工件設定編輯


        //private bool bProfibusProcess = false;

        /*
        //避免頁面接換時，元件刷新問題。
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        */

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            /*
            bool bFind = false;
            foreach (int i in MsgList)
            {
                if (i == m.Msg)
                {
                    bFind = true;
                    break;
                }
            }
            if (!bFind)
            {
                textBox1.AppendText(m.Msg.ToString() + "\r\n");
                MsgList.Add(m.Msg);
            }
            */
            if (m.Msg == 281) return;

            base.WndProc(ref m);
        }

        //private bool bM450 = false;
        public bool bM450Finish = false;
        public int M450Status;

        private Thread ThrMain;
        private bool bClose = false;
        private bool bCloseFinish = false;

        private Thread ThrRedo = null;
        private Thread ThrChangePos = null;
        private Thread ThrMeasure = null;
        private Thread ThrDressGw = null;
        private Thread ThrWaitM450 = null;
        private Thread ThrScreenDisplay = null;

        private int PmcRefleshStart;
        //private double Gw1CurrentCRASH;
        //private bool bBack = false;
        private int Request_CNC_Time_Start;
        private int SelectGwNo = 0; //編輯時使用
        private int SelectShapeNo = 0; //編輯時使用
        private int LastGwShapeNo = 0; //最後顯示的形狀

        public TabPage tab_Runin;
        public TabPage tab_ImportProg;
        public TabPage tab_CNCDataManage;
        public TabPage tab_Warmup;

        public int BalanceSlave = 99;
        //public bool M3Mode = false;

        bool btn_SaveProgVisible = false; //紀錄btn_SaveProg是否正在存在

        private string ProcID = "";
        //private bool DressMode1 = false;
        //private bool DressMode2 = false;
        string machineSeries = "M";
        public MachineType[] GWType = new MachineType[3] { MachineType.OCD, MachineType.OCD2, MachineType.OIG }; // 砂輪直頭
        // 20260302 alan add 砂輪基準點設定
        public Dictionary<int, double> CurentGw_Data = new Dictionary<int, double>();
        public bool bYAEnable = false;
        public double dLast_Y_MPos = 0;
        public double dLast_Y_APos = 0;
        public Fo_Main()
        {
            InitializeComponent();

            machineSetting.LoadFromFile(Application.StartupPath + "\\MachineSetting.xml");

            PictureBox[] pic_buf = { pic_GWRPSsetting1, pic_GWRPSsetting2, pic_GWRPSsetting3, pic_GWRPSsetting4, pic_GWRPSsetting5
                    , pic_GWRPSsetting6, pic_GWRPSsetting7, pic_GWRPSsetting8, pic_GWRPSsetting9, pic_GWRPSsetting10
                    , pic_GWRPSsetting11};

            GwRefPosSetPics = pic_buf;

            //目前[DGV_Param1,DGV_Param2,DGV_Param3,DGV_Advance,DGV_Dress1,DGV_Dress2] Column 順序必須相同，否則程式會有問題
            Edit_DGV_Index.Add("Name", 0);//參數名稱
            Edit_DGV_Index.Add("Code", 1);//顯示圖片代碼
            Edit_DGV_Index.Add("TextValue", 2);//顯示文字或數值
            Edit_DGV_Index.Add("Unit", 3);//單位
            Edit_DGV_Index.Add("PCode", 4);//PCode物件
            Edit_DGV_Index.Add("DoubleValue", 5);//浮點數值
            //避免後面的人更改到順序，這裡做一個防呆
            if (Col_Param1_Name.Index != Col_Param2_Name.Index ||
               Col_Param1_Name.Index != Col_Param3_Name.Index ||
               Col_Param1_Name.Index != Col_Advance_Name.Index ||
               Col_Param1_Name.Index != Col_Dress1_Name.Index ||
               Col_Param1_Name.Index != Col_Dress2_Name.Index ||
               Col_Param1_Code.Index != Col_Param2_Code.Index ||
               Col_Param1_Code.Index != Col_Param3_Code.Index ||
               Col_Param1_Code.Index != Col_Advance_Code.Index ||
               Col_Param1_Code.Index != Col_Dress1_Code.Index ||
               Col_Param1_Code.Index != Col_Dress2_Code.Index ||
               Col_Param1_TextValue.Index != Col_Param2_TextValue.Index ||
               Col_Param1_TextValue.Index != Col_Param3_TextValue.Index ||
               Col_Param1_TextValue.Index != Col_Advance_TextValue.Index ||
               Col_Param1_TextValue.Index != Col_Dress1_TextValue.Index ||
               Col_Param1_TextValue.Index != Col_Dress2_TextValue.Index ||
               Col_Param1_Unit.Index != Col_Param2_Unit.Index ||
               Col_Param1_Unit.Index != Col_Param3_Unit.Index ||
               Col_Param1_Unit.Index != Col_Advance_Unit.Index ||
               Col_Param1_Unit.Index != Col_Dress1_Unit.Index ||
               Col_Param1_Unit.Index != Col_Dress2_Unit.Index ||
               Col_Param1_PCode.Index != Col_Param2_PCode.Index ||
               Col_Param1_PCode.Index != Col_Param3_PCode.Index ||
               Col_Param1_PCode.Index != Col_Advance_PCode.Index ||
               Col_Param1_PCode.Index != Col_Dress1_PCode.Index ||
               Col_Param1_PCode.Index != Col_Dress2_PCode.Index ||
               Col_Param1_DoubleValue.Index != Col_Param2_DoubleValue.Index ||
               Col_Param1_DoubleValue.Index != Col_Param3_DoubleValue.Index ||
               Col_Param1_DoubleValue.Index != Col_Advance_DoubleValue.Index ||
               Col_Param1_DoubleValue.Index != Col_Dress1_DoubleValue.Index ||
               Col_Param1_DoubleValue.Index != Col_Dress2_DoubleValue.Index)
            {
                Fo_Msg.Show("Define EGV Index Error.");
            }

            #region 處理 DataGridViewCellStyle 問題
            //[砂輪資料]
            DGV_GwParam.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_GwParam.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            DGV_GwParam.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_GwParam.DefaultCellStyle.Font = new Font("微軟正黑體", 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            
            //[補正]
            DGV_Offset.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_Offset.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            DGV_Offset.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_Offset.DefaultCellStyle.Font = new Font("微軟正黑體", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            DGV_Offset.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_Offset.RowHeadersDefaultCellStyle.Font = new Font("新細明體", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            Col_OffsetName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //[重修精磨]
            DGV_Redo.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_Redo.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            DGV_Redo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_Redo.DefaultCellStyle.Font = new Font("微軟正黑體", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            DGV_Redo.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV_Redo.RowHeadersDefaultCellStyle.Font = new Font("新細明體", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            Col_R_Name.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Font font_redo = new Font("微軟正黑體", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            Col_R_PosX.DefaultCellStyle.Font = font_redo;
            Col_R_PosZ.DefaultCellStyle.Font = font_redo;
            Col_R_OfsX.DefaultCellStyle.Font = font_redo;
            Col_R_OfsZ.DefaultCellStyle.Font = font_redo;

            //[成形修整]
            //字型
            dgv_Path.DefaultCellStyle.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            dgv_Path.RowHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            dgv_Path.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            //對其方式
            dgv_Path.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_Path.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_Path.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Col_OfsPath_X.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_OfsPath_Z.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Path_No.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Path_Type.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Path_X.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Path_Z.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Path_R.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Path_Speed.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //[監視]
            DGV_Monitor_Program.ColumnHeadersDefaultCellStyle.Font = new Font("新細明體", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            //對其方式
            DGV_Monitor_Program.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Col_N.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Col_Program.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            //[程式]
            DGV_ProcList.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));

            //[編輯工序]
            Font font_dgv = new Font("微軟正黑體", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            //共通字型 - 數值、代碼、單位 (名稱會在多國語言處理)
            DGV_Param1.RowsDefaultCellStyle.Font = font_dgv;
            DGV_Param2.RowsDefaultCellStyle.Font = font_dgv;
            DGV_Param3.RowsDefaultCellStyle.Font = font_dgv;
            DGV_Advance.RowsDefaultCellStyle.Font = font_dgv;
            DGV_Dress1.RowsDefaultCellStyle.Font = font_dgv;
            DGV_Dress2.RowsDefaultCellStyle.Font = font_dgv;
            //參數1 對齊方式
            Col_Param1_Code.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Param1_TextValue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Param1_Unit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //參數2 對齊方式
            Col_Param2_Code.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Param2_TextValue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Param2_Unit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //參數3 對齊方式
            Col_Param3_Code.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Param3_TextValue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Param3_Unit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //進階 對齊方式
            Col_Advance_Code.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Advance_TextValue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Advance_Unit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //修整1 對齊方式
            Col_Dress1_Code.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Dress1_TextValue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Dress1_Unit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //修整2 對齊方式
            Col_Dress2_Code.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Col_Dress2_TextValue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Col_Dress2_Unit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            
            
            //[軟體面板選擇]
            dgv_SoftPanelList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_SoftPanelList.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            Col_SoftPanel_Name.DefaultCellStyle.Font = new Font("微軟正黑體", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));

            //[砂輪基準點設定]
            dgv_GWRPSs.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            dgv_GWRPSs.RowHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            dgv_GWRPSs.RowsDefaultCellStyle.Font = new Font("微軟正黑體", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            //對其方式
            dgv_GWRPSs.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_GWRPSs.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Col_GWRPSvalue.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            #endregion 處理 DataGridViewCellStyle 問題

            pa_SoftPanel = new Panel();
            pa_SoftPanel.Parent = this;
            pa_SoftPanel.Left = 0;
            pa_SoftPanel.Top = 0;
            pa_SoftPanel.VisibleChanged += pa_SoftPanel_VisibleChanged;

            pa_Main = new Panel();
            pa_Main.Parent = this;
            pa_Main.Dock = DockStyle.Fill;
            pa_Main.BringToFront();

            TC_Main.Parent = pa_Main;
            TC_Main.Dock = DockStyle.Fill;


            string softpanel_file = Application.StartupPath + "\\SoftPanel.xml";
            LoadSoftPanelFromXML(softpanel_file);

            tab_Runin = new TabPage("Runin");
            tab_Runin.BackColor = Color.DimGray;
            TC_Main.TabPages.Add(tab_Runin);

            tab_ImportProg = new TabPage("ImportProg");
            tab_ImportProg.BackColor = Color.DimGray;
            TC_Main.TabPages.Add(tab_ImportProg);

            tab_CNCDataManage = new TabPage("CNCDataManage");
            tab_CNCDataManage.BackColor = Color.DimGray;
            TC_Main.TabPages.Add(tab_CNCDataManage);

            tab_Warmup = new TabPage("Warmup");
            tab_Warmup.BackColor = Color.DimGray;
            TC_Main.TabPages.Add(tab_Warmup);

            TC_Main.Dock = DockStyle.Fill;

            ReadGwIni();

            iQueryAlarmTick = Environment.TickCount;

            pa_AlarmTip.Width = 1024;
            pa_AlarmTip.Left = 0;
            pa_AlarmTip.Parent = this;
            pa_AlarmTip.BringToFront();

            pa_Tip.Width = 1024;
            pa_Tip.Left = 0;
            pa_Tip.Parent = this;
            pa_Tip.BringToFront();

            btn_PosSetSave.Left = 864;
            btn_PosSetSave.Top = 8;
            btn_PosSetSave.Parent = pa_Bottom;

            btn_SaveDressGw.Left = 864;
            btn_SaveDressGw.Top = 8;
            btn_SaveDressGw.Parent = pa_Bottom;

            btn_SaveGrindCoor.Left = 864;
            btn_SaveGrindCoor.Top = 8;
            btn_SaveGrindCoor.Parent = pa_Bottom;

            btn_Path_Add.Top = 464;
            btn_Path_InsertFront.Top = 464;
            btn_Path_InsertBack.Top = 464;
            btn_Path_Delete.Top = 464;
            btn_Path_NewFile.Top = 464;
            btn_ClearAllOffsetPath.Top = 464;
            //btn_Path_Save.Top = 464;

            la_DiamOfsZ.Top = 464;
            tb_DiamOfsZ.Top = 504;
            la_ToolRCompFunc.Top = 464;
            cb_ToolRCompFunc.Top = 504;
            la_ToolR.Top = 464;
            tb_ToolR.Top = 504;
            //btn_EditPath.Top = 464;
            //btn_OffsetPath.Top = 464;
            //TC_Path.Height = 472;

            //pic_Warning.Parent = btn_ArgDress2;
            //pic_Warning.Left = btn_ArgDress2.Width - pic_Warning.Width - 2;
            //pic_Warning.Top = btn_ArgDress2.Height - pic_Warning.Height - 2;

            dgv_Path.Height = 456;

            #region 資料庫
            database.FileName = Application.StartupPath + "\\data.db";
            //讀取資料庫
            if (!File.Exists(database.FileName))
            {
                database.Connect();

                //建立工件(程式)選擇的資料表
                database.ExecuteNonQuery("CREATE TABLE AlarmHistory (Time TEXT, Code Text, Axis INTEGER, Path INTEGER);");
            }
            else
            {
                database.Connect();
            }
            #endregion 資料庫

            pa.Width = 300;
            pa.Height = 80;
            pa.BackColor = Color.Yellow;
            pa.Left = (this.Width - 300) / 2;
            pa.Top = 0;
            pa.Visible = false;
            pa.Parent = this;
            pa.BringToFront();



            la.Parent = pa;
            la.AutoSize = false;
            la.Left = 10;
            la.Width = 280;
            la.Top = 30;
            la.Height = 20;
            la.Font = new Font("Times New Roman", 12);
            la.ForeColor = Color.Black;
            la.TextAlign = ContentAlignment.MiddleCenter;

            pa_Loading.Width = 300;
            pa_Loading.Height = 80;
            pa_Loading.BackColor = Color.Yellow;
            pa_Loading.Left = (this.Width - 300) / 2;
            pa_Loading.Top = (this.Height - 80) / 2;
            pa_Loading.Visible = false;
            pa_Loading.Parent = this;
            pa_Loading.BringToFront();

            la_Loading.Parent = pa_Loading;
            la_Loading.AutoSize = false;
            la_Loading.Left = 10;
            la_Loading.Width = 280;
            la_Loading.Top = 30;
            la_Loading.Height = 20;
            la_Loading.Font = new Font("Times New Roman", 12);
            la_Loading.ForeColor = Color.Black;
            la_Loading.TextAlign = ContentAlignment.MiddleCenter;

            Logo.WindowState = FormWindowState.Maximized;
            Logo.TopLevel = false;
            Logo.Parent = this;

            PrevPage.Push(tab_Monitor);
            CB.SelectedIndexChanged += new EventHandler(CB_SelectedIndexChanged);
            CB.DropDownClosed += new EventHandler(CB_DropDownClosed);
            CB.DropDownStyle = ComboBoxStyle.DropDownList;
            Units.Fo_Main = this;

            CB2.SelectedIndexChanged += new EventHandler(CB2_SelectedIndexChanged);
            CB2.DropDownClosed += new EventHandler(CB2_DropDownClosed);
            CB2.DropDownStyle = ComboBoxStyle.DropDownList;
            CB2.LostFocus += new EventHandler(CB2_LostFocus);
            BTN.Click += new EventHandler(BTN_Click);



            //pa_Unlock.Parent = this;
            //pa_Unlock.Visible = false;

            SoftPanelLayout();


            //讀取設定值
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");

            //0:M2(2顆砂輪), 1:M3(2顆砂輪)
            machineSeries = ini.ReadString("System", "MachineSeries", "M");
            GwCount = ini.ReadInteger("System", "GWCount", 3);
            // 是否斜頭
            GWType[0] = (MachineType)ini.ReadInteger("System", "GW1Type", 0);
            GWType[1] = (MachineType)ini.ReadInteger("System", "GW2Type", 2);
            GWType[2] = (MachineType)ini.ReadInteger("System", "GW3Type", 1);

            //初始化
            if (ini.ReadInteger("System", "Init", 1) == 1)
            {
                ini.WriteInteger("System", "Init", 0);

                ini.WriteBool("System", "InchMode", false);

                
                //CNC
                ini.WriteString("CNC", "IP", "192.168.168.2");
                ini.WriteInteger("CNC", "Port", 8193);

                //維護頁
                ini.WriteInteger("UI", "pa_Language", 1);
                ini.WriteInteger("UI", "pa_ProcessParam", 1);
                ini.WriteInteger("UI", "pa_ScreenDisplay", 1);
                ini.WriteInteger("UI", "pa_Balance", 0);
                ini.WriteInteger("UI", "pa_PositionSet", 1);
                ini.WriteInteger("UI", "pa_RunSpindle", 0);
                ini.WriteInteger("UI", "pa_ImportProg", 1);
                ini.WriteInteger("UI", "pa_Warmup", 0);
                ini.WriteInteger("UI", "pa_CNCDataManager", 1);
                ini.WriteInteger("UI", "pa_GWRPS", 0);
                ini.WriteInteger("UI", "pa_RotationCenterOffset", 0);
                //砂輪1 轉速 
                ini.WriteInteger("UI", "pa_Monitor_GW1", 1);
                ini.WriteInteger("UI", "pa_Monitor_GW1_Left", pa_Monitor_GW1.Left);
                ini.WriteInteger("UI", "pa_Monitor_GW1_Top", pa_Monitor_GW1.Top);
                ini.WriteInteger("UI", "pa_Monitor_GW1_Width", 224);
                ini.WriteInteger("UI", "pa_Monitor_GW1_Height", 80);

                //砂輪2 轉速 
                ini.WriteInteger("UI", "pa_Monitor_GW2", 1);
                ini.WriteInteger("UI", "pa_Monitor_GW2_Left", pa_Monitor_GW2.Left);
                ini.WriteInteger("UI", "pa_Monitor_GW2_Top", pa_Monitor_GW2.Top);
                ini.WriteInteger("UI", "pa_Monitor_GW2_Width", 224);
                ini.WriteInteger("UI", "pa_Monitor_GW2_Height", 80);

                //砂輪3 轉速 
                ini.WriteInteger("UI", "pa_Monitor_GW3", 1);
                ini.WriteInteger("UI", "pa_Monitor_GW3_Left", pa_Monitor_GW3.Left);
                ini.WriteInteger("UI", "pa_Monitor_GW32_Top", pa_Monitor_GW3.Top);
                ini.WriteInteger("UI", "pa_Monitor_GW3_Width", 224);
                ini.WriteInteger("UI", "pa_Monitor_GW2_Height", 80);

                //砂輪4 轉速 
                ini.WriteInteger("UI", "pa_Monitor_GW4", 1);
                ini.WriteInteger("UI", "pa_Monitor_GW4_Left", pa_Monitor_GW4.Left);
                ini.WriteInteger("UI", "pa_Monitor_GW4_Top", pa_Monitor_GW4.Top);
                ini.WriteInteger("UI", "pa_Monitor_GW4_Width", 224);
                ini.WriteInteger("UI", "pa_Monitor_GW4_Height", 80);

                //工件主軸
                ini.WriteInteger("UI", "pa_Spindle", 1);
                ini.WriteInteger("UI", "pa_Spindle_Left", pa_Spindle.Left);
                ini.WriteInteger("UI", "pa_Spindle_Top", pa_Spindle.Top);
                ini.WriteInteger("UI", "pa_Spindle_Width", 224);
                ini.WriteInteger("UI", "pa_Spindle_Height", 128);

                //其他
                ini.WriteInteger("UI", "pa_Monitor_GW1_MS", 0);//砂輪1 衡速(廢棄)
                ini.WriteInteger("UI", "pa_Monitor_GW2", 0);//砂輪2 轉速
                ini.WriteInteger("UI", "pa_MaintainDoor", 0);//安全門
                ini.WriteInteger("UI", "pa_MonitorAbs", 1);//絕對座標
                ini.WriteInteger("UI", "pa_MonitorDistToGo", 1);//殘移動量
                ini.WriteInteger("UI", "pa_MonitorMach", 1);//機械座標
                ini.WriteInteger("UI", "pa_Monitor_DC", 1);//相對座標
                ini.WriteInteger("UI", "pa_MonitorMach", 1);//機械座標
                ini.WriteInteger("UI", "pa_Monitor_DC", 1);//相對座標
                ini.WriteInteger("UI", "pa_Monitor_Prog", 1);//程式
                ini.WriteInteger("UI", "pa_Monitor_Info", 1);//狀態資訊

                //工序選擇 - 工序ID
                ini.WriteInteger("UI", "ProcessTag1", 50);//直進刀
                ini.WriteInteger("UI", "ProcessTag2", 51);//橫進刀
                ini.WriteInteger("UI", "ProcessTag3", 52);//右端面
                ini.WriteInteger("UI", "ProcessTag4", 53);//振動進刀
                ini.WriteInteger("UI", "ProcessTag5", 54);//錐度橫進刀
                ini.WriteInteger("UI", "ProcessTag6", 55);//斜進刀
                ini.WriteInteger("UI", "ProcessTag7", 56);//外圓直進刀
                ini.WriteInteger("UI", "ProcessTag8", 57);//外圓橫進刀
                ini.WriteInteger("UI", "ProcessTag9", 58);//外圓右端面
                ini.WriteInteger("UI", "ProcessTag19", 999);//code

                // 使用小角度
                ini.WriteBool("UI", "YAEnable", true);
            }

            // 維護頁面
            pa_Language.Visible = ini.ReadInteger("UI", "pa_Language", 1) == 1;
            pa_ProcessParam.Visible = ini.ReadInteger("UI", "pa_ProcessParam", 1) == 1;
            pa_ScreenDisplay.Visible = ini.ReadInteger("UI", "pa_ScreenDisplay", 1) == 1;
            pa_Balance.Visible = ini.ReadInteger("UI", "pa_Balance", 0) == 1;
            pa_PositionSet.Visible = ini.ReadInteger("UI", "pa_PositionSet", 1) == 1;
            pa_RunSpindle.Visible = ini.ReadInteger("UI", "pa_RunSpindle", 0) == 1;
            pa_ImportProg.Visible = ini.ReadInteger("UI", "pa_ImportProg", 1) == 1;
            pa_Warmup.Visible = ini.ReadInteger("UI", "pa_Warmup", 0) == 1;
            pa_CNCDataManager.Visible = ini.ReadInteger("UI", "pa_CNCDataManager", 1) == 1;
            pa_GWRPS.Visible = ini.ReadInteger("UI", "pa_GWRPS", 0) == 1;
            pa_RotationCenterOffset.Visible = ini.ReadInteger("UI", "pa_RotationCenterOffset", 0) == 1;
            bYAEnable = ini.ReadBool("UI", "YAEnable", true);// 使用小角度
            
            Rolleropen = ini.ReadBool("System", "Rolleropen", false);//工序中 滾輪功能 顯示
            Measopen = ini.ReadBool("System", "Measopen", false);//工序中 量測功能 顯示
            Rightopen = ini.ReadBool("System", "Rightopen", false);//工序中 右側修整條件 顯示
            GapOpen = ini.ReadBool("System", "GapOpen", false);//工序中 間隙消除功能 顯示

            btn_MeasureList.Visible = Measopen;
            //bInchTrans = ini.ReadBool("System", "InchMode", false);
            
            

            Gw1Dev = ini.ReadInteger("System", "Gw1Dev", 0);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
            Gw2Dev = ini.ReadInteger("System", "Gw2Dev", 0);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
            Gw3Dev = ini.ReadInteger("System", "Gw3Dev", 0);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
            Gw4Dev = ini.ReadInteger("System", "Gw4Dev", 0);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
            RollerDev = ini.ReadInteger("System", "RollerDev", 0);//0:士林變頻器, 2:三菱變頻器
            SpindleDev = ini.ReadInteger("System", "SpindleDev", 0);//0:三菱驅動器, 1:安川驅動器, 2:士林變頻器

            SpindleChIndex = ini.ReadInteger("System", "SpindleChIndex", 0);//0:RS485 , 1:RS422(只有安川驅動器要)

            OffsetMax = ini.ReadFloat("Parameter", "OffsetMax", 0);
            OffsetMin = ini.ReadFloat("Parameter", "OffsetMin", 0);

            GW1_Grind_GAP = ini.ReadFloat("Parameter", "GW1_Grind_GAP", 0);
            GW1_Grind_CRASH = ini.ReadFloat("Parameter", "GW1_Grind_CRASH", 0);
            GW1_Dress_GAP = ini.ReadFloat("Parameter", "GW1_Dress_GAP", 0);
            GW1_Dress_CRASH = ini.ReadFloat("Parameter", "GW1_Dress_CRASH", 0);
            GW2_Grind_GAP = ini.ReadFloat("Parameter", "GW2_Grind_GAP", 0);
            GW2_Grind_CRASH = ini.ReadFloat("Parameter", "GW2_Grind_CRASH", 0);
            GW2_Dress_GAP = ini.ReadFloat("Parameter", "GW2_Dress_GAP", 0);
            GW2_Dress_CRASH = ini.ReadFloat("Parameter", "GW2_Dress_CRASH", 0);

            MasterG54Diam = ini.ReadFloat("MS", "MasterG54Diam", 0);//Master 時的外徑            
            MasterG54 = ini.ReadFloat("MS", "MasterG54", 0);//Master 時的座標
            MasterG55Diam = ini.ReadFloat("MS", "MasterG55Diam", 0);//Master 時的外徑
            MasterG55 = ini.ReadFloat("MS", "MasterG55", 0);//Master 時的座標

            MS_Speed = ini.ReadFloat("MS", "LastSpeed", 0);//最後設定的米速
            //btn_Gw1CmdMS.DisplayText = MS_Speed.ToString("0.00");
            MS_DGW_Speed = ini.ReadFloat("MS", "LastDGWSpeed", 0);//最後設定的米速
            //btn_Gw1DGWCmdMS.DisplayText = MS_DGW_Speed.ToString("0.00");

            Axis3Type = ini.ReadInteger("System", "Axis3Type", 0); //0:線性軸, 1:旋轉軸
            Axis4Type = ini.ReadInteger("System", "Axis4Type", 1); //0:線性軸, 1:旋轉軸

            //砂輪變頻器的通訊功能
            GW1_Comm_Enabled = ini.ReadInteger("System", "GW1_Comm_Enabled", 1) == 1;
            GW2_Comm_Enabled = ini.ReadInteger("System", "GW2_Comm_Enabled", 1) == 1;
            GW3_Comm_Enabled = ini.ReadInteger("System", "GW3_Comm_Enabled", 1) == 1;
            GW4_Comm_Enabled = ini.ReadInteger("System", "GW4_Comm_Enabled", 1) == 1;
            Roller_Comm_Enabled = ini.ReadInteger("System", "Roller_Comm_Enabled", 1) == 1;
            SP_Comm_Enabled = ini.ReadInteger("System", "SP_Comm_Enabled", 1) == 1;

            ch_UI_BalanceGW1.Checked = ini.ReadInteger("UI", "BalanceGW1", 0) == 1;
            ch_UI_BalanceGW2.Checked = ini.ReadInteger("UI", "BalanceGW2", 0) == 1;

            Col_Ofs_MeaOfs.Visible = ini.ReadInteger("System", "Measure_Option", 0) == 1;



            #region 多國語言
            String lang = ini.ReadString("System", "Language", "CHT");
            Units.langfile = Application.StartupPath + "\\Language\\" + lang + "\\" + lang + ".txt";
            if (!File.Exists(Units.langfile)) Fo_Msg.Show(lang + ".txt" + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
            Units.alarmfile = new TAlarmNameFile();
            Units.alarmfile.LoadFromFile(Application.StartupPath + "\\Language\\" + lang + "\\AlarmCode.txt");
            Units.LangCode = lang;

            PmcMsgList = PmcMessage.LoadFromFile(Application.StartupPath + "\\PmcMessage.txt");

            string last_filename = Application.StartupPath + "\\PmcMessage.txt";
            if (!File.Exists(last_filename))
            {
                // 檔案不存在，創建檔案並寫入初始內容
                File.WriteAllText(last_filename, "0,讀取PMC Message\r\n1,讀取PMC Alarm\r\n"); // 你可以改成你需要的初始內容
            }
            //macro讀取多國語言
            Units.MacroInfo = new MacroInfo(Application.StartupPath + "\\macro.xml");

            string tmpFileName = Application.StartupPath + "\\DefaultProcess.xml";
            if (!File.Exists(tmpFileName)) Fo_Msg.Show("DefaultProcess.xml " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
            else
            {
                XDocument xmlDefaultProcess = new XDocument();//DefaultProcess.xml
                xmlDefaultProcess = XDocument.Load(tmpFileName);//讀取XML檔案
                Units.ProcessList = LoadProcessList(xmlDefaultProcess);//讀取XML到ProcessList (不包含語言)
            }

            tmpFileName = Application.StartupPath + "\\MachineSetting.xml";
            if (!File.Exists(tmpFileName)) Fo_Msg.Show("MachineSetting.xml " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));

            SetLanguage(lang);


            #endregion 多國語言

            la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常");

            dManualZeroPoint = ini.ReadFloat("System", "dManualZeroPoint", 0);


            IPAddress = ini.ReadString("CNC", "IP", "192.168.168.2");
            Port = ini.ReadInteger("CNC", "Port", 8193);


            SetLayout();

            this.LoadLanguageFile(Units.langfile, this.Name);

            LoadProgramDB();//從檔案讀取程式庫

            //最後載入的主程式
            MainProgram = ini.ReadString("System", "MainProgram", "");
            foreach (TProgram pg in Units.ProgramDB.Programs)
            {
                if (pg.Name == MainProgram)
                {
                    OpenProgram(pg, false); //建構子，載入程式不用問是否要清除補正值
                    InitProcessExe(pg); //初始化工序開關(全開)
                    RefleshProgramLayout();//畫面重新整理
                    bReadProcessExe = true; //從控制器讀回工序開關(建構子中還尚未連線，由執行序處理)
                    break;
                }
            }


            //砂輪通訊設定
            serialPort1.PortName = ini.ReadString("Serial", "Port", "COM1");
            serialPort1.BaudRate = ini.ReadInteger("Serial", "BaudRate", 9600);
            serialPort1.DataBits = ini.ReadInteger("Serial", "DataBits", 8);
            serialPort1.StopBits = (StopBits)ini.ReadInteger("Serial", "StopBits", 1);
            serialPort1.Parity = (Parity)ini.ReadInteger("Serial", "Parity", 2);
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string p in ports)
                {
                    if (p == serialPort1.PortName)
                    {
                        serialPort1.Open();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!serialPort1.IsOpen)
            {
                tb_Debug.AppendText("串列埠開啟異常，請洽原廠技術人員。\r\n");
                pic_RS485error.Visible = true;
                //LB_CurrentAlarm.Items.Add("串列埠開啟異常。");
                //CurrentAlarm.Items.Add(new Alarm("","串列埠開啟異常。", "請洽原廠技術人員。"));
                //MessageBox.Show("串列埠開啟異常，請洽原廠技術人員。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //砂輪通訊設定
            serialPort2.PortName = ini.ReadString("Serial2", "Port", "COM1");
            serialPort2.BaudRate = ini.ReadInteger("Serial2", "BaudRate", 9600);
            serialPort2.DataBits = ini.ReadInteger("Serial2", "DataBits", 8);
            serialPort2.StopBits = (StopBits)ini.ReadInteger("Serial2", "StopBits", 1);
            serialPort2.Parity = (Parity)ini.ReadInteger("Serial2", "Parity", 2);

            if (SpindleDev == 1 && SpindleChIndex == 1 && SP_Comm_Enabled)
            {
                try
                {
                    string[] ports = SerialPort.GetPortNames();
                    foreach (string p in ports)
                    {
                        if (p == serialPort2.PortName)
                        {
                            serialPort2.Open();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (!serialPort2.IsOpen)
                {
                    tb_Debug.AppendText("串列埠開啟異常，請洽原廠技術人員。\r\n");
                    pic_RS422error.Visible = true;
                    //LB_CurrentAlarm.Items.Add("串列埠開啟異常。");
                    //CurrentAlarm.Items.Add(new Alarm("","串列埠開啟異常。", "請洽原廠技術人員。"));
                    //MessageBox.Show("串列埠開啟異常，請洽原廠技術人員。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //砂輪1站號
            Gw1.Slave = ini.ReadInteger("Gw1", "Slave", 1);
            //倍率(RPM/Hz)
            Gw1.Rate = ini.ReadFloat("Gw1", "Rate", 30);
            //顯示轉速倍率
            Gw1.ShowRate = ini.ReadFloat("Gw1", "ShowRate", 1);
            //軟體上限
            Gw1.MaxRpm = ini.ReadInteger("Gw1", "MaxRpm", 1600);
            //軟體下限
            Gw1.MinRpm = ini.ReadInteger("Gw1", "MinRpm", 0);
            //單位(0.1 Hz)
            Gw1.Unit = ini.ReadFloat("Gw1", "Unit", 0.01);
            //最後設定的指令速度(Hz)
            Gw1.CmdSpeed = ini.ReadFloat("Gw1", "Cmd", 0);
            btn_Gw1CmdRpm.DisplayText = (Gw1.CmdSpeed * Gw1.Rate).ToString("0");



            //砂輪2站號
            Gw2.Slave = ini.ReadInteger("Gw2", "Slave", 2);
            //倍率(RPM/Hz)
            Gw2.Rate = ini.ReadFloat("Gw2", "Rate", 30);
            //顯示轉速倍率
            Gw2.ShowRate = ini.ReadFloat("Gw2", "ShowRate", 1);
            //軟體上限
            Gw2.MaxRpm = ini.ReadInteger("Gw2", "MaxRpm", 1600);
            //軟體下限
            Gw2.MinRpm = ini.ReadInteger("Gw2", "MinRpm", 0);
            //單位(1 RPM)
            Gw2.Unit = ini.ReadFloat("Gw2", "Unit", 0.01);
            //最後設定的指令速度
            Gw2.CmdSpeed = ini.ReadFloat("Gw2", "Cmd", 0);
            btn_Gw2CmdRpm.DisplayText = (Gw2.CmdSpeed * Gw2.Rate).ToString("0");

            //砂輪3站號
            Gw3.Slave = ini.ReadInteger("Gw3", "Slave", 2);
            //倍率(RPM/Hz)
            Gw3.Rate = ini.ReadFloat("Gw3", "Rate", 30);
            //顯示轉速倍率
            Gw3.ShowRate = ini.ReadFloat("Gw3", "ShowRate", 1);
            //軟體上限
            Gw3.MaxRpm = ini.ReadInteger("Gw3", "MaxRpm", 1600);
            //軟體下限
            Gw3.MinRpm = ini.ReadInteger("Gw3", "MinRpm", 0);
            //單位(1 RPM)
            Gw3.Unit = ini.ReadFloat("Gw3", "Unit", 0.01);
            //最後設定的指令速度
            Gw3.CmdSpeed = ini.ReadFloat("Gw3", "Cmd", 0);
            btn_Gw3CmdRpm.DisplayText = (Gw3.CmdSpeed * Gw3.Rate).ToString("0");

            //砂輪4站號
            Gw4.Slave = ini.ReadInteger("Gw4", "Slave", 2);
            //倍率(RPM/Hz)
            Gw4.Rate = ini.ReadFloat("Gw4", "Rate", 30);
            //顯示轉速倍率
            Gw4.ShowRate = ini.ReadFloat("Gw4", "ShowRate", 1);
            //軟體上限
            Gw4.MaxRpm = ini.ReadInteger("Gw4", "MaxRpm", 1600);
            //軟體下限
            Gw4.MinRpm = ini.ReadInteger("Gw4", "MinRpm", 0);
            //單位(1 RPM)
            Gw4.Unit = ini.ReadFloat("Gw4", "Unit", 0.01);
            //最後設定的指令速度
            Gw4.CmdSpeed = ini.ReadFloat("Gw4", "Cmd", 0);
            btn_Gw4CmdRpm.DisplayText = (Gw4.CmdSpeed * Gw4.Rate).ToString("0");

            //動平衡站號
            BalanceSlave = ini.ReadInteger("Balance", "Slave", 99);

            //滾輪站號
            Roller.Slave = ini.ReadInteger("Roller", "Slave", 4);
            //倍率(RPM/Hz)
            Roller.Rate = ini.ReadFloat("Roller", "Rate", 30);
            //顯示轉速倍率
            Roller.ShowRate = ini.ReadFloat("Roller", "ShowRate", 1);
            //最大轉速
            Roller.MaxRpm = ini.ReadInteger("Roller", "MaxRpm", 1600);
            //最小轉速
            Roller.MinRpm = ini.ReadInteger("Roller", "MinRpm", 0);
            //單位(1 RPM)
            Roller.Unit = ini.ReadFloat("Roller", "Unit", 0.01);
            //最後設定的指令速度
            Roller.CmdSpeed = ini.ReadFloat("Roller", "Cmd", 0);
            btn_RollerCmdSpeed.DisplayText = (Roller.CmdSpeed * Roller.Rate).ToString("0");



            //工件主軸(三菱驅動器)
            Spindle.Slave = ini.ReadInteger("Spindle", "Slave", 5);
            //倍率(%)
            Spindle.Rate = ini.ReadFloat("Spindle", "Rate", 1);
            //顯示倍率
            Spindle.ShowRate = ini.ReadFloat("Spindle", "ShowRate", 1);
            //最大轉速
            Spindle.MaxRpm = ini.ReadInteger("Spindle", "MaxRpm", 500);
            //最小轉速
            Spindle.MinRpm = ini.ReadInteger("Spindle", "MinRpm", 0);
            //單位
            Spindle.Unit = ini.ReadFloat("Spindle", "Unit", 1);
            //最後設定的速度
            Spindle.CmdSpeed = ini.ReadInteger("Spindle", "Cmd", 50);
            UserSCode = (int)Spindle.CmdSpeed;
            btn_SpSpeed.DisplayText = UserSCode.ToString();

            //disableClickTimer = new System.Windows.Forms.Timer();
            //disableClickTimer.Interval = 500; // 延遲 500 毫秒
            //disableClickTimer.Tick += (s, e) =>
            //{
            //    isCellClickDisabled = false;
            //    disableClickTimer.Stop();
            //};

            RefleshQueryList();

        }

        public List<TProcess> LoadProcessList(XDocument xmlDefaultProcess)
        {
            List<TProcess> ProcessList = new List<TProcess>();
            //取得所有XML的工序節點
            var processes = xmlDefaultProcess.Descendants("Process");
            foreach (var process in processes)
            {
                //新增一筆工序
                TProcess p = new TProcess();
                string ProcID = process.Attribute("ID")?.Value; //工序ID 內圓1~9, 外圓11~20, 量測201~203, 自訂程式999                                                                //例外處理
                if (string.IsNullOrEmpty(ProcID))
                {
                    Fo_Msg.Show("[DefaultProcess.xml] Format Error : ID Empty", "Error.");
                    return ProcessList;
                }
                int.TryParse(ProcID, out int id);
                if (id == 0)
                {
                    Fo_Msg.Show("[DefaultProcess.xml] Format Error : ID Zero.", "Error.");
                    return ProcessList;
                }
                p.ID = id;

                //新增一筆加工程式
                string ProgNo = process.Attribute("ProgNo")?.Value; //程式號 O9010~9018, O9020~O9029, O9702~O9704

                if (id != 999 && string.IsNullOrEmpty(ProgNo))
                {
                    Fo_Msg.Show("[DefaultProcess.xml] Format Error : ProgNo Empty", "Error.");
                    return ProcessList;
                }
                TSubProgram subPrograms = new TSubProgram();//固定一個
                int.TryParse(ProgNo, out int no);
                subPrograms.ProgNo = no;

                //加入P Code 資料
                foreach (var pcode in process.Elements("PCode"))
                {
                    TArgument a = new TArgument();
                    if (int.TryParse(pcode.Attribute("pages")?.Value, out int type)) a.Type = type; //參數1, 參數2, 參數3, 進階, 修整1, 修整2, 隱藏
                    a.AddrCode = pcode.Attribute("PCode")?.Value; //P Code Macro
                    if (double.TryParse(pcode.Attribute("Show")?.Value, out double show)) a.Value = show; //預設值
                    if (double.TryParse(pcode.Attribute("Min")?.Value, out double min)) a.Min = min; // 最小值
                    if (double.TryParse(pcode.Attribute("Max")?.Value, out double max)) a.Max = max; // 最大值
                    a.Code = pcode.Attribute("Code")?.Value;//代碼
                    a.Unit = pcode.Attribute("Unit")?.Value;//單位
                    subPrograms.Arguments.Add(a);
                }

                p.SubPrograms.Add(subPrograms);

                ProcessList.Add(p);
            }
            return ProcessList;
        }
        private void BTN_Click(object sender, EventArgs e)
        {

            //按下多段橫進刀功能 跳出編輯視窗
            BTN.Visible = false;

            if (fo_TraverseStep == null)
            {
                fo_TraverseStep = new Fo_TraverseStep();
                fo_TraverseStep.TopLevel = false;
                fo_TraverseStep.Parent = this;
                fo_TraverseStep.Left = 0;
                fo_TraverseStep.Top = 80;
            }

            //依照目前是第幾段讀取指定範圍 P CODE
            //工序1 (20001~20200) 
            //工序2 (20201~20400)
            //...依此類推


            fo_TraverseStep.Show();
            fo_TraverseStep.BringToFront();

            Application.DoEvents();


            if (TempProcess == null) return; //例外處理
            if (TempProcess.SubPrograms.Count == 0) return;//例外處理
            if (TempProcess.SubPrograms[0] == null) return;//例外處理
            int index;
            TArgument a;

            //#19949	多段橫進刀功能 0：OFF  1：ON
            a = TempProcess.SubPrograms[0].GetArgument("19949");
            index = (int)Math.Round(a.Value);
            fo_TraverseStep.cb_Enabled.SelectedIndex = index;

            //#19950	多段橫進刀段數
            a = TempProcess.SubPrograms[0].GetArgument("19950");
            int steps = (int)Math.Round(a.Value);
            if (steps <= 0) steps = 1;
            index = steps - 1;

            fo_TraverseStep.cb_Step.SelectedIndex = index;

            //讀取資料
            fo_TraverseStep.dataGridView1.Rows.Clear();
            TSubProgram sp = TempProcess.SubPrograms[0];
            for (int i = 0; i < steps; i++) //最小值為1
            {
                TArgument xma = sp.GetArgument((19951 + i).ToString());
                double xm = 0;
                if (xma != null) xm = xma.Value;

                TArgument zma = sp.GetArgument((19971 + i).ToString());
                double zm = 0;
                if (zma != null) zm = zma.Value;

                fo_TraverseStep.dataGridView1.Rows.Add((i + 1).ToString(), xm.ToString(Units.DisplayFmt), zm.ToString(Units.DisplayFmt));
            }
        }

        public void LoadTraverseData()
        {
            if (TempProcess == null) return; //例外處理
            if (TempProcess.SubPrograms.Count == 0) return;//例外處理
            if (TempProcess.SubPrograms[0] == null) return;//例外處理
            TArgument a;

            //#19949	多段橫進刀功能 0：OFF  1：ON
            a = TempProcess.SubPrograms[0].GetArgument("19949");
            a.Value = fo_TraverseStep.cb_Enabled.SelectedIndex;

            //#19950	多段橫進刀段數
            a = TempProcess.SubPrograms[0].GetArgument("19950");
            int steps = fo_TraverseStep.cb_Step.SelectedIndex + 1;
            a.Value = steps;

            fo_TraverseStep.Close();
            fo_TraverseStep = null;

            SetProcessData(TempProcess);

            TC_EditProc.SelectedTab = tab_Advance;
            DGV_CellClick(DGV_Advance, null);

            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
        }

        public void SetPanelLayout()
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");

            Panel[] panels = new Panel[]{
                pa_MonitorAbs, pa_MonitorDistToGo, pa_MonitorMach,pa_Monitor_DC,pa_Monitor_Info, pa_GrindInfo,
                pa_Spindle,pa_Monitor_Prog,pa_Monitor_GW1,pa_Monitor_GW2,pa_Monitor_GW3,pa_Monitor_GW4,
                pa_MaintainDoor,pa_Roller,pa_Language, pa_ProcessParam, pa_ScreenDisplay,
                pa_Balance, pa_PositionSet, pa_RunSpindle, pa_ImportProg,pa_Warmup,
                pa_Spindle3, pa_FuncSW,pa_CNCDataManager};

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Visible = ini.ReadInteger("UI", panels[i].Name, panels[i].Visible ? 1 : 0) == 1;
                panels[i].Left = ini.ReadInteger("UI", panels[i].Name + "_Left", panels[i].Left);
                panels[i].Top = ini.ReadInteger("UI", panels[i].Name + "_Top", panels[i].Top);
                panels[i].Width = ini.ReadInteger("UI", panels[i].Name + "_Width", panels[i].Width);
                panels[i].Height = ini.ReadInteger("UI", panels[i].Name + "_Height", panels[i].Height);
            }

        }

        private void LoadSoftPanelFromXML(string filename)
        {
            //檔案不存在
            if (!File.Exists(filename))
            {
                Fo_Msg.Show("SoftPanel.xml " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
                return;
            }

            //清除所有讀取PB燈號的清單
            SoftPBLamps.Clear();
            PMC_Values.Clear();

            //讀取XML檔案
            xmlDocument.Load(filename);

            //清除舊的按鍵
            pa_SoftPanel.Controls.Clear();

            //取得根元素
            XmlNode root_x = xmlDocument.DocumentElement;
            XmlNode current_x;

            List<XmlNode> stack_x = new List<XmlNode>();

            //加入第一個元素(根)
            stack_x.Add(root_x);

            //預先讀取圖片
            string str_btn_up_file = Application.StartupPath + "\\image\\Btn_S3_60x60.png";
            string str_btn_down_file = Application.StartupPath + "\\image\\Btn_S3_60x60_L.png";
            string str_btn_lamp_file = Application.StartupPath + "\\image\\Btn_S3_60x60_GrayL3.png";
            Image img_btn_up = null;
            Image img_btn_down = null;
            Image img_btn_lamp = null;
            if (File.Exists(str_btn_up_file)) img_btn_up = Image.FromFile(str_btn_up_file);
            if (File.Exists(str_btn_down_file)) img_btn_down = Image.FromFile(str_btn_down_file);
            if (File.Exists(str_btn_lamp_file)) img_btn_lamp = Image.FromFile(str_btn_lamp_file);
            int max_height = 0;
            try
            {
                //開始解析XML檔
                while (stack_x.Count > 0)
                {
                    //從堆疊取出一個元素
                    current_x = stack_x[0];

                    if (current_x.Name == "Panel")
                    {
                        Panel p = new Panel();
                        p.Parent = pa_SoftPanel;
                        int.TryParse(current_x.Attributes["Left"].Value, out int left);
                        int.TryParse(current_x.Attributes["Top"].Value, out int top);
                        int.TryParse(current_x.Attributes["Width"].Value, out int width);
                        int.TryParse(current_x.Attributes["Height"].Value, out int height);
                        p.Left = left;
                        p.Top = top;
                        p.Width = width;
                        p.Height = height;
                        p.BorderStyle = BorderStyle.FixedSingle;

                        if ((p.Top + p.Height) > max_height) max_height = p.Top + p.Height;

                        Label label = new Label();
                        label.Text = current_x.Attributes["Title"].Value;
                        p.Name = "pa_" + label.Name;
                        label.Dock = DockStyle.Top;
                        label.Font = new Font(new FontFamily("Times New Roman"), 18, FontStyle.Bold);
                        label.ForeColor = Color.Yellow;
                        label.BackColor = Color.DarkBlue;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.AutoSize = false;
                        label.Height = 32;
                        label.Parent = p;

                        for (int i = 0; i < current_x.ChildNodes.Count; i++)
                        {
                            var x = current_x.ChildNodes[i];
                            if (x.Name == "Button")
                            {
                                Uc_RoundBtn btn = new Uc_RoundBtn();
                                int.TryParse(x.Attributes["Left"].Value, out int btn_left);
                                int.TryParse(x.Attributes["Top"].Value, out int btn_top);
                                int.TryParse(x.Attributes["Width"].Value, out int btn_width);
                                int.TryParse(x.Attributes["Height"].Value, out int btn_height);
                                btn.Left = btn_left;
                                btn.Top = btn_top;
                                btn.Width = btn_width;
                                btn.Height = btn_height;
                                btn.SizeMode = PictureBoxSizeMode.CenterImage;
                                btn.Tag = x.Attributes["Singal"].Value;
                                btn.Name = x.Attributes["Title"].Value;

                                //按下事件
                                string even_name = x.Attributes["MouseUp"].Value;
                                if (even_name == "PB_MouseUp")
                                {
                                    btn.MouseUp += new MouseEventHandler(PB_MouseUp);
                                }

                                //放開事件
                                even_name = x.Attributes["MouseDown"].Value;
                                if (even_name == "PB_MouseDown")
                                {
                                    btn.MouseDown += new MouseEventHandler(PB_MouseDown);
                                }



                                //點擊事件
                                even_name = x.Attributes["Click"].Value;
                                if (even_name == "btn_ToProbe_Click")//轉頭(端測站)
                                {
                                    btn.Click += new EventHandler(btn_ToProbe_Click);
                                    btn_ToProbe = btn;
                                }
                                else if (even_name == "btn_SVO_Click")//SVO
                                {
                                    btn.Click += new EventHandler(btn_SVO_Click);
                                }
                                else if (even_name == "PB_SwitchClick")
                                {
                                    btn.Click += new EventHandler(PB_SwitchClick);
                                }

                                //圖片
                                string file = Application.StartupPath + "\\image\\SoftPanel\\" + x.Attributes["Image"].Value;
                                if (File.Exists(file)) btn.Image = Image.FromFile(file);
                                btn.MouseDownImage = img_btn_down;
                                btn.MouseUpImage = img_btn_up;
                                btn.LampOnImage = img_btn_lamp;
                                btn.Parent = p;

                                //燈號 (EX: E2525.0)
                                string pmc_lamp = x.Attributes["Lamp"].Value;
                                if (pmc_lamp != "")
                                {
                                    PmcAddrType type; //取得英文
                                    if (pmc_lamp[0] == 'E') type = PmcAddrType.E;
                                    else if (pmc_lamp[0] == 'R') type = PmcAddrType.R;
                                    else if (pmc_lamp[0] == 'X') type = PmcAddrType.X;
                                    else if (pmc_lamp[0] == 'Y') type = PmcAddrType.Y;
                                    else if (pmc_lamp[0] == 'F') type = PmcAddrType.F;
                                    else if (pmc_lamp[0] == 'G') type = PmcAddrType.G;
                                    else if (pmc_lamp[0] == 'D') type = PmcAddrType.D;
                                    else
                                    {
                                        MessageBox.Show("Read Lamp PMC Type Error.");
                                        return;
                                    }

                                    //取得小數點位置
                                    int dot_index = pmc_lamp.IndexOf('.');
                                    if (dot_index != -1)
                                    {
                                        //EX: E2525
                                        string addr_without_dot = pmc_lamp.Substring(0, dot_index);
                                        PMC_Values[addr_without_dot] = 0;//增加一個要讀取的暫存器(重複的不會增加)
                                    }

                                    //數值分段
                                    string[] csv = pmc_lamp.Substring(1).Split('.');
                                    if (csv.Length == 2) //正常會有兩個
                                    {
                                        int.TryParse(csv[0], out int addr);
                                        int.TryParse(csv[1], out int bit);
                                        SoftPBLamps.Add(new SoftPBLamp(btn, type, addr, bit));
                                    }
                                }
                            }
                        }
                    }

                    //將目前元素中的子元素加入堆疊
                    for (int i = 0; i < current_x.ChildNodes.Count; i++)
                    {
                        var x = current_x.ChildNodes[i];
                        if (x.NodeType == XmlNodeType.Text) break;
                        if (x.Name == "Panel") stack_x.Add(x);
                    }

                    //此元素已經處理完，刪除第一個元素
                    stack_x.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            pa_SoftPanel.Height = max_height + 8;

        }

        private void SoftPanelLayout()
        {



            pa_SoftPanel.Width = this.Width - pa_ModeSelect.Width;
            //pa_SoftPanel.Height = 392;
            pa_SoftPanel.Parent = this;
            pa_SoftPanel.Visible = true;
            pa_SoftPanel.BringToFront();



            //用縮小來隱藏未使用的按鍵
            //pa_SoftPanel_Standard.Height = 256;
            //pa_SoftPanel_Axis.Height = 256;
            //pa_SoftPanel_Options.Height = 256;


            //throw new NotImplementedException();
        }

        public void RefleshQueryList()
        {
            masterSerialBus1.QueryList.Clear();
            masterSerialBus2.QueryList.Clear();
            if (GW1_Comm_Enabled)
            {
                if (Gw1Dev == 0)//士林變頻器
                {
                    //傳送指令速度到變頻器
                    //masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                    //士林變頻器 讀取監視參數(指令頻率、輸出頻率...) 
                    masterSerialBus1.QueryList.Add(this.Gw1.Slave.ToString("X2") + "0310020008");
                }
                else if (Gw1Dev == 1)//台達變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw1.Slave.ToString("X2") + "0321000005");
                }
                else if (Gw1Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw1.Slave.ToString("X2") + "0300C8000E");
                }
            }
            if (GW2_Comm_Enabled)
            {
                if (Gw2Dev == 0)//士林變頻器
                {
                    //傳送指令速度到變頻器
                    //masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                    //士林變頻器 讀取監視參數(指令轉速、輸出電流...) 
                    masterSerialBus1.QueryList.Add(this.Gw2.Slave.ToString("X2") + "0310020008");
                }
                else if (Gw2Dev == 1)//台達變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw2.Slave.ToString("X2") + "0321000005");
                }
                else if (Gw2Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw2.Slave.ToString("X2") + "0300C8000E");
                }
            }
            if (GW3_Comm_Enabled)
            {
                if (Gw3Dev == 0)//士林變頻器
                {
                    //傳送指令速度到變頻器
                    //masterSerialBus1.Add(this.Gw3.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                    //士林變頻器 讀取監視參數(指令轉速、輸出電流...) 
                    masterSerialBus1.QueryList.Add(this.Gw3.Slave.ToString("X2") + "0310020008");
                }
                else if (Gw3Dev == 1)//台達變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw3.Slave.ToString("X2") + "0321000005");
                }
                else if (Gw3Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw3.Slave.ToString("X2") + "0300C8000E");
                }
            }
            if (GW4_Comm_Enabled)
            {
                if (Gw4Dev == 0)//士林變頻器
                {
                    //傳送指令速度到變頻器
                    //masterSerialBus1.Add(this.Gw4.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                    //士林變頻器 讀取監視參數(指令轉速、輸出電流...) 
                    masterSerialBus1.QueryList.Add(this.Gw4.Slave.ToString("X2") + "0310020008");
                }
                else if (Gw4Dev == 1)//台達變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw4.Slave.ToString("X2") + "0321000005");
                }
                else if (Gw4Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Gw4.Slave.ToString("X2") + "0300C8000E");
                }
            }

            if (Roller_Comm_Enabled)
            {
                if (RollerDev == 0)//士林變頻器
                {
                    //傳送指令速度到變頻器
                    //masterSerialBus1.Add(this.Roller.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Roller.CmdSpeed / Roller.Unit)).ToString("X4"));
                    //士林變頻器 讀取監視參數(指令轉速、輸出電流...) 
                    masterSerialBus1.QueryList.Add(this.Roller.Slave.ToString("X2") + "0310020008");
                }
                else if (RollerDev == 1)//三菱變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Roller.Slave.ToString("X2") + "0300C8000E");
                }
            }
            if (SP_Comm_Enabled)
            {
                if (SpindleDev == 0)//三菱驅動器
                {
                    masterSerialBus1.QueryList.Add(Spindle.Slave.ToString("X2") + "032B020002");
                }
                else if (SpindleDev == 1)//安川驅動器
                {
                    //安川驅動器 讀取監視參數(指令轉速、輸出電流...) 0xE000~0xE002
                    if (SpindleChIndex == 0)//RS485
                        masterSerialBus1.QueryList.Add(this.Spindle.Slave.ToString("X2") + "4001030000E0000003");
                    if (SpindleChIndex == 1)//RS422
                        masterSerialBus2.QueryList.Add(this.Spindle.Slave.ToString("X2") + "4001030000E0000003");
                }
                else//士林變頻器
                {
                    masterSerialBus1.QueryList.Add(this.Spindle.Slave.ToString("X2") + "0310020008");
                }
            }
        }

        private void CB_DropDownClosed(object sender, EventArgs e)
        {
            CB.Visible = false;
        }

        private void CB2_DropDownClosed(object sender, EventArgs e)
        {
            CB2.Visible = false;
        }

        private void ReadGwIni()
        {
            //Initialize
            //for (int i = 0; i < 500; i++)
            //{
            //    CurrentGw1.Add(i + 500, 0);
            //    CurrentGw2.Add(i + 500, 0);
            //}

            TIniFile gw_ini = new TIniFile(Application.StartupPath + "\\GwMacro.ini");
            //CurrentGw1[505] = gw_ini.ReadFloat("GW1", "#505", 0);//砂輪1修整模式
            //CurrentGw1[508] = gw_ini.ReadFloat("GW1", "#508", 0);//砂輪可使用至最小寬度
            //CurrentGw1[509] = gw_ini.ReadFloat("GW1", "#509", 0);//砂輪目前寬度
            //CurrentGw1[510] = gw_ini.ReadFloat("GW1", "#510", 0);//砂輪可使用至最小外徑
            //CurrentGw1[511] = gw_ini.ReadFloat("GW1", "#511", 0);//砂輪目前目前外徑
            //CurrentGw1[512] = gw_ini.ReadFloat("GW1", "#512", 0);//砂輪右側偏擺量 -0.1~0.1
            //CurrentGw1[513] = gw_ini.ReadFloat("GW1", "#513", 0);//砂輪修整計數
            //CurrentGw1[514] = gw_ini.ReadFloat("GW1", "#514", 0);//砂輪修整次數設定
            //CurrentGw1[515] = gw_ini.ReadFloat("GW1", "#515", 0);//空修次數設定
            //CurrentGw1[516] = gw_ini.ReadFloat("GW1", "#516", 0);//外徑修整量(條件)
            //CurrentGw1[517] = gw_ini.ReadFloat("GW1", "#517", 0);//外徑修整速度(條件)
            //CurrentGw1[518] = gw_ini.ReadFloat("GW1", "#518", 0);//外徑中凸量(形狀)
            //CurrentGw1[519] = gw_ini.ReadFloat("GW1", "#519", 0);//0:左側修整 1:右側修整
            //CurrentGw1[520] = gw_ini.ReadFloat("GW1", "#520", 0);//外徑往復 0：無往復  1：往復(純粹外徑修整時)(功能)
            //CurrentGw1[521] = gw_ini.ReadFloat("GW1", "#521", 0);//砂輪左側超切行程(形狀)
            //CurrentGw1[522] = gw_ini.ReadFloat("GW1", "#522", 0);//砂輪右側超切行程(形狀)
            //CurrentGw1[523] = gw_ini.ReadFloat("GW1", "#523", 0);//外徑-修整預留量(條件)
            //CurrentGw1[524] = gw_ini.ReadFloat("GW1", "#524", 0);//左側修整量(條件)
            //CurrentGw1[525] = gw_ini.ReadFloat("GW1", "#525", 0);//左側修整速度(條件)
            //CurrentGw1[526] = gw_ini.ReadFloat("GW1", "#526", 0);//左側幅長(形狀)
            //CurrentGw1[527] = gw_ini.ReadFloat("GW1", "#527", 0);//左側逃離長(形狀)
            //CurrentGw1[528] = gw_ini.ReadFloat("GW1", "#528", 0);//左側逃離量(形狀)
            //CurrentGw1[529] = gw_ini.ReadFloat("GW1", "#529", 0);//左側R角(形狀)
            //CurrentGw1[531] = gw_ini.ReadFloat("GW1", "#531", 0);//左側-修整預留量(條件)
            //CurrentGw1[532] = gw_ini.ReadFloat("GW1", "#532", 0);//右側修整量(條件)(內圓預留)
            //CurrentGw1[533] = gw_ini.ReadFloat("GW1", "#533", 0);//右側修整速度(條件)(內圓預留)
            //CurrentGw1[534] = gw_ini.ReadFloat("GW1", "#534", 0);//右側幅長(形狀)(內圓預留)
            //CurrentGw1[535] = gw_ini.ReadFloat("GW1", "#535", 0);//右側逃離長(形狀)(內圓預留)
            //CurrentGw1[536] = gw_ini.ReadFloat("GW1", "#536", 0);//右側逃離量(形狀)(內圓預留)
            //CurrentGw1[537] = gw_ini.ReadFloat("GW1", "#537", 0);//右側R角(形狀)(內圓預留)
            //CurrentGw1[539] = gw_ini.ReadFloat("GW1", "#539", 0);//右側-修整預留量(條件)(內圓預留)
            //CurrentGw1[540] = gw_ini.ReadFloat("GW1", "#540", 0);//錐度修整平面寬度(形狀)
            //CurrentGw1[541] = gw_ini.ReadFloat("GW1", "#541", 0);//錐度修整錐度寬度(形狀)
            //CurrentGw1[542] = gw_ini.ReadFloat("GW1", "#542", 0);//錐度修整角度(形狀)

            Gw1SizeTipDiameter = gw_ini.ReadFloat("GW1", "SizeTipDiameter", 0);//外徑小於修砂提示值
            //成型修整
            //Gw1_OD_Path_DefDressRpm = gw_ini.ReadFloat("GW1", "OD_Path_DefDressRpm", 0);//預設修整速度
            //Gw1_OD_Path_Offset = gw_ini.ReadBool("GW1", "OD_Path_Offset", false);//補正功能
            //Gw1_OD_Path_ToolDiam = gw_ini.ReadFloat("GW1", "OD_Path_ToolDiam", 0);//刀鼻徑值
            //Gw1Angle = gw_ini.ReadFloat("GW1", "Angle", 0);
            /*
            if (File.Exists(Application.StartupPath + "\\GW1_OD_Path.txt"))
            {
                String[] lines = File.ReadAllLines(Application.StartupPath + "\\GW1_OD_Path.txt");//讀取砂輪1所有路徑(含左側、右側、外徑)
                if (lines != null) Gw1_OD_Path = lines.ToList();//例外處理
            }*/
            Gw1_DressBase_MaxPos = gw_ini.ReadFloat("GW1", "DressBase_MaxPos", 0);//修整座最大位置 參數 6934
            Gw1_DressBase_MinPos = gw_ini.ReadFloat("GW1", "DressBase_MinPos", 0);//修整座最小位置 參數 6954

            //CurrentGw2[504] = gw_ini.ReadFloat("GW2", "#504", 0);//砂輪2修整模式
            //CurrentGw2[548] = gw_ini.ReadFloat("GW2", "#548", 0);//砂輪可使用至最小寬度
            //CurrentGw2[549] = gw_ini.ReadFloat("GW2", "#549", 0);//砂輪目前寬度
            //CurrentGw2[550] = gw_ini.ReadFloat("GW2", "#550", 0);//砂輪可使用至最小外徑
            //CurrentGw2[551] = gw_ini.ReadFloat("GW2", "#551", 0);//砂輪目前目前外徑
            //CurrentGw2[552] = gw_ini.ReadFloat("GW2", "#552", 0);//砂輪右側偏擺量 -0.1~0.1
            //CurrentGw2[553] = gw_ini.ReadFloat("GW2", "#553", 0);//砂輪修整計數
            //CurrentGw2[554] = gw_ini.ReadFloat("GW2", "#554", 0);//砂輪修整次數設定
            //CurrentGw2[555] = gw_ini.ReadFloat("GW2", "#555", 0);//空修次數設定
            //CurrentGw2[556] = gw_ini.ReadFloat("GW2", "#556", 0);//外徑修整量(條件)
            //CurrentGw2[557] = gw_ini.ReadFloat("GW2", "#557", 0);//外徑修整速度(條件)
            //CurrentGw2[558] = gw_ini.ReadFloat("GW2", "#558", 0);//外徑中凸量(形狀)
            //CurrentGw2[559] = gw_ini.ReadFloat("GW2", "#559", 0);//0:左側修整 1:右側修整
            //CurrentGw2[560] = gw_ini.ReadFloat("GW2", "#560", 0);//外徑往復 0：無往復  1：往復(純粹外徑修整時)(功能)
            //CurrentGw2[561] = gw_ini.ReadFloat("GW2", "#561", 0);//砂輪左側超切行程(形狀)
            //CurrentGw2[562] = gw_ini.ReadFloat("GW2", "#562", 0);//砂輪右側超切行程(形狀)
            //CurrentGw2[563] = gw_ini.ReadFloat("GW2", "#563", 0);//外徑-修整預留量(條件)
            //CurrentGw2[564] = gw_ini.ReadFloat("GW2", "#564", 0);//左側修整量(條件)
            //CurrentGw2[565] = gw_ini.ReadFloat("GW2", "#565", 0);//左側修整速度(條件)
            //CurrentGw2[566] = gw_ini.ReadFloat("GW2", "#566", 0);//左側幅長(形狀)
            //CurrentGw2[567] = gw_ini.ReadFloat("GW2", "#567", 0);//左側逃離長(形狀)
            //CurrentGw2[568] = gw_ini.ReadFloat("GW2", "#568", 0);//左側逃離量(形狀)
            //CurrentGw2[569] = gw_ini.ReadFloat("GW2", "#569", 0);//左側R角(形狀)
            //CurrentGw2[571] = gw_ini.ReadFloat("GW2", "#571", 0);//左側-修整預留量(條件)
            //CurrentGw2[572] = gw_ini.ReadFloat("GW2", "#572", 0);//右側修整量(條件)(內圓預留)
            //CurrentGw2[573] = gw_ini.ReadFloat("GW2", "#573", 0);//右側修整速度(條件)(內圓預留)
            //CurrentGw2[574] = gw_ini.ReadFloat("GW2", "#574", 0);//右側幅長(形狀)(內圓預留)
            //CurrentGw2[575] = gw_ini.ReadFloat("GW2", "#575", 0);//右側逃離長(形狀)(內圓預留)
            //CurrentGw2[576] = gw_ini.ReadFloat("GW2", "#576", 0);//右側逃離量(形狀)(內圓預留)
            //CurrentGw2[577] = gw_ini.ReadFloat("GW2", "#577", 0);//右側R角(形狀)(內圓預留)
            //CurrentGw2[579] = gw_ini.ReadFloat("GW2", "#579", 0);//右側-修整預留量(條件)(內圓預留)
            //CurrentGw2[580] = gw_ini.ReadFloat("GW2", "#580", 0);//錐度修整平面寬度(形狀)
            //CurrentGw2[581] = gw_ini.ReadFloat("GW2", "#581", 0);//錐度修整錐度寬度(形狀)
            //CurrentGw2[582] = gw_ini.ReadFloat("GW2", "#582", 0);//錐度修整角度(形狀)
            //CurrentGw2[583] = gw_ini.ReadFloat("GW2", "#583", 0);//錐度+端面時端面逃離長(斜頭)(形狀)


            Gw2SizeTipDiameter = gw_ini.ReadFloat("GW2", "SizeTipDiameter", 0);//外徑小於修砂提示值
            //成型修整
            //Gw2_OD_Path_Offset = gw_ini.ReadBool("GW2", "OD_Path_Offset", false);//補正功能
            //Gw2_OD_Path_DefDressRpm = gw_ini.ReadFloat("GW2", "OD_Path_DefDressRpm", 0);//預設修整速度
            //Gw2_OD_Path_ToolDiam = gw_ini.ReadFloat("GW2", "OD_Path_ToolDiam", 0);//刀鼻徑值
            //Gw2Angle = gw_ini.ReadFloat("GW2", "Angle", 0);
            /*
            if (File.Exists(Application.StartupPath + "\\GW2_OD_Path.txt"))
            {
                String[] lines = File.ReadAllLines(Application.StartupPath + "\\GW2_OD_Path.txt");//讀取砂輪2所有路徑(含左側、右側、外徑)
                if (lines != null) Gw2_OD_Path = lines.ToList();//例外處理
            }*/
            Gw2_DressBase_MaxPos = gw_ini.ReadFloat("GW2", "DressBase_MaxPos", 0);//修整座最大位置 參數 6934
            Gw2_DressBase_MinPos = gw_ini.ReadFloat("GW2", "DressBase_MinPos", 0);//修整座最小位置 參數 6954

        }

        private void Log(String str)
        {
            String FileName = Application.StartupPath + "\\log.txt";

            /*
            if (File.Exists(FileName))
            {
                //c#檔案流讀檔案 
                using (FileStream fsRead = new FileStream(FileName, FileMode.Open))
                {
                    int fsLen = (int)fsRead.Length;
                    byte[] heByte = new byte[fsLen];
                    int r = fsRead.Read(heByte, 0, heByte.Length);
                    string myStr = System.Text.Encoding.Default.GetString(heByte);
                    textBox1.Text = myStr;
                }
            }
            */
            //String AddData = DateTime.Now.ToString() + "\r\n";
            //textBox1.AppendText(AddData);

            using (FileStream fsWrite = new FileStream(FileName, FileMode.Append))
            {
                byte[] array = Encoding.Unicode.GetBytes(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + " " + str + "\r\n");
                fsWrite.Write(array, 0, array.Length);
            }
        }

        public void WriteTCode(TProgram Program)
        {
            if (!focas.IsConnected()) return;
            if (Program == null) return;

            List<String> list = new List<String> { "" };
            int count = Program.Processes.Count;

            //----磨耗P1~P60(Wear)----
            //index0~index29個用在[加工補正]
            for (int i = 0; i < 30; i++)
            {
                if (i < count)
                {
                    double ofs_x = Program.Processes[i].OffsetX.Value;
                    double ofs_z = Program.Processes[i].OffsetZ.Value;
                    double ofs_y = Program.Processes[i].OffsetY.Value;
                    list.Add("G10P" + (i + 1).ToString() + "X" + ofs_x.ToString(Units.DisplayFmt) + "Z" + ofs_z.ToString(Units.DisplayFmt));
                }
                else
                {
                    list.Add("G10P" + (i + 1).ToString() + "X0.0000Z0.0000");
                }
            }
            //index30~index59用在[重修補正]，但[重修補正]必須加上[加工補正]，才傳給CNC
            for (int i = 0; i < 30; i++)
            {
                if (i < count)
                {
                    double ofs_x = Program.Processes[i].OffsetX.RedoValue + Program.Processes[i].OffsetX.Value;
                    double ofs_z = Program.Processes[i].OffsetZ.RedoValue + Program.Processes[i].OffsetZ.Value;
                    double ofs_y = Program.Processes[i].OffsetY.RedoValue + Program.Processes[i].OffsetY.Value;
                    list.Add("G10P" + (i + 31).ToString() + "X" + ofs_x.ToString(Units.DisplayFmt) + "Z" + ofs_z.ToString(Units.DisplayFmt));
                }
                else
                {
                    list.Add("G10P" + (i + 31).ToString() + "X0.0000Z0.0000");
                }
            }

            //----形狀P10001~P10060(Geometry)----
            //for(int i=0; i<30; i++)
            //{
            //list.Add("G10P" + (i + 10001).ToString() + "X" + Units.OffsetX.Geometry[i].ToString(Units.DisplayFmt) + "Z" + Units.OffsetZ.Geometry[i].ToString(Units.DisplayFmt) + "R" + Units.OffsetR.Geometry[i].ToString(Units.DisplayFmt) + "Y0.0000");
            //list.Add("G10P" + (i + 10001).ToString() + "X" + Units.OffsetX.Geometry[i].ToString(Units.DisplayFmt) + "Z" + Units.OffsetZ.Geometry[i].ToString(Units.DisplayFmt) + "Y0.0000");                
            //}

            list.Add("%");

            //File.WriteAllLines(Units.AppPath + "offset.txt", list, Encoding.Default);


            focas.WriteFile(FileType.Tool_Offset, list);
            //if (ret != Focas1.EW_OK)
            //{
            //MessageBox.Show(Units.GetResString("補正寫入失敗") + "(" + ret.ToString() + ")");
            //}
        }


        //從Fanuc讀取補正
        public void ReadTCode(TProgram Program)
        {
            if (!focas.IsConnected()) return;
            if (Program == null) return;



            int proc_count = Program.Processes.Count;
            for (int i = 0; i < proc_count; i++)
            {
                Program.Processes[i].OffsetX.Value = 0;
                Program.Processes[i].OffsetZ.Value = 0;
                Program.Processes[i].OffsetX.RedoValue = 0;
                Program.Processes[i].OffsetZ.RedoValue = 0;
            }

            string[] list = null;
            int ret = focas.ReadFile(FileType.Tool_Offset, out list);

            if (ret == Focas1.EW_OK)
            {
                //所有行數
                int count = list.Length;
                for (int i = 0; i < count; i++)
                {
                    double dx = 0;
                    double dz = 0;
                    //double dr = 0;

                    int ParamIndex;
                    String Data = list[i];
                    int pos;
                    //找到 T Code 數據
                    if ((pos = Data.IndexOf("P")) >= 0)
                    {
                        //P1~P64 為磨耗(Wear)
                        //可能是一位數P1
                        int.TryParse((Data.Substring(pos + 1, 1)), out int p1);
                        //可能是兩位數P10
                        int.TryParse((Data.Substring(pos + 1, 2)), out int p2);
                        //可能是三位數P100
                        int.TryParse((Data.Substring(pos + 1, 3)), out int p3);

                        //P10001~P10064 為行狀(Geometry)
                        //int p5 = (Data.Substring(pos + 1, 5)).ToInt();

                        //目前知道T Code 從1開始，因此判斷不為零的表示轉換成功，位數多的先判斷，T Code 陣列是從0開始放，因此轉Index要 - 1
                        //if (p5 != 0) ParamIndex = p5-1;
                        //else ...
                        //磨耗P1~P60 行狀P10001~P10060 --> index0~index59, index10000~index10059
                        if (p3 != 0) ParamIndex = p3 - 1;
                        else if (p2 != 0) ParamIndex = p2 - 1;
                        else if (p1 != 0) ParamIndex = p1 - 1;
                        else continue;

                        if ((pos = Data.IndexOf("X")) >= 0)
                        {
                            String StrVal = "";
                            int ValIndex = pos + 1;
                            while (true)
                            {
                                if ((Data[ValIndex] >= '0' && Data[ValIndex] <= '9') || (Data[ValIndex] == '.') || (Data[ValIndex] == '-'))
                                {
                                    StrVal += Data[ValIndex];
                                    ValIndex++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            double.TryParse(StrVal, out dx);
                        }
                        if ((pos = Data.IndexOf("Z")) >= 0)
                        {
                            String StrVal = "";
                            int ValIndex = pos + 1;
                            while (true)
                            {
                                if ((Data[ValIndex] >= '0' && Data[ValIndex] <= '9') || (Data[ValIndex] == '.') || (Data[ValIndex] == '-'))
                                {
                                    StrVal += Data[ValIndex];
                                    ValIndex++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            double.TryParse(StrVal, out dz);

                        }
                        //if ((pos = Data.IndexOf("R")) >= 0) dr = (Data.Substring(pos + 1, 6)).ToFloat();

                        //在M3 的案子中只能使用64個T Code, [加工補正]P1~P30, [重修補正]P31~P60
                        //磨耗，index0~index29為[加工補正]
                        if ((ParamIndex >= 0) && (ParamIndex < 30))
                        {
                            if (ParamIndex < proc_count)
                            {
                                Program.Processes[ParamIndex].OffsetX.Value = dx;
                                Program.Processes[ParamIndex].OffsetZ.Value = dz;
                            }
                        }
                        //磨耗，index30~index59為[重修補正] = [補正值] - [加工補正]
                        //if ((ParamIndex >= 30) && (ParamIndex < 60))
                        //{
                        //int index = ParamIndex-30;
                        //if ((index >= 0) && (index < proc_count))
                        //{
                        //Units.Fo_Main.CurrentProgram.Processes[index].OffsetX.RedoValue = dx - Units.Fo_Main.CurrentProgram.Processes[index].OffsetX.Value;
                        //Units.Fo_Main.CurrentProgram.Processes[index].OffsetZ.RedoValue = dz - Units.Fo_Main.CurrentProgram.Processes[index].OffsetZ.Value;
                        //}
                        //}
                        //形狀
                        //else if((ParamIndex >= 10000) && (ParamIndex<10030))
                        //{
                        //int index = ParamIndex - 10000;
                        //Units.OffsetX.Wear[index] = dx;
                        //Units.OffsetZ.Wear[index] = dz;
                        //Units.OffsetR.Wear[index] = dr;
                        //}
                    }
                }
            }
        }

        private void ReadAlarmHistory()
        {
            bool bIsAlarm = false;
            this.Invoke((Action)(() =>
            {
                //有異常
                if (pa_Alarm.Visible)
                {
                    bIsAlarm = true;
                }
            }));

            if (!bIsAlarm) return;

            int iAlarmTime = Environment.TickCount - iQueryAlarmTick;
            if (iAlarmTime > 5000)
            {
                iQueryAlarmTick = Environment.TickCount;

                int ret;
                string[] all_history = null;

                //時間,英文代碼+數字,軸號,系統號
                ret = focas.GetAlarmHistory(out all_history);

                if (all_history == null) return;

                //越新的資料在越後面，所以要先反轉一次再判斷
                all_history = all_history.Reverse().ToArray();
                this.Invoke(new Action(() =>
                {
                    //一筆一筆判斷資料是否已經存在資料庫，如果已經判斷到是舊的資料，後面就不再判斷，直接離開
                    foreach (string alarm in all_history)
                    {
                        string[] csv = alarm.Split(',');
                        if (csv.Length == 4)
                        {
                            string time = csv[0];
                            string code = csv[1];
                            string axis = csv[2];
                            string path = csv[3];

                            DataTable dt = database.ExecuteReader("SELECT * FROM AlarmHistory WHERE Time='" + time + "' AND Code='" + code + "' AND Axis=" + axis + " AND Path=" + path + " ;");
                            if (dt.Rows.Count > 0) continue;

                            database.ExecuteNonQuery("INSERT INTO AlarmHistory(Time, Code, Axis, Path) VALUES('" + time + "','" + code + "'," + axis + "," + path + ")");
                            string alarm_msg = Units.alarmfile.FindNameByCode(code);
                            if (alarm_msg == "" && PmcAlarmTable.FindCode(code) != null) alarm_msg = PmcAlarmTable.FindCode(code).Msg;//異常訊息修改 

                            int.TryParse(axis, out int axis_no);
                            if (axis_no != 0) alarm_msg = LanguageManager.LoadMessage(Units.langfile, "Message", 43, "第") + axis_no + LanguageManager.LoadMessage(Units.langfile, "Message", 44, "軸") + alarm_msg;


                            dgv_AlarmHistory.Rows.Add(csv[0], csv[1], alarm_msg);
                        }
                    }

                    dgv_AlarmHistory.Sort(Col_Alm_Time, ListSortDirection.Descending);
                }));




                //取得所有的Alarm
                ret = focas.GetCurrentAlarm(out String[] alarms);


                //有異常
                if (bIsAlarm)
                {
                    if (ret == SUCCESS)
                    {
                        AlarmFile tmpFile = new AlarmFile();
                        foreach (String s in alarms)
                        {
                            //分割資料(Code, Axis)
                            String[] csv = s.Split(',');
                            if (csv.Length >= 2)
                            {
                                tmpFile.Items.Add(new Alarm() { Code = csv[0], Msg = "", Axis = int.Parse(csv[1]) });
                            }
                        }

                        //判斷所有異常是否已經在清單中
                        foreach (Alarm a in tmpFile.Items)
                        {
                            bool bFind = false;
                            foreach (Alarm b in CurrentAlarm.Items)
                            {
                                if ((a.Code == b.Code) && (a.Axis == b.Axis))
                                {
                                    bFind = true; //已存在
                                    break;
                                }
                            }

                            //不存在於目前的清單中(新的Alarm) 或者 異常訊息有增減時, 更新目前異常清單
                            if (!bFind || (CurrentAlarm.Items.Count != tmpFile.Items.Count))
                            {
                                this.Invoke((Action)(() =>
                                {

                                    //清除所有
                                    LB_CurrentAlarm.Items.Clear();

                                    //這次撈到的清單(不含Troubleshooting)直接取代舊的
                                    CurrentAlarm = tmpFile;

                                    //這次全部的清單
                                    foreach (Alarm data in CurrentAlarm.Items)
                                    {
                                        //從檔案中去搜尋此異常代碼的資訊(Troubleshooting)
                                        Alarm a1 = TroubleShootingFile1.FindCode(data.Code);
                                        //找到資訊
                                        if (a1 != null)
                                        {
                                            //加入到ListBox 去顯示，將檔案搜尋到的資訊顯示出來 (不含異常排除)

                                            string axis_str = "";
                                            if (data.Axis != 0) axis_str = LanguageManager.LoadMessage(Units.langfile, "Message", 43, "第") + data.Axis + LanguageManager.LoadMessage(Units.langfile, "Message", 44, "軸");
                                            LB_CurrentAlarm.Items.Add(a1.Code + " " + axis_str + a1.Msg);

                                            //la_TroubleShooting.Text += a1.TroubleShooting.Replace("\\n", "\n") + "\n";

                                            data.TroubleShooting = a1.TroubleShooting;
                                        }
                                        //找不到資訊
                                        else
                                        {
                                            //加入到ListBox 去顯示，建立一個空的資訊，僅顯示Alarm Code

                                            if (PmcAlarmTable.FindCode(data.Code) != null)
                                            {
                                                LB_CurrentAlarm.Items.Add(data.Code + " " + PmcAlarmTable.FindCode(data.Code).Msg);
                                            }
                                            else LB_CurrentAlarm.Items.Add(data.Code);

                                        }//異常訊息修改

                                        //LB_CurrentAlarm.Items.Add(data.Code + " " + data.Msg);

                                    }
                                }));
                                break;
                            }
                        }

                    }
                    else //FANUC 通訊異常, 先清掉所有 Alarm
                    {
                        if (CurrentAlarm.Items.Count > 0)
                        {
                            this.Invoke(new Action(() =>
                            {
                                LB_CurrentAlarm.Items.Clear();
                                la_TroubleShooting.Text = "";
                                //LB_CurrentAlarm.Items.Add("沒有任何異常");
                                CurrentAlarm.Items.Clear();
                            }));
                        }
                    }


                }
            }
        }


        private void CheckInchMacro()
        {
            if (bInchTrans)//英制
            {
                foreach (XmlElement x in Units.MacroInfo.Items.Values)
                {
                    string Unit = x.GetAttribute("Unit");
                    if (Unit.Contains("mm"))
                    {
                        double.TryParse(x.GetAttribute("Max"), out double max);
                        double.TryParse(x.GetAttribute("Min"), out double min);
                        x.SetAttribute("Min", (min / 25).ToString(Units.DisplayFmt));
                        x.SetAttribute("Max", (max / 25).ToString(Units.DisplayFmt));
                        x.SetAttribute("Unit", Unit.Replace("mm", "inch"));
                    }
                }
            }
            else
            {
                foreach (XmlElement x in Units.MacroInfo.Items.Values)
                {
                    string Unit = x.GetAttribute("Unit");
                    if (Unit.Contains("inch"))
                    {
                        double.TryParse(x.GetAttribute("Max"), out double max);
                        double.TryParse(x.GetAttribute("Min"), out double min);
                        x.SetAttribute("Min", (min * 25).ToString(Units.DisplayFmt));
                        x.SetAttribute("Max", (max * 25).ToString(Units.DisplayFmt));
                        x.SetAttribute("Unit", Unit.Replace("inch", "mm"));
                    }
                }
            }
        }

        bool bFistOpen = true;

        private void CheckInchMode()
        {

            focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);
            if (F2.BIT_0() && !bInchTrans)//英制 且 未轉成英制
            {
                this.Invoke(new Action(() =>
                {
                    btn_Ofs_P01.DisplayText = "+0.001";
                    btn_Ofs_P001.DisplayText = "+0.0001";
                    btn_Ofs_P0001.DisplayText = "+0.00001";
                    btn_Ofs_N01.DisplayText = "-0.001";
                    btn_Ofs_N001.DisplayText = "-0.0001";
                    btn_Ofs_N0001.DisplayText = "-0.00001";

                    btn_Redo_P0001.DisplayText = "+0.00001";
                    btn_Redo_P001.DisplayText = "+0.0001";
                    btn_Redo_P01.DisplayText = "+0.001";
                    btn_Redo_N0001.DisplayText = "-0.00001";
                    btn_Redo_N001.DisplayText = "-0.0001";
                    btn_Redo_N01.DisplayText = "-0.001";

                    la_GWDiameterUnit.Text = "inch";
                    la_GWMinDiameterUnit.Text = "inch";
                    la_GWWidthUnit.Text = "inch";
                    la_GWMinWidthUnit.Text = "inch";
                    la_GWHLUnit.Text = "inch";
                    la_DiamAmountUnit.Text = "inch";
                    la_LeftAmountUnit.Text = "inch";
                    la_RightAmountUnit.Text = "inch";
                    la_DiamDressSpeedUnit.Text = "inch/min";
                    la_LeftDressSpeedUnit.Text = "inch/min";
                    la_RightDressSpeedUnit.Text = "inch/min";
                    la_DiamReservedAmountUnit.Text = "inch";
                    la_LeftReservedAmountUnit.Text = "inch";
                    la_RightReservedAmountUnit.Text = "inch";
                }));
                bInchTrans = true;
                Units.DisplayFmt = "0.00000";

                CheckInchMacro();

                //所有預設工序 單位包含 inch 就將預設 最小 最大 目前數值 運算為英制
                foreach (TProcess p in Units.ProcessList)
                {
                    foreach (TSubProgram sp in p.SubPrograms)
                    {
                        //所有引數(P CODE)
                        foreach (TArgument a in sp.Arguments)
                        {
                            if (a.Unit.Contains("mm"))
                            {
                                a.Max = Math.Round(a.Max / 25, 5);
                                a.Min = Math.Round(a.Min / 25, 5);
                                a.Default = Math.Round(a.Default / 25, 5);
                                a.Value = Math.Round(a.Value / 25, 5);
                                a.Unit = a.Unit.Replace("mm", "inch");
                            }
                        }
                    }
                }
                //foreach (TProcess p in Units.DefProcessDB.Processes)
                //{
                //    foreach (TSubProgram sp in p.SubPrograms)
                //    {
                //        //所有引數(P CODE)
                //        foreach (TArgument a in sp.Arguments)
                //        {
                //            if (a.Unit.Contains("mm"))
                //            {
                //                a.Max /= 25.4;
                //                a.Min /= 25.4;
                //                a.Default /= 25.4;
                //                a.Value /= 25.4;
                //                a.Unit = a.Unit.Replace("mm", "inch");
                //            }
                //        }
                //    }
                //}


                //所有使用者建立的程式
                foreach (TProgram pg in Units.ProgramDB.Programs)
                {
                    //所有工序
                    foreach (TProcess p in pg.Processes)
                    {
                        string Pcode = "//P" + p.ID.ToString();
                        //所有子程式
                        foreach (TSubProgram sp in p.SubPrograms)
                        {
                            //所有引數(P CODE)
                            foreach (TArgument a in sp.Arguments)
                            {
                                if (a.Unit.Contains("mm"))
                                {
                                    //a.Max = Math.Round(a.Max / 25.4, 5);
                                    //a.Min = Math.Round(a.Min / 25.4, 5);
                                    //a.Default = Math.Round(a.Default / 25.4, 5);
                                    // a.Value /= 25.4;
                                    a.Unit = a.Unit.Replace("mm", "inch");
                                }
                            }
                        }
                    }
                }


                //目前的程式
                if (CurrentProgram != null)
                {
                    //所有工序
                    foreach (TProcess p in CurrentProgram.Processes)
                    {
                        string Pcode = "//P" + p.ID.ToString();
                        //所有子程式
                        foreach (TSubProgram sp in p.SubPrograms)
                        {
                            //所有引數(P CODE)
                            foreach (TArgument a in sp.Arguments)
                            {
                                if (a.Unit.Contains("mm"))
                                {
                                    //a.Max = Math.Round(a.Max / 25.4, 5);
                                    //a.Min = Math.Round(a.Min / 25.4, 5);
                                    //a.Default = Math.Round(a.Default / 25.4, 5);
                                    // a.Value /= 25.4;
                                    a.Unit = a.Unit.Replace("mm", "inch");
                                }
                            }
                        }
                    }
                }
            }

            if (!F2.BIT_0() && bInchTrans) //公制 且 已轉英制 
            {
                this.Invoke(new Action(() =>
                {
                    btn_Ofs_P01.DisplayText = "+0.01";
                    btn_Ofs_P001.DisplayText = "+0.001";
                    btn_Ofs_P0001.DisplayText = "+0.0001";
                    btn_Ofs_N01.DisplayText = "-0.01";
                    btn_Ofs_N001.DisplayText = "-0.001";
                    btn_Ofs_N0001.DisplayText = "-0.0001";

                    btn_Redo_P0001.DisplayText = "+0.0001";
                    btn_Redo_P001.DisplayText = "+0.001";
                    btn_Redo_P01.DisplayText = "+0.01";
                    btn_Redo_N0001.DisplayText = "-0.0001";
                    btn_Redo_N001.DisplayText = "-0.001";
                    btn_Redo_N01.DisplayText = "-0.01";

                    la_GWDiameterUnit.Text = "mm";
                    la_GWMinDiameterUnit.Text = "mm";
                    la_GWWidthUnit.Text = "mm";
                    la_GWMinWidthUnit.Text = "mm";
                    la_GWHLUnit.Text = "mm";
                    la_DiamAmountUnit.Text = "mm";
                    la_LeftAmountUnit.Text = "mm";
                    la_RightAmountUnit.Text = "mm";
                    la_DiamDressSpeedUnit.Text = "mm/min";
                    la_LeftDressSpeedUnit.Text = "mm/min";
                    la_RightDressSpeedUnit.Text = "mm/min";
                    la_DiamReservedAmountUnit.Text = "mm";
                    la_LeftReservedAmountUnit.Text = "mm";
                    la_RightReservedAmountUnit.Text = "mm";
                }));

                bInchTrans = false;
                Units.DisplayFmt = "0.0000";

                CheckInchMacro();


                foreach (TProcess p in Units.ProcessList)
                {
                    foreach (TSubProgram sp in p.SubPrograms)
                    {
                        foreach (TArgument a in sp.Arguments)
                        {
                            if (a.Unit.Contains("inch"))
                            {
                                a.Max *= 25;
                                a.Min *= 25;
                                a.Default *= 25;
                                a.Value *= 25;
                                a.Unit = a.Unit.Replace("inch", "mm");
                            }

                        }
                    }
                }
                //foreach (TProcess p in Units.DefProcessDB.Processes)
                //{
                //    foreach (TSubProgram sp in p.SubPrograms)
                //    {
                //        foreach (TArgument a in sp.Arguments)
                //        {
                //            if (a.Unit.Contains("inch"))
                //            {
                //                a.Max *= 25.4;
                //                a.Min *= 25.4;
                //                a.Default *= 25.4;
                //                a.Value *= 25.4;
                //                a.Unit = a.Unit.Replace("inch", "mm");
                //            }
                //        }
                //    }
                //}

                //所有使用者建立的程式
                foreach (TProgram pg in Units.ProgramDB.Programs)
                {
                    foreach (TProcess p in pg.Processes)
                    {
                        foreach (TSubProgram sp in p.SubPrograms)
                        {
                            foreach (TArgument a in sp.Arguments)
                            {
                                if (a.Unit.Contains("inch"))
                                {
                                    //a.Max *= 25.4;
                                    //a.Min *= 25.4;
                                    //a.Default *= 25.4;
                                    //a.Value *= 25.4;
                                    a.Unit = a.Unit.Replace("inch", "mm");
                                }
                            }
                        }
                    }
                }

                //目前程式
                if (CurrentProgram != null)
                {
                    foreach (TProcess p in CurrentProgram.Processes)
                    {
                        foreach (TSubProgram sp in p.SubPrograms)
                        {
                            foreach (TArgument a in sp.Arguments)
                            {
                                if (a.Unit.Contains("inch"))
                                {
                                    //a.Max *= 25.4;
                                    //a.Min *= 25.4;
                                    //a.Default *= 25.4;
                                    //a.Value *= 25.4;
                                    a.Unit = a.Unit.Replace("inch", "mm");
                                }
                            }
                        }
                    }
                }
            }
        }


        private void OnConnected()
        {
            if (!bFistOpen) return;
            bFistOpen = false;

            //focas.ReadMacro(15050, out double gw_count);
            //GwCount = (int)Math.Round(gw_count);

            //focas.WriteMacro(977, 3);//OIG 300 R系列, 固定寫3

            //讀取回來目前的主軸(C軸)轉速設定值
            focas.PMC_ReadDbWord(PmcAddrType.D, 200, out uint D200);


            //C軸 倍率 E2522.0 ~ E2522.2 (0~7 = 120%~50%) 
            focas.PMC_ReadByte(PmcAddrType.E, 2522, out byte E2522);
            E2522 &= 0xF8; //先清除倍率的三個bit
            E2522 |= 2; //再固定給 100% (0~7 = 120%~50%) 設定已停用, 因此固定寫100%
            focas.PMC_WriteByte(PmcAddrType.E, 2522, E2522);

            //READY (如果剛開機, 蜂鳴器會開始叫)
            if (focas.PMC_ReadByte(PmcAddrType.E, 2100, out byte E2100) == SUCCESS)
            {
                focas.PMC_WriteByte(PmcAddrType.E, 2100, E2100.SetBit(0, true));
            }

            Dictionary<int, char> dicName = new Dictionary<int, char>();
            Dictionary<int, char> dicSubName = new Dictionary<int, char>();
            int Path1Count = 0; //計算座標系該讀哪個索引值
            focas.Param_ReadByte(987, 0, out byte AxisCount);//軸數
            for (int i = 0; i < AxisCount; i++)
            {
                focas.Param_ReadByte(1020, (short)(i + 1), out byte AxisName);//軸名稱
                focas.Param_ReadByte(1025, (short)(i + 1), out byte SubName);//軸子名稱
                char chrName = (char)AxisName;
                char chrSubName = (char)SubName;

                dicName.Add(i + 1, (char)chrName);
                dicSubName.Add(i + 1, (char)chrSubName);

                //記錄個軸的索引值 0~5
                if (chrName == 'X' && AxisNo["X"] == -1) AxisNo["X"] = Path1Count;
                else if (chrName == 'Z' && AxisNo["Z"] == -1) AxisNo["Z"] = Path1Count;
                else if (chrName == 'Y' && AxisNo["Y"] == -1) AxisNo["Y"] = Path1Count;
                else if (chrName == 'A' && AxisNo["A"] == -1) AxisNo["A"] = Path1Count;
                else if (chrName == 'B' && AxisNo["B"] == -1) AxisNo["B"] = Path1Count;
                else if (chrName == 'C' && AxisNo["C"] == -1) AxisNo["C"] = Path1Count;
                Path1Count++;
            }

            if(machineSeries == "M")
            {
                foreach(var axiszKey in AxisNo.Keys.ToList())
                {
                    
                    if (axiszKey == "X") AxisNo["X"] = 0;
                    else if (axiszKey == "Z") AxisNo["Z"] = 1;
                    else if (axiszKey == "B" && bYAEnable) AxisNo["B"] = 2;
                    else if (axiszKey == "Y" && !bYAEnable) AxisNo["Y"] = 2;
                    else if (axiszKey == "C") AxisNo["C"] = 3;
                    else AxisNo[axiszKey] = -1;
                }
            }
            Dictionary<string, int> tmpAxisNo = AxisNo.Where(k => k.Value != -1).ToDictionary(k => k.Key, k => k.Value);
            
            //監視 - 依照系統設定顯示有在使用的元件
            //focas.SystemInfoEx(out SysInfoEx info);
            string tmpAxisName = tmpAxisNo.Keys.ToList()[0];
            this.Invoke(new Action(() =>
            {
                btn_SpSpeed3.DisplayText = D200.ToString("0");

                //第一軸 顯示
                la_MonitorAbsAxis1.Visible = AxisCount > 0;
                la_MonitorAbsAxis1Value.Visible = AxisCount > 0;
                la_MonitorDistAxis1.Visible = AxisCount > 0;
                la_MonitorDistAxis1Value.Visible = AxisCount > 0;
                la_MonitorMachAxis1.Visible = AxisCount > 0;
                la_MonitorMachAxis1Value.Visible = AxisCount > 0;
                la_Monitor_Rel_X.Visible = AxisCount > 0;
                la_MonitorAbsAxis1s.Visible = AxisCount > 0 && machineSeries != "M";
                la_MonitorMachAxis1s.Visible = AxisCount > 0 && machineSeries != "M";
                la_MonitorDistAxis1s.Visible = AxisCount > 0 && machineSeries != "M";
                pic_Axis1_Origin.Visible = AxisCount > 0;//原點燈號
                if (AxisCount > 0 && tmpAxisNo.Count > 0) //第一軸 名稱
                {
                    //la_MonitorAbsAxis1.Text = dicName[1].ToString();
                    //la_MonitorDistAxis1.Text = dicName[1].ToString();
                    //la_MonitorMachAxis1.Text = dicName[1].ToString();
                    //la_Monitor_Rel_X.Text = dicName[1].ToString();

                    //la_MonitorAbsAxis1s.Text = dicSubName[1].ToString();
                    //la_MonitorMachAxis1s.Text = dicSubName[1].ToString();
                    //la_MonitorDistAxis1s.Text = dicSubName[1].ToString();
                    
                    la_MonitorAbsAxis1.Text = tmpAxisNo.Keys.ToList()[0];
                    la_MonitorDistAxis1.Text = tmpAxisNo.Keys.ToList()[0];
                    la_MonitorMachAxis1.Text = tmpAxisNo.Keys.ToList()[0];
                    la_Monitor_Rel_X.Text = tmpAxisNo.Keys.ToList()[0];
                }

                //第二軸 顯示
                la_MonitorAbsAxis2.Visible = AxisCount > 1;
                la_MonitorAbsAxis2Value.Visible = AxisCount > 1;
                la_MonitorDistAxis2.Visible = AxisCount > 1;
                la_MonitorDistAxis2Value.Visible = AxisCount > 1;
                la_MonitorMachAxis2.Visible = AxisCount > 1;
                la_MonitorMachAxis2Value.Visible = AxisCount > 1;
                la_Monitor_Rel_Z.Visible = AxisCount > 1;
                la_MonitorAbsAxis2s.Visible = AxisCount > 1 && machineSeries != "M";
                la_MonitorMachAxis2s.Visible = AxisCount > 1 && machineSeries != "M";
                la_MonitorDistAxis2s.Visible = AxisCount > 1 && machineSeries != "M";
                pic_Axis2_Origin.Visible = AxisCount > 1;//原點燈號
                if (AxisCount > 1 && tmpAxisNo.Count > 1) //第二軸 名稱
                {
                    //la_MonitorAbsAxis2.Text = dicName[2].ToString();
                    //la_MonitorDistAxis2.Text = dicName[2].ToString();
                    //la_MonitorMachAxis2.Text = dicName[2].ToString();
                    //la_Monitor_Rel_Z.Text = dicName[2].ToString();

                    //la_MonitorAbsAxis2s.Text = dicSubName[2].ToString();
                    //la_MonitorMachAxis2s.Text = dicSubName[2].ToString();
                    //la_MonitorDistAxis2s.Text = dicSubName[2].ToString();

                    la_MonitorAbsAxis2.Text = tmpAxisNo.Keys.ToList()[1];
                    la_MonitorDistAxis2.Text = tmpAxisNo.Keys.ToList()[1];
                    la_MonitorMachAxis2.Text = tmpAxisNo.Keys.ToList()[1];
                    la_Monitor_Rel_Z.Text = tmpAxisNo.Keys.ToList()[1];
                }

                //第三軸 顯示
                la_MonitorAbsAxis3.Visible = AxisCount > 2;
                la_MonitorAbsAxis3Value.Visible = AxisCount > 2;
                la_MonitorDistAxis3.Visible = AxisCount > 2;
                la_MonitorDistAxis3Value.Visible = AxisCount > 2;
                la_MonitorMachAxis3.Visible = AxisCount > 2;
                la_MonitorMachAxis3Value.Visible = AxisCount > 2;
                la_MonitorAbsAxis3s.Visible = AxisCount > 2 && machineSeries != "M";
                la_MonitorMachAxis3s.Visible = AxisCount > 2 && machineSeries != "M";
                la_MonitorDistAxis3s.Visible = AxisCount > 2 && machineSeries != "M";
                pic_Axis3_Origin.Visible = AxisCount > 2;//原點燈號
                
                if (AxisCount > 2 && tmpAxisNo.Count > 2) //第三軸 名稱
                {
                    //la_MonitorAbsAxis3.Text = dicName[3].ToString();
                    //la_MonitorDistAxis3.Text = dicName[3].ToString();
                    //la_MonitorMachAxis3.Text = dicName[3].ToString();


                    //la_MonitorAbsAxis3s.Text = dicSubName[3].ToString();
                    //la_MonitorMachAxis3s.Text = dicSubName[3].ToString();
                    //la_MonitorDistAxis3s.Text = dicSubName[3].ToString();

                    la_MonitorAbsAxis3.Text = tmpAxisNo.Keys.ToList()[2];
                    la_MonitorDistAxis3.Text = tmpAxisNo.Keys.ToList()[2];
                    la_MonitorMachAxis3.Text = tmpAxisNo.Keys.ToList()[2];
                }

                //第四軸 顯示
                la_MonitorAbsAxis4.Visible = AxisCount > 3;
                la_MonitorAbsAxis4Value.Visible = AxisCount > 3;
                la_MonitorDistAxis4.Visible = AxisCount > 3;
                la_MonitorDistAxis4Value.Visible = AxisCount > 3;
                la_MonitorMachAxis4.Visible = AxisCount > 3;
                la_MonitorMachAxis4Value.Visible = AxisCount > 3;
                la_MonitorAbsAxis4s.Visible = AxisCount > 3 && machineSeries != "M";
                la_MonitorMachAxis4s.Visible = AxisCount > 3 && machineSeries != "M";
                la_MonitorDistAxis4s.Visible = AxisCount > 3 && machineSeries != "M";
                pic_Axis4_Origin.Visible = AxisCount > 3;//原點燈號
                if (AxisCount > 3 && tmpAxisNo.Count > 3) //第四軸 名稱
                {
                    //la_MonitorAbsAxis4.Text = dicName[4].ToString();
                    //la_MonitorDistAxis4.Text = dicName[4].ToString();
                    //la_MonitorMachAxis4.Text = dicName[4].ToString();


                    //la_MonitorAbsAxis4s.Text = dicSubName[4].ToString();
                    //la_MonitorMachAxis4s.Text = dicSubName[4].ToString();
                    //la_MonitorDistAxis4s.Text = dicSubName[4].ToString();

                    la_MonitorAbsAxis4.Text = tmpAxisNo.Keys.ToList()[3];
                    la_MonitorDistAxis4.Text = tmpAxisNo.Keys.ToList()[3];
                    la_MonitorMachAxis4.Text = tmpAxisNo.Keys.ToList()[3];
                }

                //第五軸
                //la_MonitorAbsAxis5.Visible = AxisCount > 4;
                //la_MonitorAbsAxis5Value.Visible = AxisCount > 4;
                //la_MonitorDistAxis5.Visible = AxisCount > 4;
                //la_MonitorDistAxis5Value.Visible = AxisCount > 4;
                //la_MonitorMachAxis5.Visible = AxisCount > 4;
                //la_MonitorMachAxis5Value.Visible = AxisCount > 4;
                //la_MonitorAbsAxis5s.Visible = AxisCount > 4;
                //la_MonitorMachAxis5s.Visible = AxisCount > 4;
                //la_MonitorDistAxis5s.Visible = AxisCount > 4;
                //pic_Axis5_Origin.Visible = AxisCount > 4;//原點燈號
                //if (AxisCount > 4) //第五軸 名稱
                //{
                //    la_MonitorAbsAxis5.Text = dicName[5].ToString();
                //    la_MonitorDistAxis5.Text = dicName[5].ToString();
                //    la_MonitorMachAxis5.Text = dicName[5].ToString();

                //    la_MonitorAbsAxis5s.Text = dicSubName[5].ToString();
                //    la_MonitorMachAxis5s.Text = dicSubName[5].ToString();
                //    la_MonitorDistAxis5s.Text = dicSubName[5].ToString();
                //}

                ////第六軸 顯示
                //la_MonitorAbsAxis6.Visible = AxisCount > 5;
                //la_MonitorAbsAxis6Value.Visible = AxisCount > 5;
                //la_MonitorDistAxis6.Visible = AxisCount > 5;
                //la_MonitorDistAxis6Value.Visible = AxisCount > 5;
                //la_MonitorMachAxis6.Visible = AxisCount > 5;
                //la_MonitorMachAxis6Value.Visible = AxisCount > 5;
                //la_MonitorAbsAxis6s.Visible = AxisCount > 5;
                //la_MonitorMachAxis6s.Visible = AxisCount > 5;
                //la_MonitorDistAxis6s.Visible = AxisCount > 5;
                //pic_Axis6_Origin.Visible = AxisCount > 5;//原點燈號
                //if (AxisCount > 5) //第六軸 名稱
                //{
                //    la_MonitorAbsAxis6.Text = dicName[6].ToString();
                //    la_MonitorDistAxis6.Text = dicName[6].ToString();
                //    la_MonitorMachAxis6.Text = dicName[6].ToString();

                //    la_MonitorAbsAxis6s.Text = dicSubName[6].ToString();
                //    la_MonitorMachAxis6s.Text = dicSubName[6].ToString();
                //    la_MonitorDistAxis6s.Text = dicSubName[6].ToString();
                //}
            }));


            //砂輪恆速功能
            //LoadGW1SpeedLayout();


            //讀取T Code
            ReadTCode(CurrentProgram);//連上線時讀取一次

            //再寫一次，避免重修精磨不是空的
            WriteTCode(CurrentProgram);//連上線時讀取一次

            //初始化重修精磨開關            
            int len = 30;
            double[] AllZero = new double[30];
            focas.WriteMacro(730, ref len, AllZero);//重修預設全關

            //清除重修精磨的狀態
            focas.WriteMacro(966, 0);
            //TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //bool IsInch = ini.ReadBool("System", "InchMode", false);

            //CheckInchMode();

        }

        bool bCNCConnect = false;//正緣觸發連線圖示, 負緣觸發離線圖示
        bool bCmdPowerOff = false;
        public bool bShowLogoForm = false;
        public bool bShowLogo = false;
        int iRetryConnect = 0;
        private void Execute()
        {

            int ret;
            Thread.Sleep(1000);
            int iLogoStart = Environment.TickCount;//Logo 已顯示的時間
            int iConnectStart = Environment.TickCount;//連線失敗已經過的時間
            this.Invoke((Action)(() =>
            {
                //顯畫面(會先黑畫面, 沒有Logo, 沒有無訊號)
                bShowLogoForm = true;
                Logo.Show();

                //預設在監視畫面                
                TC_Main.SelectedTab = tab_Monitor;

                //讀取畫面配置
                SetPanelLayout();

                this.Opacity = 1;//顯示                
            }));



            focas.ConnectByEthernet(IPAddress, Port);//試連線(不佔主畫面執行序, 但此執行序會先卡在這)
            iConnectStart = Environment.TickCount;//連線失敗已經過的時間

            if (focas.IsConnected())//試連成功
            {
                this.Invoke((Action)(() =>
                {
                    if (Logo != null)
                    {
                        Logo.pic_Logo.Visible = true; //讓軟體一開啟就顯示LOGO
                        bShowLogo = true;
                        iLogoStart = Environment.TickCount;//開始計時顯示的時間
                    }

                    //中斷試連
                    //focas.Disconnect();
                    //正式連線
                    //focas.ConnectByEthernet(IPAddress, Port);
                    iRetryConnect = 0;

                    bCNCConnect = true; //這裡只執行一次, 不用判斷正緣觸發
                    pic_ConnectStatus.Image = Properties.Resources.connects;
                }));

                //每次連線成功檢查
                OnConnected();
            }
            else //軟體一開啟就試連失敗 (也可能FANUC開機比較晚)
            {
                bCNCConnect = false; //這裡只執行一次, 不用判斷負緣觸發
                this.Invoke((Action)(() =>
                {
                    //隱藏圖片變黑畫面
                    Logo.pic_Logo.Visible = false;
                    bShowLogo = false;
                    //顯示 FANUC NO SIGNAL
                    Logo.pa_NoSignal.Visible = true;

                    pic_ConnectStatus.Image = Properties.Resources.disconnects;
                }));

            }

            Request_CNC_Time_Start = Environment.TickCount;
            PmcRefleshStart = Environment.TickCount;
            CheckSerialHeart = Environment.TickCount;

            masterSerialBus1.Start();
            masterSerialBus2.Start();

            int iQueryMonitorOneSec = Environment.TickCount;
            int iQuerySoftPanelStart = Environment.TickCount;

            //循環開始
            while (true)
            {
                //程式關閉, 離開循環
                if (bClose) break;

                if (bRefleshSoftPanell || bRefleshMonitor)
                {
                    bRefleshMonitor = false;
                    bRefleshSoftPanell = false;
                    Thread.Sleep(500);
                    continue;
                }



                //降CPU用
                Thread.Sleep(50);

                //if (Logo.Visible) Cursor.Hide();
                //else Cursor.Show();

                //每次循環都檢查連線狀態(僅處理狀態顯示)
                if (focas.IsConnected())//仍然連著 
                {
                    if (!bCNCConnect)//正緣觸發 連線
                    {
                        bCNCConnect = true;
                        this.Invoke((Action)(() =>
                        {
                            //顯示連線狀態
                            pic_ConnectStatus.Image = Properties.Resources.connects;
                        }));
                    }
                }
                else //已斷線
                {
                    if (bCNCConnect)//負緣觸發 斷線
                    {
                        bCNCConnect = false;
                        this.Invoke((Action)(() =>
                        {
                            //顯示斷線狀態
                            pic_ConnectStatus.Image = Properties.Resources.disconnects;
                        }));
                    }
                }

                //斷線 / 未連線
                if (!focas.IsConnected())
                {
                    //距離上次試連經過多少時間
                    int iConnectTime = Environment.TickCount - iConnectStart;
                    if (iConnectTime > 5000)
                    {
                        iRetryConnect++;
                        if (iRetryConnect > 3)
                        {
                            this.Invoke((Action)(() =>
                            {
                                if (Logo != null && !bShowLogoForm)
                                {
                                    Logo.pic_Logo.Visible = false;
                                    Logo.pa_NoSignal.Visible = true;
                                    Logo.Show();
                                    bShowLogoForm = true;
                                }
                            }));
                        }

                        //自動連線
                        focas.ConnectByEthernet(IPAddress, Port);//先試連
                        iConnectStart = Environment.TickCount;//連線失敗已經過的時間


                        this.Invoke((Action)(() =>
                        {
                            if (Logo != null && bShowLogoForm)
                            {
                                //隱藏圖片變黑畫面
                                Logo.pic_Logo.Visible = false;
                                bShowLogo = false;

                                //顯示 FANUC NO SIGNAL
                                Logo.pa_NoSignal.Visible = true;
                            }
                        }));

                    }

                    //試連成功
                    if (focas.IsConnected())
                    {
                        iRetryConnect = 0;
                        this.Invoke((Action)(() =>
                        {
                            //focas.Disconnect();//斷開試連的
                            //focas.ConnectByEthernet(IPAddress, Port);//正式連

                            if (Logo != null)
                            {
                                Logo.pa_NoSignal.Visible = false; //隱藏無訊號
                                Logo.pic_Logo.Visible = true;//顯示LOGO
                                bShowLogo = true;
                                iLogoStart = Environment.TickCount;//開始計時顯示的時間
                            }
                        }));
                        //每次連線成功檢查
                        OnConnected();
                    }
                    continue;
                }
                else //已連線
                {
                    if (bShowLogo)//Logo目前顯示中
                    {
                        int iLogoTime = Environment.TickCount - iLogoStart;
                        if (iLogoTime > 5000) //已顯示3秒
                        {
                            if (Logo != null)
                            {
                                this.Invoke((Action)(() =>
                                {
                                    Logo.Hide();// 隱藏LOGO視窗
                                    bShowLogoForm = false;
                                    Logo.pic_Logo.Visible = false;
                                    bShowLogo = false;
                                }));
                            }
                        }
                    }
                }

                bool bLayout = false;
                bool bImport = false;
                bool bWarmup = false;
                bool bGWRPS2 = false;
                this.Invoke(new Action(() =>
                {
                    bLayout = fo_layout != null;
                    bImport = fo_ImportProg != null;
                    bWarmup = fo_Warmup != null;
                    bGWRPS2 = TC_Main.SelectedTab == tab_GWRPS2;
                }));

                ret = focas.ReadAllAxisPos(out Pos);
                if (Pos.Machine.Length > 3)
                {
                    if (Pos.Machine[3] == 0) //當轉到0時，紀錄最後的座標，避免下降後位置跑掉，顯示非零的數值
                    {
                        if (Pos.Machine.Length > 2) dLast_Y_MPos = Math.Round(Pos.Machine[2]);
                    }
                }
                if (Pos.Absolute.Length > 3) //當轉到0時，紀錄最後的座標，避免下降後位置跑掉，顯示非零的數值
                {
                    if (Pos.Absolute[3] == 0)
                    {
                        if (Pos.Absolute.Length > 2) dLast_Y_APos = Math.Round(Pos.Absolute[2]);
                    }
                }
                this.Invoke(new Action(() =>
                {
                    if (TC_Main.SelectedTab == tab_Monitor)
                    {

                        if (ret != SUCCESS)
                        {
                            pic_ConnectStatus.Image = Properties.Resources.disconnects;
                            bCNCConnect = false;
                            return;
                        }

                        pic_ConnectStatus.Image = Properties.Resources.connects;
                        bCNCConnect = true;
                        string Axis3Fmt;
                        string Axis4Fmt;
                        if (Axis3Type == 0) Axis3Fmt = Units.DisplayFmt;//直線軸
                        else Axis3Fmt = "0.000";//旋轉軸
                        if (Axis4Type == 0) Axis4Fmt = Units.DisplayFmt;//直線軸
                        else Axis4Fmt = "0.000";//旋轉軸



                        if (Pos.Machine.Length > 0) la_Monitor_Rel_X.Text = (Pos.Machine[0] - dManualZeroPoint).ToString(Units.DisplayFmt);//相對位置
                        if (Pos.Machine.Length > 0) la_MonitorMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);//X                        
                        if (Pos.Machine.Length > 1) la_Monitor_Rel_Z.Text = (Pos.Machine[1] - dManualZeroPointZ).ToString(Units.DisplayFmt);//相對位置
                        if (Pos.Machine.Length > 1) la_MonitorMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);//Z
                        if (Pos.Machine.Length > 2)
                        {
                           
                            if (la_MonitorMachAxis3.Text == "Y" && !bYAEnable)
                            {
                                // la_MonitorMachAxis3Value.Text = Pos.Machine[2].ToString(Axis3Fmt);//Y}
                                int deg = (int)Math.Round(Pos.Machine[2]) % 360;
                                la_MonitorMachAxis3Value.Text = deg.ToString(Axis3Fmt);//Y
                            }
                            if (la_MonitorMachAxis3.Text == "B" && bYAEnable)
                            {
                                //la_MonitorMachAxis3Value.Text = Pos.Machine[2].ToString(Axis3Fmt);//Y}
                                if (Pos.Machine[3] == 0)
                                {
                                    double pos = Math.Round(Pos.Machine[2]) + (Pos.Machine[3] / 100.0);
                                    if (pos >= 360) pos -= 360;
                                    la_MonitorMachAxis3Value.Text = pos.ToString("0.000");//Y → 顯示為B
                                }
                                else
                                {
                                    double pos = dLast_Y_MPos + (Pos.Machine[3] / 100.0);
                                    if (pos >= 360) pos -= 360;
                                    la_MonitorMachAxis3Value.Text = pos.ToString("0.000");//Y → 顯示為B
                                }
                            }
                        }
                        if (Pos.Machine.Length > 3) la_MonitorMachAxis4Value.Text = Pos.Machine[3].ToString(Axis4Fmt);//C

                        //絕對座標
                        if (Pos.Absolute.Length > 0) la_MonitorAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Absolute.Length > 1) la_MonitorAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);//Z
                        if (Pos.Absolute.Length > 2)
                        {
                            if (la_MonitorAbsAxis3.Text == "Y" && !bYAEnable)
                            {
                                //la_MonitorAbsAxis3Value.Text = Pos.Absolute[2].ToString(Axis3Fmt);//Y
                                int deg = (int)Math.Round(Pos.Absolute[2]) % 360;
                                la_MonitorAbsAxis3Value.Text = deg.ToString("0.000");//Y → 顯示為B
                            }
                            if (la_MonitorAbsAxis3.Text == "B" && bYAEnable)
                            {
                                if (Pos.Absolute[3] == 0)
                                {
                                    double pos = Math.Round(Pos.Absolute[2]) + (Pos.Absolute[3] / 100.0);
                                    if (pos >= 360) pos -= 360;
                                    la_MonitorAbsAxis3Value.Text = pos.ToString("0.000");//Y → 顯示為B
                                }
                                else
                                {
                                    double pos = dLast_Y_APos + (Pos.Absolute[3] / 100.0);
                                    if (pos >= 360) pos -= 360;
                                    la_MonitorAbsAxis3Value.Text = pos.ToString("0.000");//Y → 顯示為B
                                }
                            }
                        }
                        if (Pos.Absolute.Length > 3) la_MonitorAbsAxis4Value.Text = Pos.Absolute[3].ToString(Axis4Fmt);//C

                        //殘移動量
                        if (Pos.Distance.Length > 0) la_MonitorDistAxis1Value.Text = Pos.Distance[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Distance.Length > 1) la_MonitorDistAxis2Value.Text = Pos.Distance[1].ToString(Units.DisplayFmt);//Z
                        if (Pos.Distance.Length > 2) la_MonitorDistAxis3Value.Text = Pos.Distance[2].ToString(Axis3Fmt);//Y
                        if (Pos.Distance.Length > 3) la_MonitorDistAxis4Value.Text = Pos.Distance[3].ToString(Axis4Fmt);//C
                    }
                }));

                if (bGWRPS2)// GWRPS  砂輪基準點設定座標
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    
                    this.Invoke(new Action(() =>
                    {
                        if (ret != SUCCESS) return;
                        if (Pos.Machine.Length > 0) la_GWRPSMCx.Text = Pos.Machine[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Machine.Length > 1) la_GWRPSMCz.Text = Pos.Machine[1].ToString(Units.DisplayFmt);//Z
                        if (AxisNo["B"] >=0 && Pos.Machine.Length > AxisNo["B"]) la_GWRPSMCb.Text = Pos.Machine[AxisNo["B"]].ToString(Units.DisplayFmt);//B
                        if (Pos.Distance.Length > 0) la_GWRPSRMx.Text = Pos.Distance[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Distance.Length > 1) la_GWRPSRMz.Text = Pos.Distance[1].ToString(Units.DisplayFmt);//Z
                        if (AxisNo["B"] >= 0 && Pos.Distance.Length > AxisNo["B"]) la_GWRPSRMb.Text = Pos.Distance[AxisNo["B"]].ToString(Units.DisplayFmt);//B
                    }));
                }
                //間隔時間刷新狀態
                int iTime = Environment.TickCount - PmcRefleshStart;
                if ((iTime > 200) && !bLayout && !bImport)
                {
                    ret = focas.PMC_ReadByte(PmcAddrType.F, 0, out byte F0);
                    if (ret != SUCCESS)
                    {
                        focas.Disconnect();
                        continue;
                    }

                    if (F0.BIT_7())//程式中
                    {
                        if (!bCycleStart) //F0.7 OFF -> ON
                        {
                            //F0.7 上緣觸發
                            bCycleStart = true;

                            if (SP_Comm_Enabled)
                            {
                                if (SpindleDev == 0)
                                {
                                    //傳送指令到三菱驅動器
                                    string data = ((int)Math.Round(ProgSCode * Spindle.Rate * Spindle.Unit)).ToString("X8");
                                    masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "102106000204" + data.Substring(4, 4) + data.Substring(0, 4));
                                }
                                else if (SpindleDev == 1)
                                {
                                    //傳送指令到安川驅動器
                                    string data = ((int)Math.Round(ProgSCode * Spindle.Rate * Spindle.Unit)).ToString("X4");
                                    if (SpindleChIndex == 0)//RS485
                                        masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                                    if (SpindleChIndex == 1)//RS422
                                        masterSerialBus2.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                                }
                                else
                                {
                                    //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                                    string data = ((int)Math.Round(ProgSCode / this.Spindle.Rate / Spindle.Unit)).ToString("X4");
                                    //傳送指令到士林變頻器
                                    masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "061009" + data);
                                }
                                //紀錄目前送給驅動器中的速度
                                Spindle.CmdSpeed = ProgSCode;
                            }
                        }
                        else //只要是Cycle Start，程式中速度改變
                        {
                            //程式中速度改變了
                            if (Spindle.CmdSpeed != ProgSCode)
                            {

                                if (SpindleDev == 0)
                                {
                                    //傳送指令到三菱驅動器(FANUC F22的值 = S Code)
                                    string data = ((int)Math.Round(ProgSCode * Spindle.Rate * Spindle.Unit)).ToString("X8");
                                    if (SP_Comm_Enabled) masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "102106000204" + data.Substring(4, 4) + data.Substring(0, 4));
                                }
                                else if (SpindleDev == 1)
                                {
                                    //傳送指令到安川驅動器(FANUC F22的值 = S Code)
                                    string data = ((int)Math.Round(ProgSCode * Spindle.Rate * Spindle.Unit)).ToString("X4");
                                    if (SP_Comm_Enabled)
                                    {
                                        if (SpindleChIndex == 0)//RS485
                                            masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                                        if (SpindleChIndex == 1)//RS422
                                            masterSerialBus2.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                                    }
                                }
                                else
                                {
                                    //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                                    string data = ((int)Math.Round(ProgSCode / this.Spindle.Rate / Spindle.Unit)).ToString("X4");
                                    //傳送指令到士林變頻器
                                    if (SP_Comm_Enabled) masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "061009" + data);
                                }
                                Spindle.CmdSpeed = ProgSCode;
                            }
                        }
                    }
                    else //程式終了
                    {
                        if (bCycleStart) //F0.7 ON -> OFF
                        {
                            //下緣觸發
                            bCycleStart = false;

                            if (SpindleDev == 0)
                            {
                                //傳送指令到三菱驅動器
                                string data = ((int)Math.Round(UserSCode * Spindle.Rate * Spindle.Unit)).ToString("X8");
                                if (SP_Comm_Enabled) masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "102106000204" + data.Substring(4, 4) + data.Substring(0, 4));
                            }
                            else if (SpindleDev == 1)
                            {
                                //傳送指令到安川驅動器
                                string data = ((int)Math.Round(UserSCode * Spindle.Rate * Spindle.Unit)).ToString("X4");
                                if (SP_Comm_Enabled)
                                {
                                    if (SpindleChIndex == 0)//RS485
                                        masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                                    if (SpindleChIndex == 1)//RS422
                                        masterSerialBus2.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                                }
                            }
                            else
                            {
                                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                                string data = ((int)Math.Round(UserSCode / this.Spindle.Rate / Spindle.Unit)).ToString("X4");
                                //傳送指令到士林變頻器
                                if (SP_Comm_Enabled) masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "061009" + data);
                            }
                            //紀錄目前送給驅動器的速度
                            Spindle.CmdSpeed = UserSCode;
                        }
                    }

                    bRun = F0.BIT_7();
                    if (bRun)
                    {
                        bOneKeyCall = false;
                    }
                    else
                    {
                        if (bGWRPS2)
                        {
                            int gwStepLen = 12;
                            focas.ReadMacro(12091, ref gwStepLen, out double[] stepValue);
                            bool bAllFinish = true;
                            string[] filenames = new string[gwStepLen];
                            for (int i = 0; i < 11; i++)
                            {
                                filenames[i] = "";
                                int status = (int)Math.Round(stepValue[i]);
                                if (status != 1) bAllFinish = false;
                                if ((i + 1) < stepValue.Length)
                                {
                                    string pic_filename = Application.StartupPath + "\\image\\GwRefPosSet\\" + (i + 1) + (stepValue[i] == 1 ? "s" : "") + ".png";
                                    filenames[i] = pic_filename;
                                }
                            }
                            this.Invoke(new Action(() =>
                            {
                                int finish_status = (int)Math.Round(stepValue[11]);
                                btn_GWRPS_save.Visible = bAllFinish && finish_status == 0; //全部步驟都打勾 且 還沒存檔 才顯示 完成Btn
                                for (int i = 0; i < 11; i++)
                                {
                                    PictureBox pic = GwRefPosSetPics[i];
                                    if (pic == null) continue;

                                    //第一次執行直接顯示圖片
                                    if (!GwRefPosSetStatus.ContainsKey(i))
                                    {
                                        GwRefPosSetStatus.Add(i, (int)Math.Round(stepValue[i]));
                                        if (File.Exists(filenames[i])) pic.Image = (Bitmap)Bitmap.FromFile(filenames[i]);
                                    }
                                    else //之後狀態有改變才重新讀取圖片
                                    {
                                        int status = (int)Math.Round(stepValue[i]);
                                        if (GwRefPosSetStatus[i] != status)
                                        {
                                            GwRefPosSetStatus[i] = status;
                                            if (File.Exists(filenames[i])) pic.Image = (Bitmap)Bitmap.FromFile(filenames[i]);
                                        }
                                    }
                                }
                            }));
                        }
                    }

                    CheckInchMode();

                    //int mac_len = 100;
                    //focas.ReadMacro(600, ref mac_len, out double[] tmp600_699);
                    //Dictionary<int, double> macro600_699 = new Dictionary<int, double>();
                    //for (int i = 0; i < mac_len; i++)
                    //{
                    //    macro600_699[600 + i] = tmp600_699[i];
                    //}

                    //focas.ReadMacro(626, out double GW2_GRIND_AT_Enabled);//功能開關
                    //focas.ReadMacro(627, out double GW2_DRESS_AT_Enabled);//功能開關
                    //focas.ReadMacro(636, out double GW1_GRIND_AT_Enabled);//功能開關
                    //focas.ReadMacro(637, out double GW1_DRESS_AT_Enabled);//功能開關
                    //double GW2_GRIND_AT_Enabled = macro600_699[626];//研磨中恆速功能開關
                    //double GW2_DRESS_AT_Enabled = macro600_699[627];//修整中恆速功能開關
                    //double Macro628 = macro600_699[628];//研磨中 恆速速度
                    //double Macro629 = macro600_699[629];//修整中 恆速速度
                    //double GW1_GRIND_AT_Enabled = macro600_699[636];//研磨中恆功速能開關
                    //double GW1_DRESS_AT_Enabled = macro600_699[637];//修整中恆速功能開關
                    //double Macro638 = macro600_699[638];//研磨中 恆速速度
                    //double Macro639 = macro600_699[639];//修整中 恆速速度

                    //關機交握
                    focas.PMC_ReadByte(PmcAddrType.E, 2110, out byte E2110);
                    if (E2110.BIT_4())
                    {
                        focas.PMC_WriteByte(PmcAddrType.E, 2100, 17);
                        bCmdPowerOff = true;
                    }
                    else
                    {
                        focas.PMC_WriteByte(PmcAddrType.E, 2100, 1);
                        if (bCmdPowerOff)
                        {
                            // 註意：/s表示關機，/t 0表示立即關機，/f強制(不會因為其他軟體未關閉而卡住)
                            Process.Start("shutdown", "/s /f /t 0");
                        }
                    }

                    focas.PMC_ReadByte(PmcAddrType.D, 908, out byte D908);
                    focas.PMC_ReadByte(PmcAddrType.F, 3, out byte F3);
                    focas.PMC_ReadDbWord(PmcAddrType.F, 22, out uint F22);
                    focas.PMC_ReadByte(PmcAddrType.Y, 2, out byte Y2);
                    focas.PMC_ReadByte(PmcAddrType.Y, 3, out byte Y3);
                    focas.PMC_ReadByte(PmcAddrType.R, 502, out byte R502);
                    focas.PMC_ReadByte(PmcAddrType.E, 2822, out byte E2822);
                    ProgSCode = (int)F22;

                    //if (focas.PMC_ReadByte(PmcAddrType.R, 530, out byte R530) == SUCCESS)
                    //{
                    //    this.Invoke(new Action(() =>
                    //    {
                    //        if (R530.BIT_0() != bGW1GRIND_AT) //GW1 研磨中 電流防撞
                    //        {
                    //            pic_Gw1GrindCrash.Image = R530.BIT_0() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                    //        }
                    //        if (R530.BIT_1() != bGW1DRESS_AT) //GW1 修整中 電流防撞
                    //        {
                    //            pic_Gw1DressCrash.Image = R530.BIT_1() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                    //        }

                    //        if (R530.BIT_2() != bGW2GRIND_AT) //GW2 研磨中 電流防撞
                    //        {
                    //            pic_Gw2GrindCrash.Image = R530.BIT_2() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                    //        }

                    //        if (R530.BIT_3() != bGW2DRESS_AT) //GW1 修整中 電流防撞
                    //        {
                    //            pic_Gw2DressCrash.Image = R530.BIT_3() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                    //        }
                    //        bGW1GRIND_AT = R530.BIT_0();//砂輪1電流防撞功能砂輪1研磨
                    //        bGW1DRESS_AT = R530.BIT_1();//砂輪1電流防撞功能砂輪1修整
                    //        bGW2GRIND_AT = R530.BIT_2();//砂輪1電流防撞功能砂輪2研磨
                    //        bGW2DRESS_AT = R530.BIT_3();//砂輪1電流防撞功能砂輪2修整


                    //        if (D908.BIT_6() != bGW1_GAP_CRASH_Enabled)
                    //        {
                    //            pic_Gw1CrashEnabled.Image = D908.BIT_6() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                    //        }

                    //        if (D908.BIT_4() != bGW2_GAP_CRASH_Enabled)
                    //        {
                    //            pic_Gw2CrashEnabled.Image = D908.BIT_4() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                    //        }
                    //        bGW1_GAP_CRASH_Enabled = D908.BIT_6();//砂輪1電流防撞功能開啟
                    //        bGW2_GAP_CRASH_Enabled = D908.BIT_4();//砂輪2電流防撞功能開啟

                    //        if (focas.PMC_ReadByte(PmcAddrType.R, 513, out byte R513) == SUCCESS)
                    //        {
                    //            if (!R513.BIT_1())//清除CRASH狀態
                    //            {
                    //                if (bGW1_CRASH || bGW2_CRASH)
                    //                {
                    //                    focas.PMC_ReadByte(PmcAddrType.E, 6, out byte E6);
                    //                    E6 = E6.SetBit(1, false);//GW1 CRASH E6.1 OFF
                    //                    E6 = E6.SetBit(2, false);//GW2 CRASH E6.2 OFF
                    //                    focas.PMC_WriteByte(PmcAddrType.E, 6, E6);
                    //                    pic_Gw1Crash.Image = Properties.Resources.Lamp_E_Off;
                    //                    pic_Gw2Crash.Image = Properties.Resources.Lamp_E_Off;
                    //                    bGW1_CRASH = false;
                    //                    bGW2_CRASH = false;
                    //                }
                    //            }
                    //        }
                    //    }));
                    //}

                    //if (Math.Round(GW2_GRIND_AT_Enabled) == 1)//GW2研磨中恆速功能啟用
                    //{
                    //    if (R530.BIT_2())//研磨中
                    //    {
                    //        //focas.ReadMacro(628, out double Macro628);
                    //        if (!bGW2_GRIND_AT)//上緣觸發旗標
                    //        {
                    //            bGW2_GRIND_AT = true;//已觸發
                    //            GW2_GRIND_AT_628 = Macro628;
                    //            SetGw2Speed(Macro628);
                    //        }
                    //        else
                    //        {
                    //            if (GW2_GRIND_AT_628 != Macro628)
                    //            {
                    //                GW2_GRIND_AT_628 = Macro628;
                    //                SetGw2Speed(Macro628);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        bGW2_GRIND_AT = false;//清除為未觸發
                    //    }
                    //}
                    //else//恆速功能關閉
                    //{
                    //    GW2_GRIND_AT_628 = 0;
                    //    bGW2_GRIND_AT = false;//清除為未觸發
                    //}

                    //if (Math.Round(GW2_DRESS_AT_Enabled) == 1)//GW2修整中恆速功能啟用
                    //{
                    //    if (R530.BIT_3())//修整中
                    //    {
                    //        //focas.ReadMacro(629, out double Macro629);
                    //        if (!bGW2_DRESS_AT)//上緣觸發旗標
                    //        {
                    //            bGW2_DRESS_AT = true;
                    //            GW2_DRESS_AT_629 = Macro629;
                    //            SetGw2Speed(Macro629);
                    //        }
                    //        else
                    //        {
                    //            if (GW2_DRESS_AT_629 != Macro629)
                    //            {
                    //                GW2_DRESS_AT_629 = Macro629;
                    //                SetGw2Speed(Macro629);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        bGW2_DRESS_AT = false;//清除為未觸發
                    //    }
                    //}
                    //else//恆速功能關閉
                    //{
                    //    GW2_DRESS_AT_629 = 0;
                    //    bGW2_DRESS_AT = false;//清除為未觸發
                    //}

                    #region GW1 恆速
                    //if (Math.Round(GW1_GRIND_AT_Enabled) == 1)//GW1研磨中恆速功能啟用
                    //{
                    //    if (R530.BIT_0())//研磨中
                    //    {
                    //        //focas.ReadMacro(638, out double Macro638);
                    //        if (!bGW1_GRIND_AT)//上緣觸發旗標
                    //        {
                    //            bGW1_GRIND_AT = true;//已觸發
                    //            GW1_GRIND_AT_638 = Macro638;
                    //            SetGw1Speed(Macro638);
                    //        }
                    //        else
                    //        {
                    //            if (GW1_GRIND_AT_638 != Macro638)
                    //            {
                    //                GW1_GRIND_AT_638 = Macro638;
                    //                SetGw1Speed(Macro638);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        bGW1_GRIND_AT = false;//清除為未觸發
                    //    }
                    //}
                    //else//恆速功能關閉
                    //{
                    //    GW1_GRIND_AT_638 = 0;
                    //    bGW1_GRIND_AT = false;//清除為未觸發
                    //}

                    //if (Math.Round(GW1_DRESS_AT_Enabled) == 1)//GW1修整中恆速功能啟用
                    //{
                    //    if (R530.BIT_1())//修整中
                    //    {
                    //        //focas.ReadMacro(639, out double Macro639);
                    //        if (!bGW1_DRESS_AT)//上緣觸發旗標
                    //        {
                    //            bGW1_DRESS_AT = true;
                    //            GW1_DRESS_AT_639 = Macro639;
                    //            SetGw1Speed(Macro639);
                    //        }
                    //        else
                    //        {
                    //            if (GW1_DRESS_AT_639 != Macro639)
                    //            {
                    //                GW1_DRESS_AT_639 = Macro639;
                    //                SetGw1Speed(Macro639);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        bGW1_DRESS_AT = false;//清除為未觸發
                    //    }
                    //}
                    //else//恆速功能關閉
                    //{
                    //    GW1_DRESS_AT_639 = 0;
                    //    bGW1_DRESS_AT = false;//清除為未觸發
                    //}
                    #endregion GW1 恆速

                    if (focas.ReadMacro(1033, out double Macro1033) == SUCCESS)//砂輪旗標
                    {
                        int GwNo = (int)Math.Round(Macro1033);

                        //程式執行中
                        if (F0.BIT_7() && F3.BIT_5())
                        {
                            //量測1伸出
                            if (Y2.BIT_3())
                            {
                                //量測切組
                                focas.PMC_WriteByte(PmcAddrType.R, 2150, 0);
                            }
                            //量測2伸出
                            else if (Y3.BIT_4())
                            {
                                //量測切組
                                focas.PMC_WriteByte(PmcAddrType.R, 2150, 16);
                            }

                            if (GwNo == 1)
                            {
                                //GW1研磨中
                                if (R502.BIT_5())
                                {
                                    //音頻切組1-1
                                    focas.PMC_WriteByte(PmcAddrType.R, 2158, 0x00);
                                }
                                //GW1修砂中
                                else if (R502.BIT_6())
                                {
                                    //音頻切組1-2
                                    focas.PMC_WriteByte(PmcAddrType.R, 2158, 0x10);
                                }
                            }
                            else if (GwNo == 2)
                            {
                                //GW2研磨中
                                if (R502.BIT_5())
                                {
                                    //音頻切組1-3
                                    focas.PMC_WriteByte(PmcAddrType.R, 2158, 0x20);
                                }
                                //GW2修砂中 
                                else if (R502.BIT_6())
                                {
                                    //音頻切組1-4
                                    focas.PMC_WriteByte(PmcAddrType.R, 2158, 0x30);
                                }
                            }
                        }
                    }

                    //focas.ReadMacro(506, out double curr_gw_no);
                    //CurrentGwNo = (int)Math.Round(curr_gw_no);

                    focas.PMC_ReadByte(PmcAddrType.R, 135, out byte R135);//手動狀態 (MPG/JOG)

                    //狀態
                    focas.GetStatusInfo(out Status);
                    String AutName = Status.Automatic;

                    this.Invoke(new Action(() =>
                    {
                        //E2822.4 SBK
                        la_SBK.Visible = E2822.BIT_4();

                        pa_Alarm.Visible = Status.AlarmNo != 0;
                        pa_EMG.Visible = Status.EmergncyNo == 1;
                    }));


                    {
                        byte[] Pmc_A_Reg = new byte[20];

                        IODBPMC0 A0_A4 = new IODBPMC0(); //cdata Size=5 
                        IODBPMC0 A5_A9 = new IODBPMC0(); //cdata Size=5 
                        IODBPMC0 A10_A14 = new IODBPMC0(); //cdata Size=5 
                        IODBPMC0 A15_A19 = new IODBPMC0(); //cdata Size=5 

                        int result = Focas1.pmc_rdpmcrng(focas.FlibHndl, (short)PmcAddrType.A, 0, 0, 4, 13, A0_A4);
                        result = Focas1.pmc_rdpmcrng(focas.FlibHndl, (short)PmcAddrType.A, 0, 5, 9, 13, A5_A9);
                        result = Focas1.pmc_rdpmcrng(focas.FlibHndl, (short)PmcAddrType.A, 0, 10, 14, 13, A10_A14);
                        result = Focas1.pmc_rdpmcrng(focas.FlibHndl, (short)PmcAddrType.A, 0, 15, 19, 13, A15_A19);
                        if (result == Focas1.EW_OK)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Pmc_A_Reg[i] = A0_A4.cdata[i];
                                Pmc_A_Reg[i + 5] = A5_A9.cdata[i];
                                Pmc_A_Reg[i + 10] = A10_A14.cdata[i];
                                Pmc_A_Reg[i + 15] = A15_A19.cdata[i];
                            }
                        }




                        //讀取Pmc 訊息
                        bool bShowMsgTip = false;
                        string msgTip = "";
                        //要讀取那些 PMC 暫存器會建立在 PmcMessage.txt
                        foreach (PmcMessageData data in PmcMsgList)
                        {
                            if (data.Type != PmcMessageType.Message) continue; //這邊只處理訊息類的, 異常類的先跳過
                            //讀取暫存器
                            //focas.PMC_ReadByte(PmcAddrType.A, (ushort)(data.Address), out byte val);
                            byte val = Pmc_A_Reg[data.Address];
                            int shift = 1;
                            for (int j = 0; j < 8; j++)//bit0 ~ bit7
                            {
                                if ((val & shift) != 0)
                                {
                                    string code = "EX" + (data.Number + j).ToString("0000");
                                    msgTip += code + " " + PmcAlarmTable.FindCode(code).Msg + "\r\n";
                                    bShowMsgTip = true;
                                }
                                shift <<= 1;
                            }
                        }

                        //讀取PMC Alarm
                        bool bShowAlmTip = false;
                        string almTip = "";
                        //要讀取那些 PMC 暫存器會建立在 PmcMessage.txt
                        foreach (PmcMessageData data in PmcMsgList)
                        {
                            if (data.Type != PmcMessageType.Alarm) continue;
                            //focas.PMC_ReadByte(PmcAddrType.A, (ushort)(data.Address), out byte val);
                            byte val = Pmc_A_Reg[data.Address];
                            int shift = 1;
                            for (int j = 0; j < 8; j++)//bit0 ~ bit7
                            {
                                if ((val & shift) != 0)
                                {
                                    string code = "EX" + (data.Number + j).ToString("0000");
                                    almTip += code + " " + PmcAlarmTable.FindCode(code).Msg + "\r\n";
                                    bShowAlmTip = true;
                                }
                                shift <<= 1;
                            }
                        }



                        this.Invoke(new Action(() =>
                        {

                            //在 Top 顯示訊息(黃底)
                            pa_Tip.Visible = bShowMsgTip;
                            la_Tip.Text = msgTip;

                            //在 Top 顯示Alarm(紅底)
                            pa_AlarmTip.Visible = bShowAlmTip;
                            la_AlarmTip.Text = almTip;

                            btn_AUTO.Lamp = AutName == "MEM";
                            btn_EDIT.Lamp = AutName == "EDIT";
                            btn_MDI.Lamp = AutName == "MDI";
                            //btn_MPG.Lamp = AutName == "Handle" || AutName == "MPG";
                            //btn_JOG.Lamp = AutName == "JOG";
                            //JOG模式在按下Button才會變成JOG模式，所以先用此方式先處理，但是又要避開一鍵呼叫跳模式的問題
                            btn_MPG.Lamp = F0.BIT_6() && R135.BIT_5() && !btn_AUTO.Lamp;
                            btn_JOG.Lamp = F0.BIT_6() && R135.BIT_4() && !btn_AUTO.Lamp;
                            btn_HOME.Lamp = AutName == "Reference";
                        }));

                    }

                    //檢查 NC 是否有呼叫 PC的函式
                    //NC_Call_PC();

                    ret = focas.ReadAllAxisPos(out Pos);
                    if (ret != SUCCESS) continue;

                    if (Pos.Absolute == null) continue;
                    double y = 0;
                    if (Pos.Absolute.Length > 2) y = Pos.Absolute[2];
                    focas.PMC_ReadByte(PmcAddrType.E, 2400, out byte E2400);
                    if (Math.Floor(y) - Math.Round(y, 5) == 0) E2400 = E2400.SetBit(0, true);
                    else E2400 = E2400.SetBit(0, false);
                    focas.PMC_WriteByte(PmcAddrType.E, 2400, E2400);

                    PmcRefleshStart = Environment.TickCount;
                }



                bool bDressBaseSetting = false;
                bool bMonitor = false;
                bool bDressGwSetting = false;
                bool bDressPartsSetting = false;
                bool bEditProg = false;
                bool bManual = false;
                bool bSoftPanel = false;
                bool bMessage = false;
                bool bDressGwConv = false;
                bool bDressPartConv = false;
                bool bRegister = false;
                bool bMaintain = false;
                bool bFuncSwitch = false;
                bool bCondition = false;
               
                this.Invoke((Action)(() =>
                {
                    bMonitor = TC_Main.SelectedTab == tab_Monitor;
                    bDressGwSetting = TC_Main.SelectedTab == tab_DressGwSetting;
                    bDressPartsSetting = TC_Main.SelectedTab == tab_DressPartsSetting;
                    bEditProg = TC_Main.SelectedTab == tab_EditProc;
                    bManual = TC_Main.SelectedTab == tab_Manual;
                    bSoftPanel = pa_SoftPanel.Visible;
                    bMessage = TC_Main.SelectedTab == tab_Message;
                    bDressGwConv = TC_Main.SelectedTab == tab_DressGwConv;
                    bDressPartConv = TC_Main.SelectedTab == tab_DressPartsConv;
                    bDressBaseSetting = TC_Main.SelectedTab == tab_PosSet;
                    bRegister = TC_Main.SelectedTab == tab_Regist;
                    bMaintain = TC_Main.SelectedTab == tab_Maintenance;
                    bFuncSwitch = TC_Main.SelectedTab == tab_FuncSwitch;
                    bCondition = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab == tab_Gw_DressCondition;

                }));

                int Request_CNC_Time = Environment.TickCount - Request_CNC_Time_Start;
                if (Request_CNC_Time > 1000 && !bLayout && !bImport)
                {
                    Request_CNC_Time_Start = Environment.TickCount;

                    focas.GetTimer(out DateTime dt);

                    this.Invoke(new Action(() =>
                    {
                        la_Time.Text = dt.ToString("yyyy/MM/dd HH:mm");
                    }));

                }

                //監視
                if (bMonitor && !bRefleshMonitor)
                {
                    bMonitorBuzy = true;

                    // C 軸資料待確認
                    //focas.PMC_ReadByte(PmcAddrType.K, 9, out byte K9);
                    //if (K9.BIT_7())
                    //{
                    //    //馬達實際轉速*(參數2084*1000)/(參數2085*360)
                    //    focas.PMC_ReadDbWord(PmcAddrType.E, 5120, out uint E5120);
                    //    int m_spd = (int)E5120;

                    //    //軸號, 依照機型不同,C軸可能在第三軸或第四軸
                    //    int axis_no = AxisNo["C"] + 1;

                    //    //讀取該軸參數
                    //    focas.Param_ReadWord(2084, (short)axis_no, out short P2084);
                    //    focas.Param_ReadWord(2085, (short)axis_no, out short P2085);

                    //    //計算主軸轉速
                    //    double sp_rpm = Math.Abs((double)m_spd * (P2084 * 1000.0) / (P2085 * 360.0));

                    //    this.Invoke((Action)(() =>
                    //    {
                    //        la_SpCurrentRpm_Val.Text = sp_rpm.ToString("0");
                    //    }));
                    //}

                    //加工中, 顯示目前執行到哪個工序
                    focas.PMC_ReadByte(PmcAddrType.F, 0, out byte F0);
                    if (F0.BIT_7())
                    {
                        focas.ReadMacro(118, out double process_no);
                        int process_index = (int)Math.Round(process_no) - 1;
                        this.Invoke((Action)(() =>
                        {
                            if ((process_index >= 0) && (process_index < DGV_Monitor_Program.Rows.Count))
                            {
                                DGV_Monitor_Program.CurrentCell = DGV_Monitor_Program.Rows[process_index].Cells[0];
                            }
                        }));
                    }

                    //主軸倍率
                    focas.PMC_ReadByte(PmcAddrType.E, 2522, out byte E2522);
                    int data = E2522 & 7;
                    this.Invoke((Action)(() =>
                    {
                        if (data == 7) btn_SpRate.DisplayText = "50";
                        else if (data == 6) btn_SpRate.DisplayText = "60";
                        else if (data == 5) btn_SpRate.DisplayText = "70";
                        else if (data == 4) btn_SpRate.DisplayText = "80";
                        else if (data == 3) btn_SpRate.DisplayText = "90";
                        else if (data == 2) btn_SpRate.DisplayText = "100";
                        else if (data == 1) btn_SpRate.DisplayText = "110";
                        else if (data == 0) btn_SpRate.DisplayText = "120";
                    }));

                    //進給速度的倍率(從實體旋鈕反算)
                    focas.PMC_ReadByte(PmcAddrType.X, 11, out byte X11);
                    int rate_index = (byte)(((X11 | 3) ^ 0xFF) >> 3);
                    if (rate_index > 22) rate_index = 22;
                    double[] rate = { 0, 0.02, 0.06, 0.08, 0.10, 0.15, 0.20, 0.30, 0.40, 0.50, 0.60, 0.65, 0.70, 0.75, 0.80, 0.85, 0.90, 0.95, 1.00, 1.05, 1.10, 1.15, 1.20 };

                    //指令下的進給速度(不會跟著倍率變)
                    focas.ReadMacro(4109, out double dF);

                    //計算實際進給速度
                    //focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);
                    //if (F2.BIT_0()) la_ActF.Text = (dF * rate[rate_index]).ToString("0.00000");
                    //else la_ActF.Text = (dF * rate[rate_index]).ToString("0.#####");

                    this.Invoke((Action)(() =>
                    {
                        la_ActF.Text = (dF * rate[rate_index]).ToString("0.#####");
                    }));

                    //每秒輪詢
                    int iQueryMonitorOneSecTime = Environment.TickCount - iQueryMonitorOneSec;
                    if (iQueryMonitorOneSecTime > 1000)
                    {
                        iQueryMonitorOneSec = Environment.TickCount;
                        if (!bRun && !bOneKeyCall)
                        {
                            focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000"); //自動回O8000
                        }

                        //刀具 T Code
                        focas.ReadMacro(4120, out double act_t);


                        if (F0.BIT_7()) //加工中要顯示的訊息(每秒更新)
                        {
                            //透過 T CODE 間接知道目前是第幾工序
                            focas.ReadMacro(119, out double TCode);
                            int iTCode = (int)Math.Round(TCode);
                            int process_Index = iTCode - 1;

                            //目前執行中的工序
                            focas.ReadMacro(959, out double CD);

                            //顯示剩餘研磨量
                            focas.ReadMacro(117, out double M117);
                            focas.ReadMacro(156, out double M156);
                            focas.ReadMacro(155, out double M155);
                            focas.ReadMacro(149, out double M149);
                            focas.ReadMacro(186, out double M186);
                            focas.ReadMacro(185, out double M185);


                            focas.ReadMacro(963, out double GrindType);
                            focas.ReadMacro(158, out double sp);

                            this.Invoke((Action)(() =>
                            {
                                if ((process_Index >= 0) && (process_Index < DGV_Monitor_Program.Rows.Count))
                                {
                                    //自動選擇目前正在執行的工序
                                    DGV_Monitor_Program.CurrentCell = DGV_Monitor_Program.Rows[process_Index].Cells[0];
                                    SB_CurrentProcess.Text = iTCode.ToString();
                                }
                                else
                                {
                                    SB_CurrentProcess.Text = "-";
                                }

                                //顯示修砂次數
                                la_DressGwCounter_Val.Visible = CD == 71;//砂輪修整程式

                                //顯示目前工序名稱
                                la_ProcessName.Visible = true;
                                String name = LanguageManager.LoadMessage(Units.langfile, "ProgramName", (int)Math.Round(CD), "");
                                la_ProcessName.Text = name;


                                if (M117 == 1)//顯示剩餘研磨量
                                {
                                    double total_amount = M156 + M155 + M149 + M186 - M185;
                                    la_RemainGrindAmount.Visible = true;
                                    la_RemainGrindAmountValue.Visible = true;
                                    la_RemainGrindAmountValue.Text = total_amount.ToString(Units.DisplayFmt);
                                }
                                else//隱藏剩餘研磨量
                                {
                                    la_RemainGrindAmount.Visible = false;
                                    la_RemainGrindAmountValue.Visible = false;
                                }

                                if (GrindType == 1)
                                {
                                    pic_Exe.Image = new Bitmap(Application.StartupPath + "\\image\\ex1.png");
                                    la_Monitor_SparkoutCount.Text = "";
                                }
                                else if (GrindType == 2)
                                {
                                    pic_Exe.Image = new Bitmap(Application.StartupPath + "\\image\\ex2.png");
                                    la_Monitor_SparkoutCount.Text = "";
                                }
                                else if (GrindType == 3)
                                {
                                    pic_Exe.Image = new Bitmap(Application.StartupPath + "\\image\\ex3.png");
                                    la_Monitor_SparkoutCount.Text = "";
                                }
                                else if (GrindType == 4)
                                {
                                    pic_Exe.Image = new Bitmap(Application.StartupPath + "\\image\\ex4.png");

                                    //la_Monitor_SparkoutCount.Visible = true;                                    

                                    if (double.IsNaN(sp))
                                    {
                                        la_Monitor_SparkoutCount.Text = "";
                                    }
                                    else
                                    {
                                        la_Monitor_SparkoutCount.Text = sp.ToString("0");
                                    }
                                }
                                else
                                {
                                    pic_Exe.Image = new Bitmap(80, 32);
                                    la_Monitor_SparkoutCount.Text = "";
                                }
                            }));

                        }
                        else//非加工中 隱藏那些資訊
                        {
                            this.Invoke((Action)(() =>
                            {
                                la_ProcessName.Visible = false;//工序名稱
                                la_DressGwCounter_Val.Visible = false;//修砂次數
                                la_RemainGrindAmount.Visible = false;//剩餘研磨量(名稱)
                                la_RemainGrindAmountValue.Visible = false;//剩餘研磨量(數值)

                                pic_Exe.Image = new Bitmap(80, 32);//加工模式(粗/細/精/無火花)
                                la_Monitor_SparkoutCount.Text = "";//無火花次數
                            }));
                        }


                        //主軸轉速 S
                        focas.ReadMacro(4319, out double ActS);

                        //工件數量
                        focas.Param_ReadDbWord(6711, -1, out int workpiece);
                        //工件總數
                        focas.Param_ReadDbWord(6712, -1, out int total);

                        //加工時間(循環時間)
                        focas.Param_ReadDbWord(6757, 0, out int msec);
                        focas.Param_ReadDbWord(6758, 0, out int min);
                        int total_sec = min * 60 + msec / 1000;
                        min = total_sec / 60;
                        int sec = total_sec % 60;

                        //修砂次數
                        focas.ReadMacro(513, out double c2);
                        focas.ReadMacro(924, out double t2);

                        //軸原點燈號
                        focas.PMC_ReadByte(PmcAddrType.E, 2821, out byte E2821);

                        this.Invoke(new Action(() =>
                        {
                            la_ActT.Text = act_t.ToString("00000000");//刀具 T Code
                            la_ActS.Text = ActS.ToString("0");//主軸轉速 S
                            //la_Total.Text = total.ToString();//工件總數
                            la_FinishPart.Text = workpiece.ToString();//工件數量
                            la_SingleTime.Text = min.ToString("00") + ":" + sec.ToString("00");

                            la_DressGwCounter_Val.Text = c2.ToString("0") + "/" + t2.ToString("0");

                            //軸原點
                            pic_Axis1_Origin.Image = E2821.BIT_1() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                            pic_Axis2_Origin.Image = E2821.BIT_2() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                            pic_Axis3_Origin.Image = E2821.BIT_3() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                            pic_Axis4_Origin.Image = E2821.BIT_4() ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                        }));
                    }
                    bMonitorBuzy = false;
                }
                else if (bDressBaseSetting)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;

                        if (Pos.Machine == null) return;

                        if (Pos.Machine.Length > 0) la_PosSetMach_1.Text = Pos.Machine[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Machine.Length > 1) la_PosSetMach_2.Text = Pos.Machine[1].ToString(Units.DisplayFmt);//Z
                        if (Pos.Machine.Length > 2) la_PosSetMach_3.Text = Pos.Machine[2].ToString(Units.DisplayFmt);//Y or B
                        if (Pos.Machine.Length > 3) la_PosSetMach_4.Text = Pos.Machine[3].ToString(Units.DisplayFmt);//C
                        if (Pos.Machine.Length > 4) la_PosSetMach_5.Text = Pos.Machine[4].ToString(Units.DisplayFmt);//A
                        if (Pos.Machine.Length > 5) la_PosSetMach_6.Text = Pos.Machine[5].ToString(Units.DisplayFmt);//B
                    }));
                }
                //修砂對點(欄位)
                else if (bDressGwSetting)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;

                        if (Pos.Machine == null) return;
                        //機械座標
                        if (Pos.Machine.Length > 0) la_DressMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Machine.Length > 1) la_DressMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);//Z
                                                                                                                            //if (Pos.Machine.Length > 2) la_DressMachAxis3Value.Text = Pos.Machine[2].ToString(Units.DisplayFmt);//C

                        if (Pos.Absolute == null) return;
                        //絕對座標
                        if (Pos.Absolute.Length > 0) la_DressAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Absolute.Length > 1) la_DressAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);//Z
                                                                                                                             //if (Pos.Absolute.Length > 2) la_DressAbsAxis3Value.Text = Pos.Absolute[2].ToString(Units.DisplayFmt);//C
                    }));
                }
                //修砂對點2(對話式)
                else if (bDressGwConv)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;

                        if (Pos.Machine == null) return;

                        //機械座標
                        if (Pos.Machine.Length > 0) la_DressGwMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                        if (Pos.Machine.Length > 1) la_DressGwMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);

                        if (Pos.Absolute == null) return;

                        //絕對座標
                        if (Pos.Absolute.Length > 0) la_DressGwAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);
                        if (Pos.Absolute.Length > 1) la_DressGwAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);
                    }));
                }
                //加工對點
                else if (bDressPartsSetting)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;
                        if (Pos.Machine == null) return;
                        if (Pos.Absolute == null) return;


                        //機械座標
                        if (Pos.Machine.Length > 0) la_PartsMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                        if (Pos.Machine.Length > 1) la_PartsMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                        if (AxisNo.ContainsKey("B"))
                        {
                            int bIndex = AxisNo["B"];
                            if(bIndex < 0) return;
                            if (Pos.Machine.Length > bIndex) la_PartsMachAxis3Value.Text = Pos.Machine[bIndex].ToString(Units.DisplayFmt);
                        }

                        //if (Pos.Machine.Length > 2) la_PartsMachAxis3Value.Text = Pos.Machine[2].ToString(Units.DisplayFmt);

                        //絕對座標
                        if (Pos.Absolute.Length > 0) la_PartsAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);
                        if (Pos.Absolute.Length > 1)
                        {
                            //if (bShowG59)
                            //{
                            //G54         G55         G57         G58         G59
                            //#5221,#5222,#5241,#5242,#5281,#5282,#5301,#5302,#5321,#5322                                                                
                            if (focas.ReadMacro(5322, out double data) == SUCCESS) la_PartsAbsAxis2Value.Text = (Pos.Machine[1] - data).ToString(Units.DisplayFmt);
                            //}
                            //else//G54
                            //{
                            if (Pos.Machine.Length > 1) la_PartsAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);
                            //}
                        }
                        if (AxisNo.ContainsKey("B"))
                        {
                            int bIndex = AxisNo["B"];
                            if (Pos.Absolute.Length > bIndex) la_PartsAbsAxis3Value.Text = Pos.Absolute[bIndex].ToString(Units.DisplayFmt);
                        }
                    }));
                }
                //加工對點2
                else if (bDressPartConv)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;

                        if (Pos.Machine == null) return;
                        //機械座標
                        if (Pos.Machine.Length > 0)//X
                        {
                            la_DressPartsMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                            la_DP_Rel_X.Text = (Pos.Machine[0] - dManualZeroPoint).ToString(Units.DisplayFmt);
                        }
                        if (Pos.Machine.Length > 1)//Z
                        {
                            la_DressPartsMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                            la_DP_Rel_Z.Text = (Pos.Machine[1] - dManualZeroPointZ).ToString(Units.DisplayFmt);
                        }

                        if (Pos.Absolute == null) return;
                        //絕對座標
                        if (Pos.Absolute.Length > 0) la_DressPartsAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Absolute.Length > 1) la_DressPartsAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);//Z

                        if (Pos.Distance == null) return;
                        //殘移動量
                        if (Pos.Distance.Length > 0) la_DressPartsDistAxis1Value.Text = Pos.Distance[0].ToString(Units.DisplayFmt);//X
                        if (Pos.Distance.Length > 1) la_DressPartsDistAxis2Value.Text = Pos.Distance[1].ToString(Units.DisplayFmt);//Z

                    }));
                }
                //程式編輯
                else if (bEditProg)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;

                        if (Pos.Machine == null) return;
                        //機械座標
                        if ((AxisNo["X"] != -1) && (Pos.Machine.Length > AxisNo["X"])) la_EditMachAxis1Value.Text = Pos.Machine[AxisNo["X"]].ToString(Units.DisplayFmt);//X
                        if ((AxisNo["Z"] != -1) && (Pos.Machine.Length > AxisNo["Z"])) la_EditMachAxis2Value.Text = Pos.Machine[AxisNo["Z"]].ToString(Units.DisplayFmt);//Z
                        if ((AxisNo["B"] != -1) && (Pos.Machine.Length > AxisNo["B"])) la_EditMachAxis3Value.Text = Pos.Machine[AxisNo["B"]].ToString(Units.DisplayFmt);//B (only OIG R series) 

                        la_EditMachAxisProbe1Value.Text = la_EditMachAxis1Value.Text;
                        la_EditMachAxisProbe2Value.Text = la_EditMachAxis2Value.Text;

                        if (Pos.Absolute == null) return;
                        //絕對座標
                        if ((AxisNo["X"] != -1) && (Pos.Absolute.Length > AxisNo["X"])) la_EditAbsAxis1Value.Text = Pos.Absolute[AxisNo["X"]].ToString(Units.DisplayFmt);//X
                        if ((AxisNo["Z"] != -1) && (Pos.Absolute.Length > AxisNo["Z"])) la_EditAbsAxis2Value.Text = Pos.Absolute[AxisNo["Z"]].ToString(Units.DisplayFmt);//Z
                        if ((AxisNo["B"] != -1) && (Pos.Absolute.Length > AxisNo["B"])) la_EditAbsAxis3Value.Text = Pos.Absolute[AxisNo["B"]].ToString(Units.DisplayFmt);//B (only OIG R series) 

                        if (Pos.Distance == null) return;
                        //殘移動量
                        if ((AxisNo["X"] != -1) && (Pos.Distance.Length > AxisNo["X"])) la_EditDistAxisProbe1Value.Text = Pos.Distance[AxisNo["X"]].ToString(Units.DisplayFmt);//X
                        if ((AxisNo["Z"] != -1) && (Pos.Distance.Length > AxisNo["Z"])) la_EditDistAxisProbe2Value.Text = Pos.Distance[AxisNo["Z"]].ToString(Units.DisplayFmt);//Z

                        if (fo_TraverseStep != null)
                        {
                            if (Pos.Machine.Length > 0) fo_TraverseStep.la_EditMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);//X
                            if (Pos.Machine.Length > 1) fo_TraverseStep.la_EditMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);//Z
                            if (Pos.Absolute.Length > 0) fo_TraverseStep.la_EditAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);//X
                            if (Pos.Absolute.Length > 1) fo_TraverseStep.la_EditAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);//Z
                            la_EditMachAxisProbe1Value.Text = la_EditAbsAxis1Value.Text;
                            la_EditMachAxisProbe2Value.Text = la_EditAbsAxis2Value.Text;
                        }
                    }));
                }

                else if (bMaintain)
                {
                    this.Invoke((Action)(() =>
                    {

                        if (focas.PMC_ReadByte(PmcAddrType.X, 40, out byte X40) == SUCCESS && focas.PMC_ReadByte(PmcAddrType.K, 2, out byte K2) == SUCCESS)
                        {
                            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                            //pic_DoorStatus.Image

                            Graphics g = Graphics.FromImage(pic_DoorStatus.Image);
                            if (K2.BIT_0()) { g.FillRectangle(new SolidBrush(X40.BIT_1() ? Color.Lime : Color.Red), new Rectangle(18, 45, 7, 30)); }
                            else { g.FillRectangle(new SolidBrush(Color.White), new Rectangle(18, 45, 7, 30)); }
                            if (K2.BIT_1()) { g.FillRectangle(new SolidBrush(X40.BIT_2() ? Color.Lime : Color.Red), new Rectangle(50, 16, 36, 7)); }
                            else { g.FillRectangle(new SolidBrush(Color.White), new Rectangle(50, 16, 36, 7)); }
                            if (K2.BIT_2()) { g.FillRectangle(new SolidBrush(X40.BIT_3() ? Color.Lime : Color.Red), new Rectangle(212, 45, 7, 30)); }
                            else { g.FillRectangle(new SolidBrush(Color.White), new Rectangle(212, 45, 7, 30)); }
                            if (K2.BIT_3()) { g.FillRectangle(new SolidBrush(X40.BIT_4() ? Color.Lime : Color.Red), new Rectangle(150, 16, 36, 7)); }
                            else { g.FillRectangle(new SolidBrush(Color.White), new Rectangle(150, 16, 36, 7)); }
                            pic_DoorStatus.Refresh();
                        }

                    }));
                }


                //手動研磨
                else if (bManual)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS) return;

                        if (ret == SUCCESS)
                        {
                            if (Pos.Machine == null) return;
                            //機械座標
                            if (Pos.Machine.Length > 0) la_ManualMachAxis1Value.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                            if (Pos.Machine.Length > 1) la_ManualMachAxis2Value.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                            //if (Pos.Machine.Length > 2) la_ManualMachAxis3Value.Text = Pos.Machine[2].ToString("0.000");

                            if (Pos.Absolute == null) return;
                            //絕對座標
                            if (Pos.Absolute.Length > 0) la_ManualAbsAxis1Value.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);
                            if (Pos.Absolute.Length > 1) la_ManualAbsAxis2Value.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);
                            //if (Pos.Absolute.Length > 2)la_ManualAbsAxis3Value.Text = Pos.Absolute[2].ToString("0.000");

                            if (Pos.Distance == null) return;
                            //殘移動量
                            if (Pos.Distance.Length > 0) la_ManualDistAxis1Value.Text = Pos.Distance[0].ToString(Units.DisplayFmt);
                            if (Pos.Distance.Length > 1) la_ManualDistAxis2Value.Text = Pos.Distance[1].ToString(Units.DisplayFmt);
                            //if (Pos.Distance.Length > 2)la_ManualDistAxis3Value.Text = Pos.Distance[2].ToString("0.000");
                        }
                        else
                        {
                            focas.Disconnect();
                            //pic_ConnectStatus.Image = Properties.Resources.disconnects;
                        }
                    }));
                }
                else if (bMessage)
                {

                    if (Status.AlarmNo != 0) //CNC目前有異常發生
                    {
                        ReadAlarmHistory(); //讀取歷史訊息
                    }
                    else  //CNC目前沒有異常
                    {
                        this.Invoke(new Action(() =>
                        {
                            //清除目前顯示的異常
                            if (CurrentAlarm.Items.Count > 0)
                            {
                                LB_CurrentAlarm.Items.Clear();
                                la_TroubleShooting.Text = "";
                                CurrentAlarm.Items.Clear();
                            }
                        }));
                    }
                }
                else if (bWarmup)
                {
                    ret = focas.ReadAllAxisPos(out Pos);
                    this.Invoke((Action)(() =>
                    {

                        if (ret != SUCCESS)
                        {
                            focas.Disconnect();
                            //pic_ConnectStatus.Image = Properties.Resources.disconnects;
                        }
                        else
                        {
                            if (Pos.Machine == null) return;
                            //機械座標
                            if (Pos.Machine.Length > 0) fo_Warmup.la_WarnMach_1.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                            if (Pos.Machine.Length > 1) fo_Warmup.la_WarnMach_2.Text = Pos.Machine[1].ToString(Units.DisplayFmt);

                            if (Pos.Absolute == null) return;
                            //絕對座標
                            if (Pos.Absolute.Length > 0) fo_Warmup.la_WarnAbs_1.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);
                            if (Pos.Absolute.Length > 1) fo_Warmup.la_WarnAbs_2.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);

                            if (Pos.Distance == null) return;
                            //殘移動量
                            if (Pos.Distance.Length > 0) fo_Warmup.la_WarnDist_1.Text = Pos.Distance[0].ToString(Units.DisplayFmt);
                            if (Pos.Distance.Length > 1) fo_Warmup.la_WarnDist_2.Text = Pos.Distance[1].ToString(Units.DisplayFmt);

                        }

                        focas.ReadMacro(827, out double WarnTimesCount);
                        fo_Warmup.la_WarnTimes_Count.Text = WarnTimesCount.ToString("0");
                    }));
                }
                else if (bFuncSwitch)
                {
                    this.Invoke((Action)(() =>
                    {
                        focas.PMC_ReadByte(PmcAddrType.K, 30, out Byte K30);
                        focas.PMC_ReadByte(PmcAddrType.K, 31, out Byte K31);
                        btn_SW1_OFF.Lamp = !K30.BIT_0();
                        btn_SW1_ON.Lamp = K30.BIT_0();

                        btn_SW2_OFF.Lamp = !K30.BIT_1();
                        btn_SW2_ON.Lamp = K30.BIT_1();

                        btn_SW3_OFF.Lamp = !K30.BIT_2();
                        btn_SW3_ON.Lamp = K30.BIT_2();

                        btn_SW4_OFF.Lamp = !K30.BIT_3();
                        btn_SW4_ON.Lamp = K30.BIT_3();

                        btn_SW5_OFF.Lamp = !K30.BIT_4();
                        btn_SW5_ON.Lamp = K30.BIT_4();

                        btn_SW6_OFF.Lamp = !K30.BIT_6();
                        btn_SW6_ON.Lamp = K30.BIT_6();

                        btn_SW7_OFF.Lamp = !K31.BIT_0();
                        btn_SW7_ON.Lamp = K31.BIT_0();

                        btn_SW8_OFF.Lamp = !K31.BIT_1();
                        btn_SW8_ON.Lamp = K31.BIT_1();

                        btn_SW9_OFF.Lamp = !K31.BIT_2();
                        btn_SW9_ON.Lamp = K31.BIT_2();

                        btn_SW10_OFF.Lamp = !K31.BIT_7();
                        btn_SW10_ON.Lamp = K31.BIT_7();
                    }));
                }
                else if (bCondition)
                {
                    this.Invoke((Action)(() =>
                    {
                        if (Pos.Machine == null) return;
                        //機械座標
                        if (Pos.Machine.Length > 0) la_Cond_Mach_1.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                        if (Pos.Machine.Length > 1) la_Cond_Mach_2.Text = Pos.Machine[1].ToString(Units.DisplayFmt);

                        if (Pos.Absolute == null) return;
                        //絕對座標
                        if (Pos.Absolute.Length > 0) la_Cond_Abs_1.Text = Pos.Absolute[0].ToString(Units.DisplayFmt);
                        if (Pos.Absolute.Length > 1) la_Cond_Abs_2.Text = Pos.Absolute[1].ToString(Units.DisplayFmt);

                        if (Pos.Distance == null) return;
                        //殘移動量
                        if (Pos.Distance.Length > 0) la_Cond_Dist_1.Text = Pos.Distance[0].ToString(Units.DisplayFmt);
                        if (Pos.Distance.Length > 1) la_Cond_Dist_2.Text = Pos.Distance[1].ToString(Units.DisplayFmt);

                    }));
                }
                //軟體面板
                if (bSoftPanel && !bRefleshSoftPanell)
                {
                    int iQuerySoftPanelTime = Environment.TickCount - iQuerySoftPanelStart;
                    if (iQuerySoftPanelTime > 500)
                    {
                        bSoftPanelBuzy = true;

                        iQuerySoftPanelStart = Environment.TickCount;

                        string[] keys = PMC_Values.Keys.ToArray();

                        //讀取所有軟體面板的Lamp 暫存器
                        for (int i = 0; i < keys.Length; i++)
                        {
                            string addr = keys[i];
                            if (addr != "")
                            {
                                PmcAddrType type = PmcAddrType.E; //取得英文
                                if (addr[0] == 'E') type = PmcAddrType.E;
                                else if (addr[0] == 'R') type = PmcAddrType.R;
                                else if (addr[0] == 'X') type = PmcAddrType.X;
                                else if (addr[0] == 'Y') type = PmcAddrType.Y;
                                else if (addr[0] == 'F') type = PmcAddrType.F;
                                else if (addr[0] == 'G') type = PmcAddrType.G;
                                else if (addr[0] == 'D') type = PmcAddrType.D;

                                if (ushort.TryParse(addr.Substring(1), out ushort n))
                                {
                                    if (focas.PMC_ReadByte(type, n, out byte val) == SUCCESS)
                                    {
                                        PMC_Values[addr] = val;
                                    }
                                }
                            }
                        }

                        this.Invoke(new Action(() =>
                        {

                            foreach (var lamp in SoftPBLamps)
                            {
                                byte val = PMC_Values[lamp.Type.ToString() + lamp.Addr];
                                lamp.SoftPB.Lamp = val.GetBit(lamp.Bit);
                            }
                        }));

                        bSoftPanelBuzy = false;
                    }
                }

                //畫面事件中要讀取控制器的動作
                while (Actions.Count > 0)
                {
                    try
                    {
                        this.Invoke(new Action(() => { pa_Loading.Visible = true; }));
                        Action action = Actions.Dequeue();
                        action();
                        this.Invoke(new Action(() => { pa_Loading.Visible = false; }));
                    }
                    catch
                    { 
                    }
                }

                //程式開啟時，從機台讀回目前哪些工序被設為要執行的
                if (bReadProcessExe)
                {

                    ReadProcessExe();


                    bReadProcessExe = false;
                }
            }
            bCloseFinish = true;
        }



        public void LoadLanguage(string lang)
        {
            String tmpFileName;

            tmpFileName = Application.StartupPath + "\\Language\\" + lang + "\\DefaultProcessLang.xml";  // XML 檔案路徑
            if (!File.Exists(tmpFileName)) Fo_Msg.Show("DefaultProcessLang.xml " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
            else Units.xmlDefaultProcessLang = XDocument.Load(tmpFileName); //預設工序的語言檔

            tmpFileName = Application.StartupPath + "\\Language\\" + lang + "\\Alarm.txt";
            if (!File.Exists(tmpFileName)) Fo_Msg.Show("Alarm.txt " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
            TroubleShootingFile1 = new AlarmFile(tmpFileName);//異常表
            Units.alarmfile.LoadFromFile(tmpFileName);

            tmpFileName = Application.StartupPath + "\\Language\\" + lang + "\\PmcMessageLang.txt";
            if (!File.Exists(tmpFileName)) Fo_Msg.Show("PmcMessageLang.txt " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
            PmcAlarmTable = new AlarmFile(tmpFileName);//異常表

            Units.MacroInfo.LoadLangFile(Units.langfile);//Macro讀取多國語言
        }

        void SetLayout()
        {

            pa_SoftPanel.Parent = this;
            pa_SoftPanel.Left = 0;
            pa_SoftPanel.Top = this.Height - pa_Bottom.Height - pa_SoftPanel.Height;

            //pa_Unlock.Left = 0;
            //pa_Unlock.Top = pa_SoftPanel.Top - pa_Unlock.Height;

            TC_Main.SizeMode = TabSizeMode.Fixed;
            TC_Main.Appearance = TabAppearance.FlatButtons;
            TC_Main.ItemSize = new Size(0, 1);
            this.NativeTabControl1.AssignHandle(TC_Main.Handle);

            TC_Path.SizeMode = TabSizeMode.Fixed;
            TC_Path.Appearance = TabAppearance.FlatButtons;
            TC_Path.ItemSize = new Size(0, 1);
            this.NativeTabControl4.AssignHandle(TC_Path.Handle);

            tc_PositionSet.SizeMode = TabSizeMode.Fixed;
            tc_PositionSet.Appearance = TabAppearance.FlatButtons;
            tc_PositionSet.ItemSize = new Size(0, 1);
            this.NativeTabControl3.AssignHandle(tc_PositionSet.Handle);

            TC_GW.SizeMode = TabSizeMode.Fixed;
            TC_GW.Appearance = TabAppearance.FlatButtons;
            TC_GW.ItemSize = new Size(0, 1);
            this.NativeTabControl2.AssignHandle(TC_GW.Handle);

            //tc_GW_Shape.SizeMode = TabSizeMode.Fixed;
            //tc_GW_Shape.Appearance = TabAppearance.FlatButtons;
            //tc_GW_Shape.ItemSize = new Size(0, 1);
            //this.NativeTabControl3.AssignHandle(tc_GW_Shape.Handle);

            TC_EditProc.SizeMode = TabSizeMode.Fixed;
            TC_EditProc.Appearance = TabAppearance.FlatButtons;
            TC_EditProc.ItemSize = new Size(0, 1);

            Uc_RoundBtn[] btn_GwData = { btn_Gw_GwData, btn_Gw_ShapeSelect, btn_Gw_ShapeData, btn_Gw_DressCondition, btn_RegisterGw_Save };
            for (int i = 0; i < btn_GwData.Length; i++)
            {
                btn_GwData[i].Parent = pa_Bottom;
                btn_GwData[i].Left = 544 + 80 * i;
                btn_GwData[i].Top = 8;
            }


            SetMonitorBtns();


        }

        private void SetMonitorBtns()
        {
            List<Uc_RoundBtn> btns = new List<Uc_RoundBtn>();

            btns.Add(btn_Redo);
            btns.Add(btn_DressGw1);
            btns.Add(btn_Monitor_ToChgPos2);
            btns.Add(btn_ToolSelect);
            if (Measopen) btns.Add(btn_MeasureList);
            for (int i = btns.Count - 1; i >= 0; i--)
            {
                Uc_RoundBtn btn = btns[i];
                btn.Parent = pa_Bottom;
                btn.Left = 784 - i * 80;
                btn.Top = 8;
            }
        }

        //引數ComboBox 選擇改變
        private void CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            CB.Visible = false;
            if (DGV_ProcList == null) return;
            if (Edit_DGV.CurrentRow == null) return;


            //取得目前的行
            int Row = Edit_DGV.CurrentCell.RowIndex;
            //取得選擇後的名稱，顯示在目前的欄位中
            if (Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value.ToString() != CB.Text)
            {
                Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value = CB.Text;
                //此行引數的數值欄位(隱藏)的數值 = 此引數對照名稱的數值
                var matchingText = Edit_AVN.Texts.FirstOrDefault(text => text.Name == CB.Text);
                if (matchingText == null)
                    return;
                int val = int.Parse(matchingText.Value);
                Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["DoubleValue"]].Value = val;
                TArgument a = Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
                if (a == null)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 97, "File Error"));
                    return;
                }

                a.Value = val;
                btn_SaveProg.Visible = true;
                btn_SaveProgVisible = true;

                try
                {
                    //內圓沒有端面選擇 固定都是使用G54 遇到特殊客戶需要G59時再來追加
                    //if ((sp.ProgNo == 9001) && (a.AddrCode == "Z"))
                    //{
                    //bShowG59 = Edit_AVN.GetValue(CB.Text) == 1;
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //修整時機
                //if (Edit_AVN.ID.IndexOf(".T") != 0 && Edit_DGV == DGV_Dress1)
                //{
                //DGV_ProcList_CellDoubleClick(DGV_Dress1, null);
                //}

                //量測功能
                /*
                if (Edit_AVN.ID == "19863" && Edit_DGV == DGV_Advance)
                {
                    TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                    if (ini.ReadInteger("System", "Measure_Option", 0) == 1)
                    {
                        //WriteBackArg();
                        //DGV_Edit_CellClick(DGV_Advance, null);
                        if (val == 1)
                        {
                            try
                            {
                                TC_EditProc.SelectedTab = tab_Measure;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            btn_ArgDress2.Visible = true;
                            btn_MeasureElecZero.Enabled = false;
                            //pic_Warning.Visible = true;
                            //la_MeasureMsg.Visible = false;
                        }
                        else
                        {
                            btn_ArgDress2.Visible = false;
                            //pic_Warning.Visible = false;
                        }
                    }
                }
                */
            }

        }

        private void CB2_LostFocus(object sender, EventArgs e)
        {
            //CB2.Visible = false;
            //disableClickTimer.Start();
        }
        private void CB2_SelectedIndexChanged(object sender, EventArgs e) //加工參數 - 下拉式選項
        {
            CB2.Visible = false;
            if (dgv_MP_Param.CurrentRow == null) return;

            int Row = dgv_MP_Param.CurrentRow.Index;



            if (dgv_MP_Param.Rows[Row].Cells[Col_MP_ParamShow.Index].Value.ToString() != CB2.Text)
            {
                //取得Xml節點
                XmlElement x = (XmlElement)dgv_MP_Param.Rows[Row].Cells[Col_Param_XmlNode.Index].Value;

                dgv_MP_Param.Rows[Row].Cells[Col_MP_ParamShow.Index].Value = CB2.Text;
                Dictionary<int, string> Val2Txt = (Dictionary<int, string>)dgv_MP_Param.Rows[Row].Cells[Col_MP_ENUM.Index].Value;
                if (Val2Txt != null && Val2Txt.Count > 0)
                {
                    foreach (int val in Val2Txt.Keys)
                    {
                        if (Val2Txt[val] == CB2.Text)
                        {
                            dgv_MP_Param.Rows[Row].Cells[Col_MP_ParamValue.Index].Value = val;
                            x.SetAttribute("Value", val.ToString());
                            break;
                        }
                    }
                }
            }
        }

        private void pic_ConnectStatus_Click(object sender, EventArgs e)
        {
            Fo_CncConnect form = new Fo_CncConnect();
            if (form.ShowDialog() == DialogResult.OK)
            {
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteString("CNC", "IP", form.TB_IP.Text);
                ini.WriteString("CNC", "Port", form.TB_Port.Text);

                focas.Disconnect();

                IPAddress = form.TB_IP.Text;
                int.TryParse(form.TB_Port.Text, out Port);
                if (form.rb_Ethernet.Checked)
                {
                    focas.ConnectByEthernet(IPAddress, Port);
                }
                else
                {
                    focas.ConnectByHSSB();

                }
            }
        }

        private void btn_Prev_Click(object sender, EventArgs e)
        {
            pic_Descript.Visible = false;

            if (fo_TraverseStep != null)//多段橫進刀
            {
                LoadTraverseData();//儲存並關閉視窗
                return;
            }
            if (TC_Main.SelectedTab == tab_EditProc)
            {
                if (LB_GM_Code.Visible)
                    LB_GM_Code_TextChanged();
                else if (TB_GM_Code.Visible)
                    TB_GM_Code_TextChange();
            }
            //GwEditCheck();
            PrevPage.Pop();
            TC_Main.SelectedTab = PrevPage.Peek();
            btn_Prev.Visible = PrevPage.Count > 1;

        }

        private void btn_Regist_Click(object sender, EventArgs e)
        {
            CheckSaveProgram();

            TC_Main.SelectedTab = tab_Regist;
            pa_SoftPanel.Visible = false;

            PrevPage.Clear();
            PrevPage.Push(tab_Regist);
            btn_Prev.Visible = false;
        }

        private void btn_Monitor_Click(object sender, EventArgs e)
        {


            CheckSaveProgram();
            //GwEditCheck();
            while (bMonitorBuzy) Application.DoEvents();
            bRefleshMonitor = true;
            Application.DoEvents();

            TC_Main.SelectedTab = tab_Monitor;

            if (CurrentProgram == null)
            {
                la_CurrentProgram.Text = "----";
                DGV_Monitor_Program.Rows.Clear();
            }

            pa_SoftPanel.Visible = false;

            PrevPage.Clear();
            PrevPage.Push(tab_Monitor);
            btn_Prev.Visible = false;
        }

        private void SelectProgramList(int id)
        {
            //tab_EditPorgPicture3False();
            for (int i = 0; i < DGV_ProgList.Rows.Count; i++)
            {
                TProgram pg = (TProgram)DGV_ProgList.Rows[i].Cells[Col_TProgram.Index].Value;
                if (pg == null) continue;
                if (id == pg.ID)
                {
                    DGV_ProgList.CurrentCell = DGV_ProgList.Rows[i].Cells[0];
                    break;
                }
            }
        }

        private void btn_Program_Click(object sender, EventArgs e)
        {
            if (fo_TraverseStep != null)
            {
                LoadTraverseData(); //多段橫進刀
            }

            //GwEditCheck(); //離開 砂輪編輯 確認

            //關閉軟體面板
            pa_SoftPanel.Visible = false;


            if (TC_Main.SelectedTab == tab_EditProc || TC_Main.SelectedTab == tab_ProcSelect)//編輯工序 - 按下程式
            {
                btn_Prev.PerformClick();//回上一層 
            }
            else if (TC_Main.SelectedTab == tab_ProcList) //已經在 程式 這頁 
            {
                return;
            }
            else //從其他頁面 按下程式
            {
                //目前已載入程式
                if (CurrentProgram != null)
                {
                    TC_Main.SelectedTab = tab_ProcList; //切到 程式 頁

                    //編輯程式
                    EditProgram(CurrentProgram);

                    PrevPage.Clear();
                    PrevPage.Push(tab_ProcList);
                    btn_Prev.Visible = false;
                }
                else //目前沒有載入程式，進入程式清單
                {
                    TC_Main.SelectedTab = tab_ProgList; //切到 程式清單

                    PrevPage.Clear();
                    PrevPage.Push(tab_ProgList);
                    btn_Prev.Visible = false;
                }
            }


        }

        private void ShowProgListFromDB() //程式頁 - 顯示左側的 程式清單
        {
            //清除欄位
            DGV_ProgList.Rows.Clear();
            int count = 0;
            foreach (TProgram prog in Units.ProgramDB.Programs)
            {
                count++;
                DGV_ProgList.Rows.Add(count.ToString(), prog.Name, prog);
            }

            RefleshProgListBtn();//更新下方按鍵
        }

        private void RefleshProgListBtn()
        {
            bool HasProg = DGV_ProgList.Rows.Count > 0;  //有程式才啟用
            btn_Prog_Del.Enabled = HasProg;//移除程式
            btn_Prog_SaveAs.Enabled = HasProg;//另存程式
            btn_Prog_Open.Enabled = HasProg;//開啟
            btn_Prog_Call.Enabled = HasProg;//呼叫
        }


        //讀取加工程式庫
        public void LoadProgramDB()
        {
            String FileName = Application.StartupPath + "\\ProgramDB.txt";

            //重新讀取
            Units.ProgramDB.LoadFromFile(FileName);//只有數值，不含名稱(且未比對預設工序)
            int count = Units.ProgramDB.Programs.Count;
            for (int i = 0; i < count; i++)
            {
                TProgram program = Units.ProgramDB.Programs[i];
                //例外處理
                if (program == null)
                {
                    ShowProgListFromDB();
                    //ShowProgramList();//更新畫面加工程式列表
                    return;
                }

                //讀取所有工序名稱
                LoadNameFromDef(program);
            }
            ShowProgListFromDB();
            //ShowProgramList();//更新畫面加工程式列表
        }

        //程式從預設工序庫中讀取名稱
        public void LoadNameFromDef(TProgram program)
        {
            //傳入程式中的所有工序
            int proc_count = program.Processes.Count;
            for (int i = 0; i < proc_count; i++)
            {
                //讀取工序(檔案的)
                TProcess process = program.Processes[i];

                //例外處理
                if (process == null) return;

                //取得 預設工序的複製品
                TProcess def_process = GetDefProcess(process.ID);

                //例外處理
                if (def_process == null) return;

                //填入工序名稱
                //process.Name = def_process.Name;

                //所有子程式(檔案的)
                foreach (var sp in process.SubPrograms)
                {
                    //假設ProgramDB.txt皆是舊版格式, 預設工序皆是新版, 從新版複製一份為基礎
                    //避免改版後，新的引數可能會與舊的引數不同，無法匹配導致文字錯誤
                    //主要是把 檔案 的 數值 丟到 預設工序的複製品
                    TSubProgram def_sp = def_process.GetSubProgram(sp.ProgNo);

                    //例外處理
                    if (def_sp == null) return;

                    //防呆用 避免 修整1 與 修整2 的 修整回退量 數值不同
                    Dictionary<string, double> tmp = new Dictionary<string, double>();

                    //所有引數
                    foreach (var def_a in def_sp.Arguments)
                    {
                        //目前只有#19943(修整回退量) 在修整1與修整2都有(共用), 所以要判斷Type
                        string def_NumDotPCode = def_a.Type + "." + def_a.AddrCode;

                        //搜尋資料
                        foreach (var a in sp.Arguments)
                        {
                            //目前只有#19943(修整回退量) 在修整1與修整2都有(共用), 所以要判斷Type
                            string NumDotPCode = a.Type + "." + a.AddrCode;
                            if (NumDotPCode == def_NumDotPCode)//資料匹配
                            {
                                def_a.Value = a.Value;//填入數值
                                def_a.CheckValueLimit = true;//開啟上下限判斷
                                                             // break;
                            }
                        }

                        //修整1 與 修整2 資料同步
                        if (def_a.AddrCode == "19943")//#19943(修整回退量)
                        {
                            if (!tmp.ContainsKey("19943")) tmp.Add("19943", def_a.Value);//寫入//判斷有沒有讀到過
                            else def_a.Value = tmp["19943"];//讀取
                        }
                    }
                    //sp.Assign(def_sp);
                }
                program.Processes[i] = def_process;//將 預設工序的複製品 取代 檔案的工序
            }
        }

        public void LoadProcessDbName()
        {
            foreach (var p in Units.ProcessList)
            {
                //尋找語言檔中的工序
                var lang_processNode = Units.xmlDefaultProcessLang.Descendants("Process").FirstOrDefault(x => x.Attribute("ID")?.Value == p.ID.ToString());
                if (lang_processNode == null)
                {
                    Fo_Msg.Show("[DefaultProcessLang.xml] Format Error : ID[" + p.ID + "] Process ID Not Found.", "Error.");
                    continue;
                }

                //取得工序名稱
                var lang_attr_name = lang_processNode.Attribute("Name");
                if (lang_attr_name == null)
                {
                    Fo_Msg.Show("[DefaultProcessLang.xml] Format Error : ID[" + p.ID + "] Process Name Not Found.", "Error.");
                    //continue;
                }
                p.Name = lang_attr_name.Value;

                //例外處理, 正常會有一支程式, 所有PCode會在裡面
                if (p.SubPrograms.Count <= 0) continue;
                var sp = p.SubPrograms[0];
                if (sp == null)
                {
                    Fo_Msg.Show("[DefaultProcess.xml] Format Error : ID[" + p.ID + "] Sub Program Not Found.", "Error.");
                    continue;
                }

                foreach (TArgument a in sp.Arguments)
                {
                    int pci = 0;

                    var lang_argNode = lang_processNode.Descendants("PCode").FirstOrDefault(x => x.Attribute("No")?.Value == a.AddrCode);
                    if (lang_argNode != null)
                    {
                        var lang_arg_name = lang_argNode.Attribute("Name");
                        if (lang_arg_name != null)
                        {
                            a.Name = lang_arg_name.Value;
                        }
                        else
                        {
                            a.Name = "O" + sp.ProgNo + "." + a.AddrCode + "(Undefine)";
                        }
                        a.SortIndex = pci;
                    }
                }
            }
        }

        //依類型取得預設工序
        public TProcess GetDefProcess(int id)
        {
            TProcess def_process = null;
            int def_proc_count2 = Units.ProcessList.Count;

            for (int k = 0; k < def_proc_count2; k++)
            {
                //同類型工序
                if (Units.ProcessList[k].ID == id)
                {
                    //取得預設工序
                    def_process = Units.ProcessList[k].Clone();
                    break;
                }
            }
            return def_process;
            ////例外處理，內圓Type 1~10; 外圓Type 101~107  內圓D 21~40   外圓DE 41~60;
            //if (!(((int)id >= 1 && (int)id <= 20) || ((int)id >= 21 && (int)id <= 40) || ((int)id >= 41 && (int)id <= 60) || ((int)id == 201) || ((int)id == 999)))
            //{
            //    //Fo_Msg.Show(Units.GetResString("匯入程式錯誤，未定義之工序類型。"));
            //    return null;
            //}

            ////從預設工序庫搜尋
            //TProcess def_process = null;
            //int def_proc_count = Units.DefProcessDB.Processes.Count;
            //for (int k = 0; k < def_proc_count; k++)
            //{
            //    //同類型工序
            //    if (Units.DefProcessDB.Processes[k].ID == id)
            //    {
            //        //取得預設工序
            //        def_process = Units.DefProcessDB.Processes[k].Clone();
            //        break;
            //    }
            //}
            //return def_process;

        }

        private void btn_Message_Click(object sender, EventArgs e)
        {
            if (TC_Main.SelectedTab == tab_Message) return;
            CheckSaveProgram();
            //GwEditCheck();
            //LB_CurrentAlarm.Items.Clear();
            dgv_AlarmHistory.Rows.Clear();
            la_TroubleShooting.Text = "";

            DataTable dt = database.ExecuteReader("SELECT * FROM AlarmHistory ORDER BY Time DESC LIMIT 50;");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string time = dt.Rows[i][0].ToString();
                string code = dt.Rows[i][1].ToString();
                string axis = dt.Rows[i][2].ToString();
                //string path = dt.Rows[i][3].ToString();

                string alarm_msg = Units.alarmfile.FindNameByCode(code);
                if (alarm_msg == "" && PmcAlarmTable.FindCode(code) != null) alarm_msg = PmcAlarmTable.FindCode(code).Msg;//異常訊息修改 
                int.TryParse(axis, out int axis_no);
                if (axis_no != 0) alarm_msg = LanguageManager.LoadMessage(Units.langfile, "Message", 43, "第") + axis_no + LanguageManager.LoadMessage(Units.langfile, "Message", 44, "軸") + alarm_msg;

                dgv_AlarmHistory.Rows.Add(time, code, alarm_msg);
            }
            dgv_AlarmHistory.Sort(Col_Alm_Time, ListSortDirection.Descending);

            TC_Main.SelectedTab = tab_Message;

            pa_SoftPanel.Visible = false;

            PrevPage.Clear();
            PrevPage.Push(tab_Message);
            btn_Prev.Visible = false;

            /*
            Application.DoEvents();

            dgv_AlarmHistory.Rows.Clear();

            focas.Connect();
            if (focas.IsConnect)
            {
                String[] his = focas.ReadAlarmHistory();

                foreach(string s in his)
                {
                    String[] csv = s.Split(',');
                    if (csv.Length >= 3)
                    {
                        //時間、異常碼、訊息
                        dgv_AlarmHistory.Rows.Add(csv[0], csv[1], csv[2]);
                    }
                }
                dgv_AlarmHistory.Sort(Col_Alm_Time, ListSortDirection.Descending);
            }
            */
        }

        private void tab_Paint(object sender, PaintEventArgs e)
        {
            TabPage p = (TabPage)sender;
            if (p == null)
                return;

            e.Graphics.DrawImage(Properties.Resources.background2, new Rectangle(0, 0, p.Width, p.Height));
        }

        private void tab_PaintRect(object sender, PaintEventArgs e)
        {
            TabPage page = sender as TabPage;
            if (page == null) return;

            //e.Graphics.DrawImage(Properties.Resources.background2, new Rectangle(0, 0, p.Width, p.Height));

            double xr = (double)page.Parent.Parent.Width / (double)Resources.background2.Width;//倍率
            double yr = (double)page.Parent.Parent.Height / (double)Resources.background2.Height;//倍率

            //Bitmap bmp = new Bitmap(page.Parent.Parent.Width, page.Parent.Parent.Height);
            //Graphics g = Graphics.FromImage(bmp);
            //g.DrawImage(Resources.background2, 0, 0, tab_PosSet.Width, tab_PosSet.Height);

            Rectangle rect = new Rectangle((int)(page.Parent.Bounds.Location.X / xr),
                                            (int)(page.Parent.Bounds.Location.Y / yr),
                                            (int)(page.Parent.Bounds.Size.Width / xr),
                                            (int)(page.Parent.Bounds.Size.Height / yr));
            e.Graphics.DrawImage(Resources.background2,
                                 page.ClientRectangle,
                                 rect,
                                 GraphicsUnit.Pixel);
        }

        private void btn_Maintenance_Click(object sender, EventArgs e)
        {
            CheckSaveProgram();

            bool bFinish = false;
            int GwNo = 0;
            int GwType = -1;
            Actions.Enqueue(new Action(() =>
            {
                //目前砂輪號
                focas.ReadMacro(506, out double no);
                GwNo = (int)Math.Round(no);
                int shift = (GwNo - 1) * 200;

                if (GwNo > 0 && GwNo <= 4)
                {                 
                    //砂輪類型(0:外圓, 1:內圓(預留))
                    focas.ReadMacro(10004 + shift, out double type);
                    GwType = (int)Math.Round(type);
                    if (GwType < 0 || GwType > 1) GwType = 0; //例外處理
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
            string posSetImgPath = Application.StartupPath + "\\image\\";
            string fileName = posSetImgPath + "OCDPosSet.png";
            if (GwType >= 0 && GwNo > 0 && GwNo <= 4)
            {
                if (GWType[GwNo - 1] == MachineType.OCD2)
                {
                    fileName = posSetImgPath + "OCD2PosSet.png";
                }
                if (GWType[GwNo - 1] == MachineType.OCD3)
                {
                    fileName = posSetImgPath + "OCD3PosSet.png";
                }
                if (GwType == 1)
                {
                    fileName = posSetImgPath + "OIGPosSet.png";
                }
            }
            pic_Position.Image = File.Exists(fileName) ? Image.FromFile(fileName) : null;
            
            //if (TC_Main.SelectedTab != TempMaintanceTab)
            //{

            //    if (TempMaintanceTab != null)
            //    {
            //        pa_SoftPanel.Visible = false;

            //        TC_Main.SelectedTab = (TabPage)TempMaintanceTab;

            //        PrevPage.Push((TabPage)TempMaintanceTab);
            //        btn_Prev.Visible = true;
            //        return;
            //    }
            //}

            //TempMaintanceTab = null;
            TC_Main.SelectedTab = tab_Maintenance;


            //TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //pic_Balance.Visible = ini.ReadInteger("UI", ch_UI_Balance.Name, 0) == 1;
            //la_MaintainBalance.Visible = pic_Balance.Visible;
            //pic_RunSpindle.Visible = ini.ReadInteger("UI", ch_UI_Runin.Name, 0) == 1;
            //la_RunSpindle.Visible = pic_RunSpindle.Visible;

            pa_SoftPanel.Visible = false;

            PrevPage.Clear();
            PrevPage.Push(tab_Maintenance);
            btn_Prev.Visible = false;
        }

        private void SelectMode_Click(object sender, EventArgs e)
        {
            if (bCycleStart)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 46, "自動啟動中，無法切換模式"));
                return;
            }

            Uc_RoundBtn btn = sender is Uc_RoundBtn ? (Uc_RoundBtn)sender : null;
            if (btn == null) return;

            byte.TryParse(btn.Tag.ToString(), out byte tag);
            Actions.Enqueue(new Action(() =>
            {
                focas.PMC_WriteByte(PmcAddrType.E, 2521, (byte)tag);
            }));
        }

        private int ReadAllMacro() //全部讀取 (全部會放在 CurrentMacro)
        {
            //讀取所有Macro 預設FANUC 500~999

            int len = 500;
            int ret = focas.ReadMacro(500, ref len, out double[] macro);
            for (int i = 0; i < macro.Length; i++)
            {
                CurrentMacro[500 + i] = macro[i];
            }
            return ret;
        }

        private int ReadGwMacro(int GwNo) //讀取 砂輪資料 (CurrentGwMacro)
        {
            CurrentGwMacro.Clear();
            int len = 200;
            int ret = focas.ReadMacro(10000 + (GwNo - 1) * 200, ref len, out double[] macro);
            for (int i = 0; i < len; i++)
            {
                CurrentGwMacro[10000 + (i + ((GwNo - 1) * 200))] = macro[i];
            }
            return ret;
        }

        public void UpdateGwImage()
        {
            PictureBox[] pic_EditGws = { pic_EditGw1, pic_EditGw2, pic_EditGw3, pic_EditGw4 };
            Label[] la_EditGws = { la_EditGw1, la_EditGw2, la_EditGw3, la_EditGw4 };

            List<string> filenames = new List<string>();

            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                
                for (int i = 0; i < GwCount; i++)
                {
                    focas.ReadMacro(10005 + i * 200, out double shape);//1, 2, 8
                    focas.ReadMacro(10004 + i * 200, out double type);//0:外圓, 1:內圓(預留)
                    //focas.ReadMacro(671 + i * 2, out double ocd2);//0:直頭, 不等 0 :斜頭
                    if (!(shape == 1 || shape == 2 || shape == 8)) shape = 1;
                    if (type != 1) type = 0;
                    int iShape = (int)Math.Round(shape);
                    int iType = (int)Math.Round(type);

                    bool bOCD2OrOCD3 = GWType[i] == MachineType.OCD2 || GWType[i] == MachineType.OCD3;
                    
                    if (iType == 0 && (GWType[i] == MachineType.OCD2 || GWType[i] == MachineType.OCD3))
                    {
                        string machineTypeName = "OCD2";
                        if(GWType[i] == MachineType.OCD2 && iShape == 3)
                        {
                            iShape = 2;
                        }
                        if (GWType[i] == MachineType.OCD3)
                        {
                            machineTypeName = "OCD3";
                            if(iShape == 2)
                            {
                                iShape = 3;
                            }
                        }
                       
                        filenames.Add(Application.StartupPath + "\\image\\" + $"{machineTypeName}" + "\\Shape\\150x150\\Shape" + iShape + ".png");
                    }
                    else
                    {
                        filenames.Add(Application.StartupPath + "\\image\\" + (iType == 1 ? "OIG" : "OCD") + "\\Shape\\150x150\\Shape" + iShape + ".png");
                    }       
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
            for (int i = 0; i < GwCount; i++)
            {
                string filename = filenames[i];
                if (File.Exists(filename))
                {
                    pic_EditGws[i].Image = Image.FromFile(filename);
                    pic_EditGws[i].Visible = true;
                    la_EditGws[i].Visible = true;
                }
                else
                {
                    pic_EditGws[i].Visible = false;
                    la_EditGws[i].Visible = false;
                }
            }

        }

        private void pic_GW_Click(object sender, EventArgs e)
        {  
            UpdateGwImage();//砂輪選擇 - 更新四個砂輪圖片
            TC_GW.SelectedTab = tab_Gw_GwSelect; //選擇砂輪  

            TC_Main.SelectedTab = tab_GwDb;//無論一顆或多顆 都要切到 砂輪 頁
            PrevPage.Push(tab_GwDb);
            btn_Prev.Visible = true;


            la_EditGw3.Visible = pic_EditGw3.Visible = GwCount == 3;
            la_EditGw4.Visible = pic_EditGw4.Visible = GwCount == 4;
            //GwSetEdit = false;



        }

        private void pic_DressGW_Click(object sender, EventArgs e) //修整對點
        {
            InitDressGwSetting();         
        }

        private void InitDressGwSetting(bool bDressGWClick = true)
        {
            int GwNo = 0;
            int GwType = 0;
            int DressMode = 0;

            double G55X = 0;
            double G55Z = 0;

            double G56X = 0;
            double G56Z = 0;

            double G57X = 0;
            double G57Z = 0;

            double G58X = 0;
            double G58Z = 0;

            double DressToolPenSetting = 0;

            bool bFinish = false;

            Actions.Enqueue(new Action(() =>
            {
                //目前砂輪號
                focas.ReadMacro(506, out double no);
                GwNo = (int)Math.Round(no);
                int shift = (GwNo - 1) * 200;

                if (GwNo < 1 || GwNo > 4)
                {
                    bFinish = true;
                    return;
                }
                //砂輪類型(0:外圓, 1:內圓(預留))
                focas.ReadMacro(10004 + shift, out double type);
                GwType = (int)Math.Round(type);
                if (GwType < 0 || GwType > 1) GwType = 0; //例外處理

                //修整模式(形狀)
                focas.ReadMacro(10005 + shift, out double mode);
                DressMode = (int)Math.Round(mode);

                //G55
                focas.ReadMacro(10102 + shift, out G55X);
                focas.ReadMacro(10103 + shift, out G55Z);

                //G56
                focas.ReadMacro(10104 + shift, out G56X);
                focas.ReadMacro(10105 + shift, out G56Z);

                //G57
                focas.ReadMacro(10106 + shift, out G57X);
                focas.ReadMacro(10107 + shift, out G57Z);

                //G58
                focas.ReadMacro(10108 + shift, out G58X);
                focas.ReadMacro(10109 + shift, out G58Z);

                // 修刀設定
                focas.ReadMacro(558, out DressToolPenSetting);

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

            if (GwNo < 1 || GwNo > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                return;//例外處理
            }

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //是否為對話式修砂對點
            bool DGW_Conv = ini.ReadBool("System", "DressGwConv", false);

            GwDressEdit = false;

            //標題顯示目前的砂輪號
            la_DressGwSettingTitle.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 139, "修砂對點 - 砂輪") + GwNo;
            //GwSelect(No);//通知PLC 搬座標系
            string dressToolImgPath = Application.StartupPath + "\\image\\";
            string tool3PFileName = dressToolImgPath + "DressTool3P.png";
            string tool2PFileName = dressToolImgPath + "DressTool2P.png";
            
            if (GwType == 1)
            {
                tool3PFileName = dressToolImgPath + "DressTool3P_R.png";
                tool2PFileName = dressToolImgPath + "DressTool2P_R.png";
            }
            pic_DressTool_3P.Image = File.Exists(tool3PFileName) ? Image.FromFile(tool3PFileName) : null;
            pic_DressTool_2P.Image = File.Exists(tool2PFileName) ? Image.FromFile(tool2PFileName) : null;

            if (DressToolPenSetting == 0)
            {
                //三支
                pa_DressTool_3P.BackColor = Color.Lime;
                pa_DressTool_2P.BackColor = Color.Gray;
            }
            else
            {
                //兩支
                pa_DressTool_3P.BackColor = Color.Gray;
                pa_DressTool_2P.BackColor = Color.Lime;
            }

            //對話式修砂對點流程
            if (DGW_Conv)
            {
                pa_DressTool.Parent = tab_DressGwConv;
                DressGwStep = 0;
                pic_DressGwStep.Image = null;
                pic_DressGwStep.Visible = false;
                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 21, "即將開啟砂輪，請注意安全。");
                btn_DG_Btn1.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 22, "中止");
                btn_DG_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 23, "繼續");
                if (bDressGWClick)
                {
                    TC_Main.SelectedTab = tab_DressGwConv;
                    PrevPage.Push(tab_DressGwConv);
                    btn_Prev.Visible = true;
                }

            }
            //直接修改座標系
            else
            {
                pa_DressTool.Parent = tab_DressGwSetting;
                //G54         G55         G56         G57         G58         G59
                //#5221,#5222,#5241,#5242,#5261,#5262,#5281,#5282,#5301,#5302,#5321,#5322
                TB_G55X.Text = G55X.ToString(Units.DisplayFmt);//G55 X
                TB_G55Z.Text = G55Z.ToString(Units.DisplayFmt);//G55 Z
                TB_G56X.Text = G57X.ToString(Units.DisplayFmt);//G56 X
                TB_G56Z.Text = G57Z.ToString(Units.DisplayFmt);//G56 Z
                TB_G58X.Text = G58X.ToString(Units.DisplayFmt);//G58 X
                TB_G58Z.Text = G58Z.ToString(Units.DisplayFmt);//G58 Z

                string path = Application.StartupPath + "\\image\\" + (GwType == 1 ? "OIG" : "OCD") + "\\DressGW\\";

                // 不要直接改從 P Code讀取的值
                int dressModeGwType = GwType;
                //修砂對點 座標系顯示
                // XmlElement xmlCoordinate = (GwType == 1 ? machineSetting.xmlOIG_Coordinate : machineSetting.xmlOCD_Coordinate);
                //XmlElement xmlCoordinate = machineSetting.GetGwTypeDressMode(GwType, DressMode);
                XmlElement xmlTools = machineSetting.GetGwTypeTools(dressModeGwType, DressMode);
                if (GWType[GwNo - 1] == MachineType.OCD2)
                {
                    xmlTools = machineSetting.GetGwTypeTools(2, DressMode);
                    path = Application.StartupPath + "\\image\\" + "OCD2" + "\\DressGW\\";
                    dressModeGwType = 2;
                }
                if (GWType[GwNo - 1] == MachineType.OCD3)
                {
                    xmlTools = machineSetting.GetGwTypeTools(3, DressMode);
                    path = Application.StartupPath + "\\image\\" + "OCD3" + "\\DressGW\\";
                    dressModeGwType = 3;
                }

                XmlElement xmlTool = xmlTools.GetChildNodeAt(0);
                if (DressToolPenSetting == 1)
                {
                    xmlTool = xmlTools.GetChildNodeAt(1);
                }
                XmlElement xmlG55 = xmlTool.GetChildNodeAt(0);
                XmlElement xmlG57 = xmlTool.GetChildNodeAt(1);
                XmlElement xmlG58 = xmlTool.GetChildNodeAt(2);
                //這邊只設為顯示, 能不能用要看形狀
                pic_G55.Visible = GB_G55.Visible = xmlG55 != null ? xmlG55.GetAttribute("Visible") == "1" : true; //砂輪外徑修整器
                pic_G56.Visible = GB_G56.Visible = xmlG57 != null ? xmlG57.GetAttribute("Visible") == "1" : true; //砂輪左側修整器
                pic_G58.Visible = GB_G58.Visible = xmlG58 != null ? xmlG58.GetAttribute("Visible") == "1" : false; //砂輪右側修整器

                string filename = path + xmlG55.GetAttribute("Image"); //修外徑專用                   
                pic_G55.Image = File.Exists(filename) ? Image.FromFile(filename) : null;

                filename = path + xmlG57.GetAttribute("Image"); //修砂輪左側專用
                pic_G56.Image = File.Exists(filename) ? Image.FromFile(filename) : null;

                filename = path + xmlG58.GetAttribute("Image"); //修砂輪右側專用(預留)
                pic_G58.Image = File.Exists(filename) ? Image.FromFile(filename) : null;

                //用 MachineSetting.xml 檢查此砂輪是不是有這個 修整模式(形狀)
                XmlElement xmlGw = machineSetting.GetGw(GwNo, dressModeGwType);
                if (xmlGw == null) return;
                bool bFind = false;//true:合法使用, false:非法使用
                for (int i = 0; i < xmlGw.ChildNodes.Count; i++)
                {
                    XmlElement xmlShape = (XmlElement)xmlGw.ChildNodes[i];
                    int.TryParse(xmlShape.GetAttribute("DressMode"), out int m);
                    if (m == 0) continue;//未設定
                    if (DressMode == m)
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 165, "砂輪資料錯誤"));
                    return;
                }

                //砂輪參數
                //XmlElement xmlGwParam = GwType == 0 ? machineSetting.xmlOIG_Param : machineSetting.xmlOCD_Param;

                //定義的砂輪形狀(修整模式)
                //XmlElement xmlShapeDef = xmlGwParam.GetShape(DressMode);
                //XmlElement xmlShapeDef = machineSetting.GetGwTypeShapeDef(GwType, DressMode);
                //if (xmlShapeDef == null)
                //{
                //    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 97, "檔案錯誤"));
                //    return;
                //}
                //int.TryParse(xmlShapeDef.GetAttribute("DressLeft"), out int DressLeft); //這個形狀要修左側
                //int.TryParse(xmlShapeDef.GetAttribute("DressRight"), out int DressRight); //這個形狀要修右側
                //GB_G56.Enabled = DressLeft == 1; //要修左側才要設定
                //GB_G58.Enabled = DressRight == 1; //要修右側才要設定
                if (bDressGWClick)
                {
                    TC_Main.SelectedTab = tab_DressGwSetting;

                    PrevPage.Push(tab_DressGwSetting);
                    btn_Prev.Visible = true;
                }
            }
        }
        private void pic_Parts_Click(object sender, EventArgs e) //加工對點(加工設定)
        {

            if (AxisNo.ContainsKey("B"))
            {
                int bIndex = AxisNo["B"];
                if (bIndex > 0 && Pos != null && Pos.Machine.Length > bIndex && Pos.Machine[bIndex] != 0)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 155, "B軸請先回到0度"), "");
                }
            }
            // 20260304 alan 非對話式加工取得砂輪編號路徑字串
            pic_G5459X.Tag = null;
            gb_DiamCalcKind.Visible = false;            

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");

            PictureBox pc = (PictureBox)sender;

            bool bFinish = false;
            int GwNo = 0;
            int GwType = 0;
            double G54X = 0;
            double G54Z = 0;
            double G59X = 0;
            double G59Z = 0;
            int DressMode= 0;

            Actions.Enqueue(new Action(() =>
            {
                //目前砂輪號
                focas.ReadMacro(506, out double no);
                GwNo = (int)Math.Round(no);
                if (GwNo < 1 || GwNo > 4)
                {
                    bFinish = true;
                    return;//例外處理
                }
                int shift = (GwNo - 1) * 200;

                //砂輪類型(0:內圓, 1:外圓(預留))
                focas.ReadMacro(10004 + shift, out double type);
                GwType = (int)Math.Round(type);
                if (GwType < 0 || GwType > 1) GwType = 0; //例外處理

                //修整模式(形狀)
                focas.ReadMacro(10005 + shift, out double mode);
                DressMode = (int)Math.Round(mode);
                //focas.ReadMacro(5221, out G54X);
                //focas.ReadMacro(5222, out G54Z);
                //focas.ReadMacro(5321, out G59X);
                //focas.ReadMacro(5322, out G59Z);
                focas.ReadMacro(10100 + shift, out G54X);
                focas.ReadMacro(10101 + shift, out G54Z);
                focas.ReadMacro(10110 + shift, out G59X);
                focas.ReadMacro(10111 + shift, out G59Z);

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

            if (GwNo < 1 || GwNo > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                return;//例外處理
            }

            GwWorkPiEdit = false;

            //對話式 的 加工對點
            bool DWP_Conv = ini.ReadBool("System", "DressWorkpieceConv", false);

            //顯示目前 砂輪號 在標籤
            la_WorkSettingTitle.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 142, "加工對點 - 砂輪") + GwNo;

            //GwSelect(1);//加工對點 - 通知PLC 搬座標系 多顆砂輪的 R系列 取消

            //G54         G55         G57         G58         G59
            //#5221,#5222,#5241,#5242,#5281,#5282,#5301,#5302,#5321,#5322
            la_G54XValue.Text = la_CV_G54XValue.Text = G54X.ToString(Units.DisplayFmt);
            la_G54ZValue.Text = la_CV_G54ZValue.Text = G54Z.ToString(Units.DisplayFmt);//Z                
            la_G59XValue.Text = la_CV_G59XValue.Text = G59X.ToString(Units.DisplayFmt);
            la_G59ZValue.Text = la_CV_G59ZValue.Text = G59Z.ToString(Units.DisplayFmt);//Z

            if (DWP_Conv) //對話式加工對點
            {
                DressPartsStep = 0;
                pic_DressPartsStep.Image = null;
                pic_DressPartsStep.Visible = false;
                la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 24, "請開啟砂輪、主軸，請注意安全。");
                btn_DP_Btn1.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 22, "中止");
                btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 23, "繼續");
                btn_DP_Btn3.Visible = false;
                tb_DP_Field1.Visible = false;
                if(GwType == 0 && GWType[GwNo - 1] == MachineType.OCD)
                {
                    gb_DP_G59Pos.Visible = true;
                }
                else
                {
                    gb_DP_G59Pos.Visible = false;
                }
                TC_Main.SelectedTab = tab_DressPartsConv;
                PrevPage.Push(tab_DressPartsConv);
                btn_Prev.Visible = true;
            }
            else //一般加工對點
            {
                //加工對點 座標系顯示
                string path = Application.StartupPath + "\\image\\" + (GwType == 1 ? "OIG" : "OCD") + "\\DressWorkpiece\\";
                //XmlElement xmlCoordinate = (GwType == 0 ? machineSetting.xmlOIG_Coordinate : machineSetting.xmlOCD_Coordinate);
                XmlElement xmlCoordinate = machineSetting.GetGwTypeCoordinate(GwType);
                if (GWType[GwNo - 1] == MachineType.OCD2)
                {
                    xmlCoordinate = machineSetting.GetGwTypeCoordinate(2);
                    path = Application.StartupPath + "\\image\\" + "OCD2" + "\\DressWorkpiece\\";
                    GwType = 2;
                }
                if (GWType[GwNo - 1] == MachineType.OCD3)
                {
                    xmlCoordinate = machineSetting.GetGwTypeCoordinate(3);
                    path = Application.StartupPath + "\\image\\" + "OCD3" + "\\DressWorkpiece\\";
                    GwType = 3;
                }
                XmlElement xmlG54G59X = xmlCoordinate.GetChildNodeAt(0);
                XmlElement xmlG54Z = xmlCoordinate.GetChildNodeAt(1);
                XmlElement xmlG59Z = xmlCoordinate.GetChildNodeAt(2);

                //這邊只設為顯示, 能不能用要看形狀
                pa_G54Z.Visible = xmlG54Z != null ? xmlG54Z.GetAttribute("Visible") == "1" : true; //砂輪左側研磨工件右端面
                pa_G59Z.Visible = xmlG59Z != null ? xmlG59Z.GetAttribute("Visible") == "1" : false; //砂輪右側研磨工件左端面


                TC_Main.SelectedTab = tab_DressPartsSetting;
                PrevPage.Push(tab_DressPartsSetting);
                btn_Prev.Visible = true;

                TB_G54G59X.Text = la_G54XValue.Text;
                TB_G54Z.Text = la_G54ZValue.Text;
                TB_G59Z.Text = la_G59ZValue.Text;
                TB_G54G59Cal_Diam.Text = "0";
                TB_G54Cal_Length.Text = "0";
                TB_G59Cal_Length.Text = "0";


                
                String filename;
                filename = path + xmlG54G59X.GetAttribute("Image"); //研磨工件外徑
                pic_G5459X.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                pic_G5459X.Tag = GwType.ToString();

                filename = path + xmlG54Z.GetAttribute("Image"); //研磨工件左端面
                pic_G54Z.Image = File.Exists(filename) ? Image.FromFile(filename) : null;

                filename = path + xmlG59Z.GetAttribute("Image"); //研磨工件右端面
                pic_G59Z.Image = File.Exists(filename) ? Image.FromFile(filename) : null;

                if (GwType == 0)
                {
                    //gb_DiamCalcKind.Visible = true;
                    //btn_DiamIn.PerformClick();
                }
                gb_DiamCalcKind.Visible = false;
            }
        }

        private int WriteGwMacro(int GwNo, int MacroNo, double val)
        {
            int shift = (GwNo - 1) * 200;
            bool bFinish = false;
            int ret = -1;
            Actions.Enqueue(new Action(() =>
            {

                if (MacroNo == 10006 || MacroNo == 10009)
                {
                    //更新 T Code 只能傳整數, 控制器內會自己轉公英制
                    int TCode_Z = (int)Math.Round((CurrentGwMacro[10009 + shift] + CurrentGwMacro[10006 + shift]) / (bInchTrans ? 0.00001 : 0.0001));
                    focas.WriteGeom(GeomType.Z, 124 + GwNo, TCode_Z);
                }
                else if (MacroNo == 10011)
                {
                    int TCode_X = (int)Math.Round(CurrentGwMacro[10011 + shift] / (bInchTrans ? -0.00001 : -0.0001));
                    focas.WriteGeom(GeomType.X, 124 + GwNo, TCode_X);
                }


                ret = focas.WriteMacro(MacroNo + shift, val);
                bFinish = true;
            }));
            int iStart = Environment.TickCount;
            while (!bFinish)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 5000)
                {

                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                    return -1;
                }
                Application.DoEvents();
            }
            return ret;
        }

        private void pic_DressToRight_Click(object sender, EventArgs e)
        {
            pa_DressToRight.BackColor = Color.Lime;
            pa_DressToLeft.BackColor = Color.Transparent;
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            CurrentGwMacro[10019 + GwMarcoOffset] = 0;//啟始位置 0:左側(往砂輪右側修整)
            WriteGwMacro(CurrentEditGwNo, 10019, 0);//啟始位置 0:左側(往砂輪右側修整)

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void pic_DressToLeft_Click(object sender, EventArgs e)
        {
            pa_DressToLeft.BackColor = Color.Lime;
            pa_DressToRight.BackColor = Color.Transparent;

            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            CurrentGwMacro[10019 + GwMarcoOffset] = 1;//啟始位置 1:右側(往砂輪左側修整)
            WriteGwMacro(CurrentEditGwNo, 10019, 1);//啟始位置 1:右側(往砂輪左側修整)

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        //private void DGV_GwParam_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (DGV_GwParam.CurrentRow == null)
        //    {
        //        Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 98, "Error, No Select Item."));
        //        return;
        //    }

        //    String path = DGV_GwParam.CurrentRow.Cells[Col_GwPicLink.Index].Value.ToString();
        //    if (File.Exists(path)) pic_GW_Param.BackgroundImage = new Bitmap(path);

        //    if (DGV_GwParam.CurrentCell == null) return;
        //    if (DGV_GwParam.CurrentCell.ColumnIndex != Col_GP_Value.Index) return;

        //    Fo_Num form = new Fo_Num();
        //    DialogResult ret = form.ShowDialog();
        //    if (ret == DialogResult.OK)
        //    {
        //        double data = form.TmpVal;
        //        DGV_GwParam.CurrentRow.Cells[Col_GP_Value.Index].Value = data.ToString(Units.DisplayFmt);
        //    }
        //}


        private void btn_G55X_Click(object sender, EventArgs e)
        {
            double.TryParse(la_DressMachAxis1Value.Text, out double G55XPos);
            TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G55X.Text = G55XPos.ToString(Units.DisplayFmt);
            GwDressEdit = true;
        }

        private void btn_G55Z_Click(object sender, EventArgs e)
        {
            double.TryParse(la_DressMachAxis2Value.Text, out double G55ZPos);
            TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G55Z.Text = G55ZPos.ToString(Units.DisplayFmt);
            GwDressEdit = true;

        }

        private void btn_G56X_Click(object sender, EventArgs e)
        {
            double.TryParse(la_DressMachAxis1Value.Text, out double G56XPos);
            TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G56X.Text = G56XPos.ToString(Units.DisplayFmt);
            GwDressEdit = true;
        }

        private void btn_G56Z_Click(object sender, EventArgs e)
        {
            double.TryParse(la_DressMachAxis2Value.Text, out double G56ZPos);
            TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G56Z.Text = G56ZPos.ToString(Units.DisplayFmt);
            GwDressEdit = true;
        }

        private void btn_G58X_Click(object sender, EventArgs e)
        {
            double.TryParse(la_DressMachAxis1Value.Text, out double G58XPos);
            TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G58X.Text = G58XPos.ToString(Units.DisplayFmt);
            GwDressEdit = true;
        }

        private void btn_G58Z_Click(object sender, EventArgs e)
        {
            double.TryParse(la_DressMachAxis2Value.Text, out double G58ZPos);
            TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G58Z.Text = G58ZPos.ToString(Units.DisplayFmt);
            GwDressEdit = true;
        }

        private void SaveDressGw() //修砂對點 - 儲存
        {
            double.TryParse(TB_G55X.Text, out double G55XPos);
            double.TryParse(TB_G55Z.Text, out double G55ZPos);
            double.TryParse(TB_G56X.Text, out double G56XPos);
            double.TryParse(TB_G56Z.Text, out double G56ZPos);
           
            int GwNo = 0;
            bool bFinish = false;

            Actions.Enqueue(new Action(() =>
            {
                //目前砂輪號
                focas.ReadMacro(506, out double no);
                GwNo = (int)Math.Round(no);
                if (GwNo < 1 || GwNo > 4)
                {
                    bFinish = true;
                    return;//例外處理
                }

                //ReadGwMacro(GwNo);//??為什麼要讀取砂輪資料??

                int shift = (GwNo - 1) * 200;

                //G54         G55         G56         G57         G58         G59
                //#5221,#5222,#5241,#5242,#5261,#5262,#5281,#5282,#5301,#5302,#5321,#5322

                //G55座標系 修砂對點 修外徑用
                //X軸
                focas.WriteMacro(5241, G55XPos);//寫到 座標系
                focas.WriteMacro(10102 + shift, G55XPos);//寫到 砂輪資料

                //Z軸                
                focas.WriteMacro(5242, G55ZPos);//寫到 座標系
                focas.WriteMacro(10103 + shift, G55ZPos);//寫到 砂輪資料


                //G57座標系 修砂對點 修左側+外徑用
                //X軸                
                focas.WriteMacro(5281, G56XPos);//寫到 座標系
                focas.WriteMacro(10106 + shift, G56XPos);//寫到 砂輪資料

                //Z軸
                focas.WriteMacro(5282, G56ZPos);//寫到 座標系
                focas.WriteMacro(10107 + shift, G56ZPos);//寫到 砂輪資料



                //標準不使用右側,
                if (Rightopen)//右側啟用
                {
                    if (double.TryParse(TB_G58X.Text, out double G58XPos))
                    {
                        focas.WriteMacro(5301, G58XPos);//寫到 座標系
                        focas.WriteMacro(10108 + shift, G58XPos);//寫到 砂輪資料
                    }
                    if (double.TryParse(TB_G58Z.Text, out double G58ZPos))
                    {
                        focas.WriteMacro(5302, G58ZPos);//寫到 座標系
                        focas.WriteMacro(10109 + shift, G58ZPos);//寫到 砂輪資料
                    }
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

            if (GwNo < 1 || GwNo > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                return;//例外處理
            }

            GwDressEdit = false;
        }

        private void btn_SaveDressGw_Click(object sender, EventArgs e)
        {
            SaveDressGw();
            btn_Prev.PerformClick();
        }

        private void btn_G54G59X_Click(object sender, EventArgs e)
        {
            double.TryParse(la_PartsMachAxis1Value.Text, out double PosX);
            //TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G54G59X.Text = PosX.ToString(Units.DisplayFmt);
            GwWorkPiEdit = true;
        }

        private void btn_G54Z_Click(object sender, EventArgs e)
        {
            double.TryParse(la_PartsMachAxis2Value.Text, out double PosZ);
            //TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G54Z.Text = PosZ.ToString(Units.DisplayFmt);
            GwWorkPiEdit = true;
        }

        private void btn_G59Z_Click(object sender, EventArgs e)
        {
            double.TryParse(la_PartsMachAxis2Value.Text, out double PosZ);
            //TIniFile sys = new TIniFile(Application.StartupPath + "\\sys.ini");
            TB_G59Z.Text = PosZ.ToString(Units.DisplayFmt);
            GwWorkPiEdit = true;
        }

        private void SaveGrinding() //加工對點 - 儲存
        {
            double.TryParse(TB_G54G59X.Text, out double PosX);
            double.TryParse(TB_G54G59Cal_Diam.Text, out double Diam);
            double.TryParse(TB_G54Z.Text, out double LPosZ);
            double.TryParse(TB_G59Z.Text, out double RPosZ);
            double.TryParse(TB_G54Cal_Length.Text, out double LLength);
            double.TryParse(TB_G59Cal_Length.Text, out double RLength);



            bool bFinish = false;
            int GwNo = 0;
            int GwType = 0;
            Actions.Enqueue(new Action(() =>
            {
                focas.ReadMacro(506, out double no);//砂輪號
                GwNo = (int)Math.Round(no);
                if (GwNo < 1 || GwNo > 4)
                {
                    bFinish = true;
                    return;
                }

                int shift = (GwNo - 1) * 200;//用 砂輪號 偏移區塊

                
                //砂輪類型(0:內圓, 1:外圓(預留))
                focas.ReadMacro(10004 + shift, out double type);
                GwType = (int)Math.Round(type);
                if (GwType < 0 || GwType > 1) GwType = 0; //例外處理

                double val;

                //focas.Param_ReadDouble(8210, 0, out double PRM8210);
                //double rad = PRM8210 * (Math.PI / 180);

                //G54          G55          G57          G58          G59
                //#5221,#5222, #5241,#5242, #5281,#5282, #5301,#5302, #5321,#5322
                val = PosX - Diam;
                //if (btn_CurrentRotationCenterClicked.Tag != null && btn_CurrentRotationCenterClicked.Tag.ToString() == "out")
                if (GwType == 0)
                {
                    val = PosX + Diam;
                }
                focas.WriteMacro(5221, val); //控制器用的 G54X
                focas.WriteMacro(10100 + shift, val); //加工程式用的 G54X

                val = LPosZ - LLength;
                focas.WriteMacro(5222, val); //控制器用的 G54Z
                focas.WriteMacro(10101 + shift, val); //加工程式用的 G54Z

                val = PosX - Diam;
                focas.WriteMacro(5321, val); //控制器用的 G59X
                focas.WriteMacro(10110 + shift, val); //加工程式用的 G59X

                val = RPosZ - RLength;
                focas.WriteMacro(5322, val); //控制器用的 G59Z
                focas.WriteMacro(10111 + shift, val); //加工程式用的 G59Z



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

            if (GwNo < 1 || GwNo > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                return;
            }
            
            la_G54XValue.Text = (PosX - Diam).ToString(Units.DisplayFmt);
            //if (btn_CurrentRotationCenterClicked.Tag != null && btn_CurrentRotationCenterClicked.Tag.ToString() == "out")
            if(GwType == 0)
            {
                la_G54XValue.Text = (PosX + Diam).ToString(Units.DisplayFmt);
            }
            la_G54ZValue.Text = (LPosZ - LLength).ToString(Units.DisplayFmt);
            la_G59XValue.Text = (PosX - Diam).ToString(Units.DisplayFmt);
            //if (btn_CurrentRotationCenterClicked.Tag != null && btn_CurrentRotationCenterClicked.Tag.ToString() == "out")
            if (GwType == 0)
            {
                la_G59XValue.Text = (PosX + Diam).ToString(Units.DisplayFmt);
            }
            la_G59ZValue.Text = (RPosZ - RLength).ToString(Units.DisplayFmt);

            TB_G54G59X.Text = la_G54XValue.Text;
            TB_G54Z.Text = la_G54ZValue.Text;
            TB_G59Z.Text = la_G59ZValue.Text;

            TB_G54G59Cal_Diam.Text = "0"; //輔助計算的清掉
            TB_G54Cal_Length.Text = "0"; //輔助計算的清掉
            TB_G59Cal_Length.Text = "0"; //輔助計算的清掉
            GwWorkPiEdit = false;
        }

        private void btn_SaveGrindCoor_Click(object sender, EventArgs e)
        {
            SaveGrinding();
            btn_Prev.PerformClick();
        }


        private void btn_DressGw_Click(object sender, EventArgs e)
        {
            if (bRun)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 148, "程式仍在執行"), "");
                return;
            }

            Fo_ConfirnDressGw form = new Fo_ConfirnDressGw();
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            ThrDressGw = new Thread(() =>
            {
                bool bFinish = false;
                Actions.Enqueue(new Action(() =>
                {
                    //選擇程式6(R系列修砂程式)           
                    focas.WriteMacro(980, 6);//畫面要啟動的程式：修砂=6
                    OneKeyCall(8999);//一鍵呼叫
                    bFinish = true;
                }));
                int iStart = Environment.TickCount;
                while (!bFinish)
                {
                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 5000)
                    {
                        //this.Invoke(new Action(() => { 
                            //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                        //}));
                        ThrDressGw = null;
                        return;
                    }
                    Thread.Sleep(50);
                }

                Thread.Sleep(3000); //等待開始

                while (bRun) Thread.Sleep(100);//等待程式結束

                //修砂完回到O8000
                Actions.Enqueue(new Action(() =>
                {
                    focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
                }));
                ThrDressGw = null;
            });
            ThrDressGw.Start();
        }

        private void btn_Redo_Click(object sender, EventArgs e)
        {
            TC_Main.SelectedTab = tab_Redo;
            ShowRedoProgram();
            pa_SoftPanel.Visible = false;
            PrevPage.Push(tab_Redo);
            btn_Prev.Visible = true;
        }

        private void btn_Offset_Click(object sender, EventArgs e)
        {
            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                ReadTCode(CurrentProgram);//進入補正畫面時讀取一次
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

            pa_SoftPanel.Visible = false;
            TC_Main.SelectedTab = tab_Offset;
            ShowOffsetProgram();

            PrevPage.Push(tab_Offset);
            btn_Prev.Visible = true;
        }

        //進入重修頁面，固定關閉所有工序的重修
        public void ShowRedoProgram()
        {
            btn_Redo_P0001.Enabled = false;
            btn_Redo_P001.Enabled = false;
            btn_Redo_P01.Enabled = false;
            btn_Redo_N0001.Enabled = false;
            btn_Redo_N001.Enabled = false;
            btn_Redo_N01.Enabled = false;
            btn_Redo_Input.Enabled = false;
            btn_Redo_Input2.Enabled = false;

            //例外處理
            if (CurrentProgram == null)
                return;

            //刷新重修補正資料
            DGV_Redo.Rows.Clear();
            //有多少工序就顯示多少
            int process_count = CurrentProgram.Processes.Count;
            for (int i = 0; i < process_count; i++)
            {
                //例外處理
                TProcess process = CurrentProgram.Processes[i];
                if (process == null)
                    continue;

                //讀取重修補正值
                double OfsX = process.OffsetX.RedoValue;
                double OfsZ = process.OffsetZ.RedoValue;
                double OfsY = process.OffsetY.RedoValue;
                double PosX = 0;
                double PosZ = 0;

                //讀取加工位置
                if (process.SubPrograms.Count > 0)
                {
                    TArgument a;
                    a = process.SubPrograms[0].GetArgument("19810");
                    if (a != null)
                        PosX = a.Value;

                    a = process.SubPrograms[0].GetArgument("19812");
                    if (a != null)
                        PosZ = a.Value;
                }

                //顯示開關
                Bitmap bmp = Properties.Resources.BtnOff;
                //進到這頁固定關閉
                RedoEnabled[i] = false;


                Bitmap bmpNo = new Bitmap(Col_R_No.Width, 30);
                Bitmap icon = (Bitmap)Bitmap.FromFile(Application.StartupPath + "\\image\\Process\\40x40\\Process" + ((int)process.ID).ToString("00") + ".bmp");
                Graphics g = Graphics.FromImage(bmpNo);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                int id = (int)process.ID - 1;
                if (id >= 0)
                {
                    g.DrawImage(icon, 0, 0, 35, 35);
                    g.DrawString((i + 1).ToString(), new Font("Times New Roman", 12F), new SolidBrush(Color.Black), 39, 8);
                }
                g.Dispose();



                Bitmap bmpName = new Bitmap(Col_R_Name.Width, 30);
                Graphics gName = Graphics.FromImage(bmpName);
                gName.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //gName.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (id >= 0)
                {
                    gName.DrawString(process.Name, new Font("Times New Roman", 12F, FontStyle.Regular), new SolidBrush(Color.Black), 4, 0);
                    gName.DrawString(process.Memo, new Font("Times New Roman", 10F), new SolidBrush(Color.Gray), 4, 16);
                }
                gName.Dispose();


                DGV_Redo.Rows.Add(bmpNo, bmpName, false, PosX.ToString(Units.DisplayFmt), PosZ.ToString(Units.DisplayFmt), OfsX.ToString(Units.DisplayFmt), OfsZ.ToString(Units.DisplayFmt), bmp);//(工序,加工模式)                
            }

            la_R_Name.Text = CurrentProgram.Name;
        }

        public void ShowOffsetProgram()
        {
            /*---------------------------------------------
            在編輯程式頁，顯示目前呼叫的程式內容(工序)
            ---------------------------------------------*/



            //例外處理
            if (CurrentProgram == null)
                return;

            btn_Ofs_P01.Enabled = false;
            btn_Ofs_P001.Enabled = false;
            btn_Ofs_P0001.Enabled = false;
            btn_Ofs_N01.Enabled = false;
            btn_Ofs_N001.Enabled = false;
            btn_Ofs_N0001.Enabled = false;
            btn_Offset_Input.Enabled = false;

            DGV_Offset.Rows.Clear();

            int process_count = CurrentProgram.Processes.Count;
            for (int i = 0; i < process_count; i++)
            {
                TProcess process = CurrentProgram.Processes[i];
                if (process == null)
                    continue;
                double OfsX = process.OffsetX.Value;
                double OfsZ = process.OffsetZ.Value;
                double PosX = 0;
                double PosZ = 0;

                String MeasOffset = "";
                String MeasFunc = "";
                TArgument a;
                if (process.SubPrograms.Count > 0)
                {
                    a = process.SubPrograms[0].GetArgument("19810");
                    if (a != null)
                        PosX = a.Value;

                    a = process.SubPrograms[0].GetArgument("19812");
                    if (a != null)
                        PosZ = a.Value;

                    //量測功能 
                    a = process.SubPrograms[0].GetArgument("19863");
                    if (a != null)
                    {
                        if (a.Value == 1)
                        {
                            MeasOffset = "0.000";

                            //量測組別
                            a = process.SubPrograms[0].GetArgument("19865");
                            if (a != null)
                            {
                                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                                //組別1
                                if (a.Value == 1)
                                {
                                    MeasFunc = "Group1";
                                    MeasOffset = ini.ReadString("Measure", "Group1_Offset", "0.000");
                                }
                                //組別2
                                if (a.Value == 2)
                                {
                                    MeasFunc = "Group2";
                                    MeasOffset = ini.ReadString("Measure", "Group2_Offset", "0.000");
                                }
                            }
                        }
                    }
                }
                else if (process.ID == 201)//端測
                {
                    a = process.SubPrograms[0].GetArgument("19903");
                    if (a != null)
                        PosX = a.Value;

                    a = process.SubPrograms[0].GetArgument("19904");
                    if (a != null)
                        PosZ = a.Value;
                }

                Bitmap bmpNo = new Bitmap(Col_OffsetNo.Width, 30);
                Bitmap icon = (Bitmap)Bitmap.FromFile(Application.StartupPath + "\\image\\Process\\40x40\\Process" + ((int)process.ID).ToString("00") + ".bmp");
                Graphics g = Graphics.FromImage(bmpNo);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                int id = (int)process.ID - 1;
                if (id >= 0)
                {
                    g.DrawImage(icon, 0, 0, 30, 30);
                    g.DrawString((i + 1).ToString(), new Font("Times New Roman", 12F), new SolidBrush(Color.Black), 30, 8);
                }
                g.Dispose();


                Bitmap bmpName = new Bitmap(Col_OffsetName.Width, 30);
                Graphics gName = Graphics.FromImage(bmpName);
                gName.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //gName.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (id >= 0)
                {
                    gName.DrawString(process.Name, new Font("Times New Roman", 12F, FontStyle.Regular), new SolidBrush(Color.Black), 4, 0);
                    gName.DrawString(process.Memo, new Font("Times New Roman", 10F), new SolidBrush(Color.Gray), 4, 16);

                    gName.Dispose();
                }

                DGV_Offset.Rows.Add(bmpNo,
                                        bmpName,
                                        false,
                                        PosX.ToString(Units.DisplayFmt),
                                        PosZ.ToString(Units.DisplayFmt),
                                        OfsX.ToString(Units.DisplayFmt),
                                        OfsZ.ToString(Units.DisplayFmt),
                                        MeasOffset,
                                        MeasFunc, //Group1, Group2
                                        process.ID);//(工序,加工模式)                
            }


            //la_O_No.Text = CurrentProgram.ID.ToString();
            la_O_Name.Text = CurrentProgram.Name;

        }

        private String CreateProgName()
        {
            //先輸入程式檔名
            String ProgName;
            //直到輸入成功或取消
            Fo_AddProgram form = new Fo_AddProgram();
            while (true)
            {
                //跳出輸入欄位

                DialogResult ret = form.ShowDialog();
                if (ret != DialogResult.OK)
                    return "";

                //名稱
                if (form.TB_ProgName.Text == "")
                {
                    form = new Fo_AddProgram();
                    form.la_Message.Text = "Name can't be empty.";
                    form.la_Message.Visible = true;
                    continue;
                }

                //檢查程式是否重覆
                if (Units.ProgramDB.ProgNameExists(form.TB_ProgName.Text))
                {
                    form = new Fo_AddProgram();
                    form.la_Message.Text = "Program name exists.";
                    form.la_Message.Visible = true;
                    continue;
                }
                ProgName = form.TB_ProgName.Text;
                break;
            }

            return ProgName;
        }


        private void btn_Prog_Add_Click(object sender, EventArgs e)
        {
            String ProgName = CreateProgName();

            //例外處理
            if (ProgName == "")
                return;

            //自動計算程式ID(唯一)
            int no = 1;
            while (true)
            {
                if (!Units.ProgramDB.ProgIDExists(no))
                    break;
                no++;
            }

            //新增空白程式
            TempProgram = new TProgram();
            TempProgram.Name = ProgName;
            TempProgram.ID = no;
            //Units.ProgramDB.Programs.Add(TempProgram);

            //TempProgram = CurrentProgram;
            //TempProgram.Name = ProgName;
            //TempProgram.ID = no;
            //Units.ProgramDB.Programs.Add(TempProgram);

            //Units.ProgramDB.Save();

            btn_InsertProcFront.Enabled = false;
            btn_InsertProcBack.Enabled = false;
            btn_EditProc.Enabled = false;
            btn_RemoveProc.Enabled = false;
            btn_Copy.Enabled = false;
            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
            la_ProcList_Prog_Name.Text = ProgName;

            SetProcessData(null);
            DGV_ProcList.Rows.Clear();
            ProcessIndex = -1;
            TempProcess = null;

            TC_Main.SelectedTab = tab_ProcList; //工序清單
            PrevPage.Clear();
            PrevPage.Push(tab_ProcList);
            btn_Prev.Visible = false;

            ShowProgListFromDB();
        }


        //顯示工序內容(預設切到參數1)
        public void SetProcessData(TProcess process)
        {
            //focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);
            DGV_Param1.Rows.Clear();
            DGV_Param2.Rows.Clear();
            DGV_Param3.Rows.Clear();
            DGV_Advance.Rows.Clear();
            DGV_Dress1.Rows.Clear();
            DGV_Dress2.Rows.Clear();


            //例外處理
            if (process == null) return;

            TC_EditProc.Visible = true;
            uc_UserNumEditProc.Visible = process.ID != 999;

            pa_GM_Code.Visible = false;
            #region 頁面切換 
            if (process.ID == 999)//GM Code
            {
                //string code = "";
                LB_GM_Code.Items.Clear();
                TB_GM_Code.Clear();

                //LB_GM_Code.Text = code;
                TB_Input.Clear();
                pa_GM_Code.Visible = true;
                pa_GM_Code.Parent = tab_EditProc;
                pa_EditAbsAxisProbe.Visible = TC_EditProc.SelectedTab == tab_Probe;
                pa_ManualDistProbe.Visible = TC_EditProc.SelectedTab == tab_Probe;
                pa_GM_Code.BringToFront(); // 確保 pa_GM_Code 在最上層
                pa_GM_Code.Location = new Point(0, 48); // 設定在 tab_EditProc 內的位置

                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                int GMCodeMode = ini.ReadInteger("System", "GMCodeMode", 0);

                TB_GM_Code.Location = new Point(8, 32);//設定初始位置
                if (GMCodeMode == 0)
                {
                    TB_Input.Visible = LB_GM_Code.Visible = true;//預設顯示listbox
                    btn_keyboard.Visible = TB_GM_Code.Visible = false;
                    btn_InsertLine.Visible = btn_MoveUp.Visible = btn_AddLine.Visible = btn_ClearLine.Visible = btn_MoveDown.Visible = true;
                    foreach (String s in process.GM_Code)
                    {
                        LB_GM_Code.Items.Add(s);
                    }
                }
                else
                {
                    TB_Input.Visible = LB_GM_Code.Visible = false;//預設顯示listbox
                    btn_keyboard.Visible = TB_GM_Code.Visible = true;
                    btn_InsertLine.Visible = btn_MoveUp.Visible = btn_AddLine.Visible = btn_ClearLine.Visible = btn_MoveDown.Visible = false;
                    foreach (String s in process.GM_Code)
                    {
                        TB_GM_Code.AppendText(s + "\r\n");
                    }
                }
                btn_keyboard.Parent = btn_changeenter.Parent = btn_AddLine.Parent = btn_MoveUp.Parent = btn_InsertLine.Parent
                    = btn_MoveDown.Parent = btn_ClearLine.Parent = btn_ClearAllLine.Parent = TB_Input.Parent = pa_GM_Code;//將按鈕移至外面

                btn_AddLine.Location = new Point(512, 64);
                btn_MoveUp.Location = new Point(512, 136);
                btn_InsertLine.Location = new Point(584, 64);
                btn_MoveDown.Location = new Point(584, 136);
                btn_ClearLine.Location = new Point(656, 64);
                btn_ClearAllLine.Location = new Point(656, 136);
                btn_changeenter.Location = new Point(656, 208);
                btn_keyboard.Location = new Point(512, 208);
                TB_Input.Location = new Point(488, 24);

                //TC_EditProc.SelectedTab = tab_Code;
                pic_Descript.Visible = false;
                btn_ArgParam.Visible = false;
                btn_ArgParam2.Visible = false;
                btn_ArgParam3.Visible = false;
                btn_ArgAdvance.Visible = false;
                btn_ArgDress1.Visible = false;
                btn_ArgDress2.Visible = false;
                return;
            }
            else if (process.ID == 201)//端測
            {
                TC_EditProc.SelectedTab = tab_Probe;
                pa_EditAbsAxisProbe.Visible = TC_EditProc.SelectedTab == tab_Probe;
                pa_ManualDistProbe.Visible = TC_EditProc.SelectedTab == tab_Probe;
                pa_Probe.Parent = tab_EditProc;
                pa_Probe.Location = new Point(8, 136);
                pa_EditAbsAxisProbe.Location = new Point(0, 48);
                pa_ManualDistProbe.Location = new Point(256, 48);
                TC_EditProc.Visible = false;

                panel8.Visible = false;
                btn_ArgParam.Visible = false;
                btn_ArgParam2.Visible = false;
                btn_ArgParam3.Visible = false;
                btn_ArgAdvance.Visible = false;
                btn_ArgDress1.Visible = false;
                btn_ArgDress2.Visible = false;

                uc_UserNumEditProc.la_Msg.Text = label11.Text;

                if (bInchTrans)
                {
                    la_ProbeRange.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 110, "(0.04~4，預設0.4)");
                }
                else
                {
                    la_ProbeRange.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 109, "(1~100，預設10)");
                }
                Fo_ProbeMsg form = new Fo_ProbeMsg();
                form.Show();

                foreach (TSubProgram s in process.SubPrograms)
                {
                    if (s.ProgNo == 9028)
                    {
                        TArgument a;
                        //a = s.GetArgument("19905");
                        //if (a != null)
                        //{
                        ////端測方向
                        //MeasureDir = (int)Math.Round(a.Value);
                        //focas.WriteMacro(19905, a.Value);
                        //}
                        a = s.GetArgument("19902");// 完成狀態590待確認
                        if (a != null)
                        {
                            if (a.Value == 0)
                            {
                                TB_Ready.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 8, "未設定");
                            }
                            else
                            {
                                TB_Ready.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 9, "設定完成");
                            }
                            TB_Ready.Tag = a;
                        }
                        a = s.GetArgument("19906");//端測距離
                        if (a != null)
                        {
                            TB_Dist.Text = a.Value.ToString(Units.DisplayFmt);
                            TB_Dist.Tag = a;
                            double dist = a.Value;
                            Actions.Enqueue(new Action(() =>
                            {
                                focas.WriteMacro(589, dist);
                            }));
                        }
                        a = s.GetArgument("19903");//X起始位置
                        if (a != null)
                        {
                            TB_X.Text = a.Value.ToString(Units.DisplayFmt);
                            TB_X.Tag = a;
                        }
                        a = s.GetArgument("19904");//Z起始位置
                        if (a != null)
                        {
                            TB_Z.Text = a.Value.ToString(Units.DisplayFmt);
                            TB_Z.Tag = a;
                        }
                        a = s.GetArgument("19907");//Master值
                        if (a != null)
                        {
                            TB_Master.Text = a.Value.ToString(Units.DisplayFmt);
                            TB_Master.Tag = a;
                        }
                        a = s.GetArgument("19908");//端完安全位置
                        if (a != null)
                        {
                            TB_PROBE_SafePos.Text = a.Value.ToString(Units.DisplayFmt);
                            TB_PROBE_SafePos.Tag = a;
                        }
                        TC_EditProc.SelectedTab = tab_Probe;
                    }
                }

                return;
            }
            else //研磨工序
            {
                panel8.Visible = true;
                btn_ArgParam.Visible = true;
                btn_ArgParam2.Visible = true;
                btn_ArgParam3.Visible = true;
                btn_ArgAdvance.Visible = true;
                btn_ArgDress1.Visible = true;
                btn_ArgDress2.Visible = true;

                //if ((TC_EditProc.SelectedTab != tab_Param1) &&
                //    (TC_EditProc.SelectedTab != tab_Advance) &&
                //    (TC_EditProc.SelectedTab != tab_Dress1) &&
                //    (TC_EditProc.SelectedTab != tab_Dress2))
                {
                    //預設顯示 參數1
                    TC_EditProc.SelectedTab = tab_Param1;
                }

                //外圓右端、外圓右斜、內圓
                //    if (process.ID == 3 ||
                //        process.ID == 7 ||
                //        process.ID == 101 ||
                //        process.ID == 102 ||
                //        process.ID == 103)//G54
                //    {
                //        //3,7
                //        bShowG59 = false;
                //    }
                //    else if (process.ID == 4 ||
                //             process.ID == 8)//G59
                //    {
                //        //4,8
                //        bShowG59 = true;
                //    }
                //    else //看左右端選擇
                //    {
                //        //1,2,5,6
                //        if (process.SubPrograms.Count > 0)
                //        {
                //            //目前選擇的工序是右原點
                //            TArgument a = process.SubPrograms[0].GetArgument("19816");//以前的O9001.Z
                //            if (a != null)
                //            {
                //                //顯示G59坐標系
                //                bShowG59 = a.Value == 1;
                //            }
                //        }
                //    }
            }
            #endregion 頁面切換
            pa_EditAbsAxisProbe.Visible = TC_EditProc.SelectedTab == tab_Probe;
            pa_ManualDistProbe.Visible = TC_EditProc.SelectedTab == tab_Probe;

            TSubProgram sp = null;//子程式
            if (process.SubPrograms.Count > 0) sp = process.SubPrograms[0]; //現在只剩下主要的巨集，輔助的O9001~O9003 已廢棄
            if (sp == null) return;//例外處理

            TProcess def_proc = null;//搜尋預設工序
            foreach (var p in Units.ProcessList)
            {
                if (p.ID == process.ID)
                {
                    def_proc = p;
                    break;
                }
            }
            if (def_proc == null) return;//例外處理

            //從語言檔產生下拉式選項清單
            //先搜尋第一筆符合ID的工序
            var processNode = Units.xmlDefaultProcessLang.Descendants("Process").FirstOrDefault(x => x.Attribute("ID")?.Value == process.ID.ToString());
            //再取得此工序中所有P CODE
            var pcodes = processNode.Elements("PCode").Select(x => new
            {
                PCodeNo = x.Attribute("No")?.Value,
                PCodeName = x.Attribute("Name")?.Value,
                Texts = x.Elements("Text").Select(text => new //如果此P CODE 有建立下拉式選項的文字(數值對照表)
                {
                    Value = text.Attribute("Value")?.Value,
                    Name = text.Attribute("Name")?.Value
                }).ToList()
            });

            //這些單位顯示整數格式
            string[] unit = { "times", "rpm", "steps", "sec" };

            //讀取所有P CODE(工序條件)            
            int arg_count = sp.Arguments.Count;
            for (int j = 0; j < arg_count; j++)
            {
                TArgument a = sp.Arguments[j];
                if (a == null) continue;//例外處理                

                String StrValue = "";
                if (unit.Contains(a.Unit))//顯示整數
                {
                    StrValue = a.Value.ToString("0");
                }
                else //浮點數值或文字
                {
                    StrValue = a.Value.ToString(Units.DisplayFmt);//顯示浮點數                                                       
                    foreach (var pcode in pcodes)//將 下拉式類型的變數 數值轉為對應的文字
                    {
                        if (pcode.Texts.Count == 0) continue;//數值
                        if (a.AddrCode != pcode.PCodeNo) continue;//不同P CODE
                        foreach (var vn in pcode.Texts)//每個數值比對
                        {
                            if (a.Value.ToString() == vn.Value)//找到數值
                            {
                                StrValue = vn.Name;//將此數值轉為文字
                                break;
                            }
                        }
                    }
                }

                if (a.AddrCode == "19865")//使用組別
                {
                    bOnload_DGV_Advance = true;
                    cb_MeasureGroup.Text = a.Value.ToString();
                    TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                    tb_MeasureRange.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Range", 0).ToString("0.0000");
                    tb_MeasureRoughPos.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Rough", 0).ToString("0.000");
                    tb_MeasureFinePos.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Fine", 0).ToString("0.000");
                    tb_MeasurePrecisionPos.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Precision", 0).ToString("0.000");
                    tb_MeasureSparkless.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Sparkless", 0).ToString("0.0000");
                    bOnload_DGV_Advance = false;
                }

                double.TryParse(a.Value.ToString(Units.DisplayFmt), out double DataValue);

                if (a.AddrCode == "19880")//砂輪號
                {
                    //只有一顆砂輪不用選砂輪號
                    if (GwCount <= 1)
                    {
                        a.Value = 1;
                        continue;
                    }

                    if (Gw2CantUse && Gw2CantUseID.Contains(process.ID))
                    {
                        a.Value = 1;
                        //continue;
                    }
                   
                }

                if (!Rolleropen && (a.AddrCode == "19933" || a.AddrCode == "19944"))
                    continue;//滾輪選配功能

                if (!Measopen && (a.AddrCode == "19863" || a.AddrCode == "19864" || a.AddrCode == "19865" || a.AddrCode == "19866" || a.AddrCode == "19867"))
                    continue;//量測選配功能

                if (!Rightopen && (a.AddrCode == "19928" || a.AddrCode == "19929" || a.AddrCode == "19939" || a.AddrCode == "19940"))
                    continue;//右側修整選配功能

                if (!GapOpen && (a.AddrCode == "19859" || a.AddrCode == "19860" || a.AddrCode == "19861" || a.AddrCode == "19862"))
                    continue;//間隙消除

                //DataGridView 欄位應該遵守順序 : 參數名稱, 代碼, 顯示文字或數值, 單位, PCode, 浮點數值
                if (a.Type == 0) DGV_Param1.Rows.Add(a.Name, a.Code, StrValue, a.Unit, a, a.Value);
                else if (a.Type == 1) DGV_Param2.Rows.Add(a.Name, a.Code, StrValue, a.Unit, a, a.Value);
                else if (a.Type == 2) DGV_Param3.Rows.Add(a.Name, a.Code, StrValue, a.Unit, a, a.Value);
                else if (a.Type == 3) DGV_Advance.Rows.Add(a.Name, a.Code, StrValue, a.Unit, a, a.Value);
                else if (a.Type == ArgType.DressTiming1) DGV_Dress1.Rows.Add(a.Name, a.Code, StrValue, a.Unit, a, a.Value);
                else if (a.Type == ArgType.DressTiming2) DGV_Dress2.Rows.Add(a.Name, a.Code, StrValue, a.Unit, a, a.Value);
            }//將所有P CODE 已經填入 DataGrindView

            for (int i = 0; i < DGV_Param1.Rows.Count; i++) DGV_Param1.Rows[i].Height = 34;
            for (int i = 0; i < DGV_Param2.Rows.Count; i++) DGV_Param2.Rows[i].Height = 34;
            for (int i = 0; i < DGV_Param3.Rows.Count; i++) DGV_Param3.Rows[i].Height = 34;
            for (int i = 0; i < DGV_Advance.Rows.Count; i++) DGV_Advance.Rows[i].Height = 34;
            for (int i = 0; i < DGV_Dress1.Rows.Count; i++) DGV_Dress1.Rows[i].Height = 34;
            for (int i = 0; i < DGV_Dress2.Rows.Count; i++) DGV_Dress2.Rows[i].Height = 34;

            if (TC_EditProc.SelectedTab == tab_Param1)
            {
                if (DGV_Param1.Rows.Count == 0) return;
                DGV_Param1.CurrentCell = DGV_Param1.Rows[0].Cells[0];
                DGV_CellClick(DGV_Param1, null);//
            }
            //if (process.ID != 201) //不是端測的工序(研磨工序)
            //{
            //    TC_EditProc.SelectedTab = tab_Param1;//預設切到 參數1

            //    if (DGV_Param1.Rows.Count == 0) return;//例外處理

            //    //數字鍵盤顯示 第一個 要顯示參數名稱
            //    uc_UserNumEditProc.la_Msg.Text = DGV_Param1.Rows[0].Cells[Col_Param1_Name.Index].Value.ToString();

            //    //取得 第一個 要顯示參數
            //    TArgument a = DGV_Param1.Rows[0].Cells[Col_Param1_PCode.Index].Value as TArgument;
            //    if (a == null) return;


            //    if (def_proc.SubPrograms.Count == 0) return;

            //    //取得預設工序中的此P CODE
            //    TArgument def_a = null;
            //    foreach (var aa in def_proc.SubPrograms[0].Arguments)
            //    {
            //        if (aa.AddrCode == a.AddrCode)
            //        {
            //            def_a = aa;
            //            break;
            //        }
            //    }

            //    //有設定上限或下限
            //    if ((def_a.Min != 0 || def_a.Max != 0))
            //    {
            //        uc_UserNumEditProc.la_Msg.Text += "\r\n" + def_a.Min.ToString("0.#####") + " ~ " + def_a.Max.ToString("0.#####");
            //    }

            //    string procid = "ID" + process.ID;
            //    string picAddr = a.AddrCode + ".png"; //[PCode No].png
            //    string PicPath = Application.StartupPath + "\\image\\ProcImage\\" + procid + "\\" + picAddr;

            //    //如果之資料夾裡有此檔案就會顯示
            //    if (File.Exists(PicPath))
            //    {
            //        Bitmap img1 = (Bitmap)Bitmap.FromFile(PicPath);
            //        Bitmap bmp = new Bitmap(330, 330);
            //        Graphics g = Graphics.FromImage(bmp);
            //        g.DrawImage(img1, 0, 0, 330, 330);
            //        pic_Descript.Image = bmp;
            //        g.Dispose();
            //        img1.Dispose();
            //        //顯示照片
            //        pic_Descript.Visible = true;
            //    }
            //    else
            //    {
            //        pic_Descript.Visible = false;
            //    }

            //    a = DGV_Param1.Rows[0].Cells[Col_Param1_PCode.Index].Value as TArgument;
            //    if (a == null) return;
            //    int[] Buf1 = { 9012, 9019, 9022 };//這些工序的X1 要用位置
            //    int[] Buf2 = { 9019 };//這些工序的X2 要用位置
            //    int[] Buf3 = { 9010, 9011, 9014, 9015, 9016, 9017, 9018, 9020, 9021, 9023, 9024, 9025 };//這些工序的Z1 要用位置
            //    int[] Buf4 = { 9011, 9014, 9018, 9021, 9025 };//這些工序的Z2 要用位置
            //    Dictionary<string, int[]> MemMapping = new Dictionary<string, int[]>();
            //    MemMapping["19810"] = Buf1;
            //    MemMapping["19811"] = Buf2;
            //    MemMapping["19812"] = Buf3;
            //    MemMapping["19813"] = Buf4;
            //    bool bShowMEM = false;
            //    int[] Buf = new int[0];
            //    if (MemMapping.ContainsKey(a.AddrCode)) Buf = MemMapping[a.AddrCode];
            //    foreach (int no in Buf)
            //    {
            //        if (no == sp.ProgNo)
            //        {
            //            bShowMEM = true;
            //            break;
            //        }
            //    }
            //    uc_UserNumEditProc.btn_Memory.Enabled = bShowMEM;

            //}

        }

        private void LoadProcessImage()//讀取工序圖片
        {


            string filename;
            PictureBox[] buf = {pic_Process1, pic_Process2, pic_Process3, pic_Process4, pic_Process5, pic_Process6,
                                pic_Process7, pic_Process8, pic_Process9,pic_Process10, pic_Process11, pic_Process12, pic_Process13,
                                pic_Process14, pic_Process15, pic_Process16, pic_Process17, pic_Process18, pic_Process19, pic_Process20};

            Label[] labels = {la_Process1, la_Process2, la_Process3, la_Process4, la_Process5, la_Process6,
                              la_Process7, la_Process8, la_Process9,la_Process10,la_Process11,la_Process12,la_Process13,
                              la_Process14,la_Process15,la_Process16,la_Process17,la_Process18,la_Process19, la_Process20};

            for (int i = 0; i < buf.Length; i++)
            {
                XmlElement xmlProcess = machineSetting.xmlProcessList.GetChildNodeAt(i);
                int.TryParse(xmlProcess.GetAttribute("ID"), out int id);
                filename = Application.StartupPath + "\\image\\Process\\150x150\\Process" + id.ToString("00") + ".bmp";
                buf[i].Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                buf[i].Tag = id;
                if (id == 0)
                {
                    buf[i].Visible = false;
                    labels[i].Visible = false;
                }
                else
                {
                    if (id != 0)//工序只要有開就顯示
                    {
                        buf[i].Visible = true;
                        labels[i].Visible = true;
                        var processNode = Units.xmlDefaultProcessLang.Descendants("Process")
                                 .FirstOrDefault(x => x.Attribute("ID")?.Value == id.ToString());
                        if (processNode != null)
                        {
                            labels[i].Text = processNode.LastAttribute.Value;
                        }
                        else
                        {
                            labels[i].Text = "-";
                        }
                    }
                    else//其他沒開的就隱藏
                    {
                        buf[i].Visible = false;
                        labels[i].Visible = false;
                    }
                }
            }
        }

        private void ubtn_InsertProc_Click(object sender, EventArgs e)
        {
            LoadProcessImage();//讀取工序圖片
            CreateProcessMode = CreateMode.Insert;//前插入
            TC_Main.SelectedTab = tab_ProcSelect;//切到 工序選擇 頁
            PrevPage.Push(tab_ProcSelect);
            btn_Prev.Visible = true;
        }

        private void btn_InsertProcBack_Click(object sender, EventArgs e)
        {
            LoadProcessImage();//讀取工序圖片
            CreateProcessMode = CreateMode.InsertBack;//後插入
            TC_Main.SelectedTab = tab_ProcSelect;//切到 工序選擇 頁
            PrevPage.Push(tab_ProcSelect);
            btn_Prev.Visible = true;
        }

        private void ubtn_Prog_Del_Click(object sender, EventArgs e)
        {
            //例外處理
            if (DGV_ProgList.CurrentRow == null) return;

            int index = DGV_ProgList.CurrentRow.Index;
            //例外處理
            if (index < 0) return;

            TProgram pg = DGV_ProgList.CurrentRow.Cells[Col_TProgram.Index].Value as TProgram;
            //例外處理
            if (pg == null) return;

            DialogResult ret = Fo_Msg.Show(
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 5, "是否要刪除程式"),
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                        MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
                return;

            //清除預覽
            DGV_ProgView.Rows.Clear();

            //目前已經有開啟程式時, 
            if (CurrentProgram != null)
            {
                if (pg.ID == CurrentProgram.ID) //要檢查刪除的是否是開啟的
                {
                    CurrentProgram = null; //清空目前程式

                    //清空上一頁, 避免回到 程式 頁
                    PrevPage.Clear();
                    btn_Prev.Visible = false;
                }
            }

            Units.ProgramDB.Programs.Remove(pg);//刪除程式
            Units.ProgramDB.Save();//儲存

            DGV_ProgList.Rows.RemoveAt(index);//顯示程式清單 - 刪除程式, 會觸發事件重新顯示右邊的預覽

            RefleshProgListBtn();//更新下方按鍵
        }

        private void ubtn_Prog_SaveAs_Click(object sender, EventArgs e)
        {
            //例外處理
            if (DGV_ProgList.CurrentRow == null) return;

            //例外處理
            int index = DGV_ProgList.CurrentRow.Index;
            if (index < 0) return;

            TProgram pg = (TProgram)DGV_ProgList.CurrentRow.Cells[Col_TProgram.Index].Value;
            //例外處理
            if (pg == null) return;

            //複製程式
            TProgram cpy = pg.Clone();

            //例外處理
            if (cpy == null) return;

            cpy.Name = CreateProgName();
            //例外處理
            if (cpy.Name == "") return;

            //自動計算程式ID(唯一)
            int no = 1;
            while (true)
            {
                if (!Units.ProgramDB.ProgIDExists(no))
                    break;

                no++;
            }

            cpy.ID = no;

            Units.ProgramDB.Programs.Add(cpy); //加入程式
            Units.ProgramDB.Save(); //儲存

            ShowProgListFromDB(); //重新顯示程式清單

            SelectProgramList(no); //選擇剛剛加入的程式

        }

        private void ubtn_Prog_Open_Click(object sender, EventArgs e)
        {


            //清空量測數據
            Actions.Enqueue(new Action(() =>
            {
                double[] buf = new double[90];
                int len = 90;
                focas.WriteMacro(11000, ref len, buf);
            }));
            //例外處理
            if (DGV_ProgList.CurrentRow == null) return;

            //例外處理
            int index = DGV_ProgList.CurrentRow.Index;
            if (index < 0) return;

            //取得程式
            TProgram pg = (TProgram)DGV_ProgList.CurrentRow.Cells[Col_TProgram.Index].Value;
            foreach (var pc in pg.Processes)
            {
                if (pc.ID == 201)
                {
                    foreach (TSubProgram s in pc.SubPrograms)
                    {
                        TArgument a = s.GetArgument("19902");//使用/不使用
                        a.Value = 0;
                    }
                }
            }
            //例外處理
            if (pg == null) return;

            TC_Main.SelectedTab = tab_ProcList; //程式
            TempProgram = pg.Clone(); //複製程式到 編輯用的 暫存


            OpenProgram(pg, true); //呼叫，開啟程式(工序清單,補正值), 不處理畫面
            InitProcessExe(pg); //初始化工序開關, 預設全開, 但端測例外
            RefleshProgramLayout();//監視 - 工序清單重新整理
            WriteProcessExe();//將開關狀態寫入控制器

            ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)

            //編譯
            String src = "";

            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                src = CompilerProgram(pg);
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

            //換行格式切換
            src = src.Replace("\r\n", "\n");
            //依照換行符號將資料分行
            String[] lines = src.Split('\n');

            //先將程式輸出至檔案
            String FileName = Application.StartupPath + "\\ncfiles\\O8001.txt";
            File.WriteAllLines(FileName, lines);

            //取得程式名稱
            String Name = DGV_ProgList.CurrentRow.Cells[Col_ProgName.Index].Value.ToString();

            //有連線才寫
            if (!focas.IsConnected())
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 88, "Device Disconnect."));
                return;
            }
            int ret = -1;
            bFinish = false;
            Actions.Enqueue(new Action(() =>
            {

                focas.DeleteNcProgram("//CNC_MEM/USER/PATH1/O8001");
                ret = focas.WriteFile(FileType.NC_Program, lines.ToList(), "//CNC_MEM/USER/PATH1/");
                bFinish = true;
            }));
            int iStart2 = Environment.TickCount;
            while (!bFinish)
            {
                int iTime = Environment.TickCount - iStart2;
                if (iTime > 5000)
                {

                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                    return;
                }
                Application.DoEvents();
            }

            if (ret != SUCCESS)
            {
                //寫入失敗
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 90, "Write NC Program Fail.") + "(" + ret.ToString() + ")");
                return;
            }

            Actions.Enqueue(new Action(() =>
            {
                //設定主程式為選擇的程式
                focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
            }));

            //記憶最後呼叫的主程式
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("System", "MainProgram", pg.Name);
            MainProgram = pg.Name;

            PrevPage.Push(tab_ProcList);
            btn_Prev.Visible = true;
        }

        private void ShowProcessList()//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)
        {
            //清空所有欄位資料
            DGV_ProcList.Rows.Clear();

            la_ProcList_Prog_Name.Text = TempProgram.Name;

            //讀取所有工序
            int process_count = TempProgram.Processes.Count;
            for (int i = 0; i < process_count; i++)
            {
                TProcess process = TempProgram.Processes[i];

                //例外處理
                if (process == null)
                    continue;


                String FileName = Application.StartupPath + "\\image\\Process\\40x40\\Process" + ((int)process.ID).ToString("00") + ".bmp";
                Image img_Proc = File.Exists(FileName) ? Bitmap.FromFile(FileName) : new Bitmap(40, 40);

                Image img_Btn = TempExecEnabled[i] ? Resources.SwitchOn : Resources.SwitchOff;

                DGV_ProcList.Rows.Add(img_Proc, process.Name, process, process.ID, process.Memo, img_Btn);//(工序,加工模式)
                DGV_ProcList.Rows[DGV_ProcList.Rows.Count - 1].Cells[4].Style.ForeColor = Color.Gray;
            }


            bool bFlag = process_count > 0;

            btn_EditProc.Enabled = bFlag;//編輯
            btn_RemoveProc.Enabled = bFlag;//移除工序
            btn_Copy.Enabled = bFlag;//複製工序
            btn_InsertProcFront.Enabled = bFlag;//前插入
            btn_InsertProcBack.Enabled = bFlag;//後插入

        }

        private List<bool> TempExecEnabled = new List<bool>();

        //顯示要編輯的程式(不是空程式會顯示第一筆工序)
        private void EditProgram(TProgram program)
        {
            //例外處理
            if (program == null)
                return;

            //編輯目前的程式，紀錄目前開關的狀態
            if (program == CurrentProgram)
            {
                TempExecEnabled.Clear();
                for (int i = 0; i < ExecEnabled.Count; i++) TempExecEnabled.Add(ExecEnabled[i]); //複製目前開關狀態
            }

            TempProgram = program.Clone();//複製一份來編輯
            ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)

            btn_SaveProg.Visible = false;
            btn_SaveProgVisible = false;
        }

        private void btn_Prog_Call_Click(object sender, EventArgs e)
        {
            //例外處理
            if (DGV_ProgList.CurrentRow == null)
                return;

            //選擇的程式索引
            //int index = DGV_ProgList.CurrentRow.Index;
            int ret;

            TProgram pg = DGV_ProgList.CurrentRow.Cells[Col_TProgram.Index].Value as TProgram;
            if (pg == null) return;

            //編譯
            String src = CompilerProgram(pg);
            //換行格式切換
            src = src.Replace("\r\n", "\n");
            //依照換行符號將資料分行
            String[] lines = src.Split('\n');

            //先將程式輸出至檔案
            String FileName = Application.StartupPath + "\\ncfiles\\O8001.txt";
            File.WriteAllLines(FileName, lines);

            //取得程式名稱
            String Name = DGV_ProgList.CurrentRow.Cells[Col_ProgName.Index].Value.ToString();

            //有連線才寫
            if (!focas.IsConnected())
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 88, "Device Disconnect."));
                return;
            }


            //不是在EDIT模式
            if (!btn_EDIT.Lamp)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 89, "Not In EDIT Mode."));
                return;
            }

            focas.DeleteNcProgram("//CNC_MEM/USER/PATH1/O8001");
            ret = focas.WriteFile(FileType.NC_Program, lines.ToList(), "//CNC_MEM/USER/PATH1/");


            if (ret != SUCCESS)
            {
                //寫入失敗
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 90, "Write NC Program Fail.") + "(" + ret.ToString() + ")");
                return;
            }

            //顯示(刷新)程式列表，從維護可以查看有沒有上傳成功
            ShowNcProgramList();

            OpenProgram(pg, true); //呼叫，開啟程式(工序清單,補正值)
            InitProcessExe(pg); //初始化工序開關(全開)
            RefleshProgramLayout();//畫面重新整理
            WriteProcessExe();//將開關狀態寫入控制器(全開)

            //設定主程式為選擇的程式
            focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");

            //記憶最後呼叫的主程式
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("System", "MainProgram", Name);

            //切回到監視頁面
            btn_Monitor.PerformClick();
        }

        //OpenProgram時會執行
        private void WriteProcessExe()
        {
            Actions.Enqueue(new Action(() =>
            {
                int process_count = CurrentProgram.Processes.Count;
                for (int i = 0; i < 30; i++)
                {
                    if (i < process_count)
                    {
                        focas.WriteMacro(700 + i, ExecEnabled[i] ? 1 : 0);
                        focas.WriteMacro(730 + i, 0);//重修預設全關
                    }
                    else
                    {
                        focas.WriteMacro(700 + i, 0);//沒有工序的部分關閉
                        focas.WriteMacro(730 + i, 0);//重修預設全關
                    }
                }
            }));
        }

        //從NC讀取檔案後顯示在畫面上
        private void ShowNcProgramList()
        {
            /*
            //清空ListView
            LV_ProgList.Items.Clear();

            //讀取檔案列表
            String[,] FileNames;
            int ret = deltaCnc1.CncInfo.READ_dir_list("C:\\WORK\\", out FileNames);
            if (ret == (int)ApiCode.SUCCESS)
            {
                if (FileNames == null)
                    return;

                //回傳資料是二維陣列，N x 3
                //N 代表有多少支程式
                //取得第一維度GetLength(0)的長度，第二維度固定長度3，所以不用GetLength(1)
                for (int i = 0; i < FileNames.GetLength(0); i++)
                {

                    ListViewItem item = new ListViewItem(FileNames[i, 0]);//檔名
                    String size = FileNames[i, 1];//檔案大小
                    String type = FileNames[i, 2];//是檔案(File) or 是資料夾(Directory)
                    if (type == "File")
                    {
                        item.ImageIndex = 0;
                    }
                    else
                    {
                        item.ImageIndex = 1;
                    }
                    item.SubItems.Add(size);
                    LV_ProgList.Items.Add(item);
                }
            }
            */
        }

        public void InitProcessExe(TProgram program)
        {
            //刪除所有舊的
            ExecEnabled.Clear();
            RedoEnabled.Clear();
            TempExecEnabled.Clear();
            //重建(預設工序全開)
            int process_count = program.Processes.Count;
            for (int i = 0; i < process_count; i++)
            {
                TProcess proc = program.Processes[i];

                bool bFlag = proc.ID != 201 ? true : false; //端測預設關閉, 對點後才允許開啟
                                                            //新增加工選擇
                ExecEnabled.Add(bFlag);
                //新增重修精磨選擇
                RedoEnabled.Add(false);
                TempExecEnabled.Add(bFlag);
            }
        }

        public void RefleshProcessExe()
        {
            //刪除所有舊的
            ExecEnabled.Clear();
            RedoEnabled.Clear();

            //重建
            int process_count = TempExecEnabled.Count;
            for (int i = 0; i < process_count; i++)
            {
                //新增加工選擇
                ExecEnabled.Add(TempExecEnabled[i]);
                //新增重修精磨選擇
                RedoEnabled.Add(false);
            }
        }

        public void RefleshProgramLayout()
        {
            //讀取監視頁 程式清單(預設工序全開)
            LoadDGV_Program();
            //程式列表，顯示目前開啟的是哪個程式
            la_CurrentProgram.Text = CurrentProgram.Name;
            btn_Redo.Enabled = true;
            btn_Offset.Enabled = true;
        }

        public void OpenProgram(TProgram program, bool bAskClearTCode)
        {
            int ErrorCode = 0;
            try
            {
                if (program == null) return;
                //讀取程式
                CurrentProgram = program;

                Actions.Enqueue(new Action(() =>
                {
                    //加工程式呼叫第幾支程式 (EX:1 = O8001 為第一支)
                    focas.WriteMacro(958, 1);
                }));

                ErrorCode = 5;

                if (bAskClearTCode)
                {
                    //是否清除補償值
                    DialogResult ret = Fo_Msg.Show(
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 2, "是否要清除補償值"),
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
                                        MessageBoxButtons.YesNo);

                    //清除
                    if (ret == DialogResult.Yes)
                    {
                        for (int i = 0; i < CurrentProgram.Processes.Count; i++)
                        {
                            CurrentProgram.Processes[i].OffsetX.Value = 0;
                            CurrentProgram.Processes[i].OffsetZ.Value = 0;
                        }

                        Actions.Enqueue(new Action(() =>
                        {
                            WriteTCode(CurrentProgram);//OPEN時 清除? 使用者按下是
                        }));
                    }
                    //讀取
                    else
                    {
                        Actions.Enqueue(new Action(() =>
                        {
                            //只讀補正
                            ReadTCode(CurrentProgram);//OPEN時 清除? 使用者按下否
                        }));
                    }
                }
                else
                {
                    Actions.Enqueue(new Action(() =>
                    {
                        //只讀補正
                        ReadTCode(CurrentProgram);//OPEN時 讀取
                    }));
                }

                ErrorCode = 6;

                //紀錄最後開啟的程式
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteString("System", "MainProgram", CurrentProgram.Name);

                ErrorCode = 7;
            }
            catch (Exception)
            {
                Fo_Msg.Show("[Application Error] OpenProgram Error:" + ErrorCode.ToString("0000"), "Error");
            }
        }

        //only for Open Program 使用
        public void LoadDGV_Program()//監視頁 程式清單
        {
            DGV_Monitor_Program.Rows.Clear();
            int count = CurrentProgram.Processes.Count;
            for (int i = 0; i < count; i++)
            {
                TProcess process = CurrentProgram.Processes[i];
                Bitmap bmp = new Bitmap(100, 30);
                Bitmap icon = (Bitmap)Bitmap.FromFile(Application.StartupPath + "\\image\\Process\\40x40\\Process" + ((int)process.ID).ToString("00") + ".bmp");
                Graphics g = Graphics.FromImage(bmp);
                //Point pt = new Point(0, 0);
                int id = (int)process.ID - 1;
                if (id >= 0)
                {
                    g.DrawImage(icon, 0, 0, 30, 30);
                    g.DrawString("N" + (1000 + i * 1000).ToString(), DGV_Monitor_Program.Font, new SolidBrush(Color.Black), 30, 8);
                }
                g.Dispose();
                String StrProc = "";

                //GM-Code
                if (process.ID == 999)
                {
                    String[] lines = process.GM_Code.ToArray();
                    foreach (String s in lines) StrProc += s + " ";
                }
                else if (process.ID == 201)//端測
                {
                    StrProc += process.Name;
                    foreach (TSubProgram s in process.SubPrograms)
                    {
                        if (s.ProgNo == 9028)
                        {
                            TArgument a = s.GetArgument("19903");
                            if (a != null) StrProc += " X:" + a.Value.ToString(Units.DisplayFmt);
                            a = s.GetArgument("19904");
                            if (a != null) StrProc += " Z:" + a.Value.ToString(Units.DisplayFmt);
                            a = s.GetArgument("19907");
                            if (a != null) StrProc += " MASTER:" + a.Value.ToString(Units.DisplayFmt);
                        }
                    }
                }
                else
                {

                    StrProc += process.Name;
                    if (process.SubPrograms.Count == 3)
                    //foreach (TSubProgram s in process.SubPrograms)
                    {
                        TSubProgram s = process.SubPrograms[2];
                        if (s != null)
                        {
                            TArgument a = s.GetArgument("19810");
                            if (a != null) StrProc += " X:" + a.Value.ToString(Units.DisplayFmt);
                            a = s.GetArgument("19812");
                            if (a != null) StrProc += " Z:" + a.Value.ToString(Units.DisplayFmt);
                        }
                    }
                }


                int idd = (int)process.ID - 1;
                Bitmap bmpName = new Bitmap(271, 35);
                Graphics gName = Graphics.FromImage(bmpName);
                gName.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //gName.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (idd >= 0)
                {
                    gName.DrawString(process.Name, new Font("Times New Roman", 14F), new SolidBrush(Color.Black), 4, 0);
                    gName.DrawString(process.Memo, new Font("Times New Roman", 12F), new SolidBrush(Color.Black), 4, 18);
                }
                gName.Dispose();

                DGV_Monitor_Program.Rows.Add(bmp, bmpName, ExecEnabled[i] ? Resources.SwitchOn : Resources.SwitchOff);
                DGV_Monitor_Program.Rows[i].MinimumHeight = 35;

            }
        }

        public String CompilerProgram(TProgram program)
        {
            if (program == null)
                return "";
            String prog = "\r\n";

            //String PName = "";
            //if (program.Name != "")
            //{
            //PName = "(" + program.Name + ")";
            //}

            prog += "O8001\r\n";

            int ExeAddr = 700;
            int RepAddr = 966;
            int RedAddr = 730;

            String EQ = "EQ";
            String NE = "NE";
            String WAIT = "G31";

            int count = program.Processes.Count;
            for (int i = 0; i < count; i++)
            {
                if (count > 30)
                    continue;

                //讀取工序
                TProcess p = program.Processes[i];
                if (p == null)
                    continue;
                //設定工序N 號(流水號)
                prog += "N" + (1000 + i * 1000).ToString() + "\r\n" + WAIT + "\r\n";
                prog += "IF[#" + (ExeAddr + i).ToString() + NE + "1]GOTO" + ((i + 2) * 1000).ToString() + "\r\n";
                prog += "IF[[#" + RepAddr + EQ + "1]AND[#" + (RedAddr + i).ToString() + NE + "1.]]GOTO" + ((i + 2) * 1000).ToString() + "\r\n";
                //GM Code
                if (p.ID == 999)
                {
                    foreach (String s in p.GM_Code)
                        prog += s + "\r\n";
                }
                else
                {
                    TSubProgram sp = null;
                    if (p.SubPrograms.Count > 0) sp = p.SubPrograms[0];
                    if (sp == null) continue;
                    //呼叫副程式
                    prog += "G65P" + sp.ProgNo.ToString();
                    TArgument a = sp.GetArgument("19801");
                    if (a != null)
                    {
                        prog += "A" + a.Value.ToString(Units.DisplayFmt);
                    }

                    double[] macros = new double[200];
                    foreach (TArgument arg in sp.Arguments)
                    {
                        if (arg == null) continue;
                        int.TryParse(arg.AddrCode, out int PCodeNo);
                        int index = PCodeNo - 19801;
                        if (index >= 0 && index <= 20000) macros[index] = arg.Value;
                    }
                    int len = 200;
                    int ret = focas.WriteMacro(19801 + (i + 1) * 200, ref len, macros);

                    prog += "\r\n";
                }

                prog += WAIT + "\r\n";
            }
            prog += "N" + ((count + 1) * 1000).ToString() + "\r\n";
            prog += "M99\r\n%\r\n";

            return prog;
        }

        public String GetStrPara(TSubProgram sp)
        {
            String prog = "";


            //呼叫副程式
            prog += "G65P" + sp.ProgNo.ToString();
            int arg_count = sp.Arguments.Count;
            for (int k = 0; k < arg_count; k++)
            {
                TArgument a = sp.Arguments[k];
                prog += a.AddrCode + a.Value.ToString(Units.DisplayFmt);
            }
            /*
            int adv_count = sp.AdvArguments.Count;
            for (int k = 0; k < adv_count; k++)
            {
                TArgument a = sp.AdvArguments[k];
                prog += a.AddrCode + a.Value.ToString(Units.DisplayFmt);
            }
            */
            if (sp.ProgNo == GrindingDress.ProgNo)
            {
                arg_count = GrindingDress.Arguments.Count;
                for (int k = 0; k < arg_count; k++)
                {
                    TArgument a = GrindingDress.Arguments[k];
                    prog += a.AddrCode + a.Value.ToString(Units.DisplayFmt);
                }
                /*
                adv_count = GrindingDress.AdvArguments.Count;
                for (int k = 0; k < adv_count; k++)
                {
                    TArgument a = GrindingDress.AdvArguments[k];
                    prog += a.AddrCode + a.Value.ToString(Units.DisplayFmt);
                }
                */
            }

            prog += "\r\n";

            return prog;
        }

        private void DGV_ProgList_CurrentCellChanged(object sender, EventArgs e)
        {
            //例外處理
            if (DGV_ProgList.CurrentRow == null)
                return;

            TProgram pg = DGV_ProgList.CurrentRow.Cells[Col_TProgram.Index].Value as TProgram;
            if (pg == null) return;

            //預覽
            LoadTProgram(DGV_ProgView, pg);
        }

        //預覽
        public void LoadTProgram(DataGridView DGV, TProgram program)
        {
            //例外處理
            if (program == null)
                return;

            //刷新列表
            DGV.Rows.Clear();
            //所有工序重新加入欄位中
            int process_count = program.Processes.Count;
            for (int i = 0; i < process_count; i++)
            {
                TProcess process = program.Processes[i];
                //例外處理
                if (process == null) continue;

                //工序圖路徑
                String FileName = Application.StartupPath + "\\image\\Process\\40x40\\Process" + ((int)process.ID).ToString("00") + ".bmp";
                Bitmap bmp = null;
                if (File.Exists(FileName))
                {
                    //載入圖片
                    bmp = (Bitmap)Bitmap.FromFile(FileName);
                    //繪製圖片
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(bmp, new Rectangle(0, 0, 40, 40));
                }
                //資料加入欄位
                DGV.Rows.Add(bmp, process.Name, process.Memo);//(工序,加工模式)               
            }
        }

        private void DGV_ProgList_DoubleClick(object sender, EventArgs e)
        {
            this.btn_Prog_Open.PerformClick();
        }

        private void btn_EditProc_Click(object sender, EventArgs e)
        {
            DGV_ProcList_CellDoubleClick(DGV_ProcList, null);

            Edit_DGV = DGV_Param1;
        }

        private void ubtn_AddProc_Click(object sender, EventArgs e)
        {
            /*
            if (pic_Warning.Visible)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 83, "請先完成量測資料設定"));
                return;
            }
            */

            LoadProcessImage();

            Edit_DGV = DGV_Param1;

            CreateProcessMode = CreateMode.Add;
            TC_Main.SelectedTab = tab_ProcSelect;
            PrevPage.Push(tab_ProcSelect);
            btn_Prev.Visible = true;
        }


        private void ubtn_RemoveProc_Click(object sender, EventArgs e)
        {
            DialogResult ret = Fo_Msg.Show(
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 129, "是否要刪除此工序"),
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                        MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes) return;

            //例外處理
            if (DGV_ProcList.CurrentRow == null) return;

            //例外處理
            int index = DGV_ProcList.CurrentRow.Index;
            if (index < 0) return;

            if (CurrentProgram != null)
            {
                if (TempProgram.ID == CurrentProgram.ID) TempExecEnabled.RemoveAt(index);
            }

            DGV_ProcList.Rows.RemoveAt(index);
            TempProgram.Processes.RemoveAt(index);

            if (DGV_ProcList.RowCount == 0)
            {
                btn_EditProc.Enabled = false;
                btn_InsertProcBack.Enabled = false;
                btn_InsertProcFront.Enabled = false;
                btn_RemoveProc.Enabled = false;
                btn_Copy.Enabled = false;
            }
            else
            {
                btn_EditProc.Enabled = true;
                btn_InsertProcBack.Enabled = true;
                btn_InsertProcFront.Enabled = true;
                btn_RemoveProc.Enabled = true;
                btn_Copy.Enabled = true;
            }

            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
            Col_ProcList_Btn.Visible = false;
        }

        private void ubtn_SaveProg_Click(object sender, EventArgs e)
        {

            btn_SaveProg.Focus();
            Application.DoEvents();

            if (TempProgram == null)
                return;

            //重新編號9001.A
            //int ProcessNo = 1;
            for (int i = 0; i < TempProgram.Processes.Count; i++)
            {
                TProcess proc = TempProgram.Processes[i];
                if (proc == null)
                    continue;

                //GM Code 沒有引數、沒有SubProgram
                if (proc.ID == 999)
                    continue;

                if (proc.SubPrograms.Count <= 0)
                    continue;

                if (i >= 30)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 7, "已超過工序使用上限。"));
                    break;
                }

                TArgument a = proc.SubPrograms[0].GetArgument("19801");//工序號，顯示以及使用TCode
                if (a == null) continue;
                a.Value = i + 1;

            }

            //將程式存回程式庫
            bool bFind = false;
            for (int i = 0; i < Units.ProgramDB.Programs.Count; i++)
            {
                TProgram p = Units.ProgramDB.Programs[i];
                if (p.ID == TempProgram.ID)
                {
                    Units.ProgramDB.Programs[i] = TempProgram.Clone();
                    bFind = true;
                    break;
                }
            }
            if (!bFind)
            {
                Units.ProgramDB.Programs.Add(TempProgram);
            }



            //儲存程式庫
            Units.ProgramDB.Save();
            ShowProgListFromDB();

            if (CurrentProgram == null)
            {
                InitProcessExe(TempProgram);//不同程式，全部開啟
            }
            else
            {
                if (TempProgram.Name == CurrentProgram.Name)
                {
                    RefleshProcessExe();//儲存, 經過編輯開關會多會少，更新到目前設定
                }
                else
                {
                    InitProcessExe(TempProgram);//不同程式，全部開啟
                }
            }

            CurrentProgram = TempProgram.Clone();//複製回去
            RefleshProgramLayout();//畫面重新整理
            WriteProcessExe();//將設定寫到控制器

            //寫入 T Code
            Actions.Enqueue(new Action(() =>
            {
                WriteTCode(CurrentProgram);
            }));

            //編譯
            String src = "";

            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                src = CompilerProgram(TempProgram);
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

            //換行格式切換
            src = src.Replace("\r\n", "\n");
            //依照換行符號將資料分行
            String[] lines = src.Split('\n');

            //先將程式輸出至檔案
            String FileName = Application.StartupPath + "\\ncfiles\\O8001.txt";
            if (!Directory.Exists(Application.StartupPath + "\\ncfiles\\")) Directory.CreateDirectory(Application.StartupPath + "\\ncfiles\\");
            File.WriteAllLines(FileName, lines);

            //取得程式名稱
            String Name = TempProgram.Name;
            //if(DGV_ProgList.CurrentRow.Cells[Col_ProgName.Index].Value!= null) Name = DGV_ProgList.CurrentRow.Cells[Col_ProgName.Index].Value.ToString();

            //記憶最後呼叫的主程式
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("System", "MainProgram", Name);
            MainProgram = Name;

            //有連線才寫
            if (!focas.IsConnected())
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 88, "Device Disconnect."));
                return;
            }

            //不是在EDIT模式
            if (!btn_EDIT.Lamp)
            {

                Actions.Enqueue(new Action(() =>
                {
                    focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
                }));
            }

            int ret = -1;
            bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                focas.DeleteNcProgram("//CNC_MEM/USER/PATH1/O8001");
                ret = focas.WriteFile(FileType.NC_Program, lines.ToList(), "//CNC_MEM/USER/PATH1/");
                bFinish = true;
            }));
            int iStart2 = Environment.TickCount;
            while (!bFinish)
            {
                int iTime = Environment.TickCount - iStart2;
                if (iTime > 5000)
                {

                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                    return;
                }
                Application.DoEvents();
            }

            if (ret != SUCCESS)
            {
                //寫入失敗
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 90, "Write NC Program Fail.") + "(" + ret.ToString() + ")");
                return;
            }


            Actions.Enqueue(new Action(() =>
            {
                //設定主程式為選擇的程式
                focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
            }));

            ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)
            btn_SaveProg.Visible = false;
            btn_SaveProgVisible = false;

            Col_ProcList_Btn.Visible = true;
        }

        private void DGV_ProcList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CB.Visible = false;
            BTN.Visible = false;

            //例外處理
            if (DGV_ProcList.CurrentRow == null)
                return;

            //例外處理
            int index = DGV_ProcList.CurrentRow.Index;
            if (index < 0)
                return;

            //目前選擇的工序
            TProcess p = TempProgram.Processes[index];
            ProcessIndex = index;
            TempProcess = p;

            //紀錄工序ID
            ProcID = p.ID.ToString();
            int.TryParse(ProcID, out int procid);

            if (procid == 201 || procid == 202 || procid == 203) //量測工序
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 155, "B軸請先回0度"), "");
            }

            //編輯畫面 顯示此工序
            SetProcessData(p);

            //顯示工序名稱
            la_EditProgTitle.Text = DGV_ProcList.CurrentRow.Cells[1].Value.ToString();

            TC_Main.SelectedTab = tab_EditProc;

            PrevPage.Push(tab_EditProc);
            btn_Prev.Visible = true;

            //顯示存檔
            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
        }



        public void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            uc_UserNumEditProc.btn_OK.Enabled = true;
            CB.Items.Clear();
            CB.Parent = null;
            BTN.Parent = null;

            //目前正在編輯的 DataGridView
            Edit_DGV = (DataGridView)sender;

            //例外處理
            if (Edit_DGV.CurrentCell == null) return;

            //目前編輯的工序 
            if (TempProcess == null) return;

            //TProcess process = null;
            //if ((ProcessIndex >= 0) && (ProcessIndex < TempProgram.Processes.Count))
            //{
            //process = TempProgram.Processes[ProcessIndex];
            //}
            //if (process == null) return;

            TProcess def_proc = null;//搜尋預設工序
            foreach (var p in Units.ProcessList)
            {
                if (p.ID == TempProcess.ID)
                {
                    def_proc = p;
                    break;
                }
            }
            if (def_proc == null) return;//例外處理

            TSubProgram sp = null;
            if (TempProcess.SubPrograms.Count > 0) sp = TempProcess.SubPrograms[0];//固定只剩下主程式
            if (sp == null) return;

            int Row = Edit_DGV.CurrentCell.RowIndex;
            int Col = Edit_DGV.CurrentCell.ColumnIndex;

            if (Edit_DGV.Rows.Count == 0) return;
            if (Edit_DGV.Rows[Row].Cells.Count == 0) return;

            TArgument a = Edit_DGV.CurrentRow.Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
            if (a == null) return;


            //取得預設工序中的此P CODE
            TArgument def_a = null;
            foreach (var aa in def_proc.SubPrograms[0].Arguments)
            {
                if (aa.AddrCode == a.AddrCode)
                {
                    def_a = aa;
                    break;
                }
            }

            //尋找照片位置
            string procid = "ID" + ProcID;
            string picAddr = a.AddrCode + ".png"; //[PCode No].png
            string PicPath = Application.StartupPath + "\\image\\ProcImage\\" + procid + "\\" + picAddr;

            //如果之資料夾裡有此檔案就會顯示
            if (File.Exists(PicPath))
            {
                pic_Descript.Image = Image.FromFile(PicPath);
                pic_Descript.Visible = true;//顯示照片
            }
            else
            {
                pic_Descript.Visible = false;
            }


            uc_UserNumEditProc.la_Msg.Text = Edit_DGV.CurrentRow.Cells[Edit_DGV_Index["Name"]].Value.ToString();
            if (a.AddrCode == "19880") //砂輪號 最大值隨著 #10050 砂輪數改變
            {
                uc_UserNumEditProc.la_Msg.Text += "\r\n" + def_a.Min.ToString("0.#####") + " ~ " + GwCount.ToString("0.#####"); // 顯示所選工序的上下限
                uc_UserNumEditProc.btn_OK.Enabled = false;
            }
            else
            {
                uc_UserNumEditProc.la_Msg.Text += "\r\n" + def_a.Min.ToString("0.#####") + " ~ " + def_a.Max.ToString("0.#####"); // 顯示所選工序的上下限
            }
            int[] Buf1 = { 9012, 9013, 9014, 9018, 9022, 9026, 9032, 9038 };//這些工序的 #19810(X or X1) 要用記憶
            int[] Buf2 = { 9019, 9026 };//這些工序的 #19811(X2) 要用記憶
            int[] Buf3 = { 9010, 9011, 9013, 9014, 9015, 9016, 9017, 9018, 9020, 9021, 9023, 9025, 9030, 9031, 9033, 9034, 9036, 9037, 9038, 9112, 9113 };//這些工序的 #19812 (Z or Z1) 要用記憶
            int[] Buf4 = { 9011, 9014, 9015, 9017, 9018, 9021, 9025, 9031, 9037 };//這些工序的 #19813(Z2) 要用記憶
            int[] Buf5 = { 9040, 9041 };

            Dictionary<string, int[]> MemMapping = new Dictionary<string, int[]>();
            MemMapping["19810"] = Buf1; //X1
            MemMapping["19811"] = Buf2; //X2
            MemMapping["19812"] = Buf3; //Z1
            MemMapping["19813"] = Buf4; //Z2
            MemMapping["19908"] = Buf5; //Z
            MemMapping["19909"] = Buf5; //X1
            MemMapping["19910"] = Buf5; //X2
            MemMapping["19911"] = Buf5; //ZS

            bool bShowMEM = false;
            int[] Buf = new int[0];
            if (MemMapping.ContainsKey(a.AddrCode)) Buf = MemMapping[a.AddrCode];
            foreach (int no in Buf)
            {
                if (no == sp.ProgNo)
                {
                    bShowMEM = true;
                    break;
                }
            }
            //uc_UserNumEditProc.btn_Memory.Visible = bShowMEM;
            uc_UserNumEditProc.btn_Memory.Enabled = bShowMEM;


            /*
            if (Buf5.Contains(a.AddrCode))
            {
                if (a.AddrCode != "19823" || (a.AddrCode == "19823" && ProcID == "47"))
                {
                    uc_UserNumEditProc.btn_OK.Enabled = false;
                }

            }
            else
            {
                uc_UserNumEditProc.btn_OK.Enabled = true;
            }
            */

            if (e == null) return;
            int.TryParse(ProcID, out int id);

            bool bODGw = true;
            if(id >= 50)
            {
                bODGw = false;
            }

            var processNode = Units.xmlDefaultProcessLang.Descendants("Process").FirstOrDefault(x => x.Attribute("ID")?.Value == TempProcess.ID.ToString());
            var pcodesWithTexts = processNode.Elements("PCode")
                        .Where(x => x.Elements("Text").Any())
                        .Select(x => new PCodeInfo
                        {
                            PCodeNo = x.Attribute("No")?.Value,
                            Texts = x.Elements("Text")
                                .Select(text => new TextInfo
                                {
                                    Value = text.Attribute("Value")?.Value,
                                    Name = text.Attribute("Name")?.Value
                                }).ToList()
                        }).ToList();
            var matchingPCode = pcodesWithTexts.FirstOrDefault(pcode => pcode.PCodeNo == a.AddrCode);
            Edit_AVN = matchingPCode;

            if (matchingPCode != null) //顯示文字
            {
                if (a.AddrCode == "19865")
                {
                    
                    bool bFinish = false;
                    ushort val = 0;
                    Actions.Enqueue(new Action(() =>
                    {

                        focas.PMC_ReadWord(PmcAddrType.D, 68, out val);
                        bFinish = true;
                    }));

                    int iStart = Environment.TickCount;
                    while (!bFinish)
                    {
                        int iTime = Environment.TickCount - iStart;
                        if (iTime > 5000)
                        {

                            //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                            break;
                        }
                        Application.DoEvents();
                    }

                    if (val < 1) val = 1;
                    for (int i = 0; i < val; i++)
                    {
                        if (i >= matchingPCode.Texts.Count) break;
                        CB.Items.Add(matchingPCode.Texts[i].Name);
                    }
                    CB.Text = Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value.ToString();

                    Rectangle rect = Edit_DGV.GetCellDisplayRectangle(Col, Row, false);
                    CB.Parent = Edit_DGV;
                    CB.Font = new Font("Times New Roman", 20);
                    CB.DropDownStyle = ComboBoxStyle.DropDownList;
                    CB.Left = 0;
                    CB.Top = rect.Top;
                    CB.Width = Edit_DGV.Width;
                    CB.Height = 20;
                    CB.Visible = true;
                    uc_UserNumEditProc.btn_OK.Enabled = false;
                    CB.Focus();
                }
                else if (a.AddrCode == "19880") //砂輪號
                {
                    int no = 0;
                    foreach (var vn in matchingPCode.Texts)
                    {
                        no++;
                        if (no > GwCount) break;

                        //if (Gw2CantUse && Gw2CantUseID.Contains(id) && no == 2) continue;
                        if(bODGw && GWType[no - 1] == MachineType.OIG)
                        {
                            continue;
                        }
                        if (!bODGw && GWType[no - 1] != MachineType.OIG)
                        {
                            continue;
                        }
                        CB.Items.Add(vn.Name);

                    }
                    CB.Text = Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value.ToString();

                    Rectangle rect = Edit_DGV.GetCellDisplayRectangle(Col, Row, false);
                    CB.Parent = Edit_DGV;
                    CB.Font = new Font("Times New Roman", 20);
                    CB.DropDownStyle = ComboBoxStyle.DropDownList;
                    CB.Left = 0;
                    CB.Top = rect.Top;
                    CB.Width = Edit_DGV.Width;
                    CB.Height = 20;
                    CB.Visible = true;
                    uc_UserNumEditProc.btn_OK.Enabled = false;
                    CB.Focus();
                }
                else if (a.AddrCode == "19949")
                {
                    Rectangle rect = Edit_DGV.GetCellDisplayRectangle(Col_Advance_TextValue.Index, Row, false);
                    BTN.Parent = Edit_DGV;
                    BTN.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 150, "Edit");
                    BTN.Left = rect.Left - 4;
                    BTN.Top = rect.Top - 4;
                    BTN.Width = rect.Width + 8;
                    BTN.Height = rect.Height + 8;
                    BTN.Visible = true;
                    BTN.Focus();
                }
                else //其他文字 直接顯示
                {
                    foreach (var vn in matchingPCode.Texts)
                    {
                        CB.Items.Add(vn.Name);
                    }
                    CB.Text = Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value.ToString();

                    Rectangle rect = Edit_DGV.GetCellDisplayRectangle(Col, Row, false);
                    CB.Parent = Edit_DGV;
                    CB.Font = new Font("Times New Roman", 20);
                    CB.DropDownStyle = ComboBoxStyle.DropDownList;
                    CB.Left = 0;
                    CB.Top = rect.Top;
                    CB.Width = Edit_DGV.Width;
                    CB.Height = 20;
                    CB.Visible = true;
                    uc_UserNumEditProc.btn_OK.Enabled = false;
                    CB.Focus();
                }
            }
            else //顯示數值
            {
                /*
                if (bShowMEM)
                {
                    //跳出數字鍵盤
                    Fo_NumMem form = new Fo_NumMem();
                    //位置不要擋到座標
                    form.StartPosition = FormStartPosition.Manual;
                    form.Left = this.Left + 495;
                    form.Top = this.Top + 680 - form.Height;
                    form.ParaName = Edit_DGV.Rows[Row].Cells[0].Value.ToString();
                    //form.SetVal(double.Parse(Edit_DGV.Rows[Row].Cells[5].Value.ToString()));
                    DialogResult ret = form.ShowDialog();
                    if (ret != DialogResult.OK) return;

                    double val = form.TmpVal;
                    if (val < a.Min)
                    {
                        val = a.Min;
                    }
                    if (val > a.Max)
                    {
                        val = a.Max;
                    }
                    //數值寫回顯示欄
                    Edit_DGV.Rows[Row].Cells[6].Value = val.ToString(Units.DisplayFmt);

                    string addr = Edit_DGV.Rows[Row].Cells[8].Value.ToString();
                    foreach (DataGridViewRow row in DGV_Param.Rows)
                    {
                        if (row.Cells[7].Value.ToString().Equals(addr))
                        {
                            DGV_Param.Rows[row.Index].Cells[5].Value = val.ToString(Units.DisplayFmt);
                            break;
                        }
                    }

                    //寫回引數
                    a.Value = val;
                    btn_SaveProg.Visible = true;
                    btn_SaveProgVisible = true;
                }
                else
                {
                    //跳出數字鍵盤

                    Fo_NumTitle form = new Fo_NumTitle();
                    //位置不要擋到座標
                    form.StartPosition = FormStartPosition.Manual;
                    form.Left = this.Left + 495;
                    form.Top = this.Top + 680 - form.Height;
                    form.ParaName = Edit_DGV.Rows[Row].Cells[0].Value.ToString();


                    //form.SetVal(double.Parse(Edit_DGV.Rows[Row].Cells[5].Value.ToString()));
                    DialogResult ret = form.ShowDialog();
                    if (ret != DialogResult.OK) return;

                    double val = form.TmpVal;
                    if (val < a.Min)
                    {
                        val = a.Min;
                    }
                    if (val > a.Max)
                    {
                        val = a.Max;
                    }
                    //數值寫回顯示欄
                    Edit_DGV.Rows[Row].Cells[6].Value = val.ToString(Units.DisplayFmt);

                    string addr = Edit_DGV.Rows[Row].Cells[8].Value.ToString();
                    foreach (DataGridViewRow row in DGV_Param.Rows)
                    {
                        if (row.Cells[7].Value.ToString().Equals(addr))
                        {
                            DGV_Param.Rows[row.Index].Cells[5].Value = val.ToString(Units.DisplayFmt);
                            break;
                        }
                    }

                    //寫回引數
                    a.Value = val;
                    btn_SaveProg.Visible = true;
                    btn_SaveProgVisible = true;
                }
                */
            }

        }

        public bool GetMemoryValue(out double value)
        {
            //沒有點選任何一個 DataGridView
            if (Edit_DGV == null)
            {
                value = 0;
                return false;
            }

            //沒有可選的東西
            if (Edit_DGV.CurrentRow == null)
            {
                value = 0;
                return false;
            }

            TArgument a = Edit_DGV.CurrentRow.Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
            if (a == null)
            {
                value = 0;
                return false;
            }

            if (TempProcess == null)
            {
                value = 0;
                return false;
            }

            if (TempProcess.SubPrograms.Count == 0)
            {
                value = 0;
                return false;
            }

            TSubProgram sp = TempProcess.SubPrograms[0];

            //Z1 Z2
            if (a.AddrCode == "19812" || a.AddrCode == "19813")
            {
                return double.TryParse(la_EditAbsAxis2Value.Text, out value);
            }
            else if (a.AddrCode == "19911" || a.AddrCode == "19908") //量測工序 ZS Z
            {
                return double.TryParse(la_EditMachAxis2Value.Text, out value);
            }

            //Z2
            //if (a.AddrCode == "19813")
            //{
            //    //終點Z2
            //    double Z2;
            //    //外圓工序
            //    if (TempProcess.ID >= 1 && TempProcess.ID <= 10)
            //    {
            //        //起點Z1
            //        double Z1 = sp.GetArgument("19812").Value;

            //        //目前位置
            //        double Z = double.Parse(la_EditAbsAxis2Value.Text);

            //        //砂輪寬
            //        focas.ReadMacro(509, out double GwWidth);


            //        if (Z > Z1)
            //        {
            //            Z2 = Z + GwWidth;
            //        }
            //        else if (Z < Z1)
            //        {
            //            Z2 = Z - GwWidth;
            //        }
            //        else
            //        {
            //            if (bShowG59)
            //            {
            //                Z2 = Z - GwWidth;
            //            }
            //            else
            //            {
            //                Z2 = Z + GwWidth;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        double.TryParse(la_EditAbsAxis2Value.Text, out Z2);
            //    }
            //    value = Z2;
            //    return true;
            //}

            //X
            if (a.AddrCode == "19810" || a.AddrCode == "19811") //X
            {
                bool flag = double.TryParse(la_EditAbsAxis1Value.Text, out value);
                int[] CheckOD = { 9018, 9038 };
                if (CheckOD.Contains(TempProcess.SubPrograms[0].ProgNo))
                {
                    value *= -1;
                }
                return flag;
            }
            else if (a.AddrCode == "19909" || a.AddrCode == "19910")//量測工序 X1 X2
            {
                return double.TryParse(la_EditMachAxis1Value.Text, out value);
            }

            value = 0;
            return false;
        }

        /*
        private void DGV_Normal_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //設定值 = 5
            if (e.ColumnIndex != 5)
                return;

            //這些是用名稱顯是的設定值，不用搬
            if (Edit_AVN != null)
                return;

            //數值 = 設定值
            DGV_Param.Rows[e.RowIndex].Cells[1].Value = DGV_Param.Rows[e.RowIndex].Cells[5].Value;
            DGV_Param.RefreshEdit();
        }
        */

        //任一個工序中的條件數值改變
        private void Edit_DGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //顯示文字
            if (e.ColumnIndex != Edit_DGV_Index["TextValue"])
                return;

            //這些是用下拉式選單，改變的是字串，不用處理
            if (Edit_AVN != null)
                return;

            string[] unit = { "times", "rpm", "steps" };

            /*
            Edit_DGV_Index.Add("Name", 0);//參數名稱
            Edit_DGV_Index.Add("Code", 1);//顯示圖片代碼
            Edit_DGV_Index.Add("TextValue", 2);//顯示文字或數值
            Edit_DGV_Index.Add("Unit", 3);//單位
            Edit_DGV_Index.Add("PCode", 4);//PCode物件
            Edit_DGV_Index.Add("DoubleValue", 5);//浮點數值             
            */

            //目前正在編輯的 DataGridView
            DataGridView dgv = (DataGridView)sender;

            //其他非下拉式選單都是純數值
            //浮點數值 = 顯示文字
            dgv.Rows[e.RowIndex].Cells[Edit_DGV_Index["DoubleValue"]].Value = dgv.Rows[e.RowIndex].Cells[2].Value;

            if (unit.Contains((dgv.Rows[e.RowIndex].Cells[3].Value.ToString())))
            {
                string StrValue = dgv.Rows[e.RowIndex].Cells[2].Value.ToString();
                int dotIndex = StrValue.IndexOf('.'); // 找到小数点的位置
                if (dotIndex != -1) // 如果找到小数点
                {
                    dgv.Rows[e.RowIndex].Cells[2].Value = StrValue.Substring(0, dotIndex); // 截取小数点之前的部分
                }
            }

            dgv.RefreshEdit();

            //
            if (dgv != Edit_DGV) return;

            //處理修整回退量 同步
            DataGridView Dest_DGV = null;
            if (Edit_DGV == DGV_Dress1)
            {
                Dest_DGV = DGV_Dress2;
            }
            else if (Edit_DGV == DGV_Dress2)
            {
                Dest_DGV = DGV_Dress1;
            }
            if (Dest_DGV != null)
            {
                //Edit_DGV.Rows[e.RowIndex].Cells[Edit_DGV_Index["DoubleValue"]].Value;

                TArgument a = Edit_DGV.CurrentRow.Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
                if (a == null) return;

                if (a.AddrCode != "19943") return; //修整回退量

                //搜尋 要同步的修整回退量
                for (int i = 0; i < Dest_DGV.Rows.Count; i++)
                {
                    TArgument dest_a = Dest_DGV.Rows[i].Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
                    if (dest_a.AddrCode != "19943") continue;
                    //dest_a.Value = a.Value;
                    Dest_DGV.Rows[i].Cells[Edit_DGV_Index["TextValue"]].Value = Edit_DGV.CurrentCell.Value;
                }

                //if (dt.Tag.ToString() == "1")//修整1的話
                //{
                //    foreach (DataGridViewRow row in DGV_Dress2.Rows)
                //    {
                //        TArgument a2 = row.Cells[Edit_DGV_Index["PCode"]].Value as TArgument; //修整2
                //        if (a2.AddrCode == "19943" && row.Cells[2].Value.ToString() != Edit_DGV.CurrentRow.Cells[2].Value.ToString()) //Pcode要正確 兩邊的數值要不一樣 不然會無限輪迴
                //        {
                //            row.Cells[2].Value = ans;
                //        }
                //    }
                //}
                //else if (dt.Tag.ToString() == "2")
                //{
                //    foreach (DataGridViewRow row in DGV_Dress1.Rows)
                //    {
                //        TArgument a2 = row.Cells[Edit_DGV_Index["PCode"]].Value as TArgument; //修整1
                //        if (a2.AddrCode == "19943" && row.Cells[2].Value.ToString() != Edit_DGV.CurrentRow.Cells[2].Value.ToString()) //Pcode要正確 兩邊的數值要不一樣 不然會無限輪迴
                //        {
                //            row.Cells[2].Value = ans;
                //        }
                //    }
                //}


                ////共用 修整回退量
                ////搜尋修整2的引數  
                //foreach (DataGridViewRow row in Dest_DGV.Rows)
                //{
                //    TArgument a2 = row.Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
                //    if (a2 == null) continue;
                //    if (a.AddrCode == "19943")//修整回退量
                //    {
                //        row.Cells[Edit_DGV_Index["DoubleValue"]].Value = Edit_DGV.CurrentCell.Value;
                //        row.Cells[Edit_DGV_Index["TextValue"]].Value = Edit_DGV.CurrentCell.Value;
                //        Dest_DGV.RefreshEdit();
                //    }
                //}

            }
        }


        private void DGV_InProcArg_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //設定值 = 6
            if (e.ColumnIndex != 6)
                return;

            if (DGV_Dress1.CurrentCell == null)
                return;

            //這些是用名稱顯是的設定值，不用搬
            if (Edit_AVN != null) return;

            //數值 = 設定值
            DGV_Dress1.Rows[e.RowIndex].Cells[1].Value = DGV_Dress1.CurrentCell.Value;

            //if (DGV_Dress1.CurrentRow.Cells[8].Value.ToString() == "19943")
            //{
            //    foreach (DataGridViewRow row in DGV_Dress2.Rows)
            //    {
            //        if (row.Cells[8].Value.ToString().Equals("19943"))
            //        {
            //            DGV_Dress2.Rows[row.Index].Cells[6].Value = DGV_Dress1.CurrentCell.Value;
            //            DGV_Dress2.Rows[row.Index].Cells[1].Value = DGV_Dress1.CurrentCell.Value;
            //            DGV_Dress2.RefreshEdit();
            //        }
            //    }
            //}


            DGV_Dress1.RefreshEdit();

        }

        private void DGV_InProcArg2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //設定值 = 6
            if (e.ColumnIndex != 6)
                return;
            if (DGV_Dress2.CurrentCell == null)
                return;

            //這些是用名稱顯是的設定值，不用搬
            if (Edit_AVN != null) return;

            //數值 = 設定值
            DGV_Dress2.Rows[e.RowIndex].Cells[1].Value = DGV_Dress2.CurrentCell.Value;

            //if (DGV_Dress2.CurrentRow.Cells[8].Value.ToString() == "19943")
            //{
            //    foreach (DataGridViewRow row in DGV_Dress1.Rows)
            //    {
            //        if (row.Cells[8].Value.ToString().Equals("19943"))
            //        {
            //            DGV_Dress1.Rows[row.Index].Cells[6].Value = DGV_Dress2.CurrentCell.Value;
            //            DGV_Dress1.Rows[row.Index].Cells[1].Value = DGV_Dress2.CurrentCell.Value;
            //            DGV_Dress1.RefreshEdit();
            //        }
            //    }
            //}
            DGV_Dress2.RefreshEdit();
        }


        /*
        private void btn_UseCurrentPosCal_Click(object sender, EventArgs e)
        {
            if (DGV_ProcList.CurrentRow == null)
                return;

            TArgument a = DGV_Param.CurrentRow.Cells[Col_N_Arg.Index].Value as TArgument;
            TProcess p = DGV_ProcList.CurrentRow.Cells[Col_EP_TProc.Index].Value as TProcess;
            TSubProgram sp = DGV_Param.CurrentRow.Cells[Col_N_SubProg.Index].Value as TSubProgram;
            String ID = "O" + sp.ProgNo.ToString() + "." + a.AddrCode;

            //Z1
            if (ID == "O9011.C" ||
                ID == "O9014.C" ||
                ID == "O9021.C" ||
                ID == "O9010.C" ||
                ID == "O9020.C")
            {
                DGV_Param.CurrentRow.Cells[Col_N_ParamValue.Index].Value = double.Parse(la_EditAbsAxis2Value.Text);
                DGV_Param.CurrentRow.Cells[Col_N_Show.Index].Value = double.Parse(la_EditAbsAxis2Value.Text);
            }

            //Z2
            if (ID == "O9011.D" ||
                ID == "O9014.D" ||
                ID == "O9021.D")
            {
                if (sp == null)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 84, "操作錯誤"));
                    return;
                }

                //終點Z2
                double Z2;

                if (p.ID == 1 ||
                    p.ID == 2 ||
                    p.ID == 3 ||
                    p.ID == 4 ||
                    p.ID == 5 ||
                    p.ID == 6 ||
                    p.ID == 7 ||
                    p.ID == 8 ||
                    p.ID == 9 ||
                    p.ID == 10)
                {
                    //起點Z1
                    double Z1 = sp.GetArgument("19812").Value;

                    //目前位置
                    double Z = double.Parse(la_EditAbsAxis2Value.Text);

                    //砂輪寬
                    focas.ReadMacro(509, out double GwWidth);


                    if (Z > Z1)
                    {
                        Z2 = Z + GwWidth;
                    }
                    else if (Z < Z1)
                    {
                        Z2 = Z - GwWidth;
                    }
                    else
                    {
                        if (bShowG59)
                        {
                            Z2 = Z - GwWidth;
                        }
                        else
                        {
                            Z2 = Z + GwWidth;
                        }
                    }
                }
                else
                {
                    Z2 = double.Parse(la_EditAbsAxis2Value.Text);
                }


                DGV_Param.CurrentRow.Cells[Col_N_ParamValue.Index].Value = Z2;
                DGV_Param.CurrentRow.Cells[Col_N_Show.Index].Value = Z2;
            }

            //X
            if (ID == "O9012.X" || //X
                ID == "O9013.X") //X
            {
                DGV_Param.CurrentRow.Cells[Col_N_ParamValue.Index].Value = double.Parse(la_EditAbsAxis1Value.Text);
                DGV_Param.CurrentRow.Cells[Col_N_Show.Index].Value = double.Parse(la_EditAbsAxis1Value.Text);
            }

        }
        */


        private void SelectProcess(Object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl == null)
            {
                Fo_Msg.Show("Select Process Error.");
                return;
            }

            //將工序ID存在Tag 
            int.TryParse(ctrl.Tag.ToString(), out int process_id);

            TProcess def_p = Units.ProcessList.FirstOrDefault(x => x.ID == process_id);
            if (def_p == null)
            {
                Fo_Msg.Show("DefaultProcess.xml Error. ID:[" + process_id + "] Not Found.", "");
                return;
            }

            TProcess p = def_p.Clone();


            if (DGV_ProcList.CurrentRow != null)
            {
                if (CreateProcessMode == CreateMode.Insert) //前插入
                {
                    int index = DGV_ProcList.CurrentRow.Index;//目前位置
                    TempProgram.Processes.Insert(index, p);//工序插入到目前位置之前


                    TempExecEnabled.Insert(index, true);//補開關}                        


                    ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)
                    DGV_ProcList.CurrentCell = DGV_ProcList.Rows[index].Cells[0];//選擇剛剛插入的工序
                    ProcessIndex = index;//紀錄為目前正在編輯的工序
                    TempProcess = p;
                    la_EditProgTitle.Text = DGV_ProcList.Rows[index].Cells[1].Value.ToString();
                }
                else if (CreateProcessMode == CreateMode.InsertBack) //後插入
                {
                    int index = DGV_ProcList.CurrentRow.Index + 1;//目前位置後一個位置
                    TempProgram.Processes.Insert(index, p);//工序插入到目前位置之後

                    TempExecEnabled.Insert(index, true);//補開關

                    ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)
                    DGV_ProcList.CurrentCell = DGV_ProcList.Rows[index].Cells[0];//選擇剛剛插入的工序
                    ProcessIndex = index;//紀錄為目前正在編輯的工序
                    TempProcess = p;
                    la_EditProgTitle.Text = DGV_ProcList.Rows[index].Cells[1].Value.ToString();
                }
                else //新增
                {
                    TempProgram.Processes.Add(p);//工序加入到最後面

                    TempExecEnabled.Add(true);//補開關


                    ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)
                    DGV_ProcList.CurrentCell = DGV_ProcList.Rows[TempProgram.Processes.Count - 1].Cells[0];//選擇剛剛插入的工序
                    ProcessIndex = TempProgram.Processes.Count - 1;//紀錄為目前正在編輯的工序
                    TempProcess = p;
                    la_EditProgTitle.Text = DGV_ProcList.Rows[ProcessIndex].Cells[1].Value.ToString();
                }
            }
            else //沒有資料，前插入、後插入、新增，都是用新增的
            {
                TempProgram.Processes.Add(p);//工序加入到最後面

                TempExecEnabled.Add(true);//補開關

                ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)
                DGV_ProcList.CurrentCell = DGV_ProcList.Rows[TempProgram.Processes.Count - 1].Cells[0];//選擇剛剛插入的工序
                ProcessIndex = TempProgram.Processes.Count - 1;//紀錄為目前正在編輯的工序
                TempProcess = p;
                la_EditProgTitle.Text = DGV_ProcList.Rows[ProcessIndex].Cells[1].Value.ToString();
            }

            Col_ProcList_Btn.Visible = false;

            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;

            if (p != null)
            {
                ProcID = p.ID.ToString(); //記錄工序ID
                SetProcessData(p);//編輯畫面顯示此工序
            }
            else
            {
                Fo_Msg.Show("Undifine Process.");
            }


            TC_Main.SelectedTab = tab_EditProc;

            PrevPage.Pop();
            PrevPage.Push(tab_EditProc);
            btn_Prev.Visible = true;

            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
        }

        private void btn_ManualUsePos1_Click(object sender, EventArgs e)
        {
            TB_ManualZStart.Text = la_ManualMachAxis2Value.Text;
        }

        private void btn_ManualUsePos2_Click(object sender, EventArgs e)
        {
            TB_ManualZEnd.Text = la_ManualMachAxis2Value.Text;
        }

        private void uBtn_ManualSave_Click(object sender, EventArgs e)
        {
            if (TB_ManualZStart.Text == "" ||
               TB_ManualZEnd.Text == "" ||
               TB_ManualSpeed.Text == "" ||
               TB_SPSpeed.Text == "")
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 85, "資料不可為空值"));
            }

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("System", "ManualStartZ", TB_ManualZStart.Text);
            ini.WriteString("System", "ManualEndZ", TB_ManualZEnd.Text);
            ini.WriteString("System", "ManualSpeed", TB_ManualSpeed.Text);
            ini.WriteString("System", "SPSpeed", TB_SPSpeed.Text);

            double.TryParse(TB_ManualZStart.Text, out double z1);
            double.TryParse(TB_ManualZEnd.Text, out double z2);
            double.TryParse(TB_ManualSpeed.Text, out double speed);
            double.TryParse(TB_SPSpeed.Text, out double rpm);

            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(940, z1);
                focas.WriteMacro(941, z2);
                focas.WriteMacro(942, speed);
                focas.WriteMacro(657, rpm);
            }));

            /*
            short[] arr = new short[6];
            float[] f = new float[3];
            f[0] = float.Parse(TB_ManualZStart.Text);
            f[1] = float.Parse(TB_ManualZEnd.Text);
            f[2] = float.Parse(TB_ManualSpeed.Text);

            Buffer.BlockCopy(f, 0, arr, 0, 12);

            int ret = deltaCnc1.PLC_WriteValue(PlcDevice.D, 1336, arr, 6);

            //int ret = deltaCnc1.CncInfo.WRITE_PLC_ADDR((uint)PlcDevice.D, 1336, 6, );

            if (ret != (int)ApiCode.SUCCESS)
            {
                Fo_Msg.Show("Write Data Fail.");
            }
            */
        }

        private void uBtn_ManualZero_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            dManualZeroPoint = double.Parse(la_ManualMachAxis1Value.Text);
            ini.WriteFloat("System", "dManualZeroPoint", dManualZeroPoint);
        }

        private void uc_RoundBtn2_Click(object sender, EventArgs e)
        {
            /*
            short[] arr = new short[2];
            int ret;

            //Z軸停秒起點
            arr[0] = short.Parse(TB_D616.Text);
            //Z軸停秒終點
            arr[1] = short.Parse(TB_D617.Text);
            ret = focas.(PlcDevice.D, 616, arr, 2);
            if (ret != (int)ApiCode.SUCCESS)
            {
                Fo_Msg.Show("Write Data Fail.");
            }

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("System", "D616", arr[0]);
            ini.WriteInteger("System", "D617", arr[1]);
             */
        }

        private void btn_ManualStart_Click(object sender, EventArgs e)
        {
            //if (focas.GetInput("MPG"))
            //{
            //deltaCnc1.AddCmd_Bit(PlcDevice.M, 438, PLC_RW_Func.Write, 1, 1);
            /*
            //單節模式
            if (focas.GetInput("SBK"))
            {
                DialogResult ret = Fo_Msg.Show(
                    LanguageManager.LoadMessage(Units.langfile, "Message", 13, "單節模式無法啟動修砂，是否關閉單節？"),
                    LanguageManager.LoadMessage(Units.langfile, "Message", 6, "注意"),
                    MessageBoxButtons.YesNo);
                if (ret != DialogResult.Yes)
                    return;

                //目前是ON，將它OFF
                //deltaCnc1.AddCmd_Bit(PlcDevice.M, 1060, PLC_RW_Func.Write, 0, 1);
            }
            */

            Actions.Enqueue(new Action(() =>
            {
                //手動研磨
                focas.WriteMacro(980, 8);
                OneKeyCall(8999);
            }));

            /*
            if (focas.Vendor == CncVendor.Delta)
            {
                DeltaCnc dc = cnc1.Device as DeltaCnc;
                dc.CallProgram(9998);
            }
            else if (focas.Vendor == CncVendor.Fanuc)
            {
                FanucCnc fc = cnc1.Device as FanucCnc;
                focas.PMC_WriteDbWord(PmcAddrType.D, 1596, 8999);
                focas.PMC_WriteByte(PmcAddrType.E, 4402, 1);//模式：模式切到自動並執行，執行完返回剛剛的模式
                focas.PMC_WriteByte(PmcAddrType.E, 4500, 1);//啟動 ON
                Thread.Sleep(1000);
                focas.PMC_WriteByte(PmcAddrType.E, 4500, 0);//啟動 OFF
            }
            else
            {
                throw new NotImplementedException();
            }
            */

            //}
            //else
            //{
            //Fo_Msg.Show("Please switch to MPG mode.");
            //}
        }


        private void RedoOffsetMouseDown(object sender, MouseEventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;


            if (DGV_Redo.CurrentCell == null)
                return;
            int Row = DGV_Redo.CurrentCell.RowIndex;
            int Col = DGV_Redo.CurrentCell.ColumnIndex;
            if (CurrentProgram == null)
                return;
            if (CurrentProgram.Processes[Row] == null)
                return;

            double.TryParse(btn.DisplayText.ToString(), out double offset);

            if (Col == Col_R_OfsX.Index)
            {
                TOffset ofs = CurrentProgram.Processes[Row].OffsetX;
                ofs.RedoValue += offset;
                if (Math.Round(ofs.RedoValue, 5) > Math.Round(OffsetMax, 5))
                {
                    ofs.RedoValue = OffsetMax;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (Math.Round(ofs.RedoValue, 5) < Math.Round(OffsetMin, 5))
                {
                    ofs.RedoValue = OffsetMin;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                DGV_Redo.CurrentCell.Value = ofs.RedoValue.ToString(Units.DisplayFmt);
            }
            else if (Col == Col_R_OfsZ.Index)
            {
                TOffset ofs = CurrentProgram.Processes[Row].OffsetZ;
                ofs.RedoValue += offset;
                if (Math.Round(ofs.RedoValue, 5) > Math.Round(OffsetMax, 5))
                {
                    ofs.RedoValue = OffsetMax;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (Math.Round(ofs.RedoValue, 5) < Math.Round(OffsetMin, 5))
                {
                    ofs.RedoValue = OffsetMin;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                DGV_Redo.CurrentCell.Value = ofs.RedoValue.ToString(Units.DisplayFmt);
            }

        }

        int RedoClickCount = 0;
        private void uBtn_RedoStart_Click(object sender, EventArgs e)
        {
            if (RedoClickCount > 3)
            {
                ThrRedo.Abort();
                ThrRedo = null;
                RedoClickCount = 0;
            }

            if (ThrRedo != null)
            {
                Fo_Msg.Show("RE-GRIND Running...");
                RedoClickCount++;
                return;
            }

            ThrRedo = new Thread(() =>
            {
                bool bFinish = false;
                Actions.Enqueue((Action)(() =>
                {
                    //重修精磨開啟
                    focas.WriteMacro(966, 1);
                    //那些需要重修
                    for (int i = 0; i < RedoEnabled.Count; i++)
                    {
                        focas.WriteMacro(730 + i, RedoEnabled[i] ? 1 : 0);//重修預設全關                
                    }

                    //寫入 T Code
                    WriteTCode(CurrentProgram);

                    bFinish = true;
                }));

                int iStart = Environment.TickCount;
                while (!bFinish)
                {
                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 5000)
                    {
                        //this.Invoke(new Action(() => { 
                            //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                        //}));
                        return;
                    }
                    Thread.Sleep(50);
                }

                bFinish = false;
                Actions.Enqueue((Action)(() =>
                {
                    OneKeyCall(8000);
                    bFinish = true;
                }));

                while (!bFinish)
                {
                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 5000)
                    {
                        //this.Invoke(new Action(() => { 
                            //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                        //}));
                        ThrRedo = null;
                        return;
                    }
                    Thread.Sleep(50);
                }

                //延遲，先讓程式跑一下
                Thread.Sleep(500);

                //等待 CycleStart 狀態變為ON  
                while (true)
                {
                    //重修精磨結束
                    if (bClose) return;

                    Thread.Sleep(100);

                    if (bCycleStart) break;
                }

                //程式結束 等待CycleStart 狀態變為OFF  
                while (true)
                {
                    if (bClose) return;

                    Thread.Sleep(100);

                    if (!bCycleStart) break;
                }



                this.Invoke((Action)(() =>
                {
                    //重修完成，取消所有工序的重修精磨
                    for (int i = 0; i < RedoEnabled.Count; i++)
                    {
                        RedoEnabled[i] = false;
                        DGV_Redo.Rows[i].Cells[Col_Btn.Index].Value = Properties.Resources.BtnOff;
                    }
                }));

                //清除重修精磨補正值
                for (int i = 0; i < CurrentProgram.Processes.Count; i++)
                {
                    //X軸補正
                    CurrentProgram.Processes[i].OffsetX.RedoValue = 0;
                    //Z軸補正
                    CurrentProgram.Processes[i].OffsetZ.RedoValue = 0;
                    //Y軸補正
                    CurrentProgram.Processes[i].OffsetY.RedoValue = 0;
                }

                Actions.Enqueue(new Action(() =>
                {
                    //寫入
                    WriteTCode(CurrentProgram);

                    //重修精磨結束
                    focas.WriteMacro(966, 0);
                }));


                ThrRedo = null;
            });
            ThrRedo.Start();
        }



        private void uBtn_SaveOffset_Click(object sender, EventArgs e)
        {

            //bWriteTCode = true;
            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                WriteTCode(CurrentProgram);
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

            btn_SaveOffset.Enabled = false;

        }

        private void DGV_Offset_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV_Offset.CurrentCell == null) return;

            if (e.ColumnIndex >= Col_OffsetX.Index && e.ColumnIndex <= Col_OffsetZ.Index)
            {
                btn_Ofs_P01.Enabled = true;
                btn_Ofs_P001.Enabled = true;
                btn_Ofs_P0001.Enabled = true;
                btn_Ofs_N01.Enabled = true;
                btn_Ofs_N001.Enabled = true;
                btn_Ofs_N0001.Enabled = true;
                btn_Offset_Input.Enabled = true;
                btn_Offset_Input2.Enabled = true;

            }
            else if (DGV_Offset.CurrentCell.ColumnIndex == Col_Measure.Index)
            {
                int row = DGV_Offset.CurrentCell.RowIndex;
                String Group = DGV_Offset.Rows[row].Cells[Col_Measure.Index].Value.ToString();
                if (Group == "Group1" || Group == "Group2")
                {
                    btn_Ofs_P01.Enabled = true;
                    btn_Ofs_P001.Enabled = true;
                    btn_Ofs_P0001.Enabled = false;
                    btn_Ofs_N01.Enabled = true;
                    btn_Ofs_N001.Enabled = true;
                    btn_Ofs_N0001.Enabled = false;
                    btn_Offset_Input.Enabled = true;
                    btn_Offset_Input2.Enabled = true;
                }
                else
                {
                    btn_Ofs_P01.Enabled = false;
                    btn_Ofs_P001.Enabled = false;
                    btn_Ofs_P0001.Enabled = false;
                    btn_Ofs_N01.Enabled = false;
                    btn_Ofs_N001.Enabled = false;
                    btn_Ofs_N0001.Enabled = false;
                    btn_Offset_Input.Enabled = false;
                    btn_Offset_Input2.Enabled = false;
                }
            }
            else
            {
                btn_Ofs_P01.Enabled = false;
                btn_Ofs_P001.Enabled = false;
                btn_Ofs_P0001.Enabled = false;
                btn_Ofs_N01.Enabled = false;
                btn_Ofs_N001.Enabled = false;
                btn_Ofs_N0001.Enabled = false;
                btn_Offset_Input.Enabled = false;
                btn_Offset_Input2.Enabled = false;
            }
        }




        private void DGV_Offset_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV_Offset.CurrentCell == null)
                return;
            int Row = DGV_Offset.CurrentCell.RowIndex;
            int Col = DGV_Offset.CurrentCell.ColumnIndex;
            if (CurrentProgram == null)
                return;
            if (CurrentProgram.Processes[Row] == null)
                return;

            if (Col == Col_OffsetX.Index)
            {
                TOffset ofs = CurrentProgram.Processes[Row].OffsetX;
                ofs.Value = Convert.ToDouble(DGV_Offset.CurrentCell.Value.ToString());
            }
            else if (Col == Col_OffsetZ.Index)
            {
                TOffset ofs = CurrentProgram.Processes[Row].OffsetZ;
                ofs.Value = Convert.ToDouble(DGV_Offset.CurrentCell.Value.ToString());
            }

        }

        private void pic_FuncSwitch_Click(object sender, EventArgs e)
        {
            //if (focas.ReadMacro(655, out double val) == SUCCESS)
            //{
            //    int flag = (int)Math.Round(val);
            //    if (flag == 0)
            //    {
            //        pic_SW_CloseCoolt.Image = Properties.Resources.SwitchOff;
            //        bProgramEndStopWater = false;
            //    }
            //    else
            //    {
            //        pic_SW_CloseCoolt.Image = Properties.Resources.SwitchOn;
            //        bProgramEndStopWater = true;
            //    }
            //}

            TC_Main.SelectedTab = tab_FuncSwitch;

            PrevPage.Push(tab_FuncSwitch);
            btn_Prev.Visible = true;
        }

        private void pic_Language_Click(object sender, EventArgs e)
        {
            TC_Main.SelectedTab = tab_Language;
            PrevPage.Push(tab_Language);
            btn_Prev.Visible = true;

            //目前的語言
            //TIniFile ini = new TIniFile(Application.StartupPath + "\\Language\\" + Units.LangCode + "\\font.ini");


            string languagePath = Application.StartupPath + "\\Language";
            if (!Directory.Exists(languagePath)) return;


            dgv_Language.Rows.Clear();

            // 取得 Language 資料夾下的所有語言(資料夾)
            string[] subDirectories = Directory.GetDirectories(languagePath);
            foreach (string dir in subDirectories)
            {
                string folderName = Path.GetFileName(dir); // 資料夾名稱
                //讀取語言設定檔
                TIniFile lang_ini = new TIniFile(Application.StartupPath + "\\Language\\" + folderName + "\\font.ini");

                if (folderName == Units.LangCode) la_Current_Language_Val.Text = lang_ini.ReadString("lang", "lang", Units.LangCode);

                string filename = Application.StartupPath + "\\Language\\" + folderName + "\\img.png";
                Bitmap bmp = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage(bmp);
                if (File.Exists(filename)) g.DrawImage(Image.FromFile(filename), 0, 0, 100, 100);
                g.Dispose();
                dgv_Language.Rows.Add(lang_ini.ReadString("lang", "lang", folderName), folderName, bmp); // 預設將未知資料夾名稱直接顯示

            }



        }
        private void btn_ParamPageClick(object sender, MouseEventArgs e) //砂輪1參數 / 砂輪2參數 / 選配參數
        {
            foreach (object obj in pa_ParamBtn.Controls)
            {
                Uc_RoundBtn b = obj as Uc_RoundBtn;
                if (b == null) continue;

                b.Lamp = b == sender;
            }

            dgv_MP_Param.Rows.Clear();
            Uc_RoundBtn btn = sender as Uc_RoundBtn;
            if (btn == null) return;

            //目前要切換的頁面
            String CurrentPage = btn.Tag.ToString();

            //取得根元素(OptionParam)
            XmlNode root_x = ParamXmlDoc.DocumentElement;
            //開始解析XML檔
            foreach (XmlElement xPage in root_x.ChildNodes)
            {
                if (xPage.Name != "Page") continue;//例外處理
                string PageName = xPage.GetAttribute("Name");
                if (PageName == "" || PageName != CurrentPage) continue;//例外處理 + 過濾

                foreach (XmlElement x in xPage.ChildNodes)
                {
                    //子節點 (Param) 
                    if (x.Name != "Param") continue;//例外處理

                    //從語言檔讀取參數名稱
                    string Name = x.GetAttribute("Text");

                    //變數種類
                    string Type = x.GetAttribute("Type");
                    int.TryParse(Type, out int type);
                    //變數位址
                    string Addr = x.GetAttribute("Addr");
                    //最大值
                    string sMax = x.GetAttribute("Max");
                    //最小值
                    string sMin = x.GetAttribute("Min");
                    //預設值
                    //string sDefault = x.GetAttribute("Default");
                    //是否顯示
                    bool bVisible = x.GetAttribute("Visible") == "1";
                    if (!bVisible) continue;

                    //格式化
                    string sFmt = x.GetAttribute("Format");

                    //單位
                    string sUnit = x.GetAttribute("Unit");
                    if (bInchTrans)
                    {
                        if (sUnit == "mm")
                        {
                            //1 mm = 0.03937 inch 讓數字好看 直接乘以0.04 或 除以25
                            sUnit = "inch";
                            double.TryParse(sMax, out double max);
                            double.TryParse(sMin, out double min);
                            sMax = (max / 25).ToString(Units.DisplayFmt);
                            sMin = (min / 25).ToString(Units.DisplayFmt);
                        }
                        else if (sUnit == "mm/s")
                        {
                            //1 mm = 0.03937 inch 讓數字好看 直接乘以0.04 或 除以25
                            sUnit = "inch/s";
                            double.TryParse(sMax, out double max);
                            double.TryParse(sMin, out double min);
                            sMax = (max / 25).ToString(Units.DisplayFmt);
                            sMin = (min / 25).ToString(Units.DisplayFmt);
                        }
                        else if (sUnit == "mm/min")
                        {
                            //1 mm = 0.03937 inch 讓數字好看 直接乘以0.04 或 除以25
                            sUnit = "inch/min";
                            double.TryParse(sMax, out double max);
                            double.TryParse(sMin, out double min);
                            sMax = (max / 25).ToString(Units.DisplayFmt);
                            sMin = (min / 25).ToString(Units.DisplayFmt);
                        }
                        else if (sUnit == "m/s")
                        {
                            //1 m/s = 196.8504 f/m 讓數字好看 直接乘以200
                            double.TryParse(sMax, out double max);
                            double.TryParse(sMin, out double min);
                            sMax = (max * 200).ToString("0");
                            sMin = (min * 200).ToString("0");
                            sUnit = "sfm";
                        }
                    }
                    //數值
                    double.TryParse(x.GetAttribute("Value"), out double value);

                    //顯示文字 (下拉式選單 / 數值字串)
                    string show_val = XmlToString(x, sFmt);

                    //加入到dataGridView中(順便調整高度)
                    int index = dgv_MP_Param.Rows.Add(Name, type, Addr, show_val, value, GetVal2Txt(x), sMax, sMin, sUnit, x);
                    dgv_MP_Param.Rows[index].Height = 30;

                }
            }
            if (dgv_MP_Param.Rows.Count > 0)
            {
                dgv_MP_Param_CellClick(dgv_MP_Param, null);
            }
        }

        private string XmlToString(XmlElement x, string format) //有下拉式選單就顯示文字, 沒有就顯示數值
        {
            double.TryParse(x.GetAttribute("Value"), out double value);
            string text = value.ToString(format); //目前數值

            int Value = (int)Math.Round(value);
            //int.TryParse(text, out int Value);
            //此Param 還有子節點，表示此參數是 下拉式選單(ComboBox)
            for (int j = 0; j < x.ChildNodes.Count; j++)
            {
                XmlElement child = (XmlElement)x.ChildNodes[j];
                if (child.Name != "Value") continue;//例外處理

                //定義數值
                int.TryParse(child.GetAttribute("Value"), out int DefValue);
                if (DefValue != Value) continue;//往下個子節點比對

                //顯示的文字
                text = child.GetAttribute("Text");
            }
            return text;
        }

        private Dictionary<int, string> GetVal2Txt(XmlElement x)
        {
            //建立需要轉成字串的結構
            Dictionary<int, string> Val2Txt = new Dictionary<int, string>();

            //此Param 還有子節點，表示此參數是 下拉式選單(ComboBox)
            for (int j = 0; j < x.ChildNodes.Count; j++)
            {
                XmlElement child = (XmlElement)x.ChildNodes[j];
                if (child.Name != "Value") continue;//例外處理

                if (child.GetAttribute("Visible") == "0") continue;//隱藏

                //定義數值
                int.TryParse(child.GetAttribute("Value"), out int DefValue);

                //定義的文字
                String DefText = child.GetAttribute("Text");
                Val2Txt.Add(DefValue, DefText);
            }

            return Val2Txt;
        }


        Dictionary<string, Uc_RoundBtn> SpBtnDic = null;
        private void pic_SP_Click(object sender, EventArgs e) //按下 加工維護
        {
            un_ProcessParam.btn_Memory.Font = new Font("微軟正黑體", 12f, (FontStyle)0);

            bool bCreateBtn = false;
            if (SpBtnDic == null)
            {
                bCreateBtn = true;
                SpBtnDic = new Dictionary<string, Uc_RoundBtn>();
            }

            dgv_MP_Param.Rows.Clear();

            pa_ParamMsg.Visible = false;//隱藏警示訊息
            Application.DoEvents();

            //全部讀取 (全部會放在 CurrentMacro)
            //if (ReadAllMacro() != SUCCESS) return;

            string filename = Application.StartupPath + "\\OptionParam.xml";
            if (!File.Exists(filename))
            {
                Fo_Msg.Show("OptionParam.xml " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
                return;
            }

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");

            TIniFile lang_ini = new TIniFile(Units.langfile);

            //左側Button 建立索引值
            int index = 0;

            Actions.Enqueue(new Action(() =>
            {

                ParamXmlDoc.Load(filename);
                //取得根元素(OptionParam)
                XmlNode root_x = ParamXmlDoc.DocumentElement;
                //開始解析XML檔
                foreach (XmlElement xPage in root_x.ChildNodes)
                {
                    //子節點 (Page)                 
                    if (xPage.Name != "Page") continue;

                    string PageName = xPage.GetAttribute("Name");
                    if (PageName == "") continue;//例外處理

                    bool PageVisible = xPage.GetAttribute("Visible") == "1";
                    if (!PageVisible) continue;//不顯示

                    this.Invoke(new Action(() =>
                    {

                        if (bCreateBtn && !SpBtnDic.ContainsKey(PageName))
                        {


                            Uc_RoundBtn btn = new Uc_RoundBtn();

                            btn.Top = 48 * index;
                            index++;

                            btn.Left = 0;
                            btn.Width = 152;
                            btn.Height = 48;
                            btn.Parent = pa_ParamBtn;
                            btn.Tag = PageName;
                            btn.MouseDownImage = Properties.Resources.Btn_S4_136x60_BL;
                            btn.MouseUpImage = Properties.Resources.Btn_S4_136x60_B;
                            btn.LampOnImage = Properties.Resources.Btn_S4_136x60_G;
                            btn.MouseDown += btn_ParamPageClick;
                            btn.DisplayText = lang_ini.ReadString("MaintainParam", PageName, "Param" + index);

                            SpBtnDic.Add(PageName, btn);
                        }

                    }));

                    foreach (XmlElement x in xPage.ChildNodes)
                    {
                        //此Param 的語言編號

                        //從語言檔讀取參數名稱


                        //變數種類
                        string Type = x.GetAttribute("Type");
                        int.TryParse(Type, out int type);
                        //變數位址
                        string Addr = x.GetAttribute("Addr");
                        string name = lang_ini.ReadString("MaintainParam", Addr, "");
                        x.SetAttribute("Text", name);

                        //最大值
                        string sMax = x.GetAttribute("Max");
                        //最小值
                        string sMin = x.GetAttribute("Min");
                        //預設值
                        string sDefault = x.GetAttribute("Default");
                        //是否顯示
                        bool bVisible = x.GetAttribute("Visible") == "1";

                        //單位
                        string sUnit = x.GetAttribute("Unit");
                        if (bInchTrans) //英制
                        {
                            if (sUnit == "mm")
                            {
                                //1 mm = 0.03937 inch 讓數字好看 直接乘以0.04 或 除以25
                                sUnit = "inch";
                                double.TryParse(sMax, out double max);
                                double.TryParse(sMin, out double min);
                                double.TryParse(sDefault, out double def);
                                sMax = (max / 25).ToString(Units.DisplayFmt);
                                sMin = (min / 25).ToString(Units.DisplayFmt);
                                if (sDefault != "") sDefault = (def / 25).ToString();
                            }
                            else if (sUnit == "mm/s")
                            {
                                //1 mm = 0.03937 inch 讓數字好看 直接乘以0.04 或 除以25
                                sUnit = "inch/s";
                                double.TryParse(sMax, out double max);
                                double.TryParse(sMin, out double min);
                                double.TryParse(sDefault, out double def);
                                sMax = (max / 25).ToString(Units.DisplayFmt);
                                sMin = (min / 25).ToString(Units.DisplayFmt);
                                if (sDefault != "") sDefault = (def / 25).ToString();
                            }
                            else if (sUnit == "mm/min")
                            {
                                //1 mm = 0.03937 inch 讓數字好看 直接乘以0.04 或 除以25
                                sUnit = "inch/min";
                                double.TryParse(sMax, out double max);
                                double.TryParse(sMin, out double min);
                                double.TryParse(sDefault, out double def);
                                sMax = (max / 25).ToString(Units.DisplayFmt);
                                sMin = (min / 25).ToString(Units.DisplayFmt);
                                if (sDefault != "") sDefault = (def / 25).ToString();
                            }
                            else if (sUnit == "m/s")
                            {
                                //1 m/s = 196.8504 f/m 讓數字好看 直接乘以200
                                double.TryParse(sMax, out double max);
                                double.TryParse(sMin, out double min);
                                double.TryParse(sDefault, out double def);
                                sMax = (max * 200).ToString("0");
                                sMin = (min * 200).ToString("0");
                                if (sDefault != "") sDefault = (def * 200).ToString();
                                sUnit = "sfm";
                            }
                        }


                        //此Param 還有子節點，表示此參數有字串選單(ComboBox)
                        for (int j = 0; j < x.ChildNodes.Count; j++)
                        {
                            XmlElement child = (XmlElement)x.ChildNodes[j];

                            //數值
                            string XVal = child.GetAttribute("Value");
                            int.TryParse(XVal, out int xval);

                            //文字編號(語言檔)
                            string XNo = child.GetAttribute("TextNo");
                            int.TryParse(XNo, out int xno);

                            //讀取多國語言文字
                            string XText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", xno, "");

                            //寫到文字屬性
                            child.SetAttribute("Text", XText);
                        }

                        //變數類型為MACRO
                        if (type == 1) //Macro
                        {
                            //位址為純數字 500-999
                            int.TryParse(Addr, out int addr);

                            focas.ReadMacro(addr, out double val);

                            //存入Value屬性
                            x.SetAttribute("Value", val.ToString());

                            //小數位定義
                            if (x.GetAttribute("Format") == "") x.SetAttribute("Format", Units.DisplayFmt);
                        }
                        else if (type == 2) //PMC Byte
                        {
                            //判斷變數種類
                            PmcAddrType pmcType = PmcAddrType.E;
                            if (Addr[0] == 'K') pmcType = PmcAddrType.K;
                            else if (Addr[0] == 'E') pmcType = PmcAddrType.E;
                            else if (Addr[0] == 'D') pmcType = PmcAddrType.D;
                            //有小數點的表示只寫1個bit
                            if (Addr.Contains('.'))//Bit
                            {
                                //取得小數點索引值
                                int pos = Addr.IndexOf('.');
                                //取得暫存器位址 跟要寫入的位元
                                int.TryParse(Addr.Substring(1, Addr.Length - 3), out int pmc_addr);
                                int.TryParse(Addr.Substring(pos + 1), out int pmc_bit);

                                //取得暫存器數值
                                focas.PMC_ReadByte(pmcType, (ushort)pmc_addr, out byte tmpPMC_Byte);
                                int bit_val = tmpPMC_Byte.GetBit(pmc_bit) ? 1 : 0;
                                //存入Value屬性
                                x.SetAttribute("Value", bit_val.ToString());

                                //小數位定義
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0");
                            }
                            else //Byte
                            {
                                //取得暫存器位址
                                int.TryParse(Addr.Substring(1), out int pmc_addr);

                                //取得暫存器數值
                                focas.PMC_ReadByte(pmcType, (ushort)pmc_addr, out byte tmpPMC_Byte);
                                //存入Value屬性
                                x.SetAttribute("Value", tmpPMC_Byte.ToString());

                                //小數位定義
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0");
                            }
                        }
                        else if (type == 3) //PMC WORD
                        {
                            //判斷變數種類
                            PmcAddrType pmcType = PmcAddrType.E;
                            if (Addr[0] == 'K') pmcType = PmcAddrType.K;
                            else if (Addr[0] == 'E') pmcType = PmcAddrType.E;
                            else if (Addr[0] == 'D') pmcType = PmcAddrType.D;

                            //取得暫存器位址
                            int.TryParse(Addr.Substring(1), out int pmc_addr);

                            //取得暫存器數值
                            focas.PMC_ReadWord(pmcType, (ushort)pmc_addr, out ushort tmpPMC_Word);
                            //存入Value屬性
                            x.SetAttribute("Value", tmpPMC_Word.ToString());

                            //小數位定義
                            if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0");
                        }
                        else if (type == 4) //PMC DWORD
                        {
                            //判斷變數種類
                            PmcAddrType pmcType = PmcAddrType.E;
                            if (Addr[0] == 'K') pmcType = PmcAddrType.K;
                            else if (Addr[0] == 'E') pmcType = PmcAddrType.E;
                            else if (Addr[0] == 'D') pmcType = PmcAddrType.D;

                            //取得暫存器位址
                            int.TryParse(Addr.Substring(1), out int pmc_addr);

                            //取得暫存器數值
                            focas.PMC_ReadDbWord(pmcType, (ushort)pmc_addr, out uint tmpPMC_DWORD);
                            //存入Value屬性
                            x.SetAttribute("Value", tmpPMC_DWORD.ToString());

                            //小數位定義
                            if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0");
                        }
                        else if (type == 5) //PC
                        {
                            double value = 0;
                            if (Addr == "OffsetMax") //補正上限
                            {
                                value = OffsetMax;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", Units.DisplayFmt); //小數位定義
                            }
                            else if (Addr == "OffsetMin") //補正下限
                            {
                                value = OffsetMin;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", Units.DisplayFmt); //小數位定義
                            }
                            else if (Addr == "GW1_MaxRPM") //砂輪1 最大轉速(倍率用)
                            {
                                value = ini.ReadInteger("Gw1", "NowRPM", 1600);
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0"); //小數位定義
                            }
                            else if (Addr == "GW1_MaxHz") //砂輪1 最大頻率(預留)
                            {
                                double.TryParse(sDefault, out value);
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0"); //小數位定義
                            }
                            else if (Addr == "GW2_MaxRPM") //砂輪2 最大轉速(倍率用)
                            {
                                value = ini.ReadInteger("Gw2", "NowRPM", 1600);
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0"); //小數位定義
                            }
                            else if (Addr == "GW2_MaxHz") //砂輪2 最大頻率(預留)
                            {
                                double.TryParse(sDefault, out value);
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0"); //小數位定義
                            }
                            else if (Addr == "GW1_Grind_GAP") //砂輪1 研磨中 間隙消除
                            {
                                value = GW1_Grind_GAP;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW1_Grind_CRASH") //砂輪1 研磨中 電流防撞
                            {
                                value = GW1_Grind_CRASH;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW1_Dress_GAP") //砂輪1 修整中 間隙消除
                            {
                                value = GW1_Dress_GAP;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW1_Dress_CRASH") //砂輪1 修整中 電流防撞
                            {
                                value = GW1_Dress_CRASH;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW2_Grind_GAP") //砂輪2 研磨中 間隙消除
                            {
                                value = GW2_Grind_GAP;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW2_Grind_CRASH") //砂輪2 研磨中 電流防撞
                            {
                                value = GW2_Grind_CRASH;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW2_Dress_GAP") //砂輪2 修整中 間隙消除
                            {
                                value = GW2_Dress_GAP;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            else if (Addr == "GW2_Dress_CRASH") //砂輪2 修整中 電流防撞
                            {
                                value = GW2_Dress_CRASH;
                                if (x.GetAttribute("Format") == "") x.SetAttribute("Format", "0.0"); //小數位定義
                            }
                            //存入Value屬性
                            x.SetAttribute("Value", value.ToString());
                        }
                    }
                }

                this.Invoke(new Action(() =>
                {

                    if (SpBtnDic == null) return;
                    Uc_RoundBtn btn1 = null;
                    btn1 = SpBtnDic.Values.First();
                    if (btn1 != null) btn_ParamPageClick(btn1, null);//MouseDown Event
                }));
            }));

            //TempMaintanceTab = tab_ProcessParam;
            TC_Main.SelectedTab = tab_ProcessParam;
            PrevPage.Push(tab_ProcessParam);
            btn_Prev.Visible = true;


        }


        private void SetLanguage(string lang)
        {
            //focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("System", "Language", lang);
            Units.langfile = Application.StartupPath + "\\Language\\" + lang + "\\" + lang + ".txt";
            this.LoadLanguageFile(Units.langfile, "Fo_Main");

            TIniFile langini = new TIniFile(Application.StartupPath + "\\Language\\" + lang + "\\font.ini");
            la_Current_Language_Val.Text = langini.ReadString("lang", "lang", Units.LangCode);

            la_Loading.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 134, "Loading...");

            //加工對點
            String fontName = langini.ReadString("la_Parts", "FontName", "Times New Roman");
            float fontSize = (float)langini.ReadFloat("la_Parts", "FontSize", 12);
            int fontStyle = langini.ReadInteger("la_Parts", "FontStyle", 1);
            la_Parts.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //修砂對點
            fontName = langini.ReadString("la_DressGW", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("la_DressGW", "FontSize", 12);
            fontStyle = langini.ReadInteger("la_DressGW", "FontStyle", 1);
            la_DressGW.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //砂輪
            fontName = langini.ReadString("GWData", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("GWData", "FontSize", 12);
            fontStyle = langini.ReadInteger("GWData", "FontStyle", 1);
            la_GW.Font = new Font(fontName, la_GW.Font.Size, la_GW.Font.Style);
            la_GWDiameter.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_GWMinDiameter.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_GWWidth.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_GWMinWidth.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_GWHL.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_GWDressTimes.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_DryRun.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);


            //編輯工序上方按鍵
            fontName = langini.ReadString("EditProcBtn", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("EditProcBtn", "FontSize", 12);
            fontStyle = langini.ReadInteger("EditProcBtn", "FontStyle", 1);
            btn_ArgParam.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ArgParam2.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ArgParam3.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ArgAdvance.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ArgDress1.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ArgDress2.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //編輯工序
            fontName = langini.ReadString("DGV", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("DGV", "FontSize", 14);
            fontStyle = langini.ReadInteger("DGV", "FontStyle", 1);
            Col_Param1_Name.CellTemplate.Style.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            Col_Param2_Name.CellTemplate.Style.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            Col_Param3_Name.CellTemplate.Style.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            Col_Advance_Name.CellTemplate.Style.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            Col_Dress1_Name.CellTemplate.Style.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            Col_Dress2_Name.CellTemplate.Style.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //砂輪
            DGV_GwParam.RowsDefaultCellStyle.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            //程式
            DGV_ProcList.RowsDefaultCellStyle.Font = new Font(fontName, 18);

            //對點大按鍵
            fontName = langini.ReadString("LargeBtn", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("LargeBtn", "FontSize", 16);
            fontStyle = langini.ReadInteger("EditProgDGV", "FontStyle", 0);
            btn_DG_Btn1.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_DG_Btn2.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_DP_Btn1.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_DP_Btn2.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_DP_Btn3.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //維護
            fontName = langini.ReadString("MaintainLabel", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("MaintainLabel", "FontSize", 12);
            fontStyle = langini.ReadInteger("MaintainLabel", "FontStyle", 0);
            la_MaintainLanguage.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);  //語言
            la_MaintainSP.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);        //加工參數
            la_ScreenDisplay.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);     //ScreenDisplay
            la_ImportProg.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);        //匯入程式
            la_CNCDataManager.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);    //CNC資料管理
            la_Position.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);          //位置設定
            la_Warmup.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);            //暖機
            la_MaintainBalance.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);   //砂輪手動平衡
            la_RunSpindle.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);        //高速主軸跑合
                                                                                            //la_AutoDoorSetting.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
                                                                                            //la_Manual.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Monitor_Door.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);      //自動門

            //小按鍵
            fontName = langini.ReadString("BottomBtn", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("BottomBtn", "FontSize", 12);
            fontStyle = langini.ReadInteger("BottomBtn", "FontStyle", 0);
            //下方區塊
            btn_Regist.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Monitor.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Program.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Message.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Maintenance.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_MeasureList.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ToolSelect.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Monitor_ToChgPos2.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_DressGw1.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Redo.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Offset.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_SoftPanel.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            //登錄 - 砂輪
            btn_Gw_GwData.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Gw_ShapeSelect.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Gw_ShapeData.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Gw_DressCondition.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            //btn_RegisterGw_Save.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            //維護 - 加工參數
            uBtn_UnitChange.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            uBtn_Default.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            uBtn_ProcessParam.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            //程式 - 指令(GM-Code)
            btn_AddLine.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_InsertLine.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_MoveUp.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_MoveDown.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ClearLine.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ClearAllLine.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_keyboard.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_changeenter.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            //程式
            btn_EditProc.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_AddProc.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_InsertProcFront.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_InsertProcBack.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_RemoveProc.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_Copy.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_SaveProg.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            btn_ProgList.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //工序選擇
            fontName = langini.ReadString("ProcessSelect", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("ProcessSelect", "FontSize", 12);
            fontStyle = langini.ReadInteger("ProcessSelect", "FontStyle", 0);
            la_Process1.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process2.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process3.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process4.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process5.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process6.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process7.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process8.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process9.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process10.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process11.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process12.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process13.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process14.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process15.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process16.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process17.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process18.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process19.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            la_Process20.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            // 位置設定
            fontName = langini.ReadString("la_IDOD", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("la_IDOD", "FontSize", 10);
            fontStyle = langini.ReadInteger("la_IDOD", "FontStyle", 0);
            GB_ODParam.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);

            //數字鍵盤 訊息
            fontName = langini.ReadString("UserNum", "FontName", "Times New Roman");
            fontSize = (float)langini.ReadFloat("UserNum", "FontSize", 12);
            fontStyle = langini.ReadInteger("UserNum", "FontStyle", 0);
            uc_UserNumSetGW.la_Msg.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);
            uc_UserNumEditProc.la_Msg.Font = new Font(fontName, fontSize, (FontStyle)fontStyle);


            //ReadDressToolSetting();
            LoadLanguage(lang);
            LoadProcessDbName();//預設工序庫讀取名稱 (DefaultProcessLang.xml -> DefaultProcess.xml)
            LoadProgramDB();

            //公制 -> 英制
            if (bInchTrans)
            {
                foreach (TProcess p in Units.ProcessList)
                {
                    foreach (TSubProgram sp in p.SubPrograms)
                    {
                        foreach (TArgument a in sp.Arguments)
                        {
                            if (a.Unit.Contains("mm"))
                            {
                                a.Unit = a.Unit.Replace("mm", "inch");
                            }
                        }
                    }
                }

                foreach (TProgram pg in Units.ProgramDB.Programs)
                {
                    foreach (TProcess p in pg.Processes)
                    {
                        TProcess def_p = null;
                        foreach (TProcess p2 in Units.ProcessList)
                        {
                            if (p2.ID == p.ID)
                            {
                                def_p = p2;
                                break;
                            }
                        }
                        if (def_p == null) continue;

                        foreach (TSubProgram sp in p.SubPrograms)
                        {
                            TSubProgram def_sp = null;
                            foreach (TSubProgram sp2 in def_p.SubPrograms)
                            {
                                if (sp2.ProgNo == sp.ProgNo)
                                {
                                    def_sp = sp2;
                                    break;
                                }
                            }
                            if (def_sp == null) continue;

                            foreach (TArgument a in sp.Arguments)
                            {
                                TArgument def_a = null;
                                foreach (TArgument a2 in def_sp.Arguments)
                                {
                                    if (a2.AddrCode == a.AddrCode)
                                    {
                                        def_a = a2;
                                        break;
                                    }
                                }
                                if (def_a == null) continue;

                                if (a.Unit.Contains("mm"))
                                {
                                    a.Unit = a.Unit.Replace("mm", "inch");
                                }
                            }
                        }
                    }
                }
            }
            else
            {

            }

            foreach (TProgram pg in Units.ProgramDB.Programs)
            {
                if (pg.Name == MainProgram)
                {
                    OpenProgram(pg, false);//語言切換，重新讀檔(工序清單,補正值)
                    RefleshProgramLayout();//畫面重新整理
                    break;
                }
            }

            CurrentAlarm.Items.Clear();
        }


        //private void LoadGW1SpeedLayout()//砂輪恆速功能??
        //{
        //    //砂輪恆速功能
        //    focas.ReadMacro(626, out double Macro626);
        //    bRPM_Mode = Macro626 == 0;
        //}

        private void uBtn_Default_Click(object sender, EventArgs e)
        {
            DialogResult ret = Fo_Msg.Show(
                 LanguageManager.LoadMessage(Units.langfile, "Message", 15, "是否要回復到預設值?"),
                 LanguageManager.LoadMessage(Units.langfile, "Message", 6, "注意"),
                 MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
                return;

            XmlNode root_x = ParamXmlDoc.DocumentElement;
            //開始解析XML檔
            foreach (XmlElement xPage in root_x.ChildNodes)
            {
                if (xPage.Name != "Page") continue; //例外處理
                string PageName = xPage.GetAttribute("Name");
                if (PageName == "") continue;//例外處理

                foreach (XmlElement x in xPage.ChildNodes)
                {
                    //子節點 (Param) 
                    if (x.Name != "Param") continue;

                    string strDef = x.GetAttribute("Default");

                    if (string.IsNullOrEmpty(strDef)) continue;//沒有這個屬性就不回預設值

                    double.TryParse(strDef, out double defValue);
                    string sUnit = x.GetAttribute("Unit");
                    if (bInchTrans && sUnit.Contains("mm")) defValue = defValue / 25;
                    x.SetAttribute("Value", defValue.ToString());
                }
            }

            foreach (object obj in pa_ParamBtn.Controls)
            {
                Uc_RoundBtn btn = obj as Uc_RoundBtn;
                if (btn == null) continue;

                if (btn.Lamp)
                {
                    btn_ParamPageClick(btn, null);
                    break;
                }
            }
        }

        //加工維護儲存
        private void btn_ProcessParamSave_Click(object sender, EventArgs e)
        {
            Actions.Enqueue(new Action(() =>
            {

                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                bool MSMode = ini.ReadInteger("System", "MSMode", 1) == 1; // 0:M/min, 1:M/sec

                XmlNode root_x = ParamXmlDoc.DocumentElement;
                //開始解析XML檔
                foreach (XmlElement xPage in root_x.ChildNodes)
                {
                    if (xPage.Name != "Page") continue;//例外處理
                    string PageName = xPage.GetAttribute("Name");
                    if (PageName == "") continue;

                    foreach (XmlElement x in xPage.ChildNodes)
                    {
                        //子節點 (Param)                     
                        if (x.Name != "Param") continue;

                        //變數種類
                        string Type = x.GetAttribute("Type");
                        int.TryParse(Type, out int type);
                        //變數位址
                        string Addr = x.GetAttribute("Addr");
                        //數值
                        string sValue = x.GetAttribute("Value");

                        if (type == 1)//Macro
                        {
                            int.TryParse(Addr, out int addr);
                            double.TryParse(sValue, out double value);
                            focas.WriteMacro(addr, value);
                        }
                        else if (type == 2)//PMC Byte
                        {
                            //判斷變數種類
                            PmcAddrType pmcType = PmcAddrType.E;
                            if (Addr[0] == 'K') pmcType = PmcAddrType.K;
                            else if (Addr[0] == 'E') pmcType = PmcAddrType.E;
                            else if (Addr[0] == 'D') pmcType = PmcAddrType.D;

                            //有小數點的表示只寫1個bit
                            if (Addr.Contains('.'))//Bit
                            {
                                //取得小數點索引值
                                int pos = Addr.IndexOf('.');
                                //取得暫存器位址 跟要寫入的位元
                                int.TryParse(Addr.Substring(1, Addr.Length - 3), out int pmc_addr);
                                int.TryParse(Addr.Substring(pos + 1), out int pmc_bit);

                                //取得暫存器數值(只寫某個Bit 因此其他bit要讀回來
                                focas.PMC_ReadByte(pmcType, (ushort)pmc_addr, out byte tmpPMC_Byte);
                                int.TryParse(sValue, out int value);
                                focas.PMC_WriteByte(pmcType, (short)pmc_addr, tmpPMC_Byte.SetBit(pmc_bit, value == 1 ? true : false));
                            }
                            else //Byte
                            {
                                //取得暫存器位址
                                int.TryParse(Addr.Substring(1), out int pmc_addr);
                                int.TryParse(sValue, out int value);
                                focas.PMC_WriteByte(pmcType, (short)pmc_addr, (byte)value);
                            }
                        }
                        else if (type == 3) //PMC WORD
                        {
                            //判斷變數種類
                            PmcAddrType pmcType = PmcAddrType.E;
                            if (Addr[0] == 'K') pmcType = PmcAddrType.K;
                            else if (Addr[0] == 'E') pmcType = PmcAddrType.E;
                            else if (Addr[0] == 'D') pmcType = PmcAddrType.D;

                            //取得暫存器位址
                            int.TryParse(Addr.Substring(1), out int pmc_addr);
                            int.TryParse(sValue, out int value);
                            focas.PMC_WriteWord(pmcType, (short)pmc_addr, (short)value);
                        }
                        else if (type == 4) //PMC DWORD
                        {
                            //判斷變數種類
                            PmcAddrType pmcType = PmcAddrType.E;
                            if (Addr[0] == 'K') pmcType = PmcAddrType.K;
                            else if (Addr[0] == 'E') pmcType = PmcAddrType.E;
                            else if (Addr[0] == 'D') pmcType = PmcAddrType.D;

                            //取得暫存器位址
                            int.TryParse(Addr.Substring(1), out int pmc_addr);
                            int.TryParse(sValue, out int value);
                            focas.PMC_WriteDbWord(pmcType, (short)pmc_addr, value);
                        }
                        else if (type == 5) // PC
                        {
                            if (Addr == "OffsetMax")
                            {
                                double.TryParse(sValue, out double value);
                                OffsetMax = value;
                                ini.WriteFloat("Parameter", "OffsetMax", OffsetMax);
                            }
                            else if (Addr == "OffsetMin")
                            {
                                double.TryParse(sValue, out double value);
                                OffsetMin = value;
                                ini.WriteFloat("Parameter", "OffsetMin", OffsetMin);
                            }
                            else if (Addr == "GW1_MaxRPM")//最大轉速(倍率用)
                            {
                                int.TryParse(sValue, out int value);
                                double gwHz = ini.ReadFloat("Gw1", "Hz", 60);
                                int gwRPM = ini.ReadInteger("Gw1", "NowRPM", 1600);//最大轉速(倍率用)
                                if (gwRPM != value)
                                {
                                    Gw1.Rate = value / gwHz; //     Rate  = Rmp / Hz
                                    ini.WriteFloat("Gw1", "Rate", Gw1.Rate);
                                    ini.WriteInteger("Gw1", "NowRPM", value);//最大轉速(倍率用)
                                    ini.WriteInteger("Gw1", "MaxRPM", value);//軟體上限 跟著改
                                    Gw1.MaxRpm = value;
                                }
                            }
                            else if (Addr == "GW1_MaxHz") //尚未開放
                            {
                                int.TryParse(sValue, out int value);
                                ini.WriteInteger("Gw1", "MaxHz", value);
                            }
                            else if (Addr == "GW2_MaxRPM")//最大轉速(倍率用)
                            {
                                int.TryParse(sValue, out int value);
                                double gwHz = ini.ReadFloat("Gw2", "Hz", 60);//最大頻率(倍率用)
                                int gwRPM = ini.ReadInteger("Gw2", "NowRPM", 1600);//最大轉速(倍率用)
                                if (gwRPM != value)
                                {
                                    Gw2.Rate = value / gwHz; //     Rate  = Rmp / Hz
                                    ini.WriteFloat("Gw2", "Rate", Gw2.Rate);
                                    ini.WriteInteger("Gw2", "NowRPM", value);//最大轉速(倍率用)
                                    ini.WriteInteger("Gw2", "MaxRpm", value);//軟體上限 跟著改
                                    Gw2.MaxRpm = value;
                                }
                            }
                            else if (Addr == "GW2_MaxHz") //尚未開放
                            {
                                int.TryParse(sValue, out int value);
                                ini.WriteInteger("Gw2", "MaxHz", value);
                            }
                            else if (Addr == "GW1_Grind_GAP") //GW1 GRIND GAP
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW1_Grind_GAP", value);
                                GW1_Grind_GAP = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW1_Grind_CRASH") //GW1 GRIND CRASH
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW1_Grind_CRASH", value);
                                GW1_Grind_CRASH = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW2_Grind_GAP") //GW2 GRIND GAP
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW2_Grind_GAP", value);
                                GW2_Grind_GAP = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW2_Grind_CRASH") //GW2 GRIND CRASH 
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW2_Grind_CRASH", value);
                                GW2_Grind_CRASH = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW1_Dress_GAP")//GW1 Grind
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW1_Dress_GAP", value);
                                GW1_Dress_GAP = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW1_Dress_CRASH")
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW1_Dress_CRASH", value);
                                GW1_Dress_CRASH = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW2_Dress_GAP")
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW2_Dress_GAP", value);
                                GW2_Dress_GAP = value;
                            }
                            //*********************************************************************
                            else if (Addr == "GW2_Dress_CRASH")
                            {
                                double.TryParse(sValue, out double value);
                                ini.WriteFloat("Parameter", "GW2_Dress_CRASH", value);
                                GW2_Dress_CRASH = value;
                            }
                        }
                    }
                }
            }));
            //砂輪恆速功能
            //LoadGW1SpeedLayout();
        }

        private void Fo_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ThrChangePos != null) ThrChangePos.Abort();

            if (ThrRedo != null) ThrRedo.Abort();

            if (ThrMeasure != null) ThrMeasure.Abort();

            if (ThrDressGw != null) ThrDressGw.Abort();

            if (ThrWaitM450 != null) ThrWaitM450.Abort();

            if (ScreenDisplayProcess != null)
            {
                ScreenDisplayProcess.Close();
            }

            if (CNCDataManageProcess != null)
            {
                CNCDataManageProcess.Close();
            }

            if (ThrScreenDisplay != null) ThrScreenDisplay.Abort();


            //sThrSoftPanel.Abort();

            bClose = true;
            while (!bCloseFinish) Application.DoEvents();

            masterSerialBus1.Close();
            while (!masterSerialBus1.IsClosed) Application.DoEvents();
            masterSerialBus2.Close();
            while (!masterSerialBus2.IsClosed) Application.DoEvents();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        //從機台讀回目前哪些工序被設為要執行的
        private void ReadProcessExe()
        {
            if (Units.Fo_Main.CurrentProgram == null) return;
            int process_count = Units.Fo_Main.CurrentProgram.Processes.Count;

            //全部讀取 (全部會放在 CurrentMacro)
            if (ReadAllMacro() != SUCCESS) return;

            this.Invoke(new Action(() =>
            {
                for (int i = 0; i < process_count; i++)
                {
                    ExecEnabled[i] = Math.Round(CurrentMacro[700 + i]) == 1;

                    if (ExecEnabled[i])
                    {
                        DGV_Monitor_Program.Rows[i].Cells[Col_ExeBtn.Index].Value = Properties.Resources.SwitchOn;
                    }
                    else
                    {
                        DGV_Monitor_Program.Rows[i].Cells[Col_ExeBtn.Index].Value = Properties.Resources.SwitchOff;
                    }
                }
            }));
        }



        private void DGV_Program_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            int row = e.RowIndex;
            //例外處理
            if (row < 0)
                return;

            //例外處理
            if (row >= ExecEnabled.Count)
                return;



            int col = e.ColumnIndex;

            TProcess p = CurrentProgram.Processes[row];
            if (p.ID == 201) //端測
            {
                foreach (TSubProgram s in p.SubPrograms)
                {
                    if (s.ProgNo == 9028)
                    {
                        TArgument a = s.GetArgument("19902");//使用/不使用
                        if (a != null)
                        {
                            if (a.Value == 0)
                            {
                                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 14, "端面量測未設定。"));
                                return;
                            }
                        }
                    }
                }
            }

            //以下是處理按鈕
            if (col != Col_ExeBtn.Index)
                return;
            ExecEnabled[row] = !ExecEnabled[row];
            if (ExecEnabled[row])
            {
                DGV_Monitor_Program.Rows[row].Cells[col].Value = Properties.Resources.SwitchOn;
                Actions.Enqueue(new Action(() =>
                {
                    focas.WriteMacro(700 + row, 1);
                }));
            }
            else
            {
                DGV_Monitor_Program.Rows[row].Cells[col].Value = Properties.Resources.SwitchOff;
                Actions.Enqueue(new Action(() =>
                {
                    focas.WriteMacro(700 + row, 0);
                }));
            }
        }


        public void PB_SwitchClick(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            String StrAddr = btn.Tag.ToString();
            string[] csv = StrAddr.Substring(1).Split('.');
            if (csv.Length != 2) return;
            int addr = int.Parse(csv[0]);
            int bit = int.Parse(csv[1]);

            Actions.Enqueue(new Action(() =>
            {
                focas.PMC_ReadByte(PmcAddrType.E, (ushort)addr, out byte tmp);
                bool flag = !tmp.GetBit(bit);
                tmp = tmp.SetBit(bit, flag);
                focas.PMC_WriteByte(PmcAddrType.E, (short)addr, tmp);
            }));

        }

        public void PB_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            Control ctrl = sender as Control;
            if (ctrl == null) return;
            focas.SetOutput(ctrl.Tag.ToString(), true);*/
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            String StrAddr = btn.Tag.ToString();

            /*
            foreach (SoftPanelPB pb in CurrentPBDown)
            {
                if (pb.Addr == StrAddr)
                {
                    foreach (string up in CurrentPBUp)
                    {
                        if (up == StrAddr)
                        {
                            CurrentPBUp.Remove(up);
                        }
                    }

                    pb.Start = Environment.TickCount;
                    return;
                }
            }

            SoftPanelPB pb1 = new SoftPanelPB();
            pb1.Addr = StrAddr;
            pb1.Start = Environment.TickCount;
            CurrentPBDown.Add(pb1);
            */
            string[] csv = StrAddr.Substring(1).Split('.');
            if (csv.Length != 2) return;
            int addr = int.Parse(csv[0]);
            int bit = int.Parse(csv[1]);

            Actions.Enqueue(new Action(() =>
            {
                focas.PMC_ReadByte(PmcAddrType.E, (ushort)addr, out byte tmp);
                tmp = tmp.SetBit(bit, true);
                focas.PMC_WriteByte(PmcAddrType.E, (short)addr, tmp);
            }));
        }

        public void PB_MouseUp(object sender, MouseEventArgs e)
        {
            /*
            Control ctrl = sender as Control;
            if (ctrl == null) return;
            focas.SetOutput(ctrl.Tag.ToString(), false);*/
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            String StrAddr = btn.Tag.ToString();
            /*
            foreach (SoftPanelPB pb in CurrentPBDown)
            {
                if (pb.Addr == StrAddr)
                {
                    int iTime = Environment.TickCount - pb.Start;
                    if(iTime<100)
                    {

                        CurrentPBUp.Add(StrAddr);
                        return;
                    }
                }
            }
            */
            string[] csv = StrAddr.Substring(1).Split('.');
            if (csv.Length != 2) return;
            int addr = int.Parse(csv[0]);
            int bit = int.Parse(csv[1]);

            Actions.Enqueue(new Action(() =>
            {
                focas.PMC_ReadByte(PmcAddrType.E, (ushort)addr, out byte tmp);
                tmp = tmp.SetBit(bit, false);
                focas.PMC_WriteByte(PmcAddrType.E, (short)addr, tmp);
            }));
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //如果兩個都開了
            if (pa_Alarm.Visible == pa_EMG.Visible)
            {
                //同步化
                if (la_Alarm.Visible != la_EMG.Visible)
                {
                    la_Alarm.Visible = la_EMG.Visible;
                }
            }

            la_Alarm.Visible = !la_Alarm.Visible;
            la_EMG.Visible = !la_EMG.Visible;
        }

        bool bDevelop = false;
        private void pic_Logo_DoubleClick(object sender, EventArgs e)
        {
            pa_Develop.Visible = false;

            Fo_Permission form = new Fo_Permission();
            if (form.ShowDialog() != DialogResult.OK) return;

            if (form.TB_ID.Text.ToLower() == "palmary" && form.TB_PSWD.Text.ToLower() == "16524622")
            {
                bDevelop = true;
                pa_Develop.Visible = true;
                pic_User.Image = Properties.Resources.user99;

                tb_MeasureStep.Visible = true;
                //tb_OffsetStep.Visible = true;
                //label29.Visible = true;
                //label33.Visible = true;
                //TB_PartCenterPosX.Visible = true;
                //btn_ID_CenterPos.Visible = true;
            }
            else
            {
                bDevelop = false;
                pa_Develop.Visible = false;
                pic_User.Image = Properties.Resources.user1s;

                tb_MeasureStep.Visible = false;
                //tb_OffsetStep.Visible = false;

            }


        }

        private void pic_Develop_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");

            tb_RotationCenter_Param_XC.Text = ini.ReadString("RotationCenter", "XC", Units.DisplayFmt);
            tb_RotationCenter_Param_ZC.Text = ini.ReadString("RotationCenter", "ZC", Units.DisplayFmt);

            ch_DGW_Conv.Checked = ini.ReadBool("System", "DressGwConv", false);
            ch_DWP_Conv.Checked = ini.ReadBool("System", "DressWorkpieceConv", false);
            ch_Specialopen.Checked = ini.ReadBool("System", "ch_Specialopen", false);//特殊橫進刀

            //ch_Param_CSS.Checked = ini.ReadBool("System", "ch_Param_CSS", false);//恆速功能
            //ch_Param_Detect.Checked = ini.ReadBool("System", "ch_Param_Detect", false);//電流偵測功能(GAP/CRASH)
            //ch_Param_Probe.Checked = ini.ReadBool("System", "ch_Param_Probe", false);//端測功能
            //ch_Param_GW2.Checked = ini.ReadBool("System", "ch_Param_GW2", false);//砂輪2

            Actions.Enqueue(new Action(() =>
            {

                //focas.ReadMacro(652, out double Macro652);//動力修砂功能
                //focas.ReadMacro(653, out double Macro653);//擺臂式修整座功能
                //focas.ReadMacro(654, out double Macro654);//自動門功能
                //focas.ReadMacro(658, out double Macro658);//量測治具寬
                focas.ReadMacro(10004, out double Macro10004);//GW1 Type
                focas.ReadMacro(10204, out double Macro10204);//GW2 Type
                focas.ReadMacro(10404, out double Macro10404);//GW3 Type
                focas.ReadMacro(10604, out double Macro10604);//GW4 Type
                //focas.ReadMacro(671, out double Macro671);// GW1 0:直頭 不等 0 斜頭 
                //focas.ReadMacro(673, out double Macro673);// GW2 0:直頭 不等 0 斜頭 
                //focas.ReadMacro(675, out double Macro675);// GW3 0:直頭 不等 0 斜頭 
                this.Invoke(new Action(() =>
                {
                    //ch_DGW_Func.Checked = Macro652 == 1;
                    //ch_SwingDress.Checked = Macro653 == 1;
                    //ch_AutoDoor.Checked = Macro654 == 1;
                    //btn_JigWidth.DisplayText = Macro658.ToString(Units.DisplayFmt);

                    rb_Gw1Type0.Checked = (Macro10004 == 0 && GWType[0] == MachineType.OCD);                   
                    rb_Gw1Type1.Checked = Macro10004 == 1;
                    rb_Gw1Type2.Checked = (Macro10004 == 0 && GWType[0] == MachineType.OCD2);
                    rb_Gw1Type3.Checked = (Macro10004 == 0 && GWType[0] == MachineType.OCD3);

                    rb_Gw2Type0.Checked = (Macro10204 == 0 && GWType[1] == MachineType.OCD);                
                    rb_Gw2Type1.Checked = Macro10204 == 1;
                    rb_Gw2Type2.Checked = (Macro10204 == 0 && GWType[1] == MachineType.OCD2);
                    rb_Gw2Type3.Checked = (Macro10204 == 0 && GWType[1] == MachineType.OCD3);

                    rb_Gw3Type0.Checked = (Macro10404 == 0 && GWType[2] == MachineType.OCD);
                    rb_Gw3Type1.Checked = Macro10404 == 1;
                    rb_Gw3Type2.Checked = (Macro10404 == 0 && GWType[2] == MachineType.OCD2);
                    rb_Gw3Type3.Checked = (Macro10404 == 0 && GWType[2] == MachineType.OCD3);
                }));

            }));





            ch_Meas.Checked = Measopen;//量測
            ch_Right.Checked = Rightopen;//右側修整

            ch_YAEnable.Checked = bYAEnable;
            //機型
            if (machineSeries == "M")
            {
                rb_M3.Checked = GwCount == 3;

                rb_M2.Checked = GwCount == 2;
            }
            //rb_Gw1Type0.Checked = GW1Type == MachineType.OCD;
            //rb_Gw1Type1.Checked = GW1Type == MachineType.OCD2;
            //rb_Gw1Type2.Checked = GW1Type == MachineType.OIG;

            //rb_Gw2Type0.Checked = GW2Type == MachineType.OCD;
            //rb_Gw2Type1.Checked = GW2Type == MachineType.OCD2;
            //rb_Gw2Type2.Checked = GW2Type == MachineType.OIG;

            //rb_Gw3Type0.Checked = GW3Type == MachineType.OCD;
            //rb_Gw3Type1.Checked = GW3Type == MachineType.OCD2;
            //rb_Gw3Type2.Checked = GW3Type == MachineType.OIG;

            la_Gw1GrindGap_Value.Text = GW1_Grind_GAP.ToString("0.0");
            la_Gw1GrindCrash_Value.Text = GW1_Grind_CRASH.ToString("0.0");
            la_Gw2GrindGap_Value.Text = GW2_Grind_GAP.ToString("0.0");
            la_Gw2GrindCrash_Value.Text = GW2_Grind_CRASH.ToString("0.0");

            la_Gw1DressGap_Value.Text = GW1_Dress_GAP.ToString("0.0");
            la_Gw1DressCrash_Value.Text = GW1_Dress_CRASH.ToString("0.0");
            la_Gw2DressGap_Value.Text = GW2_Dress_GAP.ToString("0.0");
            la_Gw2DressCrash_Value.Text = GW2_Dress_CRASH.ToString("0.0");

            TC_Main.SelectedTab = tab_Developer;

            PrevPage.Push(tab_Developer);
            btn_Prev.Visible = true;
        }

        private void pic_Serial_Click(object sender, EventArgs e)
        {

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            double Gw1Hz = ini.ReadFloat("Gw1", "Hz", 60);
            double Gw2Hz = ini.ReadFloat("Gw2", "Hz", 60);
            double Gw3Hz = ini.ReadFloat("Gw3", "Hz", 60);
            double Gw4Hz = ini.ReadFloat("Gw4", "Hz", 60);

            double Gw1Now = ini.ReadInteger("Gw1", "NowRPM", 0);
            double Gw2Now = ini.ReadInteger("Gw2", "NowRPM", 0);
            double Gw3Now = ini.ReadInteger("Gw3", "NowRPM", 0);
            double Gw4Now = ini.ReadInteger("Gw4", "NowRPM", 0);

            double RollerNow = ini.ReadInteger("Roller", "NowRPM", 0);
            double SpindleNow = ini.ReadInteger("Spindle", "NowRPM", 0);

            Fo_SetSerial form = new Fo_SetSerial();
            form.RB_1Bit485.Checked = serialPort1.StopBits == StopBits.One;
            form.RB_2Bit485.Checked = serialPort1.StopBits == StopBits.Two;
            form.RB_7Bit485.Checked = serialPort1.DataBits == 7;
            form.RB_8Bit485.Checked = serialPort1.DataBits == 8;
            form.RB_EVEN485.Checked = serialPort1.Parity == Parity.Even;
            form.RB_ODD485.Checked = serialPort1.Parity == Parity.Odd;
            form.RB_NONE485.Checked = serialPort1.Parity == Parity.None;

            if (serialPort1.BaudRate == 9600) form.CB_BaudRate485.SelectedIndex = 0;
            else if (serialPort1.BaudRate == 19200) form.CB_BaudRate485.SelectedIndex = 1;
            else if (serialPort1.BaudRate == 38400) form.CB_BaudRate485.SelectedIndex = 2;
            else if (serialPort1.BaudRate == 57600) form.CB_BaudRate485.SelectedIndex = 3;
            else if (serialPort1.BaudRate == 115200) form.CB_BaudRate485.SelectedIndex = 4;
            form.CB_Port485.Text = serialPort1.PortName;

            form.RB_1Bit422.Checked = serialPort2.StopBits == StopBits.One;//RS422設定
            form.RB_2Bit422.Checked = serialPort2.StopBits == StopBits.Two;
            form.RB_7Bit422.Checked = serialPort2.DataBits == 7;
            form.RB_8Bit422.Checked = serialPort2.DataBits == 8;
            form.RB_EVEN422.Checked = serialPort2.Parity == Parity.Even;
            form.RB_ODD422.Checked = serialPort2.Parity == Parity.Odd;
            form.RB_NONE422.Checked = serialPort2.Parity == Parity.None;

            if (serialPort2.BaudRate == 9600) form.CB_BaudRate422.SelectedIndex = 0;
            else if (serialPort2.BaudRate == 19200) form.CB_BaudRate422.SelectedIndex = 1;
            else if (serialPort2.BaudRate == 38400) form.CB_BaudRate422.SelectedIndex = 2;
            else if (serialPort2.BaudRate == 57600) form.CB_BaudRate422.SelectedIndex = 3;
            else if (serialPort2.BaudRate == 115200) form.CB_BaudRate422.SelectedIndex = 4;
            form.CB_Port422.Text = serialPort2.PortName;

            form.CB_SP_Channel.Visible = form.CB_SP_Dev.SelectedIndex == 1;

            form.CB_SP_Channel.SelectedIndex = SpindleChIndex;

            form.ch_GW1_Enabled.Checked = GW1_Comm_Enabled;
            form.TB_GW1_Slave.Text = Gw1.Slave.ToString("0.##");
            form.cb_GW1_Dev.SelectedIndex = Gw1Dev;
            form.TB_GW1_Rate.Text = Gw1.Rate.ToString("0.##");
            form.TB_GW1_ShowRate.Text = Gw1.ShowRate.ToString("0.##");
            form.TB_GW1_Max.Text = Gw1.MaxRpm.ToString("0.##");
            form.TB_GW1_Min.Text = Gw1.MinRpm.ToString("0.##");
            form.TB_GW1_SetRPM.Text = Gw1Now.ToString("0.##");
            form.TB_GW1_Unit.Text = Gw1.Unit.ToString("0.##");
            form.TB_GW1_SetHz.Text = Gw1Hz.ToString("0.##");

            form.ch_GW2_Enabled.Checked = GW2_Comm_Enabled;
            form.TB_GW2_Slave.Text = Gw2.Slave.ToString("0.##");
            form.cb_GW2_Dev.SelectedIndex = Gw2Dev;
            form.TB_GW2_Rate.Text = Gw2.Rate.ToString("0.##");
            form.TB_GW2_ShowRate.Text = Gw2.ShowRate.ToString("0.##");
            form.TB_GW2_Max.Text = Gw2.MaxRpm.ToString("0.##");
            form.TB_GW2_Min.Text = Gw2.MinRpm.ToString("0.##");
            form.TB_GW2_SetRPM.Text = Gw2Now.ToString("0.##");
            form.TB_GW2_Unit.Text = Gw2.Unit.ToString("0.##");
            form.TB_GW2_SetHz.Text = Gw2Hz.ToString("0.##");

            form.ch_GW3_Enabled.Checked = GW3_Comm_Enabled;
            form.TB_GW3_Slave.Text = Gw3.Slave.ToString("0.##");
            form.cb_GW3_Dev.SelectedIndex = Gw3Dev;
            form.TB_GW3_Rate.Text = Gw3.Rate.ToString("0.##");
            form.TB_GW3_ShowRate.Text = Gw3.ShowRate.ToString("0.##");
            form.TB_GW3_Max.Text = Gw3.MaxRpm.ToString("0.##");
            form.TB_GW3_Min.Text = Gw3.MinRpm.ToString("0.##");
            form.TB_GW3_SetRPM.Text = Gw3Now.ToString("0.##");
            form.TB_GW3_Unit.Text = Gw3.Unit.ToString("0.##");
            form.TB_GW3_SetHz.Text = Gw3Hz.ToString("0.##");

            form.ch_GW4_Enabled.Checked = GW4_Comm_Enabled;
            form.TB_GW4_Slave.Text = Gw4.Slave.ToString("0.##");
            form.cb_GW4_Dev.SelectedIndex = Gw4Dev;
            form.TB_GW4_Rate.Text = Gw4.Rate.ToString("0.##");
            form.TB_GW4_ShowRate.Text = Gw4.ShowRate.ToString("0.##");
            form.TB_GW4_Max.Text = Gw4.MaxRpm.ToString("0.##");
            form.TB_GW4_Min.Text = Gw4.MinRpm.ToString("0.##");
            form.TB_GW4_SetRPM.Text = Gw4Now.ToString("0.##");
            form.TB_GW4_Unit.Text = Gw4.Unit.ToString("0.##");
            form.TB_GW4_SetHz.Text = Gw4Hz.ToString("0.##");

            form.ch_Rollor_Enabled.Checked = Roller_Comm_Enabled;
            form.TB_Roller_Slave.Text = Roller.Slave.ToString("0.##");
            form.cb_Roller_Dev.SelectedIndex = RollerDev;
            form.TB_Roller_Rate.Text = Roller.Rate.ToString("0.##");
            form.TB_Roller_ShowRate.Text = Roller.ShowRate.ToString("0.##");
            form.TB_Roller_Max.Text = Roller.MaxRpm.ToString("0.##");
            form.TB_Roller_Min.Text = Roller.MinRpm.ToString("0.##");
            form.TB_Roller_SetRPM.Text = RollerNow.ToString("0.##");
            form.TB_Roller_Unit.Text = Roller.Unit.ToString("0.##");

            form.ch_Spindle_Enabled.Checked = SP_Comm_Enabled;
            form.TB_SP_Slave.Text = Spindle.Slave.ToString("0.##");
            form.TB_SP_Rate.Text = Spindle.Rate.ToString("0.##");
            form.TB_SP_ShowRate.Text = Spindle.ShowRate.ToString("0.##");
            form.TB_SP_Max.Text = Spindle.MaxRpm.ToString("0.##");
            form.TB_SP_Min.Text = Spindle.MinRpm.ToString("0.##");
            form.TB_SP_SetRPM.Text = SpindleNow.ToString("0.##");
            form.TB_SP_Unit.Text = Spindle.Unit.ToString("0.##");
            form.CB_SP_Dev.SelectedIndex = SpindleDev;

            

            if (form.ShowDialog() != DialogResult.OK) return;




            if (form.CB_Port422.Text == "") form.CB_Port422.Text = "COM";
            if (form.CB_Port485.Text == "") form.CB_Port485.Text = "COM";
            int databit = form.RB_7Bit485.Checked ? 7 : 8;
            int.TryParse(form.CB_BaudRate485.Text, out int baud);
            int stopbit = form.RB_1Bit485.Checked ? (int)StopBits.One : (int)StopBits.Two;
            int parity = 0;
            if (form.RB_EVEN485.Checked) parity = (int)Parity.Even;
            else if (form.RB_ODD485.Checked) parity = (int)Parity.Odd;
            else if (form.RB_NONE485.Checked) parity = (int)Parity.None;

            ini.WriteString("Serial", "Port", form.CB_Port485.Text);
            ini.WriteInteger("Serial", "BaudRate", baud);
            ini.WriteInteger("Serial", "DataBits", databit);
            ini.WriteInteger("Serial", "StopBits", stopbit);
            ini.WriteInteger("Serial", "Parity", parity);

            int databit422 = form.RB_7Bit422.Checked ? 7 : 8;
            int.TryParse(form.CB_BaudRate422.Text, out int baud422);
            int stopbit422 = form.RB_1Bit422.Checked ? (int)StopBits.One : (int)StopBits.Two;
            int parity422 = 0;
            if (form.RB_EVEN422.Checked) parity422 = (int)Parity.Even;
            else if (form.RB_ODD422.Checked) parity422 = (int)Parity.Odd;
            else if (form.RB_NONE422.Checked) parity422 = (int)Parity.None;

            ini.WriteString("Serial2", "Port", form.CB_Port422.Text);
            ini.WriteInteger("Serial2", "BaudRate", baud422);
            ini.WriteInteger("Serial2", "DataBits", databit422);
            ini.WriteInteger("Serial2", "StopBits", stopbit422);
            ini.WriteInteger("Serial2", "Parity", parity422);

            double.TryParse(form.TB_GW1_SetHz.Text, out Gw1Hz);
            int.TryParse(form.TB_GW1_SetRPM.Text, out int nowrpm);
            Gw1.Slave = int.Parse(form.TB_GW1_Slave.Text);
            Gw1.MaxRpm = int.Parse(form.TB_GW1_Max.Text);
            Gw1.MinRpm = int.Parse(form.TB_GW1_Min.Text);
            Gw1.Rate = double.Parse(form.TB_GW1_Rate.Text);   //計算倍率
            Gw1.ShowRate = double.Parse(form.TB_GW1_ShowRate.Text);
            Gw1.Unit = double.Parse(form.TB_GW1_Unit.Text);
            Gw1Dev = form.cb_GW1_Dev.SelectedIndex;
            ini.WriteInteger("System", "Gw1Dev", Gw1Dev);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
                                                         //最高頻率(倍率用)
            ini.WriteFloat("Gw1", "Hz", Gw1Hz);
            //砂輪站號
            ini.WriteInteger("Gw1", "Slave", Gw1.Slave);
            //轉速倍率(RPM/Hz)
            ini.WriteFloat("Gw1", "Rate", Gw1.Rate);
            //顯示轉速倍率
            ini.WriteFloat("Gw1", "ShowRate", Gw1.ShowRate);
            //軟體上限
            ini.WriteInteger("Gw1", "MaxRpm", Gw1.MaxRpm);
            //軟體下限
            ini.WriteInteger("Gw1", "MinRpm", Gw1.MinRpm);
            //最大轉速(倍率用)            
            ini.WriteInteger("Gw1", "NowRPM", nowrpm);
            //單位(0.1 Hz)
            ini.WriteFloat("Gw1", "Unit", Gw1.Unit);
            //最後設定的指令速度
            ini.WriteFloat("Gw1", "Cmd", Gw1.CmdSpeed);
            GW1_Comm_Enabled = form.ch_GW1_Enabled.Checked;
            ini.WriteInteger("System", "GW1_Comm_Enabled", GW1_Comm_Enabled ? 1 : 0);


            double.TryParse(form.TB_GW2_SetHz.Text, out Gw2Hz);
            int.TryParse(form.TB_GW2_SetRPM.Text, out nowrpm);
            Gw2.Slave = int.Parse(form.TB_GW2_Slave.Text);
            Gw2.MaxRpm = int.Parse(form.TB_GW2_Max.Text);
            Gw2.MinRpm = int.Parse(form.TB_GW2_Min.Text);
            Gw2.Rate = double.Parse(form.TB_GW2_Rate.Text);
            Gw2.ShowRate = double.Parse(form.TB_GW2_ShowRate.Text);
            Gw2.Unit = double.Parse(form.TB_GW2_Unit.Text);
            Gw2Dev = form.cb_GW2_Dev.SelectedIndex;
            ini.WriteInteger("System", "Gw2Dev", Gw2Dev);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
                                                         //最高頻率(倍率用)  
            ini.WriteFloat("Gw2", "Hz", Gw2Hz);
            //砂輪站號
            ini.WriteInteger("Gw2", "Slave", Gw2.Slave);
            //轉速倍率
            ini.WriteFloat("Gw2", "Rate", Gw2.Rate);
            //顯示轉速倍率
            ini.WriteFloat("Gw2", "ShowRate", Gw2.ShowRate);
            //軟體上限
            ini.WriteInteger("Gw2", "MaxRpm", Gw2.MaxRpm);
            //軟體下限
            ini.WriteInteger("Gw2", "MinRpm", Gw2.MinRpm);
            //最大轉速(倍率用)
            ini.WriteInteger("Gw2", "NowRPM", nowrpm);
            //單位(1 RPM)
            ini.WriteFloat("Gw2", "Unit", Gw2.Unit);
            //最後設定的指令速度
            ini.WriteFloat("Gw2", "Cmd", Gw2.CmdSpeed);
            GW2_Comm_Enabled = form.ch_GW2_Enabled.Checked;
            ini.WriteInteger("System", "GW2_Comm_Enabled", GW2_Comm_Enabled ? 1 : 0);

            double.TryParse(form.TB_GW3_SetHz.Text, out Gw2Hz);
            int.TryParse(form.TB_GW3_SetRPM.Text, out nowrpm);
            Gw3.Slave = int.Parse(form.TB_GW3_Slave.Text);
            Gw3.MaxRpm = int.Parse(form.TB_GW3_Max.Text);
            Gw3.MinRpm = int.Parse(form.TB_GW3_Min.Text);
            Gw3.Rate = double.Parse(form.TB_GW3_Rate.Text);
            Gw3.ShowRate = double.Parse(form.TB_GW3_ShowRate.Text);
            Gw3.Unit = double.Parse(form.TB_GW3_Unit.Text);
            Gw3Dev = form.cb_GW3_Dev.SelectedIndex;
            ini.WriteInteger("System", "Gw3Dev", Gw3Dev);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
                                                         //最高頻率(倍率用)  
            ini.WriteFloat("Gw3", "Hz", Gw3Hz);
            //砂輪站號
            ini.WriteInteger("Gw3", "Slave", Gw3.Slave);
            //轉速倍率
            ini.WriteFloat("Gw3", "Rate", Gw3.Rate);
            //顯示轉速倍率
            ini.WriteFloat("Gw3", "ShowRate", Gw3.ShowRate);
            //軟體上限
            ini.WriteInteger("Gw3", "MaxRpm", Gw3.MaxRpm);
            //軟體下限
            ini.WriteInteger("Gw3", "MinRpm", Gw3.MinRpm);
            //最大轉速(倍率用)
            ini.WriteInteger("Gw3", "NowRPM", nowrpm);
            //單位(1 RPM)
            ini.WriteFloat("Gw3", "Unit", Gw3.Unit);
            //最後設定的指令速度
            ini.WriteFloat("Gw3", "Cmd", Gw3.CmdSpeed);
            GW3_Comm_Enabled = form.ch_GW3_Enabled.Checked;
            ini.WriteInteger("System", "GW3_Comm_Enabled", GW3_Comm_Enabled ? 1 : 0);

            double.TryParse(form.TB_GW4_SetHz.Text, out Gw2Hz);
            int.TryParse(form.TB_GW4_SetRPM.Text, out nowrpm);
            Gw4.Slave = int.Parse(form.TB_GW4_Slave.Text);
            Gw4.MaxRpm = int.Parse(form.TB_GW4_Max.Text);
            Gw4.MinRpm = int.Parse(form.TB_GW4_Min.Text);
            Gw4.Rate = double.Parse(form.TB_GW4_Rate.Text);
            Gw4.ShowRate = double.Parse(form.TB_GW4_ShowRate.Text);
            Gw4.Unit = double.Parse(form.TB_GW4_Unit.Text);
            Gw4Dev = form.cb_GW4_Dev.SelectedIndex;
            ini.WriteInteger("System", "Gw4Dev", Gw4Dev);//0:士林變頻器, 1:台達變頻器, 2:三菱變頻器
                                                         //最高頻率(倍率用)  
            ini.WriteFloat("Gw4", "Hz", Gw4Hz);
            //砂輪站號
            ini.WriteInteger("Gw4", "Slave", Gw4.Slave);
            //轉速倍率
            ini.WriteFloat("Gw4", "Rate", Gw4.Rate);
            //顯示轉速倍率
            ini.WriteFloat("Gw4", "ShowRate", Gw4.ShowRate);
            //軟體上限
            ini.WriteInteger("Gw4", "MaxRpm", Gw4.MaxRpm);
            //軟體下限
            ini.WriteInteger("Gw4", "MinRpm", Gw4.MinRpm);
            //最大轉速(倍率用)
            ini.WriteInteger("Gw4", "NowRPM", nowrpm);
            //單位(1 RPM)
            ini.WriteFloat("Gw4", "Unit", Gw4.Unit);
            //最後設定的指令速度
            ini.WriteFloat("Gw4", "Cmd", Gw4.CmdSpeed);
            GW4_Comm_Enabled = form.ch_GW4_Enabled.Checked;
            ini.WriteInteger("System", "GW4_Comm_Enabled", GW4_Comm_Enabled ? 1 : 0);

            int.TryParse(form.TB_Roller_SetRPM.Text, out nowrpm);
            Roller.Slave = int.Parse(form.TB_Roller_Slave.Text);
            Roller.MaxRpm = int.Parse(form.TB_Roller_Max.Text);
            Roller.MinRpm = int.Parse(form.TB_Roller_Min.Text);
            Roller.Rate = double.Parse(form.TB_Roller_Rate.Text);
            Roller.ShowRate = double.Parse(form.TB_Roller_ShowRate.Text);
            Roller.Unit = double.Parse(form.TB_Roller_Unit.Text);
            //RollerDev 目前固定為 士林變頻器
            //砂輪站號
            ini.WriteInteger("Roller", "Slave", Roller.Slave);
            //轉速倍率
            ini.WriteFloat("Roller", "Rate", Roller.Rate);
            //顯示轉速倍率
            ini.WriteFloat("Roller", "ShowRate", Roller.ShowRate);
            //軟體上限
            ini.WriteInteger("Roller", "MaxRpm", Roller.MaxRpm);
            //軟體下限
            ini.WriteInteger("Roller", "MinRpm", Roller.MinRpm);
            //最大轉速(倍率用)
            ini.WriteInteger("Roller", "NowRPM", nowrpm);
            //單位(1 RPM)
            ini.WriteFloat("Roller", "Unit", Roller.Unit);
            //最後設定的指令速度
            ini.WriteFloat("Roller", "Cmd", Roller.CmdSpeed);
            Roller_Comm_Enabled = form.ch_Rollor_Enabled.Checked;
            ini.WriteInteger("System", "Roller_Comm_Enabled", Roller_Comm_Enabled ? 1 : 0);

            int.TryParse(form.TB_SP_SetRPM.Text, out nowrpm);
            Spindle.Slave = int.Parse(form.TB_SP_Slave.Text);
            Spindle.MaxRpm = int.Parse(form.TB_SP_Max.Text);
            Spindle.MinRpm = int.Parse(form.TB_SP_Min.Text);
            Spindle.Rate = double.Parse(form.TB_SP_Rate.Text);
            Spindle.ShowRate = double.Parse(form.TB_SP_ShowRate.Text);
            Spindle.Unit = double.Parse(form.TB_SP_Unit.Text);
            SpindleDev = form.CB_SP_Dev.SelectedIndex;
            ini.WriteInteger("System", "SpindleDev", SpindleDev);//0:三菱驅動器, 1:安川驅動器, 2:士林變頻器
                                                                 //砂輪站號
            ini.WriteInteger("Spindle", "Slave", Spindle.Slave);
            //轉速倍率
            ini.WriteFloat("Spindle", "Rate", Spindle.Rate);
            //顯示轉速倍率
            ini.WriteFloat("Spindle", "ShowRate", Spindle.ShowRate);
            //最大轉速
            ini.WriteInteger("Spindle", "MaxRpm", Spindle.MaxRpm);
            //最小轉速
            ini.WriteInteger("Spindle", "MinRpm", Spindle.MinRpm);
            //最大轉速(倍率用)
            ini.WriteInteger("Spindle", "NowRPM", nowrpm);
            //單位(1 RPM)
            ini.WriteFloat("Spindle", "Unit", Spindle.Unit);
            //最後設定的指令速度
            ini.WriteFloat("Spindle", "Cmd", Spindle.CmdSpeed);
            SP_Comm_Enabled = form.ch_Spindle_Enabled.Checked;
            ini.WriteInteger("System", "SP_Comm_Enabled", SP_Comm_Enabled ? 1 : 0);
            SpindleChIndex = form.CB_SP_Channel.SelectedIndex;
            ini.WriteInteger("System", "SpindleChIndex", SpindleChIndex);//0:RS485 , 1:RS422

            tb_Debug.AppendText("清除指令\r\n");
            masterSerialBus1.QueryList.Clear();
            masterSerialBus2.QueryList.Clear();


            int iStart = Environment.TickCount;
            while (masterSerialBus1.CmdList.Count > 0)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 3000)
                {
                    tb_Debug.AppendText("剩餘指令未能傳送完成\r\n");
                    break;
                }
                Application.DoEvents();
            }
            iStart = Environment.TickCount;
            while (masterSerialBus2.CmdList.Count > 0)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 3000)
                {
                    tb_Debug.AppendText("剩餘指令未能傳送完成\r\n");
                    break;
                }
                Application.DoEvents();
            }

            tb_Debug.AppendText("暫停排程中...\r\n");
            masterSerialBus1.Stop();
            masterSerialBus2.Stop();

            iStart = Environment.TickCount;
            while (!masterSerialBus1.IsStop)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 3000)
                {
                    tb_Debug.AppendText("暫停超時\r\n");
                    break;
                }
                Application.DoEvents();
            }

            iStart = Environment.TickCount;
            while (!masterSerialBus2.IsStop)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 3000)
                {
                    tb_Debug.AppendText("暫停超時\r\n");
                    break;
                }
                Application.DoEvents();
            }

            //砂輪通訊設定
            if (serialPort1.IsOpen)
            {
                try
                {
                    tb_Debug.AppendText("關閉通訊\r\n");
                    serialPort1.Close();
                }
                catch (Exception ex)
                {
                    Fo_Msg.Show(ex.Message);
                }
            }
            //砂輪通訊設定
            if (serialPort2.IsOpen)
            {
                try
                {
                    tb_Debug.AppendText("關閉通訊\r\n");
                    serialPort2.Close();
                }
                catch (Exception ex)
                {
                    Fo_Msg.Show(ex.Message);
                }
            }

            tb_Debug.AppendText("通訊設定\r\n");
            serialPort1.PortName = form.CB_Port485.Text;
            serialPort1.BaudRate = baud;
            serialPort1.DataBits = databit;
            serialPort1.StopBits = (StopBits)stopbit;
            serialPort1.Parity = (Parity)parity;

            try
            {
                string[] names = SerialPort.GetPortNames();
                foreach (string name in names)
                {
                    if (name == serialPort1.PortName)
                    {
                        tb_Debug.AppendText("開啟通訊\r\n");
                        serialPort1.Open();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            pic_RS422error.Visible = false;

            serialPort2.PortName = form.CB_Port422.Text;
            serialPort2.BaudRate = baud422;
            serialPort2.DataBits = databit422;
            serialPort2.StopBits = (StopBits)stopbit422;
            serialPort2.Parity = (Parity)parity422;
            if (SpindleDev == 1 && SpindleChIndex == 1 && SP_Comm_Enabled)
            {
                try
                {
                    string[] names = SerialPort.GetPortNames();
                    foreach (string name in names)
                    {
                        if (name == serialPort2.PortName)
                        {
                            tb_Debug.AppendText("開啟通訊\r\n");
                            serialPort2.Open();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (!serialPort2.IsOpen) Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 87, "串列埠開啟異常。"));
                pic_RS422error.Visible = !serialPort2.IsOpen;
            }
            if (!serialPort1.IsOpen) Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 87, "串列埠開啟異常。"));
            pic_RS485error.Visible = !serialPort1.IsOpen;


            pa.Visible = false;

            GW1ERROR_Count = 0;
            GW2ERROR_Count = 0;
            GW3ERROR_Count = 0;
            GW4ERROR_Count = 0;
            ROLLERERROR_Count = 0;
            SPERROR_Count = 0;

            RefleshQueryList();
            tb_Debug.AppendText("重啟排程\r\n");


            masterSerialBus1.Start();
            if (SpindleDev == 1 && SpindleChIndex == 1 && SP_Comm_Enabled)
                masterSerialBus2.Start();
        }

        private void Gw1DetectGapCrash(double a)
        {
            if (bGW1_GAP_CRASH_Enabled)//D908.6 砂輪1電流防撞功能開啟
            {
                if (bGW1GRIND_AT && (GW1_Grind_GAP > 0))
                {
                    bool bGap = (a >= GW1_Grind_GAP);
                    if (bGap != bGW1_GAP)
                    {
                        pic_Gw1Gap.Image = bGap ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                        focas.PMC_ReadByte(PmcAddrType.E, 7, out byte E7);
                        E7 = E7.SetBit(1, bGap);//GW1 CRASH E7.1 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 7, E7);
                    }
                    bGW1_GAP = bGap;
                }
                if (bGW1DRESS_AT && (GW1_Dress_GAP > 0))
                {
                    bool bGap = (a >= GW1_Dress_GAP);
                    if (bGap != bGW1_GAP)
                    {
                        pic_Gw1Gap.Image = bGap ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                        focas.PMC_ReadByte(PmcAddrType.E, 7, out byte E7);
                        E7 = E7.SetBit(1, bGap);//GW1 CRASH E7.1 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 7, E7);
                    }
                    bGW1_GAP = bGap;
                }
                if (bGW1GRIND_AT && (GW1_Grind_CRASH > 0))
                {
                    bool bCrash = (a >= GW1_Grind_CRASH);
                    if (bCrash && !bGW1_CRASH)
                    {
                        pic_Gw1Crash.Image = Properties.Resources.Lamp_E_On;
                        focas.PMC_ReadByte(PmcAddrType.E, 6, out byte E6);
                        E6 = E6.SetBit(1, true);//GW1 CRASH E6.1 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 6, E6);
                        bGW1_CRASH = true;
                    }
                }
                if (bGW1DRESS_AT && (GW1_Dress_CRASH > 0))
                {
                    bool bCrash = (a >= GW1_Dress_CRASH);
                    if (bCrash && !bGW1_CRASH)
                    {
                        pic_Gw1Crash.Image = Properties.Resources.Lamp_E_On;
                        focas.PMC_ReadByte(PmcAddrType.E, 6, out byte E6);
                        E6 = E6.SetBit(1, true);//GW1 CRASH E6.1 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 6, E6);
                        bGW1_CRASH = true;
                    }
                }
            }
        }

        private void Gw2DetectGapCrash(double a)
        {
            if (bGW2_GAP_CRASH_Enabled)
            {

                if (bGW2GRIND_AT && GW2_Grind_GAP > 0)
                {
                    bool bGap = a >= GW2_Grind_GAP;
                    if (bGap != bGW2_GAP)
                    {
                        pic_Gw2Gap.Image = bGap ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                        focas.PMC_ReadByte(PmcAddrType.E, 7, out byte E7);
                        E7 = E7.SetBit(3, bGap);//GW2 CRASH E7.3 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 7, E7);
                    }
                    bGW2_GAP = bGap;
                }
                if (bGW2DRESS_AT && GW2_Dress_GAP > 0)
                {
                    bool bGap = a >= GW2_Dress_GAP;
                    if (bGap != bGW2_GAP)
                    {
                        pic_Gw2Gap.Image = bGap ? Properties.Resources.Lamp_E_On : Properties.Resources.Lamp_E_Off;
                        focas.PMC_ReadByte(PmcAddrType.E, 7, out byte E7);
                        E7 = E7.SetBit(3, bGap);//GW2 CRASH E7.3 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 7, E7);
                    }
                    bGW2_GAP = bGap;
                }
                if (bGW2GRIND_AT && GW2_Grind_CRASH > 0)
                {
                    bool bCrash = a >= GW2_Grind_CRASH;
                    if (bCrash && !bGW2_CRASH)
                    {
                        pic_Gw2Crash.Image = Properties.Resources.Lamp_E_On;
                        focas.PMC_ReadByte(PmcAddrType.E, 6, out byte E6);
                        E6 = E6.SetBit(2, true);//GW2 CRASH E6.2 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 6, E6);
                        bGW2_CRASH = true;
                    }

                }
                if (bGW2DRESS_AT && GW2_Dress_CRASH > 0)
                {
                    bool bCrash = a >= GW2_Dress_CRASH;
                    if (bCrash && !bGW2_CRASH)
                    {
                        pic_Gw2Crash.Image = Properties.Resources.Lamp_E_On;
                        focas.PMC_ReadByte(PmcAddrType.E, 6, out byte E6);
                        E6 = E6.SetBit(2, true);//GW2 CRASH E6.2 ON
                        focas.PMC_WriteByte(PmcAddrType.E, 6, E6);
                        bGW2_CRASH = true;
                    }

                }
            }
        }

        private void masterSerialBus1_OnReceive(object sender, string receive, string cmd)
        {
            if (TC_Main.SelectedTab == tab_Developer) tb_serial.AppendText("R:" + receive + "\r\n");



            CheckSerialHeart = Environment.TickCount;

            //int iCommErrTime = Environment.TickCount - COMM_ERR_START;
            //if (iCommErrTime > 3) pa.Visible = false;

            //沒有通訊異常就關閉
            if (GW1ERROR_Count == 0 &&
                GW2ERROR_Count == 0 &&
                GW3ERROR_Count == 0 &&
                GW4ERROR_Count == 0 &&
                SPERROR_Count == 0 &&
                ROLLERERROR_Count == 0) pa.Visible = false;

            int iSlave = int.Parse(receive.Substring(0, 2), NumberStyles.HexNumber);
            int iFunc = int.Parse(receive.Substring(2, 2), NumberStyles.HexNumber); //只有安川驅動器例外
            int iCheckSlave = int.Parse(cmd.Substring(0, 2), NumberStyles.HexNumber);//確保送跟收對應沒問題

            if (iSlave == BalanceSlave) tb_serial.AppendText("R:" + receive + "(" + cmd + ")\r\n");

            //例外處理
            if (iSlave != iCheckSlave) return;

            if (iSlave == Gw1.Slave)//砂輪1
            {
                GW1ERROR_Count = 0;
                int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                if (iFunc != 3) return;//只處理讀取的指令
                if (Gw1Dev == 0 && iAddr == 0x1002) //士林變頻器
                {
                    //士林變頻器 (指令頻率、輸出頻率、輸出電流...)

                    //輸出頻率,
                    la_Gw1CurrentRpm.Text = (Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Gw1.Unit * Gw1.Rate).ToString("0");

                    //輸出電流, 
                    double a = Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw1CurrentA.Text = a.ToString("0.0");
                    la_Gw1Ampare_Value.Text = a.ToString("0.0");
                    Gw1DetectGapCrash(a);

                }
                else if (Gw1Dev == 1 && iAddr == 0x2100) // 台達變頻器
                {
                    //錯誤碼 0x2100
                    //Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber)

                    //狀態 0x2101
                    //Bit0~Bit1 數位操作器LED燈號,
                    //Bit2 有JOG指令,
                    //Bit3~Bit4 FWD狀態,
                    //Bit8 主頻率來源由通訊界面,
                    //Bit9 主頻率來源由類比訊號輸入
                    //Bit10 運轉指令由通訊界面
                    //Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber);

                    //01030A 0000 0000 0000 00000000
                    //指令頻率(F) 0x2102
                    //Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber);

                    //輸出頻率(H) 0x2103
                    la_Gw1CurrentRpm.Text = (Int16.Parse(receive.Substring(18, 4), NumberStyles.HexNumber) * Gw1.Unit * Gw1.Rate).ToString("0");

                    //輸出電流(VFD-V:AXX.XX, VFD-E:AXX.X, VFD-VE:AXXX.X) 0x2104
                    double a = Int16.Parse(receive.Substring(22, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw1CurrentA.Text = a.ToString("0.0");
                    la_Gw1Ampare_Value.Text = a.ToString("0.0");
                    Gw1DetectGapCrash(a);
                }
                else if (Gw1Dev == 2 && iAddr == 0x00C8) //三菱變頻器
                {
                    //01 03 1C 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000
                    //輸出頻率, 輸出電流, 輸出電壓, 異常顯示, 指令頻率, 運行轉速, 電機轉矩

                    //輸出頻率,
                    la_Gw1CurrentRpm.Text = (Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) * Gw1.Unit * Gw1.Rate).ToString("0");

                    //輸出電流, 
                    double a = Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw1CurrentA.Text = a.ToString("0.0");
                    Gw1DetectGapCrash(a);
                }
            }
            else if (iSlave == Gw2.Slave) //砂輪2
            {
                GW2ERROR_Count = 0;
                int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                if (iFunc != 3) return;//只處理讀取的指令
                if (Gw2Dev == 0 && iAddr == 0x1002) //士林變頻器
                {
                    //士林變頻器 (指令頻率、輸出頻率、輸出電流...)
                    //輸出頻率, 
                    la_Gw2CurrentRpm.Text = (Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Gw2.Unit * Gw2.Rate).ToString("0");

                    //輸出電流,
                    double a = Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw2CurrentA.Text = a.ToString("0.0");
                    la_Gw2Ampare_Value.Text = a.ToString("0.0");
                    Gw2DetectGapCrash(a);
                }
                else if (Gw2Dev == 1 && iAddr == 0x2100) // 台達變頻器
                {

                    //錯誤碼 0x2100
                    //Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber)

                    //狀態 0x2101
                    //Bit0~Bit1 數位操作器LED燈號,
                    //Bit2 有JOG指令,
                    //Bit3~Bit4 FWD狀態,
                    //Bit8 主頻率來源由通訊界面,
                    //Bit9 主頻率來源由類比訊號輸入
                    //Bit10 運轉指令由通訊界面
                    //Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber);

                    //01030A 0000 0000 0000 00000000
                    //指令頻率(F) 0x2102
                    //Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber);

                    //輸出頻率(H) 0x2103
                    la_Gw2CurrentRpm.Text = (Int16.Parse(receive.Substring(18, 4), NumberStyles.HexNumber) * Gw2.Unit * Gw2.Rate).ToString("0");

                    //輸出電流(AXX.X) 0x2104
                    double a = Int16.Parse(receive.Substring(22, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw2CurrentA.Text = a.ToString("0.0");
                    la_Gw2Ampare_Value.Text = a.ToString("0.0");
                    Gw2DetectGapCrash(a);

                }
                else if (Gw2Dev == 2 && iAddr == 0x00C8) //三菱變頻器
                {
                    //01 03 1C 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000

                    //輸出頻率, 輸出電流, 輸出電壓, 異常顯示, 指令頻率, 運行轉速, 電機轉矩

                    //輸出頻率,
                    la_Gw2CurrentRpm.Text = (Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) * Gw2.Unit * Gw2.Rate).ToString("0");

                    //輸出電流, 
                    double a = Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw2CurrentA.Text = a.ToString("0.0");
                    Gw2DetectGapCrash(a);
                }
            }
            else if (iSlave == Gw3.Slave) //砂輪3
            {
                GW3ERROR_Count = 0;
                int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                if (iFunc != 3) return;//只處理讀取的指令
                if (Gw3Dev == 0 && iAddr == 0x1002) //士林變頻器
                {
                    //士林變頻器 (指令頻率、輸出頻率、輸出電流...)
                    //輸出頻率, 
                    la_Gw3CurrentRpm.Text = (Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Gw3.Unit * Gw3.Rate).ToString("0");

                    //輸出電流,
                    double a = Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw3CurrentA.Text = a.ToString("0.0");
                    la_Gw3Ampare_Value.Text = a.ToString("0.0");
                }
                else if (Gw3Dev == 1 && iAddr == 0x2100) // 台達變頻器
                {

                    //錯誤碼 0x2100
                    //Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber)

                    //狀態 0x2101
                    //Bit0~Bit1 數位操作器LED燈號,
                    //Bit2 有JOG指令,
                    //Bit3~Bit4 FWD狀態,
                    //Bit8 主頻率來源由通訊界面,
                    //Bit9 主頻率來源由類比訊號輸入
                    //Bit10 運轉指令由通訊界面
                    //Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber);

                    //01030A 0000 0000 0000 00000000
                    //指令頻率(F) 0x2102
                    //Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber);

                    //輸出頻率(H) 0x2103
                    la_Gw3CurrentRpm.Text = (Int16.Parse(receive.Substring(18, 4), NumberStyles.HexNumber) * Gw3.Unit * Gw3.Rate).ToString("0");

                    //輸出電流(AXX.X) 0x2104
                    double a = Int16.Parse(receive.Substring(22, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw3CurrentA.Text = a.ToString("0.0");
                    la_Gw3Ampare_Value.Text = a.ToString("0.0");

                }
                else if (Gw3Dev == 2 && iAddr == 0x00C8) //三菱變頻器
                {
                    //01 03 1C 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000

                    //輸出頻率, 輸出電流, 輸出電壓, 異常顯示, 指令頻率, 運行轉速, 電機轉矩

                    //輸出頻率,
                    la_Gw3CurrentRpm.Text = (Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) * Gw3.Unit * Gw3.Rate).ToString("0");

                    //輸出電流, 
                    double a = Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw3CurrentA.Text = a.ToString("0.0");
                }
            }
            else if (iSlave == Gw4.Slave) //砂輪4
            {
                GW4ERROR_Count = 0;
                int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                if (iFunc != 3) return;//只處理讀取的指令
                if (Gw4Dev == 0 && iAddr == 0x1002) //士林變頻器
                {
                    //士林變頻器 (指令頻率、輸出頻率、輸出電流...)
                    //輸出頻率, 
                    la_Gw4CurrentRpm.Text = (Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Gw4.Unit * Gw4.Rate).ToString("0");

                    //輸出電流,
                    double a = Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw4CurrentA.Text = a.ToString("0.0");
                    la_Gw4Ampare_Value.Text = a.ToString("0.0");
                }
                else if (Gw4Dev == 1 && iAddr == 0x2100) // 台達變頻器
                {

                    //錯誤碼 0x2100
                    //Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber)

                    //狀態 0x2101
                    //Bit0~Bit1 數位操作器LED燈號,
                    //Bit2 有JOG指令,
                    //Bit3~Bit4 FWD狀態,
                    //Bit8 主頻率來源由通訊界面,
                    //Bit9 主頻率來源由類比訊號輸入
                    //Bit10 運轉指令由通訊界面
                    //Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber);

                    //01030A 0000 0000 0000 00000000
                    //指令頻率(F) 0x2102
                    //Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber);

                    //輸出頻率(H) 0x2103
                    la_Gw4CurrentRpm.Text = (Int16.Parse(receive.Substring(18, 4), NumberStyles.HexNumber) * Gw4.Unit * Gw4.Rate).ToString("0");

                    //輸出電流(AXX.X) 0x2104
                    double a = Int16.Parse(receive.Substring(22, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw4CurrentA.Text = a.ToString("0.0");
                    la_Gw4Ampare_Value.Text = a.ToString("0.0");

                }
                else if (Gw4Dev == 2 && iAddr == 0x00C8) //三菱變頻器
                {
                    //01 03 1C 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000

                    //輸出頻率, 輸出電流, 輸出電壓, 異常顯示, 指令頻率, 運行轉速, 電機轉矩

                    //輸出頻率,
                    la_Gw4CurrentRpm.Text = (Int16.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) * Gw4.Unit * Gw4.Rate).ToString("0");

                    //輸出電流, 
                    double a = Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * 0.01;
                    la_Gw4CurrentA.Text = a.ToString("0.0");
                }
            }
            else if (iSlave == Roller.Slave) //士林變頻器
            {
                ROLLERERROR_Count = 0;
                int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                if (iFunc != 3) return;//只處理讀取的指令

                //士林變頻器 (指令頻率、輸出頻率、輸出電流...)
                if (iAddr == 0x1002)
                {
                    //輸出頻率, 
                    la_RollerNowSpeed_Val.Text = (Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Roller.Unit * Roller.Rate).ToString("0");

                    //輸出電流,
                    la_RollerNowA_Val.Text = (Int16.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) * 0.01).ToString("0.0");
                }

            }
            else if (iSlave == BalanceSlave)
            {
                if (iFunc != 3) return;
                if (receive.Length < 18) return;
                if (receive == "") return;

                //讀取的位址
                int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);

                //QBM301Address addr = (QBM301Address)ushort.Parse(str_addr, NumberStyles.HexNumber);


                //長度
                int len = int.Parse(cmd.Substring(8, 4), NumberStyles.HexNumber);

                //韌體版本
                if (iAddr == Units.BA_Version)
                {
                    //bVersion = true;
                    Units.BalanceVersion = receive.Substring(6, 4) + receive.Substring(10, 4) + receive.Substring(14, 4);
                    //la_FirmwareVersion.Text = "V" + version;
                    //btn_Start.Enabled = true;
                    //btn_Offset.Enabled = true;
                }
                //取得模組狀態
                else if (iAddr == Units.BA_Status)
                {
                    //Ready = 1,
                    //Busy = 2
                    int status = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber);
                    if (status == 1) Units.BalanceStatus = "Ready";
                    else if (status == 2) Units.BalanceStatus = "Busy";
                    //Status = (QBM301_Status)status;
                    //la_StatusVal.Text = Status.ToString();
                }
                //取得最後的錯誤碼
                else if (iAddr == Units.BA_Error)
                {
                    Units.BalanceError = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber);
                    //la_ErrorCodeVal.Text = "[" + ErrorCode.ToString("X4") + "]" + ((QBM301_ErrorCode)ErrorCode).ToString();
                }
                //Master or Slave mode status
                else if (iAddr == Units.BA_ModeStatus)
                {
                    Units.BalanceMode = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber);

                    //la_ModeVal.Text = ((QBM301_Mode)mode).ToString();
                }
                //Shock level
                else if (iAddr == Units.BA_ShockLevel)
                {
                    //bShockLevel = true;
                    int lv1 = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber);
                    int lv2 = int.Parse(receive.Substring(10, 4), NumberStyles.HexNumber);

                    //la_ShockLevel1Val.Text = (lv1 / 100.0).ToString("0.00");
                    //la_ShockLevel2Val.Text = (lv2 / 100.0).ToString("0.00");
                    Units.BalanceLock1 = lv1 / 100.0;
                    Units.BalanceLock2 = lv2 / 100.0;
                }
                //DO Status
                else if (iAddr == Units.BA_DOStatus)
                {
                    Units.BalanceDO = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber);
                    //la_DOVal.Text = ((QBM301_DO)DO_1) == QBM301_DO.Triggered ? "觸發" : "未觸發";
                }
                //Vibration(um P-P)
                else if (iAddr == Units.BA_Vibration_um)
                {
                    Units.BalanceVibration_um = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 100.0;
                    //la_umVal.Text = val.ToString("0.00");
                }
                //Vibration(G Peak)
                else if (iAddr == Units.BA_Vibration_G)
                {
                    Units.BalanceVibration_G = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 100.0;
                    //la_GVal.Text = val.ToString("0.00");
                }
                //RPM
                else if (iAddr == Units.BA_RPM)
                {
                    Units.BalanceRPM = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 10.0;
                    //la_RPMVal.Text = val.ToString("0.0");
                }
                //Balancing Step
                else if (iAddr == Units.BA_BalancingStep)
                {
                    Units.BalanceStep = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber);
                }
                //Initial Angle
                else if (iAddr == Units.BA_InitialAngle)
                {
                    //bInitAngle = true;
                    Units.BalanceAngle1 = short.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 10.0;
                    Units.BalanceAngle2 = short.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) / 10.0;
                    Units.BalanceAngle3 = short.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) / 10.0;
                    //la_AngleVal.Text = "[" + Angle1.ToString("0.0") + "][" + Angle2.ToString("0.0") + "][" + Angle3.ToString("0.0") + "]";
                }
                //Trial Angle
                else if (iAddr == Units.BA_TrialAngle)
                {
                    Units.BalanceTrialAngle = int.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 10.0;
                    //la_TrialAngleVal.Text = val.ToString("0.0");
                }
                //Angle
                else if (iAddr == Units.BA_Angle)
                {
                    Units.BalanceAngle1 = short.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 10.0;
                    Units.BalanceAngle2 = short.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) / 10.0;
                    Units.BalanceAngle3 = short.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) / 10.0;
                    //la_AngleVal.Text = "[" + Angle1.ToString("0.0") + "][" + Angle2.ToString("0.0") + "][" + Angle3.ToString("0.0") + "]";
                }
                //Narrow Band Vibration(um,p-p)
                else if (iAddr == Units.BA_NarrowBandVibration)
                {
                    Units.BalanceVibration1_um = short.Parse(receive.Substring(6, 4), NumberStyles.HexNumber) / 10.0;
                    Units.BalanceVibration2_um = short.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) / 10.0;
                    Units.BalanceVibration3_um = short.Parse(receive.Substring(14, 4), NumberStyles.HexNumber) / 10.0;
                    //la_NarrowVal.Text = "[" + Vibration1.ToString("0.0") + "][" + Vibration2.ToString("0.0") + "][" + Vibration3.ToString("0.0") + "]";
                }
            }
            else if (iSlave == Spindle.Slave)//暫時沒有要顯示目前轉速 電流
            {
                SPERROR_Count = 0;
                if (SpindleDev == 0)//Spindle 三菱驅動器
                {
                    int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                    if (iFunc == 3)
                    {
                        if (iAddr == 0x2B02)
                        {
                            if (receive.Length < 14) return;

                            //轉速 0x2B02
                            Spindle.OutSpeed = int.Parse(receive.Substring(10, 4) + receive.Substring(6, 4), NumberStyles.HexNumber);
                            int out_speed = (int)Math.Round(Spindle.OutSpeed / Spindle.Rate / Spindle.Unit * Spindle.ShowRate);
                            la_SpindleNowRpmVal.Text = Math.Abs(out_speed).ToString();//RS485 三菱驅動器 轉速
                        }
                    }
                }
                else if (SpindleDev == 1)//Spindle 安川驅動器
                {
                    int iAddr = int.Parse(cmd.Substring(12, 4), NumberStyles.HexNumber);
                    iFunc = int.Parse(receive.Substring(6, 2), NumberStyles.HexNumber);
                    if (iFunc == 3)
                    {
                        if (iAddr == 0xE000)
                        {
                            //轉速 0xE000 (單位 : RPM),<<<<<<注意>>>>>>反轉會出現負值,要加絕對值
                            //this.RW_OutSpeed = Int16.Parse(Msg.Substring(16, 4), NumberStyles.HexNumber);
                            //實際轉速(RPM) = 輸出轉速(RPM) * 單位(1) / 倍率(%)
                            int out_speed = (int)Math.Abs(Int16.Parse(receive.Substring(16, 4), NumberStyles.HexNumber) * Spindle.Unit / Spindle.Rate);
                            la_SpindleNowRpmVal.Text = out_speed.ToString();//RS485 安川驅動器 轉速

                            //0xE001
                            //Int16.Parse(Msg.Substring(20, 4), NumberStyles.HexNumber);

                            //內部轉矩 0xE002 (%)
                            //Int16.Parse(Msg.Substring(24, 4), NumberStyles.HexNumber);

                        }
                    }
                }
                else if (SpindleDev == 2)//Spindle 士林變頻器
                {
                    int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                    if (iFunc == 3)
                    {
                        //士林變頻器 (指令頻率、輸出頻率、輸出電流...)
                        if (iAddr == 0x1002)
                        {
                            //實際轉速(RPM) = 輸出轉速(RPM) * 單位(1) / 倍率(%)
                            int out_speed = (int)Math.Abs(Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Spindle.Unit * Spindle.Rate);
                            la_SpindleNowRpmVal.Text = out_speed.ToString();//RS485 士林變頻器 轉速
                        }
                    }
                }
            }
        }

        private void btn_PROBE_SafePos_Click(object sender, EventArgs e)
        {
            //端完安全位置
            TB_PROBE_SafePos.Text = la_EditMachAxis1Value.Text;
            double.TryParse(la_EditMachAxis1Value.Text, out double value);
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(19908, value);
            }));
        }

        private void btn_ExeMeasure_Click(object sender, EventArgs e)
        {
            //int ret = 0;
            TB_Ready.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 8, "未設定");

            //量測方向(1)
            //ret = focas.WriteMacro(588, MeasureDir);
            //if (DGV_Param1.Rows.Count > 1)
            //{
            //    DGV_Param1.Rows[1].Cells[Col_Param1_DoubleValue.Index].Value = MeasureDir;
            //    TArgument a = DGV_Param1.Rows[1].Cells[Col_Param1_PCode.Index].Value as TArgument;
            //    if (a != null) a.Value = MeasureDir;
            //    btn_SaveProg.Visible = true;
            //    btn_SaveProgVisible = true;
            //}

            //量測距離
            double dist = Convert.ToDouble(TB_Dist.Text);


            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                //量測距離
                focas.WriteMacro(589, dist);

                //量測位置
                focas.ReadAllAxisPos(out Pos);

                focas.WriteMacro(980, 4);//端測
                OneKeyCall(8999);//一鍵呼叫

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

            TArgument a = TB_Dist.Tag as TArgument;
            if (a != null) a.Value = dist;

            if (Pos.Machine == null) return;
            if (Pos.Machine.Length > 0)
            {
                TB_X.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                a = TB_X.Tag as TArgument;
                if (a != null) a.Value = Pos.Machine[0];
            }
            if (Pos.Machine.Length > 1)
            {
                TB_Z.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                a = TB_Z.Tag as TArgument;
                if (a != null) a.Value = Pos.Machine[1];
            }

            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;


            ThrMeasure = new Thread(() =>
            {
                Thread.Sleep(3000);//等待一段時間避免一進來就還沒Run 我就判斷程式結束

                while (bRun) Thread.Sleep(200);//等待程式結束

                this.Invoke((Action)(() =>
                {
                    //端測使用
                    TB_Ready.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 9, "設定完成");
                    a = TB_Ready.Tag as TArgument;
                    if (a != null) a.Value = 1;
                }));

                bFinish = false;
                double master = 0;
                Actions.Enqueue(new Action(() =>
                {
                    //MASTER
                    focas.ReadMacro(591, out master);
                    bFinish = true;
                }));

                int iStart2 = Environment.TickCount;
                while (!bFinish)
                {
                    int iTime = Environment.TickCount - iStart2;
                    if (iTime > 5000)
                    {

                        //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                        ThrMeasure = null;
                        return;
                    }
                    Application.DoEvents();
                }

                this.Invoke((Action)(() =>
                {
                    a = TB_Master.Tag as TArgument;
                    if (a != null) a.Value = master;
                    TB_Master.Text = master.ToString(Units.DisplayFmt);
                }));

                Actions.Enqueue(new Action(() =>
                {
                    focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
                }));
                ThrMeasure = null;
            });
            ThrMeasure.Start();

        }



        private void pic_ScreenDisplay_Click(object sender, EventArgs e)
        {


            bool bFind = false;
            if (ScreenDisplayProcess != null)
            {
                if (ScreenDisplayProcess.HasExited)
                {
                    ScreenDisplayProcess = null;
                }
                else
                {
                    bFind = true;
                    //WinApi.ShowWindow(ScreenDisplayProcess.Handle, (int)SetWindowPosFlags.SWP_SHOWWINDOW); 
                    List<IntPtr> list = ExWinApi.GetHandles();
                    foreach (IntPtr i in list)
                    {
                        string name = i.GetText();
                        if (name.Contains("CNC Screen Display Function"))
                        {
                            Console.WriteLine("Find CNC Screen Display Function");
                            WinApi.ShowWindow(i, (int)SetWindowPosFlags.SWP_SHOWWINDOW);
                        }
                    }
                }
            }

            if (!bFind)
            {
                String app = "C:\\Program Files (x86)\\CNCScreenE\\CNCScrnE.exe";
                if (File.Exists(app))
                {
                    try
                    {
                        Process p = Process.Start(app);
                        ScreenDisplayProcess = p;
                        p.WaitForInputIdle();
                        //ScreenDisplay = p.MainWindowHandle;
                        //WinApi.SetParent(p.MainWindowHandle, this.Handle);
                        WinApi.SetParent(p.MainWindowHandle, tab_ScreenDisplay.Handle);
                        WinApi.GetWindowRect(p.MainWindowHandle, out RECT rect);
                        int w = rect.Right - rect.Left;
                        int h = rect.Bottom - rect.Top;
                        //int l = (this.Width - w) / 2;
                        //int t = (this.Height - h) / 2;
                        int l = (tab_ScreenDisplay.Width - w) / 2;
                        int t = (tab_ScreenDisplay.Height - h) / 2;
                        WinApi.SetWindowPos(p.MainWindowHandle, IntPtr.Zero, l, t, w, h, SetWindowPosFlags.SWP_SHOWWINDOW);
                        //WinApi.MoveWindow(p.MainWindowHandle, l, t, w, h, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            //TempMaintanceTab = tab_ScreenDisplay;

            TC_Main.SelectedTab = tab_ScreenDisplay;
            PrevPage.Push(tab_ScreenDisplay);
            btn_Prev.Visible = true;
        }

        private void TextBoxClick(object sender, EventArgs e)
        {
            //if (isCellClickDisabled) return;
            //isCellClickDisabled = true;//輸入時不切換往復功能
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";

            TextBox tb = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                //int.TryParse(box.Tag.ToString(), out int no);
                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            //disableClickTimer.Start();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                if (tb.Tag != null)
                {
                    int.TryParse(tb.Tag.ToString(), out int no);
                    double macrodata = data;
                    Units.MacroInfo.CheckMacroMinMax(no, ref macrodata, out Limit p);
                    if (p != null && !(p.Max == 0 && p.Min == 0))
                        data = macrodata;

                }
                tb.Text = data.ToString(Units.DisplayFmt);
            }
        }


        private void GwWorkPieceTextbox(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            TextBox tb = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                if (tb.Tag != null)
                {
                    int.TryParse(tb.Tag.ToString(), out int no);
                    Units.MacroInfo.GetMinMax(no, out double min, out double max);
                    if ((min != 0 || max != 0)) // 0 ~ 0 就不顯示
                    {
                        form.uc_UserNum1.la_Msg.Text += "\r\n" + min + " ~ " + max;
                    }
                }

            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);

                GwWorkPiEdit = true;
            }
        }


        private void TC_EditProc_Click(object sender, EventArgs e)
        {
            CB.Visible = false;
            BTN.Visible = false;
        }

        private void DGV_ProgView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV_ProgView.CurrentRow == null) return;
            int index = DGV_ProgView.CurrentRow.Index;

            if (index < 0) return;

            //編輯程式
            btn_Prog_Open.PerformClick();

            DGV_ProcList.CurrentCell = DGV_ProcList.Rows[index].Cells[0];

            CB.Visible = false;
            BTN.Visible = false;
            TProcess p = TempProgram.Processes[index];
            ProcessIndex = index;
            TempProcess = p;
            SetProcessData(p);//編輯畫面顯示此工序
        }

        private void DGV_Advance_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void DGV_InProcArg_Scroll(object sender, ScrollEventArgs e)
        {
        }


        private void btn_DG_Btn1_Click(object sender, EventArgs e)
        {


            //focas.WriteMacro(964, 1);//離開
            //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
            //Thread.Sleep(1000);
            //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

            //顯示
            btn_Prev.Visible = true;
            btn_Prev.PerformClick();

        }

        private bool bOneKeyCall = false;
        public void OneKeyCall(int prog_no)
        {


            bOneKeyCall = true;
            focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");

            //new Thread(() => { 
            //    this.Invoke(new Action(() => {
            focas.PMC_WriteDbWord(PmcAddrType.D, 456, prog_no);//畫面呼叫程式
            focas.PMC_WriteDbWord(PmcAddrType.D, 458, 2);//模式：模式切到自動並執行，執行完返回剛剛的模式
            Thread.Sleep(500);
            focas.PMC_WriteByte(PmcAddrType.E, 4500, 1);//啟動 ON
            Thread.Sleep(1000);
            focas.PMC_WriteByte(PmcAddrType.E, 4500, 0);//啟動 OFF
                                                        //    }));
                                                        //}).Start();
        }

        private void btn_DG_Btn2_Click(object sender, EventArgs e) //對話式 加工對點 - 下一步
        {
            bool bFinish = false;
            int gw_no = 0;
            double DressToolPenSetting = 0;

            Actions.Enqueue(new Action(() =>
            {
                focas.ReadMacro(506, out double no);
                gw_no = (int)Math.Round(no);
                ReadGwMacro(gw_no);
                // 修刀設定
                focas.ReadMacro(558, out DressToolPenSetting);
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

            if (gw_no < 1 || gw_no > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                return;
            }
            int shift = (gw_no - 1) * 200;
            int shape = (int)Math.Round(CurrentGwMacro[10005 + shift]);//砂輪形狀(修整模式)
            int type = (int)Math.Round(CurrentGwMacro[10004 + shift]);//砂輪型式(0:內圓, 1:外圓)

            String filename = Application.StartupPath + "\\image\\" + (type == 1 ? "OIG" : "OCD");
            int dressModeGwType = type;
            //修砂對點 座標系顯示
            XmlElement xmlTools = machineSetting.GetGwTypeTools(dressModeGwType, shape);
            if (GWType[gw_no - 1] == MachineType.OCD2)
            {
                filename = Application.StartupPath + "\\image\\" + "OCD2";
                
                xmlTools = machineSetting.GetGwTypeTools(2, shape);
                dressModeGwType = 2;
            }
            if (GWType[gw_no - 1] == MachineType.OCD3)
            {
                filename = Application.StartupPath + "\\image\\" + "OCD3";
                xmlTools = machineSetting.GetGwTypeTools(3, shape);
                dressModeGwType = 3;
            }
            
            string path = filename + "\\DressGW\\";
            
            XmlElement xmlTool = xmlTools.GetChildNodeAt(0);
            if (DressToolPenSetting == 1)
            {
                xmlTool = xmlTools.GetChildNodeAt(1);
            }
            XmlElement xmlG55 = xmlTool.GetChildNodeAt(0);
            XmlElement xmlG57 = xmlTool.GetChildNodeAt(1);
            XmlElement xmlG58 = xmlTool.GetChildNodeAt(2);
           
            string Imgfilename = xmlG55 != null ? path + xmlG55.GetAttribute("Image") : ""; //修外徑專用                   
            Image imgG55 = File.Exists(Imgfilename) ? Image.FromFile(Imgfilename) : null;

            Imgfilename = xmlG57 != null ? path + xmlG57.GetAttribute("Image") : ""; //修砂輪左側專用
            Image imgG57 = File.Exists(Imgfilename) ? Image.FromFile(Imgfilename) : null;

            Imgfilename = xmlG58 != null ? path + xmlG58.GetAttribute("Image") : ""; //修砂輪右側專用(預留)
            Image imgG58 = File.Exists(Imgfilename) ? Image.FromFile(Imgfilename) : null;

            switch (DressGwStep)
            {
                case 0://請啟動輪→下一步
                    {
                        DressGwStep = 1;

                        //focas.WriteMacro(980, 1);//O8999 執行修砂對點
                        //OneKeyCall(8999);


                        btn_DG_Btn2.DisplayText = "下一步";
                        //string fileStep1 = filename + "\\DressGW\\G55.png";
                        //if(GWType[gw_no - 1] == MachineType.OCD2)
                        //{
                        //    fileStep1 = filename + "\\DressGW\\G55.png";
                        //}
                        //if (GWType[gw_no - 1] == MachineType.OCD3)
                        //{
                        //    fileStep1 = filename + "\\DressGW\\G55.png";
                        //}
                        //pic_DressGwStep.Image = File.Exists(fileStep1) ? Image.FromFile(fileStep1) : null;
                        pic_DressGwStep.Image = imgG55;
                        pic_DressGwStep.Visible = true;
                        la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 25, "請使用手輪移動X軸，使砂輪接觸到修整器。");
                        break;
                    }
                case 1://移動X軸，砂輪接觸修整器→下一步
                    {
                        DressGwStep = 2;

                        //記錄G55X
                        TB_G55X.Text = la_DressGwMachAxis1Value.Text;

                        //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G55.png";
                        //string fileStep2 = filename + "\\DressGW\\G55.png";
                        //if (GWType[gw_no - 1] == MachineType.OCD2)
                        //{
                        //    fileStep2 = filename + "\\DressGW\\G55.png";
                        //}
                        //if (GWType[gw_no - 1] == MachineType.OCD3)
                        //{
                        //    fileStep2 = filename + "\\DressGW\\G55.png";
                        //}
                        //pic_DressGwStep.Image = File.Exists(fileStep2) ? Image.FromFile(fileStep2) : null;
                        pic_DressGwStep.Image = imgG55;
                        pic_DressGwStep.Visible = true;
                        la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 26, "請使用手輪移動Z軸，使修整器往正向離開砂輪。");
                        break;
                    }
                case 2://移動Z軸，修整器離開砂輪→下一步
                    {
                        //記錄G55Z
                        TB_G55Z.Text = la_DressGwMachAxis2Value.Text;

                        //判斷砂輪形狀
                        switch (shape)
                        {
                            case 1://外徑→跳到儲存
                                DressGwStep = 99;

                                //通知加工程式目前的選擇
                                //focas.WriteMacro(964, 0);//無條件
                                //M Code Finish
                                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                                //Thread.Sleep(1000);
                                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

                                btn_DG_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                                pic_DressGwStep.Image = null;
                                pic_DressGwStep.Visible = false;
                                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "完成設定，是否要儲存座標系。");
                                break;

                            case 2://外徑+左側→跳到左側流程
                            case 7: // 正角度斜頭 負角度斜頭
                                DressGwStep = 3;
                                //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G57.png";
                                //string fileStep3_2 = filename + "\\DressGW\\G57.png";
                                //if (GWType[gw_no - 1] == MachineType.OCD2)
                                //{
                                //    fileStep3_2 = filename + "\\DressGW\\G57.png";
                                //}
                                //if (GWType[gw_no - 1] == MachineType.OCD3)
                                //{
                                //    fileStep3_2 = filename + "\\DressGW\\G57.png";
                                //}
                                //pic_DressGwStep.Image = File.Exists(fileStep3_2) ? Image.FromFile(fileStep3_2) : null;
                                pic_DressGwStep.Image = imgG57;
                                pic_DressGwStep.Visible = true;
                                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 29, "請使用手輪移動Z軸，使修整器接觸砂輪左側。");
                                break;

                            case 3://外徑+右側→跳到右側流程
                                DressGwStep = 5;
                                //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G58.png";
                                //string fileStep3_3 = filename + "\\DressGW\\G58.png";
                                //if (GWType[gw_no - 1] == MachineType.OCD2)
                                //{
                                //    fileStep3_3 = filename + "\\DressGW\\G57.png";
                                //}
                                //if (GWType[gw_no - 1] == MachineType.OCD3)
                                //{
                                //    fileStep3_3 = filename + "\\DressGW\\G57.png";
                                //}
                                //pic_DressGwStep.Image = File.Exists(fileStep3_3) ? Image.FromFile(fileStep3_3) : null;
                                pic_DressGwStep.Image = imgG58;
                                pic_DressGwStep.Visible = true;
                                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 30, "請使用手輪移動Z軸，使修整器接觸砂輪右側。");
                                break;

                            case 4://外徑+左右側→跳到左側流程
                            case 8:
                                
                                DressGwStep = 3;
                                //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G57.png";
                                //string fileStep3_4 = filename + "\\DressGW\\G57.png";
                                //if (GWType[gw_no - 1] == MachineType.OCD2)
                                //{
                                //    fileStep3_4 = filename + "\\DressGW\\G57.png";
                                //}
                                //if (GWType[gw_no - 1] == MachineType.OCD3)
                                //{
                                //    fileStep3_4 = filename + "\\DressGW\\G57.png";
                                //}
                                //pic_DressGwStep.Image = File.Exists(fileStep3_4) ? Image.FromFile(fileStep3_4) : null;
                                pic_DressGwStep.Image = imgG57;
                                pic_DressGwStep.Visible = true;
                                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 29, "請使用手輪移動Z軸，使修整器接觸砂輪左側。");
                                
                                break;
                            case 5: // 左錐度
                            case 6: // 右錐度
                                DressGwStep = 99;

                                //通知加工程式目前的選擇
                                //focas.WriteMacro(964, 0);//無條件
                                //M Code Finish
                                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                                //Thread.Sleep(1000);
                                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

                                btn_DG_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                                pic_DressGwStep.Image = null;
                                pic_DressGwStep.Visible = false;
                                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "完成設定，是否要儲存座標系。");
                                break;
                        }

                        break;
                    }

                case 3://移動Z軸，修整器接觸砂輪左側→下一步
                    {
                        DressGwStep = 4;

                        //記錄G56 Z軸
                        TB_G56Z.Text = la_DressGwMachAxis2Value.Text;

                        //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G57.png";
                        //string fileStep4 = filename + "\\DressGW\\G57.png";
                        //if (GWType[gw_no - 1] == MachineType.OCD2)
                        //{
                        //    fileStep4 = filename + "\\DressGW\\G57.png";
                        //}
                        //if (GWType[gw_no - 1] == MachineType.OCD3)
                        //{
                        //    fileStep4 = filename + "\\DressGW\\G57.png";
                        //}
                        //pic_DressGwStep.Image = File.Exists(fileStep4) ? Image.FromFile(fileStep4) : null;
                        pic_DressGwStep.Image = imgG57;
                        pic_DressGwStep.Visible = true;
                        la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 31, "請使用手輪移動X軸，使修整器離開砂輪。");
                        if(shape == 8 && (GWType[gw_no - 1] == MachineType.OCD2 || GWType[gw_no - 1] == MachineType.OCD3))
                        {
                            DressGwStep = 6;
                        }
                        break;
                    }

                case 4://移動X軸，修整器離開砂輪→下一步
                    {
                        //記錄G57 X軸
                        TB_G56X.Text = la_DressGwMachAxis1Value.Text;

                        //判斷砂輪形狀
                        switch (shape)
                        {
                            case 2://外徑+左側→儲存
                            case 7: // 斜頭椎度(左椎頭 右椎頭)
                                DressGwStep = 99;

                                //focas.WriteMacro(964, 0);//無條件
                                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                                //Thread.Sleep(1000);
                                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

                                btn_DG_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                                pic_DressGwStep.Image = null;
                                pic_DressGwStep.Visible = false;
                                la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
                                break;

                            case 4://外徑+左右側→跳到右側流程
                            case 8:
                                
                            DressGwStep = 5;
                            //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G58.png";
                            //string fileStep5_4 = filename + "\\DressGW\\G58.png";
                            //if (GWType[gw_no - 1] == MachineType.OCD2)
                            //{
                            //    fileStep5_4 = filename + "\\DressGW\\G57.png";
                            //}
                            //if (GWType[gw_no - 1] == MachineType.OCD3)
                            //{
                            //    fileStep5_4 = filename + "\\DressGW\\G57.png";
                            //}
                            //pic_DressGwStep.Image = File.Exists(fileStep5_4) ? Image.FromFile(fileStep5_4) : null;
                            pic_DressGwStep.Image = imgG58;
                            pic_DressGwStep.Visible = true;
                            la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 30, "請使用手輪移動Z軸，使修整器接觸砂輪右側。");
                                
                            break;
                        }
                        break;
                    }
                case 5://移動Z軸，修整器接觸砂輪右側→下一步
                    {
                        DressGwStep = 6;
                        //記錄G58 Z軸
                        TB_G58Z.Text = la_DressGwMachAxis2Value.Text;

                        //filename = Application.StartupPath + "\\image\\" + (type == 0 ? "OIG" : "OCD") + "\\DressGW\\G58.png";
                        //string fileStep6 = filename + "\\DressGW\\G58.png";
                        //if (GWType[gw_no - 1] == MachineType.OCD2)
                        //{
                        //    fileStep6 = filename + "\\DressGW\\G57.png";
                        //}
                        //if (GWType[gw_no - 1] == MachineType.OCD3)
                        //{
                        //    fileStep6 = filename + "\\DressGW\\G57.png";
                        //}
                        //pic_DressGwStep.Image = File.Exists(fileStep6) ? Image.FromFile(fileStep6) : null;
                        pic_DressGwStep.Image = imgG58;
                        pic_DressGwStep.Visible = true;
                        la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 31, "請使用手輪移動X軸，使修整器離開砂輪。");
                        break;
                    }

                case 6://移動X軸，修整器離開砂輪→儲存
                    {
                        DressGwStep = 99;
                        //紀錄G58 X軸
                        TB_G58X.Text = la_DressGwMachAxis1Value.Text;

                        //focas.WriteMacro(964, 0);//無條件
                        //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                        //Thread.Sleep(1000);
                        //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

                        la_DressGwMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
                        pic_DressGwStep.Image = null;
                        pic_DressGwStep.Visible = false;
                        btn_DG_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                        break;
                    }
                case 99://儲存
                    {
                        DressGwStep = 0;

                        //G54         G55         G56         G57         G58         G59
                        //#5221,#5222,#5241,#5242,#5261,#5262,#5281,#5282,#5301,#5302,#5321,#5322
                        //double data;

                        double.TryParse(TB_G55X.Text, out double G55X);
                        double.TryParse(TB_G55Z.Text, out double G55Z);
                        double.TryParse(TB_G56X.Text, out double G56X);
                        double.TryParse(TB_G56Z.Text, out double G56Z);
                        double.TryParse(TB_G58X.Text, out double G58X);
                        double.TryParse(TB_G58Z.Text, out double G58Z);

                        bFinish = false;
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.WriteMacro(5241, G55X);
                            focas.WriteMacro(5242, G55Z);
                            focas.WriteMacro(5281, G56X); // 目前是用 G57
                            focas.WriteMacro(5282, G56Z); // 目前是用 G57
                            focas.WriteMacro(5301, G58X);
                            focas.WriteMacro(5302, G58Z);

                            focas.WriteMacro(10102, G55X);
                            focas.WriteMacro(10103, G55Z);
                            focas.WriteMacro(10106, G56X);
                            focas.WriteMacro(10107, G56Z);
                            focas.WriteMacro(10108, G58X);
                            focas.WriteMacro(10109, G58Z);
                        }));

                        btn_Prev.PerformClick();
                        break;
                    }
            }
        }

        private void tab_DressGwConv_Leave(object sender, EventArgs e)
        {
            if (DressGwStep != 0)
            {
                DressGwStep = 0;


                //focas.WriteMacro(964, 1);//離開
                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                //Thread.Sleep(1000);
                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

            }
        }

        private void DGV_Redo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (row < 0) return; //例外處理            
            if (row >= RedoEnabled.Count) return; //例外處理


            if (CurrentProgram == null) return;//例外處理
            TProcess p = CurrentProgram.Processes[row];
            if (p == null) return;//例外處理

            if (e.ColumnIndex == Col_R_OfsX.Index || e.ColumnIndex == Col_R_OfsZ.Index)
            {
                btn_Redo_P01.Enabled = true;
                btn_Redo_N01.Enabled = true;
                btn_Redo_P001.Enabled = true;
                btn_Redo_N001.Enabled = true;
                btn_Redo_P0001.Enabled = true;
                btn_Redo_N0001.Enabled = true;
                btn_Redo_Input.Enabled = true;
                btn_Redo_Input2.Enabled = true;
            }
            else
            {
                btn_Redo_P01.Enabled = false;
                btn_Redo_N01.Enabled = false;
                btn_Redo_P001.Enabled = false;
                btn_Redo_N001.Enabled = false;
                btn_Redo_P0001.Enabled = false;
                btn_Redo_N0001.Enabled = false;
                btn_Redo_Input.Enabled = false;
                btn_Redo_Input2.Enabled = false;
            }



            //以下是處理按鈕
            if (col != Col_Btn.Index) return;
            RedoEnabled[row] = !RedoEnabled[row];
            if (RedoEnabled[row])
            {
                DGV_Redo.Rows[row].Cells[col].Value = Properties.Resources.BtnOn;
            }
            else
            {
                DGV_Redo.Rows[row].Cells[col].Value = Properties.Resources.BtnOff;
            }

            if (DGV_Redo.CurrentCell == null)
                return;
        }

        private void btn_DP_Btn1_Click(object sender, EventArgs e)
        {
            switch (DressPartsStep)
            {
                case 0://開啟砂輪→中止
                    {
                        //上一頁
                        btn_Prev.PerformClick();
                        break;
                    }

                default://其他→中止
                    {

                        //focas.WriteMacro(964, 1);//離開
                        //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                        //Thread.Sleep(1000);
                        //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

                        btn_Prev.PerformClick();
                        break;
                    }
            }
        }

        private bool bPassX;
        private bool bPassG54Z;
        private bool bPassG59Z;

        private enum M450_Condition
        {
            Next = 0,
            Exit = 1,
            Pass = 2
        }

        private void M450_Finish(M450_Condition Condition)
        {
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(964, (double)Condition);//無條件
                focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                Thread.Sleep(1000);
                focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF
            }));

        }

        private void ShowSetG54G59X(int gwNo, Image imgG5459X)
        {
            //focas.ReadMacro(506, out double val);
            //int gw_no = (int)Math.Round(val);
            //focas.ReadMacro(505, out val);
            //int shape = (int)Math.Round(val);


            DressPartsStep = 1;

            btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 32, "下一步");
            btn_DP_Btn3.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 33, "跳過");
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = true;
            //String filename = Application.StartupPath + "\\image\\GW" + SelectGwNo + "\\DressWorkpiece\\G54G59X.png";
            //pic_DressPartsStep.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            pic_DressPartsStep.Image = imgG5459X;
            pic_DressPartsStep.Visible = true;
            if (GWType[gwNo - 1] != MachineType.OIG)
            {
                la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 34, "請使用手輪移動軸向，使砂輪接觸到工件外徑。");
            }
            else
            {
                la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 170, "請使用手輪移動軸向，使砂輪接觸到工件內徑。");
            }
        }

        private void ShowSetG54Z(Image imgG54Z)
        {
            //double val;
            //focas.ReadMacro(506, out val);
            //int gw_no = (int)Math.Round(val);

            DressPartsStep = 2;
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = true;
            //String filename = Application.StartupPath + "\\image\\GW" + SelectGwNo + "\\DressWorkpiece\\G54Z.png";
            //pic_DressPartsStep.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            pic_DressPartsStep.Image = imgG54Z;
            pic_DressPartsStep.Visible = true;
            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 35, "請使用手輪移動軸向，使砂輪接觸到工件的右端面。");
        }

        private void ShowSetG59Z(Image imgG59Z)
        {
            //double val;
            //focas.ReadMacro(506, out val);
            //int gw_no = (int)Math.Round(val);

            DressPartsStep = 3;
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = true;
            //String filename = Application.StartupPath + "\\image\\GW" + SelectGwNo + "\\DressWorkpiece\\G59Z.png";
            //pic_DressPartsStep.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            pic_DressPartsStep.Image = imgG59Z;
            pic_DressPartsStep.Visible = true;
            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 36, "請使用手輪移動軸向，使砂輪接觸到工件的左端面。");
        }

        private void ShowSetDiam(int gwNo, string imgPath)
        {
            //focas.ReadMacro(506, out double val);
            //int gw_no = (int)Math.Round(val);

            DressPartsStep = 4;
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = true;
            //String filename = Application.StartupPath + "\\image\\GW" + SelectGwNo + "\\DressWorkpiece\\G54G59X_Diam.png";
            String filename = imgPath + "G54G59X_Diam.png";
            pic_DressPartsStep.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            pic_DressPartsStep.Visible = true;
            if (GWType[gwNo - 1] != MachineType.OIG)
            {
                la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 37, "請輸入工件外徑尺寸");
            }
            else
            {
                la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 169, "請輸入工件內徑尺寸");
            }
            tb_DP_Field1.Text = "";
            tb_DP_Field1.Visible = true;
        }

        private void ShowSetG54Length(string imgPath)
        {
            //focas.ReadMacro(506, out double val);
            //int gw_no = (int)Math.Round(val);

            DressPartsStep = 5;
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = true;
            //String filename = Application.StartupPath + "\\image\\GW" + SelectGwNo + "\\DressWorkpiece\\G54Z_Length.png";
            String filename = imgPath + "G54Z_Length.png";
            pic_DressPartsStep.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            pic_DressPartsStep.Visible = true;
            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 38, "請輸入工件右端面位置");
            tb_DP_Field1.Text = "";
            tb_DP_Field1.Visible = true;
        }

        private void ShowSetG59Length(string imgPath)
        {
            //focas.ReadMacro(506, out double val);
            //int gw_no = (int)Math.Round(val);

            DressPartsStep = 6;
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = true;
            //String filename = Application.StartupPath + "\\image\\GW" + SelectGwNo + "\\DressWorkpiece\\G59Z_Length.png";
            String filename = imgPath + "G59Z_Length.png";
            pic_DressPartsStep.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
            pic_DressPartsStep.Visible = true;
            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 39, "請輸入工件左端面位置");
            tb_DP_Field1.Text = "";
            tb_DP_Field1.Visible = true;
        }

        private void ShowSetFinish()
        {
            DressPartsStep = 99;
            pic_DressPartsStep.Image = null;
            pic_DressPartsStep.Visible = false;
            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
            btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
            btn_DP_Btn1.Visible = true;
            btn_DP_Btn2.Visible = true;
            btn_DP_Btn3.Visible = false;
            tb_DP_Field1.Text = "";
            tb_DP_Field1.Visible = false;
        }

        private void btn_DP_Btn2_Click(object sender, EventArgs e)
        {
            bool bNext = sender == btn_DP_Btn2; //Btn2:下一步, Btn3:跳過
            M450_Condition Condition = M450_Condition.Next;

            if (!bNext) Condition = M450_Condition.Pass; //0:無條件(下一步), 1:離開, 2:原設定(跳過)


            //double val;
            ////focas.ReadMacro(506, out val);
            ////int gw_no = (int)Math.Round(val);
            //focas.ReadMacro(505, out val);
            //int shape = (int)Math.Round(val);
            
            int GwNo = 0;
            bool bFinish = false;
            int GwType = 0;
            int shape = 0;
            Actions.Enqueue(new Action(() =>
            {
                //目前砂輪號
                focas.ReadMacro(506, out double no);
                GwNo = (int)Math.Round(no);
                if (GwNo < 1 || GwNo > 4)
                {
                    bFinish = true;
                    return;//例外處理
                }
                ReadGwMacro(GwNo);
                int shift = (GwNo - 1) * 200;

                //砂輪類型(0:內圓, 1:外圓(預留))
                //focas.ReadMacro(10004 + shift, out double type);
                int type = (int)Math.Round(CurrentGwMacro[10004 + shift]);//砂輪型式(0:內圓, 1:外圓)
                GwType = type;
                if (GwType < 0 || GwType > 1) GwType = 0; //例外處理

                //修整模式(形狀)
                //focas.ReadMacro(10005 + shift, out double mode);
                //shape = (int)Math.Round(mode);

                shape = (int)Math.Round(CurrentGwMacro[10005 + shift]);//砂輪形狀(修整模式)
                

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

            if (GwNo < 1 || GwNo > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                return;//例外處理
            }
            //加工對點 座標系顯示
            string path = Application.StartupPath + "\\image\\" + (GwType == 1 ? "OIG" : "OCD") + "\\DressWorkpiece\\";
            //XmlElement xmlCoordinate = (GwType == 0 ? machineSetting.xmlOIG_Coordinate : machineSetting.xmlOCD_Coordinate);
            XmlElement xmlCoordinate = machineSetting.GetGwTypeCoordinate(GwType);
            if (GWType[GwNo - 1] == MachineType.OCD2)
            {
                xmlCoordinate = machineSetting.GetGwTypeCoordinate(2);
                path = Application.StartupPath + "\\image\\" + "OCD2" + "\\DressWorkpiece\\";
                GwType = 2;
            }
            if (GWType[GwNo - 1] == MachineType.OCD3)
            {
                xmlCoordinate = machineSetting.GetGwTypeCoordinate(3);
                path = Application.StartupPath + "\\image\\" + "OCD3" + "\\DressWorkpiece\\";
                GwType = 3;
            }
            XmlElement xmlG54G59X = xmlCoordinate.GetChildNodeAt(0);
            XmlElement xmlG54Z = xmlCoordinate.GetChildNodeAt(1);
            XmlElement xmlG59Z = xmlCoordinate.GetChildNodeAt(2);
            String filename;
            filename = path + xmlG54G59X.GetAttribute("Image"); //研磨工件外徑
            Image imgG5459X = File.Exists(filename) ? Image.FromFile(filename) : null;
            
           
            filename = path + xmlG54Z.GetAttribute("Image"); //研磨工件左端面
            Image imgG54Z = File.Exists(filename) ? Image.FromFile(filename) : null;
            
            filename = path + xmlG59Z.GetAttribute("Image"); //研磨工件右端面
            Image imgG59Z = File.Exists(filename) ? Image.FromFile(filename) : null;
            
            switch (DressPartsStep)
            {
                case 0://即將開啟砂輪→下一步
                    {
                        bPassX = false;
                        bPassG54Z = false;
                        bPassG59Z = false;

                        focas.WriteMacro(980, 2);//O8999 執行加工對點
                        OneKeyCall(8999);

                        ShowSetG54G59X(GwNo, imgG5459X);
                        break;
                    }
                case 1://移動X軸，砂輪接觸工件 → 下一步
                    {
                        //記錄G54 G59 X
                        if (bNext) TB_G54G59X.Text = la_DressPartsMachAxis1Value.Text;
                        else bPassX = true;

                        btn_DP_Btn1.Visible = false;
                        btn_DP_Btn2.Visible = false;
                        btn_DP_Btn3.Visible = false;
                        M450_Finish(Condition);


                        //這顆砂輪，對外徑是使用G54 還是 G59
                        //TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                        //if (ini.ReadString("Shape", "GW" + SelectGwNo + "_DiamCoor", "G54") == "G54")
                        //{
                        //    ShowSetG54Z(imgG54Z);
                        //}
                        //else
                        //{
                        //    ShowSetG59Z(imgG59Z);
                        //    bPassG54Z = true;
                        //}
                        ShowSetG54Z(imgG54Z);
                        //if (GwType != 0)
                        //{
                        //    bPassG54Z = true;
                        //}
                        //else
                        //{
                        //    ShowSetG59Z(imgG59Z);
                            
                        //}

                        //ThrWaitM450 = new Thread(() =>
                        //{
                        //Thread.Sleep(1000);
                        //while (!focas.GetInput("M450"))
                        //{
                        //if (DressPartsStep == 0) return;
                        //Thread.Sleep(100);
                        //}
                        //this.Invoke((Action)(() =>
                        //{
                        //ShowSetG54Z();
                        //}));
                        //ThrWaitM450 = null;
                        //});
                        //ThrWaitM450.Start();

                        break;
                    }
                case 2://請使用手輪移動Z軸，將工件往正向移動至接觸砂輪 → 下一步
                    {
                        //記錄G54Z
                        if (bNext) TB_G54Z.Text = la_DressPartsMachAxis2Value.Text;
                        else bPassG54Z = true;

                        btn_DP_Btn1.Visible = false;
                        btn_DP_Btn2.Visible = false;
                        btn_DP_Btn3.Visible = false;
                        M450_Finish(Condition);
                        if (GwType == 0)
                        {
                            ShowSetG59Z(imgG59Z);
                        }
                        else
                        {
                            //DressPartsStep = 3;
                            //btn_DP_Btn1.Visible = true;
                            //btn_DP_Btn2.Visible = true;
                            //btn_DP_Btn3.Visible = true;
                            bPassG59Z = true;
                            if (!bPassX)
                            {
                                ShowSetDiam(GwNo, path);
                                
                            }
                            else
                            {
                                if (!bPassG54Z)
                                {
                                    ShowSetG54Length(path);
                                }
                                else
                                {
                                    ShowSetFinish();
                                }
                            }
                        }
                        //if (shape == 3 || shape == 4)
                        //{
                        //    ShowSetG59Z(imgG59Z);
                        //}
                        //else
                        //{
                        //    bPassG59Z = true;
                        //    if (!bPassX)
                        //    {
                        //        ShowSetDiam();
                        //    }
                        //    else if (!bPassG54Z)
                        //    {
                        //        ShowSetG54Length();
                        //    }
                        //    else
                        //    {
                        //        DressPartsStep = 0;
                        //        btn_Prev.PerformClick();
                        //    }
                        //}
                        /*
                        ThrWaitM450 = new Thread(() =>
                        {
                            Thread.Sleep(1000);
                            while (!focas.GetInput("M450"))
                            {
                                //if (DressPartsStep == 0) return;
                                Thread.Sleep(100);
                            }
                            this.Invoke((Action)(() =>
                            {
                                ShowSetG59Z();
                            }));
                            ThrWaitM450 = null;
                        });
                        ThrWaitM450.Start();
                        */
                        break;
                    }

                case 3://請使用手輪移動Z軸，將工件往負向移動至接觸砂輪 → 下一步
                    {
                        //記錄G59 Z軸
                        if (bNext) TB_G59Z.Text = la_DressPartsMachAxis2Value.Text;
                        else bPassG59Z = true;

                        btn_DP_Btn1.Visible = false;
                        btn_DP_Btn2.Visible = false;
                        btn_DP_Btn3.Visible = false;
                        M450_Finish(Condition);

                        if (!bPassX)
                        {
                            ShowSetDiam(GwNo, path);
                        }
                        else if (!bPassG54Z)
                        {
                            ShowSetG54Length(path);
                        }
                        else if (!bPassG59Z)
                        {
                            ShowSetG59Length(path);
                        }
                        else
                        {
                            //DressPartsStep = 0;
                            //btn_Prev.PerformClick();
                            DressPartsStep = 99;

                            pic_DressPartsStep.Image = null;
                            pic_DressPartsStep.Visible = false;
                            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
                            btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                            btn_DP_Btn1.Visible = true;
                            btn_DP_Btn2.Visible = true;
                            btn_DP_Btn3.Visible = false;
                            tb_DP_Field1.Text = "";
                            tb_DP_Field1.Visible = false;
                        }

                        /*
                        ThrWaitM450 = new Thread(() =>
                        {
                            Thread.Sleep(1000);
                            while (focas.GetInput("RUN"))
                            {
                                //if (DressPartsStep == 0) return;
                                Thread.Sleep(100);
                            }
                            this.Invoke((Action)(() =>
                            {
                                if (!bPassX)
                                {
                                    ShowSetDiam();
                                }
                                else if (!bPassG54Z)
                                {
                                    ShowSetG54Length();
                                }
                                else if (!bPassG59Z)
                                {
                                    ShowSetG59Length();
                                }
                                else
                                {
                                    DressPartsStep = 0;
                                    btn_Prev.PerformClick();
                                }
                            }));
                            ThrWaitM450 = null;
                        });
                        ThrWaitM450.Start();
                        */

                        break;
                    }
                case 4://輸入工件外徑尺寸
                    {
                        if (bNext) TB_G54G59Cal_Diam.Text = tb_DP_Field1.Text;

                        if (!bPassG54Z)
                        {
                            ShowSetG54Length(path);
                        }
                        else if (!bPassG59Z)
                        {
                            ShowSetG59Length(path);
                        }
                        else if (bPassG59Z && bPassG54Z && bPassX)
                        {
                            //DressPartsStep = 0;
                            //btn_Prev.PerformClick();
                            DressPartsStep = 99;

                            pic_DressPartsStep.Image = null;
                            pic_DressPartsStep.Visible = false;
                            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
                            btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                            btn_DP_Btn1.Visible = true;
                            btn_DP_Btn2.Visible = true;
                            btn_DP_Btn3.Visible = false;
                            tb_DP_Field1.Text = "";
                            tb_DP_Field1.Visible = false;
                        }
                        else
                        {
                            ShowSetFinish();
                        }
                        break;
                    }

                case 5://輸入工件端面位置
                    {
                        if (bNext) TB_G54Cal_Length.Text = tb_DP_Field1.Text;
                        if (!bPassG59Z)
                        {
                            ShowSetG59Length(path);
                        }
                        else if (bPassG59Z && bPassG54Z && bPassX)
                        {
                            DressPartsStep = 99;
                            
                            pic_DressPartsStep.Image = null;
                            pic_DressPartsStep.Visible = false;
                            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
                            btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                            btn_DP_Btn1.Visible = true;
                            btn_DP_Btn2.Visible = true;
                            btn_DP_Btn3.Visible = false;
                            tb_DP_Field1.Text = "";
                            tb_DP_Field1.Visible = false;
                            //btn_Prev.PerformClick();
                        }
                        else
                        {
                            ShowSetFinish();
                        }
                        break;
                    }

                case 6://輸入工件端面位置
                    {
                        if (bNext) TB_G59Cal_Length.Text = tb_DP_Field1.Text;

                        if (bPassG59Z && bPassG54Z && bPassX)
                        {
                            //DressPartsStep = 0;
                            //btn_Prev.PerformClick();
                            DressPartsStep = 99;

                            pic_DressPartsStep.Image = null;
                            pic_DressPartsStep.Visible = false;
                            la_DressPartsMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 27, "設定完成，是否要儲存座標系。");
                            btn_DP_Btn2.DisplayText = LanguageManager.LoadMessage(Units.langfile, "Message", 28, "儲存");
                            btn_DP_Btn1.Visible = true;
                            btn_DP_Btn2.Visible = true;
                            btn_DP_Btn3.Visible = false;
                            tb_DP_Field1.Text = "";
                            tb_DP_Field1.Visible = false;
                        }
                        else
                        {
                            ShowSetFinish();
                        }
                        break;
                    }
                case 99://儲存
                    {
                        DressPartsStep = 0;

                        if (TB_G54G59X.Text == "")
                            TB_G54G59X.Text = "0";

                        if (TB_G54G59Cal_Diam.Text == "")
                            TB_G54G59Cal_Diam.Text = "0";

                        if (TB_G54Z.Text == "")
                            TB_G54Z.Text = "0";

                        if (TB_G54Cal_Length.Text == "")
                            TB_G54Cal_Length.Text = "0";

                        if (TB_G59Z.Text == "")
                            TB_G59Z.Text = "0";

                        if (TB_G59Cal_Length.Text == "")
                            TB_G59Cal_Length.Text = "0";

                        double PosX = 15;// double.Parse(TB_G54G59X.Text);
                        double Diam = double.Parse(TB_G54G59Cal_Diam.Text);
                        double LPosZ = 16;// double.Parse(TB_G54Z.Text);
                        double RPosZ = 16;// double.Parse(TB_G59Z.Text);
                        double LLength = double.Parse(TB_G54Cal_Length.Text);
                        double RLength = double.Parse(TB_G59Cal_Length.Text);

                        //la_CV_G54XValue.Text = (PosX - Diam).ToString(Units.DisplayFmt);
                        //la_CV_G54ZValue.Text = (LPosZ - LLength).ToString(Units.DisplayFmt);
                        //la_CV_G59XValue.Text = (PosX - Diam).ToString(Units.DisplayFmt);
                        //la_CV_G59ZValue.Text = (RPosZ - RLength).ToString(Units.DisplayFmt);

                        //TB_G54G59X.Text = la_CV_G54XValue.Text;
                        //TB_G54Z.Text = la_CV_G54ZValue.Text;
                        //TB_G59Z.Text = la_CV_G59ZValue.Text;

                        //G54         G55         G57         G58         G59
                        //#5221,#5222,#5241,#5242,#5281,#5282,#5301,#5302,#5321,#5322
                        //focas.WriteMacro(5221, PosX - Diam);
                        //focas.WriteMacro(5222, LPosZ - LLength);

                        //focas.WriteMacro(5321, PosX - Diam);
                        //focas.WriteMacro(5322, RPosZ - RLength);

                        int shift = (GwNo - 1) * 200;

                        Actions.Enqueue(new Action(() =>
                        {
                            double val;

                            //focas.Param_ReadDouble(8210, 0, out double PRM8210);
                            //double rad = PRM8210 * (Math.PI / 180);

                            //G54          G55          G57          G58          G59
                            //#5221,#5222, #5241,#5242, #5281,#5282, #5301,#5302, #5321,#5322
                            val = PosX - Diam;
                            //if (btn_CurrentRotationCenterClicked.Tag != null && btn_CurrentRotationCenterClicked.Tag.ToString() == "out")
                            if (GwType == 0)
                            {
                                val = PosX + Diam;
                            }
                            focas.WriteMacro(5221, val); //控制器用的 G54X
                            focas.WriteMacro(10100 + shift, val); //加工程式用的 G54X

                            val = LPosZ - LLength;
                            focas.WriteMacro(5222, val); //控制器用的 G54Z
                            focas.WriteMacro(10101 + shift, val); //加工程式用的 G54Z

                            val = PosX - Diam;
                            focas.WriteMacro(5321, val); //控制器用的 G59X
                            focas.WriteMacro(10110 + shift, val); //加工程式用的 G59X

                            val = RPosZ - RLength;
                            focas.WriteMacro(5322, val); //控制器用的 G59Z
                            focas.WriteMacro(10111 + shift, val); //加工程式用的 G59Z

                            bFinish = true;
                        }));

                        iStart = Environment.TickCount;
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

                        if (GwNo < 1 || GwNo > 4)
                        {
                            Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"), "");
                            return;
                        }

                        double valG54X = PosX - Diam;
                        
                        if (GwType == 0)
                        {
                            valG54X = PosX + Diam;
                        }
                        la_CV_G54XValue.Text = (valG54X).ToString(Units.DisplayFmt);
                        la_CV_G54ZValue.Text = (LPosZ - LLength).ToString(Units.DisplayFmt);
                        la_CV_G59XValue.Text = (PosX - Diam).ToString(Units.DisplayFmt);
                        la_CV_G59ZValue.Text = (RPosZ - RLength).ToString(Units.DisplayFmt);

                        btn_Prev.PerformClick();
                        break;
                    }
            }
        }

        private void tab_DressPartsConv_Leave(object sender, EventArgs e)
        {
            if ((DressPartsStep > 0) && (DressPartsStep < 99))
            {

                //focas.WriteMacro(964, 1);//離開
                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 1);//啟動 ON
                //Thread.Sleep(1000);
                //focas.PMC_WriteByte(PmcAddrType.E, 4501, 0);//啟動 OFF

                DressPartsStep = 0;
            }
        }

        private void btn_Monitor_Rel_Zero_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            dManualZeroPoint = double.Parse(la_MonitorMachAxis1Value.Text);
            ini.WriteFloat("System", "dManualZeroPoint", dManualZeroPoint);
        }

        private void btn_Monitor_RelZ_Zero_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            dManualZeroPointZ = double.Parse(la_MonitorMachAxis2Value.Text);
            ini.WriteFloat("System", "dManualZeroPointZ", dManualZeroPointZ);
        }

        private void btn_DP_Rel_ZeroZ_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            dManualZeroPointZ = double.Parse(la_MonitorMachAxis2Value.Text);
            ini.WriteFloat("System", "dManualZeroPointZ", dManualZeroPointZ);
        }


        private void btn_DP_Rel_Zero_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            dManualZeroPoint = double.Parse(la_DressPartsMachAxis1Value.Text);
            ini.WriteFloat("System", "dManualZeroPoint", dManualZeroPoint);

        }
        private void btn_Offset_Input2_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Offset_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
            }
            double dVal = 0;
            if (DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetX.Index ||
                DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetZ.Index)
            {
                double.TryParse(DGV_Offset.CurrentCell.Value.ToString(), out dVal);
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                double new_val = dVal + data;
                if (new_val > OffsetMax)
                {
                    new_val = OffsetMax;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn_Offset_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (new_val < OffsetMin)
                {
                    new_val = OffsetMin;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn_Offset_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (CurrentProgram == null)
                    return;

                if (CurrentProgram.Processes[DGV_Offset.CurrentCell.RowIndex] == null)
                    return;

                string msg = "[" + dVal.ToString(Units.DisplayFmt) + "] -> [" + new_val.ToString(Units.DisplayFmt) + "] ?";
                ret = Fo_Msg.Show(msg, "", MessageBoxButtons.YesNo);
                if (ret != DialogResult.Yes) return;

                if (DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetX.Index)
                {
                    TOffset ofs = CurrentProgram.Processes[DGV_Offset.CurrentCell.RowIndex].OffsetX;
                    ofs.Value = new_val;
                    DGV_Offset.CurrentCell.Value = ofs.Value.ToString(Units.DisplayFmt);
                }
                else if (DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetZ.Index)
                {
                    TOffset ofs = CurrentProgram.Processes[DGV_Offset.CurrentCell.RowIndex].OffsetZ;
                    ofs.Value = new_val;
                    DGV_Offset.CurrentCell.Value = ofs.Value.ToString(Units.DisplayFmt);
                }
                btn_SaveOffset.Enabled = true;
            }
        }

        private void btn_Offset_Input_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = DGV_Offset.CurrentCell.Value.ToString();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Offset_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;


            }
            double dVal;
            if (DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetX.Index ||
                DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetZ.Index)
            {
                if (double.TryParse(DGV_Offset.CurrentCell.Value.ToString(), out dVal)) form.SetVal(dVal);
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                if (data > OffsetMax)
                {
                    data = OffsetMax;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn_Offset_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (data < OffsetMin)
                {
                    data = OffsetMin;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn_Offset_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (CurrentProgram == null)
                    return;

                if (CurrentProgram.Processes[DGV_Offset.CurrentCell.RowIndex] == null)
                    return;

                if (DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetX.Index)
                {
                    TOffset ofs = CurrentProgram.Processes[DGV_Offset.CurrentCell.RowIndex].OffsetX;
                    ofs.Value = data;
                    DGV_Offset.CurrentCell.Value = ofs.Value.ToString(Units.DisplayFmt);
                }
                else if (DGV_Offset.CurrentCell.ColumnIndex == Col_OffsetZ.Index)
                {
                    TOffset ofs = CurrentProgram.Processes[DGV_Offset.CurrentCell.RowIndex].OffsetZ;
                    ofs.Value = data;
                    DGV_Offset.CurrentCell.Value = ofs.Value.ToString(Units.DisplayFmt);
                }

                btn_SaveOffset.Enabled = true;
            }
        }

        private void btn_Redo_Input_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = DGV_Redo.CurrentCell.Value.ToString();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Redo_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;


            }
            double dVal;
            if (DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsX.Index || DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsZ.Index)
            {
                if (double.TryParse(DGV_Redo.CurrentCell.Value.ToString(), out dVal)) form.SetVal(dVal);
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

                if (CurrentProgram == null)
                    return;

                if (CurrentProgram.Processes[DGV_Redo.CurrentCell.RowIndex] == null)
                    return;

                TOffset ofs = null;
                if (DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsX.Index)
                {
                    ofs = CurrentProgram.Processes[DGV_Redo.CurrentCell.RowIndex].OffsetX;
                }
                else if (DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsZ.Index)
                {
                    ofs = CurrentProgram.Processes[DGV_Redo.CurrentCell.RowIndex].OffsetZ;
                }

                if (ofs == null) return;

                ofs.RedoValue = data;
                if (Math.Round(ofs.RedoValue, 5) > Math.Round(OffsetMax, 5))
                {
                    ofs.RedoValue = OffsetMax;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn_Redo_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (Math.Round(ofs.RedoValue, 5) < Math.Round(OffsetMin, 5))
                {
                    ofs.RedoValue = OffsetMin;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn_Redo_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                DGV_Redo.CurrentCell.Value = ofs.RedoValue.ToString(Units.DisplayFmt);


            }
        }
        private void btn_Redo_Input2_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Redo_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
            }
            double dVal = 0;
            if (DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsX.Index || DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsZ.Index)
            {
                double.TryParse(DGV_Redo.CurrentCell.Value.ToString(), out dVal);
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

                if (CurrentProgram == null)
                    return;

                if (CurrentProgram.Processes[DGV_Redo.CurrentCell.RowIndex] == null)
                    return;

                TOffset ofs = null;
                if (DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsX.Index)
                {
                    ofs = CurrentProgram.Processes[DGV_Redo.CurrentCell.RowIndex].OffsetX;
                }
                else if (DGV_Redo.CurrentCell.ColumnIndex == Col_R_OfsZ.Index)
                {
                    ofs = CurrentProgram.Processes[DGV_Redo.CurrentCell.RowIndex].OffsetZ;
                }

                if (ofs == null) return; //例外處理

                double new_val = dVal + data;

                if (new_val > OffsetMax)
                {
                    new_val = OffsetMax;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn_Redo_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (new_val < OffsetMin)
                {
                    new_val = OffsetMin;
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn_Redo_Input.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }

                String msg = "[" + dVal.ToString(Units.DisplayFmt) + "] -> [" + new_val.ToString(Units.DisplayFmt) + "] ?";
                ret = Fo_Msg.Show(msg, "", MessageBoxButtons.YesNo);
                if (ret != DialogResult.Yes) return;
                ofs.RedoValue = new_val;
                DGV_Redo.CurrentCell.Value = new_val.ToString(Units.DisplayFmt);


            }
        }
        private void btn_Monitor_ToChgPos2_Click(object sender, EventArgs e)
        {
            if (bRun)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 148, "程式仍在執行"), "");
                return;
            }

            DialogResult ret = Fo_Msg.Show(
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 149, "是否要回換料位置"),
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
                                        MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
                return;

            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(980, 10);//O8999 執行回換料位置
                OneKeyCall(8999);
            }));
            /*
            if (focas.Vendor == CncVendor.Fanuc)
            {
                FanucCnc fc = cnc1.Device as FanucCnc;
                focas.PMC_WriteDbWord(PmcAddrType.D, 1596, 8999);
                focas.PMC_WriteByte(PmcAddrType.E, 4402, 1);//模式：模式切到自動並執行，執行完返回剛剛的模式
                focas.PMC_WriteByte(PmcAddrType.E, 4500, 1);//啟動 ON
                Thread.Sleep(1000);
                focas.PMC_WriteByte(PmcAddrType.E, 4500, 0);//啟動 OFF
            }
            else
            {
                throw new NotImplementedException();
            }
            */

        }

        private void Fo_Main_Load(object sender, EventArgs e)
        {


            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                bCloseFinish = true;
                Fo_Msg.Show("Software is already running.");
                this.Close();
                return;
            }

            string[] DefAxisName = { "X", "Z", "Y", "A", "B", "C" };
            //初始化軸向編號
            for (int i = 0; i < DefAxisName.Length; i++) AxisNo.Add(DefAxisName[i], -1);//X,Z,Y,A,B,C

            //btn_Monitor.Lamp = true;
            //TC_Main.SelectedTab = tab_Monitor;

            ThrMain = new Thread(Execute);
            ThrMain.SetApartmentState(ApartmentState.STA);
            ThrMain.Start();

            //focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);
            //bInchTrans = Convert.ToBoolean(F2);

            ThrScreenDisplay = new Thread(ScanScreenDisplay);
            ThrScreenDisplay.Start();

            // 示例 Base64 字符串（实际应用中替换为你的真实数据）
            string base64String = "iVBORw0KGgoAAAANSUhEUgAAAPoAAAD6CAYAAACI7Fo9AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOvAAADrwBlbxySQAAVH1JREFUeF7tvVmvXVeWpXceEij4yb+gXgyUAfvBbzZQGQqFulAoIpRNVaVhAy7Ahv0P3L04I+Kl/ANs+MldZbmq3FSV7XLfZaZIUezEvhEpShSpniKpvm9jG3PvPc8Z5ztjrr3P5eGVMnkDGFhrjjnmXHM181IM3nvuouu6RYXFonti8ZOuW/zVrlv8lSAOcIAD/GAQPRm9GT266J5g/671MomxwZ/uEzDxAQ5wgB8uhp59mv280eiLRfc741eHAxzgAH9RMfTw79hG75v8X0LAP9N1i0e7bvHvd93iP+m6xX/ZdYv/AmPOE8pVfs45Ut/KV4E6rlutSbviWr7M73wENayTevW19kGdy+liyDE+8Z8bbi9wuVv+sHPtOftXRBxjGO/263T0V7450LhqfcYEoiejNx8be1V7d+jlZbOvGp1/kv/LprkJFuEKos7lcDzBHFVMlY+cq5Ogj3aL03GKo93SkN92fbUVlY6881d5qhindTnpYxzBOqilTb7K/18V8W6t0JJzdbn5VIyrIRG9Gj2LP9nXGr3/O7kKnkCS3Oi9QovUnHo49Dnw4HN0cZqbcYyp+G3gLoK5aCfHWMc5qCbmLv82+YjMV+UlsoYcee68j6qual21GcfcDoxxmKNRbVVrpSMc7ziXS884eld7efw7+9Do+n+8/SuNxTShzqkj57SVvvI5UOvsKT5GXqrTcl6Ba+0F95KjVS/t5NiQHFucq9VpmKOKdXA62pWPazGOnKuR+fcCl0vXirH1hYDgnjRO/2T/q8Of6sM/oSUZ/53/n5oCmLxaaC4Yz7U4T/zXRTxzqa7S0qfzjHf68OXc1Vut53JtiyqHy0/O+XOs9utGzlsxyjsuR65frcUcGl+tTe3UvOJoVz7WEWMF5iCYm3rHBf6zjb+zP7FYPCJE/KVeDz2T8SJoK6eHziKo3+tac8E1tgHjmEv3y1gF9+3gLthhai3WmKjO1qFa3+UOLTmHORpimxh3F7qP4Kt9Vaj0eQ5uzWpewWkqzvEOqYtezr7ue1z/s/0/NItVi5DXA1AwziF1U7HKc65jhVZ+2tRTx/UrHUHdXF8FxjCetTmtahxa+ZiHY0vHnMzv1mLMHDCO8czptDrSz3iXgzrVO44+ZzOv+v6D9f98X6x9x1v8kc+FAn/XJCJaPmoiX+VTzF2X/rTJEy0/fVXtFU//HORe3J7maGjn+VX+Kd5B98k4dwZcnzEOzDM3TvWMy5G5E3lWlZ9o1ZM5WhoHnlXlc+Ceo5ezr/seX/13/GZwIIrWzdOu+DnzCtTrJqt4d0jMU43MqVzOncbNiSlfyz8XUzl0D9TSdjHOR67itzmnqTUdr3nZqNS7HIypfJVGc7MZGcvamDM5p6eG/mxw1WpvbzS6LjZ3Iern5tjGR9tpaTuOPs6pZT6u34pp\r\n        gbn2iqm8rEs56lo812hxrbmzyTlthVYsfepnDnJO53JzTeqdhnW1xqncXEdRNjoDNUHO/8QsSptgTNjU\r\n        OOgGGTO15tThhI856ScXiBjGUUt7LvSSXA76VcealKty8V7oJ5c5XV63fvpzLad1Nbhcc6GxzDsHUzGt\r\n        827FOr3GMDb128YpykbnBdDmnJoKU0XPyUUfbfKV34HarMXlopZcFUMwxxy4OK6To6uDOnKOD/AxuTUdGFfpyfGcOFLrfLQrnnYFt2cX63T0U0O0dMxFv+rKRtdgBjIJOfJzCql81DmO/N9r+FgX/ZWvmhO6F+ZogTFTqGKrvFOc+nRs+aihTVRxc6AxVXxLo3vOeUvvYsi1UMVVa87xTa3b0pWNTmEim4io+IBbuKXfRpNw\r\n        awSnOXKuXwgqbYXQzNFVqC5DefruFaxZz6G1F54P/crnyHWm1iBCO+cu1J96rpW5qHc296c2czJPy19x\r\n        ewFrVLg61J7V6DxEFq484QqqOBejdrV+pa+4uevQ7/gWqM8crVzUtLQtZIzGM1fLp1xVh8a69RnnRuZ1\r\n        nFuDtgPzcF6tNUdD2/lcHGOnNJWPOcjnPFE2uooi8L9BkrBdwqooFsACnZ5+t0alr+pzWuq4FuMYS42u\r\n        nefGWBfnoNq5MXPh8vHc9N6ppa16xrZiNLfzT8GdcQVXk4Pz8wzSppZcnil5B/q3ORtqcx7rz2p0wh1s\r\n        cm6sCnSPIkaXv4prQXPlXFFpHejTPfFSlWdcC62HwPpSW61drcs8u0Lm1JHrOI5we2CM09AfqM6yFdvi\r\n        Ezx3jZmqrYrRevm2qvW4Fm3Vl42egRU0CZNT63ja5JiT66ieOvrngLHM4fzOx7kbiYxxfvVtC7c2R8ep\r\n        j2BuN2deck67rV559dF2eVUzJ9bxLVQxXLPSOK5aw3HK53zPjc4FdCH6A39/hoZaIvjAVA76dW3lws6c\r\n        qpvCXB0xp+YK1N8ruIfWGu78nK185aOuZTtfq27GU0ue/oqfQq677X1tu06glVvvibpmo2cTTBVU+Ss+\r\n        fVMH21o/OG7GwcU7boqPsVpPY9yc4/2CrkNQozqXR/nYt3I5Ju/iyZGvNC24eK2l0nCeduXXXIwj6Ne6\r\n        NJ65GOdycF5B74frZo6q0Q8dOnSA+4CNC63Ay5yLjP0H4BwYozy1lc/FVetWPOFqcjm4drV+lYec883J\r\n        WWlaoLaak2v5CF3joNH3F83LcDwfALltEM2fI3OErf7kmIN5Kl1yqSFYQ9jJad7UuDWYP3NQ04J+QdSa\r\n        qvWoo5brsz7mUp7nWdXA+ClE3oNG319sXMJewUejfOtxVRwxlaPFp8+BscpzJFw8beZz6zqevnvhnJ/r\r\n        kGuB6+To4OJiPGj0/cXGZRC8OHeBLZ3qK44+l7Nl5/wfGo52aFRH7Tb7Sk0rX4tr8QnW69bSWplPeben\r\n        qVjlqhyBqIt5lCMOGn1/sXEBu0Y+TD5YPoCw+YgrrYIxrceoubQuN7qYRGrIE1yLPtfATqsa+ufETcVO\r\n        cW6fqtO96Nm3ajto9P3FxuXxUhxXIbWKKZ/TqD2Xr+ZzUNVEroLmYdwUXB76mbNac8pHVH63Vour4OIS\r\n        B42+v9i4HL0Yh5aOvv/WcNTqPPUujjlC49YkUpvz5OesQa1bz+VhbNo65nzOnhnPecVpTvoIl5s1Uufg\r\n        8hDBHzT6/mLjEvYK91DJhU0u+epRaB6NVc75t4HLS5vrVTq1XaM4cI2I4zrMUdVAfXKZ061NXtevdFzH\r\n        aQg9j4NG3180L2bO5W0D5tHcOSfX8hHkXQ63/n9X5GI+x1eayjelUX7OOgr6XT7mVI5+rjtnPcYS6Tto\r\n        9P2FvRBe2K6Ra7gGq5DauTWGJmKmtOqv6tE8HFtxCbffuTHUx6jz1M7Zq8vL/Dlv1ccc22iTO2j0/cXG\r\n        JcyFewzJOahmTjxtl0Pn+gWAesao7bROx3kFp3drca4x5JNTn/OTJyrN3Hj6WGcFV+dBo+8vlofPy1K+\r\n        0lDvOF5yCy5W45mTugrUsl7ytFXnxioH8ynPnMwzlZuc81GjOeeu34p32rk4aPT9xcZluUvcBSLnfz9C\r\n        +bSVz/VVX8XlXPNrvMZU8USr1uR4TtRW3Bzw/GnfC1gTbXJ7WdudHXHQ6PuLjQvYNfTC3QNQbo6fo4Jr\r\n        aw7aLqbF008tY6h3PmdXesa4dVtcZTu4Glpcq3bVqP+g0fcXG5fMy2n5pvA/GG5ujr3q3Jqpo5bxqok8\r\n        Tq88/eGr4lyM07XQinX506aPGuX0/Fp68pWmwkGj7y82LoDIx0t+L2jlUV+lc02W2iqG8VNzB/q1oZmH\r\n        9txc5B1yPeqrecZUvoqbQq7fam715xr6Reeg0fcXG5fDRzQXVWzaOjqohqCWMeSUr/K6ONpOW+VmXCu/\r\n        i3Ox1GlN1M6B0zM3Oeoqv/roJxc4aPT9hb0kd4G7gMv7j0bQn5yLp85pHVo6V0PCcQRroj841bTO2sXP\r\n        xTaxXLuqR7XOP7Wmy3vQ6PuLyUuaQjaay+P4tJ1P/c7WZiFfxZDX9elzesZSp/noY46Kr2qq9A5u/ana\r\n        XCxj4ryTY7O29pDxThM4aPT9xdqFOOjFToGxzMF81Kme2laM6ulr+Zmz0rU4zaFjxTOeOemnxnGaq/I5\r\n        P3Mx1sUQmcNpXVxyB42+v9i4iCnoxbqLbKHS/2PDVUht5gqbeVv50qealr5CxCToI1LnmuFeoHuZqkM1\r\n        sX6lJ89adR9O14pX30Gj7y82LkwvpfWYK56xjqPP5eNc/c7myPxci3PqdY1K08rB2EpPUOviuIbLWelo\r\n        k9e4Km+lc35yqTto9P3F8uAVvMQ5qOLD/ifmomOefAX1uxwKVwf1lYaghvYU7/ZFbdYSWvocuA/6A5or\r\n        x6oWp63Gau7qcD6nO2j0/cXGBcxFPBT3iAin4yOkv9Ixr3tA9wJ+YZlan75sIKd3sbRzrPZVxVWc8ye0\r\n        IefuVe0WR1+uo/Z+NvrfOXKx+9mztx5oLC+I4EXtBcxJqK6aq1ZHx3Pu4GrQucvBdahvcfRxfcdX8U5P\r\n        v8axNuqocSPnjHH5yOuY2M9Gf+TIbV+U4/5H47uf0HXnrp06p9e9qd9pqZ+D6qKJ8OeaLb2rq7W/zEdu\r\n        SstxCtSp3Xor6mvlqLg5dbZ8riaNcfvIuZ57aw2N5RvTuLC/10ZXsDjlOa8OMX0JxhF6qA6t9XSdVt2M\r\n        DW2CMfcK3TfPIDmOCmqpz/Miz1i3nmqor/wV7+y551nlyLVoUxMjz0FzkSOvfq6pPHM4nrZyzFs3+jOC\r\n        oVGfEQT/TI+0V1jqnhlwaMSy0VkgNzw1r2yXz2lcDo13OVo+apifsVxrL+Dac8HYKp/jnN/V1cqpvNPQ\r\n        1xpb+aZ8zOHyp97xzs81Ko55mIt5OVfufyryUr+vjf7c7aGwOZup/OR0dLkT4Us/Y9OvOuZyNrkfAtxZ\r\n        bQM9h+SqnPrAtjk35nFrEroOY6t4rU9zVDbrp5+xodV3V50BwTNLPvNxXdqMSbs6o+DrRj/UHZYmXzXx\r\n        4TUfm5wNf+iZ0A8x/Z/oLNZtloUqr35nM0YPiXFuPpW3xbl8tFN3v7DNGtRqvdwDYzWemmq/tDWWMS43\r\n        5y4XR9VtE8NYxhDkw84vAm4N1TM/UWmYu9IFqkY/fPhwd/jQ4WFUgDukvmcPrTQm9pEjdzYPg0Wqnzz9\r\n        yv/PM3Mxjj6nczHVOIXQJehrQeNasa5mwuXimMhzdesyT2i5to4uBzWqVY4gr7aLd7ldbMVnHPNVOV0O\r\n        2vQx3xwt4zjPsWr0Z599tnv28LPDqHDcEofXddA++hwaPaCPw3ExT1s34uIIbnYu9JDnrKPQejWeX4ju\r\n        F7atN2raS0yMulfuOzkXNwXG0d4LqhzK61lUo9O0EJpt3qHL6c424XJqfYmq0Y8cOdIdebbAkSPds0ee\r\n        7dHrUjvyRxLPruPRo2OjV0U7nkVXOmoYPxd6OK01W35yrq4WP+WrwBpYI3XO77TkWn7mpp66Kg95V6uL\r\n        c1odK445qHdrunzOT53TVmtwzrXIUZd21ejPPfdcE0eOHulBvsdRwz33XPfY0bvrG0/8U8Np4dwYdS6e\r\n        m5/SBsJHPfNxTmgu8up3YK4WpuK4vsaRY0yOc/ecMbqm6l0+5Z1POfpoT/GJ6kzmxE5h23jqaVfgnTOu\r\n        8lWNfvTo0R7PCVbcc91zxwYkt4Zjhjt6tN3o7hIcV/mmbB6Irul8apNXv+bhqLl1LeWqteYi83EPFbZd\r\n        L/Jyv625jq39Ba/nkOs4vdZQjcytc2qrGPqrerhOpWEd9JHbNXSNqtGPHT024NiAvnkVx48ufUvN8ZyP\r\n        vszR5znaPXZsbPQsgIe77dxtyOWv4mjH/H8peBfDdelzfAWXaw5PuBjHt+Yujj7WQI5z5qGO8c5PTuOq\r\n        eJeLsczTiqv8lUbzT8VRr/54l8zDWIfMUTX68WPHBxwfcCyaGEjfSpPzY6v4ZZ5jQ6Pn4lG4NtVU4erL\r\n        TVfI/OQ1T+VXDdGKmQte4P0C87t9U5Nwj4oxLh+xzZ21/O6tBGhXPs1dzeciY1xs1siz4ZkRjg+udVct\r\n        n0PV6CdOHF/D8RMnRozc8cCJTaxpVnx8AXjs2Lv+oKtDq+aKKsZxGtPSMreLc1rm5GML6EPg2oxvIfWM\r\n        Yd3UuhjqucaUjmu4WPrIMQd5N6+4ls0562IswXj6nH/OGtS4/A6Vlm+savSTJ092J0+eWOFE2Ce7Ez1/\r\n        sjt5YuBOBJ8Yfb1G+R4nusePj43uNpXzOQfixiqHctQyZxU/VzsV16rhXqC5WR9Hzh2YV3MQzMfczsd4\r\n        p6/81JFzNRGuJs63Bet2uRznYjmn7n/FOk5L1I3+fPf8yZNAcImBC12P51dY163wxPH3NgtQxAZ05Mao\r\n        Z6zTZK65aOn1EnI91TM27ITTzL2kvYA5abd4xwX0flztjKOtefQs6WdOp3Fah+rsqck8fEfcc86ZK21X\r\n        j+aYw2UerqFcngs1CvVVjf78qVM9TvV4fsDzgVMDlr4Bvf7086uYUdfz43zZ6FVxjieXh6wHRY6oDpN6\r\n        +gjqqnFOjilM6Vs5W5yOhItpxVGvOpeDtlvfcVUc16GPuejT0eWh363rNG4kR1Q56WM+2syTqBr91OnT\r\n        S5zucao7fSpwesCSH3Dq9Kkl1vnT3akx5okT7w3F/G+mKG6aB+H4qdicx3qOTzh/2jFmvW4d5moh87q6\r\n        6aeP/imtolW/znV0efXemIv5uCZt5qjA2sirzRqYi2BN5HUfrdy0lW/FVeurjuszx5SdqBr99JkzA04P\r\n        OHPmdI/T49gj+DVkgw/z3pY8y0ZvFR7QA+BhVbpqTlBXaYOv1nVgLl4Q+W2R5+Dgcuec6zkN16l8RK7v\r\n        1uJ5KPJsdS3NwTjHtXj63J5czfQRbj2Xk3ZLwzusdIxxOu5TdVWjnzl7pjtz9mx35syAs729QthnzwTO\r\n        jkg7viis7D7HmOeJk++vF6gHwcNwfm5eecZzJFfFu1zkmNvp3OhyzOGYvwXqqzyOb3GcuzUJaphDNS7G\r\n        +ahhXMtfjRUqf+bUPOSUZ/xUTLU2tczDeEXV6GfPne3OnDvbnT13rjt7NhDNHqC9jnOBM8MYOXq+z3O2\r\n        ++mJ91cF/u+mMG42NNS15lNwB5WIdZwv16/gYlrrOOiF0DcHGs8ctOf4eBZz5hnn5tSRc3U73mmoJxfg\r\n        HVa15Zz6Fqidsp2v0uj7r9A6I42tGv3c+XPd2fPnunPnz3fnzgXOjdD5Js4LIof6fnryg82NsPA5fOV3\r\n        Ptrb5pmrUaSfh97KtQuwhl2v18rHs3C+KWxTd2qq5k5o8wayMXQNzt367i4r7RyNW5M65XRUXdaVewq4\r\n        WutGP9/j/ATm6gJPnvhgfVMsmhx1U353IC0/1027xVW5E/+HieWc0DWISlfx9NN2+oqbimF+FzcHrCHm\r\n        eY45Mjdr0Dnr4VjloU2f8s7nOPKVhv5tYqjJ+jRX1egXLlzoLly8MIwXLva4eOFCj4EjLnYXLwYuDJC4\r\n        1DwZf6LHxWkzVFCNm09tPnROow8o/a4m5VyMIriqLtpT0EtqxToNz0lr4v507+Q4J7hX1tHKq3B7cLU6\r\n        vtp3peNZuDXI0SYfI9cjuDfu1+W9VzB/5K0a/dLFiz2G5r3UY5PLefCXukuXAhcHSFyvuXSxe/L58U90\r\n        PSgWmZegl9Hi3Jw5NTfzMZ5ax3F0OajheD9Q5eba1LFu3Qt9bk6bOVoax09pHKZ0Lb7lc1wVw3rnarkW\r\n        89DP/M7PvGWj903bwLKxBZdHkB/x5PMfbhZAWzfAjWhM5VfbaZ2GnNNxnvb/Wfiom9KoVtHy05f+qCkw\r\n        Z93Uam7mq2w3z1z0O5s882k9VWyVh3uhvQ3v/Hq+DoxtadNPjYudqquKrxr98uVL3eVLl7vLl4EXhvGF\r\n        hq/ifhaN3mqM6tHpXB9SlWcOmGcuskYX77htUeWei9b5zgXPnn6He6nZrcF8TrNXMHflm6tzvPrzPPVu\r\n        uR/aRMY6neMYWzX6Cy+80DdpjIrL43glucsvdFcur/uC64EcPzv10fpBOKSPhbriaZNz/hacnhz9Dk5L\r\n        rsq7K7B+1uB8GsealFe/jtuC+TWPy7kX/ZyRMQ4up/qquYuZMyeqOqv8tKtGv3Llyr3hhQEvXBkQ3FPa\r\n        6FXxOrqCE/9XI55IPmNipLY6LF2HMVN6F8d8FZhjLhhb2eSpafmJ3BPjnM1Yx6utd+b8VRxtd+dORw3t\r\n        CsxRradn1Yon52znoybtqtGvXrnaXb2auNKA6tb1V65cHXB1wFqjs9CAaxKH0FXaiq/8WYt7UFNw2ona\r\n        fnRkscTvChz3u8+NELvXJK8+p8+8I7e2dubKuOSYY7SX6yr37IpbyyW5l6NoN/Khnr/OPKaepY09pYbn\r\n        vnEnyem9O10L1DJfpUu0+qBVT/L5BYX+1KhdNfqLL764xLURyr14bQT5Bp46PTa6FtsqsDo0jZ3jq9Zx\r\n        emorP+MZqzrRLx/qg4aju0N8Meq/IKmNceOO/m9zp7wz+mkrz/kU53wuP21CY6hVXvcbqBr92rVr3bWX\r\n        XuquXXupe2lEzAeE71r3YuDagOT6ccRLI9LuG721ybnQuNhQQn3MnXbqvic8dGxx39A/8gcAsdffPb7o\r\n        Hgo79p37Pz7i2GJ15nwHxJS/At8gfY5jjhb0PTMXwdzsiUDV6C+99NKO8PIwXnup+/npjzeLZHE511E1\r\n        LX4uNM8+46F4oAe4J2RDL+0Tq7H3nZBG1ztPtDi+kSmuxbMB54I1Oh813AvjqkZ/+eWXR7w04HragpdG\r\n        kHd46eXu52c+8QU5jqP6ybX8Lk/M/x8cxC7BmgQPnVwcYFeIxj45gL7m3es7cXbFt3KZuy59zKc2UfEV\r\n        Kn3Z6Nevr+H69Ze76zG+nAhbQV/ErOdYa/QoKJpNi0lbxyxctdVmmK/C/WzyCTz0/OIA94gf6zwa+9SA\r\n        nId/401s8zbItfhE3jH5Kkdo03axWvfU2oxRO1E1+vVXXlnDK69cH3D9lRGjrUjfkovY6z1i/gttdBZF\r\n        bu4GqeNc/Wp/T/hxPMQD3BMeOr0+riG406bR+WbcfA7yLite/W6utsYyXxVD22mdrmr0V165scKNV1bo\r\n        mz6RGrF73fW1mBuBaPSzY6OzGIdqA46b40v8v+aQdg2uKfjxmcX9w/jIf4h4eIdY5j272vvDcg6hWZ65\r\n        uw/leGecx3thPHO5NRyY241ExU/5iKrRb9y42d24OeDmzRsDbgy4scTNESv75qjP2CF+wC/OfjosGodX\r\n        HeBcPjdJPm0dq/XUt0/oH+f9Ahv/B4RoxF2h3+u5Rffw2QG5f7XLu654+uY0EWPS5tjKxxwVKl2VV/mI\r\n        rRr91VdfXcdNxc3u1VcT0I24KUhu2ei5OA+ZHA+MfMVpDO1Kv0twD4KH44E+iDi/Y5xbdD8ex8BPsM7a\r\n        Pcy4l6W/0qm/0jCe78Kt4XLRv82aGqN22eivvdbjtcCrgVfX8dqr3as9QjPY6cvY3tfHDvjl2c82i/z/\r\n        ioKVnwNu1iE0mTfnFRi7Izx8YfFg4uIOIXl/omtEk4/z5Znzwbv5tpjK45qTurRbmkT1Hluxun6MVaO/\r\n        9vrrA157vXu9x2vreP21pSbmyfdfGDI2fMv411eNrkVxE9pobDxuhjk4KiK25d8n/OTi4gD3CQ9fGr4Q\r\n        xHx55u4Lt3sH5DgS+iZzDQfGZaz6ORL0a6y+61ZM1eivv/F690bfxCPeeGPA64HkBzt0PSJmqUndOH/j\r\n        je7pc59tHkTgT4vitGgFN0t7Kp45HDR+h/jJ5cX9w6UfLh7ZIXKvy7ng4XHcuE/ee94JOerJ6306f7xl\r\n        amlX67h8lbaVy/FVo7/55hvdm2+80Tdujzff7N54QxH8MA9dj4h5M3mJDfvNN7unz4+Nroehdo6tTbTA\r\n        vFWO0FG7T/jJlcX9wws7BL+I3CMe2TFiv4+8MKCfBxf2OPLc15Bvwr0Nh6m3Ur2z9JHbK6pc7B+HutHf\r\n        HBs3RkE0NrkljF7QN3o2WVVUVbSzmYs281VaB9a1I2w05w8Uj+wYj+4QrG9pX110PxnRvEe+AedzUJ+b\r\n        M5bvya1LjcvDfHM01FWN/tZbb+0cT5//fL0QbrYqlODm3Capb9n7iEdfXBzgHvFIjFeHMZq7H0cuNeWb\r\n        0Mcf458ZH1Hx9Ld0br3kXRz5iNN1ApmLOZkjUDX6rbdvCd4ecGvA203c6m4FJD59T18YG70qTPmY60Zc\r\n        jPPn3OkDekDfA/hoHxhc2x0eeWndfkx947g887l37ZqN75E6vruWdg4ibm6M2xc5zVU2+q13erxz69Ya\r\n        br0zNnIi7MSSi1j4b93qfu/CF+tF8XD00CpUWm7YgT6N2SW4ruDReKT3CdEAP1i8vDv0+81xzL08h3Fu\r\n        74N3o/6W1unoxz1vaF2OKtbldjmoJVRTNfo779zu3rl9u7t9+50B77zTvRO4PYy344tA2iO3xO3bMk//\r\n        7e73o9FZjOLPDVfxuukppM7l2Wc8en1x/xCP/AeKx3aI2OtjI/r5yOm8bAQ2Ef3J61vhu2GczkPrcjJ+\r\n        DkdQQ5tQf9Xot/smv93d6RHNrvbt7s47g337zjhu4M7Kf2f4orGnRg9OQT+hTe1iaO8zHntlcYAd4dEb\r\n        4/zGons87JGP+dq5V3fu3kflr+ZujZzzCwb9czXktQ76K1SNfufOnR5379zusbLvdHdvD+i5u+O4gbsD\r\n        wn/3Tnf7zp2h0V2hPEBuoIohHO+4Fn+f8Xg8yvuEePAPCpb7vSn7vznixtjoevbV29F3xrfmtK0caT9j\r\n        tMzt4qp8jG+B60UtMVaNfvfu2NQx3r27Qt/sMQo3Ihp60Ay403Mr/P7FL1cLT6G1sSpH8M6XHC+jBebY\r\n        ER5/dXHf8NgDgtzr49HUuf9o+LBfW3RP3FxsnPsatBHnYEqbjTmlc9D31np36tvLOlWjv/vuu927d98d\r\n        xjXcNdyAuzmPOMSG7/cvjY0+1Uzqa82n8lTYS8yO8PhriwPsAE+8Po7R5DEC5R3z3VRviG+N8XO0LsbF\r\n        axxzKahnXvrVrhr9/ffe28B7ifeVex9Y6Rj3B9noLMZtiL5DM3ShcToXk/P7Aa4peOKNxX3D4/H4HwD0\r\n        +835iMdj/3IOPPfmG6A/+fQ5jeOc341u/Tn5GOviK3/Z6O+/vzO8N+IPLn21uYEKVcMGr5uhjjY3XukI\r\n        Pdgd4ok3FwfYMR5/c9H9NJpcxvKe4x6Sjzn9LaRWY/JuVUebmPoDiT7a2yLqKRv9gw+6D5r40HCbvsiT\r\n        +ENt9NysHp6zteCKcz7mcjH3C7oX4KdvLQ6wC0RTv7XonhjtJ94e5jlu3EkF9xb4jsjR1remtoJxbqSO\r\n        tvO5eNUmqkb/4MMPuw8//GBEzIEPVvNBu4nkYwz84eWvNjegRc7FYYxz8vAwvif0D/EBxE93jD7nLSD9\r\n        txbD20jkW4k7aL2LOTZzZf4WWlrN79aiL9+S1qBvjHHprxr9o48+6j766ENB2CM+HKEc8OEItf/w8teb\r\n        TcoxMWVXnOPdGtWh7AKxToEn3lk8kPjpfcKTtxbdkzGPRr89zt9ZbN4J75tvpOVjrGqondJwnmAsteRd\r\n        bkI1VaN//PHH3ccffdyPH3380YCPPhYMdq8ZEdzHHyciLjE0+x++gEYnXIFuYwTjKh3Bg9kVuI4gHuMD\r\n        iTs7hOR7csz/5DhPe3m/vGdn63ugXzkdGedimZd5mNONbt7SpJ+aqtE/+fiTAZ980n3c4+MV9/En3ccj\r\n        lPvk44+7T0LXYz02Gv9vZKMTLLDi/hLgybuL+4d47A8AfqYjziB9yzOvGmQKjFM8a7iq2Rjj/GzWnOs6\r\n        Ls7lr1A1+qeffLrCp0DL18DfeOGbzQJYaIwJ+hw0jqPmaeXYR/zs3cUBdoAn31t0T8Y4ouej2Uf70KFD\r\n        3aFDz3SHDh0e54e6w+N46PCh7tAz4zztcf5MjOF7ZoxL3WHxLfWr3OmPNfqmnHrDtJVzPudnfof0V43+\r\n        2aefrfDZZ92nn33a47PAyH8a+GxdN2gHvWqD/5tXvlkvwBXl5g7csPKtg2F8Bc2zQ/zs/cV9Q//4HwD8\r\n        TBF71/mIZQOOzddDmrtv2rE5+3nvXzVucjmuEF88jO9w8M/03MZb4nviG3ScA3WMp19jq0b//LPPV/j8\r\n        8+6zzz/r8Xngs8Dn3WeBz6lLDM2tvr7Rj5gNEFpk6nNjYSvnYlrc94yffbA4wC4RjW24oXGlEfv50IjL\r\n        hu2bM5tcdfjTX8ceh9dz638RxPzwM5vNp+8+5srzPc/pEUXqGad5q0b/4ovPB3z+heDz7vOeS36YB6dI\r\n        fqUbtMtGrwpKHw/FbUL9GsORXJVnl9CagI1H+aDgw93hqRE/+2jRPfWB2OJfNWU2Yza5NLc09vJPdW3Y\r\n        Zfwz3eHAM0OD638FrKHPNfxXgX0PLZtz906dzsW4sWr0L7/8YsAXX67hi55LfpgHlxhiFBn7Rfe3rny7\r\n        2iSRBW1j60YJ8rSZY5fgOoKn4nE+gPj5DvHUxyM4/2jR/Wy0jx4/2h07erw7duzoiGMDjo7jcj74j/ac\r\n        aoMb5r1vjIt55O5Hxoz5Ur98C/omHDdl01fpHJ9j2ehffdV9+dWX3VdfftV99cWAL7/8ssdXQPK9L2IS\r\n        0Pytq2OjPzcuXo1ZYNgsWOfhr2Jz7vw6vx/Iugx+/sni/uHjBwNPjXuNscenw/7TjvmpU6e7U6dOdadP\r\n        n1rO+zHs06e70z0X46l+nvagSTu0Y47gTqd/5MfY8K3xI+y7IOfg3ix92hdOR33V6F999XX31VdfdV+P\r\n        iHmPrwckv+ZLLvxLzdcjvur+6Op3q0efRWiRLLiyqxxTuahLbh/x888W9w/x4HcFfhH5IUH3GvOR+4XU\r\n        f+HChe7ihQvdhYsXNuZLXDw/AvaFi9151V242MefD92ov3j+Ys8PkDXSvnhh4+433qJyfLc6Oj1559d5\r\n        1ejffP1N983XXw/4ZsDXa/imx6BbR/oU33zzTfdHL0qjB44WRQdPjpt3m6kQfs1ZrbsrRP4Cv/h8cd/w\r\n        8wcFny26p2LP0eSy934cv+hduXq1u3r1Sne1H692V6+8uGb3/jVOtFdf7F68MsxfXHIDH7p1btSN+mXu\r\n        q1c338Vc5Fut3qjyLY2++bLRx+YMfBv49pvuG8U33658S21w33bfjr6VbuDWGp2Np3YWmZxrTNVrnOqc\r\n        hjkqULsjsDkPsD1+/sWi+0UCfOKVV653129c765ff6V75fqN7vor1/tf7X09fr132tdfGfzjfIn0hV40\r\n        Yd+Ats+3zD1oMp53v/Hmc8637ZB6avlWaSdXNfq33367xHeB777tvk30/HfrviX3Xffd6Fth4P7oxd+u\r\n        FuamHSofc0z5HDcFHtaO8IsvFwe4R/wy59HoX4nvi5UvfqXY8leCxZx2/hqxpV908qvHBn/82rHVrx5b\r\n        5VrPv/IN+v7O+a6IfBvkHVTHONqJYyNfNfp3333Xffvb3/Zjj98O428Fa77EdxEjceL/o2u/XRUVBbCo\r\n        qaI1jvEVr7Hkvgf88uvFfcMvHiD8Mho85uOYdowB/cTiHrdWn0x8+9bt1acaW4R20MdHl/MTkIccor0d\r\n        Ovnk43feGe473yvfZOst0sd45qo4omz0aPIRv1V8N4K800D3r0aja0NyTnAz5MjryANTPbl9xC+/WRxg\r\n        R/iF4ZJfffBJfMrRONdxbf6efEhKzBOrHPHBKR+MMTFf+oQb8N5w13zb+Qb4VhX0TeUg8s077axG18Zl\r\n        A8/lotHjP921ALcBtznOORK6efqYa5/xy+8WB7hHPP3tont6HH+ZCJ/MP/nk0x6ffvpJ98mI+HmLnPd2\r\n        aHo+5sEN+pz38aMm50O+1I3xIx/j2n1v+8bcW8959Z6PG87lLBv9u+96osKvf/3r5X+ax5x+IjR9o0dh\r\n        //ZvNvwbCE0W+e/M1OfG5+bvD8D4qAlsk3Om/q/9etH98rcDYk4/sSf92Bx/0fWKObH/0R//cf9Pul9+\r\n        /VX3x3/8xxt+IjT5T8Rz9fHPxvFPzXP0/XvIppzxNpbvP2Lmvv/Q8v3ne6waPf5Pt41kgl/96lfL/zPu\r\n        17/61YafCE3//7qzkAqxuTyYbTY69yAzP3nm3KJxt9X/879ZdL/XDYg5/cS2+r/2m0X3dDcg5vQTP2S9\r\n        Yk7sb379m67rftvF/37zm+m7+M1vft1r43+/nqWP/MP/5uRfe88z3sZSP7dfqrc31ejxz2MbyQR//Ktf\r\n        Lf/57Ve/mv6KFl8Ylo0+p3H30uih3SZ/fwDGx5w8vApb1hB/Mj392wFz/pTai37r/wL4geoVc2L7P6HH\r\n        b+6a8yfu8Cd6fJPY11vqv5p118v3Nrdxt33/+oVB9clVjf7111+X+CrRf+OMfCdcj9R90339VWC0v/l6\r\n        +M64E92ALGAuGEPbQTVunrU4MNeOwP8M3Sn076x/mTHu9RfC9X9fl7+359+xh79Pr/6+3f9dOv9+vfy7\r\n        df4dHn9//yR+FDv/ni5/X8/c8XfyfCvRkNXb4buiJn35hcD5Wrmrua5XNXr/verxFbH/e8vwlWttPn61\r\n        1G9/beLrr7q/lX+iu+Irm4VXfm6WuhbvwLw7Av8f4gPsAfHPauM8xsDTXy+6p4Xr/5/x+D0D73/QvffB\r\n        6v8Vj/8H/YP8f9Tz/3nH/7se/PD/sKtu5Xv/gxVn34va1bvTuYsn18rbypnzqtH7n0L7Kn9YRX+g5avh\r\n        h1R6X/ArXxNfjT/UkoufNIURodH5lD51Gvd9IGsw4L8H7xTxb8kPEsZ957+d61nkv4Hnb/Md/i389vLf\r\n        0+Pf0od/K7+19u/mqzH/3Tz8g93/OvD4N3P57cLL++bdz3mr2yDXYN6wW28+9VWjfyE/ahrzL+JHVO2P\r\n        n65+DDW+OCx/ZNVo+0Z3xWszsFCCGsZN2Qp+xdwVuI6A3+V1gL3j5+PYN3k0e/q+Wmx+19pr+R1xw3e8\r\n        rb77Tb7jbTmufzfd8rvnlrErzdr9Vu9M/Q6MV56jA+OoDbtq9PhEmM/1wyU+/6Ln/IdKjLr8sIq1D59Y\r\n        af5m/Dx6tQkWWfm4kYrjvOL2Gfr92A8UxsbcBZbf4y7f8x5r9N/vHv7PF8P3qfff1z5+v/r4Pej6Pe/L\r\n        73MfdZvfu/5Kd6Pnhu9hH75vPvwD98r16+135N6d8s8X77GVMxGx5LiOomr04WOixo+P6j8SSiAfJ7WB\r\n        /LipHsNHSCX6T5hhAVFwovI5mxuiXeWjXYGxO8LGT2LtEvyx1R8Q4ifNdgXdb2+PP822PIPPF/1PmeVP\r\n        kQ0/dRY/rbb6abTlT6DFT529KD95FvPxJ9GuxE+0pW+MXeUcf0LN3HGPOe+RmkpHTGmcv2p0/fTWzwTr\r\n        9uqDIYcPihTf+GGSPca4tUZ3xVSc4yu9i2vluB/QLxgAG2Cn4M+UP4B4ajyH/Jnw/Pnw+Fnx5c+kb/xc\r\n        +jq31FKTuc4PP5+enH1riuTVr2+lNWcu1VRvjlzMq0bvP5d9/Fz3+OeHBO382Of+nxzy2wHHj4Fefrvh\r\n        GNd/3LMryBUa4ynY1eigGub5HrHxIQq7hPk0lr+UwKfJ6Dzt4VNjhk98yU+DyU+BWX4qTH76zMjpp87o\r\n        J82sxS5j1vX9/bK59O0pb96F1Tkt13C+zKXvvmz0jV/OMP7ShvGXMQxY/+UNwW384of8RQ+fjL/AQQuL\r\n        QnSD3KwrWsGNtsB42vsEfpbaLhEflviDxce7w/Jz4/Qz5MZ18ixWn+e2+oy440fzc95ijM+UE41+3lt+\r\n        Nlzqev/x1efNxZix8hl0s98U332FlqblC+gXmlaj669aGn7d0ojlr2eKcfiVSx/Hr2Ja/p61UZ+/jilz\r\n        fPzRZqNr0bp5boI+6jWGfmq+Z/ATTR8YsPHvEfmpr/kpsFwvP945P8U1PwH2cP8prcOnufaQT3+NT3pd\r\n        fca7+ZRX/ZTYtV/eEHkPtd8Y36C+7zlvnXbFOV/Mq0b/cPzlivnLEnuMv1yx/22p4ctftpi8avUXMvZf\r\n        FMbfvZYLuyLJqX3a6JNnnIt3XMRWYNyOsPExyA8I+obcEZZ53x9t+Wz3p8Z5NqH7aObhs9fZxIe7Z+Ij\r\n        oeOXOsTnva81+vDFoY+ROP1o6PzlD7xvC31j7o223nTGVDmS0xytRv/gww9G5K89Hubx65LT1//q5B7D\r\n        r1ZOrf5KZZ33vzaZRWtx5MhXNnn6NZ/Lu2twLcGT8RtVHkDwt8rcC5b58FtbnhrH8A9/0q7/6euaPn/1\r\n        Uv8nOP4Uj4aPP+WXfvlM+KHx1V7Pbd+E2q45nZ5vmPPqPVNbNfr7H3ywxAcfvC8IWwHf+wJo//DSV+sF\r\n        nkExRLUhtzlurLId76C5dwj9fWEPFMyvVtoz4pcpIn9vj797LX4H28Z98m51JO8wFUO+BafNN+L4lq0c\r\n        R51Xjb76xIzV9wHzUzfWIZ+4EZ/Y0SO/33jAH2SjB6LJdVQE5/gKbvOaP/yt9fYR/M2gu8TGrxf+IeHu\r\n        7rDcc851HOcbjaPzfF/6FmgnxzxEaqp1qJ8Lfa98u1rTnDVCUzX6u++9t8R77wI9/273XoC+d9/r3n03\r\n        +JVvyPNu9weXvlxfnEXqBbgNqo4xlV3FVeu14neA/B3e9wMbv5P8LymefGf8wib7Dm7pj9+PzrPXt8T3\r\n        4nSVPQdcg+D6Tqs8505bcRlbNvq77w64C/T83RHKEaIZ8QcXv1wtzOKdfRY+1fAAmKflc9w+4afxIA+w\r\n        czwRzX9rbPZ3pNHNHZSoYsilTV79+XZTU71l+lxOxyVf+airGv3u3bvd3TvvdnfvxDji7t3uzt073d0e\r\n        yo/2Btb5389G14NgQTE6X3Dkac+BrtEC43aEJ24t7h/e/uHipztE7LWfj3uOccmP8+WZ8z5zzjue4qfy\r\n        VajyunxTTVv1DeHyVY1+586dJu7evtPdCRjfGu7e7u7cud3dvnOn+/0LX2wWwwLnHkzlZ6zLqWMF5twR\r\n        nnhrcYB7xZvjF5CYR6O/tY7g+/PmPetdVPacd9DyMQd9TqNajeG8hZYu+KrRb9++3cSdd+5scC28c/t2\r\n        9/sXv+gW50xhMSafNotVPrQaSz9jeAiapwLz7AiPv7n4C4G+mXaIn+4Qka+v8Y3VuLbeG4uNc18D79e9\r\n        q7lw7ytzUbuNv9JxHb5r1xeBqtH7H85f+1D7ddzGB9aHdoXbXf+D/Ynxh/h/LxudxeeGHO+QOqd3nPNP\r\n        6e4T+kd5gHvD64vu8dfHMZo9uXEMzdr9urueegcuvtI6X75nhdMR6ddG1fgWqNFcVaPfunVrK7wzYmm/\r\n        PUA1vxf/6a4FnMdG5kA3QZs8uR8AnnhtcYB7RDT5468tusfGcYnxC0CA577WAO4NOT1tFzM1Mgc5Ym59\r\n        1JNnXNXob7/19oC33+7evvXWCm+/te4rcGuEcr934fPNIqLZWeAcDTluNvyqof57wuOvLu4bHntA8PjN\r\n        RffEzdV8yec53DSNnuA7Ia/+uW/GvbXqD7HgqryMUdvlaoFrVI3+1ltvreNt2C2sad8e8Vb39PnPNwtw\r\n        xXFUv/NRxxz00941ck2Dx24sHkg8fh/Q5745zqPhBct74N0kqsZpxVDHt1iNjHNz5nR+B7yvjbVzrBr9\r\n        zTffXOINmW/ijdX8jXF8S/1vjXize/r8Z+3iuLmWhmj50u+4fcZjrywOsEtEk4v96CuL7vFXFpt3zXun\r\n        nRx91Cjoj/GCiWVexru1I8/UWoTTJl81+htvvNG9+fqb/ah4/c2cr3xvClQbjf9G5BjzPH3us80i3MFU\r\n        nOO3QWz4XnPcIx69vngg8diO0ed8edPOtXjua8g3UL0p9dNHnppsUPLqd76qSRmb2sqX6ysXqBr99dfG\r\n        T85c/h5oYt2nn6SpnNp9o7tNOvAy3AEp5zT0keOa+4BHX1oc4B7xGMZHo+GvDWNqlmfONzHF8X1wnKN1\r\n        0BiuT87Np9ao+PS1Gn3Aa4LkVr7XXk8MzbyyX+teH5H+X8Z/ul8cF86Rm3Fca/OVLsA16P8e8Mi1xQHu\r\n        FdHM0dij3c9fFPvFxerMt7lzarVRdKQu3xlBnQM1c2zlYm1yDlWjv/bqa91rr766hldbeG3Eq6+t24Jf\r\n        nvt0VRwPJwp1vINq2MyEyzd3nfuAR15c/MXA1d3i0R1C64ymznrVz3PvUb0RRTZO9T7m5KDGvde9gvFp\r\n        uzetdVSNfvPmq93Nmzctbrx6s7sZML4h7tXB/+qNAaPvF2z0VpHcDA+L8ZVOR8ZznV1B6wN+cmXxQOKR\r\n        XSMaOnLnF5Mri+5R8ZXvQ+/H3Zm7S6dhriqWOjdnHvorrpo7VI1+48aNHjdHpH3jpuDGzRHi7zU3e9y8\r\n        uYoP/OLsJ5ubu2RshRYbmuC4Kaet4OId8rJ2jJ+8sDjADvDI5UX3SM5HW30b91y9D9XsFZpb33Pm5huf\r\n        AnW0E9WeHKpG7397RfymihG9feOV7saIwX9jRNqDZtDd6G68khhy9I3OAllsbIp+atKe0invfN8DHr68\r\n        eCDxk/uBS2PuSyPEt/FG9P6zCembeisV7zSai+tQN0dfxREVXzX68Gtohl87E8hfcbOO+NU1K43+aptl\r\n        46d9/ZXuF2c+2dxYFnLZbNZtfIpzfur0UPYZP7m4OMAuEQ0+zh/Ohr84/qe7Of81OE3rzVBLn9O4txZv\r\n        fc57V43mSR851RFlo1+/3r18/eXu+suJsNdxPfyq6bnrvdZh2ehafG6cGyV4OGnzIHX+A8RPzi8eTFzY\r\n        LR4+v+gejlHsfoyGvzD+iT7nXd0LMv/Um2vVoe+6pXNvnHr2iKJq9JdffnmFlwTKv/ySYOBeGrEW89Kg\r\n        +fmZjzcLYfOywIpvxdFHm759xMPnFg8mojF3hXOL7sfjmPYaf14aPcC5vgPeEd8RY8i5fOQ4d/YcX8U7\r\n        n9YZY9XoL7300grXBLCvvbTCWswarvX4+emx0QlXGOdqOw21BHUvGM0+4MfxGA9wbzi7Gh8OnJExmv7s\r\n        YjhvvWNtAn0r276D6p25PFyLPjdWqHK5dR2qRr/24rUlXgTavhe7a9eu9XhxxLVrwb3YPXX6o7pIN29t\r\n        InwuxoH54sBa+vuIh04vHkj8eId4KBr9zAoPRYOHb7Sj4e39Oo7+6g8C12RzdU5DjjZR+fULgOuJtKtG\r\n        f/HFF1e4OqKwry5xdT1u5MIX82WjV0VrYTq64qmljpij2RVYo+DHpxYH2AWisTkXbuM+OJJzfKXhqKDO\r\n        5do2h4upNMyT86rRh98PLb9Devyd0fF7p5fc0o7fRT1g9bujB/7qleH3VAeeOvXRZjGBK8Zm8VN2cgHm\r\n        I9yB7RJcT/DQqcUB7hE/fn7AQwH1JXdy/BO9wsQdWeSb0ljet747rpG+GLd9n+Som4Oq0a9Eg155obvy\r\n        wpUeV8dx4IEN/oUVJO5n0ejcZBZLXsHGVy3j5tqO58VN6Z1NHvjRycUB7hHRyAGdp/3jE4vuR89PNHoL\r\n        7h6V4/26N7MXuHWdTS39la9q9BdeeGHA5QFXAskRo2awLwuUf2FodC2CRZGnr9owdVN5afNgXAx1ROUD\r\n        /6PjiwcSD+0S0cwnZI41Ym7vgXfEe+c9Oz/zMh9jq7y0uZ6zuQY1zKO+qtEvX768hhdy/sKl7vLlwOXu\r\n        0oh17aVRE/MXRkTc5e7JUx92i6vFBlqY0mfOGOlzeVTn5npQLqfjqxrB/+6xxQHuET86uuh+FOOInktE\r\n        ox8zjc67Tc41RQt691Mx9DPOvT0XR3sKLm/V6JcuXSpwccSl7uIIr4nx8ohL3aXLl1aNrgVwJIJ3hXOs\r\n        YnixrTiXV0cXrxzzUht/oscjBX5X8dw6fqSoYhpYi5c86evHtJG7t83aaznE1nzMrfU8NPqW9TG35Gcd\r\n        PZe5dT1ZK0Z7B7xDhbmrpp4xbk5NFU/O5WJexlNHX9XoFy9e6C5cvLiGixcuDOh967gY/l5jMPqefP4D\r\n        X2CFKT/xouGmUMVUazteOZevirlf4FpzUMVuy6lPz8L5WXeloZ682rrm1Prh15zu7hhDjj7NR38VQ5+L\r\n        qey5vqrRL1y4sAXOr9vnFasvEE+e/GCzAAUPmnag2kzwoc/Loz/zZc4cK23yqqfW5XM1/0UA97Yt3L4d\r\n        RziN4/J+yacv47Rh3FjlUDiNctWccG+G2Mu72cteqkY/f/58d/7c+WE8f24cz3fnxrGJiFsimv18d+H8\r\n        uaHRWZxulJtWjryOzKcgV8W5vFNr0891nI9r0k9Qez/AdaZqUH6OTn0aR43zMRfnLX3OCeaqfGpTrzw1\r\n        Lp8D/cyvOZjP8Rx1XjX6uXPnepw/d7bHYJ9f8iubUH/Ej/Pz57qfnnx/sxBnT/EtMKY6oMA1o9UY5iIq\r\n        f8XvBVrLvYK5uY7aeTbJ86x2ib2ssW0M91chdVOji8m501CrdTOeXOrJOV3lKxv97Lnu7Nmz3bkeZ/q5\r\n        R+gU6/6Ij/HMubPdE9noWXiCBSbnfBpLvfO14hir4IW06nW5Km3F7we4tqtjap8VTw11jpuT0/laXOvh\r\n        t3Av8a165oDaXdSQdp571ehnz5wdcabHmUDf8MOYGPxnB/8StAfuiRPS6CxGi0qQa2mT56apqXzMrX5y\r\n        Lp5g/JwYalvYSwxjqzz0UVdpGcP1qHG8xrl8Lgd9Lb+zCeZy+V+CtsrdyuHi6VeOuile16ga/czpMyNO\r\n        d6cFZ87AHnXKnR7t9dgz3RMn3tssyCEPMYukX+H8ymmuXaPK7S6RdeZFEMx1vxBrufodxzg3n8rB/Wmj\r\n        JB8cczKO8VPaynbxTk9truF01FT5mHuOVn1Zj47h11wx1zOuGl0b99SpdZxu4FToR9BnG10L0gNNnhdC\r\n        ew7n/FNwaztQ4+Lc/vTBzHk8u4CeqT4Ijq2zU38Vw1gd80FSq3nc+s7HWNpcn3kSWRNzEC7fFObW7UbH\r\n        6TilU1SN/vyp57vnn1ecGqFzBfQR3+dY+ZeNzkI5ElN+Bx4EfY4jr7bzVTHk7gW6TrVmC4xjjpeNnlzy\r\n        HB2nI/O4ul0eh2pdF+O4Sk9uKneF0OZ+W3FV3ta6rM+B+Yiq0U8+f7I7eTLx/BInYA9Q7YiIf35d8/jx\r\n        9zYL4GOowM2kXcUHn3A69ZEnV/mr3AqtO3S8oCkw17ZgPdtgTryeQerd2dKukLo5a6fOrUdondusMeeO\r\n        6VM762OM01bg/mK+7f1WjX7i5InuxInAyR4nZU6Er8cyJuzgwz/ExfzxY+/6wpVzqDTcTMunGo6t+Zxc\r\n        U5jK47QO1M4Bc2guHbmG44gq3sW1fC2+pXE2R2oc7zRzwBjmpIZ++pjf8Vq7y8W8iarRj5843h0/HjjR\r\n        48Tx4z3SVvS+EyN6zTCe6P1pn+gei0ZnQVXRiesTnNsc9YxJm1zGk3fxbh3H0e9A3b2C+d06tAn159zl\r\n        ynNx+cKnsfQ7cF1djyPnzqYv/e4NMZY8/VOgXvNUPoJrc1TkeTtf1ejHjh/rjgeOBY53x0Zk4w4YNMeO\r\n        H+8Rul6/jBsQucL/2LG7q4JYCFFpknd+5bKRnd7Fagx1jqvgDvl+Iy/XQXU8Ewf63RkQLR9zuNxZa6u+\r\n        KsdcuBjHzYXWq7zuiTEVmMPB5VOOOfS8Yqwa/eixo92xwNHAse5oIJp2DYMm+N53dMQybszR49jQ6LxM\r\n        XqAW7DTcDH0uh8vNfMxFTOnoc/o5qNZtgTFz46llDHn1kyNS0xo1D2sjnMbV0srp+LBfaeSsYrmmxlYa\r\n        tadiyVV56HNxMZaNfvS57ujRozvFRqO7Yh3HzU4htXGB5HaBVi7nyzqc7/vA3PPUuiu9Pib6iNY5kMvm\r\n        Y6wDYyvwPTB/ladae87ew1fFu3oqLcF6uR+iavTnnjvSPffccyOOdEfSPlLjyBKhHfRHMseR57pHj46N\r\n        zs3QdhuhLucxJqglTw251nq09+JzNd0r8oJZ+zZgTayXuefUX2mYm376OBKsO+2suYpzMQ70a3761O+g\r\n        e9HYVgz9jOFYzatGj2Y98uyRfnz2yLM9lpzgWcwHDPpn+9gRzx4ZGt0dDotTziF8N0ycbnoKlc7ldH6H\r\n        OfVTMwXG30+01qOP9eXc6dzIHIyrQD3jMid5p6l4l5s+xnF0Oqd34NqMI5iPucOuGv3ZZw+PeHaFw4Jn\r\n        n+0Oj1j5j3SHD694xbOHD3ePPHdnszgiGzgQ80qbvsrOmFaOKbRiWj6nYX3bQC9NL68CtS5G64l5qz7G\r\n        Jse4Vo45/tTwzrg+11Xe6StO+RwzR4y5R7cGeQdqtAZ3bhwrVHnUx/1WjX748KERhwccEiR3+HB3SObR\r\n        5GE7HD50qPsbR99cLy4vrLVBclXslI98BeZiXpePPuoqbleo9kDQzzj6VVdxzOdyk2Oulr7KW/mrXA4u\r\n        l8vL0cVXNvVzRjenhmsR1JWN/syhvjkPbYFnAocPdYcOPzPMBeH/p39+tPuTP3u++5M/Ozki5s93f/Kn\r\n        p7o/+dNxHvyfB57v/u6fCf78ZD+GJsc+JuP+9Pnu7wVSM8bb9cQecqv+1Nq6G+sAa7oxvh9jDyOXNcWe\r\n        Ni6EyItxoHZKfy+4adZw64UuffSTy3nGtMBcLZ5cta7yjKk4B+bgWrSpoXZK73RzkXFVox96ZrORW1g1\r\n        dTR5YrPZDx16Zh2xTgK+Zk6NI9bWxHo2fw2Xe3PPuZbb43rMxkV8X5jTbE7LONoOe9G4R62cfoFRm3mo\r\n        d3bM1XZrU8t8zOk4V2uVq8pBVPtm3vvR6EMTrTfSur5q9HWf5mw2esSVjY710ISuWddsrmP3TN+Wja4X\r\n        3gLjtgXz8JG0Ro11echxDeqYrxqZs+XjOuR0pI/+5FTvbDe29M52oL+KmZM3uLLRNx7vAXaBjUuYgj6c\r\n        6iJbYGzGx/iq0TtMrVvlqWJYB7kqb0vr+CrecdQSrbz0c+7Wpc7ZFaqaua7ODxp9f7FxadsgHw1BXYV4\r\n        GPo4Yj4Vz8dEH3PSR446IjUcXU5nOx/HCsznEP7WmbVyZJw2KvWMpYb+imONB42+v9i4kF1AH4O79ErH\r\n        GB0rXnNxzphq7uIrLXU5n8qlvtdm6JmTerc2QR/1c3xTfCuP8vQdNPr+Yu0iHHhx2yBzxMNOML9DS6f5\r\n        U6drKU+4uiofUfm4NnVVLck7P3OQdzFOR1R7p6+KZ5zbQytWtQeNvr/YuIgfAtxjquwWQks9bUJjqtHN\r\n        +QXGrVNx5Oc2NPNUtluDGse5uOSocyP9OY/9HTT6/mLtQu4F+gC2BePTVl458lO2y00tdYnXC22lp9/p\r\n        pny5JnnG5DrJuTjGaFzlJ9fSu1jlnT+4g0bfX6w9ljngpe0H8gHzIUc9jmO8g8bp3MVzfca6OlTXytkC\r\n        4yLG1aD+jKnuq4qn7bh72Q9jDhp9f7H2eO4Xco056zgNOdrJ6RrUqO1qcXFuzli3ZpXbocrrNI6jr6qn\r\n        xblY5p7SUlf5kj9o9P3FxmXohThQ62Loo99p3hCeWuVTxxzUu9gW5mgc3Pppq48j/fQxhj6nC+j5UBO+\r\n        Vh7NxTWn4lwOzpU7aPT9xcYl7ALxoCpQy7gY+cjUpyPzqd3yVbmJilcw1xwfa3F7aWGOlmdR1TKFKpY1\r\n        qCZjUkNt2AeNvr/Y+pHNQeZ0oJZx5Fw+p2es06uOOZWv/A4ur8YzF2tlPmpcDqLS5Rr0k3dca+SceZjT\r\n        +Q4afX+xPPh7hV46Qe0UXOybRqd6tSttxWuOHJ2W62zDKSJ3gGvOja+w17gKegbuTqbWa/kPGn1/sXEB\r\n        2yAfrGuKbaA5NJdybh0X43KSr+ypmHi4rIlaB2pp6xqVfwpaF/PtJSfrybFq3tYXSK4d84NG319sXAqR\r\n        l7QrMH/gLfh15NzlmqMj53zcN+t0tVW5pnjWTh3922ipifOlptoLdQRz0Ka2Wueg0fcX/SMgeLn7hXgM\r\n        +Sjpy7qq+vjokqNuClX+Cq01Mlf12FVT5aPNGD0v5qJNMA/9Ct7N1FuhVvPEeNDo+4uNC9pP5GPJh0A7\r\n        OeerdMS2MZWW/ip/NZJz+bhm2i6+0pFzc11f9RydtsVpHeSoP2j0/cXGpbkL2xbMN4W3Zd3W+vTRJlr+\r\n        yqf8HE2Lb+0pudg7+TwT8py7vIxRTZ6zy+X0XIfr0d4GB42+v1heroKXcr8QD48Pmrby7hEyTuMdV8Wn\r\n        3YpxPqdLnjoX38rjuCm4NcixNgXPxYHxPDvC1VI1ehnA5E6jtvM7jn6C+ehX31QN5FprVzkqbaXnertE\r\n        Vccc6F6q0XHM0UJrTfUzP2NdnJs7jjmqvNS4POSczXU1n8tf5XWYm0+5stGZOMZbZlHqWADBgiKn5nVr\r\n        UO98zN1C5mCsmyvn+MqvddJ3v5HrEdTx3APVHhxSW42aJ7i5+apczDvFE1nDVB2qJ+d8zMea004dz5kc\r\n        70VzVfXrOjrmvNnoWhgXZBFzbHLMy/UC1YFW/F7BHFVthOq0bqfJ/RDMuUtwrUDWp/eoe9D4qf04XvOo\r\n        lj6Nczz9DvTp/pxfedVynILTaY6Wv7LJ00+74pJXX9noKSSYvOJpU+NyJt6Bj/6wVUMt63J8lVt9TsN8\r\n        zE3b5WqBdbCmOWBcIs6M5+ZiHJd8NU+bdbg8LbRi3ZpcjzqCPme7Gpi/snVkrql1nJZ5q/WoJV82ejjj\r\n        UTApC8mHwwfUQuZg/gotDb8ouDp0vcqXc91rNgbXZj1O46D73QuYb69wDa/QtVSX86qWKmeln4LLx1y0\r\n        M869LXdP1FQ5NYeORMXfC6Ke3NM2+XUfZaMzqAI33hpdkdscKjdLnvHMtU08bfLqb3Gcf5/gPlt1zfE5\r\n        DfPrmi09a6OPI7VT+adsxweq90k9czGv6qlzGubZCzS22egs6LbhaLs5OdqaqwXNF7VwQ250sRypdXBx\r\n        1FRa5afgcjlQt23MFM+cgbz/Ka3yrgbaDvRXMY5zfl3bacgpdN8uz1xuCnP2Qo61VeuWje4SMpgLTfEa\r\n        H35q1KaPyFyp44bJbQv3Rc3t38HV1EKeBfdAMM7Fz10z4/SsqkdCuLMhwqe1uHkrntpt9qWxU2s4/9w1\r\n        XawD82gcfS1e70pr5NmqnTFlo7uFMomCybmQg9O6vNRPzRlPm2s6m+s6tNZXvoohuO4cMMecfJVfeeen\r\n        TjUa53JUHPNM6amjz+lbo9O7uaKKrWyOjmN+l4c5puBqaDZ64s4ItV2hyam20m0LLZrra35dWzfOmhzn\r\n        6nSc8pU/wTWmkDVPgXHbQOOZj7bTM9+UL86gVTt5p6l8rbdQ2W6tGLe9qwDfm+NcDczjeBfr+pAxzp5sdFe8S+jAmKl4p3e6qZg5XNjknNbNq7iEPhoX/30j6uC9tmpradz98Gxpz8VUDNfIMeFqo4axbuQaeb88\r\n        Q6dtQWsgXH300+fqyXmz0TWZwiUjz5Gci3Fztanh6LhqLeXJMSfBWNVW9v0E65sD5mCuFq/jnJyag3MXX61V5XBr0Ddlu1itjetS6+Jac65LH+PcGsxDv2omG/1ukciBWiL8VV4Fi3R62tRPYUrf8nNtZ5PbBfJcCOqmYpKnlvycPVT5pmqr4oipN7PtWVOruV0tmV99TqfaFpdznnPYqqOfa81B3kGgbHQu4kZXdKWhLn1u\r\n        EzwYp6EubbdGpSdXzQnW7mzmYW1zwdz3AldjKy/1Tlv5OWcu+phX4Rqdc83Nc2Z+2s5HTcVPcVlbK54xU1yFXEe/aORYNroWWBWqHP2MpcahKnwu3p2Iq+pWH/2sxeloM6/aDppjLpijAuMYW3E6Uuv4KVvj6CdUx/Vot+C08UYqH2NbMRw5dzaReRnDOLduy++0k43uimESNhiL0Rw5r/JWOVoxXD85p0+d880F16q4BB8KQf0cMMc2+fJs5t4Fz1LX4HqtXKFlrlYMedrK6xk7P7lE6hm37RkRTu+4ip+zru455roXxs1qdN1wq4gpnfpdXvLO79adAmuqbMe31qo05J1mLpjLQTVzdcrpWGGve2EcY7l+pVGeWoJxGlOtS95xzK1zxtGmT2Oq\r\n        L4DUqoac86uu2egU5/w9k5watUNPnhpyyeuGOW/Fc12noX5K59bUmtRHjr69gLkI6ufEMJ65yDs4/zZvhHzEMr6qZcomeDaqr9ZVPZH6lob5yJFnrphXPeT2QTvmZaOHgJuuEikyRmOZh/Yc3vm4RnUYDi5fHmhL0+Kdnzm/T+QZZT1z9ppw5zoV7ziH1N3rWXF/BPfg6tdaXEwrV7UuEbEuf5WTfPrI8fx0XjY6D81txNlcqNJQR42bOw3rc3rybs74al7FVHDrbQPdx67B/FqvW1851knM0bu8BPlWTsaQd3nmxDs/wfXoV551TMVW67o4cmlPNvr7JsE2yPiEKyg0aaverek2PRXTAvWsg+tRT4Rf9/N9I+tXqL91dqkPjYJruPhKxzXIc3TY\r\n        1jf1vtx9ubNyoIZ29Y4cUssa3Vk6Xtegb1aj6wUzwRS0oGqsMEfX8rX8mls1XNPFk3O245hnW2TeKTDOIXX6MAJ8jJqvmiuXfOUn10KVr5WHPtoV53y5PjnGtMD4bfNw/znqPbk59c1G1yK56am5s5UnVE+u5WfeD8wa6me8mzO2WsvNiak8qnGgdtdgbVP1tnQcGUNbec4J6piT917l0hhqmL8VwzXpc6jWcSP1VR6iii8bvQrUeWy0WnAb5IFVCL9qdE0XW9WkWhc3Zz+5b/LqJzfHtx/gOTqO55w1M67SM5Zc6qu4bcDak4uROWk7uNoceF6BqXOq4qu6kle/2y/haov5ZKOn2C3gOMZRp/kYv63tfNW69DFO/S0Nea5Vca383ze0Lq3N7cX5HKbiKr5l01dp6HMal8vxyjFH5aeOMdWa1Dmec3KVrtnoLKoqtuVXmwUwtgXmYb5KOwcfjnCxtJNz9VDDOTkH5qnAuG1yOE3G5jm4NVy86p2fPH1pV75qXaKloz2FKpfWk3P6GZPg+2Iel5P5mXvKp/FbNTqL5SU7m9wcaFzOA64mB7fmnFjG0Sa0Rh4yterbFrreXs/0XpA15Lqt/emZkCMqfltUeaJOV8824PkzlzuLai3G8y7V1todGJdzV0+gbHQm0SLIKbhAi6tiW/MpvVuHnK7f4itOfQTXYwx9zEXf/dApr7VxrjZzM34qzsUSGu/yak6OTutyTuV3uRhTzasYF6+jm7sY1bTyOLtsdC7mFq6KmsM7mzFTWhfzkfGRq3K5GII61kBfladC5t8GzDGVL3nW1sqVeheX/Jw8U9B1NA/XSI7xmYP+Vn1uT3PAOpmX4DvcFq5Gt1eHPTW6Qyw0tZhqdaww5Sf0wqZi3aFPXYA+Qrff5FzuHxpYm9adc2paWmro4ziFqTOkf05++ph7as/kHFw9Gpv23HwKjdU6uXfmDrtsdBY6ZXPOBalRXxVb2ZqjQkvHmlwd9GlOp9WYKZuY8m8L1uSguiqOHG3yLo9ybk36OXcxXN/5Kp3z0SbPvOTpp87FkufcrRP42ORgvLPLRmdAhViYiybUR5txqiPX4l2eSrsLZO5qjeSzLur0fOaecSt2Kp5aF8PHw9qpV43OGUf/FCqd1l1pCO6Tb1HHCi7HVMwukPvUtao5z0P3pvWXjd66SEWLY7FOQ7h1Oa/0RPgUVYz6OTo4fcVz7XuB7qUFxhHUTcVT6zjHt2zyVR6na/k1B7WBbCLlqCfHtdzo9BVUz1jmZSxR5Uk70Wx0wiWoCqKu0rT0GvcJ1uOctuZvcZXP1eXsrIt8xWXevYA5NXcFal09ynM/6q9iKo75WrlYc5WPMY7TPITjq3y0ObpczEOePuZ1vsp2OdNmnWWj88LvBZlLm9U9gORVH+AGNbaVR7kq3qHlz3q0Thcz5a+gF0VQuy1YU0DzTtV7L2faQubT+lytybv1CX0DGUeNQ65brc98c2pR/RTn4Pbs7s3tOe21Rv8rYsTvbHILTh0CF3KxzlaOc+Z3cDo3uty0GedyOJtx5L5vzNkL6yavHPXUMY9CY1pgDW7O9RxHf5WrpXMcfQ7UuJyqS79q6CPXiotezr7ue/xfEOJPi8QOrmDGVbyzW/O0P0W809Omby861VBX7ZV+jScYw/hdgPmY262p9VOjWjd3tvKtOHLO75C6fCeMczmV4/uihvmoDUQOp9M9uzqIag8uH+OCj17Ovu57/N8U4t8tAvUAcq48D6jaLMG4CtSxJmpatSVcfap1fqebwjbaRF4iQd22MTwj5sh74/kpGE+uystRtVmn1lvlmvJzDfrItVBpyYedtdPnYqp5a/+6Lx15zxkXvZx93ff4PxLin+26xZsTxXIhFkS9jvRVcH7GMq+L41rMMQUXSz/X/77A2glq1NYcnFNPXcVPreX0jKv0zsd1OWdu2sw3ZZNnHs4dx5z0tWzyyr0x9nL29T+OcdH9zuKfE/JfNwkUn6GwsHUkqoLI72WzBOOrkWDtoas48lOo1ryfyLNUtDRpU7NX5BuJOfNW9ewVeh/MnT53Fqpz9fCdK8ixByp/BcYzB/Mpwsda/zXp56G3f2fRxfgPxBH4j01ChSusmjtMaedyubHwJVRfca2cU9AcHKkjV/lZ1xyeNjn6GZcxhMbpA6Mv9fnQ6KOOPsZU8XP4ueDaLl9yreZyepfL8arf9lwIxsT4d9DLf7+L/8W0G5r9b0Pwb3Td4i0UpXA8OVcY+UqvNnMzrtKSc/NtchDUU/c5clW65KZ0LoZclY8xXMvldzbHuX6nzTnrYqxqnN9pK55r0688/czJ/OSIqRqoncMlolejZ7WH//bQ5OuNHh9f89chjP/O//e6bvHM+EvcMmk+4Mp2BQWnOsY45KFMacOfGmppK7RO6jRn6qhxWuZ1cFqODvS1Hk1VawtVjONbZ8P9OY3Gch8OGcscyUU8fbQVc86N91rNCZ5Ncq5GxlBPnSJ68s/HHtW/kweil98zjd4b8QPs/JP9AAc4wF8sRA9Lk280ek/EV5L4O7v+H3QHOMABfviInv2Hw38NbPQ1iaUj/m3un3Td4t/qusW/mN9dc4ADHOAHg+jJ6M3o0ejVjzcbPPH/A9ZVqLd4ULBFAAAAAElFTkSuQmCC";
            //base64String = base64String.Replace("\\r\\n        ", "");
            // 保存的目标路径
            string outputPath = @"D:\output_image.png";

            try
            {
                // 1. 清洗 Base64 字符串（去除可能存在的 Data URI 前缀）
                if (base64String.Contains(","))
                {
                    base64String = base64String.Split(',')[1];
                }

                // 2. 将 Base64 字符串转换为字节数组
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // 3. 将字节数组保存为 PNG 文件
                File.WriteAllBytes(outputPath, imageBytes);

                Console.WriteLine($"图片已成功保存至: {Path.GetFullPath(outputPath)}");
            }
            catch (FormatException)
            {
                Console.WriteLine("错误：输入的字符串不是有效的 Base64 格式。");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"文件保存失败: {ex.Message}");
            }
        }

        private void ScanScreenDisplay()
        {
            Thread.Sleep(5000);
            while (true)
            {
                Thread.Sleep(500);

                List<IntPtr> form_list = ExWinApi.GetHandles();
                foreach (IntPtr handle in form_list)
                {
                    if (handle.GetClassName() == "#32770" && handle.GetText() == "Function Key")
                    {
                        IntPtr parent = WinApi.GetParent(handle);
                        this.Invoke(new Action(() =>
                        {
                            //if (TC_Main.SelectedTab == tab_ScreenDisplay)
                            //{
                            //if (parent != this.Handle)
                            if (parent != tab_ScreenDisplay.Handle)
                            {
                                //WinApi.SetParent(handle, this.Handle);
                                WinApi.SetParent(handle, tab_ScreenDisplay.Handle);
                                RECT rect;
                                WinApi.GetWindowRect(handle, out rect);
                                int w = rect.Right - rect.Left;
                                int h = rect.Bottom - rect.Top;
                                WinApi.SetWindowPos(handle, IntPtr.Zero, 0, 0, w, h, SetWindowPosFlags.SWP_SHOWWINDOW);
                            }


                            //}
                        }));
                        break;
                    }

                }

                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    //Console.WriteLine(p.MainWindowTitle);
                    //Console.WriteLine(p.MainWindowTitle);
                    if (p.MainWindowTitle.Contains("CNC Screen Display Function"))
                    {
                        //Console.WriteLine("Screen Display Exists");
                        //WinApi.ShowWindow(p.MainWindowHandle, 10);
                        //WinApi.SetForegroundWindow(p.MainWindowHandle);
                        this.Invoke(new Action(() =>
                        {
                            IntPtr parent = WinApi.GetParent(p.MainWindowHandle);
                            //if (parent != this.Handle)
                            if (parent != tab_ScreenDisplay.Handle)
                            {
                                //WinApi.SetParent(p.MainWindowHandle, this.Handle);
                                WinApi.SetParent(p.MainWindowHandle, tab_ScreenDisplay.Handle);
                                RECT rect;
                                WinApi.GetWindowRect(p.MainWindowHandle, out rect);
                                int w = rect.Right - rect.Left;
                                int h = rect.Bottom - rect.Top;
                                //int l = (tab_ScreenDisplay.Width - w) / 2;
                                //int t = (tab_ScreenDisplay.Height - h) / 2;
                                WinApi.SetWindowPos(p.MainWindowHandle, IntPtr.Zero, 165, 0, w, h, SetWindowPosFlags.SWP_SHOWWINDOW);
                            }


                        }));
                        //return;
                        //break;

                    }

                }

                this.Invoke(new Action(() =>
                {
                    if (WinApi.GetForegroundWindow() == this.Handle &&
                        TC_Main.SelectedTab == tab_ScreenDisplay &&
                        Control.MouseButtons == MouseButtons.None)
                    {

                        IntPtr child = WinApi.FindWindowEx(tab_ScreenDisplay.Handle, IntPtr.Zero, null, null);
                        while (child != IntPtr.Zero)
                        {

                            if (child.GetText().Contains("CNC Screen Display Function"))
                            {
                                this.Invoke(new Action(() =>
                                {
                                    WinApi.SetForegroundWindow(child); //置頂
                                }));
                                break;
                            }

                            child = child.Next();//這一層的下一個
                        }

                    }
                }));
            }
        }

        private void TC_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_Regist.Lamp = TC_Main.SelectedTab == tab_Regist;
            btn_Monitor.Lamp = TC_Main.SelectedTab == tab_Monitor;
            btn_Program.Lamp = (TC_Main.SelectedTab == tab_ProgList) || (TC_Main.SelectedTab == tab_ProcList);
            btn_Maintenance.Lamp = TC_Main.SelectedTab == tab_Maintenance;
            btn_Message.Lamp = TC_Main.SelectedTab == tab_Message;

            if (!ThrMain.IsAlive)
            {
                ThrMain = new Thread(Execute);
                ThrMain.Start();
            }


            btn_PosSetSave.Visible = TC_Main.SelectedTab == tab_PosSet;
            btn_SaveDressGw.Visible = TC_Main.SelectedTab == tab_DressGwSetting;
            btn_SaveGrindCoor.Visible = TC_Main.SelectedTab == tab_DressPartsSetting;

            btn_MeasureList.Visible = TC_Main.SelectedTab == tab_Monitor && Measopen;

            if (TC_Main.SelectedTab != tab_GwDb) LastGwShapeNo = 0;

            if (fo_maintan != null) fo_maintan.Hide();
            if (fo_monitor != null) fo_monitor.Hide();

            btn_Gw_GwData.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;
            btn_Gw_ShapeSelect.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;
            btn_Gw_ShapeData.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;
            btn_Gw_DressCondition.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;


            //TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");


            if (TC_Main.SelectedTab != tab_ImportProg)
            {
                if (fo_ImportProg != null)
                {
                    fo_ImportProg.Close();
                }
            }

            if (TC_Main.SelectedTab != tab_Warmup)
            {
                if (fo_Warmup != null)
                {
                    fo_Warmup.Close();
                }
            }

            //if (TC_Main.SelectedTab != tab_MacroLimit && macrosave)
            //{
            //    macrosave = false;
            //    Units.MacroLimitListmm.Items.Clear();
            //    Units.MacroLimitListName.ItemsName.Clear();
            //    Units.Fo_Main.bInchTrans = false;
            //    for (int i = 0; i < dgv_MacroLimit.Rows.Count; i++)
            //    {
            //        int.TryParse(dgv_MacroLimit.Rows[i].Cells[Col_MacroNo.Index].Value.ToString(), out int no);
            //        string name = dgv_MacroLimit.Rows[i].Cells[Col_MacroName.Index].Value.ToString();
            //        double.TryParse(dgv_MacroLimit.Rows[i].Cells[Col_MacroMin.Index].Value.ToString(), out double min);
            //        double.TryParse(dgv_MacroLimit.Rows[i].Cells[Col_MacroMax.Index].Value.ToString(), out double max);
            //        string unit = dgv_MacroLimit.Rows[i].Cells[Col_MacroUnit.Index].Value.ToString();
            //        if (no == 0) continue;
            //        Limit data = new Limit(no, min, max, unit);
            //        LimitName dataName = new LimitName(no, name);
            //        Units.MacroLimitListmm.Items.Add(no, data);
            //        Units.MacroLimitListName.ItemsName.Add(no, dataName);
            //    }
            //    Units.MacroLimitListmm.Save();
            //    Units.MacroLimitListName.SavelangName();
            //    Units.MacroInfo = Units.MacroLimitListmm.DeepCopy();
            //    CheckInchMacro();
            //}

            //bool bShow = TC_Main.SelectedTab == tab_Monitor;
            btn_Monitor_ToChgPos2.Visible = TC_Main.SelectedTab == tab_Monitor;
            btn_Redo.Visible = TC_Main.SelectedTab == tab_Monitor;
            btn_Offset.Visible = TC_Main.SelectedTab == tab_Monitor;
            btn_MeasureList.Visible = (TC_Main.SelectedTab == tab_Monitor && Measopen);

            btn_ToolSelect.Visible = (TC_Main.SelectedTab == tab_Monitor ||
                                     TC_Main.SelectedTab == tab_Regist ||
                                     TC_Main.SelectedTab == tab_GWRPS ||
                                     TC_Main.SelectedTab == tab_GWRPS2);

            btn_DressGw1.Visible = (TC_Main.SelectedTab == tab_Monitor) || (TC_Main.SelectedTab == tab_DressGwSetting);


            //if (RB_OIG.Checked) btn_IDPOS.Visible = bShow;
        }
        int GW1ERROR_Count = 0;
        int GW2ERROR_Count = 0;
        int GW3ERROR_Count = 0;
        int GW4ERROR_Count = 0;
        int ROLLERERROR_Count = 0;
        int SPERROR_Count = 0;
        int COMM_ERR_START = Environment.TickCount;//RS485
        int COMM_ERR_START2 = Environment.TickCount;//RS422
                                                    //int COMM_ERR_START = Environment.TickCount;
        private void masterSerialBus1_OnError(object sender, string msg)
        {
            MasterSerialBus masterSerialBus = sender as MasterSerialBus;

            String cmd = masterSerialBus.CmdList[0];
            if (cmd == "") return;
            if (cmd.Length < 2) return;
            int slave = int.Parse(cmd.Substring(0, 2), NumberStyles.HexNumber);
            if (Gw1.Slave == slave)
            {
                GW1ERROR_Count++;
                if (GW1ERROR_Count >= 3)
                {
                    la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 47, "砂輪1變頻器通訊異常");
                    tb_serialMsg.AppendText(LanguageManager.LoadMessage(Units.langfile, "Message", 47, "砂輪1變頻器通訊異常") + "\r\n");
                    pa.Visible = true;
                }
            }
            else if (Gw2.Slave == slave)
            {
                GW2ERROR_Count++;
                if (GW2ERROR_Count >= 3)
                {
                    la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 48, "砂輪2變頻器通訊異常");
                    tb_serialMsg.AppendText(LanguageManager.LoadMessage(Units.langfile, "Message", 48, "砂輪2變頻器通訊異常") + "\r\n");
                    pa.Visible = true;
                }
            }
            else if (Gw3.Slave == slave)
            {
                GW3ERROR_Count++;
                if (GW3ERROR_Count >= 3)
                {
                    la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 49, "砂輪3變頻器通訊異常");
                    tb_serialMsg.AppendText(LanguageManager.LoadMessage(Units.langfile, "Message", 49, "砂輪3變頻器通訊異常") + "\r\n");
                    pa.Visible = true;
                }
            }
            else if (Gw4.Slave == slave)
            {
                GW4ERROR_Count++;
                if (GW4ERROR_Count >= 3)
                {
                    la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 79, "砂輪4變頻器通訊異常");
                    tb_serialMsg.AppendText(LanguageManager.LoadMessage(Units.langfile, "Message", 79, "砂輪4變頻器通訊異常") + "\r\n");
                    pa.Visible = true;
                }
            }
            else if (Roller.Slave == slave)
            {
                ROLLERERROR_Count++;
                if (ROLLERERROR_Count >= 3)
                {
                    la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 133, "滾輪變頻器通訊異常");
                    tb_serialMsg.AppendText(LanguageManager.LoadMessage(Units.langfile, "Message", 133, "滾輪變頻器通訊異常") + "\r\n");
                    pa.Visible = true;
                }
            }
            else if (Spindle.Slave == slave)
            {
                SPERROR_Count++;
                if (SPERROR_Count >= 3)
                {
                    la.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 117, "工件主軸驅動器通訊異常");
                    tb_serialMsg.AppendText(LanguageManager.LoadMessage(Units.langfile, "Message", 117, "工件主軸驅動器通訊異常") + "\r\n");
                    pa.Visible = true;

                }
            }
        }

        private void pic_UserManual_Click(object sender, EventArgs e)
        {
            String FileName = Application.StartupPath + "\\Manual.pdf";
            if (File.Exists(FileName)) Process.Start(FileName);
        }

        private void LB_CurrentAlarm_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = LB_CurrentAlarm.SelectedIndex;
            if (index < 0)
                return;


            Alarm a = CurrentAlarm.Items[index];
            la_TroubleShooting.Text = a.TroubleShooting.Replace("\\n", "\n");
            if (la_TroubleShooting.Text == "" && PmcAlarmTable.FindCode(a.Code) != null) la_TroubleShooting.Text = PmcAlarmTable.FindCode(a.Code).TroubleShooting; //異常訊息修改
        }

        private void dgv_AlarmHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /*
            if (e.RowIndex < 0) return;
            String code = dgv_AlarmHistory.Rows[e.RowIndex].Cells[Col_Alm_Code.Index].Value.ToString();
            Alarm a = TroubleShootingFile1.FindCode(code);
            if (a == null) return;
            la_TroubleShooting.Text = a.TroubleShooting.Replace("\\n", "\n");*/
        }

        private void pa_SoftPanel_VisibleChanged(object sender, EventArgs e)
        {
            btn_SoftPanel.Lamp = pa_SoftPanel.Visible;
        }

        private void btn_Gw1CmdRpm_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_Gw1CmdRpm.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Gw1CmdRpm.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Gw1.MinRpm + " ~ " + Gw1.MaxRpm; //顯示上下限
            }

            //顯示並等待結果
            if (form.ShowDialog() == DialogResult.OK)
            {
                //結果如果有按儲存

                double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);
                if (dVal < Gw1.MinRpm) dVal = Gw1.MinRpm;
                if (dVal > Gw1.MaxRpm) dVal = Gw1.MaxRpm;

                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                this.Gw1.CmdSpeed = dVal / this.Gw1.Rate;
                btn_Gw1CmdRpm.DisplayText = dVal.ToString("0");


                //傳送指令到變頻器
                if (Gw1Dev == 0) //士林變頻器
                {
                    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                }
                else if (Gw1Dev == 1)//台達變頻器
                {
                    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                }
                else if (Gw1Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                }

                //紀錄
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteFloat("Gw1", "Cmd", this.Gw1.CmdSpeed);
            }
        }

        private void btn_Gw2CmdRpm_Click(object sender, EventArgs e)
        {
            //顯示設定的轉速
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_Gw2CmdRpm.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Gw2CmdRpm.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Gw2.MinRpm + " ~ " + Gw2.MaxRpm; //顯示上下限


            }
            //顯示並等待結果
            if (form.ShowDialog() == DialogResult.OK)
            {
                //結果如果有按儲存
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);
                if (dVal < Gw2.MinRpm) dVal = Gw2.MinRpm;
                if (dVal > Gw2.MaxRpm) dVal = Gw2.MaxRpm;

                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                this.Gw2.CmdSpeed = dVal / this.Gw2.Rate;
                btn_Gw2CmdRpm.DisplayText = dVal.ToString("0");

                //傳送指令到變頻器
                if (Gw2Dev == 0)//士林變頻器
                {
                    masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                }
                else if (Gw2Dev == 1)//台達變頻器
                {
                    masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                }
                else if (Gw2Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                }
                //紀錄
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteFloat("Gw2", "Cmd", this.Gw2.CmdSpeed);
            }
        }

        private void btn_Gw3CmdRpm_Click(object sender, EventArgs e)
        {
            //顯示設定的轉速
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_Gw3CmdRpm.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Gw3CmdRpm.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Gw3.MinRpm + " ~ " + Gw3.MaxRpm; //顯示上下限


            }
            //顯示並等待結果
            if (form.ShowDialog() == DialogResult.OK)
            {
                //結果如果有按儲存
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);
                if (dVal < Gw3.MinRpm) dVal = Gw3.MinRpm;
                if (dVal > Gw3.MaxRpm) dVal = Gw3.MaxRpm;

                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                this.Gw3.CmdSpeed = dVal / this.Gw3.Rate;
                btn_Gw3CmdRpm.DisplayText = dVal.ToString("0");

                //傳送指令到變頻器
                if (Gw3Dev == 0)//士林變頻器
                {
                    masterSerialBus1.Add(this.Gw3.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw3.CmdSpeed / Gw3.Unit)).ToString("X4"));
                }
                else if (Gw3Dev == 1)//台達變頻器
                {
                    masterSerialBus1.Add(this.Gw3.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw3.CmdSpeed / Gw3.Unit)).ToString("X4"));
                }
                else if (Gw3Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.Add(this.Gw3.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw3.CmdSpeed / Gw3.Unit)).ToString("X4"));
                }
                //紀錄
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteFloat("Gw3", "Cmd", this.Gw3.CmdSpeed);
            }
        }

        private void btn_Gw4CmdRpm_Click(object sender, EventArgs e)
        {
            //顯示設定的轉速
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_Gw4CmdRpm.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_Gw4CmdRpm.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Gw4.MinRpm + " ~ " + Gw4.MaxRpm; //顯示上下限


            }
            //顯示並等待結果
            if (form.ShowDialog() == DialogResult.OK)
            {
                //結果如果有按儲存
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);
                if (dVal < Gw4.MinRpm) dVal = Gw4.MinRpm;
                if (dVal > Gw4.MaxRpm) dVal = Gw4.MaxRpm;

                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                this.Gw4.CmdSpeed = dVal / this.Gw4.Rate;
                btn_Gw4CmdRpm.DisplayText = dVal.ToString("0");

                //傳送指令到變頻器
                if (Gw4Dev == 0)//士林變頻器
                {
                    masterSerialBus1.Add(this.Gw4.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw4.CmdSpeed / Gw4.Unit)).ToString("X4"));
                }
                else if (Gw4Dev == 1)//台達變頻器
                {
                    masterSerialBus1.Add(this.Gw4.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw4.CmdSpeed / Gw4.Unit)).ToString("X4"));
                }
                else if (Gw4Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.Add(this.Gw4.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw4.CmdSpeed / Gw4.Unit)).ToString("X4"));
                }
                //紀錄
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteFloat("Gw4", "Cmd", this.Gw4.CmdSpeed);
            }
        }

        private void ChangeGwShape(int no, int gwDataOffset)
        {

            PictureBox[] pics = { pic_Gw_S1, pic_Gw_S2, pic_Gw_S3, pic_Gw_S4, pic_Gw_S5, pic_Gw_S6, pic_Gw_S8 };
            Panel[] panels = { pa_Gw_S1, pa_Gw_S2, pa_Gw_S3, pa_Gw_S4, pa_Gw_S5, pa_Gw_S6, pa_Gw_S8 };
            bool bFind = false;
            for (int i = 0; i < pics.Length; i++)
            {
                if (pics[i].Image == null)
                {
                    panels[i].BackColor = Color.Transparent;
                    continue;
                }
                int.TryParse(pics[i].Tag.ToString(), out int shape_no);
                if (no == shape_no)
                {
                    SelectShapeNo = no;
                    panels[i].BackColor = Color.Lime;
                    bFind = true;
                }
                else
                {
                    panels[i].BackColor = Color.Transparent;
                }
            }
            if (!bFind)
            {
                panels[0].BackColor = Color.Lime;
                SelectShapeNo = 1;//沒找到就固定給 形狀1(只修外徑)
            }

            //依照內圓/外圓去取得 XML 設定值
            XmlElement xmlGwType = (CurrentGwMacro[10004 + gwDataOffset] == 1 ? machineSetting.xmlOIG_Param : machineSetting.xmlOCD_Param);
            double dGwType = CurrentGwMacro[10004 + gwDataOffset];
           
            
            if (dGwType == 0 && (GWType[CurrentEditGwNo - 1] == MachineType.OCD2 || GWType[CurrentEditGwNo - 1] == MachineType.OCD3))
            {
                xmlGwType = machineSetting.xmlOCD_PA_Param;
                if (GWType[CurrentEditGwNo - 1] == MachineType.OCD3)
                {
                    xmlGwType = machineSetting.xmlOCD_NA_Param;
                }
            }
            //取得這個形狀(修整模式)的畫面設定值
            XmlElement xmlShape = xmlGwType.GetShape(SelectShapeNo);
            int.TryParse(xmlShape.GetAttribute("DressLeft"), out int dress_left);
            int.TryParse(xmlShape.GetAttribute("DressRight"), out int dress_right);
            pa_LeftCondition.Enabled = dress_left == 1;
            pa_RightCondition.Enabled = dress_right == 1;

            btn_ToAndBack.Enabled = la_ToAndBack.Enabled = SelectShapeNo == 1;//形狀1(只修外徑)才有往復修整
            if (SelectShapeNo != 1)
            {
                CurrentGwMacro[10020 + gwDataOffset] = 0;//設定為不使用
                WriteGwMacro(CurrentEditGwNo, 10020 + gwDataOffset, 0);

                btn_ToAndBack.DisplayText = "OFF";

                //btn_RegisterGw_Save.Visible = true;
                //GwSetEdit = true;
            }
        }



        private void pic_Gw1ShapeSelect_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic == null) return;
            if (int.Parse(pic.Tag.ToString()) == 0) return;
            int.TryParse(pic.Tag.ToString(), out int no);
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            ChangeGwShape(no, GwMarcoOffset); //顯示砂輪形狀
            CurrentGwMacro[10005 + GwMarcoOffset] = SelectShapeNo; //寫回暫存
            WriteGwMacro(CurrentEditGwNo, 10005 , SelectShapeNo);

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }


        //編輯砂輪
        private void pic_EditGw_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic == null) return;
            int.TryParse(pic.Tag.ToString(), out int no); //GW1~4


            CurrentEditGwNo = no;
            DGV_GwParam.Rows.Clear();

            string title = LanguageManager.LoadMessage(Units.langfile, "Message", 136, "砂輪") + " " + no;
            la_GW_DressConditionTitle.Text = title;
            la_GW_ShapeDataTitle.Text = title;
            la_GW_ShapeSelectTitle.Text = title;
            la_GW_GwDataTitle.Text = title;
            la_OD_Path.Text = title;

            //讀取砂輪資料 (CurrentGwMacro)
            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                ReadGwMacro(no);
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

            int GwMarcoOffset = (no - 1) * 200;
            double dGwType = CurrentGwMacro[10004 + GwMarcoOffset];


            //讀取砂輪畫面 從MachineSetting.xml
            //XmlElement xmlGw = dGwType == 0 ? machineSetting.GetGw(no) : machineSetting.GetGw(no);
            string dressName = dGwType == 0 ? "OCD" : "OIG";
            
            if (dGwType == 0 && (GWType[CurrentEditGwNo - 1] == MachineType.OCD2 || GWType[CurrentEditGwNo - 1] == MachineType.OCD3))
            {
                dGwType = 2;
                dressName = "OCD2";
                if (GWType[CurrentEditGwNo - 1] == MachineType.OCD3)
                {
                    dGwType = 3;
                    dressName = "OCD3";
                }
            }
            string dressToLeftAnRight = Application.StartupPath + "\\image\\" + dressName + "\\DressGW\\";
            string dressLeftName = dressToLeftAnRight + "DressToLeft.png";
            string dressRightName = dressToLeftAnRight + "DressToRight.png";
            pic_DressToLeft.Image = File.Exists(dressLeftName) ? Image.FromFile(dressLeftName) : null;
            pic_DressToRight.Image = File.Exists(dressRightName) ? Image.FromFile(dressRightName) : null;

            XmlElement xmlGw = machineSetting.GetGw(no, (int)dGwType);

            String dgw_file = Application.StartupPath + "\\GW" + no + ".xml";
            if (File.Exists(dgw_file)) dgwFile.LoadFromFile(dgw_file); //讀取成形修整的檔案

            string path;

            TIniFile ini = new TIniFile(Units.langfile);

            //圖片來源
            
            string imgPathName = "OCD"; //#10004 = 0:外圓, 1:內圓
            if (dGwType == 1) imgPathName = "OIG";
            else if(dGwType == 2) imgPathName = "OCD2";
            else if (dGwType == 3) imgPathName = "OCD3";

            path = Application.StartupPath + "\\image\\" + imgPathName + "\\Shape\\150x150\\";

            //砂輪形狀選擇 - 載入圖片 (Fo_UI_Setting 中設定的)
            PictureBox[] pics = { pic_Gw_S1, pic_Gw_S2, pic_Gw_S3, pic_Gw_S4, pic_Gw_S5, pic_Gw_S6, pic_Gw_S8 };
            Label[] lbs = { la_GW_Shape1, la_GW_Shape2, la_GW_Shape3, la_GW_Shape4, la_GW_Shape5, la_GW_Shape6, la_GW_Shape7 };
            
            for (int i = 0; i < pics.Length; i++)
            {
                if (i < xmlGw.ChildNodes.Count)
                {
                    XmlElement x = (XmlElement)xmlGw.ChildNodes[i];
                    int.TryParse(x.GetAttribute("DressMode"), out int shape_no);
                    pics[i].Tag = shape_no;
                    //載入對應的形狀圖片(沒有就清空)
                    string filename = path + "Shape" + shape_no + ".png";
                    pics[i].Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                    pics[i].Visible = pics[i].Image != null;

                    //形狀名稱
                    string shape_name = ini.ReadString(imgPathName + "_Shape", "Shape" + shape_no, "");
                    lbs[i].Text = shape_name;
                }
                else
                {
                    pics[i].Tag = 0;
                    pics[i].Image = null;                              
                }
            }


            try
            {
                int shape_no = (int)Math.Round(CurrentGwMacro[10005 + GwMarcoOffset]); //砂輪形狀
                ChangeGwShape(shape_no, GwMarcoOffset); //顯示目前設定的形狀, 含例外處理, 沒有就選形狀1(只修外徑)
                CurrentGwMacro[10005 + GwMarcoOffset] = SelectShapeNo;//避免有修正成形狀1, 固定寫回

                //砂輪資料
                btn_GWDiameter.DisplayText = CurrentGwMacro[10011 + GwMarcoOffset].ToString(Units.DisplayFmt);//目前砂輪外徑
                btn_GWMinDiameter.DisplayText = CurrentGwMacro[10010 + GwMarcoOffset].ToString(Units.DisplayFmt);//最小砂輪外徑
                btn_GWWidth.DisplayText = CurrentGwMacro[10009 + GwMarcoOffset].ToString(Units.DisplayFmt);//目前砂輪寬度
                btn_GWMinWidth.DisplayText = CurrentGwMacro[10008 + GwMarcoOffset].ToString(Units.DisplayFmt);//最小砂輪寬度
                btn_GWHL.DisplayText = CurrentGwMacro[10006 + GwMarcoOffset].ToString(Units.DisplayFmt);//砂輪柄長
                btn_GWDressTimes.DisplayText = CurrentGwMacro[10014 + GwMarcoOffset].ToString("0");//修整次數                                                         
                btn_GWAirDress.DisplayText = CurrentGwMacro[10015 + GwMarcoOffset].ToString("0");//空修次數
                bool bShowGWHL = CurrentGwMacro[10004 + GwMarcoOffset] == 1;
                la_GWHL.Visible = la_GwData_Code5.Visible = btn_GWHL.Visible = la_GWHLUnit.Visible = bShowGWHL;
                // 斜頭需要設定角度
                CheckOCD2Type(out double ocd2);
                
                btn_GWAngle.DisplayText = Math.Abs(ocd2).ToString(Units.DisplayFmt);
                bool bShowGWAngle = false;
                if (GWType[CurrentEditGwNo - 1] == MachineType.OCD2 || GWType[CurrentEditGwNo - 1] == MachineType.OCD3)
                {
                    bShowGWAngle = true;
                }
                la_GWAngle.Visible = la_GwData_Code6.Visible = btn_GWAngle.Visible = la_GWAngleUnit.Visible = bShowGWAngle;
                //修整條件
                btn_D_DD.DisplayText = CurrentGwMacro[10016 + GwMarcoOffset].ToString(Units.DisplayFmt);//外徑修整量
                btn_D_DDD.DisplayText = CurrentGwMacro[10023 + GwMarcoOffset].ToString(Units.DisplayFmt);//外徑修整預留量
                btn_D_DS.DisplayText = CurrentGwMacro[10017 + GwMarcoOffset].ToString(Units.DisplayFmt);//外徑修整速度
                btn_L_DD.DisplayText = CurrentGwMacro[10024 + GwMarcoOffset].ToString(Units.DisplayFmt);//左側修整量
                btn_L_DDD.DisplayText = CurrentGwMacro[10031 + GwMarcoOffset].ToString(Units.DisplayFmt);//左側修整預留量
                btn_L_DS.DisplayText = CurrentGwMacro[10025 + GwMarcoOffset].ToString(Units.DisplayFmt);//左側修整速度
                btn_R_DD.DisplayText = CurrentGwMacro[10032 + GwMarcoOffset].ToString(Units.DisplayFmt);//右側修整量
                btn_R_DDD.DisplayText = CurrentGwMacro[10039 + GwMarcoOffset].ToString(Units.DisplayFmt);//右側修整預留量
                btn_R_DS.DisplayText = CurrentGwMacro[10033 + GwMarcoOffset].ToString(Units.DisplayFmt);//右側修整速度

                //tb_DiamOfsZ.Text = dgwFile.DGWDiamOffsetZ.ToString(Units.DisplayFmt);
                tb_DiamOfsZ.Text = CurrentGwMacro[10048 + GwMarcoOffset].ToString(Units.DisplayFmt);//成形 - 外徑修整Z軸補正

                //外徑修整啟始方向(位置) (0:從砂輪左側修到右側, 1:從砂輪右側修到左側)
                if (Math.Round(CurrentGwMacro[10019 + GwMarcoOffset]) == 0)
                {
                    pa_DressToRight.BackColor = Color.Lime;
                    pa_DressToLeft.BackColor = Color.Transparent;
                }
                else
                {
                    pa_DressToLeft.BackColor = Color.Lime;
                    pa_DressToRight.BackColor = Color.Transparent;
                }

                //砂輪往復修整
                if (Math.Round(CurrentGwMacro[10020 + GwMarcoOffset]) == 1)
                {
                    btn_ToAndBack.DisplayText = "ON";
                }
                else
                {
                    btn_ToAndBack.DisplayText = "OFF";
                }

                //動力修砂轉向
                int index = (int)Math.Round(CurrentGwMacro[10007 + GwMarcoOffset]);
                if (index < 0 || index > 1) index = 0;//例外處理
                                                      //if (index < CB_PWGWDress.Items.Count) CB_PWGWDress.SelectedIndex = index;
                btn_RollerRotation.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", (1008 + index), "正轉/反轉");

                //修整不補正座標系
                index = (int)Math.Round(CurrentGwMacro[10047 + GwMarcoOffset]);
                if (index < 0 || index > 1) index = 0;//例外處理
                                                      //if (index < cb_DressGwNoOffset.Items.Count) cb_DressGwNoOffset.SelectedIndex = index;
                btn_DressGwNoOffset.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", (1010 + index), "補正/不補正");

                //加工完成後砂輪停止
                index = (int)Math.Round(CurrentGwMacro[10013 + GwMarcoOffset]);
                if (index < 0 || index > 1) index = 0;//例外處理
                                                      //cb_AfterMachining.SelectedIndex = index;
                btn_AfterMachining.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", (1002 + index), "不停止/停止");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Fo_Msg.Show(ex.Message);
            }


            //GwSetEdit = false;

            TC_GW.SelectedTab = tab_Gw_GwData;


            btn_GWDiameter.PerformClick();
        }


        private bool WritePath()
        {
            //成形修整路徑轉出程式碼
            //編輯砂輪資料按下儲存才會寫到對應的程式號
            //GW1 : O8002
            //GW2 : O8003
            //GW3 : O8004
            int prog_no = 8002 + (CurrentEditGwNo - 1);
            string src = "";

            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                src = CompilerPath(prog_no);
                bFinish = true;
            }));

            int iStart = Environment.TickCount;
            while (!bFinish)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 5000)
                {

                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                    return false;
                }
                Application.DoEvents();
            }

            //換行格式切換
            src = src.Replace("\r\n", "\n");
            //依照換行符號將資料分行
            String[] lines = src.Split('\n');

            //先將程式輸出至檔案
            String FileName = Application.StartupPath + "\\ncfiles\\O" + prog_no + ".txt";
            File.WriteAllLines(FileName, lines);


            //有連線才寫
            if (!focas.IsConnected())
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 88, "Device Disconnect."));
                return false;
            }


            //不是在EDIT模式
            if (!btn_EDIT.Lamp)
            {
                Actions.Enqueue(new Action(() =>
                {
                    focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
                }));
                //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 89, "Not EDIT Mode."));
                //return false;
            }

            int ret = -1;
            Actions.Enqueue(new Action(() =>
            {
                focas.DeleteNcProgram("//CNC_MEM/USER/PATH1/O" + prog_no);
                ret = focas.WriteFile(FileType.NC_Program, lines.ToList(), "//CNC_MEM/USER/PATH1/");
            }));

            if (ret != SUCCESS)
            {
                //寫入失敗
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 90, "Write NC Program Fail.") + "(" + ret.ToString() + ")");
                return false;
            }


            //設定程式號 1=O8001, 2=O8002 ...
            //ret = focas.WriteMacro(958, 1);

            Actions.Enqueue(new Action(() =>
            {
                //設定主程式為選擇的程式
                focas.SelectMainNcProgram("//CNC_MEM/USER/PATH1/O8000");
            }));

            //切回到監視頁面
            //btn_Monitor.PerformClick();
            return true;
        }

        private void SaveGwData()
        {
            int gw_no = CurrentEditGwNo;

            double val;

            int shift = (gw_no - 1) * 200;

            Actions.Enqueue(new Action(() =>
            {
                //更新 - 砂輪資料
                focas.WriteMacro(10005 + shift, CurrentGwMacro[10005 + shift]);//修整(形狀)模式選擇
                focas.WriteMacro(10009 + shift, CurrentGwMacro[10009 + shift]);//砂輪目前寬度
                focas.WriteMacro(10008 + shift, CurrentGwMacro[10008 + shift]);//砂輪最小寬度
                focas.WriteMacro(10006 + shift, CurrentGwMacro[10006 + shift]);//砂輪炳長
                focas.WriteMacro(10011 + shift, CurrentGwMacro[10011 + shift]);//砂輪目前外徑
                focas.WriteMacro(10010 + shift, CurrentGwMacro[10010 + shift]);//砂輪最小外徑
                focas.WriteMacro(10013 + shift, CurrentGwMacro[10013 + shift]);//加工結束停止砂輪
                focas.WriteMacro(10014 + shift, CurrentGwMacro[10014 + shift]);//修整次數
                focas.WriteMacro(10015 + shift, CurrentGwMacro[10015 + shift]);//空修次數


                //更新 T Code 只能傳整數, 控制器內會自己轉公英制
                int TCode_Z = (int)Math.Round((CurrentGwMacro[10009 + shift] + CurrentGwMacro[10006 + shift]) / (bInchTrans ? 0.00001 : 0.0001));
                focas.WriteGeom(GeomType.Z, 124 + gw_no, TCode_Z);
                int TCode_X = (int)Math.Round(CurrentGwMacro[10011 + shift] / (bInchTrans ? -0.00001 : -0.0001));
                focas.WriteGeom(GeomType.X, 124 + gw_no, TCode_X);

                //更新 - 修整條件
                focas.WriteMacro(10016 + shift, CurrentGwMacro[10016 + shift]);//外徑修整量
                focas.WriteMacro(10017 + shift, CurrentGwMacro[10017 + shift]);//外徑修整速度
                focas.WriteMacro(10023 + shift, CurrentGwMacro[10023 + shift]);//外徑修整預留量
                focas.WriteMacro(10024 + shift, CurrentGwMacro[10024 + shift]);//左側修整量
                focas.WriteMacro(10031 + shift, CurrentGwMacro[10031 + shift]);//左側修整預留量
                focas.WriteMacro(10025 + shift, CurrentGwMacro[10025 + shift]);//左側修整速度
                focas.WriteMacro(10032 + shift, CurrentGwMacro[10032 + shift]);//右側修整量
                focas.WriteMacro(10039 + shift, CurrentGwMacro[10039 + shift]);//右側修整預留量
                focas.WriteMacro(10033 + shift, CurrentGwMacro[10033 + shift]);//右側修整速度
                focas.WriteMacro(10020 + shift, CurrentGwMacro[10020 + shift]);//往復修整 (0:無往復, 1:往復修整)
                focas.WriteMacro(10019 + shift, CurrentGwMacro[10019 + shift]);//修整方向 (0:由左至右, 1:由右至左)
                focas.WriteMacro(10007 + shift, CurrentGwMacro[10007 + shift]);//動力修砂方向 (0:正轉, 1:反轉)
                focas.WriteMacro(10047 + shift, CurrentGwMacro[10047 + shift]);//加工不補正 (0:補正, 1:不補正)

            }));

            //成型修整
            if (SelectShapeNo == 8)
            {
                dgwFile.SaveToFile(Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml");

                int mode = 0;
                if (dgwFile.LeftList.Count > 0) mode |= 1;//成形模式 - 包含左側修整
                if (dgwFile.DiamList.Count > 0) mode |= 2;//成形模式 - 包含外徑修整
                if (dgwFile.RightList.Count > 0) mode |= 4;//成形模式 - 包含右側修整(預留)

                Actions.Enqueue(new Action(() =>
                {
                    focas.WriteMacro(10048 + shift, mode);//成形模式
                    focas.WriteMacro(10049 + shift, CurrentGwMacro[10049 + shift]);//成形外徑修整Z軸補正
                    focas.WriteMacro(10050 + shift, dgwFile.LeftList.Count > 0 ? dgwFile.LeftList[0].X * 2 : 0);//左側起始位置X
                    focas.WriteMacro(10051 + shift, dgwFile.LeftList.Count > 0 ? dgwFile.LeftList[0].Z : 0);//左側起始位置Z
                    focas.WriteMacro(10052 + shift, dgwFile.DiamList.Count > 0 ? dgwFile.DiamList[0].X * 2 : 0);//外徑起始位置X
                    focas.WriteMacro(10053 + shift, dgwFile.DiamList.Count > 0 ? dgwFile.DiamList[0].Z : 0);//外徑起始位置Z
                    focas.WriteMacro(10054 + shift, dgwFile.RightList.Count > 0 ? dgwFile.RightList[0].X * 2 : 0);//右側起始位置X
                    focas.WriteMacro(10055 + shift, dgwFile.RightList.Count > 0 ? dgwFile.RightList[0].Z : 0);//右側起始位置Z
                    focas.WriteMacro(10056 + shift, CurrentGwMacro[10056 + shift]);//外徑刀尖功能
                    focas.WriteMacro(10057 + shift, CurrentGwMacro[10057 + shift]);//外徑刀尖半徑
                    focas.WriteMacro(10058 + shift, CurrentGwMacro[10058 + shift]);//左側刀尖功能
                    focas.WriteMacro(10059 + shift, CurrentGwMacro[10059 + shift]);//左側刀尖半徑
                }));
                WritePath();//將路徑寫到控制器 GW1:O8002, GW2:O8003, GW3:O8004


            }
            else
            {
                int no;
                //砂輪形狀參數
                for (int i = 0; i < DGV_GwParam.Rows.Count; i++)
                {
                    if (!double.TryParse(DGV_GwParam.Rows[i].Cells[Col_GP_Value.Index].Value.ToString(), out val)) continue;
                    if (!int.TryParse(DGV_GwParam.Rows[i].Cells[Col_GwParam_MacroNo.Index].Value.ToString(), out no)) continue;

                    Actions.Enqueue(new Action(() =>
                    {
                        focas.WriteMacro(no + shift, val);
                    }));
                }
            }

            //GwSetEdit = false;
        }

        private void btn_RegisterGw_Save_Click(object sender, EventArgs e)
        {
            SaveGwData();

            btn_Prev.PerformClick();
        }

        private void btn_Gw_GwData_Click(object sender, EventArgs e)
        {
            TC_GW.SelectedTab = tab_Gw_GwData;
        }

        //下一步(盡可能改成只切畫面，徑可能在按下直頭、斜頭、內圓砂輪時就讀完)
        private void btn_Gw_ShapeData_Click(object sender, EventArgs e)
        {
            //從目前顯示的型狀判斷選擇的砂輪號
            int gw_no = CurrentEditGwNo;

            if (SelectShapeNo == 8)//成型修整(不是我們出廠寫好的型狀，而是由客戶自行去構成)，要另外的頁面處理
            {
                //成形修整編輯畫面上方 顯示目前砂輪尺寸
                la_OD_Path_Width_Val.Text = btn_GWWidth.DisplayText;
                la_OD_Path_Diameter_Val.Text = btn_GWDiameter.DisplayText;

                if (Rightopen) btn_EditRightPath.Visible = Rightopen;//右側修整(預留)

                //成形修整 - 預覽
                btn_PathPreview.PerformClick();

                TC_GW.SelectedTab = tab_Gw_ShapeEdit;//成型修整設定頁面
                return;
            }


            if (SelectShapeNo != LastGwShapeNo) //有換形狀時要重新產生 對應的 參數
            {
                LastGwShapeNo = SelectShapeNo;

                //String AppPath = Application.StartupPath;
                DGV_GwParam.Rows.Clear();

                int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
                //讀取 - 砂輪形狀資料
                XmlElement xmlGwType = (CurrentGwMacro[10004 + GwMarcoOffset] == 0 ? machineSetting.xmlOCD_Param : machineSetting.xmlOIG_Param);
                double dGwType = CurrentGwMacro[10004 + GwMarcoOffset];

               
                int iOCD2OrOCD3 = 0;
                if (GWType[CurrentEditGwNo - 1] == MachineType.OCD2)
                {
                    xmlGwType = machineSetting.xmlOCD_PA_Param;
                    iOCD2OrOCD3 = 2;
                }
                if (GWType[CurrentEditGwNo - 1] == MachineType.OCD3)
                {
                    xmlGwType = machineSetting.xmlOCD_NA_Param;
                    iOCD2OrOCD3 = 3;
                }
                if (xmlGwType == null) return;//例外處理
                XmlElement xmlShape = xmlGwType.GetShape(SelectShapeNo);//尋找形狀節點
                if (xmlShape == null) return;//例外處理

                TIniFile ini = new TIniFile(Units.langfile);//多國語言

                string filepath = Application.StartupPath + "\\image\\" + (CurrentGwMacro[10004 + GwMarcoOffset] == 1 ? "OIG" : "OCD") + "\\Param\\";
                if(iOCD2OrOCD3 == 2)
                {
                    filepath = Application.StartupPath + "\\image\\" + "OCD2" + "\\Param\\";
                }
                if (iOCD2OrOCD3 == 3)
                {
                    filepath = Application.StartupPath + "\\image\\" + "OCD3" + "\\Param\\";
                }
                //將形狀參數 讀取到 DataGridView
                for (int i = 0; i < xmlShape.ChildNodes.Count; i++)
                {
                    XmlElement xParam = (XmlElement)xmlShape.ChildNodes[i];
                    if (xParam.Name != "Param") continue;
                    int.TryParse(xParam.GetAttribute("Macro"), out int macro_no);
                    string pic_filename = filepath + "Shape" + SelectShapeNo + "\\" + (i + 1) + ".png";//設定圖片路徑
                    string param_name = ini.ReadString("Macro", macro_no.ToString(), "");//從語言檔 讀取 參數名稱
                    DGV_GwParam.Rows.Add(param_name,
                                         CurrentGwMacro[macro_no + GwMarcoOffset].ToString(Units.DisplayFmt),
                                         pic_filename,
                                         macro_no);

                    if (i == 0)
                    {
                        pic_GW_Param.BackgroundImage = File.Exists(pic_filename) ? new Bitmap(pic_filename) : null;
                    }
                }

                for (int i = 0; i < DGV_GwParam.Rows.Count; i++)
                {
                    DGV_GwParam.Rows[i].Height = 48; //使用者好按的大小
                }

                if (DGV_GwParam.Rows.Count > 0) //預設選擇的參數
                {
                    DGV_GwParam.Rows[0].Cells[0].Selected = true;

                    //設定數字鍵盤
                    uc_UserNumSetGW.la_Msg.Text = DGV_GwParam.Rows[0].Cells[0].Value.ToString(); //顯示名稱
                    uc_UserNumSetGW.la_Num.Text = DGV_GwParam.Rows[0].Cells[1].Value.ToString(); //顯示數值
                    int.TryParse(DGV_GwParam.CurrentRow.Cells[3].Value.ToString(), out int no); // 抓隱藏欄位 macro no
                    Units.MacroInfo.GetMinMax(no, out double min, out double max); // 抓上下限並顯示
                    if ((min != 0 || max != 0))
                    {
                        uc_UserNumSetGW.la_Msg.Text += "\r\n" + min + " ~ " + max; // 0 ~ 0 就不顯示
                    }
                }
            }

            TC_GW.SelectedTab = tab_Gw_ShapeData;
        }

        private void SetHead(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = sender as Uc_RoundBtn;
            if (btn == null) return;
            int tag = int.Parse(btn.Tag.ToString());

            string msg = "";
            if (tag == 1) msg = LanguageManager.LoadMessage(Units.langfile, "Message", 40, "是否要旋轉到砂輪 1");
            else if (tag == 2) msg = LanguageManager.LoadMessage(Units.langfile, "Message", 41, "是否要旋轉到砂輪 2");
            else if (tag == 3) msg = LanguageManager.LoadMessage(Units.langfile, "Message", 42, "是否要旋轉到砂輪 3");

            DialogResult ret = Fo_Msg.Show(msg, LanguageManager.LoadMessage(Units.langfile, "Message", 6, "注意"), MessageBoxButtons.YesNo);

            if (ret == DialogResult.Yes)
            {
                //設定轉頭目標站號
                if (focas.WriteMacro(507, tag) != Focas1.EW_OK)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 91, "Write #507 Fail."));
                    return;
                }

                //畫面啟動程式(轉頭)
                if (focas.WriteMacro(980, 5) != Focas1.EW_OK)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 92, "Write #980 Fail."));
                    return;
                }

                OneKeyCall(8999);

            }
        }

        public void ShowOSKeyboard(object sender, EventArgs e)
        {
            try
            {
                Process p = Process.Start(Environment.SystemDirectory + "\\osk.exe");
            }
            catch (Exception)
            {
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                Process p = Process.Start(Environment.SystemDirectory + "\\osk.exe");
            }
            catch (Exception)
            {
            }
        }

        private void masterSerialBus1_OnSend(object sender, string cmd)
        {
            if (TC_Main.SelectedTab == tab_Developer) tb_serial.AppendText("S:" + cmd + "\r\n");
        }


        private void pic_Position_Click(object sender, EventArgs e) //位置設定
        {

            tc_PositionSet.SelectedTab = tab_PosSet_ChangePartPos; //預設在換料位置
            int GwNo = 0;
            double PosX = 0;
            double PosZ = 0;
            double TowerPosX = 0;
            double TowerPosZ = 0;
            double M564 = 0;
            double M565 = 0;
            double DressBaseMax = 0;
            double DressBaseMin = 0;
            SysInfoEx info = null;
            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                focas.ReadMacro(506, out double no);//讀取目前的砂輪號
                GwNo = (int)Math.Round(no);
                if (GwNo < 1 || GwNo > 4)
                {
                    bFinish = true;
                    return;
                }

                ReadGwMacro(GwNo); //讀取 砂輪資料 (CurrentGwMacro)
                focas.SystemInfoEx(out info);//讀取系統資訊
                focas.Param_ReadDouble(1241, 1, out PosX);//換料位置X
                focas.Param_ReadDouble(1241, 2, out PosZ);//換料位置Z
                // 轉頭安全位置
                focas.Param_ReadDouble(1243, 1, out TowerPosX);//換料位置X
                focas.Param_ReadDouble(1243, 2, out TowerPosZ);//換料位置Z
                // 內圓修砂安全區間
                focas.ReadMacro(564, out M564);
                focas.ReadMacro(565, out M565);
                // 修整座最大最小值
                focas.Param_ReadDouble(6934, 0, out DressBaseMax);
                focas.Param_ReadDouble(6954, 0, out DressBaseMin);
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


            if (GwNo < 1 || GwNo > 4)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"));
                return;
            }
            int shift = (GwNo - 1) * 200;
            //內圓中心位置設定
            btn_PosSet_IDCenterPos.Visible = CurrentGwMacro[10004 + shift] == 1;

            //外圓安全位置
            btn_PosSet_ODSafePos.Visible = CurrentGwMacro[10004 + shift] == 0;

            //GW 內圓工件中心位置
            la_PartCenterPosX.Text = CurrentGwMacro[10002 + shift].ToString(Units.DisplayFmt);

            //GW 外圓安全位置 
            la_ODSafePos_X.Text = tb_Gw2_OD_SafePosX.Text = CurrentGwMacro[10003 + shift].ToString(Units.DisplayFmt);

            //參考位置1(換料位置) X軸
            if (bInchTrans) PosX /= 25.4;
            TB_ChangePartPosX.Text = PosX.ToString(Units.DisplayFmt);

            //參考位置1(換料位置) Z軸
            if (bInchTrans) PosZ /= 25.4;
            TB_ChangePartPosZ.Text = PosZ.ToString(Units.DisplayFmt);

            // 轉頭安全位置
            if (bInchTrans) TowerPosX /= 25.4;
            TB_TowerSafePosX.Text = TowerPosX.ToString(Units.DisplayFmt);
            
            if (bInchTrans) TowerPosZ /= 25.4;
            TB_TowerSafePosZ.Text = TowerPosZ.ToString(Units.DisplayFmt);

            // 內圓修砂安全區間
            TB_ID_DressRevZ1.Text = M564.ToString(Units.DisplayFmt);
            TB_ID_DressRevZ2.Text = M565.ToString(Units.DisplayFmt);

            // 修整座最大最小值
            if (bInchTrans) DressBaseMax /= 25.4;
            TB_DressBaseMax.Text = DressBaseMax.ToString(Units.DisplayFmt);

            if (bInchTrans) DressBaseMin /= 25.4;
            TB_DressBaseMin.Text = DressBaseMin.ToString(Units.DisplayFmt);

            //清空上次的設定位置
            TB_Gw1PosX1.Text = "";
            TB_Gw1PosX2.Text = "";

            //依照目前軸數顯示

            la_PosSetMach_1.Visible = la_PosSetMach1.Visible = info.Axis > 0;
            la_PosSetMach_2.Visible = la_PosSetMach2.Visible = info.Axis > 1;
            la_PosSetMach_3.Visible = la_PosSetMach3.Visible = info.Axis > 2;
            la_PosSetMach_4.Visible = la_PosSetMach4.Visible = info.Axis > 3;
            la_PosSetMach_5.Visible = la_PosSetMach5.Visible = info.Axis > 4 && machineSeries != "M";
            la_PosSetMach_6.Visible = la_PosSetMach6.Visible = info.Axis > 5 && machineSeries != "M";
            la_PosSetMach1.Text = la_MonitorAbsAxis1.Text;
            la_PosSetMach2.Text = la_MonitorAbsAxis2.Text;
            la_PosSetMach3.Text = la_MonitorAbsAxis3.Text;
            la_PosSetMach4.Text = la_MonitorAbsAxis4.Text;
            la_PosSetMach5.Text = la_MonitorAbsAxis5.Text;
            la_PosSetMach6.Text = la_MonitorAbsAxis6.Text;


            TIniFile ini = new TIniFile(Units.langfile);

            la_PositionSetting.Text = ini.ReadString("Fo_Main", "la_PositionSetting", "位置設定 - 砂輪") + GwNo;

            TC_Main.SelectedTab = tab_PosSet;

            PrevPage.Push(tab_PosSet);
            btn_Prev.Visible = true;
        }

        private void btn_ID_RetractPosZ_Click(object sender, EventArgs e)
        {
            if (Pos.Machine.Length > 1) TB_ChangePartPosZ.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bChangePart = true;
        }

        private void btn_ID_RetractPosX_Click(object sender, EventArgs e)
        {
            if (Pos.Machine.Length > 0) TB_ChangePartPosX.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bChangePart = true;
        }

        private void SetGw1Speed(double val)
        {
            double dVal = val;
            if (dVal < Gw1.MinRpm) dVal = Gw1.MinRpm;
            if (dVal > Gw1.MaxRpm) dVal = Gw1.MaxRpm;

            //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
            this.Gw1.CmdSpeed = dVal / this.Gw1.Rate;
            btn_Gw1CmdRpm.DisplayText = dVal.ToString("0");


            //傳送指令到變頻器
            if (Gw1Dev == 0)//士林變頻器
            {
                masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
            }
            else if (Gw1Dev == 1)//台達變頻器
            {
                masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
            }
            else if (Gw1Dev == 2)//三菱變頻器
            {
                masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
            }
            //紀錄
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteFloat("Gw1", "Cmd", this.Gw1.CmdSpeed);
        }

        private void SetGw2Speed(double val)
        {
            double dVal = val;
            if (dVal < Gw2.MinRpm) dVal = Gw2.MinRpm;
            if (dVal > Gw2.MaxRpm) dVal = Gw2.MaxRpm;

            //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
            this.Gw2.CmdSpeed = dVal / this.Gw2.Rate;
            btn_Gw2CmdRpm.DisplayText = dVal.ToString("0");


            //傳送指令到變頻器
            if (Gw2Dev == 0)//士林變頻器
            {
                masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
            }
            else if (Gw2Dev == 1)//台達變頻器
            {
                masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
            }
            //紀錄
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteFloat("Gw2", "Cmd", this.Gw2.CmdSpeed);
        }

        private void SetGw3Speed(double val)
        {
            double dVal = val;
            if (dVal < Gw3.MinRpm) dVal = Gw3.MinRpm;
            if (dVal > Gw3.MaxRpm) dVal = Gw3.MaxRpm;

            //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
            this.Gw3.CmdSpeed = dVal / this.Gw3.Rate;
            btn_Gw3CmdRpm.DisplayText = dVal.ToString("0");


            //傳送指令到變頻器
            if (Gw3Dev == 0)//士林變頻器
            {
                masterSerialBus1.Add(this.Gw3.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw3.CmdSpeed / Gw3.Unit)).ToString("X4"));
            }
            else if (Gw3Dev == 1)//台達變頻器
            {
                masterSerialBus1.Add(this.Gw3.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw3.CmdSpeed / Gw3.Unit)).ToString("X4"));
            }
            //紀錄
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteFloat("Gw3", "Cmd", this.Gw3.CmdSpeed);
        }


        private void btn_ArgNormal_Click(object sender, EventArgs e)
        {
            TC_EditProc.SelectedTab = tab_Param1;
            if (DGV_Param1.Rows.Count == 0) return;
            if (DGV_Param1.Rows[0].Cells.Count == 0) return;
            DGV_Param1.Rows[0].Cells[0].Selected = true;
            //DataGridViewCellEventArgs d = new DataGridViewCellEventArgs(0, 0);
            //DressMode1 = false;
            //DressMode2 = false;
            DGV_CellClick(DGV_Param1, null);
            DGV_Param1.Focus();
        }
        private void btn_ArgParam2_Click(object sender, EventArgs e)
        {
            TC_EditProc.SelectedTab = tab_Param2;
            if (DGV_Param2.Rows.Count == 0) return;
            if (DGV_Param2.Rows[0].Cells.Count == 0) return;
            DGV_Param2.Rows[0].Cells[0].Selected = true;
            //DataGridViewCellEventArgs d = new DataGridViewCellEventArgs(0, 0);
            //DressMode1 = false;
            //DressMode2 = false;
            DGV_CellClick(DGV_Param2, null);
            DGV_Param2.Focus();
        }

        private void btn_ArgParam3_Click(object sender, EventArgs e)
        {
            TC_EditProc.SelectedTab = tab_Param3;
            if (DGV_Param3.Rows.Count == 0) return;
            if (DGV_Param3.Rows[0].Cells.Count == 0) return;
            DGV_Param3.Rows[0].Cells[0].Selected = true;
            //DataGridViewCellEventArgs d = new DataGridViewCellEventArgs(0, 0);
            //DressMode1 = false;
            //DressMode2 = false;
            DGV_CellClick(DGV_Param3, null);
            DGV_Param3.Focus();
        }

        private void btn_ArgAdvance_Click(object sender, EventArgs e)
        {
            TC_EditProc.SelectedTab = tab_Advance;
            if (DGV_Advance.Rows.Count == 0) return;
            if (DGV_Advance.Rows[0].Cells.Count == 0) return;
            DGV_Advance.Rows[0].Cells[0].Selected = true;
            //DataGridViewCellEventArgs d = new DataGridViewCellEventArgs(0, 0);
            //DressMode1 = false;
            //DressMode2 = false;
            DGV_CellClick(DGV_Advance, null);
            DGV_Advance.Focus();
        }

        private void btn_ArgDress1_Click(object sender, EventArgs e)
        {
            TC_EditProc.SelectedTab = tab_Dress1;
            if (DGV_Dress1.Rows.Count == 0) return;
            if (DGV_Dress1.Rows[0].Cells.Count == 0) return;
            DGV_Dress1.Rows[0].Cells[0].Selected = true;
            //DataGridViewCellEventArgs d = new DataGridViewCellEventArgs(0, 0);
            //DressMode1 = true;
            //DressMode2 = false;
            DGV_CellClick(DGV_Dress1, null);
            DGV_Dress1.Focus();
        }

        private void btn_ArgDress2_Click(object sender, EventArgs e)
        {
            TC_EditProc.SelectedTab = tab_Dress2;
            if (DGV_Dress2.Rows.Count == 0) return;
            if (DGV_Dress2.Rows[0].Cells.Count == 0) return;
            DGV_Dress2.Rows[0].Cells[0].Selected = true;
            //DataGridViewCellEventArgs d = new DataGridViewCellEventArgs(0, 0);
            //DressMode1 = false;
            //DressMode2 = true;
            DGV_CellClick(DGV_Dress2, null);
            DGV_Dress2.Focus();
        }

        private void TC_EditProc_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_ArgParam.Lamp = TC_EditProc.SelectedTab == tab_Param1 ? true : false;
            btn_ArgParam2.Lamp = TC_EditProc.SelectedTab == tab_Param2 ? true : false;
            btn_ArgParam3.Lamp = TC_EditProc.SelectedTab == tab_Param3 ? true : false;
            btn_ArgAdvance.Lamp = TC_EditProc.SelectedTab == tab_Advance ? true : false;
            btn_ArgDress1.Lamp = TC_EditProc.SelectedTab == tab_Dress1 ? true : false;
            btn_ArgDress2.Lamp = TC_EditProc.SelectedTab == tab_Dress2 ? true : false;
            //btn_ArgDress2.BackColor = TC_EditProc.SelectedTab == tab_Measure ? Color.Lime : SystemColors.Control;
        }


        //執行 建立量測資料 流程
        private bool bCreateMeasData = false;
        private void btn_CreateMeasData_Click(object sender, EventArgs e)
        {

            //隱藏訊息
            la_MeasureMsg.Visible = false;

            //如果沒有執行中，就開始執行
            if (!bCreateMeasData)
            {
                //設為執行中
                bCreateMeasData = true;

                //粗磨測點控制
                if (!double.TryParse(tb_MeasureRoughPos.Text, out double val))
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 66, "資料輸入錯誤");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bCreateMeasData = false;
                    return;
                }
                int Rough = (int)(val * 1000.0);
                byte Rough_a = (byte)Rough;
                byte Rough_b = (byte)(Rough >> 8);

                //細磨測點控制
                if (!double.TryParse(tb_MeasureFinePos.Text, out val))
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 66, "資料輸入錯誤");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bCreateMeasData = false;
                    return;
                }
                int Fine = (int)(val * 1000.0);
                byte Fine_a = (byte)Fine;
                byte Fine_b = (byte)(Fine >> 8);

                //精磨測點控制
                if (!double.TryParse(tb_MeasurePrecisionPos.Text, out val))
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 66, "資料輸入錯誤");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bCreateMeasData = false;
                    return;
                }
                int Precision = (int)(val * 1000.0);
                byte Precision_a = (byte)Precision;
                byte Precision_b = (byte)(Precision >> 8);

                //判斷 R1092.4 必須在手動模式 
                byte R1092;
                focas.PMC_ReadByte(PmcAddrType.R, 1092, out R1092);
                if (!R1092.BIT_4())
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 67, "錯誤，請先切到手動模式");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bCreateMeasData = false;
                    return;
                }

                //讀取PLC狀態
                focas.PMC_ReadByte(PmcAddrType.Y, 2, out byte Y2);
                focas.PMC_ReadByte(PmcAddrType.Y, 3, out byte Y3);

                //量測組別(for NC Code 引數)
                int.TryParse(cb_MeasureGroup.Text, out int iGroup);

                //量測組別 (for P7)
                byte Group = 0;
                if (iGroup == 1) Group = 0;
                else if (iGroup == 2) Group = 0x10;

                //依照設定組別偵測硬體狀態
                //第一組
                if (iGroup == 1)
                {
                    //量測1的測爪
                    if (Y2.BIT_3())
                    {
                        //顯示錯誤訊息
                        la_MeasureMsg.BackColor = Color.Red;
                        la_MeasureMsg.ForeColor = Color.Yellow;
                        la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 68, "錯誤，請先量測縮回");
                        la_MeasureMsg.Visible = true;
                        //設為停止
                        bCreateMeasData = false;
                        return;
                    }
                }
                //第二組
                else if (iGroup == 2)
                {
                    //量測1的測爪
                    if (Y3.BIT_4())
                    {
                        //顯示錯誤訊息
                        la_MeasureMsg.BackColor = Color.Red;
                        la_MeasureMsg.ForeColor = Color.Yellow;
                        la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 68, "錯誤，請先量測縮回");
                        la_MeasureMsg.Visible = true;
                        //設為停止
                        bCreateMeasData = false;
                        return;
                    }
                }

                //P7 指令
                byte[] cmd1 = { 0, 0, 0, 0, 0, 13 }; //Set Measure System
                byte[] cmd2 = { 0, 0, 0, Group, 0, 14 }; //Get Information
                byte[] cmd3 = { 0, 0, 0, Group, 0, 1 };//Set Writer Program Data Session Enabled
                byte[] cmd4 = { 0, 0, Rough_b, Rough_a, 0, 141 };//Set Measure Control 3
                byte[] cmd5 = { 0, 0, Fine_b, Fine_a, 0, 140 };//Set Measure Control 2
                byte[] cmd6 = { 0, 0, Precision_b, Precision_a, 0, 139 };//Set Measure Control 1
                byte[] cmd7 = { 0, 0, 0, 0, 0, 10 };//Set Check program data
                byte[] cmd8 = { 0, 0, 0, 1, 0, 3 };//Set Close Programmed Data session Leave
                object[] cmds = { cmd1, cmd2, cmd3, cmd4, cmd5, cmd6, cmd7, cmd8 };

                //接收回碼
                byte[] response = { 0, 0, 0, 0, 0, 0 };
                new Thread(() =>
                {
                    //檢查是否下達成功
                    bool bFail = false;

                    //傳送所有指令
                    for (int j = 0; j < cmds.Length; j++)
                    {
                        //不能中止，指令必須要有Check 跟 Close，否則後續操作會有問題
                        //if (!bCreateMeasData) return;

                        //指令
                        byte[] cmd = cmds[j] as byte[];
                        //主要的指令碼(指令下達成功時會回這個指令碼)
                        byte cmd_code = cmd[5];

                        //傳送指令(R2164 Code 最後送，其他資料先送)
                        this.Invoke(new Action(() =>
                        {
                            //傳送指令
                            for (int i = 0; i < 6; i++) focas.PMC_WriteByte(PmcAddrType.R, (short)(2169 - i), cmd[i]);
                        }));

                        //延遲
                        Thread.Sleep(100);

                        //接收回碼 R2114~R2119
                        this.Invoke(new Action(() =>
                        {
                            //接收指令
                            for (int i = 0; i < 6; i++) focas.PMC_ReadByte(PmcAddrType.R, (ushort)(2114 + i), out response[i]);
                        }));


                        //R2114=13 && R2116=1 (只有這個指令碼要多判斷資料)
                        if ((j == 1) && ((response[0] != cmd_code) || (response[2] != 1))) bFail = true;
                        else if (response[0] != cmd_code) bFail = true;

                        //清除
                        this.Invoke(new Action(() =>
                        {
                            ProfibusCmd_Clear();
                        }));

                        Thread.Sleep(100);
                    }

                    this.Invoke(new Action(() =>
                    {
                        if (bFail)
                        {
                            la_MeasureMsg.BackColor = Color.Red;
                            la_MeasureMsg.ForeColor = Color.Yellow;
                            la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 69, "建立失敗");
                        }
                        else
                        {
                            la_MeasureMsg.BackColor = Color.Blue;
                            la_MeasureMsg.ForeColor = Color.Yellow;
                            la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 70, "建立完成");
                        }
                        la_MeasureMsg.Visible = true;
                    }));

                    //設為停止
                    bCreateMeasData = false;

                    TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                    String gp = "Group" + cb_MeasureGroup.Text;
                    ini.WriteBool("Measure", gp + "_CreateFinish", true);
                    ini.WriteString("Measure", gp + "_Rough", tb_MeasureRoughPos.Text);
                    ini.WriteString("Measure", gp + "_Fine", tb_MeasureFinePos.Text);
                    ini.WriteString("Measure", gp + "_Precision", tb_MeasurePrecisionPos.Text);


                }).Start();
            }
            //如果執行中，就停止
            else
            {
                //停止
                bCreateMeasData = false;
            }
        }

        private void ProfibusCmd_Clear()
        {
            focas.PMC_WriteByte(PmcAddrType.R, 2169, 0);
            focas.PMC_WriteByte(PmcAddrType.R, 2168, 0);
            focas.PMC_WriteByte(PmcAddrType.R, 2167, 0);
            focas.PMC_WriteByte(PmcAddrType.R, 2166, 0);
            focas.PMC_WriteByte(PmcAddrType.R, 2165, 0);
            focas.PMC_WriteByte(PmcAddrType.R, 2164, 0);
        }

        private bool bOnload_DGV_Advance = false;

        private void cb_MeasureGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProcessIndex < 0) return;
            if (bOnload_DGV_Advance) return;



            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            tb_MeasureRange.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Range", 0).ToString(Units.DisplayFmt);
            tb_MeasureRoughPos.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Rough", 0).ToString(Units.DisplayFmt);
            tb_MeasureFinePos.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Fine", 0).ToString(Units.DisplayFmt);
            tb_MeasurePrecisionPos.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Precision", 0).ToString(Units.DisplayFmt);
            tb_MeasureSparkless.Text = ini.ReadFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Sparkless", 0).ToString(Units.DisplayFmt);

            btn_MeasureElecZero.Enabled = false;
            //pic_Warning.Visible = true;


            la_MeasureMsg.Visible = false;

            TProcess EditProcess = TempProgram.Processes[ProcessIndex];
            if (EditProcess == null)
                return;
            try
            {
                if (EditProcess.SubPrograms.Count > 1)
                {
                    //量測組別
                    TArgument a = EditProcess.SubPrograms[0].GetArgument("19865");
                    if (a != null)
                    {
                        a.Value = int.Parse(cb_MeasureGroup.Text);
                        btn_SaveProg.Visible = true;
                        btn_SaveProgVisible = true;
                        //找 量測組別
                        /*
                        for (int i = 0; i < DGV_Advance.Rows.Count; i++)
                        {
                            //寫回去到DGV中
                            TArgument find_a = (TArgument)DGV_Advance.Rows[i].Cells[6].Value;
                            if (a == find_a)
                            {
                                DGV_Advance.Rows[i].Cells[0].Value = a.Value.ToString();
                                DGV_Advance.Rows[i].Cells[1].Value = a.Value.ToString();
                            }
                        }*/
                    }
                    //重新讀取
                    //DGV_Edit_CellClick(DGV_Advance, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private bool bSaveMeasureData = false;
        private void btn_SaveMeasureData_Click(object sender, EventArgs e)
        {
            //隱藏訊息
            la_MeasureMsg.Visible = false;

            //如果沒有執行中，就開始執行
            if (!bSaveMeasureData)
            {
                //設為執行中
                bSaveMeasureData = true;

                double val;
                //粗磨測點控制
                if (!double.TryParse(tb_MeasureRoughPos.Text, out val))
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 66, "資料輸入錯誤");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bSaveMeasureData = false;
                    return;
                }
                int Rough = (int)(val * 1000.0);
                byte Rough_a = (byte)Rough;
                byte Rough_b = (byte)(Rough >> 8);
                byte Rough_c = (byte)(Rough >> 16);
                byte Rough_d = (byte)(Rough >> 24);

                //細磨測點控制
                if (!double.TryParse(tb_MeasureFinePos.Text, out val))
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 66, "資料輸入錯誤");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bSaveMeasureData = false;
                    return;
                }
                int Fine = (int)(val * 1000.0);
                byte Fine_a = (byte)Fine;
                byte Fine_b = (byte)(Fine >> 8);
                byte Fine_c = (byte)(Fine >> 16);
                byte Fine_d = (byte)(Fine >> 24);

                //精磨測點控制
                if (!double.TryParse(tb_MeasurePrecisionPos.Text, out val))
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 66, "資料輸入錯誤");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bSaveMeasureData = false;
                    return;
                }
                int Precision = (int)(val * 1000.0);
                byte Precision_a = (byte)Precision;
                byte Precision_b = (byte)(Precision >> 8);
                byte Precision_c = (byte)(Precision >> 16);
                byte Precision_d = (byte)(Precision >> 24);

                //判斷 R1092.4 必須在手動模式 
                byte R1092;
                focas.PMC_ReadByte(PmcAddrType.R, 1092, out R1092);
                if (!R1092.BIT_4())
                {
                    //顯示錯誤訊息
                    la_MeasureMsg.BackColor = Color.Red;
                    la_MeasureMsg.ForeColor = Color.Yellow;
                    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 67, "錯誤，請先切到手動模式");
                    la_MeasureMsg.Visible = true;
                    //設為停止
                    bSaveMeasureData = false;
                    return;
                }

                byte Y2, Y3;
                focas.PMC_ReadByte(PmcAddrType.Y, 2, out Y2);
                focas.PMC_ReadByte(PmcAddrType.Y, 3, out Y3);

                //使用組別(for NC Code 引數)
                int iGroup = 0;
                int.TryParse(cb_MeasureGroup.Text, out iGroup);

                //使用組別(for P7)
                byte Group = 0;
                if (iGroup == 1) Group = 0;
                else if (iGroup == 2) Group = 0x10;

                if (iGroup == 1)
                {
                    if (Y2.BIT_3())
                    {
                        //顯示錯誤訊息
                        la_MeasureMsg.BackColor = Color.Red;
                        la_MeasureMsg.ForeColor = Color.Yellow;
                        la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 68, "錯誤，請先量測縮回");
                        la_MeasureMsg.Visible = true;
                        //設為停止
                        bSaveMeasureData = false;
                        return;
                    }
                }
                if (iGroup == 2)
                {
                    if (Y3.BIT_4())
                    {
                        //顯示錯誤訊息
                        la_MeasureMsg.BackColor = Color.Red;
                        la_MeasureMsg.ForeColor = Color.Yellow;
                        la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 68, "錯誤，請先量測縮回");
                        la_MeasureMsg.Visible = true;
                        //設為停止
                        bSaveMeasureData = false;
                        return;
                    }
                }

                byte[] cmd1 = { 0, 0, 0, 0, 0, 13 }; //Set Measure System
                byte[] cmd2 = { 0, 0, 0, Group, 0, 14 }; //Get Set Information
                byte[] cmd3 = { 0, 0, 0, Group, 0, 1 };//Set Writer Program Data Session Enabled
                byte[] cmd4 = { Rough_d, Rough_c, Rough_b, Rough_a, 0, 141 };//Set Measure Control 3
                byte[] cmd5 = { Fine_d, Fine_c, Fine_b, Fine_a, 0, 140 };//Set Measure Control 2
                byte[] cmd6 = { Precision_d, Precision_c, Precision_b, Precision_a, 0, 139 };//Set Measure Control 1
                byte[] cmd7 = { 0, 0, 0, 0, 0, 10 };//Set Check program data
                byte[] cmd8 = { 0, 0, 0, 1, 0, 3 };//Set Close Programmed Data session Leave
                object[] cmds = { cmd1, cmd2, cmd3, cmd4, cmd5, cmd6, cmd7, cmd8 };


                //顯示錯誤訊息
                la_MeasureMsg.BackColor = Color.Yellow;
                la_MeasureMsg.ForeColor = Color.Black;
                la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 71, "資料寫入中...");
                la_MeasureMsg.Visible = true;
                Application.DoEvents();

                byte[] response = { 0, 0, 0, 0, 0, 0 };
                new Thread(() =>
                {
                    //寫入量測組別
                    byte R2150 = 0;
                    if (iGroup == 2)
                    {
                        R2150 = 0x10;
                    }

                    //切換量測組別
                    this.Invoke(new Action(() =>
                    {
                        focas.PMC_WriteByte(PmcAddrType.R, 2150, R2150);
                    }));

                    Thread.Sleep(100);

                    //清除
                    this.Invoke(new Action(() =>
                    {
                        ProfibusCmd_Clear();
                    }));
                    bool bFail = false;

                    for (int j = 0; j < cmds.Length; j++)
                    {
                        //if (!bSaveMeasureData) return;

                        byte[] cmd = cmds[j] as byte[];
                        byte cmd_code = cmd[5];
                        //傳送指令(R2164 Code 最後送，其他資料先送)
                        this.Invoke(new Action(() =>
                        {
                            for (int i = 0; i < 6; i++) focas.PMC_WriteByte(PmcAddrType.R, (short)(2169 - i), cmd[i]);
                        }));

                        //延遲
                        Thread.Sleep(100);

                        //接收回碼 R2114~R2119
                        this.Invoke(new Action(() =>
                        {
                            for (int i = 0; i < 6; i++) focas.PMC_ReadByte(PmcAddrType.R, (ushort)(2114 + i), out response[i]);
                        }));

                        //R2114=13 && R2116=1
                        if ((j == 1) && ((response[0] != cmd_code) || (response[2] != 1))) bFail = true;
                        else if (response[0] != cmd_code) bFail = true;

                        //清除
                        this.Invoke(new Action(() =>
                        {
                            ProfibusCmd_Clear();
                        }));

                        Thread.Sleep(100);
                    }

                    //String gp = "";
                    this.Invoke(new Action(() =>
                    {
                        if (bFail)
                        {
                            la_MeasureMsg.BackColor = Color.Red;
                            la_MeasureMsg.ForeColor = Color.Yellow;
                            la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 69, "建立失敗");
                        }
                        else
                        {
                            la_MeasureMsg.BackColor = Color.Blue;
                            la_MeasureMsg.ForeColor = Color.Yellow;
                            la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 70, "建立完成");
                        }
                        la_MeasureMsg.Visible = true;
                        btn_MeasureElecZero.Enabled = true;
                        //pic_Warning.Visible = false;

                        TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                        ini.WriteFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Range", double.Parse(tb_MeasureRange.Text));
                        ini.WriteFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Rough", double.Parse(tb_MeasureRoughPos.Text));
                        ini.WriteFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Fine", double.Parse(tb_MeasureFinePos.Text));
                        ini.WriteFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Precision", double.Parse(tb_MeasurePrecisionPos.Text));
                        ini.WriteFloat("Measure", "Group" + cb_MeasureGroup.Text + "_Sparkless", double.Parse(tb_MeasureSparkless.Text));
                        ini.WriteBool("Measure", "Group" + cb_MeasureGroup.Text + "_SaveFinish", true);
                    }));

                    if (!bFail)
                    {
                        byte E2403 = 0;
                        this.Invoke(new Action(() =>
                        {
                            focas.PMC_ReadByte(PmcAddrType.E, 2403, out E2403);
                            focas.PMC_WriteByte(PmcAddrType.E, 2403, E2403.SetBit(0, true));
                        }));
                        Thread.Sleep(1000);
                        this.Invoke(new Action(() =>
                        {
                            focas.PMC_WriteByte(PmcAddrType.E, 2403, E2403.SetBit(0, false));
                        }));


                    }

                    //TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");





                    //ini.WriteBool("Measure", "Group" + cb_MeasureGroup.Text + "_ZeroFinish", true);
                    bSaveMeasureData = false;
                }).Start();
            }
            else
            {
                bSaveMeasureData = false;
            }
        }

        //private bool bMeasureElecZero = false;

        private void btn_MeasureElecZero_Click(object sender, EventArgs e)
        {
            //la_MeasureMsg.Visible = false;

            //focas.PMC_ReadByte(PmcAddrType.Y, 2, out byte Y2);
            //focas.PMC_ReadByte(PmcAddrType.Y, 3, out byte Y3);

            ////使用組別
            //int.TryParse(cb_MeasureGroup.Text, out int iGroup);

            //if (iGroup == 1)
            //{
            //    if (!Y2.BIT_3())
            //    {
            //        la_MeasureMsg.BackColor = Color.Red;
            //        la_MeasureMsg.ForeColor = Color.Yellow;
            //        la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 72, "錯誤，請先量測伸出");
            //        la_MeasureMsg.Visible = true;
            //        //bMeasureElecZero = false;
            //        return;
            //    }
            //}
            //if (iGroup == 2)
            //{
            //    if (!Y3.BIT_4())
            //    {
            //        la_MeasureMsg.BackColor = Color.Red;
            //        la_MeasureMsg.ForeColor = Color.Yellow;
            //        la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 72, "錯誤，請先量測伸出");
            //        la_MeasureMsg.Visible = true;
            //        //bMeasureElecZero = false;
            //        return;
            //    }
            //}

            ////使用組別
            //byte Group = 0;
            //if (iGroup == 1) Group = 0;
            //else if (iGroup == 2) Group = 0x10;

            ////E2403.0 = 1 強制讓量測主機為AUTO模式(PLC處理)
            //focas.PMC_ReadByte(PmcAddrType.E, 2403, out byte E2403);
            //focas.PMC_WriteByte(PmcAddrType.E, 2403, E2403.SetBit(0, true));

            //Thread.Sleep(200);

            //focas.PMC_ReadByte(PmcAddrType.R, 2151, out byte R2151);
            ////開啟歸零模式
            //if (!R2151.BIT_4())
            //{
            //    focas.PMC_WriteByte(PmcAddrType.R, 2151, R2151.SetBit(4, true));
            //}

            //focas.PMC_ReadByte(PmcAddrType.R, 2151, out R2151);
            ////判斷有無在自動模式
            //if (!R2151.BIT_0())
            //{
            //    la_MeasureMsg.BackColor = Color.Red;
            //    la_MeasureMsg.ForeColor = Color.Yellow;
            //    la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 73, "錯誤，未在自動模式");
            //    la_MeasureMsg.Visible = true;
            //    //bMeasureElecZero = false;
            //    return;
            //}

            ////清除
            //ProfibusCmd_Clear();

            //byte[] cmd1 = { 0, 0, 0, Group, 0, 58 }; //Set Up session Enter
            //byte[] cmd2 = { 0, 0, 0, 0, 0, 61 }; //Set Up session Leave
            //byte[] cmd3 = { 0, 0, 0, Group, 0, 58 }; //Set Up session Enter
            //byte[] cmd4 = { 0, 0, 0, 0, 255, 59 };//Set Retraction Command During Setup session
            //byte[] cmd5 = { 0, 0, 0, 0, 0, 60 };//Get Set UP session state
            //byte[] cmd6 = { 0, 0, 0, 0, 255, 68 };//Set Electrical Zeroing procedure Stop
            //byte[] cmd7 = { 0, 0, 0, 0, 0, 60 };//Get Set UP session state
            //byte[] cmd8 = { 0, 0, 0, 0, 0, 69 };//Set Electrical Zeroing procedure Stop
            //byte[] cmd9 = { 0, 0, 0, 0, 0, 61 };//Set Up session Leave
            //object[] cmds = { cmd1, cmd2, cmd3, cmd4, cmd5, cmd6, cmd7, cmd8, cmd9 };

            //byte[] gp1_cmd1 = { 0, 0, 0, 0, 0, 13 }; //Set Measure System
            //byte[] gp1_cmd2 = { 0, 0, 0, 0, 0, 14 }; //Get Information
            //byte[] gp1_cmd3 = { 0, 0, 0, 0, 0, 1 };//Set Writer Program Data Session Enabled
            //byte[] gp1_cmd4 = { 0, 0, 0, 0, 0, 163 };//Set Zero Adjust
            //byte[] gp1_cmd5 = { 0, 0, 0, 0, 0, 10 };//Set Check program data
            //byte[] gp1_cmd6 = { 0, 0, 0, 1, 0, 3 };//Set Close Programmed Data session Leave
            //object[] gp1_cmds = { gp1_cmd1, gp1_cmd2, gp1_cmd3, gp1_cmd4, gp1_cmd5, gp1_cmd6 };

            //byte[] gp2_cmd1 = { 0, 0, 0, 0, 0, 13 }; //Set Measure System
            //byte[] gp2_cmd2 = { 0, 0, 0, 0x10, 0, 14 }; //Get Information
            //byte[] gp2_cmd3 = { 0, 0, 0, 0x10, 0, 1 };//Set Writer Program Data Session Enabled
            //byte[] gp2_cmd4 = { 0, 0, 0, 0, 0, 163 };//Set Zero Adjust
            //byte[] gp2_cmd5 = { 0, 0, 0, 0, 0, 10 };//Set Check program data
            //byte[] gp2_cmd6 = { 0, 0, 0, 1, 0, 3 };//Set Close Programmed Data session Leave
            //object[] gp2_cmds = { gp2_cmd1, gp2_cmd2, gp2_cmd3, gp2_cmd4, gp2_cmd5, gp2_cmd6 };

            //byte[] response = { 0, 0, 0, 0, 0, 0 };

            ////顯示錯誤訊息
            //la_MeasureMsg.BackColor = Color.Yellow;
            //la_MeasureMsg.ForeColor = Color.Black;
            //la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 74, "歸零中...");
            //la_MeasureMsg.Visible = true;

            //new Thread(() =>
            //{
            //    bool bFail = false;
            //    for (int j = 0; j < cmds.Length; j++)
            //    {

            //        byte[] cmd = cmds[j] as byte[];
            //        byte cmd_code = cmd[5];
            //        //傳送指令(R2164 Code 最後送，其他資料先送)
            //        this.Invoke(new Action(() =>
            //        {
            //            tb_MeasureStep.AppendText("傳送指令(" + (j + 1) + ")\r\n");
            //            for (int i = 0; i < 6; i++) focas.PMC_WriteByte(PmcAddrType.R, (short)(2169 - i), cmd[i]);
            //        }));

            //        if (j < 3)
            //        {
            //            tb_MeasureStep.AppendText("延遲1000 ms (" + (j + 1) + ")\r\n");
            //            //延遲
            //            Thread.Sleep(1000);
            //        }
            //        else
            //        {
            //            tb_MeasureStep.AppendText("延遲1000 ms (" + (j + 1) + ")\r\n");
            //            //延遲
            //            Thread.Sleep(1000);
            //        }

            //        //接收回碼 R2114~R2119
            //        this.Invoke(new Action(() =>
            //        {
            //            tb_MeasureStep.AppendText("接收(" + (j + 1) + ")\r\n");
            //            for (int i = 0; i < 6; i++) focas.PMC_ReadByte(PmcAddrType.R, (ushort)(2114 + i), out response[i]);
            //        }));

            //        //R2114=59||68 && R2115=255
            //        if (((cmd_code == 59) || (cmd_code == 68)) && ((response[0] != cmd_code) || (response[1] != 255)))
            //        {
            //            this.Invoke(new Action(() =>
            //            {
            //                tb_MeasureStep.AppendText(response[1] + "錯誤(" + (j + 1) + ")\r\n");
            //            }));
            //            bFail = true;
            //        }
            //        else if (response[0] != cmd_code)
            //        {
            //            this.Invoke(new Action(() =>
            //            {
            //                tb_MeasureStep.AppendText(response[1] + "錯誤(" + (j + 1) + ")\r\n");
            //            }));
            //            bFail = true;
            //        }
            //        //清除
            //        this.Invoke(new Action(() =>
            //        {
            //            ProfibusCmd_Clear();
            //        }));

            //        Thread.Sleep(100);
            //    }

            //    this.Invoke(new Action(() =>
            //    {
            //        if (bFail)
            //        {
            //            la_MeasureMsg.BackColor = Color.Red;
            //            la_MeasureMsg.ForeColor = Color.Yellow;
            //            la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 75, "歸零失敗");
            //        }
            //        else
            //        {
            //            la_MeasureMsg.BackColor = Color.Blue;
            //            la_MeasureMsg.ForeColor = Color.Yellow;
            //            la_MeasureMsg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 76, "歸零完成");
            //        }
            //        la_MeasureMsg.Visible = true;
            //    }));

            //    if (iGroup == 1)
            //    {
            //        cmds = gp1_cmds;
            //        TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //        ini.WriteFloat("Measure", "Group1_Offset", 0);
            //        dMeasureGroup1Offset = 0;
            //    }
            //    else if (iGroup == 2)
            //    {
            //        cmds = gp2_cmds;
            //        TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //        ini.WriteFloat("Measure", "Group2_Offset", 0);
            //        dMeasureGroup2Offset = 0;
            //    }
            //    //清除歸零補正值
            //    for (int j = 0; j < cmds.Length; j++)
            //    {
            //        byte[] cmd = cmds[j] as byte[];
            //        byte cmd_code = cmd[5];
            //        //傳送指令(R2164 Code 最後送，其他資料先送)
            //        this.Invoke(new Action(() =>
            //        {
            //            tb_MeasureStep.AppendText("傳送指令(" + (j + 1) + ")\r\n");
            //            for (int i = 0; i < 6; i++) focas.PMC_WriteByte(PmcAddrType.R, (short)(2169 - i), cmd[i]);
            //        }));


            //        tb_MeasureStep.AppendText("延遲1000 ms (" + (j + 1) + ")\r\n");
            //        //延遲
            //        Thread.Sleep(1000);


            //        //接收回碼 R2114~R2119
            //        this.Invoke(new Action(() =>
            //        {
            //            tb_MeasureStep.AppendText("接收(" + (j + 1) + ")\r\n");
            //            for (int i = 0; i < 6; i++) focas.PMC_ReadByte(PmcAddrType.R, (ushort)(2114 + i), out response[i]);
            //        }));

            //        if (response[0] != cmd_code)
            //        {
            //            this.Invoke(new Action(() =>
            //            {
            //                tb_MeasureStep.AppendText(response[1] + "錯誤(" + (j + 1) + ")\r\n");
            //            }));
            //            bFail = true;
            //        }
            //        //清除
            //        this.Invoke(new Action(() =>
            //        {
            //            ProfibusCmd_Clear();
            //        }));

            //        Thread.Sleep(100);
            //    }

            //    this.Invoke(new Action(() =>
            //    {
            //        //自動模式
            //        focas.PMC_WriteByte(PmcAddrType.E, 2403, E2403.SetBit(0, false));

            //        //
            //        focas.PMC_ReadByte(PmcAddrType.R, 2151, out R2151);
            //        focas.PMC_WriteByte(PmcAddrType.R, 2151, R2151.SetBit(4, false));
            //    }));

            //    //bMeasureElecZero = false;
            //}).Start();


        }

        private void tb_MeasureRange_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            double.TryParse(tb.Text, out double val);
            tb.Text = val.ToString(Units.DisplayFmt);
            //TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //if (ini.ReadBool("Measure", "Group" + cb_MeasureGroup.Text + "_CreateFinish", false))
            //{

            //}
        }


        private void pic_OD_PathSelect(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            //選擇bit0:左側/bit1:外徑/bit2:右側
            int tag = int.Parse(pic.Tag.ToString());

            //選擇的砂輪
            //int gw_no = CurrentEditGwNo;
            //if (gw_no < 1 || gw_no > 3) gw_no = 1;

            //Dictionary<int, double> CurrentGw;
            //if (gw_no == 2) CurrentGw = CurrentGw2;
            //else CurrentGw = CurrentGw1;

            //用計算的方式設定Switch
            //if ((tmpSelectPath & tag) != 0)
            //{
            //    tmpSelectPath &= (~tag);
            //}
            //else
            //{
            //    tmpSelectPath |= tag;
            //}

            //ShowShapePath();
        }

        //private void ShowShapePath()
        //{
        //    pic_OD_Path_Left.Image = (tmpSelectPath & 1) != 0 ? Properties.Resources.SwitchOn : Properties.Resources.SwitchOff;
        //    pic_OD_Path_Diam.Image = (tmpSelectPath & 2) != 0 ? Properties.Resources.SwitchOn : Properties.Resources.SwitchOff;
        //    pic_OD_Path_Right.Image = (tmpSelectPath & 4) != 0 ? Properties.Resources.SwitchOn : Properties.Resources.SwitchOff;

        //    //修整量
        //    GB_Left.Enabled = (tmpSelectPath & 1) != 0;
        //    GB_Right.Enabled = (tmpSelectPath & 4) != 0;
        //}


        private void btn_OD_Path_DeleteAll_Click(object sender, EventArgs e)
        {
            //double defSpeed = 0;
            //if (btn_OD_Path_Left.Lamp) defSpeed = double.Parse(TB_L_DS.Text);
            //else if (btn_OD_Path_Diam.Lamp) defSpeed = double.Parse(TB_D_DS.Text);
            //else if (btn_OD_Path_Right.Lamp) defSpeed = double.Parse(TB_R_DS.Text);
            //dgv_OD_Path.Rows.Clear();
            //dgv_OD_Path.Rows.Add("1",
            //                        LanguageManager.LoadMessage(Units.langfile, "Message", 80, "起點"),
            //                        "0",
            //                        "0",
            //                        "",
            //                        defSpeed.ToString(Units.DisplayFmt),
            //                        "",
            //                        "");
            //Current_OD_Path.Clear();

            ////種類,X,Z,R,F,OfsX,OfsZ
            //Current_OD_Path.Add("0,0,0,," + defSpeed.ToString(Units.DisplayFmt) + ",,");
        }

        private void btn_PathPreview_Click(object sender, EventArgs e)
        {
            btn_PathPreview.Lamp = true;
            btn_EditLeftPath.Lamp = false;
            btn_EditDiamPath.Lamp = false;
            btn_EditRightPath.Lamp = false;

            DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_PathPreview, true, -1);
            DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_PathPreview, false, -1);
            DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_PathPreview, false, -1);

            //切到預覽
            TC_Path.SelectedTab = tab_PreviewPath;
        }

        private void btn_EditLeftPath_Click(object sender, EventArgs e)
        {
            btn_PathPreview.Lamp = false;
            btn_EditLeftPath.Lamp = true;
            btn_EditDiamPath.Lamp = false;
            btn_EditRightPath.Lamp = false;

            SetPathLayout();


            cb_ToolRCompFunc.SelectedIndex = (int)Math.Round(CurrentGwMacro[10058]);
            tb_ToolR.Text = CurrentGwMacro[10059].ToString(Units.DisplayFmt);


            dgv_Path.Rows.Clear();
            un_PathNum.la_Num.Text = "";
            un_PathNum.la_Msg.Text = "";

            int no = 1;
            foreach (DGWData data in dgwFile.LeftList)
            {
                int index = dgv_Path.Rows.Add(no,
                                    data.Type,
                                    data.X.ToString(Units.DisplayFmt),
                                    data.Z.ToString(Units.DisplayFmt),
                                    (data.Type != 2 && data.Type != 3) ? "" : data.R.ToString(Units.DisplayFmt),
                                    (data.Type == 0) ? "RAPID" : data.Feed.ToString(Units.DisplayFmt),
                                    data.OffsetX.ToString(Units.DisplayFmt),
                                    data.OffsetZ.ToString(Units.DisplayFmt),
                                    LanguageManager.LoadMessage(Units.langfile, "Message", 156, "歸零"));
                dgv_Path.Rows[index].Height = 48;
                no++;
            }
            DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_EditPath, true, 0);

            //切到編輯頁面(左側/右側/外徑共用)
            TC_Path.SelectedTab = tab_EditPath;
            vsb_Path.Maximum = Math.Max(0, dgv_Path.Rows.Count - 1);
            vsb_Path.LargeChange = dgv_Path.DisplayedRowCount(false);
        }

        private void btn_EditDiamPath_Click(object sender, EventArgs e)
        {
            btn_PathPreview.Lamp = false;
            btn_EditLeftPath.Lamp = false;
            btn_EditDiamPath.Lamp = true;
            btn_EditRightPath.Lamp = false;

            SetPathLayout();


            cb_ToolRCompFunc.SelectedIndex = (int)Math.Round(CurrentGwMacro[10056]);
            tb_ToolR.Text = CurrentGwMacro[10057].ToString(Units.DisplayFmt);
            la_DiamOfsZ.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 158, "Z軸補正");


            dgv_Path.Rows.Clear();
            un_PathNum.la_Num.Text = "";
            un_PathNum.la_Msg.Text = "";

            int no = 1;
            foreach (DGWData data in dgwFile.DiamList)
            {
                int index = dgv_Path.Rows.Add(no,
                                    data.Type,
                                    data.X.ToString(Units.DisplayFmt),
                                    data.Z.ToString(Units.DisplayFmt),
                                    (data.Type != 2 && data.Type != 3) ? "" : data.R.ToString(Units.DisplayFmt),
                                    (data.Type == 0) ? "RAPID" : data.Feed.ToString(Units.DisplayFmt),
                                    data.OffsetX.ToString(Units.DisplayFmt),
                                    data.OffsetZ.ToString(Units.DisplayFmt),
                                    LanguageManager.LoadMessage(Units.langfile, "Message", 156, "歸零"));
                dgv_Path.Rows[index].Height = 48;
                no++;
            }

            DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_EditPath, true, 0);

            //切到編輯頁面(左側/右側/外徑共用)
            TC_Path.SelectedTab = tab_EditPath;
            vsb_Path.Maximum = Math.Max(0, dgv_Path.Rows.Count - 1);
            vsb_Path.LargeChange = dgv_Path.DisplayedRowCount(false);
        }

        private void btn_EditRightPath_Click(object sender, EventArgs e)
        {
            btn_PathPreview.Lamp = false;
            btn_EditLeftPath.Lamp = false;
            btn_EditDiamPath.Lamp = false;
            btn_EditRightPath.Lamp = true;

            SetPathLayout();

            dgv_Path.Rows.Clear();
            un_PathNum.la_Num.Text = "";
            un_PathNum.la_Msg.Text = "";

            int no = 1;
            foreach (DGWData data in dgwFile.RightList)
            {
                int index = dgv_Path.Rows.Add(no,
                                    data.Type,
                                    data.X.ToString(Units.DisplayFmt),
                                    data.Z.ToString(Units.DisplayFmt),
                                    (data.Type != 2 && data.Type != 3) ? "" : data.R.ToString(Units.DisplayFmt),
                                    (data.Type == 0) ? "RAPID" : data.Feed.ToString(Units.DisplayFmt),
                                    data.OffsetX.ToString(Units.DisplayFmt),
                                    data.OffsetZ.ToString(Units.DisplayFmt),
                                    LanguageManager.LoadMessage(Units.langfile, "Message", 156, "歸零"));
                dgv_Path.Rows[index].Height = 48;
                no++;
            }

            DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_EditPath, true, 0);

            //切到編輯頁面(左側/右側/外徑共用)
            TC_Path.SelectedTab = tab_EditPath;
            vsb_Path.Maximum = Math.Max(0, dgv_Path.Rows.Count - 1);
            vsb_Path.LargeChange = dgv_Path.DisplayedRowCount(false);
        }

        private void DrawPath(List<DGWData> list, Color color, PathOrigin origin, PictureBox pic, bool bNew, int index)
        {
            //focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);

            //la_PathError.Visible = false;
            //la_PathError.Text = "";

            int pic_width = pic.Width;
            int pic_height = pic.Height;


            //砂輪照比例縮圖
            //假設砂輪寬60 mm ， 左右各加5 mm 的過切，再5 mm 的留白
            //起點超過範圍就不管了
            //等於80 mm : 350 pixel，倍率 = 80/350 = 0.2285 mm/px，
            //繪圖左上角為座標(0,0)，往右是正，往下是正
            //砂輪1的左下角是座標的(0,0)，往右是負，往下是負

            //砂輪寬度
            double GW_Width = double.Parse(la_OD_Path_Width_Val.Text);
            if (bInchTrans) GW_Width *= 25.4;

            //倍率
            double rate = (GW_Width + 20.0) / pic_width;

            //建立一張新的圖
            Bitmap bmp;

            if (bNew || pic.Image == null)
            {
                bmp = new Bitmap(pic_width, pic_height);
            }
            else
            {
                bmp = (Bitmap)pic.Image;
            }

            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //邊緣10 mm轉像素
            double edge = 10.0 / rate;//pixel


            //背景色
            if (bNew)
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, pic_width, pic_height));


                if (CurrentGwMacro[10004] == 1)//GW2 外圓
                {
                    //外圓砂輪
                    int sx = (int)Math.Round(edge);
                    int sy = 0;
                    int pw = (int)Math.Round(GW_Width / rate);
                    int ph = pic_height - (int)Math.Round(edge);
                    g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(sx, sy, pw, ph));
                }
                else
                {
                    //內圓砂輪
                    int sx = (int)Math.Round(edge);
                    int sy = (int)Math.Round(edge);
                    int pw = (int)Math.Round(GW_Width / rate);
                    //int ph = pic_height - (int)Math.Round(edge);
                    int ph = (int)Math.Round(GW_Width / rate);
                    g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(sx, sy, pw, ph));
                }
            }

            double tmp_x = 0;
            double tmp_y = 0;

            double last_x = 0;
            double last_y = 0;

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //先回復成白色, 後面R有問題再改紅色
                    if (pic == pic_EditPath)
                    {
                        dgv_Path.Rows[i].Cells[Col_Path_R.Index].Style.BackColor = Color.White;
                    }

                    //string line = list[i];
                    DGWData data = list[i];
                    //String[] data = line.Split(',');//路徑種類, X, Z, R, F, OffsetX, OffsetZ, Dir
                    //if (data.Length >= 7)
                    //{

                    double y = data.X;//機械X軸 = 畫面y軸
                    double x = data.Z;//機械Z軸 = 畫面x軸
                    if (bInchTrans)
                    {
                        x *= 25.4;
                        y *= 25.4;
                    }

                    double ox;//原點
                    double oy;//原點
                    if (CurrentGwMacro[10004] == 1)//外圓修整方向不同
                    {
                        if (origin == PathOrigin.Right)//右原點
                        {
                            ox = pic_width - edge;
                            oy = pic_height - edge;
                        }
                        else //左原點
                        {
                            ox = edge;
                            oy = pic_height - edge;
                        }
                    }
                    else
                    {
                        if (origin == PathOrigin.Right)//右原點
                        {
                            ox = pic_width - edge;
                            oy = edge;
                        }
                        else //左原點
                        {
                            ox = edge;
                            oy = edge;
                        }
                    }


                    //換算成pixel
                    double px_x;
                    double px_y;

                    //if (CurrentGwMacro[10004] == 1)//外圓
                    //{
                    px_x = x / rate * (-1) + ox;
                    px_y = y / rate + oy;
                    //}
                    //else//內圓
                    //{
                    //    px_x = x / rate * (-1) + ox;
                    //    px_y = y / rate + oy;
                    //}


                    if (i == 0)
                    {
                        //起點畫一個小圓
                        g.FillEllipse(new SolidBrush(color), (int)(px_x - 5), (int)(px_y - 5), 11, 11);
                    }
                    else if (i > 0)
                    {
                        //直線
                        if (data.Type != 2 && data.Type != 3)
                        {
                            if (btn_OffsetPath.Lamp && i == index) //補正 - 凸顯補正受影響的線段
                            {
                                g.DrawLine(new Pen(color, 3F), (float)tmp_x, (float)tmp_y, (float)px_x, (float)px_y);
                            }
                            else //編輯 or 預覽
                            {
                                g.DrawLine(new Pen(color, 1F), (float)tmp_x, (float)tmp_y, (float)px_x, (float)px_y);
                            }
                        }
                        //R角 (CW or CCW)
                        else if (data.Type == 2 || data.Type == 3)
                        {
                            double r = data.R;
                            if (bInchTrans) r *= 25.4;
                            double px_r = r / rate;

                            //上一筆資料
                            DblPoint dA = new DblPoint(tmp_x, tmp_y);

                            //這一筆資料
                            DblPoint dB = new DblPoint(px_x, px_y);

                            //中心點(非圓的中心點)
                            double cx = (dA.X + dB.X) / 2.0;
                            double cy = (dA.Y + dB.Y) / 2.0;

                            //AB 向量長度
                            DblPoint dV = new DblPoint(dB.X - dA.X, dB.Y - dA.Y);

                            //AB 直線長度
                            double l = Math.Sqrt(Math.Pow(Math.Round(dV.X, 2), 2) + Math.Pow(Math.Round(dV.Y, 2), 2));
                            double d = Math.Round(px_r * 2, 2);


                            double e = l / 2.0;
                            //等腰三角形高
                            double h = Math.Sqrt(px_r * px_r - e * e);

                            //垂直偏移向量
                            double tdx = -h * dV.Y / l;
                            double tdy = h * dV.X / l;

                            DblPoint[] CenterPt = null;

                            if (l == d)
                            {
                                CenterPt = new DblPoint[] { new DblPoint(cx, cy) };
                            }
                            if (l < d)
                            {
                                CenterPt = new DblPoint[] { new DblPoint(cx - tdx, cy - tdy), new DblPoint(cx + tdx, cy + tdy) };
                            }
                            if (l == 0)
                            {
                                Pen err_pen = new Pen(Color.Red, 3F);
                                err_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                                g.DrawLine(err_pen, (float)tmp_x, (float)tmp_y, (float)px_x, (float)px_y);
                                if (i == index)
                                {
                                    last_x = px_x;
                                    last_y = px_y;
                                }
                                tmp_x = px_x;
                                tmp_y = px_y;
                                if (pic == pic_EditPath)
                                {
                                    //string msg = LanguageManager.LoadMessage(Units.langfile, "Message", 94, "R角路徑錯誤，參考點位置與前一筆相同。");
                                    //if (la_PathError.Text == "")
                                    //{
                                    //la_PathError.Text = msg;
                                    //}
                                    //else
                                    //{
                                    //    la_PathError.Text = la_PathError.Text + "\r\n" + msg;
                                    //    Size textSize = TextRenderer.MeasureText(msg, la_PathError.Font);
                                    //    la_PathError.Height = textSize.Height;
                                    //}
                                    //la_PathError.Visible = true;
                                    dgv_Path.Rows[i].Cells[Col_Path_R.Index].Style.BackColor = Color.Red;
                                }
                                //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 94, "R角路徑錯誤，參考點位置與前一筆相同。"));
                                //return;//無限多解
                                continue;
                            }
                            if (l > d)
                            {
                                Pen err_pen = new Pen(Color.Red, 3F);
                                err_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                                g.DrawLine(err_pen, (float)tmp_x, (float)tmp_y, (float)px_x, (float)px_y);
                                if (i == index)
                                {
                                    last_x = px_x;
                                    last_y = px_y;
                                }
                                tmp_x = px_x;
                                tmp_y = px_y;
                                if (pic == pic_EditPath)
                                {
                                    //string msg = LanguageManager.LoadMessage(Units.langfile, "Message", 95, "R角路徑錯誤，R角過小。");
                                    //if (la_PathError.Text == "")
                                    //{
                                    //    la_PathError.Text = msg;
                                    //}
                                    //else
                                    //{
                                    //    la_PathError.Text = la_PathError.Text + "\r\n" + msg;
                                    //    Size textSize = TextRenderer.MeasureText(msg, la_PathError.Font);
                                    //    la_PathError.Height = textSize.Height;
                                    //}
                                    //la_PathError.Visible = true;
                                    dgv_Path.Rows[i].Cells[Col_Path_R.Index].Style.BackColor = Color.Red;
                                }
                                //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 95, "R角路徑錯誤，R角過小。"));
                                //return;//無解
                                continue;
                            }

                            DblPoint CurrentPt = null;
                            double StartAngle = 0;
                            double Angle;
                            //方向
                            int c = 2;
                            if (data.Type == 2) c = 1;
                            if (c == 1) Angle = 9999; //順時針
                            else Angle = -9999; //逆時針
                            foreach (DblPoint pt in CenterPt)
                            {

                                //取得圓的左上角
                                double ax1 = (cx - tdx - px_r);
                                double ay1 = (cy - tdy - px_r);
                                //取得圓的右下角
                                double ax2 = (cx - tdx + px_r);
                                double ay2 = (cy - tdy + px_r);

                                //DrawArc 的零點位置
                                DblPoint dZ = new DblPoint(ax2, pt.Y);

                                //零點到A點的角度
                                double a_A = Math.Atan((dA.Y - pt.Y) / (dA.X - pt.X)) * (180 / Math.PI);
                                if (dA.X < pt.X) a_A += 180;
                                if (a_A < 0) a_A += 360;

                                //零點到B點的角度
                                double a_B = Math.Atan((dB.Y - pt.Y) / (dB.X - pt.X)) * (180 / Math.PI);
                                if (dB.X < pt.X) a_B += 180;
                                if (a_B < 0) a_B += 360;

                                //A點到B點角度
                                double dAB = a_B - a_A;
                                if (dAB < 0) dAB += 360;

                                //順時針
                                if (c == 1)
                                {
                                    if (dAB < Angle)
                                    {
                                        StartAngle = a_A;
                                        Angle = dAB;
                                        CurrentPt = pt;//選擇路徑比較短的圓心
                                    }
                                }
                                //逆時針
                                if (c == 2)
                                {
                                    dAB -= 360;
                                    if (dAB > Angle)
                                    {
                                        StartAngle = a_A;
                                        Angle = dAB;
                                        CurrentPt = pt;//選擇路徑比較短的圓心   
                                    }
                                }

                            }
                            //FPen.Width = 3;
                            //FPen.Color = color;
                            if (btn_OffsetPath.Lamp && i == index) //補正 - 凸顯補正受影響的線段
                            {
                                g.DrawArc(new Pen(color, 3F), (float)(CurrentPt.X - px_r), (float)(CurrentPt.Y - px_r), (float)(2 * px_r) - 1, (float)(2 * px_r) - 1, (float)StartAngle, (float)Angle);
                            }
                            else //編輯 or 預覽
                            {
                                try
                                {
                                    float fx = (float)(CurrentPt.X - px_r);
                                    float fy = (float)(CurrentPt.Y - px_r);
                                    float fw = (float)(2 * px_r) - 1;
                                    float fh = (float)(2 * px_r) - 1;
                                    if (fw < 1) fw = 1;
                                    if (fh < 1) fh = 1;

                                    g.DrawArc(new Pen(color, 1F), fx, fy, fw, fh, (float)StartAngle, (float)Angle);

                                }
                                catch (Exception ex)
                                {
                                    Fo_Msg.Show(ex.Message);
                                }
                            }
                        }
                    }

                    if (i == index)
                    {
                        last_x = px_x;
                        last_y = px_y;
                    }

                    tmp_x = px_x;
                    tmp_y = px_y;
                    //}
                }
            }
            if (index >= 0)
            {
                g.DrawLine(new Pen(Color.Red, 3F), (float)(last_x - 7), (float)(last_y), (float)(last_x + 7), (float)(last_y));
                g.DrawLine(new Pen(Color.Red, 3F), (float)(last_x), (float)(last_y - 7), (float)(last_x), (float)(last_y + 7));
            }

            pic.Image = bmp;
        }

        //private void LoadODPathToDGV(List<String> list)
        //{
        //    Current_OD_Path = list;
        //    dgv_OD_Path.Rows.Clear();

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        string line = list[i];
        //        String[] data = line.Split(',');//路徑種類, X, Z, R, F, OffsetX, OffsetZ, Dir
        //        if (data.Length >= 7)
        //        {
        //            //這邊不分左側/外徑/右側
        //            int type = int.Parse(data[0]);
        //            if (type == 0) type = 80;
        //            if (type == 1) type = 81;
        //            if (type == 2) type = 111;
        //            if (type == 3) type = 112;

        //            dgv_OD_Path.Rows.Add("", LanguageManager.LoadMessage(Units.langfile, "Message", type, ""), data[1], data[2], data[3], data[4], data[5], data[6]);//路徑種類, X, Z, R, F, OffsetX, OffsetZ
        //        }
        //    }

        //    if (list.Count == 0)
        //    {
        //        dgv_OD_Path.Rows.Add("", "0", "0", "", tmp_OD_Path_DefDressRpm, "0", "0", "");//路徑種類, X, Z, R, F, OffsetX, OffsetZ, Dir
        //        list.Add("0,0,0,," + tmp_OD_Path_DefDressRpm + ",0,0,");
        //    }
        //}

        private void tb_OD_Path_ToolDiam_TextChanged(object sender, EventArgs e)
        {
            //tmp_OD_Path_ToolDiam = double.Parse(tb_OD_Path_ToolDiam.Text);
        }

        private void tb_OD_Path_Z_Offset_TextChanged(object sender, EventArgs e)
        {
            //tmp_OD_Path_OffsetZ = double.Parse(tb_OD_Path_Z_Offset.Text);
        }

        private string CompilerPath(int no)
        {
            //string fmt = Units.DisplayFmt;

            string code = "%\r\nO" + no + "\r\nGOTO#110\r\nN1\r\n";


            for (int i = 0; i < this.dgwFile.LeftList.Count; i++)
            {
                //路徑種類,X,Z,R,F,補正X,補正Z,方向
                DGWData data = dgwFile.LeftList[i];
                string x = (data.X * 2.0 + data.OffsetX).ToString(Units.DisplayFmt);
                string z = (data.Z + data.OffsetZ).ToString(Units.DisplayFmt);
                string f = (data.Feed).ToString(Units.DisplayFmt);
                string r = (data.R).ToString(Units.DisplayFmt);




                if (data.Type == 0)
                {
                    code += "G00X" + x + "Z" + z + "\r\n";
                }
                else if (data.Type == 1)
                {
                    code += "G01X" + x + "Z" + z + "F" + f + "\r\n";
                }
                else if (data.Type == 2)//CW 順時針R角
                {
                    code += "G02X" + x + "Z" + z + "R" + r + "F" + f + "\r\n";
                }
                else if (data.Type == 3)//CCW 逆時針R角
                {
                    code += "G03X" + x + "Z" + z + "R" + r + "F" + f + "\r\n";
                }
            }

            code += "M99\r\nN2\r\n";
            for (int i = 0; i < dgwFile.DiamList.Count; i++)
            {
                //路徑種類,X,Z,R,F,補正X,補正Z,方向
                DGWData data = dgwFile.DiamList[i];
                string x = (data.X * 2.0 + data.OffsetX).ToString(Units.DisplayFmt);
                string z = (data.Z + data.OffsetZ).ToString(Units.DisplayFmt);
                string f = (data.Feed).ToString(Units.DisplayFmt);
                string r = (data.R).ToString(Units.DisplayFmt);

                if (data.Type == 0)
                {
                    code += "G00X" + x + "Z" + z + "\r\n";
                }
                else if (data.Type == 1)
                {
                    code += "G01X" + x + "Z" + z + "F" + f + "\r\n";
                }
                else if (data.Type == 2)//CW 順時針R角
                {
                    code += "G02X" + x + "Z" + z + "R" + r + "F" + f + "\r\n";
                }
                else if (data.Type == 3)//CCW 逆時針R角
                {
                    code += "G03X" + x + "Z" + z + "R" + r + "F" + f + "\r\n";
                }
            }

            code += "M99\r\nN3\r\n";

            for (int i = 0; i < dgwFile.RightList.Count; i++)
            {
                //路徑種類,X,Z,R,F,補正X,補正Z,方向
                DGWData data = dgwFile.RightList[i];
                string x = (data.X * 2.0 + data.OffsetX).ToString(Units.DisplayFmt);
                string z = (data.Z + data.OffsetZ).ToString(Units.DisplayFmt);
                string f = (data.Feed).ToString(Units.DisplayFmt);
                string r = (data.R).ToString(Units.DisplayFmt);

                if (data.Type == 0)
                {
                    code += "G00X" + x + "Z" + z + "\r\n";
                }
                else if (data.Type == 1)
                {
                    code += "G01X" + x + "Z" + z + "F" + f + "\r\n";
                }
                else if (data.Type == 2)//CW 順時針R角
                {
                    code += "G02X" + x + "Z" + z + "R" + r + "F" + f + "\r\n";
                }
                else if (data.Type == 3)//CCW 逆時針R角
                {
                    code += "G03X" + x + "Z" + z + "R" + r + "F" + f + "\r\n";
                }
            }

            code += "M99\r\n%";

            return code;
        }

        private void dgv_Path_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_Path.CurrentCell == null) return;

            int col = dgv_Path.CurrentCell.ColumnIndex;
            if (col < 0) return;

            if (col == Col_Path_No.Index) return;
            if (col == Col_OfsPath_Zero.Index) return;

            int row = dgv_Path.CurrentCell.RowIndex;
            if (row < 0) return;

            tb_SelectTextBox = null;
            tb_DiamOfsZ.BackColor = Color.White;
            tb_DiamOfsZ.ForeColor = Color.Black;
            tb_ToolR.BackColor = Color.White;
            tb_ToolR.ForeColor = Color.Black;

            if (col == Col_Path_Type.Index)
            {
                un_PathNum.la_Msg.Text = "Type\n0:G00 RAPID, 1:G01 CUTTING, 2:G02 CW, 3:G03 CCW";
                un_PathNum.la_Msg.Font = new Font("Times New Roman", 10F);
                un_PathNum.la_Num.Text = dgv_Path.CurrentCell.Value.ToString();
            }
            else
            {
                un_PathNum.la_Msg.Text = dgv_Path.Columns[col].HeaderText + "\r\n\r\n";
                un_PathNum.la_Msg.Font = new Font("Times New Roman", 10F);
                string val = dgv_Path.CurrentCell.Value.ToString();
                if (val == "RAPID") val = Units.DisplayFmt;
                un_PathNum.la_Num.Text = val;
            }

            if (btn_EditLeftPath.Lamp) DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_EditPath, true, row);
            else if (btn_EditDiamPath.Lamp) DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_EditPath, true, row);
            else if (btn_EditRightPath.Lamp) DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_EditPath, true, row);
        }

        private void TB_CoorClick(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            TextBox tb = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                if (tb.Tag != null)
                {
                    int.TryParse(tb.Tag.ToString(), out int no);
                    Units.MacroInfo.GetMinMax(no, out double min, out double max);
                    if ((min != 0 || max != 0)) // 0 ~ 0 就不顯示
                    {
                        form.uc_UserNum1.la_Msg.Text += "\r\n" + min + " ~ " + max;
                    }
                }

            }
            form.SetVal(double.Parse(tb.Text));
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);
                GwWorkPiEdit = true;

            }
        }

        private void tb_MeasureSparkless_TextChanged(object sender, EventArgs e)
        {
            TProcess EditProcess = TempProgram.Processes[ProcessIndex];
            if (EditProcess == null)
                return;
            try
            {
                if (EditProcess.SubPrograms.Count > 2)
                {
                    //P4進給速度
                    TArgument a = EditProcess.SubPrograms[2].GetArgument("19867");
                    if (a != null)
                    {


                        int ProgNo = EditProcess.SubPrograms[2].ProgNo;
                        if (ProgNo == 9010 ||
                            ProgNo == 9012 ||
                            ProgNo == 9013 ||
                            ProgNo == 9015 ||
                            ProgNo == 9016 ||
                            ProgNo == 9017 ||
                            ProgNo == 9020 ||
                            ProgNo == 9022 ||
                            ProgNo == 9023 ||
                            ProgNo == 9024)
                        {
                            a.Value = double.Parse(tb_MeasureSparkless.Text);
                            btn_SaveProg.Visible = true;
                            btn_SaveProgVisible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        private void btn_Path_Import_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\DGW\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML File|*.xml";
            dialog.DefaultExt = "XML File";
            dialog.InitialDirectory = path;
            DialogResult ret = dialog.ShowDialog();
            if (ret != DialogResult.OK) return;

            string filename = Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml";
            File.Copy(dialog.FileName, filename, true);
            dgwFile.LoadFromFile(dialog.FileName);

            int shift = (CurrentEditGwNo - 1) * 100;

            int mode = 0;
            if (dgwFile.LeftList.Count > 0) mode |= 1;//成形模式 - 包含左側修整
            if (dgwFile.DiamList.Count > 0) mode |= 2;//成形模式 - 包含外徑修整
            if (dgwFile.RightList.Count > 0) mode |= 4;//成形模式 - 包含右側修整(預留)
            CurrentGwMacro[10048] = mode;//成形模式

            tb_DiamOfsZ.Text = dgwFile.DGWDiamOffsetZ.ToString(Units.DisplayFmt);
            CurrentGwMacro[10049] = dgwFile.DGWDiamOffsetZ;//成形外徑修整Z軸補正
            CurrentGwMacro[10050] = dgwFile.LeftList.Count > 0 ? dgwFile.LeftList[0].X * 2 : 0; //左側起始位置X
            CurrentGwMacro[10051] = dgwFile.LeftList.Count > 0 ? dgwFile.LeftList[0].Z : 0;//左側起始位置Z
            CurrentGwMacro[10052] = dgwFile.DiamList.Count > 0 ? dgwFile.DiamList[0].X * 2 : 0;//外徑起始位置X
            CurrentGwMacro[10053] = dgwFile.DiamList.Count > 0 ? dgwFile.DiamList[0].Z : 0;//外徑起始位置Z
            CurrentGwMacro[10054] = dgwFile.RightList.Count > 0 ? dgwFile.RightList[0].X * 2 : 0;//右側起始位置X
            CurrentGwMacro[10055] = dgwFile.RightList.Count > 0 ? dgwFile.RightList[0].Z : 0;//右側起始位置Z
            CurrentGwMacro[10056] = dgwFile.Diam_ToolComp;//外徑刀尖補償方式
            CurrentGwMacro[10057] = dgwFile.Diam_ToolR;//外徑刀尖半徑
            CurrentGwMacro[10058] = dgwFile.Left_ToolComp;//左側刀尖補償方式
            CurrentGwMacro[10059] = dgwFile.Left_ToolR;//左側刀尖半徑

            DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_PathPreview, true, -1);
            DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_PathPreview, false, -1);
            DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_PathPreview, false, -1);

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void btn_Path_Export_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\DGW\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML File|*.xml";
            dialog.DefaultExt = "XML File";
            dialog.InitialDirectory = path;
            DialogResult ret = dialog.ShowDialog();
            if (ret != DialogResult.OK) return;

            if (double.TryParse(tb_DiamOfsZ.Text, out double offset_z)) dgwFile.DGWDiamOffsetZ = offset_z;
            else dgwFile.DGWDiamOffsetZ = 0;


            dgwFile.Diam_ToolComp = CurrentGwMacro[10056];
            dgwFile.Diam_ToolR = CurrentGwMacro[10057];
            dgwFile.Left_ToolComp = CurrentGwMacro[10058];
            dgwFile.Left_ToolR = CurrentGwMacro[10059];

            dgwFile.SaveToFile(dialog.FileName);
        }

        private void la_Tip_MouseDown(object sender, MouseEventArgs e)
        {
            string[] lines = la_Tip.Text.Split('\n');
            int len = 0;
            foreach (string s in lines) if (s.Trim() != "") len++;
            pa_Tip.Height = 22 * len;
            pa_Tip.BringToFront();
        }

        private void la_Tip_MouseUp(object sender, MouseEventArgs e)
        {
            pa_Tip.Height = 22;
            pa_Tip.BringToFront();
        }
        private void la_AlarmTip_MouseDown(object sender, MouseEventArgs e)
        {
            string[] lines = la_AlarmTip.Text.Split('\n');
            int len = 0;
            foreach (string s in lines) if (s.Trim() != "") len++;
            pa_AlarmTip.Height = 22 * len;
            pa_AlarmTip.BringToFront();
        }

        private void la_AlarmTip_MouseUp(object sender, MouseEventArgs e)
        {
            pa_AlarmTip.Height = 22;
            pa_AlarmTip.BringToFront();
        }

        private void btn_ID_PosX1_Click(object sender, EventArgs e)
        {
            if (Pos.Machine.Length > 0) TB_Gw1PosX1.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bIDCenter = true;
        }


        private void btn_ID_PosX2_Click(object sender, EventArgs e)
        {
            if (Pos.Machine.Length > 0) TB_Gw1PosX2.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bIDCenter = true;
        }


        private void pic_RunSpindle_Click(object sender, EventArgs e)
        {
            if (fo_Runin == null)
            {
                fo_Runin = new Fo_Runin();
                fo_Runin.TopLevel = false;
                fo_Runin.Parent = tab_Runin;
                fo_Runin.Top = 0;
                fo_Runin.Left = 0;
                fo_Runin.Show();
                fo_Runin.BringToFront();
            }
            else
            {
                fo_Runin.LoadLanguageFile(Units.langfile, fo_Runin.Name);
            }

            TC_Main.SelectedTab = tab_Runin;


        }



        private void pic_UI_Click(object sender, EventArgs e)
        {

            Fo_UI_Setting form = new Fo_UI_Setting();
            form.TopLevel = false;
            form.Parent = this;
            form.Left = 0;
            form.Top = 0;
            form.Show();
            form.BringToFront();
        }



        private void btn_SpRate_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_SpRate.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_SpRate.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + "50" + " ~ " + "120"; //顯示上下限


            }
            //顯示並等待結果
            if (form.ShowDialog() == DialogResult.OK)
            {
                //結果如果有按儲存

                double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);
                if (dVal < 50) dVal = 50;
                if (dVal > 120) dVal = 120;

                int step = ((int)(dVal)) / 10 * 10;
                btn_SpRate.DisplayText = step.ToString("0");

                Actions.Enqueue(new Action(() =>
                {
                    focas.PMC_ReadByte(PmcAddrType.E, 2522, out byte E2522);
                    E2522 &= 0xF8;

                    if (step == 50) E2522 |= 7;
                    else if (step == 60) E2522 |= 6;
                    else if (step == 70) E2522 |= 5;
                    else if (step == 80) E2522 |= 4;
                    else if (step == 90) E2522 |= 3;
                    else if (step == 100) E2522 |= 2;
                    else if (step == 110) E2522 |= 1;
                    else if (step == 120) E2522 |= 0;

                    //E2522.0 ~ E2522.2
                    focas.PMC_WriteByte(PmcAddrType.E, 2522, E2522);

                }));

                //focas.PMC_WriteByte(PmcAddrType.D, 242, (byte)Math.Round(dVal));

            }
        }

        private void pic_Balance_Click(object sender, EventArgs e)
        {
            focas.ReadMacro(506, out double gw_no);
            int no = (int)Math.Round(gw_no);
            if (no == 1)
            {
                if (!ch_UI_BalanceGW1.Checked)
                {
                    Fo_Msg.Show("砂輪1 未啟用動平衡功能");
                    return;
                }
            }
            else if (no == 2)
            {
                if (!ch_UI_BalanceGW2.Checked)
                {
                    Fo_Msg.Show("砂輪2 未啟用動平衡功能");
                    return;
                }
            }
            else
            {
                Fo_Msg.Show("GW Station Error.");
                return;
            }

            masterSerialBus1.QueryList.Clear();



            //Status            
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_Status.ToString("X4") + "0004");
            //Error code
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_Error.ToString("X4") + "0004");
            //Mode
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_ModeStatus.ToString("X4") + "0004");
            //DO Status
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_DOStatus.ToString("X4") + "0004");

            //Balancing Step
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_BalancingStep.ToString("X4") + "0004");
            //Trial Angle
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_TrialAngle.ToString("X4") + "0004");
            //Angle
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_Angle.ToString("X4") + "0004");
            //Vibration(um P-P)
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_Vibration_um.ToString("X4") + "0004");
            //Vibration(G Peak)
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_Vibration_G.ToString("X4") + "0004");
            //RPM
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_RPM.ToString("X4") + "0004");
            //NarrowBandVibration(G)
            masterSerialBus1.AddQuery(BalanceSlave.ToString("X2") + "03" + Units.BA_NarrowBandVibration.ToString("X4") + "0004");

            masterSerialBus1.Add(BalanceSlave.ToString("X2") + "10" + Units.BA_ModeStatus.ToString("X4") + "0001020001");

            Fo_Balance form = new Fo_Balance();
            //form.btn_SoftPanel.Click += new System.EventHandler(this.btn_SoftPanel_Click);


            if (no == 1) form.tb_Balance_Step1_RPM.Text = btn_Gw1CmdRpm.DisplayText;
            if (no == 2) form.tb_Balance_Step1_RPM.Text = btn_Gw2CmdRpm.DisplayText;

            form.TopLevel = false;
            form.Parent = this;
            form.Left = 0;
            form.Top = 80;
            form.Show();
            form.BringToFront();
        }

        private void ch_UI_BalanceGW(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            CheckBox ch = (CheckBox)sender;
            ini.WriteInteger("UI", "BalanceGW" + ch.Tag.ToString(), ch.Checked ? 1 : 0);
        }

        private void btn_RollerCmdSpeed_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_RollerCmdSpeed.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_RollerCmdSpeed.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Roller.MinRpm + " ~ " + Roller.MaxRpm; //顯示上下限


            }
            //顯示並等待結果
            if (form.ShowDialog() == DialogResult.OK)
            {
                //結果如果有按儲存

                double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);
                if (dVal < Roller.MinRpm) dVal = Roller.MinRpm;
                if (dVal > Roller.MaxRpm) dVal = Roller.MaxRpm;

                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                this.Roller.CmdSpeed = dVal / this.Roller.Rate;
                btn_RollerCmdSpeed.DisplayText = dVal.ToString("0");


                //傳送指令到變頻器
                masterSerialBus1.Add(this.Roller.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Roller.CmdSpeed / Roller.Unit)).ToString("X4"));

                //紀錄
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteFloat("Roller", "Cmd", this.Roller.CmdSpeed);
            }
        }

        private void btn_ToProbe_Click(object sender, EventArgs e)
        {
            string msg = LanguageManager.LoadMessage(Units.langfile, "Message", 108, "是否要旋轉到端測位置");
            DialogResult ret = Fo_Msg.Show(msg, LanguageManager.LoadMessage(Units.langfile, "Message", 6, "注意"), MessageBoxButtons.YesNo);

            if (ret == DialogResult.Yes)
            {
                //畫面啟動程式(轉頭)
                if (focas.WriteMacro(980, 11) != Focas1.EW_OK)
                {
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 92, "Write #980 Fail."));
                    return;
                }

                OneKeyCall(8999);

            }
        }

        private void tb_DressGw_TextBoxClick(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            TextBox tb = (TextBox)sender;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                if (tb.Tag != null)
                {
                    int.TryParse(tb.Tag.ToString(), out int no);
                    Units.MacroInfo.GetMinMax(no, out double min, out double max);
                    if ((min != 0 || max != 0)) // 0 ~ 0 就不顯示
                    {
                        form.uc_UserNum1.la_Msg.Text += "\r\n" + min + " ~ " + max;
                    }
                }

            }
            double.TryParse(tb.Text, out double val);
            form.SetVal(val);
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);
                GwDressEdit = true;
            }
        }

        private void LB_GM_Code_TextChanged()
        {
            //目前編輯的工序 
            TProcess process = null;
            if (ProcessIndex >= 0)
            {
                if (ProcessIndex >= TempProgram.Processes.Count) return;
                process = TempProgram.Processes[ProcessIndex];
            }
            if (process == null) return;

            process.GM_Code.Clear();

            foreach (String s in LB_GM_Code.Items)
            {
                String Data = s.Trim();
                //Data = Data.Replace(";", "\r\n");
                if (Data != "")
                {
                    process.GM_Code.Add(Data);

                }
            }
        }

        private void btn_SpSpeed_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_SpSpeed.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_SpSpeed.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Spindle.MinRpm + " ~ " + Spindle.MaxRpm; //顯示上下限


            }
            var ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            int.TryParse(form.uc_UserNum1.la_Num.Text, out int val);
            if (val > Spindle.MaxRpm)
                val = Spindle.MaxRpm;
            if (val < Spindle.MinRpm)
                val = Spindle.MinRpm;

            UserSCode = val;

            //紀錄
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteFloat("Spindle", "Cmd", UserSCode);

            //重新設定轉速(轉速(RPM) = 刻度(RPM) * 倍率(%))
            this.Spindle.CmdSpeed = UserSCode;
            btn_SpSpeed.DisplayText = UserSCode.ToString();


            if (SpindleDev == 0)
            {
                //傳送指令到三菱驅動器
                string data = ((int)Math.Round(this.Spindle.CmdSpeed * this.Spindle.Rate * Spindle.Unit)).ToString("X8");
                masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "102106000204" + data.Substring(4, 4) + data.Substring(0, 4));
            }
            else if (SpindleDev == 1)
            {
                //傳送指令到安川驅動器
                string data = ((int)Math.Round(this.Spindle.CmdSpeed * this.Spindle.Rate * Spindle.Unit)).ToString("X4");
                if (SpindleChIndex == 0)//RS485
                    masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                if (SpindleChIndex == 1)//RS422
                    masterSerialBus2.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
            }
            else
            {
                //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                string data = ((int)Math.Round(this.Spindle.CmdSpeed / this.Spindle.Rate / Spindle.Unit)).ToString("X4");
                //傳送指令到士林變頻器
                masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "061009" + data);
            }


        }

        private void btn_SVO_Click(object sender, EventArgs e)
        {
            if (SP_Comm_Enabled)//判斷有無啟用主軸
            {
                if (SpindleDev == 0)
                {
                    //傳送指令到三菱驅動器
                    string data = ((int)Math.Round(this.Spindle.CmdSpeed * this.Spindle.Rate * Spindle.Unit)).ToString("X8");
                    masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "102106000204" + data.Substring(4, 4) + data.Substring(0, 4));

                }
                else if (SpindleDev == 1)
                {
                    //傳送指令到安川驅動器
                    string data = ((int)Math.Round(this.Spindle.CmdSpeed * this.Spindle.Rate * Spindle.Unit)).ToString("X4");
                    if (SpindleChIndex == 0)//RS485
                        masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                    if (SpindleChIndex == 1)//RS422
                        masterSerialBus2.Add(this.Spindle.Slave.ToString("X2") + "4001100000030100010002" + data);
                }
                else
                {
                    //重新設定頻率(頻率(Hz) = 刻度(RPM) / 倍率(RPM/Hz))
                    string data = ((int)Math.Round(this.Spindle.CmdSpeed / this.Spindle.Rate / Spindle.Unit)).ToString("X4");
                    //傳送指令到士林變頻器
                    if (SP_Comm_Enabled) masterSerialBus1.Add(this.Spindle.Slave.ToString("X2") + "061009" + data);
                }
            }

            if (GW1_Comm_Enabled)//判斷有無啟用砂輪1
            {
                //傳送指令到變頻器
                if (Gw1Dev == 0)//士林變頻器
                {
                    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                }
                else if (Gw1Dev == 1)//台達變頻器
                {
                    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                }
                else if (Gw1Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                }

                ////傳送指令到變頻器
                //if (Gw1Dev == 1)//三菱
                //{
                //    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));

                //}
                //else //士林
                //{
                //    masterSerialBus1.Add(this.Gw1.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw1.CmdSpeed / Gw1.Unit)).ToString("X4"));
                //}
            }
            if (GW2_Comm_Enabled)//判斷有無啟用砂輪2
            {
                //傳送指令到變頻器
                //masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));

                //傳送指令到變頻器
                if (Gw2Dev == 0)//士林變頻器
                {
                    masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                }
                else if (Gw2Dev == 1)//台達變頻器
                {
                    masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "062001" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                }
                else if (Gw2Dev == 2)//三菱變頻器
                {
                    masterSerialBus1.Add(this.Gw2.Slave.ToString("X2") + "06000E" + ((int)Math.Round(this.Gw2.CmdSpeed / Gw2.Unit)).ToString("X4"));
                }
            }
            if (GW4_Comm_Enabled)//判斷有無啟用滾輪
            {
                masterSerialBus1.Add(this.Roller.Slave.ToString("X2") + "061009" + ((int)Math.Round(this.Roller.CmdSpeed / Roller.Unit)).ToString("X4"));
            }

        }

        private void btn_ClearAllLine_Click(object sender, EventArgs e)
        {
            LB_GM_Code.Items.Clear();
            TB_GM_Code.Clear();
        }
        private void btn_ClearLine_Click(object sender, EventArgs e)
        {
            if (LB_GM_Code.SelectedIndex != -1)
            {
                LB_GM_Code.Items.RemoveAt(LB_GM_Code.SelectedIndex);
            }
        }

        private void btn_AddLine_Click(object sender, EventArgs e)
        {


            if (String.IsNullOrEmpty(TB_Input.Text)) return;

            LB_GM_Code.Items.Add(TB_Input.Text);
            TB_Input.Text = "";
            //LB_GM_Code.Text += form.TB_Input.Text + "\r\n";
            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
        }
        private void btn_InsertLine_Click(object sender, EventArgs e)
        {
            if (LB_GM_Code.SelectedIndex != -1)
            {

                if (String.IsNullOrEmpty(TB_Input.Text)) return;

                int selectedIndex = LB_GM_Code.SelectedIndex;
                LB_GM_Code.Items.Insert(selectedIndex, TB_Input.Text);

            }
        }
        //public System.Windows.Forms.Timer disableClickTimer;         // 定時器
        //public bool isCellClickDisabled = false; // 用於暫時屏蔽事件
        private void dgv_MP_Param_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            CB2.Items.Clear();
            CB2.Parent = null;
            //if (isCellClickDisabled) return; // 如果被禁用，直接返回

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            //bool MSMode = ini.ReadInteger("System", "MSMode", 1) == 1; // 0:M/min, 1:M/sec

            if (dgv_MP_Param.CurrentRow == null) return;

            int Row = dgv_MP_Param.CurrentCell.RowIndex;

            int.TryParse(dgv_MP_Param.Rows[Row].Cells[Col_MP_ParamType.Index].Value.ToString(), out int type);

            XmlElement x = (XmlElement)dgv_MP_Param.Rows[Row].Cells[Col_Param_XmlNode.Index].Value;
            Dictionary<int, string> Val2Txt = (Dictionary<int, string>)dgv_MP_Param.Rows[Row].Cells[Col_MP_ENUM.Index].Value;
            if (Val2Txt == null || Val2Txt.Count <= 0)
            {
                un_ProcessParam.la_Num.Text = dgv_MP_Param.CurrentRow.Cells[Col_MP_ParamShow.Index].Value.ToString();
                un_ProcessParam.la_Msg.Font = new Font("微軟正黑體", 12f, (FontStyle)0);
                un_ProcessParam.la_Msg.Text = dgv_MP_Param.CurrentRow.Cells[0].Value.ToString(); //名稱
                string sMax = dgv_MP_Param.CurrentRow.Cells[Col_MP_Max.Index].Value.ToString();
                string sMin = dgv_MP_Param.CurrentRow.Cells[Col_MP_Min.Index].Value.ToString();
                string sUnit = dgv_MP_Param.CurrentRow.Cells[Col_MP_Unit.Index].Value.ToString();
                double.TryParse(sMax, out double max);
                double.TryParse(sMin, out double min);
                //有定義範圍
                if ((max != 0 || min != 0))
                {
                    un_ProcessParam.la_Msg.Text += "\r\n" + sMin + " ~ " + sMax;
                }
                return;
            }

            if (e == null) return;

            //下拉式選項
            foreach (string txt in Val2Txt.Values)
            {
                CB2.Items.Add(txt);
            }

            CB2.Text = dgv_MP_Param.CurrentCell.Value.ToString();
            Rectangle rect = dgv_MP_Param.GetCellDisplayRectangle(0, Row, false);
            CB2.Parent = dgv_MP_Param;
            CB2.Font = new Font("Times New Roman", 20);
            CB2.DropDownStyle = ComboBoxStyle.DropDownList;
            CB2.Left = 0;
            CB2.Top = rect.Top;
            CB2.Width = dgv_MP_Param.Width;
            CB2.Height = 20;
            CB2.Visible = true;
            CB2.Focus();

        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            if (DGV_ProcList.CurrentRow == null)
            {
                return;
            }
            int index = DGV_ProcList.CurrentRow.Index;
            if (index < 0)
                return;
            if (CurrentProgram != null)
                if (TempProgram.ID == CurrentProgram.ID) TempExecEnabled.Add(true);

            //複製工序
            TProcess p = TempProgram.Processes[index].Clone();
            ProcessIndex = TempProgram.Processes.Count;//紀錄為目前正在編輯的工序
            TempProcess = p;
            TempProgram.Processes.Add(p);//將工序加入到清單最後


            //路徑
            String FileName = Application.StartupPath + "\\image\\Process\\40x40\\Process" + (p.ID).ToString("00") + ".bmp";
            if (File.Exists(FileName))
            {
                //工序清單新增一筆
                DGV_ProcList.Rows.Add(Bitmap.FromFile(FileName), p.Name, p, p.Memo);//(工序,加工模式)
                DGV_ProcList.Rows[DGV_ProcList.Rows.Count - 1].Cells[4].Style.ForeColor = Color.Gray;
            }
            else
            {
                //工序清單新增一筆
                DGV_ProcList.Rows.Add(new Bitmap(40, 40), p.Name, p, p.Memo);//(工序,加工模式)  
                DGV_ProcList.Rows[DGV_ProcList.Rows.Count - 1].Cells[4].Style.ForeColor = Color.Gray;
            }
            DGV_ProcList.CurrentCell = DGV_ProcList.Rows[ProcessIndex].Cells[0];
            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
            Col_ProcList_Btn.Visible = false;
            SetProcessData(p);//編輯畫面顯示此工序
        }

        private void btn_SoftPanel_MouseDown(object sender, MouseEventArgs e)
        {
            while (bSoftPanelBuzy) Application.DoEvents();
            bRefleshSoftPanell = true;
            //Application.DoEvents();

            pa_SoftPanel.Visible = !pa_SoftPanel.Visible;
            pa_SoftPanel.BringToFront();
            //btn_SoftPanel.Lamp = pa_SoftPanel.Visible;

        }

        private void Layout_Setting(object sender, EventArgs e)//Label Double Click 可以編輯 Panel
        {
            if (!bDevelop) return; //例外處理

            Label la = (Label)sender;
            if (fo_layout != null) fo_layout.Close();
            fo_layout = new Fo_Layout(la);
            fo_layout.ch_Grid.Checked = bGridMode;
            la.MouseMove += fo_layout.label1_MouseMove;
            fo_layout.Text = "Layout - " + la.Text;
            fo_layout.Show();

        }


        private void btn_OffsetMouseDown(object sender, MouseEventArgs e)
        {
            Uc_RoundBtn btn = sender as Uc_RoundBtn;
            if (btn == null) return;



            if (DGV_Offset.CurrentCell == null)
                return;
            int Row = DGV_Offset.CurrentCell.RowIndex;
            int Col = DGV_Offset.CurrentCell.ColumnIndex;

            if (CurrentProgram == null)
                return;
            if (CurrentProgram.Processes[Row] == null)
                return;


            //double offset = 0.01;
            double.TryParse(btn.DisplayText.ToString(), out double offset);

            if (Col == Col_OffsetX.Index)
            {
                TOffset ofs = CurrentProgram.Processes[Row].OffsetX;
                ofs.Value += offset;
                if (Math.Round(ofs.Value, 5) > Math.Round(OffsetMax, 5))
                {
                    ofs.Value = OffsetMax;
                    Application.DoEvents();
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (Math.Round(ofs.Value, 5) < Math.Round(OffsetMin, 5))
                {
                    ofs.Value = OffsetMin;
                    Application.DoEvents();
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }

                DGV_Offset.CurrentCell.Value = ofs.Value.ToString(Units.DisplayFmt);
            }
            else if (Col == Col_OffsetZ.Index)
            {
                TOffset ofs = CurrentProgram.Processes[Row].OffsetZ;
                ofs.Value += offset;
                if (Math.Round(ofs.Value, 5) > Math.Round(OffsetMax, 5))
                {
                    ofs.Value = OffsetMax;
                    Application.DoEvents();
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 115, "超過預設補正值上限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                if (Math.Round(ofs.Value, 5) < Math.Round(OffsetMin, 5))
                {
                    ofs.Value = OffsetMin;
                    Application.DoEvents();
                    Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 116, "超過預設補正值下限"));
                    //WinApi.SendMessage(btn.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
                }
                DGV_Offset.CurrentCell.Value = ofs.Value.ToString(Units.DisplayFmt);
            }


            btn_SaveOffset.Enabled = true;
        }

        private void CheckSaveProgram()
        {
            if (fo_TraverseStep != null)
            {
                fo_TraverseStep.Close();
                fo_TraverseStep = null;
            }



            pic_Descript.Visible = false;

            if (btn_SaveProgVisible)
            {
                DialogResult ret = Fo_Msg.Show(
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 118, "是否要存檔"),
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                        MessageBoxButtons.YesNo);
                if (ret == DialogResult.Yes)
                {
                    if (TC_Main.SelectedTab == tab_EditProc)
                    {
                        if (LB_GM_Code.Visible) LB_GM_Code_TextChanged();
                        else if (TB_GM_Code.Visible) TB_GM_Code_TextChange();
                    }

                    btn_SaveProg.PerformClick();
                }
                else
                {
                    btn_SaveProg.Visible = false;
                    btn_SaveProgVisible = false;
                }
            }

        }

        private void pic_Exit_Click(object sender, EventArgs e)
        {
            DialogResult ret = Fo_Msg.Show(
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 119, "是否要關機"),
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                        MessageBoxButtons.YesNo);
            if (ret == DialogResult.Yes)
            {
                // 註意：/s表示關機，/t 0表示立即關機，/f強制(不會因為其他軟體未關閉而卡住)
                Process.Start("shutdown", "/s /f /t 0");
            }
        }

        private void la_MaintenanceTitle_DoubleClick(object sender, EventArgs e)
        {

        }

        private void tab_Monitor_DoubleClick(object sender, EventArgs e)
        {
            if (!bDevelop) return;//例外處理

            if (fo_monitor == null) fo_monitor = new Fo_Monitor_List();
            fo_monitor.Show();
        }

        private void tab_Maintenance_DoubleClick(object sender, EventArgs e)
        {
            if (!bDevelop) return;//例外處理
            if (fo_maintan == null) fo_maintan = new Fo_MaintainceList();
            fo_maintan.Show();
        }

        private void uBtn_UnitChange_Click(object sender, EventArgs e)
        {
            //focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);

            bool bInchFlag = bInchTrans;


            DialogResult ret = Fo_Msg.Show(
                                      LanguageManager.LoadMessage(Units.langfile, "Message", 121, "確認是否要公英制轉換"),
                                      LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                      MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
            {
                return;
            }

            //
            //pa_ParamMsg.Location = new Point(8, 496);
            //pa_ParamMsg.Size = new Size(632, 88);
            pa_ParamMsg.Visible = true;
            Application.DoEvents();

            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(980, 3);//R系列公英制轉換
                OneKeyCall(8999);
            }));

            Thread UnitChange = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(2000);

                    int iWaitCycleStart = Environment.TickCount;
                    bool bWaitTimeout = false;
                    //這個迴圈等待程式啟動
                    while (true)
                    {
                        int iTime = Environment.TickCount - iWaitCycleStart;
                        if (iTime > 10000)
                        {
                            bWaitTimeout = true;
                            break;
                        }

                        Thread.Sleep(50);
                        byte F0 = 0;
                        bool bFinish = false;
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.PMC_ReadByte(PmcAddrType.F, 0, out F0);
                            bFinish = true;
                        }));
                        int iStart = Environment.TickCount;
                        while (!bFinish)
                        {
                            int iTime2 = Environment.TickCount - iStart;
                            if (iTime2 > 5000)
                            {

                                //this.Invoke(new Action(() => { 
                                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常")); 
                                //}));
                                return;
                            }
                            Thread.Sleep(50);
                        }

                        if (F0.BIT_7()) break;//程式啟動了
                    }

                    if (bWaitTimeout)
                    {
                        this.Invoke(new Action(() =>
                        {
                            DialogResult ret2 = Fo_Msg.Show(
                            LanguageManager.LoadMessage(Units.langfile, "Message", 120, "程式啟動失敗"),
                            LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                            MessageBoxButtons.OK);

                            pa_ParamMsg.Visible = false;
                            Application.DoEvents();
                        }));

                        return;
                    }

                    //這個迴圈等待#970=5
                    while (true)
                    {
                        Thread.Sleep(50);
                        double Macro970 = 0;
                        byte F0 = 0;
                        bool bFinish = false;
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.ReadMacro(970, out Macro970);
                            focas.PMC_ReadByte(PmcAddrType.F, 0, out F0);
                            bFinish = true;
                        }));

                        int iStart = Environment.TickCount;
                        while (!bFinish)
                        {
                            int iTime = Environment.TickCount - iStart;
                            if (iTime > 5000)
                            {

                                //this.Invoke(new Action(() => { 
                                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常")); 
                                //}));
                                return;
                            }
                            Thread.Sleep(50);
                        }

                        if (Macro970 == 5) break; //加工程式回到原點後，下#970=5 等待公英制轉換完畢
                        if (!F0.BIT_7())
                        {
                            this.Invoke(new Action(() =>
                            {
                                pa_ParamMsg.Visible = false;
                                Application.DoEvents();
                            }));
                            return; //程式中斷
                        }
                    }

                    TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");


                    // OptionParam.xml
                    double offmax = ini.ReadFloat("Parameter", "OffsetMax", 0);
                    double offmin = ini.ReadFloat("Parameter", "OffsetMin", 0);
                    if (bInchFlag)// Inch -> mm
                    {
                        offmax = offmax * 25.4;
                        offmin = offmin * 25.4;
                        ini.WriteFloat("Parameter", "OffsetMax", offmax);
                        ini.WriteFloat("Parameter", "OffsetMin", offmin);
                    }
                    else// mm -> inch
                    {
                        offmax = offmax / 25.4;
                        offmin = offmin / 25.4;
                        ini.WriteFloat("Parameter", "OffsetMax", offmax);
                        ini.WriteFloat("Parameter", "OffsetMin", offmin);
                    }
                    OffsetMax = offmax;
                    OffsetMin = offmin;


                    Actions.Enqueue(new Action(() =>
                    {
                        focas.WriteMacro(970, 0);
                    }));

                    while (true)
                    {
                        Thread.Sleep(50);
                        byte F0 = 0;
                        bool bFinish = false;
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.PMC_ReadByte(PmcAddrType.F, 0, out F0);
                            bFinish = true;
                        }));

                        int iStart = Environment.TickCount;
                        while (!bFinish)
                        {
                            int iTime = Environment.TickCount - iStart;
                            if (iTime > 5000)
                            {

                                //this.Invoke(new Action(() => { 
                                    //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常")); 
                                //}));
                                return;
                            }
                            Thread.Sleep(50);
                        }

                        if (!F0.BIT_7()) break;//程式結束了
                    }

                    this.Invoke(new Action(() =>
                    {
                        pic_SP_Click(pic_SP, null);
                    }));

                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show(ex.Message);
                    }));
                }




            });
            UnitChange.Start();
            //pic_SP_Click(pic_SP, e);
        }

        //bool bNumSigned = false;//負號旗標


        private void btn_ProgList_Click(object sender, EventArgs e)
        {
            CheckSaveProgram();

            //清空右邊的程式預覽
            DGV_ProgView.Rows.Clear();

            //讀取程式庫
            LoadProgramDB();

            //重新顯示左側的清單
            ShowProgListFromDB();

            //選擇目前的程式
            if (CurrentProgram != null) SelectProgramList(CurrentProgram.ID);

            //DGV_Edit重新整理
            //ShowProcessList();//重新將 TempProgram 載入DGV_ProcList(TempExecEnabled 要正確)

            //切到程式頁
            TC_Main.SelectedTab = tab_ProgList;

            //關閉軟體面板
            pa_SoftPanel.Visible = false;

            PrevPage.Push(tab_ProgList);
            btn_Prev.Visible = true;
        }

        private void pic_ImportProg_Click(object sender, EventArgs e)
        {
            //關閉軟體面板
            pa_SoftPanel.Visible = false;

            fo_ImportProg = new Fo_ImportProg();
            fo_ImportProg.TopLevel = false;
            fo_ImportProg.Parent = tab_ImportProg;
            fo_ImportProg.Left = 0;
            fo_ImportProg.Top = 0;
            fo_ImportProg.Width = 944;
            fo_ImportProg.Height = 600;
            fo_ImportProg.Show();

            //切到程式頁
            PrevPage.Clear();
            TC_Main.SelectedTab = tab_ImportProg;
            PrevPage.Push(tab_ImportProg);
            btn_Prev.Visible = false;
        }

        private void DGV_ProcList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CB.Visible = false;
            BTN.Visible = false;
            //例外處理
            if (DGV_ProcList.CurrentRow == null)
                return;

            //例外處理
            int index = DGV_ProcList.CurrentRow.Index;
            if (index < 0)
                return;

            //目前選擇的工序
            TProcess p = TempProgram.Processes[index];
            ProcessIndex = index;
            TempProcess = p;

            if (DGV_ProcList.CurrentCell.ColumnIndex == Col_ProcList_Memo.Index)
            {
                Fo_Keyboard form = new Fo_Keyboard();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    p.Memo = form.TB_Input.Text;
                    DGV_ProcList.CurrentRow.Cells[Col_ProcList_Memo.Index].Value = p.Memo;

                    btn_SaveProg.Visible = true;
                    btn_SaveProgVisible = true;
                }
                return;
            }
            else if (DGV_ProcList.CurrentCell.ColumnIndex == Col_ProcList_Btn.Index)
            {
                //非編輯工序才能按加工的開關
                ExecEnabled[index] = TempExecEnabled[index] = !TempExecEnabled[index];

                Actions.Enqueue(new Action(() =>
                {
                    focas.WriteMacro(700 + index, TempExecEnabled[index] ? 1 : 0);
                }));
                DGV_ProcList.CurrentRow.Cells[Col_ProcList_Btn.Index].Value = TempExecEnabled[index] ? Properties.Resources.SwitchOn : Properties.Resources.SwitchOff;
                //監視 - 加工開關也跟著改變
                DGV_Monitor_Program.Rows[index].Cells[Col_ExeBtn.Index].Value = TempExecEnabled[index] ? Properties.Resources.SwitchOn : Properties.Resources.SwitchOff;
            }
        }

        private void pic_CNCDataManager_Click(object sender, EventArgs e)
        {
            bool bFind = false;
            if (this.CNCDataManageProcess != null)
            {
                if (CNCDataManageProcess.HasExited)
                {
                    CNCDataManageProcess = null;
                }
                else
                {
                    bFind = true;
                    List<IntPtr> list = ExWinApi.GetHandles();
                    foreach (IntPtr i in list)
                    {
                        string name = i.GetText();
                        if (name.Contains("CNC Data Management Tool"))
                        {
                            Console.WriteLine("Find CNC Data Management Tool");
                            WinApi.ShowWindow(i, (int)SetWindowPosFlags.SWP_SHOWWINDOW);
                        }
                    }
                }
            }

            if (!bFind)
            {
                String app = "C:\\Program Files (x86)\\FANUC\\CNCDataManagementTool\\CNCDataManagementTool.exe";
                if (File.Exists(app))
                {
                    try
                    {
                        Process p = Process.Start(app);
                        CNCDataManageProcess = p;
                        p.WaitForInputIdle();
                        //ScreenDisplay = p.MainWindowHandle;
                        //WinApi.SetParent(p.MainWindowHandle, this.Handle);
                        WinApi.SetParent(p.MainWindowHandle, tab_CNCDataManage.Handle);
                        WinApi.GetWindowRect(p.MainWindowHandle, out RECT rect);
                        int w = rect.Right - rect.Left;
                        int h = rect.Bottom - rect.Top;
                        //int l = (this.Width - w) / 2;
                        //int t = (this.Height - h) / 2;
                        int l = (tab_CNCDataManage.Width - w) / 2;
                        int t = (tab_CNCDataManage.Height - h) / 2;
                        WinApi.SetWindowPos(p.MainWindowHandle, IntPtr.Zero, l, t, w, h, SetWindowPosFlags.SWP_SHOWWINDOW);
                        //WinApi.MoveWindow(p.MainWindowHandle, l, t, w, h, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            //TempMaintanceTab = tab_CNCDataManage;

            TC_Main.SelectedTab = tab_CNCDataManage;
            PrevPage.Push(tab_CNCDataManage);
            btn_Prev.Visible = true;
        }


        private void btn_PositionSave_Click(object sender, EventArgs e)//位置設定 - 儲存
        {

            DialogResult ret = Fo_Msg.Show(
                   LanguageManager.LoadMessage(Units.langfile, "Message", 145, "是否要儲存"),
                   LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
                   MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
            {
                return;
            }

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            focas.PMC_ReadByte(PmcAddrType.F, 2, out byte F2);


        }


        private void MPCOE_SeriesChoose(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");

            DialogResult r = Fo_Msg.Show(
                                LanguageManager.LoadMessage(Units.langfile, "Message", 152, "是否進行初始化設定"),
                                LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
                                MessageBoxButtons.YesNo);
            if (r != DialogResult.Yes)
            {
                return;
            }

            ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            if (rb_M2.Checked)
            {                
                GwCount = 2;
                ini.WriteString("System", "MachineSeries", "M");
            }
            else if (rb_M3.Checked)
            {   
                GwCount = 3;
                ini.WriteString("System", "MachineSeries", "M");
            }
            ini.WriteInteger("System", "GWCount", GwCount);


            RadioButton rd = sender as RadioButton;
            if (rd == null) return;
            string sr = rd.Tag.ToString();//類型
            ResetProcess();//重設工序 ini
            ResetProglist();//刪除程式清單

            //Actions.Enqueue(new Action(() =>
            //{
            //    //focas.WriteMacro(977, 3);//機型 (此系列固定為 3)
            //    focas.WriteMacro(15050, GwCount);// 設定砂輪數量
            //}));

            
            //ini.WriteInteger("System", "GW3_Comm_Enabled", (MachType == MachineType.M3) ? 1 : 0);// 砂輪3是否啟用
            

            SetMonitorBtns();//切換機型 - 監視 - 下方按鍵變更
        }

        private void ResetProcess()
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            for (int i = 1; i <= 20; i++)//清空工序
            {
                ini.WriteString("UI", "ProcessTag" + i, "");
                ini.WriteString("UI", "ProcessImg" + i, "");
            }


            

            ini.WriteInteger("UI", "ProcessTag20", 999);//code
            ini.WriteString("UI", "ProcessImg20", "\\image\\Process\\150x150\\Process999.png");//code

        }
        private void ResetProglist()
        {
            if (DGV_ProgList.CurrentRow == null) return;

            int index = DGV_ProgList.RowCount;
            //例外處理
            if (index < 1) return;

            for (int i = index - 1; i >= 0; i--)
            {

                TProgram pg = DGV_ProgList.Rows[i].Cells[Col_TProgram.Index].Value as TProgram;
                //例外處理
                if (pg == null) return;

                //清除預覽
                DGV_ProgView.Rows.Clear();

                //目前已經有開啟程式時, 
                if (CurrentProgram != null)
                {
                    if (pg.ID == CurrentProgram.ID) //要檢查刪除的是否是開啟的
                    {
                        CurrentProgram = null; //清空目前程式
                    }
                }

                Units.ProgramDB.Programs.Remove(pg);//刪除程式
                DGV_ProgList.Rows.RemoveAt(i);//顯示程式清單 - 刪除程式, 會觸發事件重新顯示右邊的預覽

            }
            Units.ProgramDB.Save();//儲存


            RefleshProgListBtn();//更新下方按鍵
        }

        //private void ResetOptionParam(bool Vis)
        //{
        //    string filename = Application.StartupPath + "\\OptionParam.xml";
        //    if (!File.Exists(filename))
        //    {
        //        Fo_Msg.Show("OptionParam.xml " + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
        //        return;
        //    }
        //    //取得根元素(OptionParam)
        //    var doc = XDocument.Load(filename);
        //    var paramNodes = doc.Descendants("Param").ToList();
        //    string[] Gw2Addr = { "608", "609", "610", "611", "612", "613", "614", "615", "616","617","618", "619","626","627","630","631", "GW2_MaxRPM", "GW2_Grind_CRASH", "GW2_Dress_CRASH" };
        //    foreach (var param in doc.Descendants("Param"))
        //    {
        //        string addrValue = param.Attribute("Addr")?.Value;
        //        if (Gw2Addr.Contains(addrValue))
        //        {
        //            param.SetAttributeValue("Visible", Vis ? "0" : "1");
        //        }
        //    }
        //    doc.Save(filename);
        //}

        private void DGV_GwParam_Click(object sender, EventArgs e)
        {

            if (DGV_GwParam.Rows.Count <= 0) return;

            uc_UserNumSetGW.la_Num.Text = DGV_GwParam.CurrentRow.Cells[1].Value.ToString(); //數值
            uc_UserNumSetGW.la_Msg.Text = DGV_GwParam.CurrentRow.Cells[0].Value.ToString(); //名稱
            String path = DGV_GwParam.CurrentRow.Cells[Col_GwPicLink.Index].Value.ToString();//圖示
            if (File.Exists(path)) pic_GW_Param.BackgroundImage = new Bitmap(path);

            int.TryParse(DGV_GwParam.CurrentRow.Cells[3].Value.ToString(), out int no); // 抓隱藏欄位 macro

            Units.MacroInfo.GetMinMax(no, out double min, out double max); // 抓上下限並顯示
            if ((min != 0 || max != 0)) // 0 ~ 0 就不顯示
            {
                uc_UserNumSetGW.la_Msg.Text += "\r\n" + min + " ~ " + max;
            }

        }

        //bool gwedit = false;

        //private void GwEditCheck()
        //{
        //    TempMaintanceTab = null;//清空維護紀錄
        //    if (GwSetEdit)  //砂輪
        //    {
        //        //gwedit = true;
        //        btn_RegisterGw_Save.PerformClick();
        //    }
        //    else if (GwDressEdit) //修砂
        //    {
        //        //gwedit = true;
        //        btn_SaveDressGw.PerformClick();
        //    }
        //    else if (GwWorkPiEdit) //工件設定
        //    {
        //        //gwedit = true;
        //        btn_SaveGrindCoor.PerformClick();
        //    }
        //    GwSetEdit = false;
        //    GwDressEdit = false;
        //    GwWorkPiEdit = false;
        //    //gwedit = false;
        //}

        private void btn_FinishCountClear_Click(object sender, EventArgs e)
        {
            if (bCycleStart)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 77, "程式執行中不可使用"));
                return;
            }

            Actions.Enqueue(new Action(() =>
            {
                focas.Param_WriteDbWord(6711, 0, 0);
            }));
        }


        private void pic_Warmup_Click(object sender, EventArgs e)
        {
            //關閉軟體面板
            pa_SoftPanel.Visible = false;

            fo_Warmup = new Fo_Warmup(0);
            fo_Warmup.TopLevel = false;
            fo_Warmup.Parent = tab_Warmup;
            fo_Warmup.Left = 0;
            fo_Warmup.Top = 0;
            fo_Warmup.Width = 944;
            fo_Warmup.Height = 600;
            fo_Warmup.Show();

            //切到程式頁
            PrevPage.Clear();
            TC_Main.SelectedTab = tab_Warmup;
            PrevPage.Push(tab_Warmup);
            btn_Prev.Visible = false;


        }

        private void ch_FuncClick(object sender, EventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            if (ch == null) return; //例外處理
            int.TryParse(ch.Tag.ToString(), out int no);

            bool bChech = ch.Checked;
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(no, bChech ? 1 : 0);
            }));
            if (ch == ch_DGW_Func)//滾輪功能 
            {
                TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
                ini.WriteBool("System", "Rolleropen", ch.Checked);
                Rolleropen = ch.Checked;
            }
        }

        private void DGV_Monitor_Program_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            //例外處理
            if (row < 0)
                return;

            //例外處理
            if (row >= ExecEnabled.Count)
                return;
            DGV_Monitor_Program.Rows[row].Selected = false;
        }

        private void btn_Monitor_RelY_Zero_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            dManualZeroPointY = double.Parse(la_MonitorMachAxis3Value.Text);
            ini.WriteFloat("System", "dManualZeroPointY", dManualZeroPointY);
        }

        private void btn_657_Click(object sender, EventArgs e)
        {

        }


        private void btn_Search_Click(object sender, EventArgs e)
        {
            int start_index = 0;
            if (DGV_ProgList.CurrentRow != null)
            {
                start_index = DGV_ProgList.CurrentRow.Index;
            }

            for (int i = 0; i < DGV_ProgList.Rows.Count; i++)
            {
                int index = (i + start_index + 1) % DGV_ProgList.Rows.Count;
                string prog_name = DGV_ProgList.Rows[index].Cells[Col_ProgName.Index].Value.ToString().ToUpper();
                if (prog_name.Contains(tb_Search.Text.ToUpper()))
                {
                    DGV_ProgList.CurrentCell = DGV_ProgList.Rows[index].Cells[0];
                    break;
                }
            }
        }

        private void masterSerialBus2_OnReceive(object sender, string receive, string cmd)
        {
            if (TC_Main.SelectedTab == tab_Developer) tb_serial.AppendText("R:" + receive + "\r\n");

            CheckSerialHeart = Environment.TickCount;

            int iCommErrTime = Environment.TickCount - COMM_ERR_START;//RS485
            int iCommErrTime2 = Environment.TickCount - COMM_ERR_START2;//RS422
            if (iCommErrTime > 3 && iCommErrTime2 > 3) pa.Visible = false;

            if (receive.Length < 2) return;
            int iSlave = int.Parse(receive.Substring(0, 2), NumberStyles.HexNumber);
            if (receive.Length < 4) return;
            int iFunc = int.Parse(receive.Substring(2, 2), NumberStyles.HexNumber);


            int iCheckSlave = int.Parse(cmd.Substring(0, 2), NumberStyles.HexNumber);

            //例外處理
            if (iSlave != iCheckSlave) return;

            if (iSlave == Spindle.Slave)
            {
                SPERROR_Count = 0;
                if (SpindleDev == 0)
                {
                    //Spindle 三菱驅動器
                    int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                    if (iFunc == 3)
                    {
                        if (iAddr == 0x2B02)
                        {
                            if (receive.Length < 14) return;

                            //轉速 0x2B02
                            Spindle.OutSpeed = int.Parse(receive.Substring(10, 4) + receive.Substring(6, 4), NumberStyles.HexNumber);
                            int out_speed = (int)Math.Round(Spindle.OutSpeed / Spindle.Rate / Spindle.Unit * Spindle.ShowRate);
                            la_SpindleNowRpmVal.Text = Math.Abs(out_speed).ToString();//RS422 三菱驅動器 轉速
                        }
                    }
                }
                else if (SpindleDev == 1)
                {
                    //Spindle 安川驅動器
                    int iAddr = int.Parse(cmd.Substring(12, 4), NumberStyles.HexNumber);

                    iFunc = int.Parse(receive.Substring(6, 2), NumberStyles.HexNumber);

                    if (iFunc == 3)
                    {
                        if (iAddr == 0xE000)
                        {
                            //轉速 0xE000 (單位 : RPM),<<<<<<注意>>>>>>反轉會出現負值,要加絕對值
                            //this.RW_OutSpeed = Int16.Parse(Msg.Substring(16, 4), NumberStyles.HexNumber);
                            //實際轉速(RPM) = 輸出轉速(RPM) * 單位(1) / 倍率(%)
                            int out_speed = (int)Math.Abs(Int16.Parse(receive.Substring(16, 4), NumberStyles.HexNumber) * Spindle.Unit / Spindle.Rate);
                            la_SpindleNowRpmVal.Text = out_speed.ToString();//RS422 安川驅動器 轉速

                            //0xE001
                            //Int16.Parse(Msg.Substring(20, 4), NumberStyles.HexNumber);

                            //內部轉矩 0xE002 (%)
                            //Int16.Parse(Msg.Substring(24, 4), NumberStyles.HexNumber);

                        }
                    }
                }
                else if (SpindleDev == 2)//Spindle 士林變頻器
                {
                    int iAddr = int.Parse(cmd.Substring(4, 4), NumberStyles.HexNumber);
                    if (iFunc == 3)
                    {
                        //士林變頻器 (指令頻率、輸出頻率、輸出電流...)
                        if (iAddr == 0x1002)
                        {
                            //實際轉速(RPM) = 輸出轉速(RPM) * 單位(1) / 倍率(%)
                            int out_speed = (int)Math.Abs(Int16.Parse(receive.Substring(10, 4), NumberStyles.HexNumber) * Spindle.Unit * Spindle.Rate);
                            la_SpindleNowRpmVal.Text = out_speed.ToString();//RS422 士林變頻器 轉速
                        }
                    }
                }
            }
        }

        private void btn_Language_Save_Click(object sender, EventArgs e)
        {
            int index = dgv_Language.CurrentRow.Index;
            if (index < 0) return;

            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            string lang = dgv_Language.CurrentRow.Cells[Col_LangCode.Index].Value.ToString();
            Units.LangCode = lang;
            SetLanguage(lang);

            if (SpBtnDic != null)
            {
                foreach (var btn in SpBtnDic.Values)
                {
                    btn.Parent = null;
                }
                SpBtnDic = null;
            }
            dgv_MP_Param.Rows.Clear();
        }

        private void pic_User_Click(object sender, EventArgs e)
        {
            pa_Develop.Visible = false;

            Fo_Permission form = new Fo_Permission();
            if (form.ShowDialog() != DialogResult.OK) return;

            if (form.TB_ID.Text.ToLower() == "palmary" && form.TB_PSWD.Text.ToLower() == "16524622")
            {
                bDevelop = true;
                pa_Develop.Visible = true;
                pic_User.Image = Properties.Resources.user99;

                tb_MeasureStep.Visible = true;
                //tb_OffsetStep.Visible = true;
                //label29.Visible = true;
                //label33.Visible = true;
                //TB_PartCenterPosX.Visible = true;
                //btn_ID_CenterPos.Visible = true;
            }
            else
            {
                bDevelop = false;
                pa_Develop.Visible = false;
                pic_User.Image = Properties.Resources.user1s;

                tb_MeasureStep.Visible = false;
                //tb_OffsetStep.Visible = false;

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Focas1.ODBDGN data = new Focas1.ODBDGN();
            ushort h = focas.FlibHndl;


            for (int i = 0; i < 4; i++)
            {
                int ret = Focas1.cnc_diagnoss(h, 302, (short)(i + 1), 12, data);
                if (ret == 0)
                {
                    double val = data.u.rdata.dgn_val * Math.Pow(10, -data.u.rdata.dec_val);
                    tb_Debug.AppendText(val.ToString() + "\r\n");
                }
                else
                {
                    tb_Debug.AppendText("FAIL" + i + ":" + ret + "\r\n");
                }
            }



        }


        XmlDocument EditDoc = new XmlDocument();
        List<XmlElement> xList = new List<XmlElement>();
        private void pic_EditSP_Click(object sender, EventArgs e)
        {
            dgv_EditProcParam.Rows.Clear();


            TC_Main.SelectedTab = tab_EditSP;

            string filename = Application.StartupPath + "\\OptionParam.xml";

            // 檢查檔案是否存在
            if (!System.IO.File.Exists(filename))
            {
                Fo_Msg.Show("OptionParam.xml" + LanguageManager.LoadMessage(Units.langfile, "Message", 12, "檔案丟失"));
                return;
            }

            // 載入 XML
            EditDoc.Load(filename);

            checkedListBox1.Items.Clear();
            xList.Clear();

            //取得根元素(OptionParam)
            XmlNode root_x = EditDoc.DocumentElement;
            //開始解析XML檔
            foreach (XmlElement xPage in root_x.ChildNodes)
            {
                if (xPage.Name != "Page") continue;
                string PageName = xPage.GetAttribute("Name");
                if (PageName == "") continue;
                checkedListBox1.Items.Add(PageName);
                xList.Add(xPage);

                foreach (XmlElement x in xPage.ChildNodes)
                {
                    if (x.Name != "Param") continue;

                    string memo = x.GetAttribute("Memo");
                    Bitmap bmp = x.GetAttribute("Visible") == "1" ? Resources.SwitchOn : Resources.SwitchOff;
                    string page = x.GetAttribute("Page");


                    dgv_EditProcParam.Rows.Add(bmp, memo, page, x);
                }
            }


            PrevPage.Push(tab_EditSP);
            btn_Prev.Visible = true;
        }
        private void dgv_EditProcParam_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_EditSelectValue.Rows.Clear();

            int Col = e.ColumnIndex;
            if (Col < 0) return;

            int Row = e.RowIndex;
            if (Row < 0) return;

            XmlElement x = (XmlElement)dgv_EditProcParam.Rows[Row].Cells[Col_EditProcParam_Node.Index].Value;

            if (Col == Col_EditProcParam_Visible.Index)
            {
                string sVisible = x.GetAttribute("Visible");
                bool bVisible = sVisible == "1";//顯示的狀態
                bVisible = !bVisible;//切換

                dgv_EditProcParam.Rows[Row].Cells[Col].Value = bVisible ? Resources.SwitchOn : Resources.SwitchOff;
                x.SetAttribute("Visible", bVisible ? "1" : "0");
            }

            //此Param 還有子節點，表示此參數是 下拉式選單(ComboBox)
            if (x.ChildNodes.Count <= 0) return;
            for (int j = 0; j < x.ChildNodes.Count; j++)
            {
                XmlElement child = (XmlElement)x.ChildNodes[j];
                if (child.Name != "Value") continue;//例外處理

                //文字
                string text = child.GetAttribute("Memo");

                //顯示(可能會沒有此屬性, 會得到"", 所以如果有得到"0", 就是曾經手動修改讓他隱藏)
                string sVisible = child.GetAttribute("Visible");
                Bitmap bmp = sVisible == "0" ? Resources.SwitchOff : Resources.SwitchOn;
                if (sVisible == "") child.SetAttribute("Visible", "1");

                dgv_EditSelectValue.Rows.Add(text, bmp, child);
            }

        }
        private void dgv_EditSelectValue_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int Col = e.ColumnIndex;
            if (Col == 0) return;
            if (Col < 0) return;

            int Row = e.RowIndex;
            if (Row < 0) return;

            XmlElement x = (XmlElement)dgv_EditSelectValue.Rows[Row].Cells[2].Value;

            bool bVisible = x.GetAttribute("Visible") == "1";//目前是 ON的?
            dgv_EditSelectValue.Rows[Row].Cells[Col].Value = bVisible ? Resources.SwitchOff : Resources.SwitchOn; //如果ON的就OFF / 如果OFF的就ON
            x.SetAttribute("Visible", bVisible ? "0" : "1"); //如果ON的就OFF / 如果OFF的就ON
        }

        private void uc_UserNumEditProc_OnBtnOkClick(object sender, EventArgs e)
        {
            //目前編輯的工序 
            if (TempProcess == null) return;

            //端測
            if (TempProcess.ID == 201)
            {
                double.TryParse(uc_UserNumEditProc.la_Num.Text, out double data);


                bool bFinish = false;
                byte F2 = 0;
                Actions.Enqueue(new Action(() =>
                {
                    focas.PMC_ReadByte(PmcAddrType.F, 2, out F2);
                    bFinish = true;
                }));

                int iStart = Environment.TickCount;
                while (!bFinish)
                {
                    int iTime = Environment.TickCount - iStart;
                    if (iTime > 5000)
                    {

                        //Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 45, "通訊異常"));
                        break;
                    }
                    Application.DoEvents();
                }
                
                if (F2.BIT_0()) //inch 0.04 ~ 4
                {
                    if (data < 0.04) data = 0.04;
                    if (data > 4) data = 4;
                }
                else //mm 1~100
                {
                    if (data < 1) data = 1;
                    if (data > 100) data = 100;
                }
                TB_Dist.Text = data.ToString(Units.DisplayFmt);


                return;
            }



            if (Edit_DGV == null) return;
            if (Edit_DGV.CurrentCell == null)
                return;

            int Row = Edit_DGV.CurrentCell.RowIndex;



            TArgument a = Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["PCode"]].Value as TArgument;
            if (a == null) return;

            double.TryParse(uc_UserNumEditProc.la_Num.Text, out double val);
            if (val < a.Min)
            {
                val = a.Min;
                uc_UserNumEditProc.la_Num.Text = val.ToString("0.#####");
            }
            if (val > a.Max)
            {
                val = a.Max;
                uc_UserNumEditProc.la_Num.Text = val.ToString("0.#####");
            }
            var processNode = Units.xmlDefaultProcessLang.Descendants("Process")//xml
                                 .FirstOrDefault(x => x.Attribute("ID")?.Value == TempProcess.ID.ToString());
            var pcodesWithTexts = processNode.Elements("PCode")
                    .Where(x => x.Elements("Text").Any())
                    .Select(x => new PCodeInfo
                    {
                        PCodeNo = x.Attribute("No")?.Value,
                        Texts = x.Elements("Text")
                            .Select(text => new TextInfo
                            {
                                Value = text.Attribute("Value")?.Value,
                                Name = text.Attribute("Name")?.Value
                            }).ToList()
                    }).ToList();
            var matchingPCode = pcodesWithTexts.FirstOrDefault(pcode => pcode.PCodeNo == a.AddrCode);
            string[] unit = { "times", "rpm", "steps", "sec" };

            if (matchingPCode == null)
            {
                if (unit.Contains(a.Unit))
                {
                    val = (int)val;
                    Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value = val.ToString("0");
                }
                else
                {
                    //數值寫回顯示欄
                    val = Math.Round(val, 5);

                    Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value = val.ToString(Units.DisplayFmt);
                }

                DataGridViewCell cell = Edit_DGV.CurrentCell;
                new Thread(() =>
                {
                    this.Invoke(new Action(() =>
                    {
                        Edit_DGV.CurrentCell = null;
                    }));
                    Thread.Sleep(250);
                    this.Invoke(new Action(() =>
                    {
                        Edit_DGV.CurrentCell = cell;
                    }));
                }).Start();

                //寫回引數
                a.Value = val;
                btn_SaveProg.Visible = true;
                btn_SaveProgVisible = true;
            }
            else
            {
                CB.Visible = false;
                BTN.Visible = false;
                foreach (var vn in matchingPCode.Texts)
                {
                    if (vn.Value == ((int)val).ToString())
                    {
                        a.Value = val;
                        Edit_DGV.Rows[Row].Cells[Edit_DGV_Index["TextValue"]].Value = vn.Name;
                        break;
                    }
                }

                //使用組別
                if (a.AddrCode == "19865")
                {
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.PMC_WriteWord(PmcAddrType.D, 68, (short)val);
                    }));                  
                }
            }
            //uc_UserNumEditProc.la_Num.Text = "0";
            //bNumSigned = false;
        }

        private void uc_UserNumEditProc_OnBtnMemoryClick(object sender, EventArgs e)
        {
            /*筆記
            MemMapping["19810"] = Buf1; //X1
            MemMapping["19811"] = Buf2; //X2
            MemMapping["19812"] = Buf3; //Z1
            MemMapping["19813"] = Buf4; //Z2
            */


            if (Edit_DGV == null) return;
            if (Edit_DGV.CurrentCell == null)
                return;

            int Row = Edit_DGV.CurrentCell.RowIndex;


            //目前編輯的工序 
            if (TempProcess == null) return;

            //TSubProgram sp = Edit_DGV.Rows[Row].Cells[3].Value as TSubProgram;
            TArgument a = Edit_DGV.Rows[Row].Cells[4].Value as TArgument;
            if (a == null) return;
            bool ret = GetMemoryValue(out double val);

            if (!ret) return;

            if (TempProcess.ID >= 7 && TempProcess.ID <= 9 && a.AddrCode == "19810")
            {
                if (val < 0) val = val * -1;
                else if (val > 0) val = 0;
            }


            uc_UserNumEditProc.la_Num.Text = val.ToString("0.#####");
            if (val < a.Min)
            {
                val = a.Min;
                uc_UserNumEditProc.la_Num.Text = val.ToString("0.#####");
            }
            if (val > a.Max)
            {
                val = a.Max;
                uc_UserNumEditProc.la_Num.Text = val.ToString("0.#####");
            }
            val = Math.Round(val, 5);

            //數值寫回顯示欄
            Edit_DGV.Rows[Row].Cells[2].Value = val.ToString(Units.DisplayFmt);


            //寫回引數
            a.Value = val;
            btn_SaveProg.Visible = true;
            btn_SaveProgVisible = true;
        }


        private void btn_MoveUp_Click(object sender, EventArgs e)
        {
            int index = LB_GM_Code.SelectedIndex;
            if (index <= 0) return;
            var item = LB_GM_Code.Items[index];
            string temp = item.ToString();
            LB_GM_Code.Items.RemoveAt(index);
            LB_GM_Code.Items.Insert(index - 1, temp);
            LB_GM_Code.SelectedIndex = index - 1;
        }

        private void btn_MoveDown_Click(object sender, EventArgs e)
        {
            int index = LB_GM_Code.SelectedIndex;
            if (index < 0) return;
            if (index >= LB_GM_Code.Items.Count - 1) return;
            var item = LB_GM_Code.Items[index];
            string temp = item.ToString();
            LB_GM_Code.Items.RemoveAt(index);
            LB_GM_Code.Items.Insert(index + 1, temp);
            LB_GM_Code.SelectedIndex = index + 1;
        }

        private void btn_changeenter_Click(object sender, EventArgs e)
        {
            if (LB_GM_Code.Visible)
            {
                TB_GM_Code.Text = string.Join(Environment.NewLine, LB_GM_Code.Items.Cast<string>());

                // 清空 ListBox
                LB_GM_Code.Items.Clear();
                LB_GM_Code.Visible = false;

                TB_GM_Code.SelectionStart = TB_GM_Code.Text.Length;// 設定游標到文字最後
                TB_GM_Code.SelectionLength = 0;

                TB_Input.Visible = btn_InsertLine.Visible = btn_MoveUp.Visible = btn_AddLine.Visible = btn_ClearLine.Visible = btn_MoveDown.Visible = false;
                TB_GM_Code.Visible = btn_keyboard.Visible = true;
                TB_GM_Code.Focus();
            }
            else
            {
                string[] lines = TB_GM_Code.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                // 避免重複添加
                foreach (string line in lines)
                {
                    if (!LB_GM_Code.Items.Contains(line))
                    {
                        LB_GM_Code.Items.Add(line);
                    }
                }
                TB_GM_Code.Visible = btn_keyboard.Visible = false;
                TB_Input.Visible = LB_GM_Code.Visible = true;//預設顯示listbox
                TB_Input.Focus();
                btn_InsertLine.Visible = btn_MoveUp.Visible = btn_AddLine.Visible = btn_ClearLine.Visible = btn_MoveDown.Visible = true;
            }
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteInteger("System", "GMCodeMode", LB_GM_Code.Visible ? 0 : 1);
        }

        private void btn_keyboard_Click(object sender, EventArgs e)
        {
            TB_GM_Code.Focus();
            try
            {
                Process p = Process.Start(Environment.SystemDirectory + "\\osk.exe");
            }
            catch (Exception)
            {
            }
        }

        private void TB_GM_Code_TextChange()
        {
            TProcess process = null;
            if (ProcessIndex >= 0)
            {
                if (ProcessIndex >= TempProgram.Processes.Count) return;
                process = TempProgram.Processes[ProcessIndex];
            }
            if (process == null) return;

            process.GM_Code.Clear();
            List<String> lines = TB_GM_Code.Lines.ToList();
            foreach (String s in lines)
            {
                String Data = s.Trim();
                if (Data != "")
                {
                    process.GM_Code.Add(Data);
                }
            }
        }

        private void TB_Input_Click(object sender, EventArgs e)
        {
            TB_Input.Focus();
            try
            {
                Process p = Process.Start(Environment.SystemDirectory + "\\osk.exe");
            }
            catch (Exception)
            {
            }
        }

        private void TB_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_AddLine.PerformClick();
            }
        }

        private void LB_GM_Code_KeyPress(object sender, KeyPressEventArgs e)
        {
            TB_Input.Text += e.KeyChar;
            TB_Input.SelectionStart = TB_Input.Text.Length; // 移動游標到文字最後
            TB_Input.Focus();
        }

        private void tb_Search_Click(object sender, EventArgs e)
        {
            Fo_Keyboard form = new Fo_Keyboard();
            if (form.ShowDialog() == DialogResult.OK)
            {
                tb_Search.Text = form.TB_Input.Text;
            }
        }

        private void uc_UserNumSetGW_OnBtnOkClick(object sender, EventArgs e) //砂輪資料 - 形狀參數(DataGridView) - 輸入(數字鍵盤)
        {
            if (DGV_GwParam.Rows.Count <= 0) return;//例外處理
            if (DGV_GwParam.CurrentRow == null) return;
            if (DGV_GwParam.CurrentRow.Cells[Col_GwParam_MacroNo.Index].Value == null) return; //例外處理

            double.TryParse(uc_UserNumSetGW.la_Num.Text, out double data);//取得輸入的數值

            int.TryParse(DGV_GwParam.CurrentRow.Cells[Col_GwParam_MacroNo.Index].Value.ToString(), out int no); // 取得 Macro 位址

            Units.MacroInfo.CheckMacroMinMax(no, ref data);//檢查上下限並修正, 沒問題會回傳 true

            DGV_GwParam.CurrentRow.Cells[1].Value = data.ToString(Units.DisplayFmt); //更新畫面
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            CurrentGwMacro[no + GwMarcoOffset] = data;

            DataGridViewCell cell = DGV_GwParam.CurrentCell;//先記錄目前位置
            DGV_GwParam.CurrentCell = null; //未選擇任何欄位

            WriteGwMacro(CurrentEditGwNo, no, data);

            if (DGV_GwParam.CurrentCell == null) DGV_GwParam.CurrentCell = cell; //選擇原本的欄位

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void dgv_SoftPanelList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btn_SoftPanelChange.PerformClick();
        }

        private void btn_SoftPanelChange_Click(object sender, EventArgs e)
        {

            if (dgv_SoftPanelList.CurrentRow != null)
            {
                // 取得選取的那筆的路徑（第二欄）
                string selectedPath = dgv_SoftPanelList.CurrentRow.Cells[1].Value.ToString();

                string destinationFile = Path.Combine(Application.StartupPath, "SoftPanel.xml");

                try
                {
                    File.Copy(selectedPath, destinationFile, true);
                    LoadSoftPanelFromXML(destinationFile);
                    pa_SoftPanel.Left = 0;
                    pa_SoftPanel.Top = this.Height - pa_Bottom.Height - pa_SoftPanel.Height;
                }
                catch { }
            }
        }

        private void btn_SoftPanelRe_Click(object sender, EventArgs e)
        {
            string SoftPanelPath = Path.Combine(Application.StartupPath, "SoftPanel");

            if (Directory.Exists(SoftPanelPath))
            {
                dgv_SoftPanelList.Rows.Clear();

                string[] xmlFiles = Directory.GetFiles(SoftPanelPath, "*.xml");

                foreach (string file in xmlFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file); // 只顯示檔名，不含副檔名
                    dgv_SoftPanelList.Rows.Add(fileName, file); // 加入一列，顯示名稱 + 路徑
                }
            }
        }

        private void pic_SoftPanel_Click(object sender, EventArgs e)
        {
            TC_Main.SelectedTab = tab_SoftPanel;

            PrevPage.Push(tab_SoftPanel);
            btn_Prev.Visible = true;

            string SoftPanelPath = Path.Combine(Application.StartupPath, "SoftPanel");

            if (Directory.Exists(SoftPanelPath))
            {
                dgv_SoftPanelList.Rows.Clear();

                string[] xmlFiles = Directory.GetFiles(SoftPanelPath, "*.xml");

                foreach (string file in xmlFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file); // 只顯示檔名，不含副檔名
                    dgv_SoftPanelList.Rows.Add(fileName, file); // 加入一列，顯示名稱 + 路徑
                }
            }
        }

        private void pic_default_Click(object sender, EventArgs e)
        {
            Fo_DefaultProcess form = new Fo_DefaultProcess();
            form.TopLevel = false;
            form.Parent = Units.Fo_Main;
            form.Left = 0;
            form.Top = 0;
            form.Show();
            form.BringToFront();
        }

        XmlDocument xmlMacroDoc = new XmlDocument();

        private void pic_MacroLimit_Click(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + "\\macro.xml";
            if (!File.Exists(filename)) return;
            xmlMacroDoc.Load(filename);

            dgv_MacroLimit.Rows.Clear();

            XmlElement root = xmlMacroDoc.DocumentElement;
            if (root.Name != "MacroInfo") return;

            foreach (XmlElement x in root.ChildNodes)
            {
                int.TryParse(x.GetAttribute("No"), out int no);
                string name = x.GetAttribute("Memo");
                double.TryParse(x.GetAttribute("Max"), out double max);
                double.TryParse(x.GetAttribute("Min"), out double min);
                string Unit = x.GetAttribute("Unit");
                dgv_MacroLimit.Rows.Add(no, name, min, max, Unit, x);
            }

            TC_Main.SelectedTab = tab_MacroLimit;
            PrevPage.Push(tab_MacroLimit);
            btn_Prev.Visible = true;

        }

        private void dgv_MacroLimit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            XmlElement x = dgv_MacroLimit.CurrentRow.Cells[Col_MacroXml.Index].Value as XmlElement;
            if (x == null) return;

            if (e.ColumnIndex == Col_MacroMax.Index || e.ColumnIndex == Col_MacroMin.Index)
            {
                Fo_Num form = new Fo_Num();
                form.uc_UserNum1.la_Num.Text = dgv_MacroLimit.CurrentCell.Value.ToString();
                string name = dgv_MacroLimit.CurrentRow.Cells[Col_MacroName.Index].Value.ToString();//名稱
                name += " " + dgv_MacroLimit.Columns[e.ColumnIndex].HeaderText;//欄位
                name += "\r\n目前設定值 : " + dgv_MacroLimit.CurrentCell.Value.ToString();
                form.uc_UserNum1.la_Msg.Text = name;
                DialogResult ret = form.ShowDialog();
                if (ret != DialogResult.OK) return;

                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                dgv_MacroLimit.CurrentCell.Value = data.ToString("0.#####");
                if (e.ColumnIndex == Col_MacroMax.Index) x.SetAttribute("Max", data.ToString("0.#####"));
                else if (e.ColumnIndex == Col_MacroMin.Index) x.SetAttribute("Min", data.ToString("0.#####"));
            }
            else if (e.ColumnIndex == Col_MacroName.Index || e.ColumnIndex == Col_MacroUnit.Index)
            {
                Fo_Keyboard form = new Fo_Keyboard();
                form.TB_Input.Text = dgv_MacroLimit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    dgv_MacroLimit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = form.TB_Input.Text;
                }
                if (e.ColumnIndex == Col_MacroName.Index) x.SetAttribute("Memo", form.TB_Input.Text);
                else if (e.ColumnIndex == Col_MacroUnit.Index) x.SetAttribute("Unit", form.TB_Input.Text);
            }
        }

        private void tab_MacroLimit_Leave(object sender, EventArgs e)
        {
            xmlMacroDoc.Save(Application.StartupPath + "\\macro.xml");
        }

        private void ch_Specialopen_Click(object sender, EventArgs e) //特殊橫進刀
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteBool("System", "ch_Specialopen", ch_Specialopen.Checked);
            //int no = 3;
            //if (rb_OIG_Standard.Checked)
            //    int.TryParse(rb_OIG_Standard.Tag.ToString(), out no);
            //else if (rb_OIG_D.Checked)
            //    int.TryParse(rb_OIG_D.Tag.ToString(), out no);
            //else if (rb_OIG_DE.Checked)
            //    int.TryParse(rb_OIG_DE.Tag.ToString(), out no);
            //else
            //    return;
            //ResetProcess(no);//重設工序
            //ResetProglist();//刪除程式清單

            for (int i = 1; i <= 20; i++)
            {
                if (ch_Specialopen.Checked)//開啟多段功能
                {
                    int id = ini.ReadInteger("UI", "ProcessTag" + i.ToString(), 0);
                    if (id == 2) ini.WriteInteger("UI", "ProcessTag" + i.ToString(), 10);//恆進刀(特殊)
                }
                else//關閉多段功能
                {
                    int id = ini.ReadInteger("UI", "ProcessTag" + i.ToString(), 0);
                    if (id == 10) ini.WriteInteger("UI", "ProcessTag" + i.ToString(), 2);//恆進刀(標準)
                }
            }
        }


        private void Ch_OptionClick(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteBool("System", cb.Tag.ToString(), cb.Checked);//量測功能 右側修整功能
            switch (cb.Tag.ToString())
            {
                case "Measopen":
                    Measopen = cb.Checked;
                    break;
                case "Rightopen":
                    Rightopen = cb.Checked;
                    break;
                case "GapOpen":
                    GapOpen = cb.Checked;
                    break;

            }
            SetMonitorBtns();
        }


        private void ch_DGW_Conv_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteBool("System", "DressGwConv", ch_DGW_Conv.Checked);
        }

        private void ch_DWP_Conv_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteBool("System", "DressWorkpieceConv", ch_DWP_Conv.Checked);
        }

        private void btn_PosSet_ChangePartPos_Click(object sender, EventArgs e)
        {
            tc_PositionSet.SelectedTab = tab_PosSet_ChangePartPos;
        }

        private void btn_PosSet_IDCenterPos_Click(object sender, EventArgs e)
        {
            btn_WorkCenterPosByID.Lamp = true;
            btn_WorkCenterPosByOD.Lamp = false;
            pic_IDCenterPos_Gw1_X1.Visible = true;
            pic_IDCenterPos_Gw1_X2.Visible = true;
            pic_ODCenterPos_Gw1_X1.Visible = false;
            pic_ODCenterPos_Gw1_X2.Visible = false;
            tc_PositionSet.SelectedTab = tab_PosSet_IDCenterPos;
            //if (MachType == MachineType.OIG_R)
            {
                Label[] buf1 = { la_PosSetMach1, la_PosSetMach2, la_PosSetMach3, la_PosSetMach4, la_PosSetMach5, la_PosSetMach6 };
                Label[] buf2 = { la_PosSetMach_1, la_PosSetMach_2, la_PosSetMach_3, la_PosSetMach_4, la_PosSetMach_5, la_PosSetMach_6 };
                for (int i = 0; i < buf1.Length; i++)
                {
                    if (buf1[i].Text == "B")
                    {
                        double.TryParse(buf2[i].Text, out double pos);
                        if (pos != 0) Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 155, "B軸請先回到0度"));
                    }
                }
            }
        }
        private void btn_PosSet_RetractSafePos_Click(object sender, EventArgs e)
        {
            tc_PositionSet.SelectedTab = tab_PosSet_ODSafePos;
        }

        private void btn_OD_PosX_Click(object sender, EventArgs e)
        {
            if (Pos.Machine.Length > 0) tb_Gw2_OD_SafePosX.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bSafePos = true;
        }

        private void tc_PositionSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_PosSet_ChangePartPos.Lamp = tc_PositionSet.SelectedTab == tab_PosSet_ChangePartPos;
            btn_PosSet_IDCenterPos.Lamp = tc_PositionSet.SelectedTab == tab_PosSet_IDCenterPos;
            btn_PosSet_ODSafePos.Lamp = tc_PositionSet.SelectedTab == tab_PosSet_ODSafePos;

            btn_PosSet_TowerSafePos.Lamp = tc_PositionSet.SelectedTab == tab_PosSet_TowerSafePos;
            btn_PosSet_IDRevSafePos.Lamp = tc_PositionSet.SelectedTab == tab_PosSet_IDRevSafePos;
            btn_PosSet_DressBaseMaxMin.Lamp = tc_PositionSet.SelectedTab == tab_PosSet_DressMaxMinValue;

        }

        private void tab_EditSP_Leave(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + "\\OptionParam.xml";
            EditDoc.Save(filename);
        }

        private void tb_RotationCenter_Param_XC_TextChanged(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("RotationCenter", "XC", tb_RotationCenter_Param_XC.Text);
        }

        private void tb_RotationCenter_Param_ZC_TextChanged(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteString("RotationCenter", "ZC", tb_RotationCenter_Param_ZC.Text);
        }

        private void btn_PosSetSave_Click(object sender, EventArgs e)
        {
            bPosSetSave = false;
            btn_PosSetSave.Enabled = false;

            bool bFinish = false;
            double no = 0;

            Actions.Enqueue(new Action(() =>
            {
                focas.ReadMacro(506, out no);//讀取目前砂輪號
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

            int GwNo = (int)Math.Round(no);
            int shift = (GwNo - 1) * 200;
            if (GwNo < 1 || GwNo > 4) //例外處理
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 164, "砂輪號錯誤"));
                return;
            }

            if (bChangePart)
            {
                bChangePart = false;
                //換料位置X
                if (Double.TryParse(TB_ChangePartPosX.Text, out double SafePosX))
                {
                    if (bInchTrans) SafePosX *= 25.4;
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.Param_WriteDouble(1241, SafePosX, 1);
                    }));
                }

                //換料位置Z
                if (Double.TryParse(TB_ChangePartPosZ.Text, out double SafePosZ))
                {
                    if (bInchTrans) SafePosZ *= 25.4;
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.Param_WriteDouble(1241, SafePosZ, 2);
                    }));
                }
            }

            if (bIDCenter) //內圓中心位置有變更
            {
                bIDCenter = false;

                //GW1 內圓工件中心位置
                if (TB_Gw1PosX1.Text != "" && TB_Gw1PosX2.Text != "")
                {
                    //用兩個位置計算中心位置
                    double.TryParse(TB_Gw1PosX1.Text, out double x1);
                    double.TryParse(TB_Gw1PosX2.Text, out double x2);
                    double GWCenterPos = (x1 + x2) / 2.0;
                    la_PartCenterPosX.Text = GWCenterPos.ToString(Units.DisplayFmt);



                    Actions.Enqueue(new Action(() =>
                    {
                        if (bInchTrans)//目前是英制
                        {
                            focas.WriteMacro(10090 + shift, GWCenterPos / 25.4);//GW1 內圓工件中心位置(公制)
                            focas.WriteMacro(10091 + shift, GWCenterPos);//GW1 內圓工件中心位置(英制)
                        }
                        else //目前是公制
                        {
                            focas.WriteMacro(10090 + shift, GWCenterPos);//GW1 內圓工件中心位置(公制)
                            focas.WriteMacro(10091 + shift, GWCenterPos / 25.4);//GW1 內圓工件中心位置(英制)
                        }
                    }));
                }

            }

            if (bSafePos)
            {
                bSafePos = false;

                //GW2 外圓安全位置
                if (tb_Gw2_OD_SafePosX.Text != "")
                {
                    double.TryParse(tb_Gw2_OD_SafePosX.Text, out double x);

                    Actions.Enqueue(new Action(() =>
                    {
                        if (bInchTrans)//目前是英制
                        {
                            focas.WriteMacro(10092 + shift, x / 25.4);//GW1 外圓安全位置(公制)
                            focas.WriteMacro(10093 + shift, x);//GW1 外圓安全位置(英制)
                        }
                        else //目前是公制
                        {
                            focas.WriteMacro(10092 + shift, x);//GW1 外圓安全位置(公制)
                            focas.WriteMacro(10093 + shift, x / 25.4);//GW1 外圓安全位置(英制)
                        }
                    }));
                }

            }
            
            if (bTowerSafePos)
            {
                bTowerSafePos = false;
                //轉頭安全位置X
                if (Double.TryParse(TB_TowerSafePosX.Text, out double TowerSafePosX))
                {
                    if (bInchTrans) TowerSafePosX *= 25.4;
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.Param_WriteDouble(1243, TowerSafePosX, 1);
                    }));
                }

                //轉頭安全位置Z
                if (Double.TryParse(TB_TowerSafePosZ.Text, out double TowerSafePosZ))
                {
                    if (bInchTrans) TowerSafePosZ *= 25.4;
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.Param_WriteDouble(1243, TowerSafePosZ, 2);
                    }));
                }
            }

            if (bIDRevSafePos)
            {
                bIDRevSafePos = false;
                // 內圓修砂反向位置Z1
                if (Double.TryParse(TB_ID_DressRevZ1.Text, out double M564))
                {
                   
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.WriteMacro(564, M564);
                    }));
                }

                // 內圓修砂反向位置Z1
                if (Double.TryParse(TB_ID_DressRevZ2.Text, out double M565))
                {
                    
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.WriteMacro(565, M565);
                    }));
                }
            }

            if (bDressMaxMinValue)
            {
                bDressMaxMinValue = false;
                //修整座上下最大值
                if (Double.TryParse(TB_DressBaseMax.Text, out double dressBaseMax))
                {
                    if (bInchTrans) dressBaseMax *= 25.4;
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.Param_WriteDouble(6934, dressBaseMax, 0);
                    }));
                }

                //修整座上下最小值
                if (Double.TryParse(TB_DressBaseMin.Text, out double dressBaseMin))
                {
                    if (bInchTrans) dressBaseMin *= 25.4;
                    Actions.Enqueue(new Action(() =>
                    {
                        focas.Param_WriteDouble(6954, dressBaseMin, 0);
                    }));
                }
            }
        }

        private void tb_ChangePart_Click(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);

                bPosSetSave = true;
                btn_PosSetSave.Enabled = true;
                bChangePart = true;
            }
        }

        private void tab_PosSet_Leave(object sender, EventArgs e)
        {
            if (bPosSetSave)
            {
                string msg = LanguageManager.LoadMessage(Units.langfile, "Message", 145, "是否要存檔?");
                string title = LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告");
                DialogResult ret = Fo_Msg.Show(msg, title, MessageBoxButtons.YesNo);
                if (ret == DialogResult.Yes) btn_PosSetSave.PerformClick();
                btn_PosSetSave.Enabled = bPosSetSave = false;
            }
        }

        private void tb_ChangePart_KeyPress(object sender, KeyPressEventArgs e)
        {
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bChangePart = true;
        }

        private void tb_IDCenter_Click(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            Fo_Num form = new Fo_Num();
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);

                if (TB_Gw1PosX1.Text != "" && TB_Gw1PosX2.Text != "")
                {
                    bPosSetSave = true;
                    btn_PosSetSave.Enabled = true;
                    bIDCenter = true;
                }
            }
        }

        private void tb_IDCenter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (TB_Gw1PosX1.Text != "" && TB_Gw1PosX2.Text != "")
            {
                bPosSetSave = true;
                btn_PosSetSave.Enabled = true;
                bIDCenter = true;
            }
        }

        private void tb_Gw2_OD_SafePosX_Click(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);

                bPosSetSave = true;
                btn_PosSetSave.Enabled = true;
                bSafePos = true;
            }
        }

        private void tb_Gw2_OD_SafePosX_KeyPress(object sender, KeyPressEventArgs e)
        {
            bPosSetSave = true;
            btn_PosSetSave.Enabled = true;
            bSafePos = true;
        }

        private void btn_MeasureList_Click(object sender, EventArgs e)
        {
            TC_Main.SelectedTab = tab_MeasureList;

            PrevPage.Push(tab_MeasureList);
            pa_SoftPanel.Visible = false;
            btn_Prev.Visible = true;

            Application.DoEvents();

            dgv_MeasureList.Rows.Clear();
            if (CurrentProgram != null) EditProgram(CurrentProgram);
            ShowMeasureList();
        }

        private void ShowMeasureList()
        {
            if (TempProgram == null) return;
            int process_count = TempProgram.Processes.Count;
            //讀取數據
            Actions.Enqueue(new Action(() =>
            {
                for (int i = 0; i < process_count; i++)
                {
                    TProcess process = TempProgram.Processes[i];

                    //例外處理
                    if (process == null) continue;
                    if (process.SubPrograms.Count == 0) continue;

                    //非量測工序跳過
                    if (process.SubPrograms[0].ProgNo != 9028 &&
                        process.SubPrograms[0].ProgNo != 9040 &&
                        process.SubPrograms[0].ProgNo != 9041)
                    {
                        continue;
                    }


                    focas.ReadMacro(11000 + i, out double val1);//量測1
                    focas.ReadMacro(11030 + i, out double val2);//量測2
                    focas.ReadMacro(11060 + i, out double val3);//量測3

                    //工序號, 名稱, 形式(停用), 量測1, 量測2, 量測3
                    this.Invoke(new Action(() =>
                    {
                        dgv_MeasureList.Rows.Add(i + 1, process.Name, "", val1.ToString(Units.DisplayFmt), val2.ToString(Units.DisplayFmt), val3.ToString(Units.DisplayFmt));
                    }));
                }
            }));
        }

        private void un_PathNum_OnBtnOkClick(object sender, EventArgs e)
        {
            if (tb_SelectTextBox != null) //有選擇欄位
            {
                double.TryParse(un_PathNum.la_Num.Text, out double v);
                tb_SelectTextBox.Text = v.ToString(Units.DisplayFmt);

                if (tb_SelectTextBox == tb_ToolR) //刀尖補償
                {
                    if (btn_EditDiamPath.Lamp)
                    {
                        dgwFile.Diam_ToolR = v;
                        CurrentGwMacro[10057] = v;//修外徑 刀尖半徑
                                                  //WriteGwMacro(CurrentEditGwNo, 10057, v);
                    }
                    else
                    {
                        dgwFile.Left_ToolR = v;
                        CurrentGwMacro[10059] = v;//修左側 刀尖半徑
                                                  //WriteGwMacro(CurrentEditGwNo, 10059, v);
                    }
                }
                else if (tb_SelectTextBox == tb_DiamOfsZ)
                {
                    dgwFile.DGWDiamOffsetZ = v;
                    CurrentGwMacro[10049] = v;
                    //WriteGwMacro(CurrentEditGwNo, 10049, v);//外徑修整Z軸補正
                }
                return; // 跳過後面路徑 DataGridView 
            }

            if (dgv_Path.CurrentCell == null) return;
            int row = dgv_Path.CurrentCell.RowIndex;
            if (row < 0) return;
            int col = dgv_Path.CurrentCell.ColumnIndex;

            //編號不能編輯
            if (col == Col_Path_No.Index) return;

            //歸零按鍵不能編輯
            if (col == Col_OfsPath_Zero.Index) return;

            //數值
            double.TryParse(un_PathNum.la_Num.Text, out double val);

            #region 閃一下讓使用者感覺到有輸入
            DataGridViewCell cell = dgv_Path.CurrentCell;
            new Thread(() =>
            {
                this.Invoke(new Action(() =>
                {
                    dgv_Path.CurrentCell = null;
                }));
                Thread.Sleep(250);
                this.Invoke(new Action(() =>
                {
                    dgv_Path.CurrentCell = cell;
                }));
            }).Start();
            #endregion

            //只有 Type 是整數 其他輸入值為浮點數
            dgv_Path.CurrentCell.Value = (col == Col_Path_Type.Index) ? val.ToString("0") : val.ToString(Units.DisplayFmt);

            //
            List<DGWData> list = null;
            Color color = Color.Black;
            double feed = 0;
            PathOrigin dir = PathOrigin.Left;
            if (btn_EditLeftPath.Lamp) //編輯左側
            {
                list = dgwFile.LeftList;
                if (row >= list.Count) return; //例外處理
                color = Color.Aqua;
                feed = list[row].Feed;
                if (feed == 0) //例外處理 數值零預設給 修整條件的速度
                {
                    feed = CurrentGwMacro[10025];
                    list[row].Feed = feed;
                }
            }
            else if (btn_EditDiamPath.Lamp) //編輯外徑
            {
                list = dgwFile.DiamList;
                if (row >= list.Count) return; //例外處理
                color = Color.Lime;
                feed = list[row].Feed;
                if (feed == 0) //例外處理 數值零預設給 修整條件的速度
                {
                    feed = CurrentGwMacro[10017];
                    list[row].Feed = feed;
                }
            }
            else if (btn_EditRightPath.Lamp) //編輯右側(預留)
            {
                list = dgwFile.RightList;
                if (row >= list.Count) return; //例外處理
                color = Color.Yellow;
                feed = list[row].Feed;
                if (feed == 0) //例外處理 數值零預設給 修整條件的速度
                {
                    feed = CurrentGwMacro[10033];
                    list[row].Feed = feed;
                }
                dir = PathOrigin.Right;
            }
            if (list == null) return; //例外處理

            if (col == Col_Path_Type.Index)//指令Type
            {
                int type = (int)val;
                list[row].Type = type;

                //Type 0 (G00) 顯示 RAPID, 其他 (G01, G02, G03)顯示修整速度
                dgv_Path.CurrentRow.Cells[Col_Path_Speed.Index].Value = (type == 0) ? "RAPID" : feed.ToString(Units.DisplayFmt);
                //Type 2, 3 (G02, G03) 才顯示 R的數值
                dgv_Path.CurrentRow.Cells[Col_Path_R.Index].Value = (type != 2 && type != 3) ? "" : list[row].R.ToString(Units.DisplayFmt);
            }
            else if (col == Col_Path_X.Index) list[row].X = val; //座標X
            else if (col == Col_Path_Z.Index) list[row].Z = val; //座標Z
            else if (col == Col_Path_R.Index) //半徑R
            {
                list[row].R = val;
                //如果輸入R 時, 不是 Type2 或 Type3 就自動轉為 Type2
                if (list[row].Type != 2 && list[row].Type != 3)
                {
                    list[row].Type = 2;
                    dgv_Path.CurrentRow.Cells[Col_Path_Type.Index].Value = "2";
                    //顯示修整速度(避免還是RAPID)
                    dgv_Path.CurrentRow.Cells[Col_Path_Speed.Index].Value = list[row].Feed.ToString(Units.DisplayFmt);
                }
            }
            else if (col == Col_Path_Speed.Index) //Feed
            {
                list[row].Feed = val;
                //如果原本是RAPID, 將Type 自動更改為 Type1 (G01)
                if (list[row].Type == 0)
                {
                    list[row].Type = 1;
                    dgv_Path.CurrentRow.Cells[Col_Path_Type.Index].Value = "1";
                }
            }
            else if (col == Col_OfsPath_X.Index) list[row].OffsetX = val; //補正X
            else if (col == Col_OfsPath_Z.Index) list[row].OffsetZ = val; //補正Z

            DrawPath(list, color, dir, pic_EditPath, true, row);

            //dgwFile.SaveToFile(Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml");

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;

        }

        private void btn_Path_Delete_Click(object sender, MouseEventArgs e)
        {
            if (dgv_Path.CurrentRow == null) return;
            int index = dgv_Path.CurrentRow.Index;
            if (index < 0) return;

            if (btn_EditLeftPath.Lamp)
            {
                dgwFile.LeftList.RemoveAt(index);
                DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_EditPath, true, -1);
            }
            else if (btn_EditDiamPath.Lamp)
            {
                dgwFile.DiamList.RemoveAt(index);
                DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_EditPath, true, -1);
            }
            else if (btn_EditRightPath.Lamp)
            {
                dgwFile.RightList.RemoveAt(index);
                DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_EditPath, true, -1);
            }

            dgv_Path.Rows.RemoveAt(index);
            for (int i = 0; i < dgv_Path.Rows.Count; i++)
            {
                dgv_Path.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

            //dgwFile.SaveToFile(Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml");

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void btn_Path_NewFile_MouseDown(object sender, MouseEventArgs e)
        {
            DialogResult ret = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 157, "確認要清除成形修整資料?"),
                                            LanguageManager.LoadMessage(Units.langfile, "Message", 3, "警告"),
                                            MessageBoxButtons.YesNo);

            if (ret != DialogResult.Yes) return;

            dgv_Path.Rows.Clear();

            dgwFile.LeftList.Clear();
            dgwFile.DiamList.Clear();
            dgwFile.RightList.Clear();

            int shift = (CurrentEditGwNo - 1) * 100;
            CurrentGwMacro[10056] = 0; //外徑刀尖半徑補償方式
            CurrentGwMacro[10057] = 0;//外徑刀尖半徑
            CurrentGwMacro[10058] = 0;//左側刀尖半徑補償方式
            CurrentGwMacro[10059] = 0;//左側刀尖半徑

            tb_DiamOfsZ.Text = Units.DisplayFmt;
            CurrentGwMacro[10049] = 0;//外徑修整Z軸補正

            cb_ToolRCompFunc.SelectedIndex = 0;
            tb_ToolR.Text = Units.DisplayFmt;

            DrawPath(null, Color.Lime, PathOrigin.Left, pic_EditPath, true, -1);

            //WinApi.SendMessage(btn_Path_NewFile.ImageHandle, WM_LBUTTONUP, 0, IntPtr.Zero);
            //dgwFile.SaveToFile(Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml");
        }

        private void btn_Path_Add_MouseDown(object sender, MouseEventArgs e)
        {
            string no = (dgv_Path.Rows.Count + 1).ToString();

            double feed = 0;


            List<DGWData> list = null;
            Color color = Color.Black;
            PathOrigin dir = PathOrigin.Left;
            if (btn_EditLeftPath.Lamp)
            {
                list = dgwFile.LeftList;
                feed = CurrentGwMacro[10025];
                color = Color.Aqua;
            }
            else if (btn_EditDiamPath.Lamp)
            {
                list = dgwFile.DiamList;
                feed = CurrentGwMacro[10017];
                color = Color.Lime;
            }
            else if (btn_EditRightPath.Lamp)
            {
                list = dgwFile.RightList;
                feed = CurrentGwMacro[10033];
                color = Color.Yellow;
                dir = PathOrigin.Right;
            }
            if (list == null) return;
            DGWData last = null;
            if (list.Count > 0) last = list.Last();//讀取最後一筆

            DGWData data = new DGWData();
            data.Type = 1;
            data.Feed = feed;
            if (last != null)
            {
                data.X = last.X;
                data.Z = last.Z;
            }
            list.Add(data);

            //No, Type, X, Z, R, Speed
            int index = dgv_Path.Rows.Add(no, "1",
                                        Units.DisplayFmt,
                                        Units.DisplayFmt, "",
                                        feed.ToString(Units.DisplayFmt),
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 156, "歸零"));
            dgv_Path.Rows[index].Height = 48;

            DrawPath(list, color, dir, pic_EditPath, true, index);



            //vsb_Path.Maximum = Math.Max(0, dgv_Path.Rows.Count - 1);
            //vsb_Path.LargeChange = dgv_Path.DisplayedRowCount(false);

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void btn_Path_InsertFront_MouseDown(object sender, MouseEventArgs e)
        {
            if (dgv_Path.CurrentRow == null)
            {
                btn_Path_Add.PerformClick();
                return;
            }
            int index = dgv_Path.CurrentRow.Index;


            double feed = 0;
            List<DGWData> list = null;
            Color color = Color.Black;
            DGWData data = new DGWData();
            data.Type = 1;
            PathOrigin dir = PathOrigin.Left;
            if (btn_EditLeftPath.Lamp)
            {
                feed = CurrentGwMacro[10025];
                data.Feed = feed;
                list = dgwFile.LeftList;
                color = Color.Aqua;
            }
            else if (btn_EditDiamPath.Lamp)
            {
                feed = CurrentGwMacro[10017];
                data.Feed = feed;
                list = dgwFile.DiamList;
                color = Color.Lime;
            }
            else if (btn_EditRightPath.Lamp)
            {
                feed = CurrentGwMacro[10033];
                data.Feed = feed;
                list = dgwFile.RightList;
                color = Color.Yellow;
                dir = PathOrigin.Right;
            }
            //No, Type, X, Z, R, Speed
            dgv_Path.Rows.Insert(index, "",
                                        "1",
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        "",
                                        feed.ToString(Units.DisplayFmt),
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 156, "歸零"));
            dgv_Path.Rows[index].Height = 48;
            for (int i = 0; i < dgv_Path.Rows.Count; i++)
            {
                dgv_Path.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

            list.Insert(index, data);
            DrawPath(list, color, dir, pic_EditPath, true, index);

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void btn_Path_InsertBack_MouseDown(object sender, MouseEventArgs e)
        {
            if (dgv_Path.CurrentRow == null)
            {
                btn_Path_Add.PerformClick();
                return;
            }

            if (dgv_Path.CurrentRow.Index == dgv_Path.Rows.Count - 1)
            {
                btn_Path_Add.PerformClick();
                return;
            }

            int index = dgv_Path.CurrentRow.Index + 1;

            double feed = 0;
            List<DGWData> list = null;
            Color color = Color.Black;
            DGWData data = new DGWData();
            data.Type = 1;
            PathOrigin dir = PathOrigin.Left;
            if (btn_EditLeftPath.Lamp)
            {
                feed = CurrentGwMacro[10025];
                data.Feed = feed;
                list = dgwFile.LeftList;
                color = Color.Aqua;
            }
            else if (btn_EditDiamPath.Lamp)
            {
                feed = CurrentGwMacro[10017];
                data.Feed = feed;
                list = dgwFile.DiamList;
                color = Color.Lime;
            }
            else if (btn_EditRightPath.Lamp)
            {
                feed = CurrentGwMacro[10033];
                data.Feed = feed;
                list = dgwFile.RightList;
                color = Color.Yellow;
                dir = PathOrigin.Right;
            }
            //No, Type, X, Z, R, Feed
            dgv_Path.Rows.Insert(index, "",
                                        "1",
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        "",
                                        feed.ToString(Units.DisplayFmt),
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        Units.DisplayFmt,
                                        LanguageManager.LoadMessage(Units.langfile, "Message", 156, "歸零"));
            dgv_Path.Rows[index].Height = 48;
            for (int i = 0; i < dgv_Path.Rows.Count; i++)
            {
                dgv_Path.Rows[i].Cells[0].Value = (i + 1).ToString();
            }
            list.Insert(index, data);
            DrawPath(list, color, dir, pic_EditPath, true, index);

            //dgwFile.SaveToFile(Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml");

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void SetPathLayout()
        {
            //====補正 + 外徑====
            //成行修整 - 外徑 - Z軸補正
            tb_DiamOfsZ.Visible = la_DiamOfsZ.Visible = (btn_OffsetPath.Lamp && btn_EditDiamPath.Lamp);

            //====補正====
            //清除所有補正值
            btn_ClearAllOffsetPath.Visible = btn_OffsetPath.Lamp;
            la_ToolRCompFunc.Visible = cb_ToolRCompFunc.Visible = btn_OffsetPath.Lamp;
            la_ToolR.Visible = tb_ToolR.Visible = btn_OffsetPath.Lamp;

            //====編輯====
            btn_Path_Add.Visible = btn_EditPath.Lamp;
            btn_Path_InsertFront.Visible = btn_EditPath.Lamp;
            btn_Path_InsertBack.Visible = btn_EditPath.Lamp;
            btn_Path_Delete.Visible = btn_EditPath.Lamp;
            btn_Path_NewFile.Visible = btn_EditPath.Lamp;
        }

        private void btn_OffsetPath_MouseDown(object sender, MouseEventArgs e)
        {
            Col_OfsPath_X.Visible = true;
            Col_OfsPath_Z.Visible = true;
            Col_OfsPath_Zero.Visible = true;
            Col_Path_X.Visible = false;
            Col_Path_Z.Visible = false;
            Col_Path_R.Visible = false;
            Col_Path_Speed.Visible = false;

            btn_EditPath.Lamp = false;
            btn_OffsetPath.Lamp = true;

            un_PathNum.la_Num.Text = "";
            un_PathNum.la_Msg.Text = "";

            tb_DiamOfsZ.BackColor = Color.White;
            tb_DiamOfsZ.ForeColor = Color.Black;
            tb_ToolR.BackColor = Color.White;
            tb_ToolR.ForeColor = Color.Black;

            SetPathLayout();

            int index = -1;
            if (dgv_Path.CurrentCell != null) index = dgv_Path.CurrentCell.RowIndex;

            if (btn_EditLeftPath.Lamp) DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_EditPath, true, index);
            if (btn_EditDiamPath.Lamp) DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_EditPath, true, index);
            if (btn_EditRightPath.Lamp) DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_EditPath, true, index);

        }

        private void btn_EditPath_MouseDown(object sender, MouseEventArgs e)
        {
            dgv_Path.CurrentCell = null;

            Col_OfsPath_X.Visible = false;
            Col_OfsPath_Z.Visible = false;
            Col_OfsPath_Zero.Visible = false;
            Col_Path_X.Visible = true;
            Col_Path_Z.Visible = true;
            Col_Path_R.Visible = true;
            Col_Path_Speed.Visible = true;

            btn_EditPath.Lamp = true;
            btn_OffsetPath.Lamp = false;

            un_PathNum.la_Num.Text = "";
            un_PathNum.la_Msg.Text = "";

            tb_DiamOfsZ.BackColor = Color.White;
            tb_DiamOfsZ.ForeColor = Color.Black;
            tb_ToolR.BackColor = Color.White;
            tb_ToolR.ForeColor = Color.Black;

            SetPathLayout();

            int index = -1;
            if (dgv_Path.CurrentCell != null) index = dgv_Path.CurrentCell.RowIndex;

            if (btn_EditLeftPath.Lamp) DrawPath(dgwFile.LeftList, Color.Aqua, PathOrigin.Left, pic_EditPath, true, index);
            if (btn_EditDiamPath.Lamp) DrawPath(dgwFile.DiamList, Color.Lime, PathOrigin.Left, pic_EditPath, true, index);
            if (btn_EditRightPath.Lamp) DrawPath(dgwFile.RightList, Color.Yellow, PathOrigin.Right, pic_EditPath, true, index);
        }

        private void tab_GwDb_Leave(object sender, EventArgs e)
        {
            //if (!GwSetEdit) return;
            //GwSetEdit = false;
            //DialogResult ret = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 145, "是否要儲存?"),
            //                LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
            //                MessageBoxButtons.YesNo);

            //if (ret == DialogResult.Yes)
            //{
            //    SaveGwData();
            //}
        }

        private void tab_DressGwSetting_Leave(object sender, EventArgs e)
        {
            if (!GwDressEdit) return;
            GwDressEdit = false;
            DialogResult ret = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 145, "是否要儲存?"),
                            LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
                            MessageBoxButtons.YesNo);

            if (ret == DialogResult.Yes)
            {
                SaveDressGw();
            }

        }

        private void tab_DressPartsSetting_Leave(object sender, EventArgs e)
        {
            if (!GwWorkPiEdit) return;
            GwWorkPiEdit = false;
            DialogResult ret = Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 145, "是否要儲存?"),
                            LanguageManager.LoadMessage(Units.langfile, "Message", 3, "訊息"),
                            MessageBoxButtons.YesNo);

            if (ret == DialogResult.Yes)
            {
                SaveGrinding();
            }
        }

        private void TC_Path_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_EditPath.Visible = btn_OffsetPath.Visible = TC_Path.SelectedTab == tab_EditPath;

        }

        private void btn_ClearAllOffsetPath_Click(object sender, EventArgs e)
        {
            DialogResult ret = Fo_Msg.Show("是否要全部清除", "警告", MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes) return;

            List<DGWData> list = null;
            if (btn_EditLeftPath.Lamp)
            {
                list = dgwFile.LeftList;

            }
            else if (btn_EditDiamPath.Lamp)
            {
                list = dgwFile.DiamList;
            }
            else if (btn_EditRightPath.Lamp)
            {
                list = dgwFile.RightList;
            }

            foreach (DGWData data in list)
            {
                data.OffsetX = 0;
                data.OffsetZ = 0;
            }

            //No, Type, OffsetX, OffsetZ, Zero
            for (int i = 0; i < dgv_Path.Rows.Count; i++)
            {
                dgv_Path.Rows[i].Cells[Col_OfsPath_X.Index].Value = Units.DisplayFmt;
                dgv_Path.Rows[i].Cells[Col_OfsPath_Z.Index].Value = Units.DisplayFmt;
            }

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void vsb_Path_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (dgv_Path.Rows.Count > 0)
                {
                    dgv_Path.FirstDisplayedScrollingRowIndex = e.NewValue;
                }
            }
            catch { /* 若捲到最底可能會超界，可忽略 */ }
        }

        private void dgv_Path_RowsChanged(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // 更新捲軸範圍
            vsb_Path.Maximum = Math.Max(0, dgv_Path.Rows.Count - 1);
            vsb_Path.LargeChange = dgv_Path.DisplayedRowCount(false);
        }

        private void dgv_Path_RowsChanged(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            // 更新捲軸範圍
            vsb_Path.Maximum = Math.Max(0, dgv_Path.Rows.Count - 1);
            vsb_Path.LargeChange = dgv_Path.DisplayedRowCount(false);
        }

        private TextBox tb_SelectTextBox = null;
        private void tb_DiamOfsZ_Click(object sender, EventArgs e)
        {
            dgv_Path.CurrentCell = null;

            tb_DiamOfsZ.BackColor = Color.Yellow;
            tb_ToolR.BackColor = Color.White;
            un_PathNum.la_Num.Text = tb_DiamOfsZ.Text;
            un_PathNum.la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 158, "Z軸補正");
            tb_SelectTextBox = tb_DiamOfsZ;
        }

        private void tb_ToolR_Click(object sender, EventArgs e)
        {
            dgv_Path.CurrentCell = null;

            tb_DiamOfsZ.BackColor = Color.White;
            tb_ToolR.BackColor = Color.Yellow;

            un_PathNum.la_Num.Text = tb_ToolR.Text;
            un_PathNum.la_Msg.Text = la_ToolR.Text;
            tb_SelectTextBox = tb_ToolR;
        }

        private void cb_ToolRCompFunc_Click(object sender, EventArgs e)
        {
            tb_DiamOfsZ.BackColor = Color.White;
            tb_ToolR.BackColor = Color.White;
        }

        private void cb_ToolRCompFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cb_ToolRCompFunc.SelectedIndex;
            //例外處理
            if (index < 0) return;

            if (btn_EditDiamPath.Lamp)
            {
                CurrentGwMacro[10056] = index; //外徑 刀尖半徑補償方式
                                               //WriteGwMacro(CurrentEditGwNo, 10056, index);
            }
            else
            {
                CurrentGwMacro[10058] = index;//左側 刀尖半徑補償方式
                                              //WriteGwMacro(CurrentEditGwNo, 10058, index);
            }
        }

        private void btn_SpSpeed3_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_SpSpeed.DisplayText;
            if (File.Exists(FileName)) //小鍵盤顯示物件名稱 抓txt
            {

                TIniFile tIniFile = new TIniFile(FileName);
                string name = tIniFile.ReadString("Macro Show", btn_SpSpeed.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
                form.uc_UserNum1.la_Msg.Text += "\r\n" + Spindle.MinRpm + " ~ " + Spindle.MaxRpm; //顯示上下限
            }
            var ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            int.TryParse(form.uc_UserNum1.la_Num.Text, out int val);
            if (val > Spindle.MaxRpm)
                val = Spindle.MaxRpm;
            if (val < Spindle.MinRpm)
                val = Spindle.MinRpm;

            UserSCode = val;

            //紀錄
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteFloat("Spindle", "Cmd", UserSCode);

            this.Spindle.CmdSpeed = UserSCode;
            btn_SpSpeed3.DisplayText = UserSCode.ToString();

            Actions.Enqueue(new Action(() =>
            {
                focas.PMC_WriteDbWord(PmcAddrType.D, 200, val);
            }));

        }

        private void dgv_Path_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_Path.CurrentCell == null) return;

            //將 X/Z 軸補正清除
            if (e.ColumnIndex == Col_OfsPath_Zero.Index)
            {
                dgv_Path.CurrentRow.Cells[Col_OfsPath_X.Index].Value = Units.DisplayFmt;
                dgv_Path.CurrentRow.Cells[Col_OfsPath_Z.Index].Value = Units.DisplayFmt;
                if (btn_EditLeftPath.Lamp)
                {
                    dgwFile.LeftList[e.RowIndex].OffsetX = 0;
                    dgwFile.LeftList[e.RowIndex].OffsetZ = 0;
                }
                else if (btn_EditDiamPath.Lamp)
                {
                    dgwFile.DiamList[e.RowIndex].OffsetX = 0;
                    dgwFile.DiamList[e.RowIndex].OffsetZ = 0;
                }
                else if (btn_EditRightPath.Lamp)
                {
                    dgwFile.RightList[e.RowIndex].OffsetX = 0;
                    dgwFile.RightList[e.RowIndex].OffsetZ = 0;
                }
            }

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void pa_LanguageHide_Click(object sender, EventArgs e)
        {
            if (bDevelop)
            {
                Fo_MultLangEdit form = new Fo_MultLangEdit();
                form.ShowDialog();
            }
        }

        private void btn_G55X_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G55X_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G55X.Text, out double x);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);


            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + x.ToString(Units.DisplayFmt) + "] --> [" + (x + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G55X.Text = (x + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G55Z_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G55Z_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G55Z.Text, out double z);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + z.ToString(Units.DisplayFmt) + "] --> [" + (z + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G55Z.Text = (z + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G56X_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G56X_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G56X.Text, out double x);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + x.ToString(Units.DisplayFmt) + "] --> [" + (x + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G56X.Text = (x + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G56Z_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G56Z_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G56Z.Text, out double z);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + z.ToString(Units.DisplayFmt) + "] --> [" + (z + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G56Z.Text = (z + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G58X_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G58X_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G58X.Text, out double x);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + x.ToString(Units.DisplayFmt) + "] --> [" + (x + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G58X.Text = (x + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G58Z_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G58Z_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G58Z.Text, out double z);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + z.ToString(Units.DisplayFmt) + "] --> [" + (z + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G58Z.Text = (z + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G54G59X_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G54G59X_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G54G59X.Text, out double x);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + x.ToString(Units.DisplayFmt) + "] --> [" + (x + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G54G59X.Text = (x + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G54Z_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G54Z_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G54Z.Text, out double z);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + z.ToString(Units.DisplayFmt) + "] --> [" + (z + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G54Z.Text = (z + data).ToString(Units.DisplayFmt);
            }
        }

        private void btn_G59Z_Input_Click(object sender, EventArgs e)
        {
            Fo_Num form = new Fo_Num();
            form.StartPosition = FormStartPosition.Manual;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2 + 100;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            string name = "";
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                name = tIniFile.ReadString("Macro Show", btn_G59Z_Input.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;

            }
            DialogResult ret = form.ShowDialog();
            if (ret != DialogResult.OK) return;

            //原本位置
            double.TryParse(TB_G59Z.Text, out double z);

            //+輸入的數值
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);

            //位置改變提示
            DialogResult ret2 = Fo_Msg.Show(name + "\r\n[" + z.ToString(Units.DisplayFmt) + "] --> [" + (z + data).ToString(Units.DisplayFmt) + "]", "", MessageBoxButtons.YesNo);

            if (ret2 == DialogResult.Yes)
            {
                TB_G59Z.Text = (z + data).ToString(Units.DisplayFmt);
            }
        }

        private void cb_F_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null) return;
            int index = cb.SelectedIndex;
            int val = -1;
            if (index > 0) val = index - 1;
            if (int.TryParse(cb.Tag.ToString(), out int addr))
            {
                focas.PMC_WriteByte(PmcAddrType.D, (short)addr, (byte)val);
            }
        }

        private void SW_K_OFF(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            String StrAddr = btn.Tag.ToString();

            string[] csv = StrAddr.Substring(1).Split('.');
            if (csv.Length != 2) return;
            int addr = int.Parse(csv[0]);
            int bit = int.Parse(csv[1]);

            focas.PMC_ReadByte(PmcAddrType.K, (ushort)addr, out byte tmp);
            tmp = tmp.SetBit(bit, false);
            focas.PMC_WriteByte(PmcAddrType.K, (short)addr, tmp);
        }

        private void SW_K_ON(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            String StrAddr = btn.Tag.ToString();

            string[] csv = StrAddr.Substring(1).Split('.');
            if (csv.Length != 2) return;
            int addr = int.Parse(csv[0]);
            int bit = int.Parse(csv[1]);

            focas.PMC_ReadByte(PmcAddrType.K, (ushort)addr, out byte tmp);
            tmp = tmp.SetBit(bit, true);
            focas.PMC_WriteByte(PmcAddrType.K, (short)addr, tmp);
        }

        private void pic_DIY_Panel_Click(object sender, EventArgs e)
        {
            TC_Main.SelectedTab = tab_DIY_Panel;
            Application.DoEvents();

            ComboBox[] cbs = { cb_F1, cb_F2, cb_F3, cb_F4, cb_F5, cb_F6, cb_F7, cb_F8, cb_F9, cb_F10, cb_F11, cb_F12, cb_F13, cb_F14, cb_F15 };
            foreach (ComboBox cb in cbs)
            {

                if (int.TryParse(cb.Tag.ToString(), out int addr))
                {
                    focas.PMC_ReadByte(PmcAddrType.D, (ushort)addr, out byte val);
                    int index = (sbyte)val;
                    index += 1;
                    if (index > (cb.Items.Count - 1)) index = 0;
                    cb.SelectedIndex = index;
                }

            }

            PrevPage.Push(tab_DIY_Panel);
            btn_Prev.Visible = true;
        }

        private void btn_ToolSelect_Click(object sender, EventArgs e)
        {
            pa_SoftPanel.Hide();
            Fo_ToolSelect form = new Fo_ToolSelect();
            form.ShowDialog();
        }

        private void pa_Gw_S8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pa_Gw_S6_Paint(object sender, PaintEventArgs e)
        {

        }


        private void un_Gw_GwData_OnBtnOkClick(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = un_Gw_GwData.Tag as Uc_RoundBtn;
            if (btn == null) return;//例外處理
            int.TryParse(btn.Tag.ToString(), out int no);// Macro Number

            double.TryParse(un_Gw_GwData.la_Num.Text, out double data);//取得數值

            Units.MacroInfo.CheckMacroMinMax(no, ref data); //檢查上下限並修正, 沒問題會回傳 true

            btn.DisplayText = data.ToString(Units.DisplayFmt);//更新欄位數值(浮點數型)
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            if (btn.Name.Contains("GWAngle"))
            {
                GwMarcoOffset = (CurrentEditGwNo - 1) * 2;
                if(GWType[CurrentEditGwNo - 1] == MachineType.OCD3)
                {
                    data = data * -1;
                }
                Action_WriteMacro(no + GwMarcoOffset, data);           
            }
            else
            {           
                CurrentGwMacro[no + GwMarcoOffset] = data;//寫回暫存
                WriteGwMacro(CurrentEditGwNo, no, data);
            }
        }
        private void un_Gw_Condition_OnBtnOkClick(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = un_Gw_Condition.Tag as Uc_RoundBtn;
            if (btn == null) return;//例外處理
            if (btn.Tag == null) return;//例外處理
            int.TryParse(btn.Tag.ToString(), out int no);// Macro Number

            double.TryParse(un_Gw_Condition.la_Num.Text, out double data);//取得數值

            Units.MacroInfo.CheckMacroMinMax(no, ref data); //檢查上下限並修正, 沒問題會回傳 true


            if (btn == btn_GWDressTimes || btn == btn_GWAirDress) //這兩個是次數
            {
                btn.DisplayText = data.ToString("0");//更新欄位數值(整數型)
                un_Gw_Condition.la_Num.Text = data.ToString("0");
            }
            else //其他是尺寸
            {
                btn.DisplayText = data.ToString(Units.DisplayFmt);//更新欄位數值(浮點數型)
                un_Gw_Condition.la_Num.Text = data.ToString(Units.DisplayFmt);
            }
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            CurrentGwMacro[no + GwMarcoOffset] = data;//寫回暫存
            WriteGwMacro(CurrentEditGwNo, no, data);
        }
        private void TC_GW_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_Gw_GwData.Lamp = TC_GW.SelectedTab == tab_Gw_GwData;
            btn_Gw_ShapeSelect.Lamp = TC_GW.SelectedTab == tab_Gw_ShapeSelect;
            btn_Gw_ShapeData.Lamp = TC_GW.SelectedTab == tab_Gw_ShapeData || TC_GW.SelectedTab == tab_Gw_ShapeEdit;
            btn_Gw_DressCondition.Lamp = TC_GW.SelectedTab == tab_Gw_DressCondition;

            btn_Gw_GwData.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;
            btn_Gw_ShapeSelect.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;
            btn_Gw_ShapeData.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;
            btn_Gw_DressCondition.Visible = TC_Main.SelectedTab == tab_GwDb && TC_GW.SelectedTab != tab_Gw_GwSelect;

        }

        private void btn_Gw_ShapeSelect_Click(object sender, EventArgs e)
        {
            TC_GW.SelectedTab = tab_Gw_ShapeSelect;
        }

        private void btn_Gw_DressCondition_Click(object sender, EventArgs e)
        {
            TC_GW.SelectedTab = tab_Gw_DressCondition;
        }

        private void btn_Path_Save_Click(object sender, EventArgs e)
        {
            dgwFile.SaveToFile(Application.StartupPath + "\\GW" + CurrentEditGwNo + ".xml");

            int shift = (CurrentEditGwNo - 1) * 200;

            int mode = 0;
            if (dgwFile.LeftList.Count > 0) mode |= 1;//成形模式 - 包含左側修整
            if (dgwFile.DiamList.Count > 0) mode |= 2;//成形模式 - 包含外徑修整
            if (dgwFile.RightList.Count > 0) mode |= 4;//成形模式 - 包含右側修整(預留)

            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(10048 + shift, mode);//成形模式
                focas.WriteMacro(10049 + shift, CurrentGwMacro[10049 + shift]);//成形外徑修整Z軸補正
                focas.WriteMacro(10050 + shift, dgwFile.LeftList.Count > 0 ? dgwFile.LeftList[0].X * 2 : 0);//左側起始位置X
                focas.WriteMacro(10051 + shift, dgwFile.LeftList.Count > 0 ? dgwFile.LeftList[0].Z : 0);//左側起始位置Z
                focas.WriteMacro(10052 + shift, dgwFile.DiamList.Count > 0 ? dgwFile.DiamList[0].X * 2 : 0);//外徑起始位置X
                focas.WriteMacro(10053 + shift, dgwFile.DiamList.Count > 0 ? dgwFile.DiamList[0].Z : 0);//外徑起始位置Z
                focas.WriteMacro(10054 + shift, dgwFile.RightList.Count > 0 ? dgwFile.RightList[0].X * 2 : 0);//右側起始位置X
                focas.WriteMacro(10055 + shift, dgwFile.RightList.Count > 0 ? dgwFile.RightList[0].Z : 0);//右側起始位置Z
                focas.WriteMacro(10056 + shift, CurrentGwMacro[10056 + shift]);//外徑刀尖功能
                focas.WriteMacro(10057 + shift, CurrentGwMacro[10057 + shift]);//外徑刀尖半徑
                focas.WriteMacro(10058 + shift, CurrentGwMacro[10058 + shift]);//左側刀尖功能
                focas.WriteMacro(10059 + shift, CurrentGwMacro[10059 + shift]);//左側刀尖半徑           

            }));
            WritePath();//將路徑寫到控制器 GW1:O8002, GW2:O8003, GW3:O8004
        }


        private void btn_DressGwNoOffset_MouseDown(object sender, MouseEventArgs e)
        {
            if (un_Gw_Condition.Tag != null) ((Uc_RoundBtn)un_Gw_Condition.Tag).Lamp = false;
            un_Gw_Condition.Tag = btn_DressGwNoOffset;
            un_Gw_Condition.la_Msg.Text = "";
            un_Gw_Condition.la_Num.Text = "";
            btn_DressGwNoOffset.Lamp = true;
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            if (CurrentGwMacro[10073 + GwMarcoOffset] == 0)//補正 -> 不補正
            {
                CurrentGwMacro[10073 + GwMarcoOffset] = 1;
                btn_DressGwNoOffset.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", 1011, "不補正");
                WriteGwMacro(CurrentEditGwNo, 10073, 1);

            }
            else //不補正 -> 補正
            {
                CurrentGwMacro[10073 + GwMarcoOffset] = 0;
                btn_DressGwNoOffset.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", 1010, "補正");
                WriteGwMacro(CurrentEditGwNo, 10073, 0);

            }
        }

        private void btn_AfterMachining_MouseDown(object sender, MouseEventArgs e)
        {
            if (un_Gw_Condition.Tag != null) ((Uc_RoundBtn)un_Gw_Condition.Tag).Lamp = false;
            un_Gw_Condition.Tag = btn_AfterMachining;
            un_Gw_Condition.la_Msg.Text = "";
            un_Gw_Condition.la_Num.Text = "";
            btn_AfterMachining.Lamp = true;
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            if (CurrentGwMacro[10013 + GwMarcoOffset] == 0)//不停止 -> 停止
            {
                CurrentGwMacro[10013 + GwMarcoOffset] = 1;
                btn_AfterMachining.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", 1003, "停止");
                WriteGwMacro(CurrentEditGwNo, 10013, 1);

            }
            else //停止 -> 不停止
            {
                CurrentGwMacro[10013 + GwMarcoOffset] = 0;
                btn_AfterMachining.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", 1002, "不停止");
                WriteGwMacro(CurrentEditGwNo, 10013, 0);

            }
        }

        private void btn_RollerRotation_MouseDown(object sender, MouseEventArgs e)
        {
            if (un_Gw_Condition.Tag != null) ((Uc_RoundBtn)un_Gw_Condition.Tag).Lamp = false;
            un_Gw_Condition.Tag = btn_RollerRotation;
            un_Gw_Condition.la_Msg.Text = "";
            un_Gw_Condition.la_Num.Text = "";
            btn_RollerRotation.Lamp = true;
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            if (CurrentGwMacro[10072 + GwMarcoOffset] == 0)//正轉 -> 反轉
            {
                CurrentGwMacro[10072 + GwMarcoOffset] = 1;
                btn_RollerRotation.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", 1009, "反轉");
                WriteGwMacro(CurrentEditGwNo, 10072, 1);

            }
            else //反轉 -> 正轉
            {
                CurrentGwMacro[10072 + GwMarcoOffset] = 0;
                btn_RollerRotation.DisplayText = LanguageManager.LoadMessage(Units.langfile, "MaintainParam", 1008, "正轉");
                WriteGwMacro(CurrentEditGwNo, 10072, 0);

            }
        }

        private void btn_ToAndBack_MouseDown(object sender, MouseEventArgs e)
        {
            if (un_Gw_Condition.Tag != null) ((Uc_RoundBtn)un_Gw_Condition.Tag).Lamp = false;
            un_Gw_Condition.Tag = btn_ToAndBack;
            un_Gw_Condition.la_Msg.Text = "";
            un_Gw_Condition.la_Num.Text = "";
            btn_ToAndBack.Lamp = true;
            int GwMarcoOffset = (CurrentEditGwNo - 1) * 200;
            if (CurrentGwMacro[10020 + GwMarcoOffset] == 1) //目前是使用
            {
                CurrentGwMacro[10020 + GwMarcoOffset] = 0; //不使用
                btn_ToAndBack.DisplayText = "OFF";
                WriteGwMacro(CurrentEditGwNo, 10020, 0);

            }
            else //目前不使用
            {
                CurrentGwMacro[10020 + GwMarcoOffset] = 1; //使用
                btn_ToAndBack.DisplayText = "ON";
                WriteGwMacro(CurrentEditGwNo, 10020, 1);

            }

            //btn_RegisterGw_Save.Visible = true;
            //GwSetEdit = true;
        }

        private void btn_SaveProg_VisibleChanged(object sender, EventArgs e)
        {
            //Col_ProcList_Btn.Visible = !btn_SaveProg.Visible;
        }

        private void btn_Gw_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = sender as Uc_RoundBtn;
            if (btn == null) return;//例外處理
            if (btn.Tag == null) return;//例外處理
            btn.Lamp = true;

            int.TryParse(btn.Tag.ToString(), out int no);//tag 存放對應的Macro 位址,

            Uc_RoundBtn btn_tag = un_Gw_GwData.Tag as Uc_RoundBtn;
            if (btn_tag != null)
            {
                if (btn_tag != btn) btn_tag.Lamp = false;
                
            }

            un_Gw_GwData.Tag = btn;
            un_Gw_GwData.la_Num.Text = btn.DisplayText;//數字鍵盤 - 顯示數值
            if (File.Exists(Units.langfile))//數字鍵盤 - 讀取多國語言
            {
                TIniFile ini = new TIniFile(Units.langfile);
                string name = ini.ReadString("Macro Show", btn.Name, "");//讀取名稱
                un_Gw_GwData.la_Msg.Text = name;//數字鍵盤 - 顯示名稱
            }

            Units.MacroInfo.GetMinMax(no, out double min, out double max);//取得上下限
            if ((min != 0 || max != 0)) // 0 ~ 0 就不顯示
            {
                un_Gw_GwData.la_Msg.Text += "\r\n" + min + " ~ " + max;//名稱的下一行 顯示上下限
            }

            //載入圖檔 (暫時先寫OIG的, 以後有OCD的再補)
            string GWTypeName = CurrentGwMacro[10004 + ((CurrentEditGwNo - 1) * 200)] == 0 ? "OCD" : "OIG";
           
            if (GWType[CurrentEditGwNo - 1] == MachineType.OCD2)
            {
                GWTypeName = "OCD2";
            }
            if (GWType[CurrentEditGwNo - 1] == MachineType.OCD3)
            {
                GWTypeName = "OCD3";
            }
            string filename = Application.StartupPath + $"\\image\\{GWTypeName}\\Data\\" + no + ".png";
            pic_GwData.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
        }

        private void btn_JigWidth_Click(object sender, EventArgs e)
        {
            string FileName = Application.StartupPath + "\\Language\\" + Units.LangCode + "\\" + Units.LangCode + ".txt";
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = btn_JigWidth.DisplayText;

            //顯示並等待結果
            if (form.ShowDialog() != DialogResult.OK) return;
            double.TryParse(form.uc_UserNum1.la_Num.Text, out double dVal);

            btn_JigWidth.DisplayText = dVal.ToString(Units.DisplayFmt);
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(658, dVal);//量測治具寬度
            }));
        }

        private void un_ProcessParam_OnBtnOkClick(object sender, EventArgs e)
        {
            if (dgv_MP_Param.CurrentRow == null) return;

            XmlElement x = (XmlElement)dgv_MP_Param.CurrentRow.Cells[Col_Param_XmlNode.Index].Value;
            string sMax = dgv_MP_Param.CurrentRow.Cells[Col_MP_Max.Index].Value.ToString();
            string sMin = dgv_MP_Param.CurrentRow.Cells[Col_MP_Min.Index].Value.ToString();
            double.TryParse(sMax, out double max);
            double.TryParse(sMin, out double min);

            //取得使用者輸入的數值
            double.TryParse(un_ProcessParam.la_Num.Text, out double val);
            if (max != 0 || min != 0)
            {
                if (val > max) val = max;
                if (val < min) val = min;
            }
            //此參數的顯示格式(小數位)
            String sFmt = x.GetAttribute("Format");

            //數值寫回顯示欄
            dgv_MP_Param.CurrentRow.Cells[Col_MP_ParamShow.Index].Value = val.ToString(sFmt);
            dgv_MP_Param.CurrentRow.Cells[Col_MP_ParamValue.Index].Value = val;

            //寫回
            x.SetAttribute("Value", val.ToString());
        }

        private void btn_Condition_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = sender as Uc_RoundBtn;
            if (btn == null) return;//例外處理
            if (btn.Tag == null) return;//例外處理
            btn.Lamp = true;

            int.TryParse(btn.Tag.ToString(), out int no);//tag 存放對應的Macro 位址,

            Uc_RoundBtn btn_tag = un_Gw_Condition.Tag as Uc_RoundBtn;
            if (btn_tag != null)
            {
                if (btn_tag == btn) return;
                btn_tag.Lamp = false;
            }

            un_Gw_Condition.Tag = btn;
            un_Gw_Condition.la_Num.Text = btn.DisplayText;//數字鍵盤 - 顯示數值
            if (File.Exists(Units.langfile))//數字鍵盤 - 讀取多國語言
            {
                TIniFile ini = new TIniFile(Units.langfile);
                string name = ini.ReadString("Macro Show", btn.Name, "");//讀取名稱
                un_Gw_Condition.la_Msg.Text = name;//數字鍵盤 - 顯示名稱
            }

            Units.MacroInfo.GetMinMax(no, out double min, out double max);//取得上下限
            if ((min != 0 || max != 0)) // 0 ~ 0 就不顯示
            {
                un_Gw_Condition.la_Msg.Text += "\r\n" + min + " ~ " + max;//名稱的下一行 顯示上下限
            }

            //載入圖檔 (暫時先寫OIG的, 以後有OCD的再補)
            string filename = Application.StartupPath + "\\image\\OIG\\Data\\" + no + ".png";
            pic_GwData.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
        }

        // 20260302 alan add 砂輪基準點
        private void pic_GWRPS_Click(object sender, EventArgs e) //進入砂輪基準點設定
        {

            TC_Main.SelectedTab = tab_GWRPS;//切到砂輪基準點設定
            PrevPage.Push(tab_GWRPS);
            btn_Prev.Visible = true;

            //顯示資料
            dgv_GWRPSs.Rows.Clear();
            ShowGWRPS();

            dgv_GWRPSs.Focus();

            dgv_GWRPSs.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv_GWRPSs.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);

        }

        public void ShowGWRPS() //砂輪基準點設定
        {
            int len = 10;
            bool bFinish = false;
            int probeTipDiameterLen = 1;
            Actions.Enqueue(new Action(() =>
            {
                focas.ReadMacro(12080, ref len, out double[] values);
                for (int i = 0; i < 4; i++)
                {
                    CurentGw_Data[12080 + i] = values[i];
                }
                focas.ReadMacro(634, ref probeTipDiameterLen, out double[] probeTipDiameterValues);
                CurentGw_Data[634] = probeTipDiameterValues[0];

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

            string unit = "mm";
            if (bInchTrans) unit = "inch";

            //pic_GWRPSset.Image = null; //Properties.Resources.GWTW;
            dgv_GWRPSs.Rows.Add(PCodeName(12080), "A", CurentGw_Data[12080].ToString(Units.DisplayFmt), unit, 12080);// 砂輪1寬度A
            dgv_GWRPSs.Rows.Add(PCodeName(12081), "B", CurentGw_Data[12081].ToString(Units.DisplayFmt), unit, 12081);//砂輪1砂輪柄長B
            dgv_GWRPSs.Rows.Add(PCodeName(12082), "C", CurentGw_Data[12082].ToString(Units.DisplayFmt), unit, 12082);//砂輪2寬度C
            dgv_GWRPSs.Rows.Add(PCodeName(12083), "D", CurentGw_Data[12083].ToString(Units.DisplayFmt), unit, 12083);//砂輪2砂輪柄長D
            dgv_GWRPSs.Rows.Add(PCodeName(634), "E", CurentGw_Data[634].ToString(Units.DisplayFmt), unit, 634);//端測球徑

            int.TryParse(dgv_GWRPSs.Rows[0].Cells[4].Value.ToString(), out int no); //獲取Macro
            double min, max;
            Units.MacroInfo.GetMinMax(no, out min, out max);
            if ((min != 0 || max != 0))
            {
                //顯示範圍
                uc_UserNumGwRPS.la_Msg.Text = LanguageManager.LoadMessage(Units.langfile, "Message", 139, "範圍 : ") + min + "~" + max;
            }
            else
            {
                //沒有上下限
                uc_UserNumGwRPS.la_Msg.Text = "";
            }
        }

        public string PCodeName(int PCode)
        {
            return LanguageManager.LoadMessage(Units.langfile, "PCode", PCode, "");
        }

        private void uc_UserNumGwRPS_OnBtnOkClick(object sender, EventArgs e)
        {
            bool nv = double.TryParse(uc_UserNumGwRPS.la_Num.Text, out double value);
            bool sv = int.TryParse(dgv_GWRPSs.CurrentRow.Cells[4].Value.ToString(), out int valuecode);
            double min, max;
            Units.MacroInfo.GetMinMax(valuecode, out min, out max);

            if (nv) dgv_GWRPSs.CurrentRow.Cells[2].Value = value.ToString(Units.DisplayFmt);
            CurentGw_Data[valuecode] = value;

            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(valuecode, CurentGw_Data[valuecode]);
            }));

            uc_UserNumGwRPS.la_Num.Text = "0";
        }

        PictureBox selectedPictureBox = null; // 用來記錄當前選取的圖片

        private void btn_GWRPS_Next_Click(object sender, EventArgs e) //砂輪基準點 - 填完資料 - 下一步
        {
            btn_GWRPS_save.Visible = false;

            for (int i = 0; i < 11; i++)
            {                
                PictureBox pic = GwRefPosSetPics[i];
                if (pic == null) continue;
                if (GwRefPosSetStatus.ContainsKey(i))
                {
                    string pic_filename = Application.StartupPath + "\\image\\GwRefPosSet\\" + (i + 1) + (GwRefPosSetStatus[i] == 1 ? "s" : "") + ".png";
                    if (File.Exists(pic_filename)) pic.Image = (Bitmap)Bitmap.FromFile(pic_filename);
                }
                else
                {
                    string pic_filename = Application.StartupPath + "\\image\\GwRefPosSet\\" + (i + 1) + ".png";
                    if (File.Exists(pic_filename)) pic.Image = (Bitmap)Bitmap.FromFile(pic_filename);
                }
            }


            if (selectedPictureBox != null)
            {
                // 清除之前選取的圖片效果
                selectedPictureBox.Padding = new Padding(0);
                selectedPictureBox.BackColor = Color.Transparent;  // 還原背景顏色
            }

            TC_Main.SelectedTab = tab_GWRPS2;
            PrevPage.Push(tab_GWRPS2);
            btn_Prev.Visible = true;

            PictureBox_Click(pic_GWRPSsetting1, null);

        }
        private void PictureBox_Click(object sender, EventArgs e)
        {

            PictureBox clickedPictureBox = sender as PictureBox;

            int.TryParse(clickedPictureBox.Tag.ToString(), out int index);

            if (selectedPictureBox != null && selectedPictureBox != clickedPictureBox)
            {
                // 清除之前選取的圖片效果
                selectedPictureBox.Padding = new Padding(0);
                selectedPictureBox.BackColor = Color.Transparent;  // 還原背景顏色
            }


            // 設定新的選取效果
            clickedPictureBox.Padding = new Padding(7);  // 框的粗細
            clickedPictureBox.BackColor = Color.Red;     // 框的顏色
            selectedPictureBox = clickedPictureBox;

            TIniFile iniLang = new TIniFile(Units.langfile);
            la_GWRPSmessage.Text = iniLang.ReadString("GWRPS_Message", "Message" + (index + 1), "");

        }

        private void btn_GWRPSxz_Click(object sender, EventArgs e)
        {
            if (bRun)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 148, "程式仍在執行"), "");
                return;
            }

            if (selectedPictureBox != null && selectedPictureBox.Tag != null)
            {
                int.TryParse(selectedPictureBox.Tag.ToString(), out int result);

                // 軟鍵啟動
                // #980=2
                Actions.Enqueue(new Action(() =>
                {
                    focas.WriteMacro(980, 2);
                    focas.WriteMacro(12089, result + 1);
                    OneKeyCall(8999);
                }));
            }

        }
        private void pic_RotationCenterOffset_Click(object sender, EventArgs e) //進入旋轉位置中心設定
        {

            TC_Main.SelectedTab = tab_RotationCenterOffset;//切到旋轉位置中心設定
            PrevPage.Push(tab_RotationCenterOffset);
            btn_Prev.Visible = true;

            Actions.Enqueue(new Action(() =>
            {
                int len = 8;
                int offsetLen = 2;
                focas.ReadMacro(12070, ref len, out double[] data1);
                focas.ReadMacro(10000, ref offsetLen, out double[] data2);
                focas.ReadMacro(10100, ref offsetLen, out double[] data3);

                this.Invoke((Action)(() =>
                {
                    // 砂輪 1
                    btn_Gw1_XOffset.DisplayText = data1[4].ToString(Units.DisplayFmt);
                    la_Gw1_XCenterParam.Text = data2[0].ToString(Units.DisplayFmt);
                    btn_Gw1_ZOffset.DisplayText = data1[5].ToString(Units.DisplayFmt);
                    la_Gw1_ZCenterParam.Text = data2[1].ToString(Units.DisplayFmt);
                    // 砂輪 2
                    btn_Gw2_XOffset.DisplayText = data1[6].ToString(Units.DisplayFmt);
                    la_Gw2_XCenterParam.Text = data3[0].ToString(Units.DisplayFmt);
                    btn_Gw2_ZOffset.DisplayText = data1[7].ToString(Units.DisplayFmt);
                    la_Gw2_ZCenterParam.Text = data3[1].ToString(Units.DisplayFmt);

                    btn_Gw1_XOffset.PerformClick();
                }));
            }));

        }

        Uc_RoundBtn btn_CurrentRotationCenterClicked = null;
        private void btn_RotationCenter_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;

            if (btn_CurrentRotationCenterClicked != null)
            {
                btn_CurrentRotationCenterClicked.Lamp = false;
            }
            btn.Lamp = true;
            btn_CurrentRotationCenterClicked = btn;
            uc_UserNum_RotationCenter.la_Num.Text = btn_CurrentRotationCenterClicked.DisplayText;
        }
        private void uc_UserNumRotationCenter_OnBtnOkClick(object sender, EventArgs e)
        {
            double.TryParse(uc_UserNum_RotationCenter.la_Num.Text, out double data);
            if (btn_CurrentRotationCenterClicked.Tag != null)
            {
                int.TryParse(btn_CurrentRotationCenterClicked.Tag.ToString(), out int no);
                //focas.WriteMacro(no, data);

                switch (no)
                {
                    case 12074:
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.WriteMacro(no, data);
                            focas.ReadMacro(12070, out double data1);
                            focas.WriteMacro(10000, data + data1);
                            this.Invoke((Action)(() =>
                            {
                                la_Gw1_XCenterParam.Text = (data + data1).ToString(Units.DisplayFmt);
                            }));
                        }));

                        break;
                    case 12075:
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.WriteMacro(no, data);
                            focas.ReadMacro(12071, out double data2);
                            focas.WriteMacro(10001, data + data2);
                            this.Invoke((Action)(() =>
                            {
                                la_Gw1_ZCenterParam.Text = (data + data2).ToString(Units.DisplayFmt);
                            }));
                        }));
                        break;
                    case 12076:
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.WriteMacro(no, data);
                            focas.ReadMacro(12072, out double data3);
                            focas.WriteMacro(10100, data + data3);
                            this.Invoke((Action)(() =>
                            {
                                la_Gw2_XCenterParam.Text = (data + data3).ToString(Units.DisplayFmt);
                            }));
                        }));
                        break;
                    case 12077:
                        Actions.Enqueue(new Action(() =>
                        {
                            focas.WriteMacro(no, data);
                            focas.ReadMacro(12073, out double data4);
                            focas.WriteMacro(10101, data + data4);
                            this.Invoke((Action)(() =>
                            {
                                la_Gw2_ZCenterParam.Text = (data + data4).ToString(Units.DisplayFmt);
                            }));
                        }));
                        break;
                }
                btn_CurrentRotationCenterClicked.DisplayText = data.ToString(Units.DisplayFmt);
            }
            //uc_UserNum_RotationCenter.la_Num.Text = "0";
        }


        private void btn_GWRPSxz_Reset_Click(object sender, EventArgs e)
        {
            if (bRun)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 148, "程式仍在執行"), "");
                return;
            }

            DialogResult ret = Fo_Msg.Show(
                                       LanguageManager.LoadMessage(Units.langfile, "Message", 166, "是否要全部重置"),
                                       LanguageManager.LoadMessage(Units.langfile, "Message", 6, "警告"),
                                       MessageBoxButtons.YesNo);
            if (ret != DialogResult.Yes)
                return;

            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                int offsetLen = 12;
                focas.WriteMacro(12091, ref offsetLen, new double[12]);
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
        }

        public void WaitAction_WriteMacro(int no, double val)
        {
            bool bFinish = false;
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(no, val);
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
        }

        private void rb_Diam_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb_DiamKind = (RadioButton)sender;
            TIniFile iniLang = new TIniFile(Units.langfile);

            if (rb_DiamKind.Checked)
            {

                if (pic_G5459X.Tag != null)
                {
                    String filename;
                    int.TryParse(pic_G5459X.Tag.ToString(), out int gwType);
                    string path = Application.StartupPath + "\\image\\" + (gwType == 0 ? "OIG" : "OCD") + "\\DressWorkpiece\\";
                    XmlElement xmlCoordinate = (gwType == 0 ? machineSetting.xmlOIG_Coordinate : machineSetting.xmlOCD_Coordinate);
                    XmlElement xmlG54X = xmlCoordinate.GetChildNodeAt(3);

                    if (rb_DiamKind.Tag.ToString() == "in")
                    {
                        filename = path + xmlG54X.GetAttribute("Image"); //研磨工件內徑
                        pic_G5459X.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                        la_DP_Msg1.Text = iniLang.ReadString("Fo_Main", "la_DP_Msg1", "");
                        la_Cal_Diam.Text = iniLang.ReadString("Fo_Main", "la_Cal_Diam", "");

                    }
                    if (rb_DiamKind.Tag.ToString() == "out")
                    {
                        filename = path + xmlG54X.GetAttribute("ODImage"); //研磨工件外徑
                        pic_G5459X.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                        la_DP_Msg1.Text = iniLang.ReadString("Message", "Message167", "");
                        la_Cal_Diam.Text = iniLang.ReadString("Message", "Message168", "");
                    }
                }
            }
        }
        private void btn_Diam_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn_DiamKind = (Uc_RoundBtn)sender;
            if (btn_CurrentRotationCenterClicked != null)
            {
                btn_CurrentRotationCenterClicked.Lamp = false;
            }
            btn_DiamKind.Lamp = true;
            btn_CurrentRotationCenterClicked = btn_DiamKind;

            TIniFile iniLang = new TIniFile(Units.langfile);

            if (pic_G5459X.Tag != null)
            {
                String filename;
                int.TryParse(pic_G5459X.Tag.ToString(), out int gwType);
                string path = Application.StartupPath + "\\image\\" + (gwType == 0 ? "OIG" : "OCD") + "\\DressWorkpiece\\";
                XmlElement xmlCoordinate = (gwType == 0 ? machineSetting.xmlOIG_Coordinate : machineSetting.xmlOCD_Coordinate);
                XmlElement xmlG54X = xmlCoordinate.GetChildNodeAt(3);

                if (btn_DiamKind.Tag.ToString() == "in")
                {
                    filename = path + xmlG54X.GetAttribute("Image"); //研磨工件內徑
                    pic_G5459X.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                    la_DP_Msg1.Text = iniLang.ReadString("Fo_Main", "la_DP_Msg1", "");
                    la_Cal_Diam.Text = iniLang.ReadString("Fo_Main", "la_Cal_Diam", "");

                }
                if (btn_DiamKind.Tag.ToString() == "out")
                {
                    filename = path + xmlG54X.GetAttribute("ODImage"); //研磨工件外徑
                    pic_G5459X.Image = File.Exists(filename) ? Image.FromFile(filename) : null;
                    la_DP_Msg1.Text = iniLang.ReadString("Message", "Message167", "");
                    la_Cal_Diam.Text = iniLang.ReadString("Message", "Message168", "");
                }
            }
        }
        public void Action_WriteMacro(int no, double val)
        {
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(no, val);
            }));
        }


        private void rb_GwTypeSelect(object sender, EventArgs e)
        {
            if (sender == rb_Gw1Type0) Action_WriteMacro(10004, 0);
            else if (sender == rb_Gw1Type1) Action_WriteMacro(10004, 1);
            else if (sender == rb_Gw1Type2) Action_WriteMacro(10004, 0); // GW1斜頭是外圓，但要設角度 #671
            else if (sender == rb_Gw1Type3) Action_WriteMacro(10004, 0); // GW1斜頭是外圓，但要設角度 #671
            else if (sender == rb_Gw2Type0) Action_WriteMacro(10204, 0);
            else if (sender == rb_Gw2Type1) Action_WriteMacro(10204, 1);
            else if (sender == rb_Gw2Type2) Action_WriteMacro(10204, 0); // GW2斜頭是外圓，但要設角度 #673
            else if (sender == rb_Gw2Type3) Action_WriteMacro(10204, 0); // GW2斜頭是外圓，但要設角度 #673
            else if (sender == rb_Gw3Type0) Action_WriteMacro(10404, 0);
            else if (sender == rb_Gw3Type1) Action_WriteMacro(10404, 1);
            else if (sender == rb_Gw3Type2) Action_WriteMacro(10404, 0); // GW3斜頭是外圓，但要設角度 #675
            else if (sender == rb_Gw3Type3) Action_WriteMacro(10404, 0); // GW3斜頭是外圓，但要設角度 #675
            RadioButton rb = (RadioButton)sender;
            string tagValue = rb.Tag.ToString();
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            if (!string.IsNullOrEmpty(tagValue))
            {
                string sectionName = "";
                int value = int.Parse(tagValue);
                if (rb.Name.Contains("Gw1"))
                {
                    sectionName = "GW1Type";
                    GWType[0] = (MachineType)value;
                }
                else if (rb.Name.Contains("Gw2"))
                {
                    sectionName = "GW2Type";
                    GWType[1] = (MachineType)value;
                }
                else if (rb.Name.Contains("Gw3"))
                {
                    sectionName = "GW3Type";
                    GWType[2] = (MachineType)value;
                }
              
                if (!string.IsNullOrEmpty(sectionName))
                {
                    ini.WriteInteger("System", sectionName, value);
                }
            }
        }

        private void btn_GWRPS_save_Click(object sender, EventArgs e)
        {
            if (bRun)
            {
                Fo_Msg.Show(LanguageManager.LoadMessage(Units.langfile, "Message", 148, "程式仍在執行"), "");
                return;
            }

            // 軟鍵啟動
            // #980=2
            Actions.Enqueue(new Action(() =>
            {
                focas.WriteMacro(980, 2);//基準點設定
                focas.WriteMacro(12089, 12);//步驟12 : 完成
                OneKeyCall(8999);//一鍵呼叫
            }));

        }

        private void btn_WorkCenterPosByID_Click(object sender, EventArgs e)
        {
            pic_IDCenterPos_Gw1_X1.Visible = true;
            pic_IDCenterPos_Gw1_X2.Visible = true;
            btn_WorkCenterPosByID.Lamp = true;

            pic_ODCenterPos_Gw1_X1.Visible = false;
            pic_ODCenterPos_Gw1_X2.Visible = false;
            btn_WorkCenterPosByOD.Lamp = false;
        }

        private void btn_WorkCenterPosByOD_Click(object sender, EventArgs e)
        {
            pic_IDCenterPos_Gw1_X1.Visible = false;
            pic_IDCenterPos_Gw1_X2.Visible = false;
            btn_WorkCenterPosByID.Lamp = false;

            pic_ODCenterPos_Gw1_X1.Visible = true;
            pic_ODCenterPos_Gw1_X2.Visible = true;
            btn_WorkCenterPosByOD.Lamp = true;
        }

        //public static bool bFinish = false;
        public void NeedWait(ref bool bFinish)
        {
            DateTime dt_Try3Sencond = DateTime.Now;
            while (!bFinish)
            {
                if (!bCNCConnect || DateTime.Now > dt_Try3Sencond.AddSeconds(3))
                {
                    break;
                }
                Application.DoEvents();//等待通訊結束
            }
        }
        public void CheckOCD2Type(out double ocd2)
        {
            bool bFinish = false;
            ocd2 = 0;
            double tmpOcd2 = 0;
            Actions.Enqueue(new Action(() =>
            {
                focas.ReadMacro(671 + ((CurrentEditGwNo - 1) * 2), out tmpOcd2);//0:直頭, 不等 0 :斜頭
                bFinish = true;
            }));
            int iStart = Environment.TickCount;
            while (!bFinish)
            {
                int iTime = Environment.TickCount - iStart;
                if (iTime > 5000)
                {
                    return;
                }
                Application.DoEvents();
            }
            ocd2 = tmpOcd2;
        }

        private void btn_dgvScrollUpOrDown_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            int currentIndex = DGV_GwParam.FirstDisplayedScrollingRowIndex;
            int scrollAmount = 5; // 每次點擊滾動的列數

            if (btn.Tag != null)
            {
                if (btn.Tag.ToString() == "1")
                {
                    if (currentIndex - scrollAmount >= 0)
                    {
                        DGV_GwParam.FirstDisplayedScrollingRowIndex = currentIndex - scrollAmount;
                    }
                    else
                    {
                        DGV_GwParam.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
                if (btn.Tag.ToString() == "2")
                {
                    if (currentIndex + scrollAmount < DGV_GwParam.RowCount)
                    {
                        DGV_GwParam.FirstDisplayedScrollingRowIndex = currentIndex + scrollAmount;
                    }
                    else
                    {
                        DGV_GwParam.FirstDisplayedScrollingRowIndex = DGV_GwParam.RowCount - 1;
                    }
                }
                
            }
        }

        private void pic_DressTool_Click(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            
            if (pic.Tag != null)
            {
                double val = double.Parse(pic.Tag.ToString());
                //Action_WriteMacro(558, val);
                WaitAction_WriteMacro(558, val);
                InitDressGwSetting(false);
            }
        }

        // 轉頭安全位置
        private void btn_PosSet_TowerSafePos_Click(object sender, EventArgs e)
        {
            tc_PositionSet.SelectedTab = tab_PosSet_TowerSafePos;
        }

        // 內圓安全區間
        private void btn_PosSet_IDRevSafePos_Click(object sender, EventArgs e)
        {
            tc_PositionSet.SelectedTab = tab_PosSet_IDRevSafePos;
        }

        // 修整座最大最小值
        private void btn_PosSet_DressMaxMinValue_Click(object sender, EventArgs e)
        {
            tc_PositionSet.SelectedTab = tab_PosSet_DressMaxMinValue;
        }

        private void btn_TowerSafePos_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            if(btn.Tag != null)
            {
                string tagValue = btn.Tag.ToString();
                if(tagValue == "X")
                {
                    if (Pos.Machine.Length > 0) TB_TowerSafePosX.Text = Pos.Machine[0].ToString(Units.DisplayFmt);
                }
                if (tagValue == "Z")
                {
                    if (Pos.Machine.Length > 1) TB_TowerSafePosZ.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                }
                
                bPosSetSave = true;
                btn_PosSetSave.Enabled = true;
                bTowerSafePos = true;
            }

        }
        private void btn_ID_DressRevZ_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            if (btn.Tag != null)
            {
                string tagValue = btn.Tag.ToString();
                if (tagValue == "Z1")
                {
                    if (Pos.Machine.Length > 1) TB_ID_DressRevZ1.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                }
                if (tagValue == "Z2")
                {
                    if (Pos.Machine.Length > 1) TB_ID_DressRevZ2.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                }

                bPosSetSave = true;
                btn_PosSetSave.Enabled = true;
                bIDRevSafePos = true;
            }
        }

        private void btn_DressBaseMaxMin_Click(object sender, EventArgs e)
        {
            Uc_RoundBtn btn = (Uc_RoundBtn)sender;
            if (btn.Tag != null)
            {
                string tagValue = btn.Tag.ToString();
                if (tagValue == "Max")
                {
                    if (Pos.Machine.Length > 1) TB_DressBaseMax.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                }
                if (tagValue == "Min")
                {
                    if (Pos.Machine.Length > 1) TB_DressBaseMin.Text = Pos.Machine[1].ToString(Units.DisplayFmt);
                }

                bPosSetSave = true;
                btn_PosSetSave.Enabled = true;
                bDressMaxMinValue = true;
            }
        }

        private void TB_MatainPosition_Click(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            Fo_Num form = new Fo_Num();
            form.uc_UserNum1.la_Num.Text = tb.Text;
            if (File.Exists(Units.langfile)) //小鍵盤顯示物件名稱 抓txt
            {
                TIniFile tIniFile = new TIniFile(Units.langfile);
                string name = tIniFile.ReadString("Macro Show", tb.Name, "");
                form.uc_UserNum1.la_Msg.Text = name;
            }
            DialogResult ret = form.ShowDialog();
            if (ret == DialogResult.OK)
            {
                double.TryParse(form.uc_UserNum1.la_Num.Text, out double data);
                tb.Text = data.ToString(Units.DisplayFmt);

                if (tb.Tag != null)
                {
                    string tagValue = tb.Tag.ToString();
                    switch (tagValue)
                    {
                        case "Tower":
                            bPosSetSave = true;
                            btn_PosSetSave.Enabled = true;
                            bTowerSafePos = true;
                            break;
                        case "ID_DressRevZ":
                            bPosSetSave = true;
                            btn_PosSetSave.Enabled = true;
                            bIDRevSafePos = true;
                            break;
                        case "DressBase":
                            bPosSetSave = true;
                            btn_PosSetSave.Enabled = true;
                            bDressMaxMinValue = true;
                            break;
                    }
                }
            }
        }

        private void TB_MatainPosition_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if(tb.Tag != null)
            {
                string tagValue = tb.Tag.ToString();
                switch(tagValue)
                {
                    case "Tower":
                        bPosSetSave = true;
                        btn_PosSetSave.Enabled = true;
                        bTowerSafePos = true;
                        break;
                    case "ID_DressRevZ":
                        bPosSetSave = true;
                        btn_PosSetSave.Enabled = true;
                        bIDRevSafePos = true;
                        break;
                    case "DressBase":
                        bPosSetSave = true;
                        btn_PosSetSave.Enabled = true;
                        bDressMaxMinValue = true;
                        break;
                }
            }
            
        }

        private void ch_YAEnable_Click(object sender, EventArgs e)
        {
            TIniFile ini = new TIniFile(Application.StartupPath + "\\sys.ini");
            ini.WriteBool("UI", "YAEnable", ch_YAEnable.Checked);
            bYAEnable = ch_YAEnable.Checked;
        }
    }
}


public class SoftPBLamp
{
    public SoftPBLamp()
    {
    }

    public SoftPBLamp(object btn, PmcAddrType type, int addr, int bit)
    {
        SoftPB = (Uc_RoundBtn)btn;
        Type = type;
        Addr = addr;
        Bit = bit;
    }

    public Uc_RoundBtn SoftPB;
    public PmcAddrType Type;
    public int Addr;
    public int Bit;

}

public class DGWFile
{
    public List<DGWData> LeftList = new List<DGWData>();//左側修整資料
    public List<DGWData> DiamList = new List<DGWData>();//外徑修整資料
    public List<DGWData> RightList = new List<DGWData>();//右側修整資料

    public double DGWDiamOffsetZ;//外徑修整Z軸補正
    public double Diam_ToolComp;//外徑刀尖功能
    public double Diam_ToolR;//外徑刀尖半徑
    public double Left_ToolComp;//左側刀尖功能
    public double Left_ToolR;//左側刀尖半徑

    public DGWFile()
    {
    }

    public DGWFile(string filename)
    {
        LoadFromFile(filename);
    }

    public void LoadFromFile(String FileName)
    {
        LeftList.Clear();
        DiamList.Clear();
        RightList.Clear();

        if (!File.Exists(FileName)) return;

        //String[] lines = File.ReadAllLines(FileName);
        //PathData pd = null;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(FileName);
        XmlElement root = xmlDoc.DocumentElement;
        if (root.Name != "DGW") return;
        for (int j = 0; j < root.ChildNodes.Count; j++)
        {
            XmlElement xmlPath = (XmlElement)root.ChildNodes[j];
            if (xmlPath.Name != "Left" && xmlPath.Name != "Diameter" && xmlPath.Name != "Right") continue;

            if (xmlPath.Name == "Left")
            {
                if (double.TryParse(xmlPath.GetAttribute("ToolComp"), out double l_tool_comp)) Left_ToolComp = l_tool_comp;
                else Left_ToolComp = 0;
                if (double.TryParse(xmlPath.GetAttribute("ToolR"), out double l_tool_r)) Left_ToolR = l_tool_r;
                else l_tool_r = 0;
            }
            else if (xmlPath.Name == "Diameter")
            {
                if (double.TryParse(xmlPath.GetAttribute("ToolComp"), out double d_tool_comp)) Diam_ToolComp = d_tool_comp;
                else Diam_ToolComp = 0;
                if (double.TryParse(xmlPath.GetAttribute("ToolR"), out double d_tool_r)) Diam_ToolR = d_tool_r;
                else Diam_ToolR = 0;
                if (double.TryParse(xmlPath.GetAttribute("OffsetZ"), out double offset_z)) DGWDiamOffsetZ = offset_z;
                else DGWDiamOffsetZ = 0;
            }

            for (int i = 0; i < xmlPath.ChildNodes.Count; i++)
            {
                XmlElement xmlNode = (XmlElement)xmlPath.ChildNodes[i];
                if (xmlNode.Name != "CMD") continue;
                int.TryParse(xmlNode.GetAttribute("Type"), out int type);
                double.TryParse(xmlNode.GetAttribute("X"), out double x);
                double.TryParse(xmlNode.GetAttribute("Z"), out double z);
                double.TryParse(xmlNode.GetAttribute("R"), out double r);
                double.TryParse(xmlNode.GetAttribute("Feed"), out double f);
                double.TryParse(xmlNode.GetAttribute("OffsetX"), out double ofsx);
                double.TryParse(xmlNode.GetAttribute("OffsetZ"), out double ofsz);
                if (xmlPath.Name == "Left") LeftList.Add(new DGWData(type, x, z, r, f, ofsx, ofsz));
                if (xmlPath.Name == "Diameter") DiamList.Add(new DGWData(type, x, z, r, f, ofsx, ofsz));
                if (xmlPath.Name == "Right") RightList.Add(new DGWData(type, x, z, r, f, ofsx, ofsz));
            }
        }

        //foreach (String line in lines)
        //{
        //    if (line.Trim() == "") continue;
        //    int pos = line.IndexOf('\t');//資料

        //    if (pos >= 0)
        //    {
        //        if (pd == null) continue;
        //        pd.Path.Add(line.Trim());
        //    }
        //    else
        //    {
        //        pd = new PathData();
        //        string[] data = line.Split(',');
        //        if (data.Length == 2)
        //        {
        //            pd.Name = data[0];
        //            pd.GW_No = int.Parse(data[1]);
        //        }
        //        Paths.Add(pd);
        //    }
        //}
    }

    public void SaveToFile(String FileName)
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement root = xmlDocument.CreateElement("DGW");
        xmlDocument.AppendChild(root);

        XmlElement left = xmlDocument.CreateElement("Left");
        XmlElement diam = xmlDocument.CreateElement("Diameter");
        XmlElement right = xmlDocument.CreateElement("Right");

        root.AppendChild(left);
        root.AppendChild(diam);
        root.AppendChild(right);

        left.SetAttribute("ToolComp", Left_ToolComp.ToString("0"));
        left.SetAttribute("ToolR", Left_ToolR.ToString(Units.DisplayFmt));

        diam.SetAttribute("ToolComp", Diam_ToolComp.ToString("0"));
        diam.SetAttribute("ToolR", Diam_ToolR.ToString(Units.DisplayFmt));
        diam.SetAttribute("OffsetZ", DGWDiamOffsetZ.ToString(Units.DisplayFmt));

        foreach (DGWData data in LeftList)
        {
            XmlElement cmd = xmlDocument.CreateElement("CMD");
            cmd.SetAttribute("Type", data.Type.ToString());
            cmd.SetAttribute("X", data.X.ToString());
            cmd.SetAttribute("Z", data.Z.ToString());
            cmd.SetAttribute("R", data.R.ToString());
            cmd.SetAttribute("Feed", data.Feed.ToString());
            cmd.SetAttribute("OffsetX", data.OffsetX.ToString());
            cmd.SetAttribute("OffsetZ", data.OffsetZ.ToString());
            left.AppendChild(cmd);
        }

        foreach (DGWData data in DiamList)
        {
            XmlElement cmd = xmlDocument.CreateElement("CMD");
            cmd.SetAttribute("Type", data.Type.ToString());
            cmd.SetAttribute("X", data.X.ToString());
            cmd.SetAttribute("Z", data.Z.ToString());
            cmd.SetAttribute("R", data.R.ToString());
            cmd.SetAttribute("Feed", data.Feed.ToString());
            cmd.SetAttribute("OffsetX", data.OffsetX.ToString());
            cmd.SetAttribute("OffsetZ", data.OffsetZ.ToString());
            diam.AppendChild(cmd);
        }

        foreach (DGWData data in RightList)
        {
            XmlElement cmd = xmlDocument.CreateElement("CMD");
            cmd.SetAttribute("Type", data.Type.ToString());
            cmd.SetAttribute("X", data.X.ToString());
            cmd.SetAttribute("Z", data.Z.ToString());
            cmd.SetAttribute("R", data.R.ToString());
            cmd.SetAttribute("Feed", data.Feed.ToString());
            cmd.SetAttribute("OffsetX", data.OffsetX.ToString());
            cmd.SetAttribute("OffsetZ", data.OffsetZ.ToString());
            right.AppendChild(cmd);
        }

        xmlDocument.Save(FileName);

        //List<String> lines = new List<string>();
        //foreach (PathData p in Paths)
        //{
        //    lines.Add(p.Name + "," + p.GW_No);
        //    foreach (string d in p.Path)
        //    {
        //        lines.Add("\t" + d);
        //    }
        //}
        ////File.WriteAllLines(Application.StartupPath + "\\path.txt", lines);
        //File.WriteAllLines(FileName, lines);
    }

    
}

public class DGWData//成形修整資料
{
    public int Type = 0; //0:未定義, 1:G01, 2:G01, 3:G02, 4:G03, 5:G00
    public double X = 0;
    public double Z = 0;
    public double R = 0; //(for G02, G03)
    public double Feed = 0;
    public double OffsetX = 0;
    public double OffsetZ = 0;

    public DGWData()
    {
    }

    public DGWData(int type, double x, double z, double r, double f)
    {
        Type = type;
        X = x;
        Z = z;
        R = r;
        Feed = f;
    }

    public DGWData(int type, double x, double z, double r, double f, double ofsx, double ofsz)
    {
        Type = type;
        X = x;
        Z = z;
        R = r;
        Feed = f;
        OffsetX = ofsx;
        OffsetZ = ofsz;
    }
}


public class SoftPanelPB
{
    public string Addr;
    public int Start;
}

public class DblPoint
{
    public double X;
    public double Y;
    public DblPoint(double x, double y)
    {
        X = x;
        Y = y;
    }
}



enum CreateMode
{
    Add,
    Insert,
    InsertBack
}

enum PathOrigin
{
    Left,
    Right
}

public enum MachineType
{
    OCD = 0,
    OIG = 1,
    OCD2,
    OCD3
}

