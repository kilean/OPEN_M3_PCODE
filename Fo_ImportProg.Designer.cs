namespace OCD
{
    partial class Fo_ImportProg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fo_ImportProg));
            this.la_CNC_Path = new System.Windows.Forms.Label();
            this.la_CNCPath = new System.Windows.Forms.Label();
            this.btn_Import = new Uc_RoundBtn();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_Refresh = new Uc_RoundBtn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.la_Wait = new System.Windows.Forms.Label();
            this.la_PCPath = new System.Windows.Forms.Label();
            this.la_PC_Path = new System.Windows.Forms.Label();
            this.la_CNC = new System.Windows.Forms.Label();
            this.la_PC = new System.Windows.Forms.Label();
            this.btn_PCPath = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_Export = new Uc_RoundBtn();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Col_FileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Col_Memo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.usbDetector1 = new USBDetector(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // la_CNC_Path
            // 
            this.la_CNC_Path.Location = new System.Drawing.Point(8, 8);
            this.la_CNC_Path.Name = "la_CNC_Path";
            this.la_CNC_Path.Size = new System.Drawing.Size(120, 20);
            this.la_CNC_Path.TabIndex = 1;
            this.la_CNC_Path.Text = "CNC Path";
            this.la_CNC_Path.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // la_CNCPath
            // 
            this.la_CNCPath.BackColor = System.Drawing.Color.LightGray;
            this.la_CNCPath.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.la_CNCPath.Location = new System.Drawing.Point(136, 8);
            this.la_CNCPath.Name = "la_CNCPath";
            this.la_CNCPath.Size = new System.Drawing.Size(680, 24);
            this.la_CNCPath.TabIndex = 2;
            this.la_CNCPath.Text = "//CNC_MEM/USER/PATH1/";
            // 
            // btn_Import
            // 
            this.btn_Import.BackColor = System.Drawing.Color.Transparent;
            this.btn_Import.DisplayText = "匯入\r\n<<";
            this.btn_Import.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Import.ForeColor = System.Drawing.Color.White;
            this.btn_Import.Image = null;
            this.btn_Import.Lamp = false;
            this.btn_Import.LampOnImage = null;
            this.btn_Import.Location = new System.Drawing.Point(432, 96);
            this.btn_Import.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Import.MouseDownImage")));
            this.btn_Import.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Import.MouseUpImage")));
            this.btn_Import.Name = "btn_Import";
            this.btn_Import.Size = new System.Drawing.Size(72, 72);
            this.btn_Import.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Import.Switch = false;
            this.btn_Import.TabIndex = 56;
            this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.BackColor = System.Drawing.Color.Transparent;
            this.btn_Refresh.DisplayText = "刷新";
            this.btn_Refresh.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Refresh.ForeColor = System.Drawing.Color.White;
            this.btn_Refresh.Image = null;
            this.btn_Refresh.Lamp = false;
            this.btn_Refresh.LampOnImage = null;
            this.btn_Refresh.Location = new System.Drawing.Point(432, 352);
            this.btn_Refresh.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Refresh.MouseDownImage")));
            this.btn_Refresh.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Refresh.MouseUpImage")));
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(72, 72);
            this.btn_Refresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Refresh.Switch = false;
            this.btn_Refresh.TabIndex = 57;
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Yellow;
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.la_Wait);
            this.panel1.Location = new System.Drawing.Point(48, 264);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(848, 72);
            this.panel1.TabIndex = 60;
            this.panel1.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(8, 40);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(832, 24);
            this.progressBar1.TabIndex = 1;
            // 
            // la_Wait
            // 
            this.la_Wait.Location = new System.Drawing.Point(8, 8);
            this.la_Wait.Name = "la_Wait";
            this.la_Wait.Size = new System.Drawing.Size(832, 24);
            this.la_Wait.TabIndex = 0;
            this.la_Wait.Text = "程式寫入中...";
            this.la_Wait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // la_PCPath
            // 
            this.la_PCPath.BackColor = System.Drawing.Color.LightGray;
            this.la_PCPath.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.la_PCPath.Location = new System.Drawing.Point(136, 40);
            this.la_PCPath.Name = "la_PCPath";
            this.la_PCPath.Size = new System.Drawing.Size(680, 24);
            this.la_PCPath.TabIndex = 65;
            // 
            // la_PC_Path
            // 
            this.la_PC_Path.Location = new System.Drawing.Point(8, 40);
            this.la_PC_Path.Name = "la_PC_Path";
            this.la_PC_Path.Size = new System.Drawing.Size(120, 20);
            this.la_PC_Path.TabIndex = 64;
            this.la_PC_Path.Text = "PC Path";
            this.la_PC_Path.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // la_CNC
            // 
            this.la_CNC.Location = new System.Drawing.Point(8, 72);
            this.la_CNC.Name = "la_CNC";
            this.la_CNC.Size = new System.Drawing.Size(120, 20);
            this.la_CNC.TabIndex = 67;
            this.la_CNC.Text = "CNC List";
            // 
            // la_PC
            // 
            this.la_PC.Location = new System.Drawing.Point(544, 72);
            this.la_PC.Name = "la_PC";
            this.la_PC.Size = new System.Drawing.Size(120, 20);
            this.la_PC.TabIndex = 68;
            this.la_PC.Text = "PC List";
            // 
            // btn_PCPath
            // 
            this.btn_PCPath.Location = new System.Drawing.Point(824, 40);
            this.btn_PCPath.Name = "btn_PCPath";
            this.btn_PCPath.Size = new System.Drawing.Size(48, 32);
            this.btn_PCPath.TabIndex = 69;
            this.btn_PCPath.Text = "...";
            this.btn_PCPath.UseVisualStyleBackColor = true;
            this.btn_PCPath.Click += new System.EventHandler(this.btn_PCPath_Click);
            // 
            // btn_Export
            // 
            this.btn_Export.BackColor = System.Drawing.Color.Transparent;
            this.btn_Export.DisplayText = "匯出\r\n>>";
            this.btn_Export.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Export.ForeColor = System.Drawing.Color.White;
            this.btn_Export.Image = null;
            this.btn_Export.Lamp = false;
            this.btn_Export.LampOnImage = null;
            this.btn_Export.Location = new System.Drawing.Point(432, 176);
            this.btn_Export.MouseDownImage = ((System.Drawing.Image)(resources.GetObject("btn_Export.MouseDownImage")));
            this.btn_Export.MouseUpImage = ((System.Drawing.Image)(resources.GetObject("btn_Export.MouseUpImage")));
            this.btn_Export.Name = "btn_Export";
            this.btn_Export.Size = new System.Drawing.Size(72, 72);
            this.btn_Export.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.btn_Export.Switch = false;
            this.btn_Export.TabIndex = 70;
            this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Col_FileName,
            this.Col_Memo});
            this.listView1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.listView1.HideSelection = false;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(8, 96);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(416, 488);
            this.listView1.TabIndex = 71;
            this.listView1.TileSize = new System.Drawing.Size(400, 62);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            // 
            // Col_FileName
            // 
            this.Col_FileName.Text = "檔案名稱";
            // 
            // Col_Memo
            // 
            this.Col_Memo.Text = "備註";
            this.Col_Memo.Width = 300;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "File1_47x58.png");
            this.imageList1.Images.SetKeyName(1, "File2_47x58.png");
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.listView2.HideSelection = false;
            this.listView2.LargeImageList = this.imageList1;
            this.listView2.Location = new System.Drawing.Point(512, 96);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(416, 488);
            this.listView2.TabIndex = 72;
            this.listView2.TileSize = new System.Drawing.Size(400, 62);
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Tile;
            this.listView2.Click += new System.EventHandler(this.listView2_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "檔案名稱";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "備註";
            this.columnHeader2.Width = 300;
            // 
            // usbDetector1
            // 
            this.usbDetector1.Owner = this;
            this.usbDetector1.OnInserted += new System.Management.EventArrivedEventHandler(this.usbDetector1_OnInserted);
            this.usbDetector1.OnRemoved += new System.Management.EventArrivedEventHandler(this.usbDetector1_OnRemoved);
            // 
            // Fo_ImportProg
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(944, 600);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btn_Export);
            this.Controls.Add(this.btn_PCPath);
            this.Controls.Add(this.la_PC);
            this.Controls.Add(this.la_CNC);
            this.Controls.Add(this.la_PCPath);
            this.Controls.Add(this.la_PC_Path);
            this.Controls.Add(this.btn_Refresh);
            this.Controls.Add(this.btn_Import);
            this.Controls.Add(this.la_CNCPath);
            this.Controls.Add(this.la_CNC_Path);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Fo_ImportProg";
            this.Text = "Fo_ImportProg";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_ImportProg_FormClosing);
            this.Load += new System.EventHandler(this.Fo_ImportProg_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label la_CNC_Path;
        private System.Windows.Forms.Label la_CNCPath;
        private Uc_RoundBtn btn_Import;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private Uc_RoundBtn btn_Refresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label la_Wait;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label la_PCPath;
        private System.Windows.Forms.Label la_PC_Path;
        private System.Windows.Forms.Label la_CNC;
        private System.Windows.Forms.Label la_PC;
        private System.Windows.Forms.Button btn_PCPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private Uc_RoundBtn btn_Export;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader Col_FileName;
        private System.Windows.Forms.ColumnHeader Col_Memo;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private USBDetector usbDetector1;
    }
}