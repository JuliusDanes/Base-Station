namespace BaseStation
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbxX = new System.Windows.Forms.TextBox();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.tbxY = new System.Windows.Forms.TextBox();
            this.Robot3 = new System.Windows.Forms.PictureBox();
            this.Robot2 = new System.Windows.Forms.PictureBox();
            this.Robot1 = new System.Windows.Forms.PictureBox();
            this.Bola = new System.Windows.Forms.PictureBox();
            this.Point1 = new System.Windows.Forms.RadioButton();
            this.Lap = new System.Windows.Forms.PictureBox();
            this.gbxRefBox = new System.Windows.Forms.GroupBox();
            this.tbxIPRefBox = new System.Windows.Forms.TextBox();
            this.lblPortRefBox = new System.Windows.Forms.Label();
            this.tbxPortRefBox = new System.Windows.Forms.TextBox();
            this.lblIPRefBox = new System.Windows.Forms.Label();
            this.btnConnectRefBox = new System.Windows.Forms.Button();
            this.btnDisconnectRefBox = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbxStatus = new System.Windows.Forms.RichTextBox();
            this.gbxRobot2 = new System.Windows.Forms.GroupBox();
            this.tbxIPRobot2 = new System.Windows.Forms.TextBox();
            this.lblPortRobot2 = new System.Windows.Forms.Label();
            this.tbxPortRobot2 = new System.Windows.Forms.TextBox();
            this.lblIPRobot2 = new System.Windows.Forms.Label();
            this.btnConnnectRobot2 = new System.Windows.Forms.Button();
            this.btnDisconnnectRobot2 = new System.Windows.Forms.Button();
            this.gbxRobot3 = new System.Windows.Forms.GroupBox();
            this.tbxIPRobot3 = new System.Windows.Forms.TextBox();
            this.lblPortRobot3 = new System.Windows.Forms.Label();
            this.tbxPortRobot3 = new System.Windows.Forms.TextBox();
            this.lblIPRobot3 = new System.Windows.Forms.Label();
            this.btnConnnectRobot3 = new System.Windows.Forms.Button();
            this.btnDisconnnectRobot3 = new System.Windows.Forms.Button();
            this.gbxRobot1 = new System.Windows.Forms.GroupBox();
            this.tbxIPRobot1 = new System.Windows.Forms.TextBox();
            this.lblPortRobot1 = new System.Windows.Forms.Label();
            this.tbxPortRobot1 = new System.Windows.Forms.TextBox();
            this.lblIPCRobot1 = new System.Windows.Forms.Label();
            this.btnConnnectRobot1 = new System.Windows.Forms.Button();
            this.btnDisconnnectRobot1 = new System.Windows.Forms.Button();
            this.gbxBS = new System.Windows.Forms.GroupBox();
            this.tbxIPBS = new System.Windows.Forms.TextBox();
            this.lblPortBS = new System.Windows.Forms.Label();
            this.tbxPortBS = new System.Windows.Forms.TextBox();
            this.lblIPBS = new System.Windows.Forms.Label();
            this.btnOpenServer = new System.Windows.Forms.Button();
            this.btnCloseServer = new System.Windows.Forms.Button();
            this.tbxMessage = new System.Windows.Forms.RichTextBox();
            this.lblTimer = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Robot3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Robot2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Robot1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Lap)).BeginInit();
            this.gbxRefBox.SuspendLayout();
            this.gbxRobot2.SuspendLayout();
            this.gbxRobot3.SuspendLayout();
            this.gbxRobot1.SuspendLayout();
            this.gbxBS.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.groupBox1.Controls.Add(this.tbxX);
            this.groupBox1.Controls.Add(this.lblX);
            this.groupBox1.Controls.Add(this.lblY);
            this.groupBox1.Controls.Add(this.tbxY);
            this.groupBox1.Location = new System.Drawing.Point(930, 182);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 75);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encoder";
            // 
            // tbxX
            // 
            this.tbxX.Location = new System.Drawing.Point(37, 19);
            this.tbxX.Name = "tbxX";
            this.tbxX.Size = new System.Drawing.Size(100, 20);
            this.tbxX.TabIndex = 4;
            this.tbxX.Text = "0";
            this.tbxX.TextChanged += new System.EventHandler(this.tbxXYChanged);
            this.tbxX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.changeCounter);
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(17, 22);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(14, 13);
            this.lblX.TabIndex = 2;
            this.lblX.Text = "X";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(17, 46);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(14, 13);
            this.lblY.TabIndex = 3;
            this.lblY.Text = "Y";
            // 
            // tbxY
            // 
            this.tbxY.Location = new System.Drawing.Point(37, 43);
            this.tbxY.Name = "tbxY";
            this.tbxY.Size = new System.Drawing.Size(100, 20);
            this.tbxY.TabIndex = 5;
            this.tbxY.Text = "0";
            this.tbxY.TextChanged += new System.EventHandler(this.tbxXYChanged);
            this.tbxY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.changeCounter);
            // 
            // Robot3
            // 
            this.Robot3.BackColor = System.Drawing.Color.Transparent;
            this.Robot3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Robot3.BackgroundImage")));
            this.Robot3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Robot3.Location = new System.Drawing.Point(692, 288);
            this.Robot3.Name = "Robot3";
            this.Robot3.Size = new System.Drawing.Size(25, 25);
            this.Robot3.TabIndex = 28;
            this.Robot3.TabStop = false;
            // 
            // Robot2
            // 
            this.Robot2.BackColor = System.Drawing.Color.Transparent;
            this.Robot2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Robot2.BackgroundImage")));
            this.Robot2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Robot2.Location = new System.Drawing.Point(616, 240);
            this.Robot2.Name = "Robot2";
            this.Robot2.Size = new System.Drawing.Size(25, 25);
            this.Robot2.TabIndex = 27;
            this.Robot2.TabStop = false;
            // 
            // Robot1
            // 
            this.Robot1.BackColor = System.Drawing.Color.Transparent;
            this.Robot1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Robot1.BackgroundImage")));
            this.Robot1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Robot1.Location = new System.Drawing.Point(584, 313);
            this.Robot1.Name = "Robot1";
            this.Robot1.Size = new System.Drawing.Size(25, 25);
            this.Robot1.TabIndex = 26;
            this.Robot1.TabStop = false;
            // 
            // Bola
            // 
            this.Bola.BackColor = System.Drawing.Color.Transparent;
            this.Bola.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Bola.BackgroundImage")));
            this.Bola.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Bola.Location = new System.Drawing.Point(548, 288);
            this.Bola.Name = "Bola";
            this.Bola.Size = new System.Drawing.Size(18, 18);
            this.Bola.TabIndex = 25;
            this.Bola.TabStop = false;
            // 
            // Point1
            // 
            this.Point1.AutoSize = true;
            this.Point1.BackColor = System.Drawing.Color.Transparent;
            this.Point1.Location = new System.Drawing.Point(584, 432);
            this.Point1.Name = "Point1";
            this.Point1.Size = new System.Drawing.Size(14, 13);
            this.Point1.TabIndex = 19;
            this.Point1.TabStop = true;
            this.Point1.UseVisualStyleBackColor = false;
            // 
            // Lap
            // 
            this.Lap.BackColor = System.Drawing.SystemColors.Control;
            this.Lap.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Lap.BackgroundImage")));
            this.Lap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Lap.Location = new System.Drawing.Point(407, 182);
            this.Lap.Name = "Lap";
            this.Lap.Size = new System.Drawing.Size(450, 300);
            this.Lap.TabIndex = 18;
            this.Lap.TabStop = false;
            // 
            // gbxRefBox
            // 
            this.gbxRefBox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.gbxRefBox.Controls.Add(this.tbxIPRefBox);
            this.gbxRefBox.Controls.Add(this.lblPortRefBox);
            this.gbxRefBox.Controls.Add(this.tbxPortRefBox);
            this.gbxRefBox.Controls.Add(this.lblIPRefBox);
            this.gbxRefBox.Controls.Add(this.btnConnectRefBox);
            this.gbxRefBox.Controls.Add(this.btnDisconnectRefBox);
            this.gbxRefBox.Location = new System.Drawing.Point(12, 181);
            this.gbxRefBox.Name = "gbxRefBox";
            this.gbxRefBox.Size = new System.Drawing.Size(303, 85);
            this.gbxRefBox.TabIndex = 36;
            this.gbxRefBox.TabStop = false;
            this.gbxRefBox.Text = "RefereeBox";
            // 
            // tbxIPRefBox
            // 
            this.tbxIPRefBox.Location = new System.Drawing.Point(37, 19);
            this.tbxIPRefBox.Name = "tbxIPRefBox";
            this.tbxIPRefBox.Size = new System.Drawing.Size(150, 20);
            this.tbxIPRefBox.TabIndex = 0;
            // 
            // lblPortRefBox
            // 
            this.lblPortRefBox.AutoSize = true;
            this.lblPortRefBox.Location = new System.Drawing.Point(5, 52);
            this.lblPortRefBox.Name = "lblPortRefBox";
            this.lblPortRefBox.Size = new System.Drawing.Size(26, 13);
            this.lblPortRefBox.TabIndex = 6;
            this.lblPortRefBox.Text = "Port";
            // 
            // tbxPortRefBox
            // 
            this.tbxPortRefBox.Location = new System.Drawing.Point(37, 45);
            this.tbxPortRefBox.Name = "tbxPortRefBox";
            this.tbxPortRefBox.Size = new System.Drawing.Size(50, 20);
            this.tbxPortRefBox.TabIndex = 1;
            // 
            // lblIPRefBox
            // 
            this.lblIPRefBox.AutoSize = true;
            this.lblIPRefBox.Location = new System.Drawing.Point(14, 22);
            this.lblIPRefBox.Name = "lblIPRefBox";
            this.lblIPRefBox.Size = new System.Drawing.Size(17, 13);
            this.lblIPRefBox.TabIndex = 5;
            this.lblIPRefBox.Text = "IP";
            // 
            // btnConnectRefBox
            // 
            this.btnConnectRefBox.Location = new System.Drawing.Point(202, 17);
            this.btnConnectRefBox.Name = "btnConnectRefBox";
            this.btnConnectRefBox.Size = new System.Drawing.Size(75, 23);
            this.btnConnectRefBox.TabIndex = 3;
            this.btnConnectRefBox.Text = "Connect";
            this.btnConnectRefBox.UseVisualStyleBackColor = true;
            this.btnConnectRefBox.Click += new System.EventHandler(this.btnConnectRefBox_Click);
            // 
            // btnDisconnectRefBox
            // 
            this.btnDisconnectRefBox.Location = new System.Drawing.Point(202, 47);
            this.btnDisconnectRefBox.Name = "btnDisconnectRefBox";
            this.btnDisconnectRefBox.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnectRefBox.TabIndex = 4;
            this.btnDisconnectRefBox.Text = "Disconnect";
            this.btnDisconnectRefBox.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(318, 649);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 33;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // tbxStatus
            // 
            this.tbxStatus.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.tbxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxStatus.ForeColor = System.Drawing.SystemColors.Info;
            this.tbxStatus.Location = new System.Drawing.Point(407, 488);
            this.tbxStatus.Name = "tbxStatus";
            this.tbxStatus.Size = new System.Drawing.Size(450, 193);
            this.tbxStatus.TabIndex = 39;
            this.tbxStatus.Text = "";
            this.tbxStatus.TextChanged += new System.EventHandler(this.tbxStatus_TextChanged);
            // 
            // gbxRobot2
            // 
            this.gbxRobot2.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.gbxRobot2.Controls.Add(this.tbxIPRobot2);
            this.gbxRobot2.Controls.Add(this.lblPortRobot2);
            this.gbxRobot2.Controls.Add(this.tbxPortRobot2);
            this.gbxRobot2.Controls.Add(this.lblIPRobot2);
            this.gbxRobot2.Controls.Add(this.btnConnnectRobot2);
            this.gbxRobot2.Controls.Add(this.btnDisconnnectRobot2);
            this.gbxRobot2.Location = new System.Drawing.Point(12, 363);
            this.gbxRobot2.Name = "gbxRobot2";
            this.gbxRobot2.Size = new System.Drawing.Size(303, 85);
            this.gbxRobot2.TabIndex = 37;
            this.gbxRobot2.TabStop = false;
            this.gbxRobot2.Text = "Robot2";
            // 
            // tbxIPRobot2
            // 
            this.tbxIPRobot2.Location = new System.Drawing.Point(37, 19);
            this.tbxIPRobot2.Name = "tbxIPRobot2";
            this.tbxIPRobot2.Size = new System.Drawing.Size(150, 20);
            this.tbxIPRobot2.TabIndex = 0;
            // 
            // lblPortRobot2
            // 
            this.lblPortRobot2.AutoSize = true;
            this.lblPortRobot2.Location = new System.Drawing.Point(5, 52);
            this.lblPortRobot2.Name = "lblPortRobot2";
            this.lblPortRobot2.Size = new System.Drawing.Size(26, 13);
            this.lblPortRobot2.TabIndex = 6;
            this.lblPortRobot2.Text = "Port";
            // 
            // tbxPortRobot2
            // 
            this.tbxPortRobot2.Location = new System.Drawing.Point(37, 45);
            this.tbxPortRobot2.Name = "tbxPortRobot2";
            this.tbxPortRobot2.Size = new System.Drawing.Size(50, 20);
            this.tbxPortRobot2.TabIndex = 1;
            // 
            // lblIPRobot2
            // 
            this.lblIPRobot2.AutoSize = true;
            this.lblIPRobot2.Location = new System.Drawing.Point(14, 22);
            this.lblIPRobot2.Name = "lblIPRobot2";
            this.lblIPRobot2.Size = new System.Drawing.Size(17, 13);
            this.lblIPRobot2.TabIndex = 5;
            this.lblIPRobot2.Text = "IP";
            // 
            // btnConnnectRobot2
            // 
            this.btnConnnectRobot2.Location = new System.Drawing.Point(202, 17);
            this.btnConnnectRobot2.Name = "btnConnnectRobot2";
            this.btnConnnectRobot2.Size = new System.Drawing.Size(75, 23);
            this.btnConnnectRobot2.TabIndex = 3;
            this.btnConnnectRobot2.Text = "Connect";
            this.btnConnnectRobot2.UseVisualStyleBackColor = true;
            this.btnConnnectRobot2.Click += new System.EventHandler(this.btnConnnectRobot2_Click);
            // 
            // btnDisconnnectRobot2
            // 
            this.btnDisconnnectRobot2.Location = new System.Drawing.Point(202, 47);
            this.btnDisconnnectRobot2.Name = "btnDisconnnectRobot2";
            this.btnDisconnnectRobot2.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnnectRobot2.TabIndex = 4;
            this.btnDisconnnectRobot2.Text = "Disconnect";
            this.btnDisconnnectRobot2.UseVisualStyleBackColor = true;
            // 
            // gbxRobot3
            // 
            this.gbxRobot3.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.gbxRobot3.Controls.Add(this.tbxIPRobot3);
            this.gbxRobot3.Controls.Add(this.lblPortRobot3);
            this.gbxRobot3.Controls.Add(this.tbxPortRobot3);
            this.gbxRobot3.Controls.Add(this.lblIPRobot3);
            this.gbxRobot3.Controls.Add(this.btnConnnectRobot3);
            this.gbxRobot3.Controls.Add(this.btnDisconnnectRobot3);
            this.gbxRobot3.Location = new System.Drawing.Point(12, 454);
            this.gbxRobot3.Name = "gbxRobot3";
            this.gbxRobot3.Size = new System.Drawing.Size(303, 85);
            this.gbxRobot3.TabIndex = 38;
            this.gbxRobot3.TabStop = false;
            this.gbxRobot3.Text = "Robot3";
            // 
            // tbxIPRobot3
            // 
            this.tbxIPRobot3.Location = new System.Drawing.Point(37, 19);
            this.tbxIPRobot3.Name = "tbxIPRobot3";
            this.tbxIPRobot3.Size = new System.Drawing.Size(150, 20);
            this.tbxIPRobot3.TabIndex = 0;
            // 
            // lblPortRobot3
            // 
            this.lblPortRobot3.AutoSize = true;
            this.lblPortRobot3.Location = new System.Drawing.Point(5, 52);
            this.lblPortRobot3.Name = "lblPortRobot3";
            this.lblPortRobot3.Size = new System.Drawing.Size(26, 13);
            this.lblPortRobot3.TabIndex = 6;
            this.lblPortRobot3.Text = "Port";
            // 
            // tbxPortRobot3
            // 
            this.tbxPortRobot3.Location = new System.Drawing.Point(37, 45);
            this.tbxPortRobot3.Name = "tbxPortRobot3";
            this.tbxPortRobot3.Size = new System.Drawing.Size(50, 20);
            this.tbxPortRobot3.TabIndex = 1;
            // 
            // lblIPRobot3
            // 
            this.lblIPRobot3.AutoSize = true;
            this.lblIPRobot3.Location = new System.Drawing.Point(14, 22);
            this.lblIPRobot3.Name = "lblIPRobot3";
            this.lblIPRobot3.Size = new System.Drawing.Size(17, 13);
            this.lblIPRobot3.TabIndex = 5;
            this.lblIPRobot3.Text = "IP";
            // 
            // btnConnnectRobot3
            // 
            this.btnConnnectRobot3.Location = new System.Drawing.Point(202, 17);
            this.btnConnnectRobot3.Name = "btnConnnectRobot3";
            this.btnConnnectRobot3.Size = new System.Drawing.Size(75, 23);
            this.btnConnnectRobot3.TabIndex = 3;
            this.btnConnnectRobot3.Text = "Connect";
            this.btnConnnectRobot3.UseVisualStyleBackColor = true;
            this.btnConnnectRobot3.Click += new System.EventHandler(this.btnConnnectRobot3_Click);
            // 
            // btnDisconnnectRobot3
            // 
            this.btnDisconnnectRobot3.Location = new System.Drawing.Point(202, 47);
            this.btnDisconnnectRobot3.Name = "btnDisconnnectRobot3";
            this.btnDisconnnectRobot3.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnnectRobot3.TabIndex = 4;
            this.btnDisconnnectRobot3.Text = "Disconnect";
            this.btnDisconnnectRobot3.UseVisualStyleBackColor = true;
            // 
            // gbxRobot1
            // 
            this.gbxRobot1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.gbxRobot1.Controls.Add(this.tbxIPRobot1);
            this.gbxRobot1.Controls.Add(this.lblPortRobot1);
            this.gbxRobot1.Controls.Add(this.tbxPortRobot1);
            this.gbxRobot1.Controls.Add(this.lblIPCRobot1);
            this.gbxRobot1.Controls.Add(this.btnConnnectRobot1);
            this.gbxRobot1.Controls.Add(this.btnDisconnnectRobot1);
            this.gbxRobot1.Location = new System.Drawing.Point(12, 272);
            this.gbxRobot1.Name = "gbxRobot1";
            this.gbxRobot1.Size = new System.Drawing.Size(303, 85);
            this.gbxRobot1.TabIndex = 35;
            this.gbxRobot1.TabStop = false;
            this.gbxRobot1.Text = "Robot1";
            // 
            // tbxIPRobot1
            // 
            this.tbxIPRobot1.Location = new System.Drawing.Point(37, 19);
            this.tbxIPRobot1.Name = "tbxIPRobot1";
            this.tbxIPRobot1.Size = new System.Drawing.Size(150, 20);
            this.tbxIPRobot1.TabIndex = 0;
            // 
            // lblPortRobot1
            // 
            this.lblPortRobot1.AutoSize = true;
            this.lblPortRobot1.Location = new System.Drawing.Point(5, 52);
            this.lblPortRobot1.Name = "lblPortRobot1";
            this.lblPortRobot1.Size = new System.Drawing.Size(26, 13);
            this.lblPortRobot1.TabIndex = 6;
            this.lblPortRobot1.Text = "Port";
            // 
            // tbxPortRobot1
            // 
            this.tbxPortRobot1.Location = new System.Drawing.Point(37, 45);
            this.tbxPortRobot1.Name = "tbxPortRobot1";
            this.tbxPortRobot1.Size = new System.Drawing.Size(50, 20);
            this.tbxPortRobot1.TabIndex = 1;
            // 
            // lblIPCRobot1
            // 
            this.lblIPCRobot1.AutoSize = true;
            this.lblIPCRobot1.Location = new System.Drawing.Point(14, 22);
            this.lblIPCRobot1.Name = "lblIPCRobot1";
            this.lblIPCRobot1.Size = new System.Drawing.Size(17, 13);
            this.lblIPCRobot1.TabIndex = 5;
            this.lblIPCRobot1.Text = "IP";
            // 
            // btnConnnectRobot1
            // 
            this.btnConnnectRobot1.Location = new System.Drawing.Point(202, 17);
            this.btnConnnectRobot1.Name = "btnConnnectRobot1";
            this.btnConnnectRobot1.Size = new System.Drawing.Size(75, 23);
            this.btnConnnectRobot1.TabIndex = 3;
            this.btnConnnectRobot1.Text = "Connect";
            this.btnConnnectRobot1.UseVisualStyleBackColor = true;
            this.btnConnnectRobot1.Click += new System.EventHandler(this.btnConnnectRobot1_Click);
            // 
            // btnDisconnnectRobot1
            // 
            this.btnDisconnnectRobot1.Location = new System.Drawing.Point(202, 47);
            this.btnDisconnnectRobot1.Name = "btnDisconnnectRobot1";
            this.btnDisconnnectRobot1.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnnectRobot1.TabIndex = 4;
            this.btnDisconnnectRobot1.Text = "Disconnect";
            this.btnDisconnnectRobot1.UseVisualStyleBackColor = true;
            // 
            // gbxBS
            // 
            this.gbxBS.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.gbxBS.Controls.Add(this.tbxIPBS);
            this.gbxBS.Controls.Add(this.lblPortBS);
            this.gbxBS.Controls.Add(this.tbxPortBS);
            this.gbxBS.Controls.Add(this.lblIPBS);
            this.gbxBS.Controls.Add(this.btnOpenServer);
            this.gbxBS.Controls.Add(this.btnCloseServer);
            this.gbxBS.Location = new System.Drawing.Point(12, 62);
            this.gbxBS.Name = "gbxBS";
            this.gbxBS.Size = new System.Drawing.Size(303, 85);
            this.gbxBS.TabIndex = 34;
            this.gbxBS.TabStop = false;
            this.gbxBS.Text = "BaseStation";
            // 
            // tbxIPBS
            // 
            this.tbxIPBS.Location = new System.Drawing.Point(37, 19);
            this.tbxIPBS.Name = "tbxIPBS";
            this.tbxIPBS.Size = new System.Drawing.Size(150, 20);
            this.tbxIPBS.TabIndex = 0;
            // 
            // lblPortBS
            // 
            this.lblPortBS.AutoSize = true;
            this.lblPortBS.Location = new System.Drawing.Point(5, 52);
            this.lblPortBS.Name = "lblPortBS";
            this.lblPortBS.Size = new System.Drawing.Size(26, 13);
            this.lblPortBS.TabIndex = 6;
            this.lblPortBS.Text = "Port";
            // 
            // tbxPortBS
            // 
            this.tbxPortBS.Location = new System.Drawing.Point(37, 45);
            this.tbxPortBS.Name = "tbxPortBS";
            this.tbxPortBS.Size = new System.Drawing.Size(50, 20);
            this.tbxPortBS.TabIndex = 1;
            // 
            // lblIPBS
            // 
            this.lblIPBS.AutoSize = true;
            this.lblIPBS.Location = new System.Drawing.Point(14, 22);
            this.lblIPBS.Name = "lblIPBS";
            this.lblIPBS.Size = new System.Drawing.Size(17, 13);
            this.lblIPBS.TabIndex = 5;
            this.lblIPBS.Text = "IP";
            // 
            // btnOpenServer
            // 
            this.btnOpenServer.Location = new System.Drawing.Point(202, 17);
            this.btnOpenServer.Name = "btnOpenServer";
            this.btnOpenServer.Size = new System.Drawing.Size(75, 23);
            this.btnOpenServer.TabIndex = 3;
            this.btnOpenServer.Text = "Open";
            this.btnOpenServer.UseVisualStyleBackColor = true;
            this.btnOpenServer.Click += new System.EventHandler(this.btnOpenServer_Click);
            // 
            // btnCloseServer
            // 
            this.btnCloseServer.Location = new System.Drawing.Point(202, 47);
            this.btnCloseServer.Name = "btnCloseServer";
            this.btnCloseServer.Size = new System.Drawing.Size(75, 23);
            this.btnCloseServer.TabIndex = 4;
            this.btnCloseServer.Text = "Close";
            this.btnCloseServer.UseVisualStyleBackColor = true;
            // 
            // tbxMessage
            // 
            this.tbxMessage.BackColor = System.Drawing.SystemColors.MenuBar;
            this.tbxMessage.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxMessage.Location = new System.Drawing.Point(12, 558);
            this.tbxMessage.Name = "tbxMessage";
            this.tbxMessage.Size = new System.Drawing.Size(381, 85);
            this.tbxMessage.TabIndex = 32;
            this.tbxMessage.Text = "";
            this.tbxMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxMessage_KeyDown);
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Font = new System.Drawing.Font("OCR A Extended", 50.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lblTimer.Location = new System.Drawing.Point(521, 78);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(230, 69);
            this.lblTimer.TabIndex = 43;
            this.lblTimer.Text = "00:00";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(234, 153);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 44;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.gbxRefBox);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbxStatus);
            this.Controls.Add(this.gbxRobot2);
            this.Controls.Add(this.gbxRobot3);
            this.Controls.Add(this.gbxRobot1);
            this.Controls.Add(this.gbxBS);
            this.Controls.Add(this.tbxMessage);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Robot3);
            this.Controls.Add(this.Robot2);
            this.Controls.Add(this.Robot1);
            this.Controls.Add(this.Bola);
            this.Controls.Add(this.Point1);
            this.Controls.Add(this.Lap);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BaseStation";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Robot3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Robot2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Robot1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Lap)).EndInit();
            this.gbxRefBox.ResumeLayout(false);
            this.gbxRefBox.PerformLayout();
            this.gbxRobot2.ResumeLayout(false);
            this.gbxRobot2.PerformLayout();
            this.gbxRobot3.ResumeLayout(false);
            this.gbxRobot3.PerformLayout();
            this.gbxRobot1.ResumeLayout(false);
            this.gbxRobot1.PerformLayout();
            this.gbxBS.ResumeLayout(false);
            this.gbxBS.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbxX;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.TextBox tbxY;
        private System.Windows.Forms.PictureBox Robot3;
        private System.Windows.Forms.PictureBox Robot2;
        private System.Windows.Forms.PictureBox Robot1;
        private System.Windows.Forms.PictureBox Bola;
        public System.Windows.Forms.RadioButton Point1;
        private System.Windows.Forms.PictureBox Lap;
        private System.Windows.Forms.GroupBox gbxRefBox;
        private System.Windows.Forms.TextBox tbxIPRefBox;
        private System.Windows.Forms.Label lblPortRefBox;
        private System.Windows.Forms.TextBox tbxPortRefBox;
        private System.Windows.Forms.Label lblIPRefBox;
        private System.Windows.Forms.Button btnConnectRefBox;
        private System.Windows.Forms.Button btnDisconnectRefBox;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RichTextBox tbxStatus;
        private System.Windows.Forms.GroupBox gbxRobot2;
        private System.Windows.Forms.TextBox tbxIPRobot2;
        private System.Windows.Forms.Label lblPortRobot2;
        private System.Windows.Forms.TextBox tbxPortRobot2;
        private System.Windows.Forms.Label lblIPRobot2;
        private System.Windows.Forms.Button btnConnnectRobot2;
        private System.Windows.Forms.Button btnDisconnnectRobot2;
        private System.Windows.Forms.GroupBox gbxRobot3;
        private System.Windows.Forms.TextBox tbxIPRobot3;
        private System.Windows.Forms.Label lblPortRobot3;
        private System.Windows.Forms.TextBox tbxPortRobot3;
        private System.Windows.Forms.Label lblIPRobot3;
        private System.Windows.Forms.Button btnConnnectRobot3;
        private System.Windows.Forms.Button btnDisconnnectRobot3;
        private System.Windows.Forms.GroupBox gbxRobot1;
        private System.Windows.Forms.TextBox tbxIPRobot1;
        private System.Windows.Forms.Label lblPortRobot1;
        private System.Windows.Forms.TextBox tbxPortRobot1;
        private System.Windows.Forms.Label lblIPCRobot1;
        private System.Windows.Forms.Button btnConnnectRobot1;
        private System.Windows.Forms.Button btnDisconnnectRobot1;
        private System.Windows.Forms.GroupBox gbxBS;
        private System.Windows.Forms.Label lblPortBS;
        private System.Windows.Forms.TextBox tbxPortBS;
        private System.Windows.Forms.Label lblIPBS;
        private System.Windows.Forms.Button btnOpenServer;
        private System.Windows.Forms.Button btnCloseServer;
        private System.Windows.Forms.RichTextBox tbxMessage;
        public System.Windows.Forms.TextBox tbxIPBS;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button button1;
    }
}

