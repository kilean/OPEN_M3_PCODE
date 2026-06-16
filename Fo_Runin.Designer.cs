namespace OIG
{
    partial class Fo_Runin
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fo_Runin));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.gb_Runin_List = new System.Windows.Forms.GroupBox();
            this.btn_Runin_Delete = new Uc_RoundBtn();
            this.lb_Runin_List = new System.Windows.Forms.ListBox();
            this.btn_Runin_DeleteAll = new Uc_RoundBtn();
            this.gb_Runin_Process = new System.Windows.Forms.GroupBox();
            this.btn_Runin_Add = new Uc_RoundBtn();
            this.tb_Runin_Minute = new System.Windows.Forms.TextBox();
            this.la_Runin_Time = new System.Windows.Forms.Label();
            this.tb_Runin_RPM = new System.Windows.Forms.TextBox();
            this.la_Runin_RPM = new System.Windows.Forms.Label();
            this.btn_Runin_Start = new Uc_RoundBtn();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.gb_Runin_List.SuspendLayout();
            this.gb_Runin_Process.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.BorderSkin.BackColor = System.Drawing.Color.Lime;
            this.chart1.BorderSkin.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.BackwardDiagonal;
            this.chart1.BorderSkin.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart1.BorderSkin.PageColor = System.Drawing.Color.Transparent;
            this.chart1.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.FrameThin6;
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.LabelStyle.Format = "N0";
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "Time(min)";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            chartArea1.AxisY.Title = "Speed(RPM)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(8, 8);
            this.chart1.Name = "chart1";
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series1.LegendText = "CNC1";
            series1.Name = "Series_Proc";
            series2.BorderWidth = 3;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Name = "Series_Real";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(680, 512);
            this.chart1.TabIndex = 21;
            this.chart1.Text = "chart1";
            // 
            // gb_Runin_List
            // 
            this.gb_Runin_List.BackColor = System.Drawing.Color.Transparent;
            this.gb_Runin_List.Controls.Add(this.btn_Runin_Delete);
            this.gb_Runin_List.Controls.Add(this.lb_Runin_List);
            this.gb_Runin_List.Controls.Add(this.btn_Runin_DeleteAll);
            this.gb_Runin_List.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.gb_Runin_List.Location = new System.Drawing.Point(688, 288);
            this.gb_Runin_List.Name = "gb_Runin_List";
            this.gb_Runin_List.Size = new System.Drawing.Size(248, 304);
            this.gb_Runin_List.TabIndex = 23;
            this.gb_Runin_List.TabStop = false;
            this.gb_Runin_List.Text = "程序列表";
            // 
            // btn_Runin_Delete
            // 
            this.btn_Runin_Delete.BackColor = System.Drawing.Color.Transparent;
            this.btn_Runin_Delete.DisplayText = "刪除\r\n一筆";
            this.btn_Runin_Delete.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Runin_Delete.ForeColor = System.Drawing.Color.White;
            this.btn_Runin_Delete.Image = null;
            this.btn_Runin_Delete.Lamp = false;
            this.btn_Runin_Delete.LampOnImage = global::OIG.Properties.Resources.Btn_S3_60x60_GrayL3;
            this.btn_Runin_Delete.Location = new System.Drawing.Point(88, 224);
            this.btn_Runin_Delete.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Runin_Delete.MouseDownImage")));
            this.btn_Runin_Delete.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Runin_Delete.MouseUpImage")));
            this.btn_Runin_Delete.Name = "btn_Runin_Delete";
            this.btn_Runin_Delete.Size = new System.Drawing.Size(72, 72);
            this.btn_Runin_Delete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Runin_Delete.Switch = false;
            this.btn_Runin_Delete.TabIndex = 95;
            this.btn_Runin_Delete.Click += new System.EventHandler(this.uc_RoundBtn1_Click);
            // 
            // lb_Runin_List
            // 
            this.lb_Runin_List.FormattingEnabled = true;
            this.lb_Runin_List.ItemHeight = 20;
            this.lb_Runin_List.Location = new System.Drawing.Point(8, 32);
            this.lb_Runin_List.Name = "lb_Runin_List";
            this.lb_Runin_List.Size = new System.Drawing.Size(232, 184);
            this.lb_Runin_List.TabIndex = 0;
            // 
            // btn_Runin_DeleteAll
            // 
            this.btn_Runin_DeleteAll.BackColor = System.Drawing.Color.Transparent;
            this.btn_Runin_DeleteAll.DisplayText = "刪除\r\n全部";
            this.btn_Runin_DeleteAll.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Runin_DeleteAll.ForeColor = System.Drawing.Color.White;
            this.btn_Runin_DeleteAll.Image = null;
            this.btn_Runin_DeleteAll.Lamp = false;
            this.btn_Runin_DeleteAll.LampOnImage = global::OIG.Properties.Resources.Btn_S3_60x60_GrayL3;
            this.btn_Runin_DeleteAll.Location = new System.Drawing.Point(168, 224);
            this.btn_Runin_DeleteAll.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Runin_DeleteAll.MouseDownImage")));
            this.btn_Runin_DeleteAll.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Runin_DeleteAll.MouseUpImage")));
            this.btn_Runin_DeleteAll.Name = "btn_Runin_DeleteAll";
            this.btn_Runin_DeleteAll.Size = new System.Drawing.Size(72, 72);
            this.btn_Runin_DeleteAll.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Runin_DeleteAll.Switch = false;
            this.btn_Runin_DeleteAll.TabIndex = 94;
            this.btn_Runin_DeleteAll.Click += new System.EventHandler(this.btn_CleaarAll_Click);
            // 
            // gb_Runin_Process
            // 
            this.gb_Runin_Process.BackColor = System.Drawing.Color.Transparent;
            this.gb_Runin_Process.Controls.Add(this.btn_Runin_Add);
            this.gb_Runin_Process.Controls.Add(this.tb_Runin_Minute);
            this.gb_Runin_Process.Controls.Add(this.la_Runin_Time);
            this.gb_Runin_Process.Controls.Add(this.tb_Runin_RPM);
            this.gb_Runin_Process.Controls.Add(this.la_Runin_RPM);
            this.gb_Runin_Process.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.gb_Runin_Process.Location = new System.Drawing.Point(688, 16);
            this.gb_Runin_Process.Name = "gb_Runin_Process";
            this.gb_Runin_Process.Size = new System.Drawing.Size(248, 248);
            this.gb_Runin_Process.TabIndex = 22;
            this.gb_Runin_Process.TabStop = false;
            this.gb_Runin_Process.Text = "程序";
            // 
            // btn_Runin_Add
            // 
            this.btn_Runin_Add.BackColor = System.Drawing.Color.Transparent;
            this.btn_Runin_Add.DisplayText = "增加";
            this.btn_Runin_Add.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Runin_Add.ForeColor = System.Drawing.Color.White;
            this.btn_Runin_Add.Image = null;
            this.btn_Runin_Add.Lamp = false;
            this.btn_Runin_Add.LampOnImage = global::OIG.Properties.Resources.Btn_S3_60x60_GrayL3;
            this.btn_Runin_Add.Location = new System.Drawing.Point(168, 56);
            this.btn_Runin_Add.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Runin_Add.MouseDownImage")));
            this.btn_Runin_Add.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Runin_Add.MouseUpImage")));
            this.btn_Runin_Add.Name = "btn_Runin_Add";
            this.btn_Runin_Add.Size = new System.Drawing.Size(72, 72);
            this.btn_Runin_Add.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Runin_Add.Switch = false;
            this.btn_Runin_Add.TabIndex = 96;
            this.btn_Runin_Add.Click += new System.EventHandler(this.uc_RoundBtn1_Click_1);
            // 
            // tb_Runin_Minute
            // 
            this.tb_Runin_Minute.Location = new System.Drawing.Point(16, 192);
            this.tb_Runin_Minute.Name = "tb_Runin_Minute";
            this.tb_Runin_Minute.Size = new System.Drawing.Size(48, 29);
            this.tb_Runin_Minute.TabIndex = 2;
            this.tb_Runin_Minute.Text = "1";
            this.tb_Runin_Minute.Click += new System.EventHandler(this.TextBoxClick);
            // 
            // la_Runin_Time
            // 
            this.la_Runin_Time.AutoSize = true;
            this.la_Runin_Time.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_Runin_Time.Location = new System.Drawing.Point(16, 160);
            this.la_Runin_Time.Name = "la_Runin_Time";
            this.la_Runin_Time.Size = new System.Drawing.Size(122, 20);
            this.la_Runin_Time.TabIndex = 2;
            this.la_Runin_Time.Text = "時間(1~60分鐘)";
            // 
            // tb_Runin_RPM
            // 
            this.tb_Runin_RPM.Location = new System.Drawing.Point(16, 64);
            this.tb_Runin_RPM.Name = "tb_Runin_RPM";
            this.tb_Runin_RPM.Size = new System.Drawing.Size(104, 29);
            this.tb_Runin_RPM.TabIndex = 1;
            this.tb_Runin_RPM.Text = "0";
            this.tb_Runin_RPM.Click += new System.EventHandler(this.TextBoxClick);
            // 
            // la_Runin_RPM
            // 
            this.la_Runin_RPM.AutoSize = true;
            this.la_Runin_RPM.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.la_Runin_RPM.Location = new System.Drawing.Point(16, 32);
            this.la_Runin_RPM.Name = "la_Runin_RPM";
            this.la_Runin_RPM.Size = new System.Drawing.Size(200, 20);
            this.la_Runin_RPM.TabIndex = 0;
            this.la_Runin_RPM.Text = "轉速(0, 5000~30000 RPM)";
            // 
            // btn_Runin_Start
            // 
            this.btn_Runin_Start.BackColor = System.Drawing.Color.Transparent;
            this.btn_Runin_Start.DisplayText = "啟動";
            this.btn_Runin_Start.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Runin_Start.ForeColor = System.Drawing.Color.White;
            this.btn_Runin_Start.Image = null;
            this.btn_Runin_Start.Lamp = false;
            this.btn_Runin_Start.LampOnImage = global::OIG.Properties.Resources.Btn_S3_128x60_GrayL3;
            this.btn_Runin_Start.Location = new System.Drawing.Point(8, 520);
            this.btn_Runin_Start.MouseDownImage = global::OIG.Properties.Resources.Btn_S3_128x60_L;
            this.btn_Runin_Start.MouseUpImage = global::OIG.Properties.Resources.Btn_S3_128x60;
            this.btn_Runin_Start.Name = "btn_Runin_Start";
            this.btn_Runin_Start.Size = new System.Drawing.Size(152, 72);
            this.btn_Runin_Start.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Runin_Start.Switch = false;
            this.btn_Runin_Start.TabIndex = 93;
            this.btn_Runin_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // Fo_Runin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(944, 600);
            this.Controls.Add(this.btn_Runin_Start);
            this.Controls.Add(this.gb_Runin_List);
            this.Controls.Add(this.gb_Runin_Process);
            this.Controls.Add(this.chart1);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Fo_Runin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Fo_Runin";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.gb_Runin_List.ResumeLayout(false);
            this.gb_Runin_Process.ResumeLayout(false);
            this.gb_Runin_Process.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.GroupBox gb_Runin_List;
        private System.Windows.Forms.ListBox lb_Runin_List;
        private System.Windows.Forms.GroupBox gb_Runin_Process;
        private System.Windows.Forms.TextBox tb_Runin_Minute;
        private System.Windows.Forms.Label la_Runin_Time;
        private System.Windows.Forms.TextBox tb_Runin_RPM;
        private System.Windows.Forms.Label la_Runin_RPM;
        private Uc_RoundBtn btn_Runin_Start;
        private Uc_RoundBtn btn_Runin_DeleteAll;
        private Uc_RoundBtn btn_Runin_Delete;
        private Uc_RoundBtn btn_Runin_Add;
    }
}