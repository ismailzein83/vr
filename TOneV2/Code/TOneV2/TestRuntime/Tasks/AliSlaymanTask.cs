using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
//using Vanrise.DatabaseStructure;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using System.Linq;
using Vanrise.Common;

namespace TestRuntime.Tasks
{
    class AliSlaymanTask : ITask
    {
        public void Execute()
        {
            //VRDBStructure structure = GetMSSQLDBStructure(".\\MSSQL2014", "SlaymanDB", "sa", "p@ssw0rd");
            //Console.ReadKey();
        }


        private void BPRegulatorTest()
        {
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }

        //private VRDBStructure GetMSSQLDBStructure(string Server = "192.168.110.185", string Database = "SecurityModel", string UserID = "Development", string Password = "dev!123")
        //{
        //    return new VRDBStructureManager().GetMSSQLDBStructure(Server, Database, UserID, Password);
        //}
    }
}