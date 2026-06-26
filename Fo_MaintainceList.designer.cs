namespace OCD
{
    partial class Fo_MaintainceList
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
            this.ch_Language = new System.Windows.Forms.CheckBox();
            this.ch_ProcessParam = new System.Windows.Forms.CheckBox();
            this.ch_ScreenDisplay = new System.Windows.Forms.CheckBox();
            this.ch_Balance = new System.Windows.Forms.CheckBox();
            this.ch_Position = new System.Windows.Forms.CheckBox();
            this.ch_Runin = new System.Windows.Forms.CheckBox();
            this.ch_NCProg = new System.Windows.Forms.CheckBox();
            this.ch_Warmup = new System.Windows.Forms.CheckBox();
            this.ch_Door = new System.Windows.Forms.CheckBox();
            this.ch_Power = new System.Windows.Forms.CheckBox();
            this.ch_FuncSwitch = new System.Windows.Forms.CheckBox();
            this.ch_CNCDataManager = new System.Windows.Forms.CheckBox();
            this.ch_GWRPS = new System.Windows.Forms.CheckBox();
            this.ch_Rotation_Pos_Setting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ch_Language
            // 
            this.ch_Language.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Language.ForeColor = System.Drawing.Color.White;
            this.ch_Language.Location = new System.Drawing.Point(11, 10);
            this.ch_Language.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Language.Name = "ch_Language";
            this.ch_Language.Size = new System.Drawing.Size(149, 40);
            this.ch_Language.TabIndex = 0;
            this.ch_Language.Text = "語言";
            this.ch_Language.UseVisualStyleBackColor = false;
            this.ch_Language.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_ProcessParam
            // 
            this.ch_ProcessParam.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_ProcessParam.Location = new System.Drawing.Point(171, 10);
            this.ch_ProcessParam.Margin = new System.Windows.Forms.Padding(4);
            this.ch_ProcessParam.Name = "ch_ProcessParam";
            this.ch_ProcessParam.Size = new System.Drawing.Size(149, 40);
            this.ch_ProcessParam.TabIndex = 1;
            this.ch_ProcessParam.Text = "加工維護";
            this.ch_ProcessParam.UseVisualStyleBackColor = false;
            this.ch_ProcessParam.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_ScreenDisplay
            // 
            this.ch_ScreenDisplay.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_ScreenDisplay.Location = new System.Drawing.Point(331, 10);
            this.ch_ScreenDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.ch_ScreenDisplay.Name = "ch_ScreenDisplay";
            this.ch_ScreenDisplay.Size = new System.Drawing.Size(149, 40);
            this.ch_ScreenDisplay.TabIndex = 2;
            this.ch_ScreenDisplay.Text = "Screen Display";
            this.ch_ScreenDisplay.UseVisualStyleBackColor = false;
            this.ch_ScreenDisplay.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Balance
            // 
            this.ch_Balance.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Balance.Location = new System.Drawing.Point(491, 110);
            this.ch_Balance.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Balance.Name = "ch_Balance";
            this.ch_Balance.Size = new System.Drawing.Size(149, 40);
            this.ch_Balance.TabIndex = 3;
            this.ch_Balance.Text = "動平衡";
            this.ch_Balance.UseVisualStyleBackColor = false;
            this.ch_Balance.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Position
            // 
            this.ch_Position.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Position.Location = new System.Drawing.Point(11, 60);
            this.ch_Position.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Position.Name = "ch_Position";
            this.ch_Position.Size = new System.Drawing.Size(149, 40);
            this.ch_Position.TabIndex = 4;
            this.ch_Position.Text = "位置設定";
            this.ch_Position.UseVisualStyleBackColor = false;
            this.ch_Position.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Runin
            // 
            this.ch_Runin.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Runin.Location = new System.Drawing.Point(491, 60);
            this.ch_Runin.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Runin.Name = "ch_Runin";
            this.ch_Runin.Size = new System.Drawing.Size(149, 40);
            this.ch_Runin.TabIndex = 5;
            this.ch_Runin.Text = "高速主軸跑合";
            this.ch_Runin.UseVisualStyleBackColor = false;
            this.ch_Runin.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_NCProg
            // 
            this.ch_NCProg.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_NCProg.Location = new System.Drawing.Point(491, 10);
            this.ch_NCProg.Margin = new System.Windows.Forms.Padding(4);
            this.ch_NCProg.Name = "ch_NCProg";
            this.ch_NCProg.Size = new System.Drawing.Size(149, 40);
            this.ch_NCProg.TabIndex = 6;
            this.ch_NCProg.Text = "程式匯入";
            this.ch_NCProg.UseVisualStyleBackColor = false;
            this.ch_NCProg.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Warmup
            // 
            this.ch_Warmup.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Warmup.Location = new System.Drawing.Point(331, 60);
            this.ch_Warmup.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Warmup.Name = "ch_Warmup";
            this.ch_Warmup.Size = new System.Drawing.Size(149, 40);
            this.ch_Warmup.TabIndex = 7;
            this.ch_Warmup.Text = "暖機";
            this.ch_Warmup.UseVisualStyleBackColor = false;
            this.ch_Warmup.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Door
            // 
            this.ch_Door.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Door.Location = new System.Drawing.Point(11, 110);
            this.ch_Door.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Door.Name = "ch_Door";
            this.ch_Door.Size = new System.Drawing.Size(149, 40);
            this.ch_Door.TabIndex = 13;
            this.ch_Door.Text = "維修門";
            this.ch_Door.UseVisualStyleBackColor = false;
            this.ch_Door.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Power
            // 
            this.ch_Power.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Power.Location = new System.Drawing.Point(651, 110);
            this.ch_Power.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Power.Name = "ch_Power";
            this.ch_Power.Size = new System.Drawing.Size(149, 40);
            this.ch_Power.TabIndex = 14;
            this.ch_Power.Text = "關機";
            this.ch_Power.UseVisualStyleBackColor = false;
            this.ch_Power.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_FuncSwitch
            // 
            this.ch_FuncSwitch.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_FuncSwitch.Location = new System.Drawing.Point(651, 10);
            this.ch_FuncSwitch.Margin = new System.Windows.Forms.Padding(4);
            this.ch_FuncSwitch.Name = "ch_FuncSwitch";
            this.ch_FuncSwitch.Size = new System.Drawing.Size(149, 40);
            this.ch_FuncSwitch.TabIndex = 15;
            this.ch_FuncSwitch.Text = "功能開關";
            this.ch_FuncSwitch.UseVisualStyleBackColor = false;
            this.ch_FuncSwitch.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_CNCDataManager
            // 
            this.ch_CNCDataManager.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_CNCDataManager.Location = new System.Drawing.Point(171, 60);
            this.ch_CNCDataManager.Margin = new System.Windows.Forms.Padding(4);
            this.ch_CNCDataManager.Name = "ch_CNCDataManager";
            this.ch_CNCDataManager.Size = new System.Drawing.Size(149, 40);
            this.ch_CNCDataManager.TabIndex = 16;
            this.ch_CNCDataManager.Text = "CNC資料管理";
            this.ch_CNCDataManager.UseVisualStyleBackColor = false;
            this.ch_CNCDataManager.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_GWRPS
            // 
            this.ch_GWRPS.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_GWRPS.Location = new System.Drawing.Point(171, 110);
            this.ch_GWRPS.Margin = new System.Windows.Forms.Padding(4);
            this.ch_GWRPS.Name = "ch_GWRPS";
            this.ch_GWRPS.Size = new System.Drawing.Size(149, 40);
            this.ch_GWRPS.TabIndex = 17;
            this.ch_GWRPS.Text = "砂輪基準點設定";
            this.ch_GWRPS.UseVisualStyleBackColor = false;
            this.ch_GWRPS.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // ch_Rotation_Pos_Setting
            // 
            this.ch_Rotation_Pos_Setting.BackColor = System.Drawing.Color.DarkBlue;
            this.ch_Rotation_Pos_Setting.Location = new System.Drawing.Point(331, 110);
            this.ch_Rotation_Pos_Setting.Margin = new System.Windows.Forms.Padding(4);
            this.ch_Rotation_Pos_Setting.Name = "ch_Rotation_Pos_Setting";
            this.ch_Rotation_Pos_Setting.Size = new System.Drawing.Size(149, 40);
            this.ch_Rotation_Pos_Setting.TabIndex = 18;
            this.ch_Rotation_Pos_Setting.Text = "旋轉中心位置補正";
            this.ch_Rotation_Pos_Setting.UseVisualStyleBackColor = false;
            this.ch_Rotation_Pos_Setting.CheckedChanged += new System.EventHandler(this.OnChecked);
            // 
            // Fo_MaintainceList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 165);
            this.Controls.Add(this.ch_Rotation_Pos_Setting);
            this.Controls.Add(this.ch_GWRPS);
            this.Controls.Add(this.ch_CNCDataManager);
            this.Controls.Add(this.ch_FuncSwitch);
            this.Controls.Add(this.ch_Power);
            this.Controls.Add(this.ch_Door);
            this.Controls.Add(this.ch_Warmup);
            this.Controls.Add(this.ch_NCProg);
            this.Controls.Add(this.ch_Runin);
            this.Controls.Add(this.ch_Position);
            this.Controls.Add(this.ch_Balance);
            this.Controls.Add(this.ch_ScreenDisplay);
            this.Controls.Add(this.ch_ProcessParam);
            this.Controls.Add(this.ch_Language);
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Fo_MaintainceList";
            this.Text = "維護頁面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_MaintainceList_FormClosing);
            this.Load += new System.EventHandler(this.Fo_MaintainceList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ch_Language;
        private System.Windows.Forms.CheckBox ch_ProcessParam;
        private System.Windows.Forms.CheckBox ch_ScreenDisplay;
        private System.Windows.Forms.CheckBox ch_Balance;
        private System.Windows.Forms.CheckBox ch_Position;
        private System.Windows.Forms.CheckBox ch_Runin;
        private System.Windows.Forms.CheckBox ch_NCProg;
        private System.Windows.Forms.CheckBox ch_Warmup;
        private System.Windows.Forms.CheckBox ch_Door;
        private System.Windows.Forms.CheckBox ch_Power;
        private System.Windows.Forms.CheckBox ch_FuncSwitch;
        private System.Windows.Forms.CheckBox ch_CNCDataManager;
        private System.Windows.Forms.CheckBox ch_GWRPS;
        private System.Windows.Forms.CheckBox ch_Rotation_Pos_Setting;
    }
}