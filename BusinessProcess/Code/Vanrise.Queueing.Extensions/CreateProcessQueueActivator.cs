using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Extensions
{
    public class CreateProcessQueueActivator : QueueActivator
    {
        public string ProcessName { get; set; }

        public object ProcessInputArguments { get; set; }

        public override void Run(QueueInstance queueInstance)
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new BusinessProcess.Entities.CreateProcessInput
                {
                    ProcessName = this.ProcessName,
                    InputArguments = this.ProcessInputArguments
                });
        }

        public override void OnDisposed()
        {
        }
    }
}