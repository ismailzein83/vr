﻿using System;
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
        public OutArgument<SwitchRouteSyncInitializationData> InitializationData { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var switchRouteSynchronizerInitializeContext = new SwitchRouteSynchronizerInitializeContext { RouteRangeType = this.RouteRangeType.Get(context) };
            this.Switch.Get(context).RouteSynchronizer.Initialize(switchRouteSynchronizerInitializeContext);
            this.InitializationData.Set(context, switchRouteSynchronizerInitializeContext.InitializationData);
        }

        #region Private Classes

        private class SwitchRouteSynchronizerInitializeContext : ISwitchRouteSynchronizerInitializeContext
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
