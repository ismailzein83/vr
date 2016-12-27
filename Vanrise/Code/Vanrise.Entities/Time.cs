using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Time
    {
        #region ctor/Local Variables
       
        public Time() { }
        public Time(string time)
        {
            string[] timeArray = time.Split(':');
            for (var i = 0; i < timeArray.Length; i++)
            {
                switch (i)
                {
                    case 0: this.Hour = Int32.Parse(timeArray[i]); break;
                    case 1: this.Minute = Int32.Parse(timeArray[i]); break;
                    case 2: this.Second = Int32.Parse(timeArray[i]); break;
                    case 3: this.MilliSecond = Int32.Parse(timeArray[i]); break;
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
      
        #endregion

        #region Public Methods
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int MilliSecond { get; set; }
        public string ToShortTimeString()
        {
            return String.Format("{0:00}:{1:00}:{2:00}", this.Hour, this.Minute, this.Second);
        }
        public string ToLongTimeString()
        {
            return String.Format("{0:00}:{1:00}:{2:00}:{3:00}", this.Hour, this.Minute, this.Second, this.MilliSecond);
        }
        public bool GreaterThan(DateTime date)
        {
            if (date == null)
            {
                throw new ArgumentException("Date should not be null");
            }
            return CompareTime(date.Hour, date.Minute, date.Second, date.Millisecond);
        }
        public bool GreaterThan(Time time)
        {
            if (time == null)
            {
                throw new ArgumentException("Time should not be null");
            }
            return CompareTime(time.Hour, time.Minute, time.Second, time.MilliSecond);
        }
        public bool LessThan(DateTime date)
        {
            return !GreaterThan(date);
        }
        public bool LessThan(Time time)
        {
            return !GreaterThan(time);
        }
        public bool Equals(Time time)
        {
            return (time.Hour == Hour && time.Minute == Minute && time.Second == Second && time.MilliSecond == MilliSecond);
        }
        #endregion

        #region Private Methods
        private bool CompareTime(int hour, int minute, int second, int milliSecond)
        {
            if (this.Hour > hour)
                return true;
            else if (this.Hour == hour && this.Minute > minute)
                return true;
            else if (this.Hour == hour && this.Minute == minute && this.Second > second)
                return true;
            else if (this.Hour == hour && this.Minute == minute && this.Second == second && this.MilliSecond > milliSecond)
                return true;
            else
                return false;
        }
      
        #endregion
    }

    
}
