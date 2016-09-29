using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;
using Mediation.Generic.Data;

namespace Mediation.Generic.BP.Activities
{

    public sealed class LoadMediationRecordsByStatus : CodeActivity
    {
        [RequiredArgument]
        public InArgument<EventStatus> Status { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DataRecordTypeId { get; set; }

        [RequiredArgument]
        public InArgument<int> MediationDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<MediationRecord>> MediationRecords { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.DataRecordTypeId = DataRecordTypeId.Get(context);
            MediationRecordsManager manager = new MediationRecordsManager();
            IEnumerable<MediationRecord> mediationRecords = manager.GetMediationRecordsByStatus(MediationDefinitionId.Get(context), Status.Get(context), DataRecordTypeId.Get(context));
            MediationRecords.Set(context, mediationRecords);
        }
    }
}
