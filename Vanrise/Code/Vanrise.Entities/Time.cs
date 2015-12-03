using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Time
    {
        public Time() { }
        public Time(string time)
        {
            string[] timeArray = time.Split(':');
            for (var i = 0; i < timeArray.Length; i++)
            {
                switch (i)
                {
                    case 0: this.Hour = Convert.ToInt32(timeArray[i]); break;
                    case 1: this.Minute = Convert.ToInt32(timeArray[i]); break;
                    case 2: this.Second = Convert.ToInt32(timeArray[i]); break;
                    case 3: this.MilliSecond = Convert.ToInt32(timeArray[i]); break;
                }
            }
        }
        public Time(DateTime time)
        {
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
            this.MilliSecond = time.Millisecond;
        }
        public int Hour { get; set; }

        public int Minute { get; set; }

        public int Second { get; set; }

        public int MilliSecond { get; set; }

        public string ToShortTimeString()
        {
            return String.Format("{0}:{1}:{2}", this.Hour, this.Minute, this.Second);
        }
        public string ToLongTimeString()
        {
            return String.Format("{0}:{1}:{2}:{3}", this.Hour, this.Minute, this.Second, this.MilliSecond);
        }
        public bool isGreaterThan(DateTime time)
        {
            if (this.Hour > time.Hour)
                return true;
            else if (this.Hour == time.Hour && this.Minute > time.Minute)
                return true;
            else if (this.Hour == time.Hour && this.Minute == time.Minute && this.Second > time.Second)
                return true;
            else if (this.Hour == time.Hour && this.Minute == time.Minute && this.Second == time.Second && this.MilliSecond > time.Millisecond)
                return true;
            else
                return false;
        }
        public bool isLessThan(DateTime time)
        {
            return !isGreaterThan(time);
        }
    }
}
