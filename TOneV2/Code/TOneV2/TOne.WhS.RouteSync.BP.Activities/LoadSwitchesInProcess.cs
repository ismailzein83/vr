using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Business;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class LoadSwitchesInProcess : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<string>> SwitchIds { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, SwitchRouteSyncInitializationData>> SwitchesInitializationData { get; set; }

        [RequiredArgument]
        public InArgument<Guid?> SwitchesInitializationDataId { get; set; }

        [RequiredArgument]
        public OutArgument<List<SwitchInProcess>> SwitchesInProcess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<string, SwitchRouteSyncInitializationData> switchesInitializationData = this.SwitchesInitializationData.Get(context);
            if (switchesInitializationData == null)
            {
                Guid? switchesInitializationDataId = this.SwitchesInitializationDataId.Get(context);
                if (!switchesInitializationDataId.HasValue)
                    throw new ArgumentNullException("switchesInitializationDataId");
                switchesInitializationData = LoadSwitchesInitialiationData(switchesInitializationDataId.Value);
            }

            List<SwitchInfo> switches = new SwitchManager().GetSwitches(this.SwitchIds.Get(context));

            List<SwitchInProcess> switchesInProcess = new List<SwitchInProcess>();
            foreach (var switchInfo in switches)
            {
                SwitchRouteSyncInitializationData initializationData;
                switchesInitializationData.TryGetValue(switchInfo.SwitchId, out initializationData);
                switchesInProcess.Add(new SwitchInProcess
                    {
                        Switch = switchInfo,
                        InitializationData = initializationData
                    });
            }
            this.SwitchesInProcess.Set(context, switchesInProcess);
        }

        private Dictionary<string, SwitchRouteSyncInitializationData> LoadSwitchesInitialiationData(Guid switchesInitializationDataId)
        {
            return new Dictionary<string, SwitchRouteSyncInitializationData>();
        }
    }
}
