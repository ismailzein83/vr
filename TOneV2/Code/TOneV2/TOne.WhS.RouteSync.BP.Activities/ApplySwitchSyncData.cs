using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public sealed class ApplySwitchSyncData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<List<SwitchInfo>> Switches { get; set; }

        [RequiredArgument]
        public InArgument<int> LastVersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> SwitchSyncOutputDict { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<SwitchInfo> switches = context.GetValue(this.Switches);
            int lastVersionNumber = context.GetValue(this.LastVersionNumber);
            int routingDatabaseId = context.GetValue(this.RoutingDatabaseId);
            ConcurrentDictionary<string, SwitchSyncOutput> switchSyncOutputDict = context.GetValue(this.SwitchSyncOutputDict);

            List<string> switchIds = switches.FindAll(itm => !switchSyncOutputDict.ContainsKey(itm.SwitchId) || switchSyncOutputDict[itm.SwitchId].SwitchSyncResult == SwitchSyncResult.Succeed)
                                             .Select(itm => itm.SwitchId).ToList();

            if (switchIds.Count == 0)
                return;

            ISwitchSyncDataDataManager switchSyncDataDataManager = RoutingDataManagerFactory.GetDataManager<ISwitchSyncDataDataManager>();
            switchSyncDataDataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(routingDatabaseId); ;
            switchSyncDataDataManager.ApplySwitchesSyncData(switchIds, lastVersionNumber);
        }
    }
}