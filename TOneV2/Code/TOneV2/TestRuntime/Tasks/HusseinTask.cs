using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
namespace TestRuntime.Tasks
{
    class HusseinTask:ITask
    {

        public void Execute()
        {
            Console.WriteLine("Host Started");
            BusinessProcessRuntime.Current.TerminatePendingProcesses();
            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "UpdateCodeZoneMatchProcess",
                InputArguments = new TOne.LCRProcess.Arguments.UpdateCodeZoneMatchProcessInput
                {
                    IsFuture = false,
                    CodeEffectiveOn = DateTime.Today,
                    //GetChangedCodeGroupsOnly = false
                }
            });


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
