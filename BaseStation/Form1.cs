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
            setTransparent(picArena, new dynamic[] { picBall, picRobot1, picRobot2, picRobot3 });
            setTransparent(grpBaseStation, new dynamic[] { lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS, lblPipeBS });
            setTransparent(grpRefereeBox, new dynamic[] { lblRefereeBox, lblConnectionRB, tbxIPRB, tbxPortRB, lblPipeRB });
            setTransparent(grpRobot1, new dynamic[] { lblRobot1, lblConnectionR1, chkR1, ballR1, tbxIPR1, tbxPortR1, lblPipeR1, lblPipe2R1, lblEncoderR1, lblEncCommaR1, tbxEncXR1, tbxEncYR1, lblScreenR1, tbxScrXR1, tbxScrYR1, lblScrCommaR1, lblDegR1, tbxAngleR1, YCard1R1, YCard2R1, RCardR1, ProgressR1 });
            setTransparent(grpRobot2, new dynamic[] { lblRobot2, lblConnectionR2, chkR2, ballR2, tbxIPR2, tbxPortR2, lblPipeR2, lblPipe2R2, lblEncoderR2, lblEncCommaR2, tbxEncXR2, tbxEncYR2, lblScreenR2, tbxScrXR2, tbxScrYR2, lblScrCommaR2, lblDegR2, tbxAngleR2, YCard1R2, YCard2R2, RCardR2, ProgressR2 });
            setTransparent(grpRobot3, new dynamic[] { lblRobot3, lblConnectionR3, chkR3, ballR3, tbxIPR3, tbxPortR3, lblPipeR3, lblPipe2R3, lblEncoderR3, lblEncCommaR3, tbxEncXR3, tbxEncYR3, lblScreenR3, tbxScrXR3, tbxScrYR3, lblScrCommaR3, lblDegR3, tbxAngleR3, YCard1R3, YCard2R3, RCardR3, ProgressR3 });
            setTransparent(lblDiv, new dynamic[] { lblPenalty, lblYCard, lblRCard, lblFouls, lblCorner, lblGoalKick });
            setTransparent(ProgressR1, new dynamic[] { lblTimerR1 }); setTransparent(ProgressR2, new dynamic[] { lblTimerR2 }); setTransparent(ProgressR3, new dynamic[] { lblTimerR3 }); setTransparent(picTimer, new dynamic[] { ProgressTM });

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

        System.Threading.Timer time, timer, chkConnection;
        Dictionary<string, System.Threading.Timer> timerDict = new Dictionary<string, System.Threading.Timer>();

        private void Form1_Load(object sender, EventArgs e)
        {
            tbxIPBS.Text = GetIPAddress() /*= "192.168.165.10"*/;
            tbxPortBS.Text = "8686";
            tbxIPRB.Text = "169.254.162.201";
            tbxPortRB.Text = "28097";
            tbxIPR1.Text /*= "169.254.162.201"*/ = GetIPAddress();
            tbxPortR1.Text = tbxPortR2.Text = tbxPortR3.Text = "8686";

            resetText();
            time = new System.Threading.Timer(new TimerCallback(tickTime)); timer = new System.Threading.Timer(new TimerCallback(tickTimer)); chkConnection = new System.Threading.Timer(new TimerCallback(checkConnection));
            time.Change(1000, 1000); timer.Change(1000, 1000); chkConnection.Change(100, 100);
        }

        private void tickTime(object state)
        {
            try { 
            hc.SetText(this, lblTime, ("[  " + DateTime.Now.ToString("HH:mm:ss") + "  ]"));
            } catch (Exception) { }
        }

        private void tickTimer(object state)
        {
            try
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

                hc.SetValue(this, ProgressTM, (ProgressTM.Value - 1));
                if (ProgressTM.Value <= 0) {
                    hc.SetVisible(this, ProgressTM, false);
                    hc.SetVisible(this, picTimer, false);
                    ProgressTM.ProgressColor = Color.SeaGreen; }
                else if (ProgressTM.Value <= ProgressTM.MaxValue / 2)
                    ProgressTM.ProgressColor = Color.Goldenrod;
                else if (ProgressTM.Value <= 10)
                    ProgressTM.ProgressColor = Color.Firebrick;
                else if (ProgressTM.Value > ProgressTM.MaxValue / 2)
                    ProgressTM.ProgressColor = Color.SeaGreen;
            }
            catch (Exception)
            { }
        }

        void setTimer(string obj, int time)
        {
            dynamic[,] arr = { { "Robot1", ProgressR1, lblTimerR1 }, { "Robot2", ProgressR2, lblTimerR2 }, { "Robot3", ProgressR3, lblTimerR3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 0] == obj)
                    n = i;
            arr[n, 1].MaxValue = time; hc.SetValue(this, arr[n, 1], time);
            hc.SetText(this, arr[n, 2], arr[n, 1].MaxValue.ToString());
            hc.SetVisible(this, arr[n, 1], true); hc.SetVisible(this, arr[n, 2], true);
            timerDict.Add(obj, (new System.Threading.Timer(new TimerCallback(tickRobot), obj, 1000, 1000)));
        }

        void tickRobot(object state)
        {
            var obj = state;
            dynamic[,] arr =  { { "Robot1", ProgressR1, lblTimerR1, YCard1R1, YCard2R1 }, { "Robot2", ProgressR2, lblTimerR2, YCard1R2, YCard2R2 }, { "Robot3", ProgressR3, lblTimerR3, YCard1R3, YCard2R3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 0] == obj)
                    n = i;
            hc.SetValue(this, arr[n, 1], (arr[n, 1].Value - 1));
            hc.SetText(this, arr[n, 2], (int.Parse(arr[n, 2].Text) - 1).ToString());
            if (arr[n, 1].Value == 0)
            {
                hc.SetVisible(this, arr[n, 1], false);
                timerDict[arr[n, 0]].Change(Timeout.Infinite, Timeout.Infinite);
                timerDict.Remove(arr[n, 0]);
                arr[n, 1].ProgressColor = Color.SeaGreen;
                setCard(@"images\YellowCardNoFill.png", new dynamic[] { arr[n, 3], arr[n, 4] });
            }
            else if (arr[n, 1].Value <= arr[n, 1].MaxValue / 2)
                arr[n, 1].ProgressColor = Color.Goldenrod;
            else if (arr[n, 1].Value <= 10)
                arr[n, 1].ProgressColor = Color.Firebrick;
        }

        delegate void addCommandCallback(string text);

        private void addCommand(string text)
        {
            try
            {
                if (this.tbxStatus.InvokeRequired)
                {
                    addCommandCallback d = new addCommandCallback(addCommand);
                    this.Invoke(d, new object[] { text });
                }
                else
                    this.tbxStatus.Text += text + Environment.NewLine;
            }
            catch (Exception e)
            {
                addCommand("# Error set text tbxStatus \n\n" + e);
            }
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
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { tbxEncXR3, tbxEncYR3, tbxAngleR3 }, { tbxScrXR1, tbxScrYR1, tbxAngleR1 }, { tbxScrXR2, tbxScrYR2, tbxAngleR2 }, { tbxScrXR3, tbxScrYR3, tbxAngleR3 }, { tbxGotoX, tbxGotoY, tbxGotoAngle } };
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
            else if (e.KeyCode == Keys.PageUp)
                arr[n, 2].Text = (int.Parse(arr[n, 2].Text) + 1).ToString();
            else if (e.KeyCode == Keys.PageDown)
                arr[n, 2].Text = (int.Parse(arr[n, 2].Text) - 1).ToString();
        }

        void tbxXYChanged(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, tbxAngleR1, picRobot1 }, { tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, tbxAngleR2, picRobot2 }, { tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, tbxAngleR3, picRobot3 } };
            int n = 0;
            int[] val = new int[2];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            if ((!string.IsNullOrWhiteSpace(arr[n, 0].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 1].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 2].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 3].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 4].Text)))
            {
                if (obj.StartsWith("tbxEnc"))           // Obj is tbxEncoder & Encoder then using scale 1:20
                {
                    val[0] = (int.Parse(arr[n, 0].Text));
                    val[1] = (int.Parse(arr[n, 1].Text));
                    hc.SetText(this, arr[n, 0], val[0].ToString());          // On encoder tbx
                    hc.SetText(this, arr[n, 1], val[1].ToString());
                    hc.SetText(this, arr[n, 2], (val[0] / 20).ToString());   // On screen tbx
                    hc.SetText(this, arr[n, 3], (val[1] / 20).ToString());
                }
                else if (obj.StartsWith("tbxScr"))      // obj is tbxScreen
                {
                    val[0] = (int.Parse(arr[n, 2].Text));
                    val[1] = (int.Parse(arr[n, 3].Text));
                    hc.SetText(this, arr[n, 0], (val[0] * 20).ToString());   // On encoder tbx
                    hc.SetText(this, arr[n, 1], (val[1] * 20).ToString());
                    hc.SetText(this, arr[n, 2], val[0].ToString());          // On screen tbx
                    hc.SetText(this, arr[n, 3], val[1].ToString());
                }
                moveLoc((int.Parse(arr[n, 0].Text) / 20), (int.Parse(arr[n, 1].Text) / 20), arr[n, 5]);     // Encoder then using scale 1:20
            }
        }

        private void tbxEncScr_KeyDown(object sender, KeyEventArgs e)
        {
            changeCounter(sender, e);
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, tbxAngleR3 } };
            int n = 0;
            int[] val = new int[2];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;
            string dtGoto = arr[n, 1].Text + "," + arr[n, 2].Text + "," + arr[n, 5].Text;
            SendCallBack(_socketDict[arr[n, 0].Text], dtGoto);
        }

        private void tbxGoto_KeyDown(object sender, KeyEventArgs e)
        {
            changeCounter(sender, e);
            var chkRobot = chkRobotCollect.Split(',');
            if ((e.KeyCode == Keys.Enter) && (!string.IsNullOrWhiteSpace(tbxGotoX.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoY.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoAngle.Text)))
                if (!string.IsNullOrEmpty(chkRobotCollect))
                    foreach (var dt in chkRobot)
                    {
                        dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
                        int n = 0;
                        int[] val = new int[2];
                        for (int i = 0; i < arr.GetLength(0); i++)
                            if (arr[i, 0].Text == dt)
                                n = i;
                        new Thread(obj => GotoLoc(arr[n, 0].Text, arr[n, 1], arr[n, 2], arr[n, 3], int.Parse(tbxGotoX.Text), int.Parse(tbxGotoY.Text), int.Parse(tbxGotoAngle.Text), 20, 20, 1)).Start();
                    }
        }

        void GotoLoc(string Robot, dynamic encXRobot, dynamic encYRobot, dynamic angleRobot, int endX, int endY, int endAngle, int shiftX, int shiftY, int shiftAngle)
        {
            try
            {
                int startX = int.Parse(encXRobot.Text), startY = int.Parse(encYRobot.Text), startAngle = int.Parse(angleRobot.Text);
                if (startX > endX)
                    shiftX *= -1;
                if (startY > endY)
                    shiftY *= -1;
                if (startAngle > endAngle)
                    shiftAngle *= -1;
                addCommand("@ " + socketToName(_socketDict[Robot]) +" : Goto >> "+ ("X:" + endX + " Y:" + endY + " ∠:" + endAngle + "°"));
                bool[] chk = { true, true, true };
                while (chk[0] |= chk[1] |= chk[2])
                {
                    if (startX != endX)
                    {
                        if (Math.Abs(endX - startX) < Math.Abs(shiftX))     // Shift not corresponding
                            shiftX = (endX - startX);
                        startX += shiftX;   // On process
                    }
                    else
                        chk[0] = false;     // Done
                    if (startY != endY)
                    {
                        if (Math.Abs(endY - startY) < Math.Abs(shiftY))     // Shift not corresponding
                            shiftY = (endY - startY);
                        startY += shiftY;   // On process
                    }
                    else
                        chk[1] = false;     // Done
                    if (startAngle != endAngle)
                    {
                        if (Math.Abs(endAngle - startAngle) < Math.Abs(shiftAngle))     // Shift not corresponding
                            shiftAngle = (endAngle - startAngle);
                        startAngle += shiftAngle;   // On process
                    }
                    else
                        chk[2] = false;     // Done

                    string dtGoto = startX + "," + startY + "," + startAngle;
                    SendCallBack(_socketDict[Robot], dtGoto, "Goto");
                    Thread.Sleep(100);    // time per limit
                }
            }
            catch (Exception)
            { }
        }

        void resetText()
        {
            dynamic[] arr = { tbxEncXR1, tbxEncYR1, tbxEncXR2, tbxEncYR2, tbxEncXR3, tbxEncYR3, tbxScrXR1, tbxScrYR1, tbxScrXR2, tbxScrYR2, tbxScrXR3, tbxScrYR3, tbxGotoX, tbxGotoY, tbxAngleR1, tbxAngleR2, tbxAngleR3, tbxGotoAngle };
            foreach (var i in arr)
                i.Text = "0";
        }

        void setFormation()
        {
            string formation = cbxFormation.SelectedItem.ToString();
            int[] shift = { 20, 20, 1 };   // Distance(cm) per shift
            dynamic[,] arr = null;
            if (formation == "Stand By")
                arr = new dynamic[,] { { tbxEncXR1, tbxEncYR1, tbxAngleR1, 0, 6000, 0 }, { tbxEncXR2, tbxEncYR2, tbxAngleR2, 0, 5120, 0 }, { tbxEncXR3, tbxEncYR3, tbxAngleR3, 0, 4380, 0 } };
            else if (formation == "Kick Off")
                arr = new dynamic[,] { { tbxEncXR1, tbxEncYR1, tbxAngleR1, 4300, 3000, 0 }, { tbxEncXR2, tbxEncYR2, tbxAngleR2, 3000, 4100, 0 }, { tbxEncXR3, tbxEncYR3, tbxAngleR3, 100, 3000, 0 } };                //for (int i = 0; i < arr.GetLength(0); i++)

            new Thread(obj => GotoLoc("Robot1", arr[0, 0], arr[0, 1], arr[0, 2], arr[0, 3], arr[0, 4], arr[0, 5], shift[0], shift[1], shift[2])).Start();
            new Thread(obj => GotoLoc("Robot2", arr[1, 0], arr[1, 1], arr[1, 2], arr[1, 3], arr[1, 4], arr[1, 5], shift[0], shift[1], shift[2])).Start();
            new Thread(obj => GotoLoc("Robot3", arr[2, 0], arr[2, 1], arr[2, 2], arr[2, 3], arr[2, 4], arr[2, 5], shift[0], shift[1], shift[2])).Start();
            //for (int i = 0; i < arr.GetLength(0); i++)
            //    new Thread(obj => GotoLoc("Robot1", arr[i, 0], arr[i, 1], arr[i, 2], arr[i, 3], arr[i, 4], arr[i, 5], shift[0], shift[1], shift[2])).Start();
        }


        //////////////////////////////////////////////////////////////      COMUNICATION       //////////////////////////////////////////////////////////////
        ///
        byte[] _buffer = new byte[1024];
        static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dictionary<string, Socket> _socketDict = new Dictionary<string, Socket>();
        Dictionary<dynamic, bool> chkReconnect = new Dictionary<dynamic, bool>();
        List<dynamic> _chkRobotCollect = new List<dynamic>(), notConnectionCollect = new List<dynamic>();
        internal int port, attempts = 0, ctr = 0;
        internal string myIP, chkRobotCollect = string.Empty;

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

        void checkConnection(object state)
        {
            try
            {
                dynamic[,] arr = { { lblBaseStation, lblConnectionBS }, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
                for (int i = 0; i < arr.GetLength(0); i++)      // Check for Server and Client Connection
                    if ((((_socketDict.ContainsKey(arr[i, 0].Text)) && (!_socketDict[arr[i, 0].Text].Connected)) ^ ((arr[i, 1].Text.Equals("Open")) && (!_serverSocket.IsBound))) && (!notConnectionCollect.Contains(arr[i, 1])))
                    {
                        notConnectionCollect.Add(arr[i, 1]);
                        chkReconnect.Add(arr[i, 1].Name, true);
                    }
                    else if ((((_socketDict.ContainsKey(arr[i, 0].Text)) && (_socketDict[arr[i, 0].Text].Connected)) ^ ((arr[i, 1].Text.Equals("Open")) && (_serverSocket.IsBound))) && (notConnectionCollect.Contains(arr[i, 1])))
                        notConnectionCollect.Remove(arr[i, 1]);
                foreach (dynamic j in notConnectionCollect)          // Auto Reconnecting
                    if (chkReconnect[j.Name] == true)
                    {
                        if (j.Text == "Connected")
                            hc.SetText(this, j, "Disconnected");
                        else if (j.Text == "Open")
                            hc.SetText(this, j, "Close");
                        if (j.Text == "Disconnected") { 
                            Connection_byDistinct(j, EventArgs.Empty);
                            chkReconnect[j.Name] = false; }
                        else if (j.Text == "Close")
                            grpBaseStation_Click(grpBaseStation, EventArgs.Empty);
                    }
            }
            catch (Exception)
            { }
        }

        private void lblConnection_TextChanged(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr;
            if ((obj == lblConnectionBS.Name) ^ (obj == lblConnectionRB.Name))
                arr = new dynamic[,] { { lblConnectionBS, lblBaseStation }, { lblConnectionRB, lblRefereeBox } };
            else
                arr = new dynamic[,] { { lblConnectionR1, lblRobot1, chkR1, lblEncoderR1, lblScreenR1, tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, tbxAngleR1, lblDegR1 }, { lblConnectionR2, lblRobot2, chkR2, lblEncoderR2, lblScreenR2, tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, tbxAngleR2, lblDegR2 }, { lblConnectionR3, lblRobot3, chkR3, lblEncoderR3, lblScreenR3, tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, tbxAngleR3, lblDegR3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 0].Name == obj)
                    n = i;
            if ((arr[n, 0].Text == "Connected") ^ (arr[n, 0].Text == "Open"))
            {
                arr[n, 0].BackColor = Color.SeaGreen;
                for (int i = 0; i < arr.GetLength(1); i++)
                    arr[n, i].Enabled = true;
            }
            else
            {
                if (obj == lblConnectionBS.Name)
                    addCommand("\n# " + arr[n, 1].Text + " server is CLOSE :<");
                else
                    addCommand("\n# " + arr[n, 1].Text + " has DISCONNECTED :<");
                _socketDict.Remove(arr[n, 1].Text);
                arr[n, 0].BackColor = Color.Firebrick;
                for (int i = 0; i < arr.GetLength(1); i++)
                    if (i != 0)
                        arr[n, i].Enabled = false;
            }
        }

        string socketToIP(Socket socket)
        {
            return (socket.RemoteEndPoint.ToString().Split(':'))[0];
        }

        string socketToName(Socket socket)
        {
            dynamic[] arr = { "BaseStation", "RefereeBox", "Robot1", "Robot2", "Robot3" };
            for (int i = 0; i < arr.Length; i++)
                if ((_socketDict.ContainsKey(arr[i])) && (_socketDict[arr[i]].RemoteEndPoint == socket.RemoteEndPoint))
                    return arr[i];
            return socket.RemoteEndPoint.ToString();
        }

        void requestConnect(dynamic ipDst, dynamic port, string keyName, dynamic connection)
        {
            addCommand("# Connecting to " + ipDst + " (" + keyName + ") \t Port : " + port);
            try
            {
                attempts++;
                _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //_toServerSocket.Connect(IPAddress.Parse(ipDst = "169.254.162.201"), 100);
                _toServerSocket.Connect(IPAddress.Parse(ipDst), int.Parse(port));
                if (_toServerSocket.Connected)
                    addCommand("# Success Connecting to: " + ipDst + " (" + keyName + ") \t Port : " + port);
                _socketDict.Add(keyName, _toServerSocket);
                hc.SetText(this, connection, "Connected");
                SendCallBack(_toServerSocket, this.Text);
                _toServerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _toServerSocket);
                if (chkReconnect.ContainsKey(connection.Name))
                    chkReconnect.Remove(connection.Name);
            }
            catch (SocketException)
            {
                hc.SetText(this, tbxStatus, string.Empty);
                addCommand("# IP This Device  : " + myIP + " (" + this.Text + ")");
                addCommand("# IP Destination  : " + ipDst + " (" + keyName + ") \t Port : " + port);
                addCommand("# Connection attempts: " + attempts.ToString());
                hc.SetText(this, connection, "Disconnected");
                if (chkReconnect.ContainsKey(connection.Name))
                    chkReconnect[connection.Name] = true;
            }
        }

        void SetupServer(dynamic port)
        {
            try
            {
                if ((!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                {
                    _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    addCommand("# Setting up server...");
                    addCommand("# Open for IP : " + tbxIPBS.Text + " (" + this.Text + ") \t Port : " + port);
                    hc.SetText(this, lblConnectionBS, "Open");
                    _serverSocket.Bind(new IPEndPoint(IPAddress.Any, (this.port = int.Parse(port))));
                    _serverSocket.Listen(1);
                    _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to open server connection \n\n" + e);
                _serverSocket.Dispose();
                //hc.SetText(this, lblConnectionBS, "Close");
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
                addCommand("# FAILED to connect \n\n" + e);
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
                if (string.IsNullOrWhiteSpace(text))
                    socket.Disconnect(true);
                var _data = text.Split('|');

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
                if (Regex.IsMatch(txtMessage, "[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}"))
                {
                    var pos = txtMessage.Split(',');
                    addCommand("@ " + socketToName(_dstSocket) + " : " + ("X:" + pos[0] + " Y:" + pos[1] + " ∠:" + pos[2] + "°"));
                }
                else
                    addCommand("@ " + socketToName(_dstSocket) + " : " + txtMessage);
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
            if (Regex.IsMatch(text, "[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}"))
            {
                // If message is data X & Y from encoder
                /// Scale is 1 : 20 
                string objName = null;
                var posXYZ = text.Split(',');
                if (posXYZ.Length > 3) // If data receive multi value X & Y (error bug problem)
                {
                    posXYZ[0] = posXYZ[posXYZ.Length - 3];
                    posXYZ[1] = posXYZ[posXYZ.Length - 2];
                    posXYZ[2] = posXYZ[posXYZ.Length - 1];
                }
                text = string.Empty;
                foreach (var _temp in _socketDict)                              // Get socket name from IP
                    if (_temp.Value.RemoteEndPoint == socket.RemoteEndPoint)
                        objName = _temp.Key.ToString();

                dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
                int n = 0;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0].Text == objName)
                        n = i;
                hc.SetText(this, arr[n, 1], posXYZ[0].ToString());          // On encoder tbx
                hc.SetText(this, arr[n, 2], posXYZ[1].ToString());
                hc.SetText(this, arr[n, 3], posXYZ[2].ToString());
                //text = "X:" + posXY[0]+ " Y:" + posXY[1]+ " ∠:" + posXY[2] + "°";
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
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "s": //START
                        respone = "START";
                        timer.Change(1000, 1000);
                        goto broadcast;
                    case "W": //WELCOME (welcome message)
                        respone = "WELCOME";
                        goto broadcast;
                    case "Z": //RESET (Reset Game)
                        respone = "RESET";
                        goto broadcast;
                    case "U": //TESTMODE_ON (TestMode On)
                        respone = "TESTMODE_ON";
                        goto broadcast;
                    case "u": //TESTMODE_OFF (TestMode Off)
                        respone = "TESTMODE_OFF";
                        goto broadcast;

                    /// 3. GAME FLOW COMMANDS ///
                    case "1": //FIRST_HALF
                        respone = "FIRST_HALF";
                        hc.SetText(this, lblHalf, "1");
                        hc.SetText(this, lblTimer, "00:00");
                        goto broadcast;
                    case "2": //SECOND_HALF
                        respone = "SECOND_HALF";
                        hc.SetText(this, lblHalf, "2");
                        hc.SetText(this, lblTimer, "00:00");
                        goto broadcast;
                    case "3": //FIRST_HALF_OVERTIME
                        respone = "FIRST_HALF_OVERTIME";
                        goto broadcast;
                    case "4": //SECOND_HALF_OVERTIME
                        respone = "SECOND_HALF_OVERTIME";
                        goto broadcast;
                    case "h": //HALF_TIME
                        respone = "HALF_TIME";
                        goto broadcast;
                    case "e": //END_GAME (ends 2nd part, may go into overtime)
                        respone = "END_GAME";
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "z": //GAMEOVER (Game Over)
                        respone = "GAMEOVER";
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "L": //PARKING
                        respone = "PARKING";
                        goto broadcast;

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
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard1R1, YCard1R2, YCard1R3 });
                            goto broadcast;
                        case "R": //RED_CARD_CYAN
                            respone = "RED_CARD_CYAN";
                            setMatchInfo(new dynamic[] { lblRCard, lblFouls });
                            setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            goto broadcast;
                        case "B": //DOUBLE_YELLOW_CYAN
                            respone = "DOUBLE_YELLOW_CYAN";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard2R1, YCard2R2, YCard2R3 });
                            //setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            setTimer("Robot1", 120);
                            setTimer("Robot2", 120);
                            setTimer("Robot3", 120);
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "A": //GOAL_CYAN
                            respone = "GOAL_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalCyan });
                            goto broadcast;
                        case "D": //SUBGOAL_CYAN
                            respone = "SUBGOAL_CYAN";
                            goto broadcast;

                        /// 5. GAME FLOW COMMANDS ///
                        case "K": //KICKOFF_CYAN
                            respone = "KICKOFF_CYAN";
                            //cbxFormation.SelectedItem = "Kick Off";
                            goto broadcast;
                        case "F": //FREEKICK_CYAN
                            respone = "FREEKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblFouls });
                            break;
                        case "G": //GOALKICK_CYAN
                            respone = "GOALKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            goto broadcast;
                        case "T": //THROWN_CYAN
                            respone = "THROWN_CYAN";
                            break;
                        case "C": //CORNER_CYAN
                            respone = "CORNER_CYAN";
                            setMatchInfo(new dynamic[] { lblCorner });
                            goto broadcast;
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
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard1R1, YCard1R2, YCard1R3 });
                            goto broadcast;
                        case "r": //RED_CARD_MAGENTA
                            respone = "RED_CARD_MAGENTA";
                            setMatchInfo(new dynamic[] { lblRCard, lblFouls });
                            setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            goto broadcast;
                        case "b": //DOUBLE_YELLOW_MAGENTA
                            respone = "DOUBLE_YELLOW_MAGENTA";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard2R1, YCard2R2, YCard2R3 });
                            //setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            setTimer("Robot1", 120);
                            setTimer("Robot2", 120);
                            setTimer("Robot3", 120);
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "a": //GOAL_MAGENTA
                            respone = "GOAL_MAGENTA";
                            setMatchInfo(new dynamic[] { lblGoalMagenta });
                            goto broadcast;
                        case "d": //SUBGOAL_MAGENTA
                            respone = "SUBGOAL_MAGENTA";
                            goto broadcast;

                        /// 5. GAME FLOW COMMANDS ///
                        case "k": //KICKOFF_MAGENTA
                            respone = "KICKOFF_MAGENTA";
                            //cbxFormation.SelectedItem = "Kick Off";
                            goto broadcast;
                        case "f": //FREEKICK_MAGENTA
                            respone = "FREEKICK_MAGENTA";
                            setMatchInfo(new dynamic[] { lblFouls });
                            break;
                        case "g": //GOALKICK_MAGENTA
                            respone = "GOALKICK_MAGENTA";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            goto broadcast;
                        case "t": //THROWN_MAGENTA
                            respone = "THROWN_MAGENTA";
                            goto broadcast;
                        case "c": //CORNER_MAGENTA
                            respone = "CORNER_MAGENTA";
                            setMatchInfo(new dynamic[] { lblCorner });
                            goto broadcast;
                    }
                }
            }
            else
            {
                // If socket is Robot socket   
                switch (text)
                {
                    /// INFORMATION ///
                    case "B": //Get the ball
                        respone = "Ball on " + socketToName(socket);
                        var obj = socketToName(socket);
                        dynamic[,] arr = { {lblRobot1, ballR1 }, { lblRobot2, ballR2 }, { lblRobot3, ballR3 } };
                        int[] val = new int[2];
                        for (int i = 0; i < arr.GetLength(0); i++)
                            hc.SetVisible(this, arr[i, 1], false);
                        for (int i = 0; i < arr.GetLength(0); i++)
                            if (arr[i, 0].Text == obj)
                                hc.SetVisible(this, arr[i, 1], true);
                        goto broadcast;

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
            return string.Empty;

            multicast:
            sendByHostList(chkRobotCollect, respone);
            return string.Empty;

            end:
            if (text != string.Empty)
                addCommand("> " + socketToName(socket) + " : " + text);
            return respone;
        }

        void setMatchInfo(dynamic[] objs)
        {
            foreach (dynamic obj in objs)
                hc.SetText(this, obj, (int.Parse(obj.Text) + 1).ToString());
        }

        void setCard(string dir, dynamic[] obj)
        {
            foreach (var i in obj)
                i.BackgroundImage = Image.FromFile(dir);
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

        private void grpBaseStation_Click(object sender, EventArgs e)
        {
            if ((lblConnectionBS.Text == "Close") && (!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                new Thread(obj => SetupServer(tbxPortBS.Text)).Start();
        }

        private void tbxOpenBS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if ((lblConnectionBS.Text == "Close") && (!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                    new Thread(obj => SetupServer(tbxPortBS.Text)).Start();
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
            if ((arr[n, 2].Text == "Disconnected") && (!String.IsNullOrWhiteSpace(arr[n, 3].Text)) && (!String.IsNullOrWhiteSpace(arr[n, 4].Text)))
                new Thread(objs => requestConnect(arr[n, 3].Text, arr[n, 4].Text, arr[n, 1].Text, arr[n, 2])).Start();
        }

        private void Connection_keyEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Connection_byDistinct(sender, e);
        }

        private void Disconnect_byDistinct(object sender, EventArgs e)
        {
            try
            { 
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { lblBaseStation, lblConnectionBS}, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
            int n = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 1].Name == obj)
                    n = i;            
            if (chkReconnect.ContainsKey(arr[n, 1].Name))
                chkReconnect.Remove(arr[n, 1].Name);
            if (arr[n,1].Text == "Connected") {
                _socketDict[arr[n,0].Text].Dispose();
                hc.SetText(this, arr[n,1], "Disconnected"); }
            else if (arr[n, 1].Text == "Open") {
                _serverSocket.Dispose();
                hc.SetText(this, arr[n, 1], "Close"); }
            }
            catch
            { }
        }

        void sendFromTextBox()
        {
            var dataMessage = tbxMessage.Text.Trim().Split('|');
            if ((dataMessage.Count() == 1) && (!string.IsNullOrWhiteSpace(dataMessage[0])) && (!string.IsNullOrWhiteSpace(chkRobotCollect)))       //for to be Client
                sendByHostList(chkRobotCollect, dataMessage[0].Trim());
            else if ((dataMessage.Count() == 2) && (!string.IsNullOrWhiteSpace(dataMessage[0])) && (!string.IsNullOrWhiteSpace(dataMessage[1])))  //for to be Server
                sendByHostList(dataMessage[1].Trim(), dataMessage[0].Trim());
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

        private void tbxStatus_TextChanged(object sender, EventArgs e)
        {
            try
            {
                tbxStatus.SelectionStart = tbxStatus.Text.Length;
                tbxStatus.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show("# Error tbxStatus \n\n" + ex);
            }
        }

        private void lblTimer_TextChanged(object sender, EventArgs e)
        {
            if (lblTimer.Text == "00:00")
            {
                ProgressTM.MaxValue = 900;
                hc.SetValue(this, ProgressTM, 900);
                hc.SetVisible(this, ProgressTM, true);
                hc.SetVisible(this, picTimer, true);
            }
        }

        private void TeamSwitch_OnValueChange(object sender, EventArgs e)
        {
            if (TeamSwitch.Value == true)
                this.BackgroundImage = Image.FromFile(@"images\Background Cyan.jpg");       // Team CYAN
            else
                this.BackgroundImage = Image.FromFile(@"images\Background Magenta.jpg");    // Team MAGENTA
        }

        private void cbxFormation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFormation.SelectedIndex != -1)
                setFormation();
        }        

        private void btnTO_Click(object sender, EventArgs e)
        {
            //var a = (_socketDict.ElementAtOrDefault(0).Key).ToString();
            //MessageBox.Show(a.ToString());
            //lblTimer.Text = "00:00";
            //setFormation();
            //ProgressTM.MaxValue += 300;
            //ProgressTM.Value += 300;

            //Connection_byDistinct(lblRobot1, EventArgs.Empty);
            foreach (var i in notConnectionCollect)
                MessageBox.Show(((dynamic)i).Name.ToString());
        }
    }
}
