namespace OCD
{
    partial class Fo_ChangePartsPos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fo_ChangePartsPos));
            this.la_PosZ = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_ZPos = new System.Windows.Forms.Button();
            this.btn_XPos = new System.Windows.Forms.Button();
            this.TB_PosZ = new System.Windows.Forms.TextBox();
            this.TB_PosX = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.la_PosX = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // la_PosZ
            // 
            this.la_PosZ.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.la_PosZ, "la_PosZ");
            this.la_PosZ.Name = "la_PosZ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ZPos);
            this.groupBox1.Controls.Add(this.btn_XPos);
            this.groupBox1.Controls.Add(this.TB_PosZ);
            this.groupBox1.Controls.Add(this.TB_PosX);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label26);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btn_ZPos
            // 
            resources.ApplyResources(this.btn_ZPos, "btn_ZPos");
            this.btn_ZPos.Name = "btn_ZPos";
            this.btn_ZPos.UseVisualStyleBackColor = true;
            this.btn_ZPos.Click += new System.EventHandler(this.btn_ZPos_Click);
            // 
            // btn_XPos
            // 
            resources.ApplyResources(this.btn_XPos, "btn_XPos");
            this.btn_XPos.Name = "btn_XPos";
            this.btn_XPos.UseVisualStyleBackColor = true;
            this.btn_XPos.Click += new System.EventHandler(this.btn_XPos_Click);
            // 
            // TB_PosZ
            // 
            resources.ApplyResources(this.TB_PosZ, "TB_PosZ");
            this.TB_PosZ.Name = "TB_PosZ";
            this.TB_PosZ.Click += new System.EventHandler(this.TextBoxClick);
            // 
            // TB_PosX
            // 
            resources.ApplyResources(this.TB_PosX, "TB_PosX");
            this.TB_PosX.Name = "TB_PosX";
            this.TB_PosX.Click += new System.EventHandler(this.TextBoxClick);
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.la_PosZ);
            this.groupBox2.Controls.Add(this.la_PosX);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // la_PosX
            // 
            this.la_PosX.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.la_PosX, "la_PosX");
            this.la_PosX.Name = "la_PosX";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.TabStop = false;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // Fo_ChangePartsPos
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_Save);
            this.Name = "Fo_ChangePartsPos";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fo_ChangePartsPos_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label la_PosZ;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox TB_PosZ;
        public System.Windows.Forms.TextBox TB_PosX;
        public System.Windows.Forms.Label label25;
        public System.Windows.Forms.Label label26;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label la_PosX;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btn_Save;
        public System.Windows.Forms.Button btn_ZPos;
        public System.Windows.Forms.Button btn_XPos;
    }
}