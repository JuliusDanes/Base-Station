using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using MaterialSkin;
using MaterialSkin.Controls;

namespace BaseStation
{  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            setTransparent(Lap, new dynamic[] { PointBall, PointRobot1, PointRobot2, PointRobot3 });
            setTransparent(grpBaseStation, new dynamic[] { lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS, lblPipeBS });
            setTransparent(grpRefereeBox, new dynamic[] { lblRefereeBox, lblConnectionRB, tbxIPRB, tbxPortRB, lblPipeRB });
            setTransparent(grpRobot1, new dynamic[] { lblRobot1, lblConnectionR1, chkR1, tbxIPR1, tbxPortR1, lblPipeR1, lblEncoderR1, lblEncCommaR1, tbxEncXR1, tbxEncYR1, lblScreenR1, tbxScrXR1, tbxScrYR1, lblScrCommaR1, YCard1R1, YCard2R1, RCardR1 });
            setTransparent(grpRobot2, new dynamic[] { lblRobot2, lblConnectionR2, chkR2, tbxIPR2, tbxPortR2, lblPipeR2, lblEncoderR2, lblEncCommaR2, tbxEncXR2, tbxEncYR2, lblScreenR2, tbxScrXR2, tbxScrYR2, lblScrCommaR2, YCard1R2, YCard2R2, RCardR2 });
            setTransparent(grpRobot3, new dynamic[] { lblRobot3, lblConnectionR3, chkR3, tbxIPR3, tbxPortR3, lblPipeR3, lblEncoderR3, lblEncCommaR3, tbxEncXR3, tbxEncYR3, lblScreenR3, tbxScrXR3, tbxScrYR3, lblScrCommaR3, YCard1R3, YCard2R3, RCardR3 });
            setTransparent(lblDiv, new dynamic[] { lblPenalty, lblYCard, lblRCard, lblFouls, lblCorner, lblGoalKick });

