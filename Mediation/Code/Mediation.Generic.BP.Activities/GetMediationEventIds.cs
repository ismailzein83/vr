using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Mediation.Generic.Business;

namespace Mediation.Generic.BP.Activities
{

    public sealed class GetMediationEventIds : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<MediationRecord>> MediationRecords { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<string>> EventIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<MediationRecord> mediationRecords = MediationRecords.Get(context);
            EventIds.Set(context, mediationRecords.Select(s => s.SessionId).Distinct());
        }
    }
}
