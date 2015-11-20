using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyDataForDB : CodeActivity
    {
        public InArgument<Dictionary<string, Zone>> AffectedZonesWithCodes { get; set; }
        public InArgument<int> SellingNumberPlanId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {

            DateTime startReading = DateTime.Now;
            CodePreparationManager codePreparationManager = new CodePreparationManager();
            Dictionary<string, Zone> affectedZonesWithCodes = AffectedZonesWithCodes.Get(context);
            codePreparationManager.InsertCodePreparationObject(affectedZonesWithCodes, SellingNumberPlanId.Get(context));
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply Data For DB done and Takes: {0}", spent);
        }
    }
}
