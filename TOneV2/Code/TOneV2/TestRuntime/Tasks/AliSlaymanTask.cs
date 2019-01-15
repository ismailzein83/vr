//using System;
//using System.Collections.Generic;
//using TOne.WhS.RouteSync.Entities;
//using TOne.WhS.RouteSync.Ericsson;
//using TOne.WhS.Routing.Entities;
//using Vanrise.BusinessProcess;
//using Vanrise.Caching.Runtime;
//using Vanrise.Common.Business;
//using Vanrise.Queueing;
//using Vanrise.Runtime;
//using Vanrise.Runtime.Entities;
//using System.Linq;
//using Vanrise.Common;
//using Vanrise.DatabaseStructure;
//using Vanrise.DatabaseStructure.DBConvertors;
//using static Vanrise.DatabaseStructure.VRDBStructureManager;

//namespace TestRuntime.Tasks
//{
//    class AliSlaymanTask : ITask
//    {
//        public void Execute()
//        {
//            //VRDBStructure structure = GetMSSQLDBStructure("192.168.110.185", "TOneConfiguration", "Development", "dev!123");
//            VRDBStructure structure = GetMSSQLDBStructure();
//            string script = GetMySqlScript(structure);

//            //List<test> tests = new List<test> {
//            //new test() {  id = 1 , name = "Ali" },
//            //new test() {  id = 2 , name = "Alia" },
//            //new test() {  id = 3 , name = "Alie" },
//            //new test() {  id = 4 , name = "Alio" }
//            //};

//            //string primaryColumnKeys = string.Join(",", tests.Select(item => item.name).ToList());

//            Console.ReadKey();
//        }

//        class test
//        {
//            public int id { get; set; }
//            public string name { get; set; }
//        }


//        private void BPRegulatorTest()
//        {
//            var runtimeServices = new List<RuntimeService>();

//            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
//            runtimeServices.Add(bpService);

//            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
//            runtimeServices.Add(bpRegulatorRuntimeService);

//            RuntimeHost host = new RuntimeHost(runtimeServices);
//            host.Start();
//        }

//        private VRDBStructure GetMSSQLDBStructure(string Server = "192.168.110.185", string Database = "SecurityModel", string UserID = "Development", string Password = "dev!123")
//        {
//            return new VRDBStructureManager().GetMSSQLDBStructure(Server, Database, UserID, Password);
//        }

//        private string GetMySqlScript(VRDBStructure vrdbStructure)
//        {
//            VRDBStructureConvertorGenerateConvertedScriptContext vRDBStructureConvertorGenerateConvertedScriptContext = new VRDBStructureConvertorGenerateConvertedScriptContext(vrdbStructure);
//            MySqlDBConvertor mySqlDBConvertor = new MySqlDBConvertor();
//            return mySqlDBConvertor.GenerateConvertedScript(vRDBStructureConvertorGenerateConvertedScriptContext);
//        }

//    }
//}