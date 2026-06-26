namespace OCD
{
    partial class Fo_SetRPM
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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.la_RPM = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_N100 = new System.Windows.Forms.Button();
            this.btn_N10 = new System.Windows.Forms.Button();
            this.btn_P10 = new System.Windows.Forms.Button();
            this.btn_P100 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(16, 80);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(432, 45);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(400, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "RPM";
            // 
            // la_RPM
            // 
            this.la_RPM.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.la_RPM.ForeColor = System.Drawing.Color.White;
            this.la_RPM.Location = new System.Drawing.Point(104, 16);
            this.la_RPM.Name = "la_RPM";
            this.la_RPM.Size = new System.Drawing.Size(248, 44);
            this.la_RPM.TabIndex = 2;
            this.la_RPM.Text = "0000";
            this.la_RPM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Lime;
            this.btn_Save.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Location = new System.Drawing.Point(248, 224);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(128, 72);
            this.btn_Save.TabIndex = 3;
            this.btn_Save.Text = "設定";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Red;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(88, 224);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 72);
            this.button1.TabIndex = 4;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_N100
            // 
            this.btn_N100.Location = new System.Drawing.Point(32, 136);
            this.btn_N100.Name = "btn_N100";
            this.btn_N100.Size = new System.Drawing.Size(88, 56);
            this.btn_N100.TabIndex = 5;
            this.btn_N100.Text = "<<";
            this.btn_N100.UseVisualStyleBackColor = true;
            this.btn_N100.Click += new System.EventHandler(this.btn_N100_Click);
            // 
            // btn_N10
            // 
            this.btn_N10.Location = new System.Drawing.Point(136, 136);
            this.btn_N10.Name = "btn_N10";
            this.btn_N10.Size = new System.Drawing.Size(88, 56);
            this.btn_N10.TabIndex = 6;
            this.btn_N10.Text = "<";
            this.btn_N10.UseVisualStyleBackColor = true;
            this.btn_N10.Click += new System.EventHandler(this.btn_N10_Click);
            // 
            // btn_P10
            // 
            this.btn_P10.Location = new System.Drawing.Point(240, 136);
            this.btn_P10.Name = "btn_P10";
            this.btn_P10.Size = new System.Drawing.Size(88, 56);
            this.btn_P10.TabIndex = 7;
            this.btn_P10.Text = ">";
            this.btn_P10.UseVisualStyleBackColor = true;
            this.btn_P10.Click += new System.EventHandler(this.btn_P10_Click);
            // 
            // btn_P100
            // 
            this.btn_P100.Location = new System.Drawing.Point(344, 136);
            this.btn_P100.Name = "btn_P100";
            this.btn_P100.Size = new System.Drawing.Size(88, 56);
            this.btn_P100.TabIndex = 8;
            this.btn_P100.Text = ">>";
            this.btn_P100.UseVisualStyleBackColor = true;
            this.btn_P100.Click += new System.EventHandler(this.btn_P100_Click);
            // 
            // Fo_SetRPM
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(478, 362);
            this.Controls.Add(this.btn_P100);
            this.Controls.Add(this.btn_P10);
            this.Controls.Add(this.btn_N10);
            this.Controls.Add(this.btn_N100);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.la_RPM);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar1);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Fo_SetRPM";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "轉速設定";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TrackBar trackBar1;
        public System.Windows.Forms.Label la_RPM;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_N100;
        private System.Windows.Forms.Button btn_N10;
        private System.Windows.Forms.Button btn_P10;
        private System.Windows.Forms.Button btn_P100;
    }
}