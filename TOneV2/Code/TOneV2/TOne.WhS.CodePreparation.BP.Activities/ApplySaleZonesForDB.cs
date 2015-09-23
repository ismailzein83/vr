using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplySaleZonesForDB : CodeActivity
    {
        public InArgument<List<SaleZone>> ZonesToAdd { get; set; }
        public InArgument<List<SaleZone>> ZonesToDelete { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            saleZoneManager.DeleteSaleZones(ZonesToDelete.Get(context));
            saleZoneManager.InsertSaleZones(ZonesToAdd.Get(context));
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply SaleZones For DB done and Takes: {0}", spent);
        }
    }
}
