using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
//using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using TOne.WhS.DBSync.Business;
using TOne.WhS.DBSync.Business.SwitchMigration;
using TOne.WhS.DBSync.Entities;

namespace TestRuntime
{
    public class ZeinabTask : ITask
    {
        public void Execute()
        {
            SwitchMappingRulesMigrator migrator = new SwitchMappingRulesMigrator("Server=192.168.110.185;Database=NettalkFidaa;User ID=Development;Password=dev!123");
            migrator.LoadSwitches();
            migrator.Migrate("3", "Teles", new DateTime(2016, 08, 16));
            //SwitchMappingRulesManager switchMapping = new SwitchMappingRulesManager();
            //List<TOne.WhS.DBSync.Entities.SwitchMappingRules> switches = switchMapping.LoadSwitches();
            //string parser = "teles";
            //foreach (var item in switches)
            //{
            //    List<InParsedMapping> inParsedMappings;
            //    List<OutParsedMapping> outParsedMappings;
            //    switch (item.Name.ToLower())
            //    {
            //        case "teles":
            //            TelesSwitchParser telesSwitchParser = new TelesSwitchParser();
            //            telesSwitchParser.GetParsedMappings(item.Configuration, out inParsedMappings, out outParsedMappings);
            //            break;
            //    }
            //}

            //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            ////QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            //var runtimeServices = new List<RuntimeService>();
            ////runtimeServices.Add(queueActivationService);

            //runtimeServices.Add(bpService);

            //RuntimeHost host = new RuntimeHost(runtimeServices);
            //host.Start();

            ////BPClient bpClient = new BPClient();
            ////bpClient.CreateNewProcess(new CreateProcessInput
            ////{
            ////    InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
            ////    {
            ////        EffectiveTime = DateTime.Now,
            ////        IsFuture = false
            ////    }
            ////});
        }
    }
}
