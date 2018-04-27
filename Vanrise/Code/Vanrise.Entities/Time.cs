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
        static Time()
        {
            new Time();
            string[] parameters = new string[] { "Hour", "Minute", "Second", "MilliSecond" };
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Time), false).Add(parameters);
        }

        public Time() { }
        public Time(string time)
        {
            string[] dotParts = time.Split('.');
            
            if (dotParts.Length >= 1)
            {
                string[] timeArray = dotParts[0].Split(':');
                int length = timeArray.Length;
                if (length >= 1)
                {
                    int hour;
                    if (TryParseTimePart(timeArray[0], 23, out hour))
                        this.Hour = hour;
                    else
                        throw new Exception(String.Format("Invalid hour in Time '{0}'", time));

                    if (length >= 2)
                    {
                        int minute;
                        if (TryParseTimePart(timeArray[1], 59, out minute))
                            this.Minute = minute;
                        else
                            throw new Exception(String.Format("Invalid minute in Time '{0}'", time));

                        if (length >= 3)
                        {
                            int second;
                            if (TryParseTimePart(timeArray[2], 59, out second))
                                this.Second = second;
                            else
                                throw new Exception(String.Format("Invalid second in Time '{0}'", time));
                        }
                    }
                }

                if (dotParts.Length >= 2)
                {
                    int millisecond;
                    if (TryParseTimePart(dotParts[1], 999999999, out millisecond))
                        this.MilliSecond = millisecond;
                    else
                        throw new Exception(String.Format("Invalid millisecond in Time '{0}'", time));
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

        public Time(int hour, int minute, int second, int millisecond)
        {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.MilliSecond = millisecond;
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

        private bool TryParseTimePart(string timePart, int maxValue, out int value)
        {
            if (int.TryParse(timePart, out value) && value >= 0 && value <= maxValue)
                return true;
            else
                return false;
        }

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
