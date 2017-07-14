using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Retail.Cost.Entities;
using Retail.Cost.Business;

namespace Retail.Cost.BP.Activities
{
    public sealed class UpadeOverridenCDRCost : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> FirstDayToReprocess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime firstDayToReprocess = context.GetValue(this.FirstDayToReprocess);

            CDRCostManager cdrCostManager = new CDRCostManager();
            cdrCostManager.UpadeOverridenCostCDRAfterDate(firstDayToReprocess);

            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Upade Overriden CDR Cost is done", null);
        }
    }
}