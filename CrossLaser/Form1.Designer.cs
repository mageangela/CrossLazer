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
            label4 = new Label();
            chkOnlyTCP = new CheckBox();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // btnToggle
            // 
            btnToggle.BackColor = Color.WhiteSmoke;
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.Font = new Font("Microsoft YaHei UI", 11F);
            btnToggle.ForeColor = Color.DarkRed;
            btnToggle.Location = new Point(370, 11);
            btnToggle.Margin = new Padding(2);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(127, 35);
            btnToggle.TabIndex = 0;
            btnToggle.Text = "阻止联网";
            btnToggle.UseVisualStyleBackColor = false;
            btnToggle.Click += btnToggle_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft YaHei UI", 11F);
            lblStatus.ForeColor = SystemColors.ControlDarkDark;
            lblStatus.Location = new Point(79, 48);
            lblStatus.Margin = new Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(216, 20);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "正在初始化程序，请稍作等待...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 10F);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(7, 301);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(496, 20);
            label1.TabIndex = 2;
            label1.Text = "本程序为 MageAngela 制作的小玩具，不保证能用，如有好建议请私信联系。";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(13, 89);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(485, 200);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 10F);
            label2.ForeColor = Color.Maroon;
            label2.Location = new Point(11, 338);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(54, 20);
            label2.TabIndex = 4;
            label2.Text = "Ver 1.2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft YaHei UI", 10F);
            label3.ForeColor = Color.Maroon;
            label3.Location = new Point(385, 338);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(115, 20);
            label3.TabIndex = 5;
            label3.Text = "MageAngela.cn";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Unispace", 20F, FontStyle.Bold);
            label4.ForeColor = Color.DarkRed;
            label4.Location = new Point(75, 11);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(175, 33);
            label4.TabIndex = 6;
            label4.Text = "CrossLaser";
            // 
            // chkOnlyTCP
            // 
            chkOnlyTCP.AutoSize = true;
            chkOnlyTCP.FlatStyle = FlatStyle.Flat;
            chkOnlyTCP.Font = new Font("Microsoft YaHei UI", 10F);
            chkOnlyTCP.Location = new Point(266, 17);
            chkOnlyTCP.Name = "chkOnlyTCP";
            chkOnlyTCP.Size = new Size(93, 24);
            chkOnlyTCP.TabIndex = 7;
            chkOnlyTCP.Text = "仅阻止TCP";
            chkOnlyTCP.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(11, 9);
            pictureBox2.Margin = new Padding(2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(64, 64);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 373);
            Controls.Add(chkOnlyTCP);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(lblStatus);
            Controls.Add(btnToggle);
            Controls.Add(pictureBox2);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "CrossLaser";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
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
        private Label label4;
        private CheckBox chkOnlyTCP;
        private PictureBox pictureBox2;
    }
}
