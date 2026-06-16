namespace OIG
{
    partial class Fo_Monitor_List
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
            this.ch_Absolute = new System.Windows.Forms.CheckBox();
            this.ch_Distance = new System.Windows.Forms.CheckBox();
            this.ch_Machine = new System.Windows.Forms.CheckBox();
            this.ch_Relative = new System.Windows.Forms.CheckBox();
            this.ch_Program = new System.Windows.Forms.CheckBox();
            this.ch_WorkInfo = new System.Windows.Forms.CheckBox();
            this.ch_GW1 = new System.Windows.Forms.CheckBox();
            this.ch_GW2 = new System.Windows.Forms.CheckBox();
            this.ch_SpindleComm = new System.Windows.Forms.CheckBox();
            this.ch_Roller = new System.Windows.Forms.CheckBox();
            this.ch_SpindleRate = new System.Windows.Forms.CheckBox();
            this.ch_SpindleCAxis = new System.Windows.Forms.CheckBox();
            this.ch_GW4 = new System.Windows.Forms.CheckBox();
            this.ch_GW3 = new System.Windows.Forms.CheckBox();
            this.ch_GrindMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ch_Absolute
            // 
            this.ch_Absolute.AutoSize = true;
            this.ch_Absolute.Location = new System.Drawing.Point(16, 24);
            this.ch_Absolute.Name = "ch_Absolute";
            this.ch_Absolute.Size = new System.Drawing.Size(72, 16);
            this.ch_Absolute.TabIndex = 0;
            this.ch_Absolute.Text = "絕對座標";
            this.ch_Absolute.UseVisualStyleBackColor = true;
            this.ch_Absolute.Click += new System.EventHandler(this.ch_Absolute_Click);
            // 
            // ch_Distance
            // 
            this.ch_Distance.AutoSize = true;
            this.ch_Distance.Location = new System.Drawing.Point(16, 48);
            this.ch_Distance.Name = "ch_Distance";
            this.ch_Distance.Size = new System.Drawing.Size(72, 16);
            this.ch_Distance.TabIndex = 1;
            this.ch_Distance.Text = "殘移動量";
            this.ch_Distance.UseVisualStyleBackColor = true;
            this.ch_Distance.Click += new System.EventHandler(this.ch_Distance_Click);
            // 
            // ch_Machine
            // 
            this.ch_Machine.AutoSize = true;
            this.ch_Machine.Location = new System.Drawing.Point(16, 72);
            this.ch_Machine.Name = "ch_Machine";
            this.ch_Machine.Size = new System.Drawing.Size(72, 16);
            this.ch_Machine.TabIndex = 2;
            this.ch_Machine.Text = "機械座標";
            this.ch_Machine.UseVisualStyleBackColor = true;
            this.ch_Machine.Click += new System.EventHandler(this.ch_Machine_Click);
            // 
            // ch_Relative
            // 
            this.ch_Relative.AutoSize = true;
            this.ch_Relative.Location = new System.Drawing.Point(16, 96);
            this.ch_Relative.Name = "ch_Relative";
            this.ch_Relative.Size = new System.Drawing.Size(72, 16);
            this.ch_Relative.TabIndex = 3;
            this.ch_Relative.Text = "相對座標";
            this.ch_Relative.UseVisualStyleBackColor = true;
            this.ch_Relative.Click += new System.EventHandler(this.ch_Relative_Click);
            // 
            // ch_Program
            // 
            this.ch_Program.AutoSize = true;
            this.ch_Program.Location = new System.Drawing.Point(16, 136);
            this.ch_Program.Name = "ch_Program";
            this.ch_Program.Size = new System.Drawing.Size(48, 16);
            this.ch_Program.TabIndex = 4;
            this.ch_Program.Text = "程式";
            this.ch_Program.UseVisualStyleBackColor = true;
            this.ch_Program.Click += new System.EventHandler(this.ch_Program_Click);
            // 
            // ch_WorkInfo
            // 
            this.ch_WorkInfo.AutoSize = true;
            this.ch_WorkInfo.Location = new System.Drawing.Point(16, 160);
            this.ch_WorkInfo.Name = "ch_WorkInfo";
            this.ch_WorkInfo.Size = new System.Drawing.Size(72, 16);
            this.ch_WorkInfo.TabIndex = 5;
            this.ch_WorkInfo.Text = "加工資訊";
            this.ch_WorkInfo.UseVisualStyleBackColor = true;
            this.ch_WorkInfo.Click += new System.EventHandler(this.ch_WorkInfo_Click);
            // 
            // ch_GW1
            // 
            this.ch_GW1.AutoSize = true;
            this.ch_GW1.Location = new System.Drawing.Point(120, 24);
            this.ch_GW1.Name = "ch_GW1";
            this.ch_GW1.Size = new System.Drawing.Size(54, 16);
            this.ch_GW1.TabIndex = 6;
            this.ch_GW1.Text = "砂輪1";
            this.ch_GW1.UseVisualStyleBackColor = true;
            this.ch_GW1.Click += new System.EventHandler(this.ch_GW1_Click);
            // 
            // ch_GW2
            // 
            this.ch_GW2.AutoSize = true;
            this.ch_GW2.Location = new System.Drawing.Point(120, 48);
            this.ch_GW2.Name = "ch_GW2";
            this.ch_GW2.Size = new System.Drawing.Size(54, 16);
            this.ch_GW2.TabIndex = 8;
            this.ch_GW2.Text = "砂輪2";
            this.ch_GW2.UseVisualStyleBackColor = true;
            this.ch_GW2.Click += new System.EventHandler(this.ch_GW2_Click);
            // 
            // ch_SpindleComm
            // 
            this.ch_SpindleComm.AutoSize = true;
            this.ch_SpindleComm.Location = new System.Drawing.Point(120, 160);
            this.ch_SpindleComm.Name = "ch_SpindleComm";
            this.ch_SpindleComm.Size = new System.Drawing.Size(80, 16);
            this.ch_SpindleComm.TabIndex = 10;
            this.ch_SpindleComm.Text = "主軸(通訊)";
            this.ch_SpindleComm.UseVisualStyleBackColor = true;
            this.ch_SpindleComm.Click += new System.EventHandler(this.ch_SpindleComm_Click);
            // 
            // ch_Roller
            // 
            this.ch_Roller.AutoSize = true;
            this.ch_Roller.Location = new System.Drawing.Point(120, 136);
            this.ch_Roller.Name = "ch_Roller";
            this.ch_Roller.Size = new System.Drawing.Size(48, 16);
            this.ch_Roller.TabIndex = 13;
            this.ch_Roller.Text = "滾輪";
            this.ch_Roller.UseVisualStyleBackColor = true;
            this.ch_Roller.Click += new System.EventHandler(this.ch_Roller_Click);
            // 
            // ch_SpindleRate
            // 
            this.ch_SpindleRate.AutoSize = true;
            this.ch_SpindleRate.Location = new System.Drawing.Point(120, 184);
            this.ch_SpindleRate.Name = "ch_SpindleRate";
            this.ch_SpindleRate.Size = new System.Drawing.Size(80, 16);
            this.ch_SpindleRate.TabIndex = 14;
            this.ch_SpindleRate.Text = "主軸(倍率)";
            this.ch_SpindleRate.UseVisualStyleBackColor = true;
            this.ch_SpindleRate.Click += new System.EventHandler(this.ch_SpindleRate_Click);
            // 
            // ch_SpindleCAxis
            // 
            this.ch_SpindleCAxis.AutoSize = true;
            this.ch_SpindleCAxis.Location = new System.Drawing.Point(120, 208);
            this.ch_SpindleCAxis.Name = "ch_SpindleCAxis";
            this.ch_SpindleCAxis.Size = new System.Drawing.Size(76, 16);
            this.ch_SpindleCAxis.TabIndex = 15;
            this.ch_SpindleCAxis.Text = "主軸(C軸)";
            this.ch_SpindleCAxis.UseVisualStyleBackColor = true;
            this.ch_SpindleCAxis.Click += new System.EventHandler(this.ch_SpindleCAxis_Click);
            // 
            // ch_GW4
            // 
            this.ch_GW4.AutoSize = true;
            this.ch_GW4.Location = new System.Drawing.Point(120, 96);
            this.ch_GW4.Name = "ch_GW4";
            this.ch_GW4.Size = new System.Drawing.Size(54, 16);
            this.ch_GW4.TabIndex = 17;
            this.ch_GW4.Text = "砂輪4";
            this.ch_GW4.UseVisualStyleBackColor = true;
            this.ch_GW4.Click += new System.EventHandler(this.ch_GW4_Click);
            // 
            // ch_GW3
            // 
            this.ch_GW3.AutoSize = true;
            this.ch_GW3.Location = new System.Drawing.Point(120, 72);
            this.ch_GW3.Name = "ch_GW3";
            this.ch_GW3.Size = new System.Drawing.Size(54, 16);
            this.ch_GW3.TabIndex = 16;
            this.ch_GW3.Text = "砂輪3";
            this.ch_GW3.UseVisualStyleBackColor = true;
            this.ch_GW3.Click += new System.EventHandler(this.ch_GW3_Click);
            // 
            // ch_GrindMode
            // 
            this.ch_GrindMode.AutoSize = true;
            this.ch_GrindMode.Location = new System.Drawing.Point(16, 184);
            this.ch_GrindMode.Name = "ch_GrindMode";
            this.ch_GrindMode.Size = new System.Drawing.Size(72, 16);
            this.ch_GrindMode.TabIndex = 18;
            this.ch_GrindMode.Text = "研磨模式";
            this.ch_GrindMode.UseVisualStyleBackColor = true;
            this.ch_GrindMode.Click += new System.EventHandler(this.ch_GrindMode_Click);
            // 
            // Fo_Monitor_List
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 339);
            this.Controls.Add(this.ch_GrindMode);
            this.Controls.Add(this.ch_GW4);
            this.Controls.Add(this.ch_GW3);
            this.Controls.Add(this.ch_SpindleCAxis);
            this.Controls.Add(this.ch_SpindleRate);
            this.Controls.Add(this.ch_Roller);
            this.Controls.Add(this.ch_SpindleComm);
            this.Controls.Add(this.ch_GW2);
            this.Controls.Add(this.ch_GW1);
            this.Controls.Add(this.ch_WorkInfo);
            this.Controls.Add(this.ch_Program);
            this.Controls.Add(this.ch_Relative);
            this.Controls.Add(this.ch_Machine);
            this.Controls.Add(this.ch_Distance);
            this.Controls.Add(this.ch_Absolute);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Fo_Monitor_List";
            this.ShowIcon = false;
            this.Text = "監視頁面";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_Monitor_List_FormClosing);
            this.Load += new System.EventHandler(this.Fo_Monitor_List_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ch_Absolute;
        private System.Windows.Forms.CheckBox ch_Distance;
        private System.Windows.Forms.CheckBox ch_Machine;
        private System.Windows.Forms.CheckBox ch_Relative;
        private System.Windows.Forms.CheckBox ch_Program;
        private System.Windows.Forms.CheckBox ch_WorkInfo;
        private System.Windows.Forms.CheckBox ch_GW1;
        private System.Windows.Forms.CheckBox ch_GW2;
        private System.Windows.Forms.CheckBox ch_SpindleComm;
        private System.Windows.Forms.CheckBox ch_Roller;
        private System.Windows.Forms.CheckBox ch_SpindleRate;
        private System.Windows.Forms.CheckBox ch_SpindleCAxis;
        private System.Windows.Forms.CheckBox ch_GW4;
        private System.Windows.Forms.CheckBox ch_GW3;
        private System.Windows.Forms.CheckBox ch_GrindMode;
    }
}