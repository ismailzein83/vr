using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class InitializeSwitchRouteSync : CodeActivity
    {
        [RequiredArgument]
        public InArgument<SwitchInfo> Switch { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeType?> RouteRangeType { get; set; }

        [RequiredArgument]
        public OutArgument<Object> InitializationData { get; set; }

        [RequiredArgument]
        public OutArgument<RouteSyncDeliveryMethod> SupportedDeliveryMethod { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var switchRouteSynchronizerInitializeContext = new SwitchRouteSynchronizerInitializeContext { RouteRangeType = this.RouteRangeType.Get(context) };
            this.Switch.Get(context).RouteSynchronizer.Initialize(switchRouteSynchronizerInitializeContext);
            this.InitializationData.Set(context, switchRouteSynchronizerInitializeContext.InitializationData);
            this.SupportedDeliveryMethod.Set(context, switchRouteSynchronizerInitializeContext.SupportedDeliveryMethod);
        }

        #region Private Classes

        private class SwitchRouteSynchronizerInitializeContext : ISwitchRouteSynchronizerInitializeContext
        {
            public RouteRangeType? RouteRangeType
            {
                get;
                set;
            }

            public object InitializationData
            {
                get;
                set;
            }

            public RouteSyncDeliveryMethod SupportedDeliveryMethod
            {
                get;
                set;
            }
        }


        #endregion
    }
}
