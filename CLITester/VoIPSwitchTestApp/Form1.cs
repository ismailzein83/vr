using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

namespace VoIPSwitchTestApp
{
    public partial class Form1 : Form
    {
        #region Definitions

        private static readonly object _syncRoot = new object();

        //public bool finishCall = false;
        public static int OperatorId = 0;
        // Create a timer with a one second interval.
        System.Timers.Timer aTimer = new System.Timers.Timer(1000000);

        public RequestForCalls thRequestForCalls = new RequestForCalls();
        public GetCLIs thGetCLIs = new GetCLIs();
        public MySQLConn m = new MySQLConn();
        #endregion


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thRequestForCalls.Start();
            //m.start();
            //aTimer.Elapsed += OnTimedEvent;
            //aTimer.Enabled = true;
            //aTimer.Interval = 1000;
            //aTimer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                m.start();


                //thRequestForCalls.Start();
                //thGetCLIs.Start();
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}
