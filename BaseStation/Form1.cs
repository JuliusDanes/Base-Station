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
    public partial class Form1 /*: MaterialForm*/ : Form
    {
        public Form1()
        {
            InitializeComponent();
            setTransparent(Lap, new dynamic[] { PointBall, PointRobot1, PointRobot2, PointRobot3 });
            setTransparent(grpBaseStation, new dynamic[] { lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS, lblPipeBS });
            setTransparent(grpRefereeBox, new dynamic[] { lblRefereeBox, lblConnectionRB, tbxIPRB, tbxPortRB, lblPipeRB });
            setTransparent(grpRobot1, new dynamic[] { lblRobot1, lblConnectionR1, tbxIPR1, tbxPortR1, lblPipeR1, lblEncoderR1, lblEncCommaR1, tbxEncXR1, tbxEncYR1, lblScreenR1, tbxScrXR1, tbxScrYR1, lblScrCommaR1, YCard1R1, YCard2R1, RCardR1 });
            setTransparent(grpRobot2, new dynamic[] { lblRobot2, lblConnectionR2, tbxIPR2, tbxPortR2, lblPipeR2, lblEncoderR2, lblEncCommaR2, tbxEncXR2, tbxEncYR2, lblScreenR2, tbxScrXR2, tbxScrYR2, lblScrCommaR2, YCard1R2, YCard2R2, RCardR2 });
            setTransparent(grpRobot3, new dynamic[] { lblRobot3, lblConnectionR3, tbxIPR3, tbxPortR3, lblPipeR3, lblEncoderR3, lblEncCommaR3, tbxEncXR3, tbxEncYR3, lblScreenR3, tbxScrXR3, tbxScrYR3, lblScrCommaR3, YCard1R3, YCard2R3, RCardR3 });
            
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
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();        

        private void Form1_Load(object sender, EventArgs e)
        {
            tbxIPBS.Text /*= GetIPAddress()*/ = "192.168.165.10";
            tbxPortBS.Text = "8686";
            tbxIPRB.Text = "169.254.162.201";
            tbxPortRB.Text = "28097";
            tbxIPR1.Text /*= "169.254.162.201"*/ = GetIPAddress();
            tbxPortR1.Text = "8686";

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000;
            //timer.Start();
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
            Point point00Robot = new Point(robot.Size.Width/2, robot.Size.Height/2);
            Point newLoc = new Point((point00Lap.X + encodX - point00Robot.X), (point00Lap.Y + encodY - point00Robot.Y));
            hc.SetLocation(this, robot, newLoc);
        }

        void changeCounter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                tbxEncXR1.Text = (int.Parse(tbxEncXR1.Text) + 1).ToString();
            else if (e.KeyCode == Keys.Left)
                tbxEncXR1.Text = (int.Parse(tbxEncXR1.Text) - 1).ToString();
            else if (e.KeyCode == Keys.Up)
                tbxEncYR1.Text = (int.Parse(tbxEncYR1.Text) - 1).ToString();
            else if (e.KeyCode == Keys.Down)
                tbxEncYR1.Text = (int.Parse(tbxEncYR1.Text) + 1).ToString();
        }

        void tbxXYChanged(object sender, EventArgs e)
        {
            string dtEncoder = "X:" + tbxEncXR1.Text + ",Y:" + tbxEncYR1.Text;
            Thread th_Send = new Thread(obj => SendCallBack(_socketDict["Robot1"], dtEncoder));
            th_Send.Start();
        }


        //////////////////////////////////////////////////////////////      COMUNICATION       //////////////////////////////////////////////////////////////
        ///
        byte[] _buffer = new byte[1024];
        static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dictionary<string, Socket> _socketDict = new Dictionary<string, Socket>();
        internal int port, attempts = 0;
        internal string myIP;

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
            lblConnectionBS.Text = "Open";
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, this.port = int.Parse(port)));
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
                    sendByHostList(_data[1], respone);
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
        }

        void SendCallBack(Socket _dstSocket, string txtMessage)
        {
            //MessageBox.Show("wkkwkw 3 " + txtMessage + " --- " + typeMsg);
            addCommand("@ " + socketToIP(_dstSocket) + " : " + txtMessage);
            byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
            _dstSocket.Send(buffer);
            _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket);
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
                MessageBox.Show("host Not Found :<");
            }
        }
        
        string ResponeCallback(dynamic text, Socket socket)
        {
            string respone = string.Empty;
            string objName = null;
            int[] _posXY = new int[2];
            if (Regex.IsMatch(text, @"X:[-]{0,1}[0-9]{1,4},Y:[-]{0,1}[0-9]{1,4}"))
            {
                // If message is data X & Y from encoder
                /// Scale is 1 : 20 
                var posXY = text.Split(',');
                if (posXY.Length == 2) // If data receive only one X & Y
                {
                    _posXY[0] = int.Parse(posXY[0].Split(':')[1]) / 20;
                    _posXY[1] = int.Parse(posXY[1].Split(':')[1]) / 20;
                }
                else // If data receive multi X & Y (error problem)
                {
                    _posXY[0] = int.Parse(posXY[posXY.Length - 2].Split(':')[2]) / 20;
                    _posXY[1] = int.Parse(posXY[posXY.Length - 1].Split(':')[1]) / 20;
                }
                
                hc.SetText(this, tbxEncXR1, _posXY[0].ToString());
                hc.SetText(this, tbxEncYR1, _posXY[1].ToString());

                foreach (var _temp in _socketDict)
                    if (_temp.Value.RemoteEndPoint == socket.RemoteEndPoint)
                        objName = _temp.Key.ToString();
                if (objName == "Robot1")
                    moveLoc(_posXY[0], _posXY[1], PointRobot1);
                else if (objName == "Robot2")
                    moveLoc(_posXY[0], _posXY[1], PointRobot2);
                else if (objName == "Robot3")
                    moveLoc(_posXY[0], _posXY[1], PointRobot3);
            }
            else if (Regex.IsMatch(text, @"Robot[0-9]"))
            {
                // If will rename key in socket dictionary
                Socket temp = _socketDict[socket.RemoteEndPoint.ToString()];    // Backup
                _socketDict.Remove(socket.RemoteEndPoint.ToString());           // Remove with old key
                _socketDict.Add(text, temp);                                    // Add with new key
                Dictionary<Control, Control> ctrl = new Dictionary<Control, Control>();
                if (text == "Robot1")
                    ctrl.Add(tbxIPR1,tbxPortR1);
                else if (text == "Robot2")
                    ctrl.Add(tbxIPR2, tbxPortR2);
                else if (text == "Robot3")
                    ctrl.Add(tbxIPR3, tbxPortR3);
                
                hc.SetText(this, (ctrl.Keys.ElementAtOrDefault(0)), socketToIP(socket));
                hc.SetText(this, ctrl[ctrl.Keys.ElementAtOrDefault(0)], this.port.ToString());
            }
            else if ((_socketDict.ContainsKey("RefereeBox")) && (socket.RemoteEndPoint.ToString().Contains(_socketDict["RefereeBox"].RemoteEndPoint.ToString())))
            {
                // If socket is Referee Box socket
                switch (text)
                {
                /// 1. DEFAULT COMMANDS ///
                    case "S": //STOP
                        timer.Stop();
                        respone = "STOP";
                        goto broadcast;
                    case "s": //START
                        //timer.Start();
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
                        goto broadcast;
                    case "2": //SECOND_HALF
                        respone = "SECOND_HALF";
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
            goto end;

            broadcast:
            sendByHostList("Robot1,Robot2,Robot3", respone);
            respone = string.Empty;

            multicast:
            sendByHostList("Robot2,Robot3", respone);
            respone = string.Empty;

            end:
            return respone;
        }
        
        void reqConnect(dynamic ipDst, dynamic port, string keyName, dynamic connection)
        {
            if ((!string.IsNullOrWhiteSpace(ipDst)) && (!string.IsNullOrWhiteSpace(port)))
            {
                try
                {
                    attempts++;
                    _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //_toServerSocket.Connect(IPAddress.Parse(ipDst = "169.254.162.201"), 100);
                    _toServerSocket.Connect(IPAddress.Parse(ipDst), int.Parse(port));
                    tbxStatus.ResetText();
                    if (_toServerSocket.Connected)
                        addCommand("# Success Connecting to: " + ipDst);
                    connection.Text = "Connected";
                    SendCallBack(_toServerSocket, this.Text);
                    _socketDict.Add(keyName.ToString(), _toServerSocket);
                    _toServerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _toServerSocket);
                }
                catch (SocketException)
                {
                    tbxStatus.ResetText();
                    addCommand("# IP This Device  : " + myIP);
                    addCommand("# IP Destination  : " + ipDst);
                    addCommand("# Connection attempts: " + attempts.ToString());
                    connection.Text = "Disconnected";
                }
            }
        }

        private void grpBaseStation_Click(object sender, EventArgs e)
        {
            SetupServer(tbxPortBS.Text);
        }

        private void btnOpenServer_Click(object sender, EventArgs e)
        {
            //SetupServer(tbxPortBSa.Text);
        }

        void sendFromTextBox()
        {
            var dataMessage = tbxMessage.Text.Trim().Split('|');
            if (dataMessage.Count() == 1) //for to be Client
                SendCallBack(_toServerSocket, dataMessage[0]);
            else if (dataMessage.Count() == 2)  //for to be Server
                sendByHostList(dataMessage[1], dataMessage[0]);
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

        ///
        private void grpRefereeBox_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPRB.Text, tbxPortRB.Text, "RefereeBox", lblConnectionRB);
        }

        private void grpRobot1_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPR1.Text, tbxPortR1.Text, "Robot1", lblConnectionR1);
        }

        private void grpRobot2_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPR2.Text, tbxPortR2.Text, "Robot2", lblConnectionR2);
        }

        private void grpRobot3_Click(object sender, EventArgs e)
        {
            reqConnect(tbxIPR3.Text, tbxPortR3.Text, "Robot3", lblConnectionR3);
        }

        private void tbxStatus_TextChanged(object sender, EventArgs e)
        {
            tbxStatus.SelectionStart = tbxStatus.Text.Length;
            tbxStatus.ScrollToCaret();
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            //var a = (_socketDict.ElementAtOrDefault(0).Key).ToString();
            //MessageBox.Show(a.ToString());
            timer.Start();
        }

        private void tbxGoto_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbxGotoX.Text))
            {
                new Thread(obj => GotoLoc(tbxEncXR1.Text, tbxGotoX.Text, 1)).Start();
                new Thread(obj => GotoLoc(tbxEncYR1.Text, tbxGotoY.Text, 1)).Start();
            }
        }
        
        void GotoLoc(dynamic start, dynamic end, int anker)
        {
            for (int i = int.Parse(start); i < int.Parse(end); i += anker)
            {
                string dtGoto = "X:" + i + ",Y:" + tbxEncYR1.Text;
                //new Thread(obj => SendCallBack(_socketDict["Robot1"], dtGoto)).Start();
                moveLoc(i, int.Parse(tbxEncYR1.Text), PointRobot1);
            }
        }

        private void Connection_keyEnter(object sender, KeyEventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS }, { lblBaseStation, lblConnectionRB, tbxIPRB, tbxPortRB }, { lblRobot1, lblConnectionR1, tbxIPR1, tbxPortR1 }, { lblRobot2, lblConnectionR2, tbxIPR2, tbxPortR2 }, { lblRobot3, lblConnectionR3, tbxIPR3, tbxPortR3 } };
            int n=0;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            if ((e.KeyCode == Keys.Enter) && (!String.IsNullOrWhiteSpace(arr[n,2].Text)) && (!String.IsNullOrWhiteSpace(arr[n, 3].Text)))
                reqConnect(arr[n, 2].Text, arr[n, 3].Text, arr[n, 0].Text, arr[n, 1]);
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_toServerSocket.RemoteEndPoint.ToString());
        }
    }
}
