using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        DateTime _dateToRun;
        public DateTime DateToRun
        {
            get
            {
                return _dateToRun;
            }
            set
            {
                _dateToRun = value.ToLocalTime();
            }
        }

        public string TimeToRun { get; set; }

        public override bool CheckIfTimeToRun()
        {
            string[] timeParts = TimeToRun.Split(':');

            Console.WriteLine("Original Date to run {0}", DateToRun);

            DateToRun = DateToRun.Date;

            Console.WriteLine("Original Date to run {0}", DateToRun);

            if (timeParts.Length > 0)
            {
                DateToRun = DateToRun.AddHours(double.Parse(timeParts[0]));

                if (timeParts.Length > 1 && timeParts[1] != null)
                {
                    DateToRun = DateToRun.AddMinutes(double.Parse(timeParts[1]));
                }

                if (timeParts.Length > 2 && timeParts[2] != null)
                {
                    DateToRun = DateToRun.AddSeconds(double.Parse(timeParts[2]));
                }
            }

            bool isTimeToRun = DateToRun.Date.Equals(DateTime.Now.Date)
                && DateToRun.Hour.Equals(DateTime.Now.Hour)
                && DateToRun.Minute.Equals(DateTime.Now.Minute);

            Console.WriteLine("Hours to add are", timeParts[0]);

            Console.WriteLine("DatetoRun is {0}", DateToRun);
            
            Console.WriteLine("DatetoRun Date is {0}", DateToRun.Date);
            Console.WriteLine("DatetoRun Hour is {0}", DateToRun.Hour);
            Console.WriteLine("DatetoRun Minute is {0}", DateToRun.Minute);

            Console.WriteLine("Current Date is {0}", DateTime.Now.Date);
            Console.WriteLine("Current Hour is {0}", DateTime.Now.Hour);
            Console.WriteLine("Current Minute is {0}", DateTime.Now.Minute);
            
            Console.WriteLine("Is Time to run {0}: ", isTimeToRun);

            return isTimeToRun;
        }

    }
}
