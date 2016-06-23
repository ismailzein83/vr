using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;

namespace Mediation.Generic.BP.Activities
{

    public sealed class GetStoreStagingEventIds : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<StoreStagingRecord>> StoreStagingRecords { get; set; }
        [RequiredArgument]
        public OutArgument<IEnumerable<int>> EventIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<StoreStagingRecord> storeStagingRecords = StoreStagingRecords.Get(context);
            EventIds.Set(context, storeStagingRecords.Select(s => s.EventId));
        }
    }
}
