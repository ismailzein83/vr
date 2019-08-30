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

namespace TestRuntime.Tasks
{
    public class KalakeshTask : ITask
    {
        public void Execute()
        {
            //LastTimePeriod lastTimePeriod = new LastTimePeriod();
            ////lastTimePeriod.StartingFrom = StartingFrom.ExecutionTime;
            ////lastTimePeriod.StartingFrom = StartingFrom.Midnight;
            //lastTimePeriod.StartingFrom = StartingFrom.ExecutionTimeWithOffset;
            //lastTimePeriod.TimeUnit = TimeUnit.Day;
            //lastTimePeriod.TimeValue = 10;
            //lastTimePeriod.OffsetTimeUnit = TimeUnit.Day;
            //lastTimePeriod.OffsetValue = 10; 
            //VRTimePeriodContext context = new VRTimePeriodContext();
            //context.EffectiveDate = DateTime.Now;

            //lastTimePeriod.GetTimePeriod(context);

            //Console.Write("From: ");
            //Console.WriteLine(context.FromTime);
            //Console.Write("To: ");
            //Console.WriteLine(context.ToTime);
        }
    }
}