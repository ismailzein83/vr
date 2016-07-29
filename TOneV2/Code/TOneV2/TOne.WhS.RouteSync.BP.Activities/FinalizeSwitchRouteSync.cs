using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class FinalizeSwitchRouteSync : CodeActivity
    {
        [RequiredArgument]
        public InArgument<SwitchInfo> Switch { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeType?> RouteRangeType { get; set; }

        [RequiredArgument]
        public InArgument<SwitchRouteSyncInitializationData> InitializationData { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var switchRouteSynchronizerFinalizeContext = new SwitchRouteSynchronizerFinalizeContext { RouteRangeType = this.RouteRangeType.Get(context), InitializationData = this.InitializationData.Get(context) };
            this.Switch.Get(context).RouteSynchronizer.Finalize(switchRouteSynchronizerFinalizeContext);
        }

        #region Private Classes

        private class SwitchRouteSynchronizerFinalizeContext : ISwitchRouteSynchronizerFinalizeContext
        {
            public RouteRangeType? RouteRangeType
            {
                get;
                set;
            }

            public SwitchRouteSyncInitializationData InitializationData
            {
                get;
                set;
            }
        }


        #endregion
    }
}
