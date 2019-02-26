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
            setTransparent(grpRobot1, new dynamic[] { lblRobot1, lblConnectionR1, chkR1, ballR1, tbxIPR1, tbxPortR1, lblPipeR1, lblPipe2R1, lblEncoderR1, lblEncCommaR1, tbxEncXR1, tbxEncYR1, lblScreenR1, tbxScrXR1, tbxScrYR1, lblScrCommaR1, lblDegR1, tbxAngleR1, lblSpeedR1, lblSpeedValR1, YCard1R1, YCard2R1, RCardR1, ProgressR1 });
            setTransparent(grpRobot2, new dynamic[] { lblRobot2, lblConnectionR2, chkR2, ballR2, tbxIPR2, tbxPortR2, lblPipeR2, lblPipe2R2, lblEncoderR2, lblEncCommaR2, tbxEncXR2, tbxEncYR2, lblScreenR2, tbxScrXR2, tbxScrYR2, lblScrCommaR2, lblDegR2, tbxAngleR2, lblSpeedR2, lblSpeedValR2, YCard1R2, YCard2R2, RCardR2, ProgressR2 });
            setTransparent(grpRobot3, new dynamic[] { lblRobot3, lblConnectionR3, chkR3, ballR3, tbxIPR3, tbxPortR3, lblPipeR3, lblPipe2R3, lblEncoderR3, lblEncCommaR3, tbxEncXR3, tbxEncYR3, lblScreenR3, tbxScrXR3, tbxScrYR3, lblScrCommaR3, lblDegR3, tbxAngleR3, lblSpeedR3, lblSpeedValR3, YCard1R3, YCard2R3, RCardR3, ProgressR3 });
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
        Dictionary<int, Thread> threadDict = new Dictionary<int, Thread>();
        Dictionary<string, System.Threading.Timer> timerDict = new Dictionary<string, System.Threading.Timer>(), notifDict = new Dictionary<string, System.Threading.Timer>();
        System.Threading.Timer time, timer, chkConnection, chkAppResponding, timerSpeed;
        int thID = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            addCommand("~ Welcome to Base Station ~");
            tbxIPBS.Text = GetMyIP() /*= "192.168.165.10"*/;
            tbxPortBS.Text = "8686";
            tbxIPRB.Text = "169.254.162.201";
            tbxPortRB.Text = "28097";
            tbxIPR1.Text = tbxIPR2.Text = tbxIPR3.Text = GetMyIP();
            tbxPortR1.Text = tbxPortR2.Text = tbxPortR3.Text = "8686";

            resetText();
            resetToogle();
            foreach (var picRobot in new dynamic[] { picRobot1, picRobot2, picRobot3 })
                picRobot.BackgroundImage = null;
            addFormation();
            LRSwitch_DoubleClick(LRSwitch, EventArgs.Empty);    // Set formation Stand By (Default)
            time = new System.Threading.Timer(new TimerCallback(tickTime), null, 1000, 1000); timer = new System.Threading.Timer(new TimerCallback(tickTimer), null, 1000, 1000); chkConnection = new System.Threading.Timer(new TimerCallback(checkConnection), null, 10, 10); timerSpeed = new System.Threading.Timer(new TimerCallback(tickSpeed), 500, 500, 500);
            //chkAppResponding = new System.Threading.Timer(new TimerCallback(checkAppResponding), null, 10, 10);
        }

        void swap(ref dynamic a, ref dynamic b)
        {
            dynamic _temp = a;
            a = b;
            b = _temp;
        }

        void swapObjText(ref dynamic a, ref dynamic b)
        {
            dynamic _temp = a.Text;
            a.Text = b.Text;
            b.Text = _temp;
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
                else if ((ProgressTM.Value <= ProgressTM.MaxValue / 2) && (ProgressTM.MaxValue > 10))
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
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (obj.StartsWith(arr[i, 0]))
                    n = i;

            if (n != -1) { 
                if (!timerDict.ContainsKey(obj)) { 
                    arr[n, 1].MaxValue = time; hc.SetValue(this, arr[n, 1], time);
                    hc.SetText(this, arr[n, 2], arr[n, 1].MaxValue.ToString());
                    hc.SetVisible(this, arr[n, 1], true); hc.SetVisible(this, arr[n, 2], true);
                    timerDict.Add(obj, (new System.Threading.Timer(new TimerCallback(tickRobot), obj, 1000, 1000))); }
            }
        }

        void tickRobot(object state)
        {
            var obj = state.ToString();
            dynamic[,] arr =  { { "Robot1", ProgressR1, lblTimerR1, YCard1R1, YCard2R1 }, { "Robot2", ProgressR2, lblTimerR2, YCard1R2, YCard2R2 }, { "Robot3", ProgressR3, lblTimerR3, YCard1R3, YCard2R3 } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (obj.StartsWith(arr[i, 0]))
                    n = i;

            if (n != -1) { 
                hc.SetValue(this, arr[n, 1], (arr[n, 1].Value - 1));
                hc.SetText(this, arr[n, 2], (int.Parse(arr[n, 2].Text) - 1).ToString());
                if (arr[n, 1].Value == 0)
                {
                    hc.SetVisible(this, arr[n, 1], false);
                    timerDict[obj].Change(Timeout.Infinite, Timeout.Infinite);
                    if (timerDict.ContainsKey(obj))
                        timerDict.Remove(obj);
                    arr[n, 1].ProgressColor = Color.SeaGreen;

                    if (obj.EndsWith("DYCard"))
                        setCard(@"images\YellowCardNoFill.png", new dynamic[] { arr[n, 3], arr[n, 4] });
                }
                else if ((arr[n, 1].Value <= arr[n, 1].MaxValue / 2) && (arr[n, 1].Value > 10))
                    arr[n, 1].ProgressColor = Color.Goldenrod;
                else if (arr[n, 1].Value <= 10)
                    arr[n, 1].ProgressColor = Color.Firebrick;
                else if (arr[n, 1].Value > arr[n, 1].MaxValue / 2)
                    arr[n, 1].ProgressColor = Color.SeaGreen; }
        }

        void tickSpeed(object state)
        {
            double time = Convert.ToDouble(state);
            dynamic[,] arr =  { { tbxEncXR1, tbxEncYR1, tbxAngleR1, lblSpeedValR1 }, { tbxEncXR2, tbxEncYR2, tbxAngleR2, lblSpeedValR2 }, { tbxEncXR3, tbxEncYR3, tbxAngleR3, lblSpeedValR3 } };
            for (int i = 0; i < arr.GetLength(0); i++) {
                double  Vx = Math.Abs(int.Parse(arr[i, 0].Text) - tempPosXYZ[i, 0]) / time, 
                        Vy = Math.Abs(int.Parse(arr[i, 1].Text) - tempPosXYZ[i, 1]) / time, 
                        V = (Math.Ceiling((Math.Sqrt(Math.Pow(Vx, 2) + Math.Pow(Vy, 2))) * 100) / 100);
                hc.SetText(this, arr[i, 3], (V.ToString() + "m/s"));
                for (int j = 0; j < 3; j++)
                    tempPosXYZ[i, j] = int.Parse(arr[i, j].Text); }
        }

        void notifOutside(object state)
        {
            if (_socketDict.ContainsKey(state.ToString()))
                SendCallBack(_socketDict[state.ToString()], "OS");      // Send notif Outside to Robot
        }

        delegate void addCommandCallback(string text);

        private void addCommand(string text)
        {
            try {
                if (this.tbxStatus.InvokeRequired) {
                    addCommandCallback d = new addCommandCallback(addCommand);
                    this.Invoke(d, new object[] { text }); }
                else
                    this.tbxStatus.Text += text + Environment.NewLine; }
            catch (Exception e) {
                addCommand("# Error set text tbxStatus \n\n" + e); }
        }


        //////////////////////////////////////////////////////////////      TRACK LOCACTION       //////////////////////////////////////////////////////////////
        ///
        int scale = 20;
        int[,] tempPosXYZ = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        int[] shift = { 20, 20, 1 };
        Dictionary<string, Thread> gotoDict = new Dictionary<string, Thread>();
        Image[] imgRobot = { Image.FromFile("images/Robot 1 Attacker.png"), Image.FromFile("images/Robot 2 Defence.png"), Image.FromFile("images/Robot 3 Kiper.png") };
        private readonly int[]  _Nol         = { 0, 0, 0,            0, 0, 0,            0, 0, 0 },
                                _StandBy     = { 0, 6000, 0,         0, 5120, 0,         0, 4380, 0 },
                                _KickOff     = { 4200, 3000, 0,      3000, 4100, 0,      100, 3000, 0 },
                                _Penalty     = { 7400, 3000, 0 }, 
                                _CorrnerA    = { 9000, 0, 135 },
                                _CorrnerB    = { 9000, 6000, 225 },
                                _FreeKick    = { 0, 0, 0 };

        private void setTransparent(dynamic backImage, dynamic[] frontImages)
        {
            foreach (var frontImage in frontImages) {
                var pos = this.PointToScreen(frontImage.Location);
                pos = backImage.PointToClient(pos);
                frontImage.Parent = backImage;
                frontImage.Location = pos; }
        }

        private void setFormation()
        {
            string formation = cbxFormation.SelectedItem.ToString();
            int[] shift = { scale, scale, 1 };   // Distance(cm) per shift
            dynamic[,] arr = null;
            int x = 0, y = 1;

            if (TransposeSwitch.Value == true) {    // If condition Transpose is ON
                x = 1; y = 0; }

            if (formation == "Nol")
                arr = new dynamic[,] { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1, mirrorX(_Nol[0+x]), mirrorY(_Nol[0+y]), mirrorAngle(_Nol[2]) }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2, mirrorX(_Nol[3+x]), mirrorY(_Nol[3+y]), mirrorAngle(_Nol[5]) }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3, mirrorX(_Nol[6+x]), mirrorY(_Nol[6+y]), mirrorAngle(_Nol[8]) } };
            else if (formation == "Stand By")
                arr = new dynamic[,] { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1, mirrorX(_StandBy[0+x]), mirrorY(_StandBy[0+y]), mirrorAngle(_StandBy[2]) }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2, mirrorX(_StandBy[3+x]), mirrorY(_StandBy[3+y]), mirrorAngle(_StandBy[5]) }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3, mirrorX(_StandBy[6+x]), mirrorY(_StandBy[6+y]), mirrorAngle(_StandBy[8]) } };
            else if (formation == "Kick Off")
                arr = new dynamic[,] { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1, mirrorX(_KickOff[0+x]), mirrorY(_KickOff[0+y]), mirrorAngle(_KickOff[2]) }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2, mirrorX(_KickOff[3+x]), mirrorY(_KickOff[3+y]), mirrorAngle(_KickOff[5]) }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3, mirrorX(_KickOff[6+x]), mirrorY(_KickOff[6+y]), mirrorAngle(_KickOff[8]) } };
            else if (formation == "Penalty") {
                priorityRobot = "Robot1,Robot2,Robot3";
                if (priorityRobot != null)
                    arr = new dynamic[,] { { priorityRobot[0], priorityRobot[1], priorityRobot[2], priorityRobot[3], mirrorX(_Penalty[0+x]), mirrorY(_Penalty[0+y]), mirrorAngle(_Penalty[2]) } }; }
            else if (formation == "Corrner A") { 
                priorityRobot = "Robot2,Robot1,Robot3";
                if (priorityRobot != null)
                    arr = new dynamic[,] { { priorityRobot[0], priorityRobot[1], priorityRobot[2], priorityRobot[3], mirrorX(_CorrnerA[0+x]), mirrorY(_CorrnerA[0+y]), mirrorAngle(_CorrnerA[2]) } }; }
            else if (formation == "Corrner B") { 
                priorityRobot = "Robot2,Robot1,Robot3";
                if (priorityRobot != null)
                    arr = new dynamic[,] { { priorityRobot[0], priorityRobot[1], priorityRobot[2], priorityRobot[3], mirrorX(_CorrnerB[0+x]), mirrorY(_CorrnerB[0+y]), mirrorAngle(_CorrnerB[2]) } }; }
            else if (formation == "Free Kick") {
                priorityRobot = "Robot1,Robot2,Robot3";
                if (priorityRobot != null)
                    arr = new dynamic[,] { { priorityRobot[0], priorityRobot[1], priorityRobot[2], priorityRobot[3], mirrorX(_FreeKick[0+x]), mirrorY(_FreeKick[0+y]), mirrorAngle(_FreeKick[2]) } }; }

            //threadGoto(arr[0, 0].Text, new Thread(obj => GotoLoc(arr[0, 0].Text, arr[0, 1], arr[0, 2], arr[0, 3], arr[0, 4], arr[0, 5], arr[0, 6], shift[0], shift[1], shift[2])));
            //threadGoto(arr[1, 0].Text, new Thread(obj => GotoLoc(arr[1, 0].Text, arr[1, 1], arr[1, 2], arr[1, 3], arr[1, 4], arr[1, 5], arr[1, 6], shift[0], shift[1], shift[2])));
            //threadGoto(arr[2, 0].Text, new Thread(obj => GotoLoc(arr[2, 0].Text, arr[2, 1], arr[2, 2], arr[2, 3], arr[2, 4], arr[2, 5], arr[2, 6], shift[0], shift[1], shift[2])));
            if (arr != null)
                for (int i = 0; i < arr.GetLength(0); i++) {
                    int j = i;
                    //threadGoto(arr[j, 0].Text, new Thread(obj => GotoLoc(arr[j, 0].Text, arr[j, 1], arr[j, 2], arr[j, 3], arr[j, 4], arr[j, 5], arr[j, 6], shift[0], shift[1], shift[2])));       ///For NOT connected Arduino
                    GotoLoc(arr[j, 0].Text, arr[j, 4], arr[j, 5], arr[j, 6]);   /*For connected Arduino*/ }
        }

        private dynamic[] _priorityRobot = null;

        internal dynamic priorityRobot
        {
            get {
                return _priorityRobot; }
            set {
                this._priorityRobot = null;
                dynamic[][] arr = { new dynamic[] { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1, lblConnectionR1 }, new dynamic[] { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2, lblConnectionR2 }, new dynamic[] { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3, lblConnectionR3 } };
                foreach (var val in value.ToString().Split(','))
                    for (int i = 0; i < arr.GetLength(0); i++)
                        if ((arr[i][0].Text == val.ToString()) && (arr[i][4].Text == "Connected")) {
                            this._priorityRobot = arr[i];
                            return; }
            }
        }

        private int mirrorX(int value)
        {
            int refMirror=4500;      // Half Long/Width of Arena            
            if (TransposeSwitch.Value == true)  // If condition Transpose is ON
                refMirror = 3000;

            if (LRSwitch.Value == true)     // If condition is RIGHT
                return refMirror + (refMirror - value);
            return value;                   // If condition is LEFT
        }

        private int mirrorY(int value)
        {
            int refMirror = 3000;      // Half Wide/Height of Arena      
            if (TransposeSwitch.Value == true)  // If condition Transpose is ON
                refMirror = 4500;

            if (LRSwitch.Value == true)     // If condition is RIGHT
                return refMirror + (refMirror - value);
            return value;                   // If condition is LEFT
        }

        private int mirrorAngle(int value)
        {
            if ((LRSwitch.Value == true) && (value < 180))          // If condition is RIGHT
                return value + 180;
            else if ((LRSwitch.Value == true) && (value >= 180))    // If condition is RIGHT
                return value - 180;
            return value;                                           // If condition is LEFT
        }

        private void addFormation()
        {
            string[] items = {
                "Nol",
                "Stand By",
                "Kick Off",
                "Penalty",
                "Corrner A",
                "Corrner B",
                "Free Kick",
            };
            cbxFormation.Items.AddRange(items);
        }

        private void moveLoc(int encodX, int encodY, dynamic robot)
        {
            Point point00Lap = new Point(30, 20);                                           // Reference (0, 0) of Arena
            Point point00Robot = new Point(robot.Size.Width / 2, robot.Size.Height / 2);    // Reference (0, 0) of Robot
            Point newLoc = new Point((point00Lap.X + encodX - point00Robot.X), (point00Lap.Y + encodY - point00Robot.Y));
            hc.SetLocation(this, robot, newLoc);
        }

        private void changeCounter(object sender, KeyEventArgs e)
        {
            var obj = ((dynamic)sender);
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { tbxEncXR3, tbxEncYR3, tbxAngleR3 }, { tbxScrXR1, tbxScrYR1, tbxAngleR1 }, { tbxScrXR2, tbxScrYR2, tbxAngleR2 }, { tbxScrXR3, tbxScrYR3, tbxAngleR3 }, { tbxGotoX, tbxGotoY, tbxGotoAngle } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if ((i == 6) && (arr[i, j].Tag == obj.Tag))
                        n = i;
                    else if (arr[i, j].Name == obj.Name)
                        n = i;
            if (n != -1)
                if ((!string.IsNullOrWhiteSpace(arr[n, 0].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 1].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 2].Text))) {
                    if (e.KeyCode == Keys.Right)
                        if ((TransposeSwitch.Value == true) && (obj.Name.StartsWith("tbxEnc") ^ (obj.Tag.StartsWith("tbxGoto"))))
                            arr[n, 1].Text = (int.Parse(arr[n, 1].Text) + 1).ToString();
                        else
                            arr[n, 0].Text = (int.Parse(arr[n, 0].Text) + 1).ToString();
                    else if (e.KeyCode == Keys.Left)
                        if ((TransposeSwitch.Value == true) && (obj.Name.StartsWith("tbxEnc") ^ (obj.Tag.StartsWith("tbxGoto"))))
                            arr[n, 1].Text = (int.Parse(arr[n, 1].Text) - 1).ToString();
                        else
                            arr[n, 0].Text = (int.Parse(arr[n, 0].Text) - 1).ToString();
                    else if (e.KeyCode == Keys.Up)
                        if ((TransposeSwitch.Value == true) && (obj.Name.StartsWith("tbxEnc") ^ (obj.Tag.StartsWith("tbxGoto"))))
                            arr[n, 0].Text = (int.Parse(arr[n, 0].Text) - 1).ToString();
                        else
                            arr[n, 1].Text = (int.Parse(arr[n, 1].Text) - 1).ToString();
                    else if (e.KeyCode == Keys.Down)
                        if ((TransposeSwitch.Value == true) && (obj.Name.StartsWith("tbxEnc") ^ (obj.Tag.StartsWith("tbxGoto"))))
                            arr[n, 0].Text = (int.Parse(arr[n, 0].Text) + 1).ToString();
                        else
                            arr[n, 1].Text = (int.Parse(arr[n, 1].Text) + 1).ToString();
                    else if (e.KeyCode == Keys.PageUp)
                        arr[n, 2].Text = (int.Parse(arr[n, 2].Text) + 1).ToString();
                    else if (e.KeyCode == Keys.PageDown)
                        arr[n, 2].Text = (int.Parse(arr[n, 2].Text) - 1).ToString();

                    if ((obj.Name.StartsWith("tbxScr")) && ((e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Left)))
                        if (TransposeSwitch.Value == true)
                            hc.SetText(this, arr[n-3, 1], ((int.Parse(arr[n, 0].Text)) * scale).ToString());      // On encoder tbx
                        else
                            hc.SetText(this, arr[n-3, 0], ((int.Parse(arr[n, 0].Text)) * scale).ToString());      // On encoder tbx
                    else if ((obj.Name.StartsWith("tbxScr")) && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)))
                        if (TransposeSwitch.Value == true)
                            hc.SetText(this, arr[n-3, 0], ((int.Parse(arr[n, 1].Text)) * scale).ToString());      // On encoder tbx
                        else
                            hc.SetText(this, arr[n-3, 1], ((int.Parse(arr[n, 1].Text)) * scale).ToString());      // On encoder tbx
                }
        }

        private void goArrow(object sender, KeyEventArgs e)
        {
            var obj = ((dynamic)sender);
            dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 }, { lblRobot1, tbxScrXR1, tbxScrYR1, tbxAngleR1 }, { lblRobot2, tbxScrXR2, tbxScrYR2, tbxAngleR2 }, { lblRobot3, tbxScrXR3, tbxScrYR3, tbxAngleR3 } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if ((i == 6) && (arr[i, j].Tag == obj.Tag))
                        n = i;
                    else if (arr[i, j].Name == obj.Name)
                        n = i;

            dynamic x = "x", y = "y", z = "z", message = string.Empty;
            if (TransposeSwitch.Value == true)
                swap(ref x, ref y);

            if (n != -1)
                if ((!string.IsNullOrWhiteSpace(arr[n, 1].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 2].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 3].Text)))
                {
                    if (e.KeyCode == Keys.Right)
                        message = x + "+";
                    else if (e.KeyCode == Keys.Left)
                        message = x + "-";
                    else if (e.KeyCode == Keys.Up)
                        message = y + "-";
                    else if (e.KeyCode == Keys.Down)
                        message = y + "+";
                    else if (e.KeyCode == Keys.PageUp)
                        message = z + "+";
                    else if (e.KeyCode == Keys.PageDown)
                        message = z + "-";

                    if ((!string.IsNullOrWhiteSpace(message)) && (_socketDict.ContainsKey(arr[n, 0].Text)))     //Send command go by arrow
                        SendCallBack(_socketDict[arr[n, 0].Text], message/*, "GoByArrow"*/);
                }
        }

        private void tbxXYZChanged(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender);
            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, tbxAngleR1, picRobot1, imgRobot[0], lblRobot1, OSR1 }, { tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, tbxAngleR2, picRobot2, imgRobot[1], lblRobot2, OSR2 }, { tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, tbxAngleR3, picRobot3, imgRobot[2], lblRobot3, OSR3 } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if ((j != 6) && (arr[i, j].Name == obj.Name))
                        n = i;

            if (n != -1)
                if ((Regex.IsMatch(obj.Text, "^[-]{0,1}[0-9]{1,4}$")) && (!string.IsNullOrWhiteSpace(arr[n, 0].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 1].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 2].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 3].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 4].Text))) {
                    /// Using Scale 1:20
                    if (obj.Name.StartsWith("tbxEncX"))
                        if (TransposeSwitch.Value == true)
                            hc.SetText(this, arr[n, 3], ((int.Parse(arr[n, 0].Text)) / scale).ToString());      // On screen tbx
                        else
                            hc.SetText(this, arr[n, 2], ((int.Parse(arr[n, 0].Text)) / scale).ToString());      // On screen tbx
                    else if (obj.Name.StartsWith("tbxEncY"))
                        if (TransposeSwitch.Value == true)
                            hc.SetText(this, arr[n, 2], ((int.Parse(arr[n, 1].Text)) / scale).ToString());      // On screen tbx
                        else
                            hc.SetText(this, arr[n, 3], ((int.Parse(arr[n, 1].Text)) / scale).ToString());      // On screen tbx

                    if (obj.Name.StartsWith("tbxScr"))
                        moveLoc(int.Parse(arr[n, 2].Text), int.Parse(arr[n, 3].Text), arr[n, 5]);               /// Display Location on Screen
                    else if ((obj.Name.StartsWith("tbxAngle")) && ((float.Parse(arr[n, 4].Text) % 2) == 0))
                        RotateImage(arr[n, 5], arr[n, 6], float.Parse(arr[n, 4].Text));                         /// Display Rotate on Screen

                    dynamic w = 9000, h = 6000;
                    if (TransposeSwitch.Value == true)
                        swap(ref w, ref h);
                    if ((obj.Name.StartsWith("tbxEnc")) && (((int.Parse(arr[n, 0].Text) < 0) ^ (int.Parse(arr[n, 0].Text) > w)) || ((int.Parse(arr[n, 1].Text) < 0) ^ (int.Parse(arr[n, 1].Text) > h)))) { 
                        if (!notifDict.ContainsKey(arr[n, 7].Text)) {      /// Notification that Robot is Outside
                            hc.SetVisible(this, arr[n, 8], true);
                            notifDict.Add(arr[n, 7].Text, (new System.Threading.Timer(new TimerCallback(notifOutside), arr[n, 7].Text, 0, 3000))); } }
                    else if ((obj.Name.StartsWith("tbxEnc")) && (notifDict.ContainsKey(arr[n, 7].Text))) {
                        hc.SetVisible(this, arr[n, 8], false);
                        notifDict[arr[n, 7].Text].Change(Timeout.Infinite, Timeout.Infinite);
                        notifDict.Remove(arr[n, 7].Text); }
                }
        }

        private Image RotateImage(dynamic pictureBox, Image imgPbx, float rotationAngle)
        {
            pictureBox.Image = imgPbx;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Image img = pictureBox.Image;

            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);

            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));

            gfx.Dispose();
            return pictureBox.Image = bmp;
        }

        private void tbxEncScrAng_KeyDown(object sender, KeyEventArgs e)
        {
            var obj = ((dynamic)sender);
            dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, tbxAngleR3 } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj.Name)
                        n = i;
            if (n != -1) { 
                if (!Regex.IsMatch(obj.Text, "^[-]{0,1}[0-9]{1,4}$"))
                addCommand("# Only Can Input Number [0-9] :<");
                switch (e.KeyCode) {
                    case Keys.Right:
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.PageUp:
                    case Keys.PageDown:
                    case Keys.Enter:
                        goArrow(sender, e);     ///For Arduino is available
                        //changeCounter(sender, e);     ///For Arduino is NOT available
                        //string dtGoto = "E" + arr[n, 1].Text + "," + arr[n, 2].Text + "," + arr[n, 5].Text;
                        //if (_socketDict.ContainsKey(arr[n, 0].Text))
                        //    SendCallBack(_socketDict[arr[n, 0].Text], dtGoto);
                        break; } }
        }

        private void tbxGoto_KeyDown(object sender, KeyEventArgs e)
        {
            var obj = ((dynamic)sender);
            dynamic[] arr = { tbxGotoX, tbxGotoY, tbxGotoAngle };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i].Tag == obj.Tag)
                    n = i;
            if (n != -1) { 
                if (!Regex.IsMatch(obj.Text, "^[-]{0,1}[0-9]{1,4}$"))
                addCommand("# Only Can Input Number [0-9] :<");
                switch (e.KeyCode)
                {
                    case Keys.Right:
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.PageUp:
                    case Keys.PageDown:
                        changeCounter(sender, e);
                        break;
                    case Keys.Enter:
                        runGoto("tbx", chkRobotCollect);
                        break;
                } }
        }

        private void lblGoto_Click(object sender, EventArgs e)
        {
            runGoto("tbx", chkRobotCollect);
        }

        private void runGoto(string dtXYZ, string sourceCollect)
        {
            foreach (var robot in sourceCollect.Split(',')) {
                dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
                int n = -1;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0].Text == robot)
                        n = i;

                if (n != -1)
                    if ((dtXYZ.Equals("tbx")) && (!string.IsNullOrEmpty(sourceCollect))) {
                        if ((!string.IsNullOrWhiteSpace(tbxGotoX.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoY.Text)) && (!string.IsNullOrWhiteSpace(tbxGotoAngle.Text)))
                            //threadGoto(arr[n, 0].Text, new Thread(obj => GotoLoc(arr[n, 0].Text, arr[n, 1], arr[n, 2], arr[n, 3], int.Parse(tbxGotoX.Text), int.Parse(tbxGotoY.Text), int.Parse(tbxGotoAngle.Text), scale, scale, 1)));      ///For NOT connected Arduino
                            GotoLoc(robot, int.Parse(tbxGotoX.Text), int.Parse(tbxGotoY.Text), int.Parse(tbxGotoAngle.Text));   /*For connected Arduino*/ }
                    else if (dtXYZ.Equals("tbx"))
                        MessageBox.Show("# Please Select/Checklist the Robot");
                    else if (Regex.IsMatch(dtXYZ, "^(go|Go|gO|GO)[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}$")) {
                        var _dtXYZ = dtXYZ.Substring(2).Split(',');
                        //threadGoto(arr[n, 0].Text, new Thread(obj => GotoLoc(arr[n, 0].Text, arr[n, 1], arr[n, 2], arr[n, 3], int.Parse(_dtXYZ[0]), int.Parse(_dtXYZ[1]), int.Parse(_dtXYZ[2]), scale, scale, 1)));       ///For NOT connected Arduino
                        GotoLoc(robot, int.Parse(_dtXYZ[0]), int.Parse(_dtXYZ[1]), int.Parse(_dtXYZ[2]));   /*For connected Arduino*/ }
            }
        }

        void GotoLoc(string socketKey, int endX, int endY, int endAngle)
        {
            if (_socketDict.ContainsKey(socketKey)) {
                addCommand("@ " + socketToName(_socketDict[socketKey]) + " : Goto >> " + ("X:" + endX + " Y:" + endY + " ∠:" + endAngle + "°"));
                SendCallBack(_socketDict[socketKey], ("go"+ endX + "," + endY + "," + endAngle), "Goto"); }
        }

        void GotoLoc(string Robot, dynamic encXRobot, dynamic encYRobot, dynamic angleRobot, int endX, int endY, int endAngle, int shiftX, int shiftY, int shiftAngle)
        {
            try {
                int startX = int.Parse(encXRobot.Text), startY = int.Parse(encYRobot.Text), startAngle = int.Parse(angleRobot.Text);
                addCommand("@ " + socketToName(_socketDict[Robot]) +" : Goto >> "+ ("X:" + endX + " Y:" + endY + " ∠:" + endAngle + "°"));
                hc.SetText(this, tbxGotoX, endX.ToString());
                hc.SetText(this, tbxGotoY, endY.ToString());
                hc.SetText(this, tbxGotoAngle, endAngle.ToString());

                bool[] chk = { true, true, true };
                while ((gotoDict.ContainsKey(Robot)) && (chk[0] |= chk[1] |= chk[2])) {
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

                    string dtGoto = "E" + startX + "," + startY + "," + startAngle;
                    SendCallBack(_socketDict[Robot], dtGoto, "Goto");
                    Thread.Sleep(100);    // time per limit (milisecond)
                } }
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
            tbxMessage.Clear();
        }

        void resetToogle()
        {
            dynamic[] arr = { tglAutoReconBS, tglAutoReconRB, tglAutoReconR1, tglAutoReconR2, tglAutoReconR3 };
            foreach (var i in arr)
                i.Checked = true;
            TeamSwitch.Value = true;
            LRSwitch.Value = TransposeSwitch.Value = false;
        }


        //////////////////////////////////////////////////////////////      COMUNICATION       //////////////////////////////////////////////////////////////
        ///
        byte[] _buffer = new byte[1024];
        static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket _toServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dictionary<string, Socket> _socketDict = new Dictionary<string, Socket>();
        HashSet<string> notConnectionCollect = new HashSet<string>(), connectingCollect = new HashSet<string>();
        List<dynamic> _chkRobotCollect = new List<dynamic>();
        internal int port, attempts = 0, ctr = 0;
        internal string myIP, chkRobotCollect = string.Empty, ballOn = string.Empty;

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

        private void checkConnection(object state)
        {
            Recheck:
            try {
                dynamic[,] arr = { { lblBaseStation, lblConnectionBS }, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
                for (int i = 0; i < arr.GetLength(0); i++)          // Check for Server and Client Connection
                    if (((_socketDict.ContainsKey(arr[i, 0].Text)) && (!_socketDict[arr[i, 0].Text].Connected)) || ((arr[i, 1].Text.Equals("Open")) && (!_serverSocket.IsBound))) 
                        if (arr[i, 0].Text.Equals(lblBaseStation.Text))
                            forceDisconnect(arr[i, 0].Text);
                        else
                            forceDisconnect(_socketDict[arr[i, 0].Text]);
                foreach (string j in notConnectionCollect) {       // Auto Reconnecting
                    dynamic[,] arr2 = new dynamic[,] { { lblBaseStation, tglAutoReconBS }, { lblRefereeBox, tglAutoReconRB }, { lblRobot1, tglAutoReconR1 }, { lblRobot2, tglAutoReconR2 }, { lblRobot3, tglAutoReconR3 } };
                    int n = -1;
                    for (int i = 0; i < arr.GetLength(0); i++)
                        if (arr2[i, 0].Text == j)
                            n = i;

                    if (n != -1)
                        if (arr2[n, 1].Checked == true)     // If Auto Reconnect ON
                            if ((arr2[n, 0].Text == lblBaseStation.Text) && (!connectingCollect.Contains(arr2[n, 1].Text)))
                                grpBaseStation_Click(grpBaseStation, EventArgs.Empty);
                            else if (!connectingCollect.Contains(arr2[n, 1].Text))
                                Connection_byDistinct(arr2[n, 0], EventArgs.Empty);  } }
            catch (Exception)
            { goto Recheck; }
        }

        private void forceDisconnect(dynamic socket)
        {
            try {   // Force for Disconnect  
                dynamic[,] arr = { { lblBaseStation, lblConnectionBS }, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
                int n = -1;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if ((i != 0) && (socket.GetType() != typeof(string)) && ((_socketDict.ContainsKey(arr[i, 0].Text)) && (_socketDict[arr[i, 0].Text].RemoteEndPoint == socket.RemoteEndPoint))) {    // For RefreeBox & Robot
                        hc.SetText(this, arr[i, 1], "Disconnected");
                        _socketDict[arr[n, 0]].Dispose(); }
                    else if ((i == 0) && (socket.GetType() == typeof(string)) && (arr[i, 0].Text == socket)) {   // For BaseStation
                        hc.SetText(this, arr[i, 1], "Close");
                        _serverSocket.Dispose(); } }
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
                arr = new dynamic[,] { { lblConnectionR1, lblRobot1, chkR1, lblEncoderR1, lblScreenR1, tbxEncXR1, tbxEncYR1, tbxScrXR1, tbxScrYR1, tbxAngleR1, lblDegR1, lblSpeedR1, lblSpeedValR1 }, { lblConnectionR2, lblRobot2, chkR2, lblEncoderR2, lblScreenR2, tbxEncXR2, tbxEncYR2, tbxScrXR2, tbxScrYR2, tbxAngleR2, lblDegR2, lblSpeedR2, lblSpeedValR2 }, { lblConnectionR3, lblRobot3, chkR3, lblEncoderR3, lblScreenR3, tbxEncXR3, tbxEncYR3, tbxScrXR3, tbxScrYR3, tbxAngleR3, lblDegR3, lblSpeedR3, lblSpeedValR3 } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 0].Name == obj)
                    n = i;

            if (n != -1) { 
                if ((arr[n, 0].Text == "Connected") ^ (arr[n, 0].Text == "Open")) {
                    notConnectionCollect.Remove(arr[n, 1].Text);
                    arr[n, 0].BackColor = Color.SeaGreen;
                    for (int i = 0; i < arr.GetLength(1); i++)
                        arr[n, i].Enabled = true; }
                else {
                    if (obj == lblConnectionBS.Name)
                        addCommand("\n# " + arr[n, 1].Text + " server is CLOSE :<");
                    else
                        addCommand("\n# " + arr[n, 1].Text + " has DISCONNECTED :<");
                    if (_socketDict.ContainsKey(arr[n, 1].Text))
                        _socketDict.Remove(arr[n, 1].Text);
                    notConnectionCollect.Add(arr[n, 1].Text);
                    arr[n, 0].BackColor = Color.Firebrick;
                    for (int i = 0; i < arr.GetLength(1); i++)
                        if (i != 0)
                            arr[n, i].Enabled = false;
                } }
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
                if (_toServerSocket.Connected) {
                    if (_socketDict.ContainsKey(keyName))
                        return;
                    _socketDict.Add(keyName, _toServerSocket);
                    addCommand("# Success Connecting to: " + ipDst + " (" + keyName + ") \t Port : " + port);
                    hc.SetText(this, connection, "Connected");
                    SendCallBack(_toServerSocket, this.Text);
                    _toServerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _toServerSocket); }
                else
                    _toServerSocket.Dispose();
                connectingCollect.Remove(keyName);
            }
            catch (SocketException)
            {
                hc.SetText(this, tbxStatus, string.Empty);
                addCommand("# IP This Device  : " + myIP + " (" + this.Text + ")");
                addCommand("# IP Destination  : " + ipDst + " (" + keyName + ") \t Port : " + port);
                addCommand("# Connection attempts: " + attempts.ToString());
                notConnectionCollect.Add(keyName);
                connectingCollect.Remove(keyName);
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
                    connectingCollect.Remove(lblBaseStation.Text);
                }
            }
            catch (Exception e)
            {
                hc.SetText(this, tbxStatus, string.Empty);
                addCommand("# FAILED to open server connection \n\n" + e);
                notConnectionCollect.Add(lblBaseStation.Text);
                connectingCollect.Remove(lblBaseStation.Text);
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
                addCommand("# FAILED to connect \n\n" + e);
                forceDisconnect(lblBaseStation.Text);
            }
        }

        void ReceiveCallBack(IAsyncResult AR) /**/
        {
            Socket socket = null;
            try {
                socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                string message = Encoding.ASCII.GetString(dataBuf).Trim();
                message = new string(message.Where(c => !char.IsControl(c)).ToArray());
                if (_socketDict.ContainsValue(socket)) { 
                    if (string.IsNullOrWhiteSpace(message)) {
                        forceDisconnect(socket);
                        socket.Disconnect(true);
                        return; }
                    if ((!string.IsNullOrWhiteSpace(message)) && (!Regex.IsMatch(message, "E[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}")))
                        addCommand("> " + socketToName(socket) + " : " + message);
                    ResponeReceivedCallback(message, socket);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket); }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to receive message \n\n" + e);
                forceDisconnect(socket);
            }
        }

        void SendCallBack(Socket _dstSocket, string txtMessage)
        {
            try
            {
                txtMessage = new string(txtMessage.Trim().Where(c => !char.IsControl(c)).ToArray());
                if ((_socketDict.ContainsValue(_dstSocket)) && (!string.IsNullOrWhiteSpace(txtMessage))) {                     
                    if (Regex.IsMatch(txtMessage, "E[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}")) {
                        //var pos = txtMessage.Split(',');
                        //addCommand("@ " + socketToName(_dstSocket) + " : " + ("X:" + pos[0] + " Y:" + pos[1] + " ∠:" + pos[2] + "°"));
                    } else
                    addCommand("@ " + socketToName(_dstSocket) + " : " + txtMessage);

                    txtMessage = new string(txtMessage.Where(c => !char.IsControl(c)).ToArray());
                    byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
                    _dstSocket.Send(buffer);
                    _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket); }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
                forceDisconnect(_dstSocket);
            }
        }

        void SendCallBack(Socket _dstSocket, string txtMessage, string Goto)
        {
            try
            {
                txtMessage = new string(txtMessage.Trim().Where(c => !char.IsControl(c)).ToArray());
                if ((_socketDict.ContainsValue(_dstSocket)) && (!string.IsNullOrWhiteSpace(txtMessage))) {
                    //var pos = txtMessage.Split(',');
                    //addCommand("@ " + socketToName(_dstSocket) + " : " + ("X:" + pos[0] + " Y:" + pos[1] + " ∠:" + pos[2] + "°"));
                    byte[] buffer = Encoding.ASCII.GetBytes(txtMessage);
                    _dstSocket.Send(buffer);
                    _dstSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _dstSocket); }
            }
            catch (Exception e)
            {
                addCommand("# FAILED to send message \n\n" + e);
                forceDisconnect(_dstSocket);
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

            if ((_dtMessage[0].StartsWith("!")) && (_dtMessage[0].Length > 1)) {        // Broadcast message
                _dtMessage = new string[] { _dtMessage[0].Substring(1), "Robot1,Robot2,Robot3" }; }
            if ((_dtMessage[0].StartsWith("**")) && (_dtMessage[0].Length > 2)) {       // Forward & Broadcast message
                respone = _dtMessage[0];
                goto broadcast; }
            else if ((_dtMessage[0].StartsWith("*")) && (_dtMessage[0].Length > 1)) {   // Forward & Multicast message
                goto multicast; }

            if (_dtMessage[0].ToLower() == "myip") {
                text = "MyIP: " + GetMyIP();
                respone = GetMyIP();
                if (_dtMessage.Count() > 1)
                    goto multicast;
                MessageBox.Show(text);
                goto end; }
            else if (Regex.IsMatch(_dtMessage[0], "^(go|Go|gO|GO)[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}$")) {
                // Goto Location
                if (_dtMessage.Count() > 1)
                    runGoto(_dtMessage[0], _dtMessage[1]);
                else if (!string.IsNullOrWhiteSpace(chkRobotCollect))
                    runGoto(_dtMessage[0], chkRobotCollect);
                else
                    MessageBox.Show("# Please Select/Checklist the Robot"); }
            else if ((!string.IsNullOrWhiteSpace(_dtMessage[0])) && (((_dtMessage.Count() == 2) && (!string.IsNullOrWhiteSpace(_dtMessage[1]))) || (!string.IsNullOrWhiteSpace(chkRobotCollect)) )) {
                // If to send Robot socket   
                switch (_dtMessage[0]) {
                    /// INFORMATION ///
                    /// OTHERS ///
                    case ";":   //PING
                        respone = "ping";
                        goto multicast;
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
            if (string.IsNullOrWhiteSpace(respone))
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
            
            if ((_dtMessage[0].StartsWith("!")) && (_dtMessage[0].Length > 1)) {        // Broadcast message
                _dtMessage = new string[] { _dtMessage[0].Substring(1), "Robot1,Robot2,Robot3" }; }
            if ((_dtMessage[0].StartsWith("**")) && (_dtMessage[0].Length > 2)) {       // Forward & Broadcast message
                respone = _dtMessage[0].Substring(2);
                goto broadcast; }
            else if ((_dtMessage[0].StartsWith("*")) && (_dtMessage[0].Length > 1)) {   // Forward & Multicast message
                respone = _dtMessage[0].Substring(1);
                goto multicast; }

            if (Regex.IsMatch(_dtMessage[0], "^E[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}$"))
            {
                // If _dtMessage[0] is data X & Y from encoder
                /// Scale is 1 : 20 
                string objName = null;
                dynamic[] posXYZs = _dtMessage[0].Split('E');
                dynamic[] posXYZ = (posXYZs[posXYZs.Length - 1]).Split(',');
                posXYZ = posXYZ.Where(item => (!string.IsNullOrWhiteSpace(item))).ToArray();

                foreach (var _temp in _socketDict)                              // Get socket name from IP
                    if (_temp.Value.RemoteEndPoint == socket.RemoteEndPoint)
                        objName = _temp.Key.ToString();

                dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
                int n = -1;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0].Text == objName)
                        n = i;

                if (n != -1) { 
                    hc.SetText(this, arr[n, 1], posXYZ[0]);          // On encoder tbx
                    hc.SetText(this, arr[n, 2], posXYZ[1]);
                    hc.SetText(this, arr[n, 3], posXYZ[2]);
                    //text = "X:" + posXYZ[0] + " Y:" + posXYZ[1] + " ∠:" + posXYZ[2] + "°";
                }
            }
            else if (Regex.IsMatch(_dtMessage[0], "^(go|Go|gO|GO)[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4},[-]{0,1}[0-9]{1,4}$"))
            {
                // Goto Location
                if (_dtMessage.Count() > 1)
                    runGoto(_dtMessage[0], _dtMessage[1]);
            }
            else if (Regex.IsMatch(_dtMessage[0], "^(Robot[0-9])$"))
            {
                // If will rename key in socket dictionary
                dynamic[,] arr = { { lblRobot1.Text, lblConnectionR1, tbxIPR1, tbxPortR1 }, { lblRobot2.Text, lblConnectionR2, tbxIPR2, tbxPortR2 }, { lblRobot3.Text, lblConnectionR3, tbxIPR3, tbxPortR3 } };
                int n = -1;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 0] == _dtMessage[0])
                        n = i;
                if (n != -1)
                    if (_socketDict.ContainsKey(socket.RemoteEndPoint.ToString()))
                    {
                        Socket temp = _socketDict[socket.RemoteEndPoint.ToString()];    // Backup
                        _socketDict.Remove(socket.RemoteEndPoint.ToString());           // Remove with old key
                        _socketDict.Add(_dtMessage[0], temp);                           // Add with new key
                        hc.SetText(this, arr[n, 1], "Connected");
                        hc.SetText(this, arr[n, 2], socketToIP(socket));
                        hc.SetText(this, arr[n, 3], this.port.ToString());
                    }
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
                        picTimer.Tag = "stop";
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        goto broadcast;
                    case "s": //START
                        text = "START";
                        picTimer.Tag = "start";
                        timer.Change(500, 1000);
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
                    case "N": //DROP_BALL
                        text = "DROP_BALL";
                        goto broadcast;

                    /// 6. OTHERS ///
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
                            setTimer("Robot1-DYCard", 120);
                            setTimer("Robot2-DYCard", 120);
                            setTimer("Robot3-DYCard", 120);
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "A": //GOAL_CYAN
                            text = "GOAL_CYAN";
                            hc.SetText(this, lblGoalCyan, (int.Parse(lblGoalCyan.Text) + 1).ToString());
                            goto broadcast;
                        case "D": //SUBGOAL_CYAN
                            text = "SUBGOAL_CYAN";
                            hc.SetText(this, lblGoalCyan, (int.Parse(lblGoalCyan.Text) - 1).ToString());
                            goto broadcast;

                        /// 5. GAME FLOW COMMANDS ///
                        case "K": //KICKOFF_CYAN
                            text = "KICKOFF_CYAN";
                            //cbxFormation.SelectedItem = "Kick Off";
                            goto broadcast;
                        case "F": //FREEKICK_CYAN
                            text = "FREEKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblFouls });
                            goto broadcast;
                        case "G": //GOALKICK_CYAN
                            text = "GOALKICK_CYAN";
                            setMatchInfo(new dynamic[] { lblGoalKick });
                            goto broadcast;
                        case "T": //THROWN_CYAN
                            text = "THROWN_CYAN";
                            goto broadcast;
                        case "C": //CORNER_CYAN
                            text = "CORNER_CYAN";
                            setMatchInfo(new dynamic[] { lblCorner });
                            goto broadcast;
                        case "P": //PENALTY_CYAN
                            text = "PENALTY_CYAN";
                            goto broadcast;
                        case "O": //REPAIR_CYAN
                            text = "REPAIR_CYAN";
                            setTimer("Robot1-Repair", 30);
                            setTimer("Robot2-Repair", 30);
                            setTimer("Robot3-Repair", 30);
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
                            setTimer("Robot1-DYCard", 120);
                            setTimer("Robot2-DYCard", 120);
                            setTimer("Robot3-DYCard", 120);
                            goto broadcast;

                        /// 4. GOAL STATUS ///
                        case "a": //GOAL_MAGENTA
                            text = "GOAL_MAGENTA";
                            hc.SetText(this, lblGoalMagenta, (int.Parse(lblGoalMagenta.Text) + 1).ToString());
                            goto broadcast;
                        case "d": //SUBGOAL_MAGENTA
                            text = "SUBGOAL_MAGENTA";
                            hc.SetText(this, lblGoalMagenta, (int.Parse(lblGoalMagenta.Text) - 1).ToString());
                            goto broadcast;

                        /// 5. GAME FLOW COMMANDS ///
                        case "k": //KICKOFF_MAGENTA
                            text = "KICKOFF_MAGENTA";
                            //cbxFormation.SelectedItem = "Kick Off";
                            goto broadcast;
                        case "f": //FREEKICK_MAGENTA
                            text = "FREEKICK_MAGENTA";
                            setMatchInfo(new dynamic[] { lblFouls });
                            goto broadcast;
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
                        case "p": //PENALTY_MAGENTA
                            text = "PENALTY_MAGENTA";
                            goto broadcast;
                        case "o": //REPAIR_MAGENTA
                            text = "REPAIR_MAGENTA";
                            setTimer("Robot1-Repair", 30);
                            setTimer("Robot2-Repair", 30);
                            setTimer("Robot3-Repair", 30);
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
                    case "B_": //Get the Ball
                        respone = "B_" + socketToName(socket);
                        text = "Ball on " + socketToName(socket);
                        ballOn = socketToName(socket);
                        var obj = socketToName(socket);
                        dynamic[,] arr = { { lblRobot1, ballR1, picRobot1, imgRobot[0], "Robot 1 Attacker.png", "Robot 1 Attacker-Get Ball.png", tbxAngleR1 }, { lblRobot2, ballR2, picRobot2, imgRobot[1], "Robot 2 Defence.png", "Robot 2 Defence-Get Ball.png", tbxAngleR2 }, { lblRobot3, ballR3, picRobot3, imgRobot[2], "Robot 3 Kiper.png", "Robot 3 Kiper-Get Ball.png", tbxAngleR3 } };
                        for (int i = 0; i < arr.GetLength(0); i++) {    /// PREVIOUS
                            hc.SetVisible(this, arr[i, 1], false);
                            arr[i, 2].Image = Image.FromFile("images/" + arr[i, 4]);
                            imgRobot[i] = Image.FromFile("images/" + arr[i, 4]); }
                        for (int i = 0; i < arr.GetLength(0); i++)      /// CURRENT
                            if (arr[i, 0].Text == obj) {
                                hc.SetVisible(this, arr[i, 1], true);
                                arr[i, 2].Image = Image.FromFile("images/" + arr[i, 5]);
                                imgRobot[i] = Image.FromFile("images/" + arr[i, 5]); }
                        goto broadcast;
                    case "b_": //Lose the Ball
                        respone = "b_";
                        text = "Lose the Ball";
                        ballOn = string.Empty;
                        dynamic[,] arr2 = { { lblRobot1, ballR1, picRobot1, imgRobot[0], "Robot 1 Attacker.png", "Robot 1 Attacker-Get Ball.png", tbxAngleR1 }, { lblRobot2, ballR2, picRobot2, imgRobot[1], "Robot 2 Defence.png", "Robot 2 Defence-Get Ball.png", tbxAngleR2 }, { lblRobot3, ballR3, picRobot3, imgRobot[2], "Robot 3 Kiper.png", "Robot 3 Kiper-Get Ball.png", tbxAngleR3 } };
                        for (int i = 0; i < arr2.GetLength(0); i++) {    /// CURRENT
                            hc.SetVisible(this, arr2[i, 1], false);
                            arr2[i, 2].Image = Image.FromFile("images/" + arr2[i, 4]);
                            imgRobot[i] = Image.FromFile("images/" + arr2[i, 4]); }
                        goto broadcast;
                    case "B?": // Ball Status
                        if (string.IsNullOrWhiteSpace(ballOn))            //Lose the Ball
                            respone = "b_";
                        else
                            respone = "B_" + socketToName(socket);      //Ball on Robot
                        goto multicast;

                    /// OTHERS ///
                    case "quit": //Quit/Close Robot
                        text = "Quit/Close " + socketToName(socket);
                        forceDisconnect(socket);
                        break;
                    case "ping": //PING-REPLY
                        respone = "Reply " + this.Text;
                        goto multicast;
                    case "ip": //IP Address Info
                        respone = GetMyIP();
                        goto multicast;
                    case "get_time": //TIME NOW
                        respone = DateTime.Now.ToLongTimeString();
                        goto multicast;
                        //default:
                        //respone = text = "# Invalid Command :<";
                }
            }
            if ((string.IsNullOrWhiteSpace(respone)) && (_dtMessage.Count() > 1))
                sendByHostList(_dtMessage[1], _dtMessage[0]);
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
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    if (arr[i, j].Name == obj)
                        n = i;

            if (n != -1) { 
                if (arr[n, 0].Checked == true)
                _chkRobotCollect.Add(arr[n, 1].Text);
                else
                    _chkRobotCollect.Remove(arr[n, 1].Text);
                _chkRobotCollect.Sort();
                if (_chkRobotCollect.Count == 0)
                    chkRobotCollect = string.Empty;
                for (int i = 0; i < _chkRobotCollect.Count; i++) {
                    if (i == 0)
                        chkRobotCollect = _chkRobotCollect.ElementAtOrDefault(i);
                    else
                        chkRobotCollect += "," + _chkRobotCollect.ElementAtOrDefault(i); } }
        }

        private void grpBaseStation_Click(object sender, EventArgs e)
        {
            if (connectingCollect.Add(lblBaseStation.Text) && (lblConnectionBS.Text == "Close") && (!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                new Thread(obj => SetupServer(tbxPortBS.Text)).Start();
        }

        private void tbxOpenBS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (connectingCollect.Add(lblBaseStation.Text) && (lblConnectionBS.Text == "Close") && (!string.IsNullOrWhiteSpace(tbxIPBS.Text)) && (!string.IsNullOrWhiteSpace(tbxPortBS.Text)))
                    new Thread(obj => SetupServer(tbxPortBS.Text)).Start();
        }

        private void Connection_byDistinct(object sender, EventArgs e)
        {
            try {
                var obj = ((dynamic)sender).Name;
                dynamic[,] arr = { { grpBaseStation, lblBaseStation, lblConnectionBS, tbxIPBS, tbxPortBS }, { grpRefereeBox, lblRefereeBox, lblConnectionRB, tbxIPRB, tbxPortRB }, { grpRobot1, lblRobot1, lblConnectionR1, tbxIPR1, tbxPortR1 }, { grpRobot2, lblRobot2, lblConnectionR2, tbxIPR2, tbxPortR2 }, { grpRobot3, lblRobot3, lblConnectionR3, tbxIPR3, tbxPortR3 } };
                int n = -1;
                for (int i = 0; i < arr.GetLength(0); i++)
                    for (int j = 0; j < arr.GetLength(1); j++)
                        if (arr[i, j].Name == obj)
                            n = i;
                if (n != -1)
                    if ((connectingCollect.Add(arr[n, 1].Text)) && (arr[n, 2].Text == "Disconnected"))
                        if ((!string.IsNullOrWhiteSpace(arr[n, 3].Text)) && (!string.IsNullOrWhiteSpace(arr[n, 4].Text)))
                            new Thread(objs => requestConnect(arr[n, 3].Text, arr[n, 4].Text, arr[n, 1].Text, arr[n, 2])).Start();
                        else
                            connectingCollect.Remove(arr[n, 1].Text);
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
            try { 
                var obj = ((dynamic)sender).Name;
                dynamic[,] arr = { { lblBaseStation, lblConnectionBS}, { lblRefereeBox, lblConnectionRB }, { lblRobot1, lblConnectionR1 }, { lblRobot2, lblConnectionR2 }, { lblRobot3, lblConnectionR3 } };
                int n = -1;
                for (int i = 0; i < arr.GetLength(0); i++)
                    if (arr[i, 1].Name == obj)
                        n = i;

                if (n != -1)
                    if (arr[n,1].Text == "Connected") {
                        hc.SetText(this, arr[n,1], "Disconnected");
                        _socketDict[arr[n, 0]].Dispose(); }
                    else if (arr[n, 1].Text == "Open") {
                        hc.SetText(this, arr[n, 1], "Close");
                        _serverSocket.Dispose(); } }
            catch (Exception)
            { }
        }

        private void btnDtRobot_Click(object sender, EventArgs e)
        {
            var obj = ((dynamic)sender).Name;
            dynamic[,] arr = { { btnDtR1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { btnDtR2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { btnDtR3, tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
            int n = -1;
            for (int i = 0; i < arr.GetLength(0); i++)
                if (arr[i, 0].Name == obj)
                    n = i;

            if (n != -1) { 
                tbxGotoX.Text = arr[n, 1].Text;
                tbxGotoY.Text = arr[n, 2].Text;
                tbxGotoAngle.Text = arr[n, 3].Text; }
        }

        void sendFromTextBox()
        {
            ResponeSendCallback(tbxMessage.Text.Trim());
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

        private void LRSwitch_OnValueChange(object sender, EventArgs e)
        {
            if (LRSwitch.Value == true)
                hc.SetText(this, lblLR, "Right");
            else
                hc.SetText(this, lblLR, "Left");
        }

        private void TransposeSwitch_OnValueChange(object sender, EventArgs e)
        {
            if (TransposeSwitch.Value == true)
                hc.SetText(this, lblTranspose, "Transpose");
            else
                hc.SetText(this, lblTranspose, "No Transpose");

            dynamic[,] arr = { { lblRobot1, tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { lblRobot2, tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { lblRobot3, tbxEncXR3, tbxEncYR3, tbxAngleR3 }, { lblGoto, tbxGotoX, tbxGotoY, tbxGotoAngle } };
            for (int i = 0; i < arr.GetLength(0); i++) {    // Swap data X & Y, then sent
                int n = i;
                swapObjText(ref arr[n, 1], ref arr[n, 2]);

                if (i != 3) {    //Is not tbxGoto
                    string dtGoto = "E" + arr[i, 1].Text + "," + arr[i, 2].Text + "," + arr[i, 3].Text;
                    if (_socketDict.ContainsKey(arr[i, 0].Text))
                        SendCallBack(_socketDict[arr[i, 0].Text], dtGoto); }
                Thread.Sleep(100); }
        }

        private void LRSwitch_DoubleClick(object sender, EventArgs e)
        {
            int x = 0, y = 1;
            if (TransposeSwitch.Value == true) {    // If condition Transpose is ON
                x = 1; y = 0; }

            dynamic[,] arr = { { tbxEncXR1, tbxEncYR1, tbxAngleR1 }, { tbxEncXR2, tbxEncYR2, tbxAngleR2 }, { tbxEncXR3, tbxEncYR3, tbxAngleR3 } };
            for (int i = 0, j = 0; i < arr.GetLength(0); i++, j+=3) { 
                hc.SetText(this, arr[i, 0], mirrorX(_StandBy[j+x]).ToString());
                hc.SetText(this, arr[i, 1], mirrorY(_StandBy[j+y]).ToString());
                hc.SetText(this, arr[i, 2], mirrorAngle(_StandBy[j+2]).ToString()); }
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

        private void picTimer_DoubleClick(object sender, EventArgs e)
        {
            hc.SetText(this, lblTimer, "00:00");
        }

        private void picTimer_Click(object sender, EventArgs e)
        {
            if (picTimer.Tag == "start") { 
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                picTimer.Tag = "stop"; }
            else { 
                timer.Change(500, 1000);                
                picTimer.Tag = "start"; }
        }

        private void lblTimer_TextChanged(object sender, EventArgs e)
        {
            if (lblTimer.Text == "00:00") {
                picTimer.Tag = "start";
                ProgressTM.MaxValue = 900;
                hc.SetValue(this, ProgressTM, 900);
                hc.SetVisible(this, ProgressTM, true);
                hc.SetVisible(this, picTimer, true);
                timer.Change(1000, 1000); }
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

        private void btnRestart_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }

        private void btnTO_Click(object sender, EventArgs e)
        {
            //forceDisconnect(lblBaseStation.Text);
            //forceDisconnect(_socketDict["Robot1"]);
            //MessageBox.Show((Regex.IsMatch(tbxMessage.Text, "^(Robot[0-9])$")).ToString());
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
    }
}
