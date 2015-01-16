using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using TOne.Entities;

namespace TestServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(BulkTableService));
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            serviceHost.AddServiceEndpoint(typeof(IBulkTableService), binding, "net.pipe://localhost/BulkTableService");
            serviceHost.Opened += new EventHandler(serviceHost_Opened);
            serviceHost.Open();
            Console.ReadKey();
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Service Opened");
        }
    }
}
