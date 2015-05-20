using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class NumberProfile
    {

        public static List<NumberProfile> GetDailyList(string subscriberNumber, DateTime? fromdate, DateTime? toDate)
       {
           List<NumberProfile> numberProfiles = new List<NumberProfile>();
           try
           {
               using (Entities context = new Entities())
               {
                   numberProfiles = context.NumberProfiles
                      .Where(s =>
                           (s.SubscriberNumber == subscriberNumber && s.Day_Hour == 25 && s.Date_Day >= fromdate && s.Date_Day <= toDate)
                       )
                       .ToList();
            }
           }
           catch (Exception err)
           {
               FileLogger.Write("DataLayer.NumberProfiles.GetDailyList()", err);
           }
           return numberProfiles;
       }

        public static List<NumberProfile> GetMonthlyList(string subscriberNumber, DateTime? fromdate, DateTime? toDate)
        {
            List<NumberProfile> numberProfiles = new List<NumberProfile>();
            try
            {
                using (Entities context = new Entities())
                {
                    numberProfiles = context.NumberProfiles
                       .Where(s =>
                            (s.SubscriberNumber == subscriberNumber && s.Day_Hour == 30 && s.Date_Day >= fromdate && s.Date_Day <= toDate)
                        )
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NumberProfiles.GetMonthlyList()", err);
            }
            return numberProfiles;
        }
        public static IEnumerable<NumberProfile> GetHourlyList(string subscriberNumber, DateTime? fromdate, DateTime? toDate)
        {
            IEnumerable<NumberProfile> numberProfiles = new List<NumberProfile>();
            try
            {
                using (Entities context = new Entities())
                {
                    numberProfiles = context.NumberProfiles
                       .Where(s =>
                            (s.SubscriberNumber == subscriberNumber && s.Day_Hour != 25 && s.Day_Hour != 30 && s.Date_Day >= fromdate && s.Date_Day <= toDate)
                        )
                        .ToList().Distinct();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.NumberProfiles.GetHourly()", err);
            }
            return numberProfiles;
        }







    }
}
