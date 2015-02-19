using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class SetRoutingDatabaseReady : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingDatabaseDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            dataManager.SetReady(this.RoutingDatabaseId.Get(context));
        }
    }
}
