using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using TOne.LCR.Entities;

namespace TestServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            //////ServiceHost serviceHost = new ServiceHost(typeof(BulkTableService));
            //////NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            //////binding.MaxReceivedMessageSize = int.MaxValue;
            //////serviceHost.AddServiceEndpoint(typeof(IBulkTableService), binding, "net.pipe://localhost/BulkTableService");
            //////serviceHost.Opened += new EventHandler(serviceHost_Opened);
            //////serviceHost.Open();

            //LCRCode code = new LCRCode
            //{
            //    ID = 43,
            //    CodeGroup = "435235",
            //    SupplierId = "C$444",
            //    Value = "34"
            //};
            //ProcessManager processManager = new ProcessManager();
            //processManager.CreateNewProcess(new CreateProcessInput
            //            {
            //                InputArguments = code
            //            });

            Console.ReadKey();
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Service Opened");
        }
    }
}
