using Microsoft.CSharp.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common.MainExtensions;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;

namespace TestRuntime.Tasks
{
    public class KalakeshTask : ITask
    {
        public void Execute()
        {

            #region Last Time Period

            //while (true)
            //{
            //    LastTimePeriod lastTimePeriod = new LastTimePeriod();

            //    Console.Write("Starting from: 0.ExecutionTime - 1.Midnight - 2.ExecutionTimeWithOffset  -->  ");
            //    int startingFrom = Convert.ToInt32(Console.ReadLine());
            //    switch (startingFrom)
            //    {
            //        case 0: lastTimePeriod.StartingFrom = StartingFrom.ExecutionTime; break;
            //        case 1: lastTimePeriod.StartingFrom = StartingFrom.Midnight; break;
            //        case 2: lastTimePeriod.StartingFrom = StartingFrom.ExecutionTimeWithOffset; break;
            //        default: return;
            //    }

            //    Console.Write("Time unit: 0.Year - 1.Month - 2.Day - 3.Hour - 4.Minute  -->  ");
            //    int timeUnit = Convert.ToInt32(Console.ReadLine());
            //    switch (timeUnit)
            //    {
            //        case 0: lastTimePeriod.TimeUnit = TimeUnit.Year; break;
            //        case 1: lastTimePeriod.TimeUnit = TimeUnit.Month; break;
            //        case 2: lastTimePeriod.TimeUnit = TimeUnit.Day; break;
            //        case 3: lastTimePeriod.TimeUnit = TimeUnit.Hour; break;
            //        case 4: lastTimePeriod.TimeUnit = TimeUnit.Minute; break;
            //        default: return;
            //    }

            //    Console.Write("Time value in " + lastTimePeriod.TimeUnit.ToString() + "  -->  ");
            //    int timeValue = Convert.ToInt32(Console.ReadLine());
            //    lastTimePeriod.TimeValue = timeValue;

            //    Console.Write("Offset time unit: 0.Year - 1.Month - 2.Day - 3.Hour - 4.Minute  -->  ");
            //    int offsetTimeUnit = Convert.ToInt32(Console.ReadLine());
            //    switch (offsetTimeUnit)
            //    {
            //        case 0: lastTimePeriod.OffsetTimeUnit = TimeUnit.Year; break;
            //        case 1: lastTimePeriod.OffsetTimeUnit = TimeUnit.Month; break;
            //        case 2: lastTimePeriod.OffsetTimeUnit = TimeUnit.Day; break;
            //        case 3: lastTimePeriod.OffsetTimeUnit = TimeUnit.Hour; break;
            //        case 4: lastTimePeriod.OffsetTimeUnit = TimeUnit.Minute; break;
            //        default: return;
            //    }

            //    Console.Write("Offset value in " + lastTimePeriod.OffsetTimeUnit.ToString() + "  -->  ");
            //    int offsetValue = Convert.ToInt32(Console.ReadLine());
            //    lastTimePeriod.OffsetValue = offsetValue;

            //    DateTime effectiveDate = DateTime.Now;
            //    VRTimePeriodManager timePeriodManager = new VRTimePeriodManager();
            //    DateTimeRange dateTimeRange = timePeriodManager.GetTimePeriod(lastTimePeriod, effectiveDate);
            //    Console.WriteLine("From: " + dateTimeRange.From);
            //    Console.WriteLine("To: " + dateTimeRange.To);
            //    Console.WriteLine("----------------------------------------------------");

            //    VRTimePeriodContext context = new VRTimePeriodContext();
            //    context.EffectiveDate = DateTime.Now;

            //    try
            //    {
            //        lastTimePeriod.GetTimePeriod(context);
            //        Console.WriteLine();
            //        Console.Write("From: ");
            //        Console.WriteLine(context.FromTime);
            //        Console.Write("To: ");
            //        Console.WriteLine(context.ToTime);
            //        Console.WriteLine();
            //        Console.WriteLine("----------------------------------------------------");
            //        Console.WriteLine();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Exception raised: " + e.Message);
            //        return;
            //    }
            //}

            #endregion

            #region Cataleya Rerouted CDR Fields

            //string reroutedCDRText = "zone_id=263 call_id=65998754-4a589d4d-44dd10-7fc8709f7280-fcc11851-13c4-7225 zone_name=Nettalk sip_remote_addr=sip:212.58.25.200:5060;transport=UDP inviting_ts=1557146082 response_ts=1557146083 failure_reason=503 reason_phrase=Service Unavailable(3006) reason_hdr=Q.850;cause=41;text='NORMAL_TEMPORARY_FAILURE' warning_hdr= tgid= calling_party=880 pi=allowed extra_data=";
            //List<string> reroutedCDRFields = new List<string>()
            //{
            //    "zone_id", "call_id", "zone_name", "sip_remote_addr", "inviting_ts", "response_ts", "failure_reason",
            //    "reason_phrase", "reason_hdr", "warning_hdr", "tgid", "calling_party", "pi", "extra_data"
            //};
            //Dictionary<int, string> indexOfFieldMapping = new Dictionary<int, string>();
            //Dictionary<string, string> fieldNameValueMapping = new Dictionary<string, string>();

            //foreach(var field in reroutedCDRFields)
            //{
            //    indexOfFieldMapping.Add(reroutedCDRText.IndexOf(field), field);
            //}

            //foreach(var kvp in indexOfFieldMapping)
            //{
            //    int start = kvp.Key + kvp.Value.Length + 1;
            //    int end = start;
            //    while (true)
            //    {
            //        if (indexOfFieldMapping.ContainsKey(end) || end > reroutedCDRText.Length)
            //            break;
            //        end++;
            //    }
            //    end--;

            //    if(end > start)
            //    {
            //        fieldNameValueMapping.Add(kvp.Value, reroutedCDRText.Substring(start, end - start));
            //    }
            //    else
            //    {
            //        fieldNameValueMapping.Add(kvp.Value, null);
            //    }
            //}

            //foreach(var field in reroutedCDRFields)
            //{
            //    Console.WriteLine(field + ": " + fieldNameValueMapping[field]);
            //}

            #endregion

        }
    }
}
