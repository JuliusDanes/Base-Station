﻿using System;
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
using System.Diagnostics;

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

        System.Threading.Timer time, timer, chkConnection, chkAppResponding;
        Dictionary<string, System.Threading.Timer> timerDict = new Dictionary<string, System.Threading.Timer>();

        private void Form1_Load(object sender, EventArgs e)
        {
            addCommand("~ Welcome to Base Station ~");
            tbxIPBS.Text = GetMyIP() /*= "192.168.165.10"*/;
            tbxPortBS.Text = "8686";
            tbxIPRB.Text = "169.254.162.201";
            tbxPortRB.Text = "28097";
            tbxIPR1.Text /*= "169.254.162.201"*/ = GetMyIP();
            tbxPortR1.Text = tbxPortR2.Text = tbxPortR3.Text = "8686";

            resetText();
            time = new System.Threading.Timer(new TimerCallback(tickTime)); timer = new System.Threading.Timer(new TimerCallback(tickTimer)); chkConnection = new System.Threading.Timer(new TimerCallback(checkConnection));
            time.Change(1000, 1000); timer.Change(1000, 1000); chkConnection.Change(10, 10);
            //chkAppResponding = new System.Threading.Timer(new TimerCallback(checkAppResponding), null, 10, 10);

            notConnectionCollect.Add(lblConnectionBS);
            notConnectionCollect.Add(lblConnectionRB);
            notConnectionCollect.Add(lblConnectionR1);
            notConnectionCollect.Add(lblConnectionR2);
            notConnectionCollect.Add(lblConnectionR3);
        }

        void checkAppResponding(object state)
        {
            try {
                Process[] processes = Process.GetProcessesByName("RobotCS");
                foreach (var i in processes)
                    if (!i.Responding)      // When the application not responding
                        i.Kill(); }
            catch (Exception)
            { }
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
        Dictionary<string, Thread> gotoDict = new Dictionary<string, Thread>();

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
            string dtGoto =  "E"+ arr[n, 1].Text + "," + arr[n, 2].Text + "," + arr[n, 5].Text;
            SendCallBack(_socketDict[arr[n, 0].Text], dtGoto);
        }

        private void runGoto()
        {
            var chkRobot = chkRobotCollect.Split(',');
            if ((!string.IsNullOrWhiteSpace(tbxGotoX.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoY.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoAngle.Text)))
                if (!string.IsNullOrEmpty(chkRobotCollect))
                    foreach (var dt in chkRobot)
                    {
                        dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
                        int n = 0;
                        int[] val = new int[2];
                        for (int i = 0; i < arr.GetLength(0); i++)
                            if (arr[i, 0].Text == dt)
                                n = i;
                        threadGoto(arr[n, 0].Text, new Thread(obj => GotoLoc(arr[n, 0].Text, arr[n, 1], arr[n, 2], arr[n, 3], int.Parse(tbxGotoX.Text), int.Parse(tbxGotoY.Text), int.Parse(tbxGotoAngle.Text), 20, 20, 1)));
                    }
                else
                    MessageBox.Show("# Please Select/Checklist the Robot");
        }

        private void tbxGoto_KeyDown(object sender, KeyEventArgs e)
        {
            changeCounter(sender, e);
            if (e.KeyCode == Keys.Enter)
                runGoto();
        }

        private void lblDiv2_Click(object sender, EventArgs e)
        {
            runGoto();
        }

        void GotoLoc(string Robot, dynamic encXRobot, dynamic encYRobot, dynamic angleRobot, int endX, int endY, int endAngle, int shiftX, int shiftY, int shiftAngle)
        {
            try
            {
                int startX = int.Parse(encXRobot.Text), startY = int.Parse(encYRobot.Text), startAngle = int.Parse(angleRobot.Text);
                addCommand("@ " + socketToName(_socketDict[Robot]) +" : Goto >> "+ ("X:" + endX + " Y:" + endY + " ∠:" + endAngle + "°"));
                bool[] chk = { true, true, true };
                while (chk[0] |= chk[1] |= chk[2])
                {
                    if (startX > 12000)
                        startX = int.Parse(startX.ToString().Substring(0, 4));
                    if (startY > 9000)
                        startY = int.Parse(startY.ToString().Substring(0, 4));
                    if (startAngle > 360)
                        startAngle = int.Parse(startAngle.ToString().Substring(0, 2));

                    if ((startX > endX) && (shiftX > 0))
                        shiftX *= -1;
                    else if ((startX < endX) && (shiftX < 0))
                        shiftX *= -1;
                    if ((startY > endY) && (shiftY > 0))
                        shiftY *= -1;
                    else if ((startY < endY) && (shiftY < 0))
                        shiftY *= -1;
                    if ((startAngle > endAngle) && (shiftAngle > 0))
                        shiftAngle *= -1;
                    else if ((startAngle < endAngle) && (shiftAngle < 0))
                        shiftAngle *= -1;

                    if (startX != endX) {
                        if (Math.Abs(endX - startX) < Math.Abs(shiftX))     // Shift not corresponding
                            shiftX = (endX - startX);
                        startX += shiftX;   // On process
                    } else
                        chk[0] = false;     // Done
                    if (startY != endY) {
                        if (Math.Abs(endY - startY) < Math.Abs(shiftY))     // Shift not corresponding
                            shiftY = (endY - startY);
                        startY += shiftY;   // On process
                    } else
                        chk[1] = false;     // Done
                    if (startAngle != endAngle) {
                        if (Math.Abs(endAngle - startAngle) < Math.Abs(shiftAngle))     // Shift not corresponding
                            shiftAngle = (endAngle - startAngle);
                        startAngle += shiftAngle;   // On process
                    } else
                        chk[2] = false;     // Done

                    string dtGoto = startX + "," + startY + "," + startAngle;
                    SendCallBack(_socketDict[Robot], dtGoto, "Goto");
                    Thread.Sleep(100);    // time per limit (milisecond)
                }
            }
            catch (Exception)
            { }
        }

        void threadGoto(string keyName, Thread th)
        {
            if (gotoDict.ContainsKey(keyName)) {
                gotoDict[keyName].Abort();
                gotoDict.Remove(keyName); }
            gotoDict.Add(keyName, th);
            gotoDict[keyName].Start();
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
                arr = new dynamic[,] { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1, 0, 6000, 0 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2, 0, 5120, 0 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3, 0, 4380, 0 } };
            else if (formation == "Kick Off")
                arr = new dynamic[,] { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1, 4300, 3000, 0 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2, 3000, 4100, 0 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3, 100, 3000, 0 } };                //for (int i = 0; i < arr.GetLength(0); i++)

            threadGoto(arr[0, 0].Text, new Thread(obj => GotoLoc(arr[0, 0].Text, arr[0, 1], arr[0, 2], arr[0, 3], arr[0, 4], arr[0, 5], arr[0, 6], shift[0], shift[1], shift[2])));
            threadGoto(arr[1, 0].Text, new Thread(obj => GotoLoc(arr[1, 0].Text, arr[1, 1], arr[1, 2], arr[1, 3], arr[1, 4], arr[1, 5], arr[1, 6], shift[0], shift[1], shift[2])));
            threadGoto(arr[2, 0].Text, new Thread(obj => GotoLoc(arr[2, 0].Text, arr[2, 1], arr[2, 2], arr[2, 3], arr[2, 4], arr[2, 5], arr[2, 6], shift[0], shift[1], shift[2])));
            //for (int i = 0; i <2; i++)
            //    threadGoto(arr[i, 0].Text, new Thread(obj => GotoLoc(arr[i, 0].Text, arr[i, 1], arr[i, 2], arr[i, 3], arr[i, 4], arr[i, 5], arr[i, 6], shift[0], shift[1], shift[2])));
        }


        //////////////////////////////////////////////////////////////      COMUNICATION       //////////////////////////////////////////////////////////////
        ///
        byte[] _buffer = new byte[1024];
        static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dictionary<string, Socket> _socketDict = new Dictionary<string, Socket>();
        HashSet<dynamic> notConnectionCollect = new HashSet<dynamic>(), autoReconnectCollect = new HashSet<dynamic>();
        List<dynamic> _chkRobotCollect = new List<dynamic>();
        internal int port, attempts = 0, ctr = 0;
        internal string myIP, chkRobotCollect = string.Empty;

        string GetMyIP()
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

        void Reconnect(dynamic socket)
        {
            try
            {   // Force for reconnecting  
                dynamic[,] arr = { { lblBaseStation, lblConnectionBS }, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
                for (int i = 0; i < arr.GetLength(0); i++)
                    if ((((arr[i, 0].Text == socket) || (_socketDict.ContainsKey(arr[i, 0].Text)) && (_socketDict[arr[i, 0].Text].RemoteEndPoint == socket.RemoteEndPoint))))
                    {
                        notConnectionCollect.Add(arr[i, 1]);
                        autoReconnectCollect.Add(arr[i, 1]);
                    }
            }
            catch (Exception)
            { }
        }
        void checkConnection(object state)
        {
            Recheck:
            try
            {
                dynamic[,] arr = { { lblBaseStation, lblConnectionBS }, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
                for (int i = 0; i < arr.GetLength(0); i++)          // Check for Server and Client Connection
                    if ((((_socketDict.ContainsKey(arr[i, 0].Text)) && (!_socketDict[arr[i, 0].Text].Connected)) ^ ((arr[i, 1].Text.Equals("Open")) && (!_serverSocket.IsBound))))
                    {
                        notConnectionCollect.Add(arr[i, 1]);
                        autoReconnectCollect.Add(arr[i, 1]);
                    }
                foreach (dynamic j in notConnectionCollect)         // Auto Reconnecting
                    if (autoReconnectCollect.Contains(j))
                    {
                        if (j.Text == "Connected")
                            hc.SetText(this, j, "Disconnected");
                        else if (j.Text == "Open")
                            hc.SetText(this, j, "Close");
                        if (j.Text == "Disconnected")
                            Connection_byDistinct(j, EventArgs.Empty);
                        else if (j.Text == "Close")
                            grpBaseStation_Click(grpBaseStation, EventArgs.Empty);
                    }
            }
            catch (Exception)
            { goto Recheck; }
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
                if (_socketDict.ContainsKey(keyName))
                    return;
                _socketDict.Add(keyName, _toServerSocket);
                if (_toServerSocket.Connected)
                    addCommand("# Success Connecting to: " + ipDst + " (" + keyName + ") \t Port : " + port);
                hc.SetText(this, connection, "Connected");
                SendCallBack(_toServerSocket, this.Text);
                _toServerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _toServerSocket);
            }
            catch (SocketException)
            {
                hc.SetText(this, tbxStatus, string.Empty);
                addCommand("# IP This Device  : " + myIP + " (" + this.Text + ")");
                addCommand("# IP Destination  : " + ipDst + " (" + keyName + ") \t Port : " + port);
                addCommand("# Connection attempts: " + attempts.ToString());
                Reconnect(_toServerSocket);
                notConnectionCollect.Add(connection);
                autoReconnectCollect.Add(connection);
            }
        }

        void SetupServer(dynamic port)
        {
            try
            {
                if ((!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                {
                    addCommand("# Setting up server...");
                    _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _serverSocket.Bind(new IPEndPoint(IPAddress.Any, (this.port = int.Parse(port))));
                    _serverSocket.Listen(1);
                    _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                    addCommand("# Open for IP : " + tbxIPBS.Text + " (" + this.Text + ") \t Port : " + port);
                    hc.SetText(this, lblConnectionBS, "Open");
                }
            }
            catch (Exception e)
            {
                hc.SetText(this, tbxStatus, string.Empty);
                addCommand("# FAILED to open server connection \n\n" + e);
                Reconnect(lblBaseStation.Text);
                notConnectionCollect.Add(lblConnectionBS);
                autoReconnectCollect.Add(lblConnectionBS);
            }
        }

        void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = null;
            try
            {
                socket = _serverSocket.EndAccept(AR);
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
                //addCommand("# FAILED to connect \n\n" + e);
                Reconnect(socket);
            }
        }

        void ReceiveCallBack(IAsyncResult AR) /**/
        {
            Socket socket = null;
            try
            {
                socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                string message = Encoding.ASCII.GetString(dataBuf).Trim();
                message = new string(message.Where(c => !char.IsControl(c)).ToArray());
                if (string.IsNullOrWhiteSpace(message))
                    socket.Disconnect(true);
                ResponeReceivedCallback(message, socket);

                //var _data = message.Split('|');

                //string respone = ResponeReceivedCallback(_data[0], socket);
                //if (!string.IsNullOrEmpty(respone))
                //{
                //    if (_data.Count() == 1)
                //        SendCallBack(socket, respone);
                //    else
                //        sendByHostList(_data[1], respone);
                //}


                //if (_dtMessage[0] != string.Empty)
                //    addCommand("> " + socketToName(socket) + " : " + _dtMessage[0]);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
            }
            catch (Exception e)
            {
                addCommand("# FAILED to receive message \n\n" + e);
                Reconnect(socket);
            }
        }

        void SendCallBack(Socket _dstSocket, string txtMessage)
        {
            try
            {
                if (Regex.IsMatch(txtMessage, "[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}"))
                {
                    //var pos = txtMessage.Split(',');
                    //addCommand("@ " + soc8\[;gkjketToName(_dstSocket) + " : " + ("X:" + pos[0] + " Y:" + pos[1] + " ∠:" + pos[2] + "°"));
                }
                else
                    addCommand("@ " + socketToName(_dstSocket) + " : " + txtMessage);
                txtMessage = new string(txtMessage.Where(c => !char.IsControl(c)).ToArray());
                byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
                _dstSocket.Send(buffer);
                _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket);
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
                Reconnect(_dstSocket);
            }
        }

        void SendCallBack(Socket _dstSocket, string txtMessage, string Goto)
        {
            try
            {
                txtMessage = new string(txtMessage.Trim().Where(c => !char.IsControl(c)).ToArray());
                if (!string.IsNullOrWhiteSpace(txtMessage)) {
                    byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
                    _dstSocket.Send(buffer);
                    _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket); }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
                Reconnect(_dstSocket);
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
                //MessageBox.Show("host Not Found :<");
            }
        }
        string ResponeSendCallback(string message)
        {
            string respone = string.Empty, text = string.Empty;
            var _dtMessage = message.Split('|');

            if ((_dtMessage[0].StartsWith("!")) && (_dtMessage[0].Length > 1)) {    // Broadcast message
                _dtMessage[0] = _dtMessage[0].Substring(1);
                _dtMessage[1] = "Robot1,Robot2,Robot3"; }
            if ((_dtMessage[0].StartsWith("**")) && (_dtMessage[0].Length > 2)) {    // Forward & Broadcast message
                respone = _dtMessage[0];
                goto broadcast; }
            else if ((_dtMessage[0].StartsWith("*")) && (_dtMessage[0].Length > 1)) {    // Forward & Multicast message
                goto multicast; }

            if (_dtMessage[0].ToLower() == "myip") {
                text = "MyIP: " + GetMyIP();
                if (_dtMessage.Count() > 1)
                    respone = GetMyIP();
                goto multicast; }
            else if ((!string.IsNullOrWhiteSpace(_dtMessage[0])) && (((_dtMessage.Count() == 2) && (!string.IsNullOrWhiteSpace(_dtMessage[1]))) || (!string.IsNullOrWhiteSpace(chkRobotCollect)) )) {
                // If to send Robot socket   
                switch (_dtMessage[0]) {
                    /// INFORMATION ///
                    default:
                        //respone = text = "# Invalid Command :<";
                        goto multicast; } }
            else
                MessageBox.Show("Incorrect Format!");
            goto end;

        broadcast:
            sendByHostList("Robot1,Robot2,Robot3", respone);
            goto end;

        multicast:
            if (!string.IsNullOrWhiteSpace(respone))
                respone = _dtMessage[0];
            if (_dtMessage.Count() > 1)
                sendByHostList(_dtMessage[1], respone);
            else if (!string.IsNullOrWhiteSpace(chkRobotCollect))
                sendByHostList(chkRobotCollect, respone);
            goto end;

        end:
            if (!string.IsNullOrWhiteSpace(text))
                addCommand("# " + text);
            return respone;
        }

        string ResponeReceivedCallback(string message, Socket socket)
        {
            string respone = string.Empty, text = string.Empty;
            var _dtMessage = message.Split('|');
            
            if ((_dtMessage[0].StartsWith("!")) && (_dtMessage[0].Length > 1)) {    // Broadcast message
                _dtMessage[0] = _dtMessage[0].Substring(1);
                _dtMessage[1] = "Robot1,Robot2,Robot3"; }
            if ((_dtMessage[0].StartsWith("**")) && (_dtMessage[0].Length > 2)) {    // Forward & Broadcast message
                respone = _dtMessage[0].Substring(2);
                goto broadcast; }
            else if ((_dtMessage[0].StartsWith("*")) && (_dtMessage[0].Length > 1)) {    // Forward & Multicast message
                respone = _dtMessage[0].Substring(1);
                goto multicast; }

            if (Regex.IsMatch(_dtMessage[0], "[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}"))
            {
                // If _dtMessage[0] is data X & Y from encoder
                /// Scale is 1 : 20 
                string objName = null;
                dynamic[] posXYZ = _dtMessage[0].Split(',');
                posXYZ = posXYZ.Where(item => (!string.IsNullOrWhiteSpace(item))).ToArray();
                if (posXYZ.Length > 3) // If data receive multi value X & Y (error bug problem)
                {
                    posXYZ[0] = posXYZ[posXYZ.Length - 3].Substring(posXYZ[posXYZ.Length - 1].Length);
                    posXYZ[1] = posXYZ[posXYZ.Length - 2];
                    posXYZ[2] = posXYZ[posXYZ.Length - 1];
                }

                if ((!string.IsNullOrWhiteSpace(posXYZ[0])) && (Convert.ToInt64(posXYZ[0]) > 12000))
                    posXYZ[0] = posXYZ[0].ToString().Substring(0, 4);
                if ((!string.IsNullOrWhiteSpace(posXYZ[1])) && (Convert.ToInt64(posXYZ[1]) > 9000))
                    posXYZ[1] = posXYZ[1].ToString().Substring(0, 4);
                if ((!string.IsNullOrWhiteSpace(posXYZ[2])) && (Convert.ToInt64(posXYZ[2]) > 360))
                    posXYZ[2] = posXYZ[2].ToString().Substring(0, 2);
                foreach (var _temp in _socketDict)                              // Get socket name from IP
                    if (_temp.Value.RemoteEndPoint == socket.RemoteEndPoint)
                        objName = _temp.Key.ToString();

                dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
                int n = 0;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0].Text == objName)
                        n = i;
                hc.SetText(this, arr[n, 1], posXYZ[0]);          // On encoder tbx
                hc.SetText(this, arr[n, 2], posXYZ[1]);
                hc.SetText(this, arr[n, 3], posXYZ[2]);
                //text = "X:" + posXYZ[0] + " Y:" + posXYZ[1] + " ∠:" + posXYZ[2] + "°";
            }
            else if (Regex.IsMatch(_dtMessage[0], "[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}"))
                text = string.Empty;
            else if (Regex.IsMatch(_dtMessage[0], @"Robot[0-9]"))
            {
                // If will rename key in socket dictionary
                Socket temp = _socketDict[socket.RemoteEndPoint.ToString()];    // Backup
                _socketDict.Remove(socket.RemoteEndPoint.ToString());           // Remove with old key
                _socketDict.Add(_dtMessage[0], temp);                                    // Add with new key
                dynamic[,] arr = { { lblRobot1.Text, lblConnectionR1, tbxIPR1, tbxPortR1 }, { lblRobot2.Text, lblConnectionR2, tbxIPR2, tbxPortR2 }, { lblRobot3.Text, lblConnectionR3, tbxIPR3, tbxPortR3 } };
                int n = 0;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0] == _dtMessage[0])
                        n = i;
                hc.SetText(this, arr[n, 1], "Connected");
                hc.SetText(this, arr[n, 2], socketToIP(socket));
                hc.SetText(this, arr[n, 3], this.port.ToString());
            }
            else if ((_socketDict.ContainsKey("RefereeBox")) && (socket.RemoteEndPoint.ToString().Contains(_socketDict["RefereeBox"].RemoteEndPoint.ToString())))
            //else if (true)
            {
                // If socket is Referee Box socket 
                respone = _dtMessage[0]; //Forward the _dtMessage[0]
                switch (_dtMessage[0])       // Condition in General
                {
                    /// 1. DEFAULT COMMANDS ///
                    case "S": //STOP
                        text = "STOP";
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "s": //START
                        text = "START";
                        timer.Change(1000, 1000);
                        goto broadcast;
                    case "W": //WELCOME (welcome _dtMessage[0])
                        text = "WELCOME";
                        goto broadcast;
                    case "Z": //RESET (Reset Game)
                        text = "RESET";
                        goto broadcast;
                    case "U": //TESTMODE_ON (TestMode On)
                        text = "TESTMODE_ON";
                        goto broadcast;
                    case "u": //TESTMODE_OFF (TestMode Off)
                        text = "TESTMODE_OFF";
                        goto broadcast;

                    /// 3. GAME FLOW COMMANDS ///
                    case "1": //FIRST_HALF
                        text = "FIRST_HALF";
                        hc.SetText(this, lblHalf, "1");
                        hc.SetText(this, lblTimer, "00:00");
                        goto broadcast;
                    case "2": //SECOND_HALF
                        text = "SECOND_HALF";
                        hc.SetText(this, lblHalf, "2");
                        hc.SetText(this, lblTimer, "00:00");
                        goto broadcast;
                    case "3": //FIRST_HALF_OVERTIME
                        text = "FIRST_HALF_OVERTIME";
                        goto broadcast;
                    case "4": //SECOND_HALF_OVERTIME
                        text = "SECOND_HALF_OVERTIME";
                        goto broadcast;
                    case "h": //HALF_TIME
                        text = "HALF_TIME";
                        goto broadcast;
                    case "e": //END_GAME (ends 2nd part, may go into overtime)
                        text = "END_GAME";
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "z": //GAMEOVER (Game Over)
                        text = "GAMEOVER";
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "L": //PARKING
                        text = "PARKING";
                        goto broadcast;

                    /// 6. OTHERS ///
                    case "get_time": //TIME NOW
                        text = DateTime.Now.ToLongTimeString();
                        break;
                    default:
                        //addCommand("# Invalid Command :<");
                        break;
                }

                if (TeamSwitch.Value == true)   // Condition in CYAN Team
                {
                    switch (_dtMessage[0])
                    {
                        /// 2. PENALTY COMMANDS ///
                        case "Y": //YELLOW_CARD_CYAN
                            text = "YELLOW_CARD_CYAN";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard1R1, YCard1R2, YCard1R3 });
                            goto broadcast;
                        case "R": //RED_CARD_CYAN
                            text = "RED_CARD_CYAN";
                            setMatchInfo(new dynamic[] { lblRCard, lblFouls });
                            setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            goto broadcast;
                        case "B": //DOUBLE_YELLOW_CYAN
                            text = "DOUBLE_YELLOW_CYAN";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard2R1, YCard2R2, YCard2R3 });
                            //setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            setTimer("Robot1", 120);
                            setTimer("Robot2", 120);
                            setTimer("Robot3", 120);
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "A": //GOAL_CYAN
                            text = "GOAL_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalCyan });
                            goto broadcast;
                        case "D": //SUBGOAL_CYAN
                            text = "SUBGOAL_CYAN";
                            goto broadcast;

                        /// 5. GAME FLOW COMMANDS ///
                        case "K": //KICKOFF_CYAN
                            text = "KICKOFF_CYAN";
                            //cbxFormation.SelectedItem = "Kick Off";
                            goto broadcast;
                        case "F": //FREEKICK_CYAN
                            text = "FREEKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblFouls });
                            break;
                        case "G": //GOALKICK_CYAN
                            text = "GOALKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            goto broadcast;
                        case "T": //THROWN_CYAN
                            text = "THROWN_CYAN";
                            break;
                        case "C": //CORNER_CYAN
                            text = "CORNER_CYAN";
                            setMatchInfo(new dynamic[] { lblCorner });
                            goto broadcast;
                    }
                }
                else if (TeamSwitch.Value == false)   // Condition in MAGENTA Team
                {
                    switch (_dtMessage[0])
                    {
                        /// 2. PENALTY COMMANDS ///
                        case "y": //YELLOW_CARD_MAGENTA	
                            text = "YELLOW_CARD_MAGENTA";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard1R1, YCard1R2, YCard1R3 });
                            goto broadcast;
                        case "r": //RED_CARD_MAGENTA
                            text = "RED_CARD_MAGENTA";
                            setMatchInfo(new dynamic[] { lblRCard, lblFouls });
                            setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            goto broadcast;
                        case "b": //DOUBLE_YELLOW_MAGENTA
                            text = "DOUBLE_YELLOW_MAGENTA";
                            setMatchInfo(new dynamic[] { lblYCard, lblFouls });
                            setCard(@"images\YellowCardFill.png", new dynamic[] { YCard2R1, YCard2R2, YCard2R3 });
                            //setCard(@"images\RedCardFill.png", new dynamic[] { RCardR1, RCardR2, RCardR3 });
                            setTimer("Robot1", 120);
                            setTimer("Robot2", 120);
                            setTimer("Robot3", 120);
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "a": //GOAL_MAGENTA
                            text = "GOAL_MAGENTA";
                            setMatchInfo(new dynamic[] { lblGoalMagenta });
                            goto broadcast;
                        case "d": //SUBGOAL_MAGENTA
                            text = "SUBGOAL_MAGENTA";
                            goto broadcast;

                        /// 5. GAME FLOW COMMANDS ///
                        case "k": //KICKOFF_MAGENTA
                            text = "KICKOFF_MAGENTA";
                            //cbxFormation.SelectedItem = "Kick Off";
                            goto broadcast;
                        case "f": //FREEKICK_MAGENTA
                            text = "FREEKICK_MAGENTA";
                            setMatchInfo(new dynamic[] { lblFouls });
                            break;
                        case "g": //GOALKICK_MAGENTA
                            text = "GOALKICK_MAGENTA";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            goto broadcast;
                        case "t": //THROWN_MAGENTA
                            text = "THROWN_MAGENTA";
                            goto broadcast;
                        case "c": //CORNER_MAGENTA
                            text = "CORNER_MAGENTA";
                            setMatchInfo(new dynamic[] { lblCorner });
                            goto broadcast;
                    }
                }
            }
            else
            {
                // If socket is Robot socket   
                switch (_dtMessage[0])
                {
                    /// INFORMATION ///
                    case "B_": //Get the ball
                        respone = "B_" + socketToName(socket);
                        var obj = socketToName(socket);
                        dynamic[,] arr = { {lblRobot1, ballR1, picRobot1, "Robot 1 Attacker.png", "Robot 1 Attacker-Get Ball.png" }, { lblRobot2, ballR2, picRobot2, "Robot 2 Defence.png", "Robot 2 Defence-Get Ball.png" }, { lblRobot3, ballR3,picRobot3, "Robot 3 Kiper.png", "Robot 3 Kiper-Get Ball.png" } };
                        int[] val = new int[2];
                        for (int i = 0; i < arr.GetLength(0); i++) {
                            hc.SetVisible(this, arr[i, 1], false);
                            arr[i, 2].BackgroundImage = Image.FromFile(@"images\"+ arr[i, 3]); }
                        for (int i = 0; i < arr.GetLength(0); i++)
                            if (arr[i, 0].Text == obj) {
                                hc.SetVisible(this, arr[i, 1], true);
                                arr[i, 2].BackgroundImage = Image.FromFile(@"images\" + arr[i, 4]); }
                        goto broadcast;

                    /// OTHERS ///
                    case "ip": //TIME NOW
                        respone = GetMyIP();
                        goto multicast;
                    case "get_time": //TIME NOW
                        respone = DateTime.Now.ToLongTimeString();
                        goto multicast;
                    //default:
                        //respone = text = "# Invalid Command :<";
                }
            }
            //if ((string.IsNullOrWhiteSpace(respone)) && (_dtMessage.Count() > 1))
            //    sendByHostList(_dtMessage[1], _dtMessage[0]);
            goto end;

        broadcast:
            sendByHostList("Robot1,Robot2,Robot3", respone);
            goto end;

        multicast:
            if (_dtMessage.Count() > 1)
                sendByHostList(_dtMessage[1], respone);
            else if (!string.IsNullOrWhiteSpace(chkRobotCollect))
                sendByHostList(chkRobotCollect, respone);
            else
                SendCallBack(socket, respone);
            goto end;

        end:
            if (!string.IsNullOrWhiteSpace(text))
                addCommand("# " + text);
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
            try {
                var obj = ((dynamic)sender).Name;
                dynamic[,] arr = { { grpBaseStation, lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS }, { grpRefereeBox, lblRefereeBox, lblConnectionRB, tbxIPRB, tbxPortRB }, { grpRobot1, lblRobot1, lblConnectionR1, tbxIPR1, tbxPortR1 }, { grpRobot2, lblRobot2, lblConnectionR2, tbxIPR2, tbxPortR2 }, { grpRobot3, lblRobot3, lblConnectionR3, tbxIPR3, tbxPortR3 } };
                int n = 0;
                for (int i = 0; i < arr.GetLength(0); i++)
                    for (int j = 0; j < arr.GetLength(1); j++)
                        if (arr[i, j].Name == obj)
                            n = i;
                if ((notConnectionCollect.Remove(arr[n, 2])) && (arr[n, 2].Text == "Disconnected") && (!string.IsNullOrWhiteSpace(arr[n, 3].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 4].Text)))
                    new Thread(objs => requestConnect(arr[n, 3].Text, arr[n, 4].Text, arr[n, 1].Text, arr[n, 2])).Start();
            }
            catch (Exception)
            { }
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
            hc.SetText(this, tbxIPR2, autoReconnectCollect.Contains(arr[n, 1]).ToString());
            autoReconnectCollect.Remove(arr[n, 1]);
            if (arr[n,1].Text == "Connected") {
                _socketDict[arr[n,0].Text].Dispose();
                hc.SetText(this, arr[n,1], "Disconnected"); }
            else if (arr[n, 1].Text == "Open") {
                _serverSocket.Dispose();
                hc.SetText(this, arr[n, 1], "Close"); }
            }
            catch (Exception)
            { }
        }

        void sendFromTextBox()
        {
            //var dataMessage = tbxMessage.Text.Trim().Split('|');

            //if (!string.IsNullOrWhiteSpace(tbxMessage.Text.Trim()))
                ResponeSendCallback(tbxMessage.Text.Trim());

            //if ((!string.IsNullOrWhiteSpace(dataMessage[0])) && (((dataMessage.Count() == 2) && (!string.IsNullOrWhiteSpace(dataMessage[1]))) || (!string.IsNullOrWhiteSpace(chkRobotCollect)) || ((dataMessage[0].StartsWith("!")) || (dataMessage[0].StartsWith("**"))) ))
            //    ResponeSendCallback(tbxMessage.Text.Trim());
            //else if (dataMessage[0].ToLower() == "myip")
            //    ResponeSendCallback(tbxMessage.Text.Trim());

            //if ((dataMessage.Count() == 1) && (!string.IsNullOrWhiteSpace(dataMessage[0])) && (dataMessage[0].ToLower() == "myip"))
            //    MessageBox.Show("MyIP: " + GetMyIP());
            //else if ((dataMessage.Count() == 1) && (!string.IsNullOrWhiteSpace(dataMessage[0])) && (!string.IsNullOrWhiteSpace(chkRobotCollect)))       //for to be Client
            //    sendByHostList(chkRobotCollect, dataMessage[0].Trim());
            //else if ((dataMessage.Count() == 2) && (!string.IsNullOrWhiteSpace(dataMessage[0])) && (!string.IsNullOrWhiteSpace(dataMessage[1])))  //for to be Server
            //    sendByHostList(dataMessage[1].Trim(), dataMessage[0].Trim());
            //else
            //    MessageBox.Show("Incorrect Format!");
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
            //foreach (var i in notConnectionCollect)
            //    MessageBox.Show(((dynamic)i).Name.ToString());
            //int a = 95000;
            //if (a.ToString().Length > 4)
            //    a = int.Parse(a.ToString().Substring(1));
        }

        private void Trackbar_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            //int newWidth = 0;
            //int newHeight = 0;
            //angle = (int)(angle * 3.6);

            //Bitmap bmp = new Bitmap(img.Width, img.Height);

            //if (angle <= 90)
            //{
            //    newWidth = (int)(bmp.Width * Math.Cos(2 * Math.PI * angle / 360) + bmp.Height * Math.Sin(2 * Math.PI * angle / 360));
            //    newHeight = (int)(bmp.Height * Math.Cos(2 * Math.PI * angle / 360) + bmp.Width * Math.Sin(2 * Math.PI * angle / 360));
            //}
            //else if (angle > 90 && angle <= 180)
            //{
            //    newWidth = (int)(bmp.Width * -Math.Cos(2 * Math.PI * angle / 360) + bmp.Height * Math.Sin(2 * Math.PI * angle / 360));
            //    newHeight = (int)(bmp.Height * -Math.Cos(2 * Math.PI * angle / 360) + bmp.Width * Math.Sin(2 * Math.PI * angle / 360));
            //}
            //else if (angle > 180 && angle <= 270)
            //{
            //    newWidth = (int)(bmp.Width * -Math.Cos(2 * Math.PI * angle / 360) + bmp.Height * -Math.Sin(2 * Math.PI * angle / 360));
            //    newHeight = (int)(bmp.Height * -Math.Cos(2 * Math.PI * angle / 360) + bmp.Width * -Math.Sin(2 * Math.PI * angle / 360));
            //}
            //else if (angle > 270 && angle <= 360)
            //{
            //    newWidth = (int)(bmp.Width * Math.Cos(2 * Math.PI * angle / 360) + bmp.Height * -Math.Sin(2 * Math.PI * angle / 360));
            //    newHeight = (int)(bmp.Height * Math.Cos(2 * Math.PI * angle / 360) + bmp.Width * -Math.Sin(2 * Math.PI * angle / 360));
            //}

            //this.Text = angle.ToString() + "%%" + newWidth.ToString() + "%%" + newHeight.ToString();

            //Bitmap bit_map = new Bitmap(newWidth, newHeight);
            //Graphics gfx = Graphics.FromImage(bit_map);
            //gfx.TranslateTransform(newWidth / 2, newHeight / 2);
            //gfx.RotateTransform(angle);
            //gfx.TranslateTransform(-img.Width / 2, -img.Height / 2);
            //gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //gfx.DrawImage(img, 0, 0);
            //e.Graphics.TranslateTransform(this.Width / 2, this.Height / 2);
            //e.Graphics.DrawImage(bit_map, -bit_map.Width / 2, -bit_map.Height / 2);
        }
    }
}
