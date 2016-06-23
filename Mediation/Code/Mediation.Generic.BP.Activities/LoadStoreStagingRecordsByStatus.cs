using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;

namespace Mediation.Generic.BP.Activities
{

    public sealed class LoadStoreStagingRecordsByStatus : CodeActivity
    {
        [RequiredArgument]
        public InArgument<EventStatus> Status { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<StoreStagingRecord>> StoreStagingRecords { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            StoreStagingRecordsManager manager = new StoreStagingRecordsManager();
            IEnumerable<StoreStagingRecord> storeStagingRecords = manager.GetStoreStagingRecordsByStatus(Status.Get(context));
            StoreStagingRecords.Set(context, storeStagingRecords);
        }
    }
}
