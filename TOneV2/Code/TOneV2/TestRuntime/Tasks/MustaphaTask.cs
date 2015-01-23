using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TestRuntime.Tasks
{
    public class MustaphaTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Host Started");
            BusinessProcessRuntime.Current.TerminatePendingProcesses();

            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = DateTime.Parse("2013-05-02 00:00:00.000") };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "DailyRepricingProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);

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
    }
}
