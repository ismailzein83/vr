using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Vanrise.Common
{
    public class VRClock
    {
        static Timer s_timer;
        static bool s_isTimerEnabled;//use this variable instead of Timer.Enabled method to avoid any performance issue in the Timer.Enabled property
        static DateTime s_now;
        static int s_setNowIteration;
        static int s_offsetInMilliseconds = 250;
        static int s_maxNbOfIterations = (30 * 1000) / s_offsetInMilliseconds;//after 30 seconds
        static Object s_lockObj = new object();
        static VRClock()
        {
            s_timer = new Timer(s_offsetInMilliseconds);
            s_timer.Elapsed += timer_Elapsed;
        }

        public static DateTime Now
        {
            get
            {                
                if (!s_isTimerEnabled)
                {
                    lock(s_timer)
                    {
                        if (!s_isTimerEnabled)
                        {
                            s_now = DateTime.Now;
                            s_setNowIteration = 0;
                            s_isTimerEnabled = true;
                            s_timer.Enabled = true;
                        }
                    }
                }
                return s_now;
            }
        }
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (s_lockObj)
            {
                s_now = DateTime.Now;
                s_setNowIteration++;
            }
            if (s_setNowIteration >= s_maxNbOfIterations)
            {
                lock (s_timer)
                {
                    s_isTimerEnabled = false;
                    s_timer.Enabled = false;
                }
            }
        }
    }
}
