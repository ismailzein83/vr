using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public sealed class GetSwitchSyncData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<SwitchInfo>> Switches { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<string, SwitchSyncData>> SwitchSyncDataBySwitchId { get; set; } 

        protected override void Execute(CodeActivityContext context)
        {
            List<SwitchInfo> switches = context.GetValue(this.Switches);
            if (switches == null || switches.Count == 0)
                return;

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            ISwitchSyncDataDataManager switchSyncDataDataManager = RoutingDataManagerFactory.GetDataManager<ISwitchSyncDataDataManager>();
            switchSyncDataDataManager.RoutingDatabase = routingDatabase;

            List<string> switchIds = switches.Select(itm => itm.SwitchId).ToList();
            List<SwitchSyncData> switchSyncDataList = switchSyncDataDataManager.GetSwitchSyncDataByIds(switchIds);
            Dictionary<string, SwitchSyncData> switchSyncDataDict = switchSyncDataList.ToDictionary(itm => itm.SwitchId, itm => itm);
            
            this.SwitchSyncDataBySwitchId.Set(context, switchSyncDataDict);
        }
    }
}