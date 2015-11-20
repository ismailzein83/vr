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
    public class ApplySaleZonesForDB : CodeActivity
    {
        public InArgument<Dictionary<string, Zone>> AffectedZonesWithCodes { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;

            CodePreparationManager codePreparationManager = new CodePreparationManager();
            Dictionary<string, Zone> affectedZonesWithCodes = AffectedZonesWithCodes.Get(context);

            List<Zone> zonesToDelete = new List<Zone>();
            List<Zone> zonesToAdd = new List<Zone>();

            foreach (var saleZone in affectedZonesWithCodes)
            {
                if (saleZone.Value.Status == Status.New)
                {
                    zonesToAdd.Add(saleZone.Value);
                }
                else if (saleZone.Value.Status == Status.Changed)
                {
                    zonesToDelete.Add(saleZone.Value);
                }
            }
            codePreparationManager.DeleteSaleZones(zonesToDelete);
            codePreparationManager.InsertSaleZones(zonesToAdd);
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply SaleZones For DB done and Takes: {0}", spent);
        }
    }
}
