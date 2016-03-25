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
            var cdrSource = this.CDRSource.Get(context);
            List<CDR> cdrs = null;

            context.WriteTrackingMessage(Vanrise.Common.LogEntryType.Information, "{0} CDRs read", cdrs.Count);
            
        }
    }
}
