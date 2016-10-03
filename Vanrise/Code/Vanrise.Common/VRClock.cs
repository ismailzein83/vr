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
        static VRClock()
        {
            Timer timer = new Timer(250);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public static DateTime Now
        {
            get
            {
                return s_now;
            }
        }

        static DateTime s_now = DateTime.Now;
        static int s_setNowIteration;
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (s_setNowIteration < 100)
            {
                s_now = s_now.AddMilliseconds(250);
                s_setNowIteration++;
            }
            else
            {
                s_now = DateTime.Now;
                s_setNowIteration = 0;
            }
        }
    }
}
