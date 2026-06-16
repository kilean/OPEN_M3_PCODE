namespace OIG
{
    partial class Fo_Layout
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fo_Layout));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Left = new System.Windows.Forms.TextBox();
            this.tb_Top = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tb_Height = new System.Windows.Forms.TextBox();
            this.tb_Width = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_LeftDec = new System.Windows.Forms.Button();
            this.btn_LeftInc = new System.Windows.Forms.Button();
            this.btn_TopInc = new System.Windows.Forms.Button();
            this.btn_TopDec = new System.Windows.Forms.Button();
            this.btn_WidthInc = new System.Windows.Forms.Button();
            this.btn_WidthDec = new System.Windows.Forms.Button();
            this.btn_HeightInc = new System.Windows.Forms.Button();
            this.btn_HeightDec = new System.Windows.Forms.Button();
            this.ch_Grid = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Top";
            // 
            // tb_Left
            // 
            this.tb_Left.Location = new System.Drawing.Point(120, 40);
            this.tb_Left.Name = "tb_Left";
            this.tb_Left.Size = new System.Drawing.Size(100, 29);
            this.tb_Left.TabIndex = 2;
            this.tb_Left.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_Left_KeyDown);
            this.tb_Left.Leave += new System.EventHandler(this.tb_Left_Leave);
            // 
            // tb_Top
            // 
            this.tb_Top.Location = new System.Drawing.Point(120, 88);
            this.tb_Top.Name = "tb_Top";
            this.tb_Top.Size = new System.Drawing.Size(100, 29);
            this.tb_Top.TabIndex = 3;
            this.tb_Top.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_Top_KeyDown);
            this.tb_Top.Leave += new System.EventHandler(this.tb_Top_Leave);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(8, 8);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(60, 24);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "啟用";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // tb_Height
            // 
            this.tb_Height.Location = new System.Drawing.Point(120, 192);
            this.tb_Height.Name = "tb_Height";
            this.tb_Height.Size = new System.Drawing.Size(100, 29);
            this.tb_Height.TabIndex = 5;
            this.tb_Height.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_Height_KeyDown);
            this.tb_Height.Leave += new System.EventHandler(this.tb_Height_Leave);
            // 
            // tb_Width
            // 
            this.tb_Width.Location = new System.Drawing.Point(120, 144);
            this.tb_Width.Name = "tb_Width";
            this.tb_Width.Size = new System.Drawing.Size(100, 29);
            this.tb_Width.TabIndex = 4;
            this.tb_Width.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_Width_KeyDown);
            this.tb_Width.Leave += new System.EventHandler(this.tb_Width_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Height";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Width";
            // 
            // btn_LeftDec
            // 
            this.btn_LeftDec.Image = ((System.Drawing.Image)(resources.GetObject("btn_LeftDec.Image")));
            this.btn_LeftDec.Location = new System.Drawing.Point(224, 40);
            this.btn_LeftDec.Name = "btn_LeftDec";
            this.btn_LeftDec.Size = new System.Drawing.Size(48, 40);
            this.btn_LeftDec.TabIndex = 7;
            this.btn_LeftDec.UseVisualStyleBackColor = true;
            this.btn_LeftDec.Click += new System.EventHandler(this.btn_LeftDec_Click);
            // 
            // btn_LeftInc
            // 
            this.btn_LeftInc.Image = ((System.Drawing.Image)(resources.GetObject("btn_LeftInc.Image")));
            this.btn_LeftInc.Location = new System.Drawing.Point(280, 40);
            this.btn_LeftInc.Name = "btn_LeftInc";
            this.btn_LeftInc.Size = new System.Drawing.Size(48, 40);
            this.btn_LeftInc.TabIndex = 8;
            this.btn_LeftInc.UseVisualStyleBackColor = true;
            this.btn_LeftInc.Click += new System.EventHandler(this.btn_LeftInc_Click);
            // 
            // btn_TopInc
            // 
            this.btn_TopInc.Image = ((System.Drawing.Image)(resources.GetObject("btn_TopInc.Image")));
            this.btn_TopInc.Location = new System.Drawing.Point(280, 88);
            this.btn_TopInc.Name = "btn_TopInc";
            this.btn_TopInc.Size = new System.Drawing.Size(48, 40);
            this.btn_TopInc.TabIndex = 10;
            this.btn_TopInc.UseVisualStyleBackColor = true;
            this.btn_TopInc.Click += new System.EventHandler(this.btn_TopInc_Click);
            // 
            // btn_TopDec
            // 
            this.btn_TopDec.Image = ((System.Drawing.Image)(resources.GetObject("btn_TopDec.Image")));
            this.btn_TopDec.Location = new System.Drawing.Point(224, 88);
            this.btn_TopDec.Name = "btn_TopDec";
            this.btn_TopDec.Size = new System.Drawing.Size(48, 40);
            this.btn_TopDec.TabIndex = 9;
            this.btn_TopDec.UseVisualStyleBackColor = true;
            this.btn_TopDec.Click += new System.EventHandler(this.btn_TopDec_Click);
            // 
            // btn_WidthInc
            // 
            this.btn_WidthInc.Image = ((System.Drawing.Image)(resources.GetObject("btn_WidthInc.Image")));
            this.btn_WidthInc.Location = new System.Drawing.Point(280, 144);
            this.btn_WidthInc.Name = "btn_WidthInc";
            this.btn_WidthInc.Size = new System.Drawing.Size(48, 40);
            this.btn_WidthInc.TabIndex = 12;
            this.btn_WidthInc.UseVisualStyleBackColor = true;
            this.btn_WidthInc.Click += new System.EventHandler(this.btn_WidthInc_Click);
            // 
            // btn_WidthDec
            // 
            this.btn_WidthDec.Image = ((System.Drawing.Image)(resources.GetObject("btn_WidthDec.Image")));
            this.btn_WidthDec.Location = new System.Drawing.Point(224, 144);
            this.btn_WidthDec.Name = "btn_WidthDec";
            this.btn_WidthDec.Size = new System.Drawing.Size(48, 40);
            this.btn_WidthDec.TabIndex = 11;
            this.btn_WidthDec.UseVisualStyleBackColor = true;
            this.btn_WidthDec.Click += new System.EventHandler(this.btn_WidthDec_Click);
            // 
            // btn_HeightInc
            // 
            this.btn_HeightInc.Image = ((System.Drawing.Image)(resources.GetObject("btn_HeightInc.Image")));
            this.btn_HeightInc.Location = new System.Drawing.Point(280, 192);
            this.btn_HeightInc.Name = "btn_HeightInc";
            this.btn_HeightInc.Size = new System.Drawing.Size(48, 40);
            this.btn_HeightInc.TabIndex = 14;
            this.btn_HeightInc.UseVisualStyleBackColor = true;
            this.btn_HeightInc.Click += new System.EventHandler(this.btn_HeightInc_Click);
            // 
            // btn_HeightDec
            // 
            this.btn_HeightDec.Image = ((System.Drawing.Image)(resources.GetObject("btn_HeightDec.Image")));
            this.btn_HeightDec.Location = new System.Drawing.Point(224, 192);
            this.btn_HeightDec.Name = "btn_HeightDec";
            this.btn_HeightDec.Size = new System.Drawing.Size(48, 40);
            this.btn_HeightDec.TabIndex = 13;
            this.btn_HeightDec.UseVisualStyleBackColor = true;
            this.btn_HeightDec.Click += new System.EventHandler(this.btn_HeightDec_Click);
            // 
            // ch_Grid
            // 
            this.ch_Grid.AutoSize = true;
            this.ch_Grid.Location = new System.Drawing.Point(120, 8);
            this.ch_Grid.Name = "ch_Grid";
            this.ch_Grid.Size = new System.Drawing.Size(60, 24);
            this.ch_Grid.TabIndex = 15;
            this.ch_Grid.Text = "Grid";
            this.ch_Grid.UseVisualStyleBackColor = true;
            this.ch_Grid.CheckedChanged += new System.EventHandler(this.ch_Grid_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Fo_Layout
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(342, 243);
            this.Controls.Add(this.ch_Grid);
            this.Controls.Add(this.btn_HeightInc);
            this.Controls.Add(this.btn_HeightDec);
            this.Controls.Add(this.btn_WidthInc);
            this.Controls.Add(this.btn_WidthDec);
            this.Controls.Add(this.btn_TopInc);
            this.Controls.Add(this.btn_TopDec);
            this.Controls.Add(this.btn_LeftInc);
            this.Controls.Add(this.btn_LeftDec);
            this.Controls.Add(this.tb_Height);
            this.Controls.Add(this.tb_Width);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.tb_Top);
            this.Controls.Add(this.tb_Left);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Fo_Layout";
            this.ShowIcon = false;
            this.Text = "Layout";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_Layout_FormClosing);
            this.Load += new System.EventHandler(this.Fo_Layout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tb_Left;
        public System.Windows.Forms.TextBox tb_Top;
        public System.Windows.Forms.TextBox tb_Height;
        public System.Windows.Forms.TextBox tb_Width;
        private System.Windows.Forms.Button btn_LeftDec;
        private System.Windows.Forms.Button btn_LeftInc;
        private System.Windows.Forms.Button btn_TopInc;
        private System.Windows.Forms.Button btn_TopDec;
        private System.Windows.Forms.Button btn_WidthInc;
        private System.Windows.Forms.Button btn_WidthDec;
        private System.Windows.Forms.Button btn_HeightInc;
        private System.Windows.Forms.Button btn_HeightDec;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.CheckBox ch_Grid;
    }
}