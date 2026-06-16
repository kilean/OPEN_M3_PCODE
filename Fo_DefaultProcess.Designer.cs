namespace OIG
{
    partial class Fo_DefaultProcess
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fo_DefaultProcess));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Col_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Page = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Default = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Unit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btn_Exit = new Uc_RoundBtn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_No,
            this.Col_Name,
            this.Col_Page,
            this.Col_Default,
            this.Col_Min,
            this.Col_Max,
            this.Col_Code,
            this.Col_Unit});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.Location = new System.Drawing.Point(16, 16);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(760, 736);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // Col_No
            // 
            this.Col_No.HeaderText = "#";
            this.Col_No.Name = "Col_No";
            this.Col_No.ReadOnly = true;
            this.Col_No.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Col_No.Width = 75;
            // 
            // Col_Name
            // 
            this.Col_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Col_Name.HeaderText = "說明";
            this.Col_Name.Name = "Col_Name";
            this.Col_Name.ReadOnly = true;
            this.Col_Name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Col_Name.Width = 150;
            // 
            // Col_Page
            // 
            this.Col_Page.HeaderText = "分頁";
            this.Col_Page.Name = "Col_Page";
            this.Col_Page.ReadOnly = true;
            this.Col_Page.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Col_Page.Width = 55;
            // 
            // Col_Default
            // 
            this.Col_Default.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_Default.HeaderText = "預設值";
            this.Col_Default.Name = "Col_Default";
            this.Col_Default.ReadOnly = true;
            this.Col_Default.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_Min
            // 
            this.Col_Min.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_Min.HeaderText = "最小值";
            this.Col_Min.Name = "Col_Min";
            this.Col_Min.ReadOnly = true;
            this.Col_Min.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_Max
            // 
            this.Col_Max.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_Max.HeaderText = "最大值";
            this.Col_Max.Name = "Col_Max";
            this.Col_Max.ReadOnly = true;
            this.Col_Max.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_Code
            // 
            this.Col_Code.HeaderText = "代碼";
            this.Col_Code.Name = "Col_Code";
            this.Col_Code.ReadOnly = true;
            this.Col_Code.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Col_Code.Width = 55;
            // 
            // Col_Unit
            // 
            this.Col_Unit.HeaderText = "單位";
            this.Col_Unit.Name = "Col_Unit";
            this.Col_Unit.ReadOnly = true;
            this.Col_Unit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Location = new System.Drawing.Point(784, 16);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(216, 196);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            // 
            // btn_Exit
            // 
            this.btn_Exit.BackColor = System.Drawing.Color.Transparent;
            this.btn_Exit.DisplayText = "離開";
            this.btn_Exit.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_Exit.ForeColor = System.Drawing.Color.White;
            this.btn_Exit.Image = null;
            this.btn_Exit.Lamp = false;
            this.btn_Exit.LampOnImage = null;
            this.btn_Exit.Location = new System.Drawing.Point(912, 664);
            this.btn_Exit.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Exit.MouseDownImage")));
            this.btn_Exit.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Exit.MouseUpImage")));
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(88, 88);
            this.btn_Exit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Exit.Switch = false;
            this.btn_Exit.TabIndex = 82;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // Fo_DefaultProcess
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Fo_DefaultProcess";
            this.Text = "Fo_DefaultProcess";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_DefaultProcess_FormClosing);
            this.Load += new System.EventHandler(this.Fo_DefaultProcess_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ListBox listBox1;
        private Uc_RoundBtn btn_Exit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Page;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Default;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Min;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Max;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Unit;
    }
}