            // Create a material theme manager and add the form to manage (this)
            //MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            //materialSkinManager.AddFormToManage(this);
            //materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            //Robot1.SkinManager.ColorScheme = new ColorScheme(
            //    Primary.Blue400, Primary.Blue500,
            //    Primary.Blue500, Accent.LightBlue200,
            //    TextShade.WHITE
            //);
        }

        HelperClass hc = new HelperClass();     

        private void Form1_Load(object sender, EventArgs e)
        {
            tbxIPBS.Text /*= GetIPAddress()*/ = "192.168.165.10";
            tbxPortBS.Text = "8686";
            tbxIPRB.Text = "169.254.162.201";
            tbxPortRB.Text = "28097";
            tbxIPR1.Text /*= "169.254.162.201"*/ = GetIPAddress();
            tbxPortR1.Text = "8686";

            resetLocation();
            time.Start();
            timer.Start();
        }

        private void time_Tick(object sender, EventArgs e)
        {
            this.lblTime.Text = "[  "+ DateTime.Now.ToString("HH:mm:ss") +"  ]";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string time = lblTimer.Text;
            var _time = time.Split(':');            // split minute and second
            int count = int.Parse(_time[1]);

            if ((count < 59) && (count < 9))        // Seconds increment
                time = _time[0] + ":0" + (count + 1).ToString();
            else if ((count < 59) && (count >= 9))  // Seconds increment
                time = _time[0] + ":" + (count + 1).ToString();
            else if (int.Parse(_time[0]) < 9)       // Minutes increment
                time = "0" + (int.Parse(_time[0]) + 1).ToString() + ":" + "00";
            else                                    // Minutes increment
                time = (int.Parse(_time[0]) + 1).ToString() + ":" + "00";
            hc.SetText(this, lblTimer, time);
        }
        

        //////////////////////////////////////////////////////////////      TRACK LOCACTION       //////////////////////////////////////////////////////////////
        ///
        void setTransparent(dynamic backImage, dynamic[] frontImages)
        {
            foreach (var frontImage in frontImages)
            {
                var pos = this.PointToScreen(frontImage.Location);
                pos = backImage.PointToClient(pos);
                frontImage.Parent = backImage;
                frontImage.Location = pos;
            }
        }

        void moveLoc(int encodX, int encodY, dynamic robot)
        {
            Point point00Lap = new Point(26, 20);
            Point point00Robot = new Point(robot.Size.Width / 2, robot.Size.Height / 2);
            Point newLoc = new Point((point00Lap.X + encodX - point00Robot.X), (point00Lap.Y + encodY - point00Robot.Y));
            hc.SetLocation(this, robot, newLoc);
        }

        void changeCounter(object sender, KeyEventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1 }, { tbxEncXR2, tbxEncYR2 }, { tbxEncXR3, tbxEncYR3 }, { tbxScrXR1, tbxScrYR1 }, { tbxScrXR2, tbxScrYR2 }, { tbxScrXR3, tbxScrYR3 }, { tbxGotoX, tbxGotoY } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            if (e.KeyCode == Keys.Right)
                arr[n, 0].Text = (int.Parse(arr[n, 0].Text) + 1).ToString();
            else if (e.KeyCode == Keys.Left)
                arr[n, 0].Text = (int.Parse(arr[n, 0].Text) - 1).ToString();
            else if (e.KeyCode == Keys.Up)
                arr[n, 1].Text = (int.Parse(arr[n, 1].Text) - 1).ToString();
            else if (e.KeyCode == Keys.Down)
                arr[n, 1].Text = (int.Parse(arr[n, 1].Text) + 1).ToString();
        }

        void tbxXYChanged(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, PointRobot1 }, { tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, PointRobot2 }, { tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, PointRobot3 } };
            int n = 0;
            int[] val = new int[2];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            if ((!string.IsNullOrWhiteSpace(arr[n, 0].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 1].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 2].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 3].Text)))
            {
                if (obj.StartsWith("tbxEnc"))   // Encoder then using scale 1:20
                {
                    val[0] = (int.Parse(arr[n,0].Text));
                    val[1] = (int.Parse(arr[n,1].Text));
                    hc.SetText(this, arr[n, 0], val[0].ToString());          // On encoder tbx
                    hc.SetText(this, arr[n, 1], val[1].ToString());
                    hc.SetText(this, arr[n, 2], (val[0] / 20).ToString());   // On screen tbx
                    hc.SetText(this, arr[n, 3], (val[1] / 20).ToString());
                }
                else
                {
                    val[0] = (int.Parse(arr[n,2].Text));
                    val[1] = (int.Parse(arr[n,3].Text));
                    hc.SetText(this, arr[n, 0], (val[0] * 20).ToString());   // On encoder tbx
                    hc.SetText(this, arr[n, 1], (val[1] * 20).ToString());
                    hc.SetText(this, arr[n, 2], val[0].ToString());          // On screen tbx
                    hc.SetText(this, arr[n, 3], val[1].ToString());
                }
                moveLoc((int.Parse(arr[n, 0].Text) / 20), (int.Parse(arr[n, 1].Text) / 20), arr[n, 4]);     // Encoder then using scale 1:20
            }
        }

        private void tbxEncScr_KeyDown(object sender, KeyEventArgs e)
        {
            changeCounter(sender, e);

            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3 } };
            int n = 0;
            int[] val = new int[2];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            string dtGoto = "X:" + arr[n,1].Text + ",Y:" + arr[n,2].Text;
            SendCallBack(_socketDict[arr[n,0].Text], dtGoto);
        }

        private void tbxGoto_KeyDown(object sender, KeyEventArgs e)
        {
            changeCounter(sender, e);
            var chkRobot = chkRobotCollect.Split(',');
            if ((e.KeyCode == Keys.Enter) && (!string.IsNullOrWhiteSpace(tbxGotoX.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoY.Text)))
                if (!string.IsNullOrEmpty(chkRobotCollect))
                    foreach (var dt in chkRobot)
                    {
                        dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3 } };
                        int n = 0;
                        int[] val = new int[2];
                        for (int i = 0; i < arr.GetLength(0); i++)
                             if (arr[i, 0].Text == dt)
                                    n = i;
                        new Thread(obj => GotoLoc(arr[n,0].Text, arr[n, 1], arr[n, 2], int.Parse(tbxGotoX.Text), int.Parse(tbxGotoY.Text), 20, 20)).Start();
                    }
        }

        void GotoLoc(string Robot, dynamic encXRobot, dynamic encYRobot, int endX, int endY, int shiftX, int shiftY)
        {
            try
            {
                int startX = int.Parse(encXRobot.Text), startY = int.Parse(encYRobot.Text);
                if (startX > endX)
                    shiftX *= -1;
                if (startY > endY)
                    shiftY *= -1;
                addCommand("@ " + socketToIP(_socketDict[Robot]) + " : " + ("X:" + endX + ",Y:" + endY));
                bool[] chk = { true, true };
                while (chk[0] |= chk[1])
                {
                    if (startX != endX)
                        startX += shiftX;   // Process
                    else
                        chk[0] = false;     // Done
                    if (startY != endY)
                        startY += shiftY;   // Process
                    else
                        chk[1] = false;     // Done

                    string dtGoto = "X:" + startX + ",Y:" + startY;
                    SendCallBack(_socketDict[Robot], dtGoto, "Goto");
                    Thread.Sleep(100);    // time per limit
                }
            }
            catch (Exception e)
            {
            }
        }

        void resetLocation()
        {
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1 }, { tbxEncXR2, tbxEncYR2 }, { tbxEncXR3, tbxEncYR3 }, { tbxScrXR1, tbxScrYR1 }, { tbxScrXR2, tbxScrYR2 }, { tbxScrXR3, tbxScrYR3 }, { tbxGotoX, tbxGotoY } };
            foreach (var i in arr)
                i.Text = "0";
        }

        void setFormation()
        {
            string formation = cbxFormation.SelectedItem.ToString();
            int[] shift = {20, 20};   // Distance(cm) per shift
            dynamic[,] arr = null;
            if (formation == "Stand By")
                arr = new dynamic[,] { { tbxEncXR1, tbxEncYR1, 0, 6000 }, { tbxEncXR2, tbxEncYR2, 0, 5120 }, { tbxEncXR3, tbxEncYR3, 0, 4380 } };
            else if (formation == "Kick Off")
                arr = new dynamic[,] { { tbxEncXR1, tbxEncYR1, 4300, 3000 }, { tbxEncXR2, tbxEncYR2, 3000, 4100 }, { tbxEncXR3, tbxEncYR3, 100, 3000 } };                //for (int i = 0; i < arr.GetLength(0); i++)

            new Thread(obj => GotoLoc("Robot1", arr[0, 0], arr[0, 1], arr[0, 2], arr[0, 3], shift[0], shift[1])).Start();
            new Thread(obj => GotoLoc("Robot2", arr[1, 0], arr[1, 1], arr[1, 2], arr[1, 3], shift[0], shift[1])).Start();
            new Thread(obj => GotoLoc("Robot3", arr[2, 0], arr[2, 1], arr[2, 2], arr[2, 3], shift[0], shift[1])).Start();
            //for (int i = 0; i < arr.GetLength(0); i++)
            //    new Thread(obj => GotoLoc(arr[i, 0], arr[i, 1], arr[i, 2], arr[i, 3], 1, 1)).Start();
        }


        //////////////////////////////////////////////////////////////      COMUNICATION       //////////////////////////////////////////////////////////////
        ///
        byte[] _buffer = new byte[1024];
        static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dictionary<string, Socket> _socketDict = new Dictionary<string, Socket>();
        List<string> _chkRobotCollect = new List<string>();
        internal int port, attempts = 0;
        internal string myIP, chkRobotCollect=string.Empty;

        string GetIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    myIP = IP.ToString();
            return myIP;
        }

        delegate void addCommandCallback(string text);

        private void addCommand(string text)
        {
            if (this.tbxStatus.InvokeRequired)
            {
                addCommandCallback d = new addCommandCallback(addCommand);
                this.Invoke(d, new object[] { text });
            }
            else
                this.tbxStatus.Text += text + Environment.NewLine;
        }
        string socketToIP(Socket socket)
        {
            var _temp = socket.RemoteEndPoint.ToString().Split(':');
            return _temp[0];
        }

        void SetupServer(dynamic port)
        {
            try
            {
                if ((!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                {
                    addCommand("# Setting up server...");
                    addCommand("# IP " + this.Text + "  : " + tbxIPBS.Text);
                    hc.SetText(this, lblConnectionBS, "Open");
                    _serverSocket.Bind(new IPEndPoint(IPAddress.Any, this.port = int.Parse(port)));
                    _serverSocket.Listen(1);
                    _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
            }
            catch(Exception e)
            {
                addCommand("# FAILED to open server connection \n\n" + e);
            }
        }

        void AcceptCallback(IAsyncResult AR)
        {
            try
            { 
                Socket socket = _serverSocket.EndAccept(AR);
                if (socket.Connected)
                {
                    _socketDict.Add(socket.RemoteEndPoint.ToString(), socket);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
                    _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                    addCommand("# Success Connected to: " + socketToIP(socket));
                    //MessageBox.Show(_toServerSocketDict.Keys.Where(item => item.StartsWith("192.168.1.107")).ElementAt(0))  
                }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to connected \n\n" + e);
            }
        }

        void ReceiveCallBack(IAsyncResult AR) /**/
        {
            try
            { 
                Socket socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                string text = Encoding.ASCII.GetString(dataBuf).Trim();            
                var _data = text.Split('|');
                addCommand("> " + socketToIP(socket) + " : " + _data[0]);

                string respone = ResponeCallback(_data[0], socket);
                if (!string.IsNullOrEmpty(respone))
                {
                    if (_data.Count() == 1)
                        SendCallBack(socket, respone);
                    else
                        sendByHostList(_data[1], respone);
                }
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
            }
            catch (Exception e)
            {
                addCommand("# FAILED to receive message \n\n" + e);
            }
        }

        void SendCallBack(Socket _dstSocket, string txtMessage)
        {
            try
            { 
                addCommand("@ " + socketToIP(_dstSocket) + " : " + txtMessage);
                byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
                _dstSocket.Send(buffer);
                _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket);
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
            }
        }
        
        void SendCallBack(Socket _dstSocket, string txtMessage, string Goto)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
                _dstSocket.Send(buffer);
                _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket);
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
            }
        }

        void sendByHostList(dynamic inputHostList, string txtMsg)
        {
            try
            {
                var hostList = inputHostList.Split(',');
                foreach (var _hostList in hostList)
                {
                    try
                    {
                        SendCallBack(_socketDict[_socketDict.Keys.Where(host => host.StartsWith(_hostList)).ElementAtOrDefault(0).ToString()], txtMsg);
                    }
                    catch (Exception)
                    {
                        continue;   // If host not found then Skip
                    }
                }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
                MessageBox.Show("host Not Found :<");
            }
        }
        
        string ResponeCallback(dynamic text, Socket socket)
        {
            string respone = string.Empty;
            if (Regex.IsMatch(text, @"X:[-]{0,1}[0-9]{1,4},Y:[-]{0,1}[0-9]{1,4}"))
            {
                // If message is data X & Y from encoder
                /// Scale is 1 : 20 
                string objName = null;
                int[] _posXY = new int[2];
                var posXY = text.Split(',');
                if (posXY.Length == 2) // If data receive only one X & Y
                {
                    _posXY[0] = int.Parse(posXY[0].Split(':')[1]);
                    _posXY[1] = int.Parse(posXY[1].Split(':')[1]);
                }
                else // If data receive multi X & Y (error problems)
                {
                    _posXY[0] = int.Parse(posXY[posXY.Length - 2].Split(':')[2]);
                    _posXY[1] = int.Parse(posXY[posXY.Length - 1].Split(':')[1]);
                }
                foreach (var _temp in _socketDict)
                    if (_temp.Value.RemoteEndPoint == socket.RemoteEndPoint)
                        objName = _temp.Key.ToString();

                dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1}, { lblRobot2, tbxEncXR2, tbxEncYR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3 } };
                int n = 0;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0].Text == objName)
                        n = i;
                hc.SetText(this, arr[n,1], _posXY[0].ToString());          // On encoder tbx
                hc.SetText(this, arr[n,2], _posXY[1].ToString());
            }
            else if (Regex.IsMatch(text, @"Robot[0-9]"))
            {
                // If will rename key in socket dictionary
                Socket temp = _socketDict[socket.RemoteEndPoint.ToString()];    // Backup
                _socketDict.Remove(socket.RemoteEndPoint.ToString());           // Remove with old key
                _socketDict.Add(text, temp);                                    // Add with new key
                dynamic[,] arr = { { lblRobot1.Text, lblConnectionR1, tbxIPR1, tbxPortR1 }, { lblRobot2.Text, lblConnectionR2, tbxIPR2, tbxPortR2 }, { lblRobot3.Text, lblConnectionR3, tbxIPR3, tbxPortR3 } };
                int n = 0;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0] == text)
                        n = i;
                hc.SetText(this, arr[n, 1], "Connected");
                hc.SetText(this, arr[n, 2], socketToIP(socket));
                hc.SetText(this, arr[n, 3], this.port.ToString());
            }
            else if ((_socketDict.ContainsKey("RefereeBox")) && (socket.RemoteEndPoint.ToString().Contains(_socketDict["RefereeBox"].RemoteEndPoint.ToString())))
            //else if (true)
            {
                // If socket is Referee Box socket                
                switch (text)       // Condition in General
                {
                /// 1. DEFAULT COMMANDS ///
                    case "S": //STOP
                        respone = "STOP";
                        //timer.Stop();
                        goto broadcast;
                    case "s": //START
                        hc.SetText(this, lblTimer, "00:00");
                        //timer.Start();
                        //timer.Enabled = true;
                        respone = "START";
                        goto broadcast;
                    case "W": //WELCOME (welcome message)
                        respone = "WELCOME";
                        goto broadcast;
                    case "Z": //RESET (Reset Game)
                        respone = "RESET";
                        goto broadcast;
                    case "U": //TESTMODE_ON (TestMode On)
                        respone = "TESTMODE_ON";
                        break;
                    case "u": //TESTMODE_OFF (TestMode Off)
                        respone = "TESTMODE_OFF";
                        break;                        

                /// 3. GAME FLOW COMMANDS ///
                    case "1": //FIRST_HALF
                        respone = "FIRST_HALF";
                        hc.SetText(this, lblHalf, "1");
                        goto broadcast;
                    case "2": //SECOND_HALF
                        respone = "SECOND_HALF";
                        hc.SetText(this, lblHalf, "2");
                        goto broadcast;
                    case "3": //FIRST_HALF_OVERTIME
                        respone = "FIRST_HALF_OVERTIME";
                        goto broadcast; ;
                    case "4": //SECOND_HALF_OVERTIME
                        respone = "SECOND_HALF_OVERTIME";
                        goto broadcast;;
                    case "h": //HALF_TIME
                        respone = "HALF_TIME";
                        goto broadcast;;
                    case "e": //END_GAME (ends 2nd part, may go into overtime)
                        respone = "END_GAME";
                        goto broadcast;;
                    case "z": //GAMEOVER (Game Over)
                        respone = "GAMEOVER";
                        goto broadcast;;
                    case "L": //PARKING
                        respone = "PARKING";
                        break;

                /// 6. OTHERS ///
                    case "get_time": //TIME NOW
                        respone = DateTime.Now.ToLongTimeString();
                        break;
                    default:
                        //addCommand("# Invalid Command :<");
                        break;
                }

                if (TeamSwitch.Value == true)   // Condition in CYAN Team
                {
                    switch (text)
                    {
                        /// 2. PENALTY COMMANDS ///
                        case "Y": //YELLOW_CARD_CYAN
                            respone = "YELLOW_CARD_CYAN";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            YCard1R1.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard1R2.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard1R3.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            goto broadcast;
                        case "R": //RED_CARD_CYAN
                            respone = "RED_CARD_CYAN";
                            setMatchInfo(new dynamic[] { lblRCard, lblFouls });
                            RCardR1.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR2.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR3.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            goto broadcast;
                        case "B": //DOUBLE_YELLOW_CYAN
                            respone = "DOUBLE_YELLOW_CYAN";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            YCard2R1.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard2R2.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard2R3.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            RCardR1.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR2.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR3.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "A": //GOAL_CYAN
                            respone = "GOAL_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalCyan });
                            break;
                        case "D": //SUBGOAL_CYAN
                            respone = "SUBGOAL_CYAN";
                            break;

                        /// 5. GAME FLOW COMMANDS ///
                        case "K": //KICKOFF_CYAN
                            respone = "KICKOFF_CYAN";
                            hc.SetText(this, lblTimer, "00:00");
                            break;
                        case "F": //FREEKICK_CYAN
                            respone = "FREEKICK_CYAN";
                            break;
                        case "G": //GOALKICK_CYAN
                            respone = "GOALKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            break;
                        case "T": //THROWN_CYAN
                            respone = "THROWN_CYAN";
                            break;
                        case "C": //CORNER_CYAN
                            respone = "CORNER_CYAN";
                            setMatchInfo(new dynamic[] { lblCorner });
                            break;
                    }
                }
                else if (TeamSwitch.Value == false)   // Condition in MAGENTA Team
                {
                    switch (text)
                    {
                        /// 2. PENALTY COMMANDS ///
                        case "y": //YELLOW_CARD_MAGENTA	
                            respone = "YELLOW_CARD_MAGENTA";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            YCard1R1.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard1R2.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard1R3.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            goto broadcast;
                        case "r": //RED_CARD_MAGENTA
                            respone = "RED_CARD_MAGENTA";
                            setMatchInfo(new dynamic[] { lblRCard, lblFouls });
                            RCardR1.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR2.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR3.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            goto broadcast;
                        case "b": //DOUBLE_YELLOW_MAGENTA
                            respone = "DOUBLE_YELLOW_MAGENTA";
                            setMatchInfo(new dynamic[] { lblYCard, lblRCard, lblFouls }); ;
                            YCard2R1.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard2R2.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            YCard2R3.BackgroundImage = Image.FromFile(@"images\YellowRedCardFill.png");
                            RCardR1.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR2.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            RCardR3.BackgroundImage = Image.FromFile(@"images\RedCardFill.png");
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "a": //GOAL_MAGENTA
                            respone = "GOAL_MAGENTA";
                            setMatchInfo(new dynamic[] { lblGoalMagenta });
                            break;
                        case "d": //SUBGOAL_MAGENTA
                            respone = "SUBGOAL_MAGENTA";
                            break;

                        /// 5. GAME FLOW COMMANDS ///
                        case "k": //KICKOFF_MAGENTA
                            respone = "KICKOFF_MAGENTA";
                            hc.SetText(this, lblTimer, "00:00");
                            break;
                        case "f": //FREEKICK_MAGENTA
                            respone = "FREEKICK_MAGENTA";
                            break;
                        case "g": //GOALKICK_MAGENTA
                            respone = "GOALKICK_MAGENTA";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            break;
                        case "t": //THROWN_MAGENTA
                            respone = "THROWN_MAGENTA";
                            break;
                        case "c": //CORNER_MAGENTA
                            respone = "CORNER_MAGENTA";
                            setMatchInfo(new dynamic[] { lblCorner });
                            break;
                    }
                }
            }
            else    // for all socket         
            {                       
                switch (text)
                {
                    /// OTHERS ///
                    case "get_time": //TIME NOW
                        respone = DateTime.Now.ToLongTimeString();
                        break;
                    default:
                        //addCommand("# Invalid Command :<");
                        break;
                }
            }
            goto end;

            broadcast:
            sendByHostList("Robot1,Robot2,Robot3", respone);
            respone = string.Empty;

            multicast:
            sendByHostList(chkRobotCollect, respone);
            respone = string.Empty;

            end:
            return respone;
        }

        void setMatchInfo(dynamic[] objs)
        {
            foreach(dynamic obj in objs)
                hc.SetText(this, obj, (int.Parse(obj.Text) + 1).ToString());
        }
        
        void reqConnect(dynamic ipDst, dynamic port, string keyName, dynamic connection)
        {
            try
            {
                attempts++;
                _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //_toServerSocket.Connect(IPAddress.Parse(ipDst = "169.254.162.201"), 100);
                _toServerSocket.Connect(IPAddress.Parse(ipDst), int.Parse(port));
                hc.SetText(this, tbxStatus, string.Empty);
                if (_toServerSocket.Connected)
                    addCommand("# Success Connecting to: " + ipDst);
                hc.SetText(this, connection, "Connected");
                SendCallBack(_toServerSocket, this.Text);
                _socketDict.Add(keyName, _toServerSocket);
                _toServerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _toServerSocket);
            }
            catch (SocketException)
            {
                hc.SetText(this, tbxStatus, string.Empty);
                addCommand("# IP This Device  : " + myIP);
                addCommand("# IP Destination  : " + ipDst);
                addCommand("# Connection attempts: " + attempts.ToString());
                hc.SetText(this, connection, "Disconnected");
            }
        }

        private void grpBaseStation_Click(object sender, EventArgs e)
        {
            if((lblConnectionBS.Text == "Close") && (!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                new Thread(obj => SetupServer(tbxPortBS.Text)).Start();
        }

        void sendFromTextBox()
        {
            var dataMessage = tbxMessage.Text.Trim().Split('|');
            if ((dataMessage.Count() == 1) && (!string.IsNullOrWhiteSpace(chkRobotCollect)))       //for to be Client
                sendByHostList(chkRobotCollect, dataMessage[0]);
            else if (dataMessage.Count() == 2)  //for to be Server
                sendByHostList(dataMessage[1], dataMessage[0]);
            else
                MessageBox.Show("Incorrect Format!");
            tbxMessage.ResetText();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbxMessage.Text))
                sendFromTextBox();
        }

        private void tbxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (!string.IsNullOrWhiteSpace(tbxMessage.Text)))
                sendFromTextBox();
        }
        
        private void Connection_byDistinct(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { grpBaseStation, lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS }, { grpRefereeBox, lblRefereeBox, lblConnectionRB, tbxIPRB, tbxPortRB }, { grpRobot1, lblRobot1, lblConnectionR1, tbxIPR1, tbxPortR1 }, { grpRobot2, lblRobot2, lblConnectionR2, tbxIPR2, tbxPortR2 }, { grpRobot3, lblRobot3, lblConnectionR3, tbxIPR3, tbxPortR3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            if ((arr[n,2].Text == "Disconnected") && (!String.IsNullOrWhiteSpace(arr[n, 3].Text)) && (!String.IsNullOrWhiteSpace(arr[n, 4].Text)))
                new Thread(objs => reqConnect(arr[n, 3].Text, arr[n, 4].Text, arr[n, 1].Text, arr[n, 2])).Start();
        }

        private void tbxStatus_TextChanged(object sender, EventArgs e)
        {
            tbxStatus.SelectionStart = tbxStatus.Text.Length;
            tbxStatus.ScrollToCaret();
        }

        private void Connection_keyEnter(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter)
                Connection_byDistinct(sender, e);
        }

        private void tbxIPBS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SetupServer(tbxPortBS.Text);
        }

        private void TeamSwitch_OnValueChange(object sender, EventArgs e)
        {
            if(TeamSwitch.Value == true)
                this.BackgroundImage = Image.FromFile(@"images\Background Cyan.jpg");       // Team CYAN
            else
                this.BackgroundImage = Image.FromFile(@"images\Background Magenta.jpg");    // Team MAGENTA
        }

        private void cbxFormation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFormation.SelectedIndex != -1)
                setFormation();
        }

        private void lblConnection_TextChanged(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr;
            if ((obj == lblConnectionBS.Name) ^ (obj == lblConnectionRB.Name))
                arr = new dynamic[,] { { lblConnectionBS, lblBaseStation }, { lblConnectionRB, lblRefereeBox } };
            else
                arr = new dynamic[,] { { lblConnectionR1, lblRobot1, chkR1, lblEncoderR1, lblScreenR1, tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1 }, { lblConnectionR2, lblRobot2, chkR2, lblEncoderR2, lblScreenR2, tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2 }, { lblConnectionR3, lblRobot3, chkR3, lblEncoderR3, lblScreenR3, tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 0].Name == obj)
                    n = i;
            if ((arr[n, 0].Text == "Connected") ^ (arr[n, 0].Text == "Open"))
                for (int i = 0; i < arr.GetLength(1); i++)
                    arr[n, i].Enabled = true;
            else
                for (int i = 0; i < arr.GetLength(1); i++)
                    arr[n, i].Enabled = false;
        }

        private void ChkRobot_OnChange(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { chkR1, lblRobot1 }, { chkR2, lblRobot2 }, { chkR3, lblRobot3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            if (arr[n, 0].Checked == true)
                _chkRobotCollect.Add(arr[n, 1].Text);
            else
                _chkRobotCollect.Remove(arr[n, 1].Text);
            _chkRobotCollect.Sort();
            if (_chkRobotCollect.Count == 0)
                chkRobotCollect = string.Empty;
            for (int i = 0; i < _chkRobotCollect.Count; i++)
            {
                if (i == 0)
                    chkRobotCollect = _chkRobotCollect.ElementAtOrDefault(i);
                else
                    chkRobotCollect += "," + _chkRobotCollect.ElementAtOrDefault(i);
            }
        }


        private void btnTO_Click(object sender, EventArgs e)
        {
            //var a = (_socketDict.ElementAtOrDefault(0).Key).ToString();
            //MessageBox.Show(a.ToString());
            lblTimer.Text = "00:00";
            timer.Start();
            //timer.Enabled = true;]
            //setFormation();
        }
    }
}
