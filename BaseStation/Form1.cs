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

namespace BaseStation
{  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            setTransparent(Lap, Bola);
            setTransparent(Lap, Robot1);
            setTransparent(Lap, Robot2);
            setTransparent(Lap, Robot3);
            setTransparent(Lap, Point1);
        }

        HelperClass hc = new HelperClass();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();        

        private void Form1_Load(object sender, EventArgs e)
        {
            tbxIPBS.Text /*= GetIPAddress()*/ = "192.168.165.10";
            tbxPortBS.Text = "8686";
            tbxIPRefBox.Text = "169.254.162.201";
            tbxPortRefBox.Text = "28097";
            tbxIPRobot1.Text /*= "169.254.162.201"*/ = GetIPAddress();
            tbxPortRobot1.Text = "8686";

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string time = lblTimer.Text;
            var _time = time.Split(':');        // split minute and second
            int count = int.Parse(_time[1]);

            if ((count < 59) && (count < 9))
                lblTimer.Text = _time[0] + ":0" + (count + 1).ToString();
            else if ((count < 59) && (count >= 9))
                lblTimer.Text = _time[0] + ":" + (count + 1).ToString();
            else if (int.Parse(_time[0]) < 9)   // for every 60 seconds
                lblTimer.Text = ("0" + int.Parse(_time[0]) + 1).ToString() + ":" + "00";
            else                                // for every 60 seconds
                lblTimer.Text = (int.Parse(_time[0]) + 1).ToString() + ":" + "00";
        }


        //////////////////////////////////////////////////////////////      TRACK LOCACTION       //////////////////////////////////////////////////////////////
        ///
        void setTransparent(dynamic backImage, dynamic frontImage)
        {
            var pos = this.PointToScreen(frontImage.Location);
            pos = backImage.PointToClient(pos);
            frontImage.Parent = backImage;
            frontImage.Location = pos;
        }

        void moveLoc(int encodX, int encodY, dynamic robot)
        {
            Point point00Lap = new Point(26, 20);
            Point point00Robot = new Point(robot.Size.Width/2, robot.Size.Height/2);
            Point newLoc = new Point((point00Lap.X + encodX - point00Robot.X), (point00Lap.Y + encodY - point00Robot.Y));
            robot.Location = newLoc;
        }

        void changeCounter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                tbxX.Text = (int.Parse(tbxX.Text) + 1).ToString();
            else if (e.KeyCode == Keys.Left)
                tbxX.Text = (int.Parse(tbxX.Text) - 1).ToString();
            else if (e.KeyCode == Keys.Up)
                tbxY.Text = (int.Parse(tbxY.Text) - 1).ToString();
            else if (e.KeyCode == Keys.Down)
                tbxY.Text = (int.Parse(tbxY.Text) + 1).ToString();
        }

        void tbxXYChanged(object sender, EventArgs e)
        {
            moveLoc(int.Parse(tbxX.Text), int.Parse(tbxY.Text), Robot1);
        }


        //////////////////////////////////////////////////////////////      COMUNICATION       //////////////////////////////////////////////////////////////
        ///
        byte[] _buffer = new byte[1024];
        static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dictionary<string, Socket> _socketDict = new Dictionary<string, Socket>();
        internal int port, attempts = 0;
        internal string myIP/*, typeMessage*/;

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
            addCommand("# Setting up server...");
            addCommand("# IP "+ this.Text +"  : " + tbxIPBS.Text);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(port)));
            _serverSocket.Listen(1);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _socketDict.Add(socket.RemoteEndPoint.ToString(), socket);
            //if (socket.Connected)
            //            
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            addCommand("# Success Connected to: " + socketToIP(socket));
            //MessageBox.Show(_toServerSocketDict.Keys.Where(item => item.StartsWith("192.168.1.107")).ElementAt(0))  
        }

        void ReceiveCallBack(IAsyncResult AR) /**/
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
                    sendByIPList(_data[1], respone);
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
        }

        void sendByIPList(dynamic inputListIP, string txtMsg)
        {
            try
            {
                var listIP = inputListIP.Split(',');
                foreach (var _listIP in listIP)
                    SendCallBack(_socketDict[_socketDict.Keys.Where(IP => IP.StartsWith(_listIP)).ElementAtOrDefault(0).ToString()], txtMsg);
            }
            catch (Exception e)
            {
                MessageBox.Show("IP Not Found :<");
            }
        }
        
        void SendCallBack(Socket _dstSocket, string txtMessage)
        {
            //MessageBox.Show("wkkwkw 3 " + txtMessage + " --- " + typeMsg);
            addCommand("@ " + socketToIP(_dstSocket) + " : " + txtMessage);
            byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
            _dstSocket.Send(buffer);
            _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket);
        }

        string ResponeCallback(dynamic text, Socket socket)
        {
            string respone = string.Empty;
            if (Regex.IsMatch(text, @"X:[-]{0,1}[0-9]{1,4},Y:[-]{0,1}[0-9]{1,4}"))
            {
                // If message is data X & Y from encoder
                var posXY = text.Split(',');
                hc.SetText(this, tbxX, posXY[0].Split(':')[1]);
                hc.SetText(this, tbxY, posXY[1].Split(':')[1]);
            }
            else
            {  
                switch (text)
                {
                    /// 1. DEFAULT COMMANDS ///
                    case "S": //STOP
                        timer.Stop();
                        respone = "STOP";
                        break;
                    case "s": //START
                        timer.Start();
                        respone = "START";
                        break;
                    case "W": //WELCOME (welcome message)
                        respone = "WELCOME";
                        break;
                    case "Z": //RESET (Reset Game)
                        respone = "RESET";
                        break;
                    case "U": //TESTMODE_ON (TestMode On)
                        respone = "TESTMODE_ON";
                        break;
                    case "u": //TESTMODE_OFF (TestMode Off)
                        respone = "TESTMODE_OFF";
                        break;

                    /// 2. PENALTY COMMANDS ///
                    case "y": //YELLOW_CARD_MAGENTA	
                        respone = "YELLOW_CARD_MAGENTA";
                        break;
                    case "Y": //YELLOW_CARD_CYAN
                        respone = "YELLOW_CARD_CYAN";
                        break;
                    case "r": //RED_CARD_MAGENTA
                        respone = "RED_CARD_MAGENTA";
                        break;
                    case "R": //RED_CARD_CYAN
                        respone = "RED_CARD_CYAN";
                        break;
                    case "b": //DOUBLE_YELLOW_MAGENTA
                        respone = "DOUBLE_YELLOW_MAGENTA";
                        break;
                    case "B": //DOUBLE_YELLOW_CYAN
                        respone = "DOUBLE_YELLOW_CYAN";
                        break;

                    /// 3. GAME FLOW COMMANDS ///
                    case "1": //FIRST_HALF
                        respone = "FIRST_HALF";
                        break;
                    case "2": //SECOND_HALF
                        respone = "SECOND_HALF";
                        break;
                    case "3": //FIRST_HALF_OVERTIME
                        respone = "FIRST_HALF_OVERTIME";
                        break;
                    case "4": //SECOND_HALF_OVERTIME
                        respone = "SECOND_HALF_OVERTIME";
                        break;
                    case "h": //HALF_TIME
                        respone = "HALF_TIME";
                        break;
                    case "e": //END_GAME (ends 2nd part, may go into overtime)
                        respone = "END_GAME";
                        break;
                    case "z": //GAMEOVER (Game Over)
                        respone = "GAMEOVER";
                        break;
                    case "L": //PARKING
                        respone = "PARKING";
                        break;

                    /// 4. GOAL STATUS ///
                    case "a": //GOAL_MAGENTA
                        respone = "GOAL_MAGENTA";
                        break;
                    case "A": //GOAL_CYAN
                        respone = "GOAL_CYAN";
                        break;
                    case "d": //SUBGOAL_MAGENTA
                        respone = "SUBGOAL_MAGENTA";
                        break;
                    case "D": //SUBGOAL_CYAN
                        respone = "SUBGOAL_CYAN";
                        break;

                    /// 5. GAME FLOW COMMANDS ///
                    case "k": //KICKOFF_MAGENTA
                        respone = "KICKOFF_MAGENTA";
                        break;
                    case "K": //KICKOFF_CYAN
                        respone = "KICKOFF_CYAN";
                        break;
                    case "f": //FREEKICK_MAGENTA
                        respone = "FREEKICK_MAGENTA";
                        break;
                    case "F": //FREEKICK_CYAN
                        respone = "FREEKICK_CYAN";
                        break;
                    case "g": //KICK_MAGENTA
                        respone = "KICK_MAGENTA";
                        break;
                    case "G": //GOALKICK_CYAN
                        respone = "GOALKICK_CYAN";
                        break;
                    case "t": //THROWN_MAGENTA
                        respone = "THROWN_MAGENTA";
                        break;
                    case "T": //THROWN_CYAN
                        respone = "THROWN_CYAN";
                        break;
                    case "c": //CORNER_MAGENTA
                        respone = "CORNER_MAGENTA";
                        break;
                    case "C": //CORNER_CYAN
                        respone = "CORNER_CYAN";
                        break;

                    /// 6. OTHERS ///
                    case "get time": //TIME NOW
                        respone = DateTime.Now.ToLongTimeString();
                        break;
                    default:
                        //addCommand("# Invalid Command :<");
                        break;
                }
            }
            byte[] data = Encoding.ASCII.GetBytes(respone);
            return respone;
        }

        void reqConnect(string ipDst, dynamic port)
        {
            try
            {
                attempts++;
                //_toServerSocket.Connect(IPAddress.Parse(ipDst = "169.254.162.201"), 100);
                _toServerSocket.Connect(IPAddress.Parse(ipDst), int.Parse(port));
                tbxStatus.ResetText();
                if (_toServerSocket.Connected)
                    addCommand("# Success Connecting to: " + ipDst);
                _socketDict.Add("RefereeBox", _toServerSocket);
                _toServerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _toServerSocket);
            }
            catch (SocketException)
            {
                tbxStatus.ResetText();
                addCommand("# IP This Device  : " + myIP);
                addCommand("# IP Destination  : " + ipDst);
                addCommand("# Connection attempts: " + attempts.ToString());
            }
        }
        

        private void btnOpenServer_Click(object sender, EventArgs e)
        {
            SetupServer(tbxPortBS.Text);
        }

        void sendFromTextBox()
        {
            var dataMessage = tbxMessage.Text.Trim().Split('|');
            if (dataMessage.Count() == 1) //for to be Client
                SendCallBack(_toServerSocket, dataMessage[0]);
            else if (dataMessage.Count() == 2)  //for to be Server
                sendByIPList(dataMessage[1], dataMessage[0]);
            else
                MessageBox.Show("Incorrect Format!");
            tbxMessage.ResetText();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            sendFromTextBox();
        }

        private void tbxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                sendFromTextBox();
        }

        private void btnConnectRefBox_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPRefBox.Text, tbxPortRefBox.Text);
        }

        private void btnConnnectRobot1_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPRobot1.Text, tbxPortRobot1.Text);
        }

        private void btnConnnectRobot2_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPRobot2.Text, tbxPortRobot2.Text);
        }

        private void btnConnnectRobot3_Click(object sender, EventArgs e)
        {
            reqConnect(tbxPortRobot3.Text, tbxPortRobot3.Text);
        }

        private void tbxStatus_TextChanged(object sender, EventArgs e)
        {
            tbxStatus.SelectionStart = tbxStatus.Text.Length;
            tbxStatus.ScrollToCaret();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var a = (_socketDict.ElementAtOrDefault(0).Key).ToString();
            MessageBox.Show(a.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_toServerSocket.RemoteEndPoint.ToString());
        }
    }
}
