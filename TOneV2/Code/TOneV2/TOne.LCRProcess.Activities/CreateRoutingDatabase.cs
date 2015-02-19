﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class CreateRoutingDatabase : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabaseType> Type { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public OutArgument<int> DatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingDatabaseDataManager routingDatabaseManager = LCRDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            int databaseId = routingDatabaseManager.CreateDatabase(String.Format("{0}_{1:yyyyMMdd-HHmm}", this.Type.Get(context), this.EffectiveTime.Get(context)), this.Type.Get(context), this.EffectiveTime.Get(context));
            this.DatabaseId.Set(context, databaseId);
        }
    }
}
