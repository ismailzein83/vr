﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TestRuntime
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Host Started");
            BusinessProcessRuntime.Current.TerminatePendingProcesses();
            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            //////System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
            //////    {
            //////        for (DateTime d = DateTime.Parse(ConfigurationManager.AppSettings["RepricingFrom"]); d <= DateTime.Parse(ConfigurationManager.AppSettings["RepricingTo"]); d = d.AddDays(1))
            //////        {
            //////            TriggerProcess(d);
            //////            System.Threading.Thread.Sleep(30000);
            //////        }
            //////    });
            //////t.Start();

            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "UpdateCodeZoneMatchProcess",
                InputArguments = new TOne.LCRProcess.Arguments.UpdateCodeZoneMatchProcessInput
                {
                    IsFuture = false,
                    CodeEffectiveOn = DateTime.Today,
                    GetChangedCodeGroupsOnly = false
                }
            });
            
            //processManager.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "UpdateZoneRateProcess"
            //});
        }

        static bool _isRunning;
        static object _lockObj = new object();
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            try
            {
                //BusinessProcessRuntime.Current.LoadAndExecutePendings();

                BusinessProcessRuntime.Current.ExecutePendings();
                BusinessProcessRuntime.Current.TriggerPendingEvents();
            }
            finally
            {
                lock (_lockObj)
                {
                    _isRunning = false;
                }
            }
        }

        private static void TriggerProcess(DateTime date)
        {
            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = date };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "DailyRepricingProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);
        }
    }
}
