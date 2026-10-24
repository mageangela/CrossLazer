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
            label底部提示 = new Label();
            pictureBox1 = new PictureBox();
            label版本号 = new Label();
            label网址 = new Label();
            label标题 = new Label();
            checkBoxTCP = new CheckBox();
            pictureBox2 = new PictureBox();
            label1 = new Label();
            label阻止联机 = new Label();
            label云服务器 = new Label();
            checkBox存档 = new CheckBox();
            checkBoxUDP = new CheckBox();
            comboBox热键 = new ComboBox();
            label加速提示 = new Label();
            label2 = new Label();
            label3 = new Label();
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
            btnToggle.Location = new Point(358, 11);
            btnToggle.Margin = new Padding(2);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(127, 33);
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
            lblStatus.Location = new Point(87, 48);
            lblStatus.Margin = new Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(216, 20);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "正在初始化程序，请稍作等待...";
            // 
            // label底部提示
            // 
            label底部提示.AutoSize = true;
            label底部提示.Font = new Font("Microsoft YaHei UI", 10F);
            label底部提示.ForeColor = SystemColors.ControlDarkDark;
            label底部提示.Location = new Point(7, 397);
            label底部提示.Margin = new Padding(2, 0, 2, 0);
            label底部提示.Name = "label底部提示";
            label底部提示.Size = new Size(496, 20);
            label底部提示.TabIndex = 2;
            label底部提示.Text = "本程序为 MageAngela 制作的小玩具，不保证能用，如有好建议请私信联系。";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(13, 185);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(485, 200);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label版本号
            // 
            label版本号.AutoSize = true;
            label版本号.Font = new Font("Microsoft YaHei UI", 10F);
            label版本号.ForeColor = Color.Maroon;
            label版本号.Location = new Point(11, 434);
            label版本号.Margin = new Padding(2, 0, 2, 0);
            label版本号.Name = "label版本号";
            label版本号.Size = new Size(54, 20);
            label版本号.TabIndex = 4;
            label版本号.Text = "Ver 1.3";
            // 
            // label网址
            // 
            label网址.AutoSize = true;
            label网址.Font = new Font("Microsoft YaHei UI", 10F);
            label网址.ForeColor = Color.Maroon;
            label网址.Location = new Point(385, 434);
            label网址.Margin = new Padding(2, 0, 2, 0);
            label网址.Name = "label网址";
            label网址.Size = new Size(115, 20);
            label网址.TabIndex = 5;
            label网址.Text = "MageAngela.cn";
            // 
            // label标题
            // 
            label标题.AutoSize = true;
            label标题.Font = new Font("Unispace", 20F, FontStyle.Bold);
            label标题.ForeColor = Color.DarkRed;
            label标题.Location = new Point(83, 11);
            label标题.Margin = new Padding(2, 0, 2, 0);
            label标题.Name = "label标题";
            label标题.Size = new Size(175, 33);
            label标题.TabIndex = 6;
            label标题.Text = "CrossLaser";
            // 
            // checkBoxTCP
            // 
            checkBoxTCP.AutoSize = true;
            checkBoxTCP.FlatStyle = FlatStyle.Flat;
            checkBoxTCP.Font = new Font("Microsoft YaHei UI", 10F);
            checkBoxTCP.Location = new Point(122, 130);
            checkBoxTCP.Name = "checkBoxTCP";
            checkBoxTCP.Size = new Size(93, 24);
            checkBoxTCP.TabIndex = 7;
            checkBoxTCP.Text = "仅阻止TCP";
            checkBoxTCP.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(19, 9);
            pictureBox2.Margin = new Padding(2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(64, 64);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 10F);
            label1.ForeColor = Color.DarkRed;
            label1.Location = new Point(9, 76);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(489, 20);
            label1.TabIndex = 10;
            label1.Text = "————————————————————————————————";
            // 
            // label阻止联机
            // 
            label阻止联机.AutoSize = true;
            label阻止联机.Font = new Font("Microsoft YaHei UI", 10F);
            label阻止联机.ForeColor = Color.DarkRed;
            label阻止联机.Location = new Point(24, 97);
            label阻止联机.Margin = new Padding(2, 0, 2, 0);
            label阻止联机.Name = "label阻止联机";
            label阻止联机.Size = new Size(79, 20);
            label阻止联机.TabIndex = 14;
            label阻止联机.Text = "断玩家联机";
            // 
            // label云服务器
            // 
            label云服务器.AutoSize = true;
            label云服务器.Font = new Font("Microsoft YaHei UI", 10F);
            label云服务器.ForeColor = Color.DarkRed;
            label云服务器.Location = new Point(24, 132);
            label云服务器.Margin = new Padding(2, 0, 2, 0);
            label云服务器.Name = "label云服务器";
            label云服务器.Size = new Size(79, 20);
            label云服务器.TabIndex = 15;
            label云服务器.Text = "断云服务器";
            // 
            // checkBox存档
            // 
            checkBox存档.AutoSize = true;
            checkBox存档.FlatStyle = FlatStyle.Flat;
            checkBox存档.Font = new Font("Microsoft YaHei UI", 10F);
            checkBox存档.Location = new Point(221, 130);
            checkBox存档.Name = "checkBox存档";
            checkBox存档.Size = new Size(179, 24);
            checkBox存档.TabIndex = 16;
            checkBox存档.Text = "阻止云存档、交易服务器";
            checkBox存档.UseVisualStyleBackColor = true;
            // 
            // checkBoxUDP
            // 
            checkBoxUDP.AutoSize = true;
            checkBoxUDP.FlatStyle = FlatStyle.Flat;
            checkBoxUDP.Font = new Font("Microsoft YaHei UI", 10F);
            checkBoxUDP.Location = new Point(122, 95);
            checkBoxUDP.Name = "checkBoxUDP";
            checkBoxUDP.Size = new Size(97, 24);
            checkBoxUDP.TabIndex = 18;
            checkBoxUDP.Text = "仅阻止UDP";
            checkBoxUDP.UseVisualStyleBackColor = true;
            // 
            // comboBox热键
            // 
            comboBox热键.BackColor = SystemColors.Control;
            comboBox热键.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox热键.ForeColor = Color.Black;
            comboBox热键.FormattingEnabled = true;
            comboBox热键.Items.AddRange(new object[] { "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12" });
            comboBox热键.Location = new Point(300, 15);
            comboBox热键.Name = "comboBox热键";
            comboBox热键.Size = new Size(48, 25);
            comboBox热键.TabIndex = 20;
            // 
            // label加速提示
            // 
            label加速提示.AutoSize = true;
            label加速提示.Font = new Font("Microsoft YaHei UI", 10F);
            label加速提示.ForeColor = SystemColors.ControlDarkDark;
            label加速提示.Location = new Point(221, 97);
            label加速提示.Margin = new Padding(2, 0, 2, 0);
            label加速提示.Name = "label加速提示";
            label加速提示.Size = new Size(257, 20);
            label加速提示.TabIndex = 21;
            label加速提示.Text = "* 用于战局卡单人或者锁定单人公开战局";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 10F);
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(405, 133);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(71, 20);
            label2.TabIndex = 22;
            label2.Text = "* 用于085";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft YaHei UI", 10F);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(24, 163);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(453, 20);
            label3.TabIndex = 23;
            label3.Text = "* 请注意，当使用阻止云存档、交易服务器选项时请设置加速器路由模式";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 461);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnToggle);
            Controls.Add(label加速提示);
            Controls.Add(comboBox热键);
            Controls.Add(checkBoxUDP);
            Controls.Add(checkBox存档);
            Controls.Add(label云服务器);
            Controls.Add(label阻止联机);
            Controls.Add(label1);
            Controls.Add(checkBoxTCP);
            Controls.Add(label标题);
            Controls.Add(label网址);
            Controls.Add(label版本号);
            Controls.Add(pictureBox1);
            Controls.Add(label底部提示);
            Controls.Add(lblStatus);
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
        private Label label底部提示;
        private PictureBox pictureBox1;
        private Label label版本号;
        private Label label网址;
        private Label label标题;
        private CheckBox checkBoxTCP;
        private PictureBox pictureBox2;
        private Label label1;
        private Label label阻止联机;
        private Label label云服务器;
        private CheckBox checkBox存档;
        private CheckBox checkBoxUDP;
        private ComboBox comboBox热键;
        private Label label加速提示;
        private Label label2;
        private Label label3;
    }
}
