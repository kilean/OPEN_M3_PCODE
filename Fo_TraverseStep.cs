using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCD
{
    public partial class Fo_TraverseStep : Form
    {
        bool bNumSigned = false;//負號旗標

        public Fo_TraverseStep()
        {
            InitializeComponent();
        }

        private void cb_Step_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cb_Step.SelectedIndex;
            if (index < 0) return;
            int count = index + 1;

            int ProcessIndex = Units.Fo_Main.ProcessIndex;

            if (Units.Fo_Main.TempProcess.SubPrograms.Count <= 0) return;

            TSubProgram sp = Units.Fo_Main.TempProcess.SubPrograms[0];

            dataGridView1.Rows.Clear();
            for (int i = 0; i < count; i++)
            {
                TArgument xma = sp.GetArgument((19951 + i).ToString());
                TArgument zma = sp.GetArgument((19971 + i).ToString());
                if (xma == null) continue;
                if (zma == null) continue;
                dataGridView1.Rows.Add((i + 1).ToString(), xma.Value.ToString(Units.DisplayFmt), zma.Value.ToString(Units.DisplayFmt));
            }
        }

        private void Fo_TraverseStep_Load(object sender, EventArgs e)
        {
            this.LoadLanguageFile(Units.langfile, this.Name);
        }

        private void Fo_TraverseStep_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            Units.Fo_Main.LoadTraverseData();

        }

        private void btn_Backspace_Click(object sender, EventArgs e)
        {
            if (la_NumVal.Text == "-0")
            {
                la_NumVal.Text = la_NumVal.Text.Replace("-", "");
                bNumSigned = !bNumSigned;
            }
            if (la_NumVal.Text.Length > 1)
            {
                la_NumVal.Text = la_NumVal.Text.Substring(0, la_NumVal.Text.Length - 1);
            }
            else
            {
                la_NumVal.Text = "0";
            }
        }

        private void btn_NumClear_Click(object sender, EventArgs e)
        {
            la_NumVal.Text = "0";
            bNumSigned = false;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            int Row = dataGridView1.CurrentCell.RowIndex;

            int Col = dataGridView1.CurrentCell.ColumnIndex;

            int ProcessIndex = Units.Fo_Main.ProcessIndex;
            TSubProgram sp = Units.Fo_Main.TempProcess.SubPrograms[0];

            TArgument a = null;
            if (Col == 1)
            {
                a = sp.GetArgument((19951 + Row).ToString());
                if (a == null) return;
            }
            else if (Col == 2)
            {
                a = sp.GetArgument((19971 + Row).ToString());
                if (a == null) return;
            }
            else
            {
                return;
            }

            double.TryParse(la_NumVal.Text, out double val);
            if (val < a.Min)
            {
                val = a.Min;
                la_NumVal.Text = val.ToString("0.#####");
            }
            if (val > a.Max)
            {
                val = a.Max;
                la_NumVal.Text = val.ToString("0.#####");
            }

            //數值寫回顯示欄
            dataGridView1.Rows[Row].Cells[Col].Value = val.ToString(Units.DisplayFmt);

            //寫回引數
            a.Value = val;
            
            la_NumVal.Text = "0";
            bNumSigned = false;
        }

        private void btn_UseCurrentPos_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null)
                return;

            int ProcessIndex = Units.Fo_Main.ProcessIndex;
            int Row = dataGridView1.CurrentCell.RowIndex;
            int Col = dataGridView1.CurrentCell.ColumnIndex;

            if (Units.Fo_Main.TempProcess == null) return;
            if (Units.Fo_Main.TempProcess.SubPrograms.Count < 0) return;

            TSubProgram sp = Units.Fo_Main.TempProcess.SubPrograms[0];

            TArgument a;
            double val;
            if (Col == 1)
            {
                a = sp.GetArgument((19951 + Row).ToString());
                if (a == null) return;

                double.TryParse(Units.Fo_Main.la_EditAbsAxis1Value.Text, out val);
            }
            else if (Col == 2)
            {
                a = sp.GetArgument((19971 + Row).ToString());
                if (a == null) return;

                double.TryParse(Units.Fo_Main.la_EditAbsAxis2Value.Text, out val);
            }
            else
            {
                return;
            }

           
            la_NumVal.Text = val.ToString("0.#####");
            if (val < a.Min)
            {
                val = a.Min;
                la_NumVal.Text = val.ToString("0.#####");
            }
            if (val > a.Max)
            {
                val = a.Max;
                la_NumVal.Text = val.ToString("0.#####");
            }
            //數值寫回顯示欄
            dataGridView1.Rows[Row].Cells[Col].Value = val.ToString(Units.DisplayFmt);


            //寫回引數
            a.Value = val;

        }
        private void Btn_NumClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            if (la_NumVal.Text == "0")
            {
                la_NumVal.Text = btn.Text;
            }
            else if (la_NumVal.Text == "-0")
            {
                la_NumVal.Text = "-" + btn.Text;
            }
            else
            {
                la_NumVal.Text += btn.Text;
            }

            if (la_NumVal.Text != "0")
            {
                if (bNumSigned && !la_NumVal.Text.Contains('-'))
                {
                    la_NumVal.Text = "-" + la_NumVal.Text;
                }
            }
        }
        private void btn_Sub_Click(object sender, EventArgs e)
        {
            bNumSigned = !bNumSigned;
            if (la_NumVal.Text != "0")
            {
                if (bNumSigned)
                {
                    la_NumVal.Text = "-" + la_NumVal.Text;
                }
                else
                {
                    la_NumVal.Text = la_NumVal.Text.Replace("-", "");
                }
            }
            if (la_NumVal.Text == "0")
            {
                if (bNumSigned)
                {
                    la_NumVal.Text = "-" + la_NumVal.Text;
                }
                else
                {
                    la_NumVal.Text = la_NumVal.Text.Replace("-", "");
                }
            }
        }

        private void btn_Dot_Click(object sender, EventArgs e)
        {
            if (!la_NumVal.Text.Contains('.')) la_NumVal.Text = la_NumVal.Text + ".";

            if (la_NumVal.Text != "0")
            {
                if (bNumSigned && !la_NumVal.Text.Contains('-'))
                {
                    la_NumVal.Text = "-" + la_NumVal.Text;
                }
            }
        }
    }
}
