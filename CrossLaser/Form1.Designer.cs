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
            btnToggle = new Button();
            lblStatus = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnToggle
            // 
            btnToggle.Font = new Font("Microsoft YaHei UI", 11F);
            btnToggle.Location = new Point(26, 23);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(200, 50);
            btnToggle.TabIndex = 0;
            btnToggle.Text = "阻止联网";
            btnToggle.UseVisualStyleBackColor = true;
            btnToggle.Click += btnToggle_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft YaHei UI", 11F);
            lblStatus.Location = new Point(232, 32);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(270, 30);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "请您查看图示教程后操作...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 10F);
            label1.Location = new Point(26, 389);
            label1.Name = "label1";
            label1.Size = new Size(745, 27);
            label1.TabIndex = 2;
            label1.Text = "本程序为 MageAngela 制作的小玩具，仅用于娱乐作用，如有好建议请私信联系。";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 88);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(762, 282);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 10F);
            label2.ForeColor = Color.Maroon;
            label2.Location = new Point(26, 432);
            label2.Name = "label2";
            label2.Size = new Size(80, 27);
            label2.TabIndex = 4;
            label2.Text = "Ver 1.0\r\n";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft YaHei UI", 10F);
            label3.ForeColor = Color.Maroon;
            label3.Location = new Point(614, 432);
            label3.Name = "label3";
            label3.Size = new Size(160, 27);
            label3.TabIndex = 5;
            label3.Text = "MageAngela.cn";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 474);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(lblStatus);
            Controls.Add(btnToggle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "CrossLaser";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnToggle;
        private Label lblStatus;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
    }
}
