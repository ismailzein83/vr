using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
     public partial class Subscriber_Values
    {
         public static List<Subscriber_Values> GetList(int strategyId, string SubscriberNumber)
         {
             //if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(Description))
             //    return GetAll();

             List<Subscriber_Values> subscriber_Values = new List<Subscriber_Values>();
             try
             {
                 using (Entities context = new Entities())
                 {
                     subscriber_Values = context.Subscriber_Values
                         .Include(s => s.Criteria_Profile)
                         .Where(s =>
                             (s.StrategyId == strategyId && s.SubscriberNumber == SubscriberNumber)


                         )
                         .ToList();
                 }
             }
             catch (Exception err)
             {
                 FileLogger.Write("DataLayer.Subscriber_Values.GetList()", err);
             }
             return subscriber_Values;
         }



    }
}
