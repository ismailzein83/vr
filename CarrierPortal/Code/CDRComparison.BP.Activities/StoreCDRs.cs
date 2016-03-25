using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using CDRComparison.Entities;
using Vanrise.BusinessProcess;

namespace CDRComparison.BP.Activities
{

    public sealed class StoreCDRs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CDRSource> CDRSource { get; set; }

        public InArgument<bool> IsPartnerCDRs { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            bool isPartnerCDRs = this.IsPartnerCDRs.Get(context);
            context.WriteTrackingMessage(Vanrise.Common.LogEntryType.Information, "Started reading {0} CDRs", (isPartnerCDRs) ? "partner" : "system");

            CDRSource cdrSource = this.CDRSource.Get(context);
            var cdrs = new List<CDR>();
            context.WriteTrackingMessage(Vanrise.Common.LogEntryType.Information, "Read {0} CDRs from source", cdrs.Count);

            context.WriteTrackingMessage(Vanrise.Common.LogEntryType.Information, "Done", cdrs.Count);
        }
    }
}
