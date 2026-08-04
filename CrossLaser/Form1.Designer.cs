namespace CrossLaser
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnToggle = new System.Windows.Forms.Button();
            lblStatus = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            chkOnlyTCP = new System.Windows.Forms.CheckBox();
            pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            this.SuspendLayout();
            // 
            // btnToggle
            // 
            btnToggle.BackColor = System.Drawing.Color.WhiteSmoke;
            btnToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnToggle.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F);
            btnToggle.ForeColor = System.Drawing.Color.DarkRed;
            btnToggle.Location = new System.Drawing.Point(370, 11);
            btnToggle.Margin = new System.Windows.Forms.Padding(2);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new System.Drawing.Size(127, 35);
            btnToggle.TabIndex = 0;
            btnToggle.Text = "阻止联网";
            btnToggle.UseVisualStyleBackColor = false;
            btnToggle.Click += this.btnToggle_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F);
            lblStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            lblStatus.Location = new System.Drawing.Point(79, 48);
            lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(216, 20);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "正在初始化程序，请稍作等待...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            label1.Location = new System.Drawing.Point(7, 301);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(496, 20);
            label1.TabIndex = 2;
            label1.Text = "本程序为 MageAngela 制作的小玩具，不保证能用，如有好建议请私信联系。";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(13, 89);
            pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(485, 200);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            label2.ForeColor = System.Drawing.Color.Maroon;
            label2.Location = new System.Drawing.Point(11, 338);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(54, 20);
            label2.TabIndex = 4;
            label2.Text = "Ver 1.2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            label3.ForeColor = System.Drawing.Color.Maroon;
            label3.Location = new System.Drawing.Point(385, 338);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(115, 20);
            label3.TabIndex = 5;
            label3.Text = "MageAngela.cn";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Unispace", 20F, System.Drawing.FontStyle.Bold);
            label4.ForeColor = System.Drawing.Color.DarkRed;
            label4.Location = new System.Drawing.Point(75, 11);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(175, 33);
            label4.TabIndex = 6;
            label4.Text = "CrossLaser";
            // 
            // chkOnlyTCP
            // 
            chkOnlyTCP.AutoSize = true;
            chkOnlyTCP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            chkOnlyTCP.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            chkOnlyTCP.Location = new System.Drawing.Point(266, 17);
            chkOnlyTCP.Name = "chkOnlyTCP";
            chkOnlyTCP.Size = new System.Drawing.Size(93, 24);
            chkOnlyTCP.TabIndex = 7;
            chkOnlyTCP.Text = "仅阻止TCP";
            chkOnlyTCP.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (System.Drawing.Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new System.Drawing.Point(11, 9);
            pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(64, 64);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 373);
            this.Controls.Add(chkOnlyTCP);
            this.Controls.Add(label4);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(pictureBox1);
            this.Controls.Add(label1);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnToggle);
            this.Controls.Add(pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CrossLaser";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button btnToggle;
        private Label lblStatus;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private CheckBox chkOnlyTCP;
        private PictureBox pictureBox2;
    }
}
