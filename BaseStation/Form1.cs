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

        private void Form1_Load(object sender, EventArgs e)
        {
            tbxIPBS.Text /*= GetIPAddress()*/ = "169.254.162.201";
            tbxPortBS.Text = "8686";
            tbxIPRobot1.Text /*= "169.254.162.201"*/ = GetIPAddress();
            tbxPortRobot1.Text = "8686";
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
        internal string myIP, typeMessage;

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
            string text = Encoding.ASCII.GetString(dataBuf);
            
            var _data = text.Split('_');
            //MessageBox.Show("wkwkwk 2 " + _data[2] + " -- " + _data[0]);
            addCommand(_data[0] + "_F=" + socketToIP(socket) + ":_" + _data[2]);

            if (_data[0].Equals("@"))
            {
                string respone = ResponeCallback(_data[2]);
                if (_data.Count() == 3)
                    SendCallBack(socket, typeMessage, respone);
                else
                    sendByIPList(_data[3], typeMessage, respone);
            }
            else if (_data[0].Equals(">"))
            {
                var posXY = _data[2].Split(':');
                if (posXY.Count() == 4)
                { 
                    // If message is position X & Y from encoder
                    hc.SetText(this, tbxX, posXY[1]);
                    hc.SetText(this, tbxY, posXY[3]);
                }
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
        }

        void sendByIPList(dynamic inputListIP, string typeMsg, string txtMsg)
        {
            var listIP = inputListIP.Split(',');
            foreach (var _listIP in listIP)
                SendCallBack(_socketDict[_socketDict.Keys.Where(IP => IP.StartsWith(_listIP)).ElementAtOrDefault(0).ToString()], typeMsg, txtMsg);
        }

        void SendCallBack(Socket _dstSocket, string typeMsg, string txtMessage)
        {
            //MessageBox.Show("wkkwkw 3 " + txtMessage + " --- " + typeMsg);
            addCommand(txtMessage = (typeMsg + "_T=" + socketToIP(_dstSocket) + ":_" + txtMessage));
            byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
            _dstSocket.Send(buffer);
            _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket);
        }
        
        string ResponeCallback(string text)
        {
            string respone = string.Empty;
            if (text.ToLower() == "get time")
                respone = DateTime.Now.ToLongTimeString();
            else if (text.ToUpper() == "K")
                respone = "ON / START";
            else
                respone = "Invalid Request"; ;
            byte[] data = Encoding.ASCII.GetBytes(respone);
            typeMessage = ">";
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

        private void btnSend_Click(object sender, EventArgs e)
        {
            var dataMessage = tbxMessage.Text.Trim().Split('_');
            typeMessage = dataMessage[0];
            if (dataMessage.Count() == 2) //for to be Client
                SendCallBack(_toServerSocket, typeMessage, dataMessage[1]);
            else if (dataMessage.Count() == 3)  //for to be Server
                sendByIPList(dataMessage[2], typeMessage, dataMessage[1]);
            else
                MessageBox.Show("Incorrect Format!");
            tbxMessage.ResetText();
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

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_toServerSocket.RemoteEndPoint.ToString());
        }
    }
}
