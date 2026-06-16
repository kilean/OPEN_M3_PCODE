using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OIG
{
    public partial class Fo_NumMem : Form
    {
        //private bool bFirst;
        private bool bDot;
        public bool bMemory;
        //private int Mode;
        public String InputData;
        public double TmpVal;

        public string ParaName { get; set; }


        public void SetVal(double val)
        {
            Clear();
            la_Num.Text = val.ToString();
            TmpVal = val;
        }
        public void Clear()
        {
            //bFirst = true;
            //Mode = 0;
            TmpVal = 0;
            bDot = false;
            la_Num.Text = "0";
            InputData = "0";
        }

        //private THook hook;
        public Fo_NumMem()
        {
            InitializeComponent();
            this.LoadLanguageFile(Units.langfile, this.Name);
            //hook = new THook(KeyboardHookProc);
        }

        private void Fo_Num_Load(object sender, EventArgs e)
        {
            //bFirst = true;
            bDot = false;
            //Mode = 0;
            InputData = "0";
            //TmpVal = 0;
            //label1.Text = (ParaName != "") ? ParaName : "";
            Bitmap bmp = btn_Backspace.Image as Bitmap;
            bmp.MakeTransparent(Color.White);
            btn_Backspace.Image = bmp;
        }


        private void PutNum(String Data)
        {
            if (InputData != "0")
            {
                InputData += Data;
            }
            else
            {
                InputData = Data;
            }

            if (la_Num.Text == "-0")
            {
                if (Data == "0") return;
                InputData = "-" + Data;
            }

            la_Num.Text = InputData;
        }

       

        private void NumClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String Data = btn.Text;
            PutNum(Data);

        }
        private void btn_Backspace_Click(object sender, EventArgs e)
        {
            if (InputData.Length == 0) return;
            if (la_Num.Text == "0" && InputData == "0") return;

            if (la_Num.Text == "-0")
            {
                la_Num.Text = "0";
                InputData = "0";
            }

            InputData = InputData.Substring(0, InputData.Length - 1);
            if (InputData == "")
            {
                String Val = TmpVal.ToString();
                if (Val.Length > 9) Val = TmpVal.ToString(Units.DisplayFmt);
                la_Num.Text = "0";
                InputData = "0";
            }
            else
            {
                double val;
                double.TryParse(InputData, out val);
                int v = (int)val;
                if (val == v) bDot = false;

                String Val = TmpVal.ToString();
                la_Num.Text = InputData;
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Dot_Click(object sender, EventArgs e)
        {
            if (!bDot)
            {
                if (InputData == "") InputData = "0";
                InputData = InputData + ".";
                la_Num.Text = InputData;
                bDot = true;
                //bFirst = false;
            }            
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
                double.TryParse(la_Num.Text , out TmpVal);
                DialogResult = DialogResult.OK;
                this.Close();
        }


        private void btn_UseCurrentPos_Click(object sender, EventArgs e)
        {
            bool ret = Units.Fo_Main.GetMemoryValue(out double val);
            if (ret) SetVal(val);
        }

        private void btn_Sub_Click(object sender, EventArgs e)
        {
            char numf = InputData[0];
            string num = numf.ToString();

            InputData = (num != "-") ? "-" + InputData : InputData.TrimStart('-');         
            la_Num.Text = InputData;

        }
    }
}
