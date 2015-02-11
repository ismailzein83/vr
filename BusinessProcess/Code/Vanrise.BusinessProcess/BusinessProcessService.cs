using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;

namespace Vanrise.BusinessProcess
{
    public class BusinessProcessService : RuntimeService
    {
        protected override void OnStarted()
        {
            BusinessProcessRuntime.Current.TerminatePendingProcesses();
            base.OnStarted();
        }
        protected override void Execute()
        {
            BusinessProcessRuntime.Current.ExecutePendingsIfIdleAsync();
            BusinessProcessRuntime.Current.TriggerPendingEventsIfIdleAsync();
        }
    }
}
