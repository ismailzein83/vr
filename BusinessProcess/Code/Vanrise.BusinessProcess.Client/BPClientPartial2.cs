using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Vanrise.BusinessProcess.Client
{
    public  partial class  BPClient
    {
        #region Process Workflow Methods


        public void StartInstance(string processName, object inputArguments)
        {
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Task t = new Task(() =>
            {
                BPClient bpClient3 = new BPClient();
                bpClient3.CreateNewProcess(new CreateProcessInput { InputArguments = inputArguments as BaseProcessInputArgument });
            });
            t.ContinueWith((tt) =>
            {
            });
            t.Start();
        }



        #endregion
    }
}
