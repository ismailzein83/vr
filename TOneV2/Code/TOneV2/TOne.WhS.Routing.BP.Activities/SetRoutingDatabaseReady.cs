﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{

    public sealed class SetRoutingDatabaseReady : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            routingDatabaseManager.SetReady(this.RoutingDatabaseId.Get(context));
        }
    }
}

