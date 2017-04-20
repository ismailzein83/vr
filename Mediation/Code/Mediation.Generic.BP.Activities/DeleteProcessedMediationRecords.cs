using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Data;

namespace Mediation.Generic.BP.Activities
{

    public sealed class DeleteProcessedMediationRecords : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<string>> EventIds { get; set; }

        [RequiredArgument]
        public InArgument<Guid> MediationDefinitionId { get; set; }

        [RequiredArgument]
        public InOutArgument<bool> IsDataDeleted { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            bool deleted = dataManager.DeleteMediationRecordsBySessionIds(MediationDefinitionId.Get(context), EventIds.Get(context));
            IsDataDeleted.Set(context, deleted);
        }
    }
}
