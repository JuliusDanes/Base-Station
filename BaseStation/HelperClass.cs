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

namespace BaseStation
{
    class HelperClass
    {
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        delegate void SetLocationCallback(Form f, Control ctrl, Point point);
        delegate void SetObjectCallback(Form f, Control ctrl, dynamic obj, dynamic val);
        delegate void SetValueCallback(Form f, dynamic ctrl, dynamic val);
        delegate void SetVisibleCallback(Form f, dynamic ctrl, dynamic val);
        /// Set text property of various controls

        internal void SetText(Form form, Control ctrl, string text)
        {
            try
            {
                // InvokeRequired required compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    form.Invoke(d, new object[] { form, ctrl, text });
                }
                else
                    ctrl.Text = text;
            }
            catch (Exception e)
            {
                //MessageBox.Show("# Error setText " + ctrl + " \n\n" + e);
            }
        }

        internal void SetLocation(Form form, Control ctrl, Point point)
        {
            try
            {
                if (ctrl.InvokeRequired)
                {
                    SetLocationCallback d = new SetLocationCallback(SetLocation);
                    form.Invoke(d, new object[] { form, ctrl, point });
                }
                else
                    ctrl.Location = point;
            }
            catch (Exception e)
            {
                MessageBox.Show("# Error setLocation \n\n" + e);
            }
        }

        internal void SetObject(Form form, dynamic ctrl, dynamic obj, dynamic val)
        {
            try
            {
                if (ctrl.InvokeRequired)
                {
                    SetObjectCallback d = new SetObjectCallback(SetObject);
                    form.Invoke(d, new object[] { form, ctrl, obj, val });
                }
                else
                    obj = val;
            }
            catch (Exception e)
            {
                MessageBox.Show("# Error setObject \n\n" + e);
            }
        }

        internal void SetValue(Form form, dynamic ctrl, dynamic val)
        {
            try
            {
                if (ctrl.InvokeRequired)
                {
                    SetValueCallback d = new SetValueCallback(SetValue);
                    form.Invoke(d, new object[] { form, ctrl, val });
                }
                else
                    ctrl.Value = val;
            }
            catch (Exception e)
            {
                MessageBox.Show("# Error setValue \n\n" + e);
            }
        }

        internal void SetVisible(Form form, dynamic ctrl, dynamic val)
        {
            try
            {
                if (ctrl.InvokeRequired)
                {
                    SetVisibleCallback d = new SetVisibleCallback(SetVisible);
                    form.Invoke(d, new object[] { form, ctrl, val });
                }
                else
                    ctrl.Visible = val;
            }
            catch (Exception e)
            {
                MessageBox.Show("# Error setValue \n\n" + e);
            }
        }
    }
}
