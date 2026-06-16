namespace OIG
{
    partial class Fo_TraverseStep
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fo_TraverseStep));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cb_Enabled = new System.Windows.Forms.ComboBox();
            this.la_Enabled = new System.Windows.Forms.Label();
            this.la_Step = new System.Windows.Forms.Label();
            this.cb_Step = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Col_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_XM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_ZM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Arg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Back = new Uc_RoundBtn();
            this.btn_NumClear = new System.Windows.Forms.Button();
            this.btn_Sub = new System.Windows.Forms.Button();
            this.la_NumVal = new System.Windows.Forms.Label();
            this.btn_UseCurrentPos = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Num0 = new System.Windows.Forms.Button();
            this.btn_Dot = new System.Windows.Forms.Button();
            this.btn_Num3 = new System.Windows.Forms.Button();
            this.btn_Num2 = new System.Windows.Forms.Button();
            this.btn_Num1 = new System.Windows.Forms.Button();
            this.btn_Num6 = new System.Windows.Forms.Button();
            this.btn_Num5 = new System.Windows.Forms.Button();
            this.btn_Num4 = new System.Windows.Forms.Button();
            this.btn_Num9 = new System.Windows.Forms.Button();
            this.btn_Num8 = new System.Windows.Forms.Button();
            this.btn_Num7 = new System.Windows.Forms.Button();
            this.btn_Backspace = new System.Windows.Forms.Button();
            this.pa_EditMach = new System.Windows.Forms.Panel();
            this.la_EditMachAxis2Value = new System.Windows.Forms.Label();
            this.la_EditMachAxis2 = new System.Windows.Forms.Label();
            this.la_EditMachAxis1Value = new System.Windows.Forms.Label();
            this.la_EditMachAxis1 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.pa_EditAbs = new System.Windows.Forms.Panel();
            this.la_EditAbsAxis2Value = new System.Windows.Forms.Label();
            this.la_EditAbsAxis2 = new System.Windows.Forms.Label();
            this.la_EditAbsAxis1Value = new System.Windows.Forms.Label();
            this.la_EditAbsAxis1 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.pa_EditMach.SuspendLayout();
            this.pa_EditAbs.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 296);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // cb_Enabled
            // 
            this.cb_Enabled.FormattingEnabled = true;
            this.cb_Enabled.Items.AddRange(new object[] {
            "不使用",
            "使用"});
            this.cb_Enabled.Location = new System.Drawing.Point(528, 128);
            this.cb_Enabled.Name = "cb_Enabled";
            this.cb_Enabled.Size = new System.Drawing.Size(121, 28);
            this.cb_Enabled.TabIndex = 1;
            // 
            // la_Enabled
            // 
            this.la_Enabled.AutoSize = true;
            this.la_Enabled.BackColor = System.Drawing.Color.Transparent;
            this.la_Enabled.Location = new System.Drawing.Point(528, 104);
            this.la_Enabled.Name = "la_Enabled";
            this.la_Enabled.Size = new System.Drawing.Size(121, 20);
            this.la_Enabled.TabIndex = 2;
            this.la_Enabled.Text = "多段橫進刀功能";
            // 
            // la_Step
            // 
            this.la_Step.AutoSize = true;
            this.la_Step.BackColor = System.Drawing.Color.Transparent;
            this.la_Step.Location = new System.Drawing.Point(528, 168);
            this.la_Step.Name = "la_Step";
            this.la_Step.Size = new System.Drawing.Size(41, 20);
            this.la_Step.TabIndex = 4;
            this.la_Step.Text = "段數";
            // 
            // cb_Step
            // 
            this.cb_Step.FormattingEnabled = true;
            this.cb_Step.Items.AddRange(new object[] {
            "1段",
            "2段",
            "3段",
            "4段",
            "5段",
            "6段",
            "7段",
            "8段",
            "9段",
            "10段",
            "11段",
            "12段",
            "13段",
            "14段",
            "15段",
            "16段",
            "17段",
            "18段",
            "19段",
            "20段"});
            this.cb_Step.Location = new System.Drawing.Point(528, 192);
            this.cb_Step.Name = "cb_Step";
            this.cb_Step.Size = new System.Drawing.Size(121, 28);
            this.cb_Step.TabIndex = 3;
            this.cb_Step.SelectedIndexChanged += new System.EventHandler(this.cb_Step_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_No,
            this.Col_XM,
            this.Col_ZM,
            this.Col_Arg});
            this.dataGridView1.Location = new System.Drawing.Point(8, 312);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(512, 288);
            this.dataGridView1.TabIndex = 5;
            // 
            // Col_No
            // 
            this.Col_No.HeaderText = "段號";
            this.Col_No.Name = "Col_No";
            this.Col_No.ReadOnly = true;
            this.Col_No.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_XM
            // 
            this.Col_XM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_XM.HeaderText = "XO";
            this.Col_XM.Name = "Col_XM";
            this.Col_XM.ReadOnly = true;
            this.Col_XM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_ZM
            // 
            this.Col_ZM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_ZM.HeaderText = "ZO";
            this.Col_ZM.Name = "Col_ZM";
            this.Col_ZM.ReadOnly = true;
            this.Col_ZM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_Arg
            // 
            this.Col_Arg.HeaderText = "物件(隱藏)";
            this.Col_Arg.Name = "Col_Arg";
            this.Col_Arg.Visible = false;
            // 
            // btn_Back
            // 
            this.btn_Back.BackColor = System.Drawing.Color.Transparent;
            this.btn_Back.DisplayText = "返回";
            this.btn_Back.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Back.ForeColor = System.Drawing.Color.White;
            this.btn_Back.Image = null;
            this.btn_Back.Lamp = false;
            this.btn_Back.LampOnImage = null;
            this.btn_Back.Location = new System.Drawing.Point(864, 528);
            this.btn_Back.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Back.MouseDownImage")));
            this.btn_Back.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Back.MouseUpImage")));
            this.btn_Back.Name = "btn_Back";
            this.btn_Back.Size = new System.Drawing.Size(72, 72);
            this.btn_Back.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Back.Switch = false;
            this.btn_Back.TabIndex = 52;
            this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
            // 
            // btn_NumClear
            // 
            this.btn_NumClear.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_NumClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_NumClear.Location = new System.Drawing.Point(784, 328);
            this.btn_NumClear.Name = "btn_NumClear";
            this.btn_NumClear.Size = new System.Drawing.Size(152, 60);
            this.btn_NumClear.TabIndex = 83;
            this.btn_NumClear.Text = "C";
            this.btn_NumClear.UseVisualStyleBackColor = true;
            this.btn_NumClear.Click += new System.EventHandler(this.btn_NumClear_Click);
            // 
            // btn_Sub
            // 
            this.btn_Sub.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Sub.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Sub.Location = new System.Drawing.Point(720, 328);
            this.btn_Sub.Name = "btn_Sub";
            this.btn_Sub.Size = new System.Drawing.Size(64, 60);
            this.btn_Sub.TabIndex = 82;
            this.btn_Sub.Text = "+/-";
            this.btn_Sub.UseVisualStyleBackColor = true;
            this.btn_Sub.Click += new System.EventHandler(this.btn_Sub_Click);
            // 
            // la_NumVal
            // 
            this.la_NumVal.BackColor = System.Drawing.Color.White;
            this.la_NumVal.Font = new System.Drawing.Font("Consolas", 36F, System.Drawing.FontStyle.Bold);
            this.la_NumVal.ForeColor = System.Drawing.Color.Black;
            this.la_NumVal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_NumVal.Location = new System.Drawing.Point(528, 272);
            this.la_NumVal.Name = "la_NumVal";
            this.la_NumVal.Size = new System.Drawing.Size(254, 55);
            this.la_NumVal.TabIndex = 67;
            this.la_NumVal.Text = "0";
            this.la_NumVal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btn_UseCurrentPos
            // 
            this.btn_UseCurrentPos.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold);
            this.btn_UseCurrentPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_UseCurrentPos.Location = new System.Drawing.Point(784, 456);
            this.btn_UseCurrentPos.Name = "btn_UseCurrentPos";
            this.btn_UseCurrentPos.Size = new System.Drawing.Size(152, 60);
            this.btn_UseCurrentPos.TabIndex = 80;
            this.btn_UseCurrentPos.Text = "記憶";
            this.btn_UseCurrentPos.UseVisualStyleBackColor = true;
            this.btn_UseCurrentPos.Click += new System.EventHandler(this.btn_UseCurrentPos_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold);
            this.btn_OK.ForeColor = System.Drawing.Color.Blue;
            this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_OK.Location = new System.Drawing.Point(784, 392);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(152, 60);
            this.btn_OK.TabIndex = 79;
            this.btn_OK.Text = "確認";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Num0
            // 
            this.btn_Num0.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num0.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num0.Location = new System.Drawing.Point(720, 456);
            this.btn_Num0.Name = "btn_Num0";
            this.btn_Num0.Size = new System.Drawing.Size(64, 60);
            this.btn_Num0.TabIndex = 78;
            this.btn_Num0.Text = "0";
            this.btn_Num0.UseVisualStyleBackColor = true;
            this.btn_Num0.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Dot
            // 
            this.btn_Dot.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Dot.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Dot.Location = new System.Drawing.Point(720, 392);
            this.btn_Dot.Name = "btn_Dot";
            this.btn_Dot.Size = new System.Drawing.Size(64, 60);
            this.btn_Dot.TabIndex = 77;
            this.btn_Dot.Text = ".";
            this.btn_Dot.UseVisualStyleBackColor = true;
            this.btn_Dot.Click += new System.EventHandler(this.btn_Dot_Click);
            // 
            // btn_Num3
            // 
            this.btn_Num3.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num3.Location = new System.Drawing.Point(656, 328);
            this.btn_Num3.Name = "btn_Num3";
            this.btn_Num3.Size = new System.Drawing.Size(64, 60);
            this.btn_Num3.TabIndex = 76;
            this.btn_Num3.Text = "3";
            this.btn_Num3.UseVisualStyleBackColor = true;
            this.btn_Num3.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num2
            // 
            this.btn_Num2.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num2.Location = new System.Drawing.Point(592, 328);
            this.btn_Num2.Name = "btn_Num2";
            this.btn_Num2.Size = new System.Drawing.Size(64, 60);
            this.btn_Num2.TabIndex = 75;
            this.btn_Num2.Text = "2";
            this.btn_Num2.UseVisualStyleBackColor = true;
            this.btn_Num2.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num1
            // 
            this.btn_Num1.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num1.Location = new System.Drawing.Point(528, 328);
            this.btn_Num1.Name = "btn_Num1";
            this.btn_Num1.Size = new System.Drawing.Size(64, 60);
            this.btn_Num1.TabIndex = 74;
            this.btn_Num1.Text = "1";
            this.btn_Num1.UseVisualStyleBackColor = true;
            this.btn_Num1.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num6
            // 
            this.btn_Num6.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num6.Location = new System.Drawing.Point(656, 392);
            this.btn_Num6.Name = "btn_Num6";
            this.btn_Num6.Size = new System.Drawing.Size(64, 60);
            this.btn_Num6.TabIndex = 73;
            this.btn_Num6.Text = "6";
            this.btn_Num6.UseVisualStyleBackColor = true;
            this.btn_Num6.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num5
            // 
            this.btn_Num5.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num5.Location = new System.Drawing.Point(592, 392);
            this.btn_Num5.Name = "btn_Num5";
            this.btn_Num5.Size = new System.Drawing.Size(64, 60);
            this.btn_Num5.TabIndex = 72;
            this.btn_Num5.Text = "5";
            this.btn_Num5.UseVisualStyleBackColor = true;
            this.btn_Num5.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num4
            // 
            this.btn_Num4.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num4.Location = new System.Drawing.Point(528, 392);
            this.btn_Num4.Name = "btn_Num4";
            this.btn_Num4.Size = new System.Drawing.Size(64, 60);
            this.btn_Num4.TabIndex = 71;
            this.btn_Num4.Text = "4";
            this.btn_Num4.UseVisualStyleBackColor = true;
            this.btn_Num4.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num9
            // 
            this.btn_Num9.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num9.Location = new System.Drawing.Point(656, 456);
            this.btn_Num9.Name = "btn_Num9";
            this.btn_Num9.Size = new System.Drawing.Size(64, 60);
            this.btn_Num9.TabIndex = 70;
            this.btn_Num9.Text = "9";
            this.btn_Num9.UseVisualStyleBackColor = true;
            this.btn_Num9.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num8
            // 
            this.btn_Num8.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num8.Location = new System.Drawing.Point(592, 456);
            this.btn_Num8.Name = "btn_Num8";
            this.btn_Num8.Size = new System.Drawing.Size(64, 60);
            this.btn_Num8.TabIndex = 69;
            this.btn_Num8.Text = "8";
            this.btn_Num8.UseVisualStyleBackColor = true;
            this.btn_Num8.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Num7
            // 
            this.btn_Num7.Font = new System.Drawing.Font("Consolas", 36F);
            this.btn_Num7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Num7.Location = new System.Drawing.Point(528, 456);
            this.btn_Num7.Name = "btn_Num7";
            this.btn_Num7.Size = new System.Drawing.Size(64, 60);
            this.btn_Num7.TabIndex = 68;
            this.btn_Num7.Text = "7";
            this.btn_Num7.UseVisualStyleBackColor = true;
            this.btn_Num7.Click += new System.EventHandler(this.Btn_NumClick);
            // 
            // btn_Backspace
            // 
            this.btn_Backspace.BackColor = System.Drawing.Color.Transparent;
            this.btn_Backspace.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.btn_Backspace.Image = ((System.Drawing.Image)(resources.GetObject("btn_Backspace.Image")));
            this.btn_Backspace.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Backspace.Location = new System.Drawing.Point(784, 264);
            this.btn_Backspace.Name = "btn_Backspace";
            this.btn_Backspace.Size = new System.Drawing.Size(152, 60);
            this.btn_Backspace.TabIndex = 66;
            this.btn_Backspace.UseVisualStyleBackColor = false;
            this.btn_Backspace.Click += new System.EventHandler(this.btn_Backspace_Click);
            // 
            // pa_EditMach
            // 
            this.pa_EditMach.BackColor = System.Drawing.Color.Black;
            this.pa_EditMach.Controls.Add(this.la_EditMachAxis2Value);
            this.pa_EditMach.Controls.Add(this.la_EditMachAxis2);
            this.pa_EditMach.Controls.Add(this.la_EditMachAxis1Value);
            this.pa_EditMach.Controls.Add(this.la_EditMachAxis1);
            this.pa_EditMach.Controls.Add(this.label33);
            this.pa_EditMach.Location = new System.Drawing.Point(736, 8);
            this.pa_EditMach.Name = "pa_EditMach";
            this.pa_EditMach.Size = new System.Drawing.Size(200, 80);
            this.pa_EditMach.TabIndex = 102;
            // 
            // la_EditMachAxis2Value
            // 
            this.la_EditMachAxis2Value.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditMachAxis2Value.ForeColor = System.Drawing.Color.Yellow;
            this.la_EditMachAxis2Value.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditMachAxis2Value.Location = new System.Drawing.Point(40, 56);
            this.la_EditMachAxis2Value.Name = "la_EditMachAxis2Value";
            this.la_EditMachAxis2Value.Size = new System.Drawing.Size(160, 23);
            this.la_EditMachAxis2Value.TabIndex = 18;
            this.la_EditMachAxis2Value.Text = "0.0000";
            this.la_EditMachAxis2Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // la_EditMachAxis2
            // 
            this.la_EditMachAxis2.AutoSize = true;
            this.la_EditMachAxis2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditMachAxis2.ForeColor = System.Drawing.Color.White;
            this.la_EditMachAxis2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditMachAxis2.Location = new System.Drawing.Point(8, 56);
            this.la_EditMachAxis2.Name = "la_EditMachAxis2";
            this.la_EditMachAxis2.Size = new System.Drawing.Size(22, 24);
            this.la_EditMachAxis2.TabIndex = 17;
            this.la_EditMachAxis2.Text = "Z";
            // 
            // la_EditMachAxis1Value
            // 
            this.la_EditMachAxis1Value.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditMachAxis1Value.ForeColor = System.Drawing.Color.Yellow;
            this.la_EditMachAxis1Value.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditMachAxis1Value.Location = new System.Drawing.Point(40, 32);
            this.la_EditMachAxis1Value.Name = "la_EditMachAxis1Value";
            this.la_EditMachAxis1Value.Size = new System.Drawing.Size(160, 23);
            this.la_EditMachAxis1Value.TabIndex = 16;
            this.la_EditMachAxis1Value.Text = "0.0000";
            this.la_EditMachAxis1Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // la_EditMachAxis1
            // 
            this.la_EditMachAxis1.AutoSize = true;
            this.la_EditMachAxis1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditMachAxis1.ForeColor = System.Drawing.Color.White;
            this.la_EditMachAxis1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditMachAxis1.Location = new System.Drawing.Point(8, 32);
            this.la_EditMachAxis1.Name = "la_EditMachAxis1";
            this.la_EditMachAxis1.Size = new System.Drawing.Size(23, 24);
            this.la_EditMachAxis1.TabIndex = 15;
            this.la_EditMachAxis1.Text = "X";
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.LightSlateGray;
            this.label33.Dock = System.Windows.Forms.DockStyle.Top;
            this.label33.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold);
            this.label33.ForeColor = System.Drawing.Color.Lavender;
            this.label33.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label33.Location = new System.Drawing.Point(0, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(200, 32);
            this.label33.TabIndex = 1;
            this.label33.Text = "機械座標";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pa_EditAbs
            // 
            this.pa_EditAbs.BackColor = System.Drawing.Color.Black;
            this.pa_EditAbs.Controls.Add(this.la_EditAbsAxis2Value);
            this.pa_EditAbs.Controls.Add(this.la_EditAbsAxis2);
            this.pa_EditAbs.Controls.Add(this.la_EditAbsAxis1Value);
            this.pa_EditAbs.Controls.Add(this.la_EditAbsAxis1);
            this.pa_EditAbs.Controls.Add(this.label26);
            this.pa_EditAbs.Location = new System.Drawing.Point(528, 8);
            this.pa_EditAbs.Name = "pa_EditAbs";
            this.pa_EditAbs.Size = new System.Drawing.Size(200, 80);
            this.pa_EditAbs.TabIndex = 103;
            // 
            // la_EditAbsAxis2Value
            // 
            this.la_EditAbsAxis2Value.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditAbsAxis2Value.ForeColor = System.Drawing.Color.Yellow;
            this.la_EditAbsAxis2Value.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditAbsAxis2Value.Location = new System.Drawing.Point(40, 56);
            this.la_EditAbsAxis2Value.Name = "la_EditAbsAxis2Value";
            this.la_EditAbsAxis2Value.Size = new System.Drawing.Size(160, 23);
            this.la_EditAbsAxis2Value.TabIndex = 24;
            this.la_EditAbsAxis2Value.Text = "0.0000";
            this.la_EditAbsAxis2Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // la_EditAbsAxis2
            // 
            this.la_EditAbsAxis2.AutoSize = true;
            this.la_EditAbsAxis2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditAbsAxis2.ForeColor = System.Drawing.Color.White;
            this.la_EditAbsAxis2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditAbsAxis2.Location = new System.Drawing.Point(8, 56);
            this.la_EditAbsAxis2.Name = "la_EditAbsAxis2";
            this.la_EditAbsAxis2.Size = new System.Drawing.Size(22, 24);
            this.la_EditAbsAxis2.TabIndex = 23;
            this.la_EditAbsAxis2.Text = "Z";
            // 
            // la_EditAbsAxis1Value
            // 
            this.la_EditAbsAxis1Value.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditAbsAxis1Value.ForeColor = System.Drawing.Color.Yellow;
            this.la_EditAbsAxis1Value.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditAbsAxis1Value.Location = new System.Drawing.Point(40, 32);
            this.la_EditAbsAxis1Value.Name = "la_EditAbsAxis1Value";
            this.la_EditAbsAxis1Value.Size = new System.Drawing.Size(160, 23);
            this.la_EditAbsAxis1Value.TabIndex = 22;
            this.la_EditAbsAxis1Value.Text = "0.0000";
            this.la_EditAbsAxis1Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // la_EditAbsAxis1
            // 
            this.la_EditAbsAxis1.AutoSize = true;
            this.la_EditAbsAxis1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.la_EditAbsAxis1.ForeColor = System.Drawing.Color.White;
            this.la_EditAbsAxis1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_EditAbsAxis1.Location = new System.Drawing.Point(8, 32);
            this.la_EditAbsAxis1.Name = "la_EditAbsAxis1";
            this.la_EditAbsAxis1.Size = new System.Drawing.Size(23, 24);
            this.la_EditAbsAxis1.TabIndex = 21;
            this.la_EditAbsAxis1.Text = "X";
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.LightSlateGray;
            this.label26.Dock = System.Windows.Forms.DockStyle.Top;
            this.label26.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold);
            this.label26.ForeColor = System.Drawing.Color.Lavender;
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(0, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(200, 32);
            this.label26.TabIndex = 1;
            this.label26.Text = "絕對座標";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Fo_TraverseStep
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::OIG.Properties.Resources.background2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(944, 608);
            this.Controls.Add(this.pa_EditMach);
            this.Controls.Add(this.pa_EditAbs);
            this.Controls.Add(this.btn_NumClear);
            this.Controls.Add(this.btn_Sub);
            this.Controls.Add(this.la_NumVal);
            this.Controls.Add(this.btn_UseCurrentPos);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_Num0);
            this.Controls.Add(this.btn_Dot);
            this.Controls.Add(this.btn_Num3);
            this.Controls.Add(this.btn_Num2);
            this.Controls.Add(this.btn_Num1);
            this.Controls.Add(this.btn_Num6);
            this.Controls.Add(this.btn_Num5);
            this.Controls.Add(this.btn_Num4);
            this.Controls.Add(this.btn_Num9);
            this.Controls.Add(this.btn_Num8);
            this.Controls.Add(this.btn_Num7);
            this.Controls.Add(this.btn_Backspace);
            this.Controls.Add(this.btn_Back);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.la_Step);
            this.Controls.Add(this.cb_Step);
            this.Controls.Add(this.la_Enabled);
            this.Controls.Add(this.cb_Enabled);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Fo_TraverseStep";
            this.Text = "Fo_TraverseStep";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_TraverseStep_FormClosing);
            this.Load += new System.EventHandler(this.Fo_TraverseStep_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.pa_EditMach.ResumeLayout(false);
            this.pa_EditMach.PerformLayout();
            this.pa_EditAbs.ResumeLayout(false);
            this.pa_EditAbs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label la_Enabled;
        private System.Windows.Forms.Label la_Step;
        public System.Windows.Forms.ComboBox cb_Enabled;
        public System.Windows.Forms.ComboBox cb_Step;
        public System.Windows.Forms.DataGridView dataGridView1;
        private Uc_RoundBtn btn_Back;
        private System.Windows.Forms.Button btn_NumClear;
        private System.Windows.Forms.Button btn_Sub;
        public System.Windows.Forms.Label la_NumVal;
        private System.Windows.Forms.Button btn_UseCurrentPos;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Num0;
        private System.Windows.Forms.Button btn_Dot;
        private System.Windows.Forms.Button btn_Num3;
        private System.Windows.Forms.Button btn_Num2;
        private System.Windows.Forms.Button btn_Num1;
        private System.Windows.Forms.Button btn_Num6;
        private System.Windows.Forms.Button btn_Num5;
        private System.Windows.Forms.Button btn_Num4;
        private System.Windows.Forms.Button btn_Num9;
        private System.Windows.Forms.Button btn_Num8;
        private System.Windows.Forms.Button btn_Num7;
        private System.Windows.Forms.Button btn_Backspace;
        private System.Windows.Forms.Panel pa_EditMach;
        public System.Windows.Forms.Label la_EditMachAxis2Value;
        public System.Windows.Forms.Label la_EditMachAxis2;
        public System.Windows.Forms.Label la_EditMachAxis1Value;
        public System.Windows.Forms.Label la_EditMachAxis1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Panel pa_EditAbs;
        public System.Windows.Forms.Label la_EditAbsAxis2Value;
        public System.Windows.Forms.Label la_EditAbsAxis2;
        public System.Windows.Forms.Label la_EditAbsAxis1Value;
        public System.Windows.Forms.Label la_EditAbsAxis1;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_XM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_ZM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Arg;
    }
}