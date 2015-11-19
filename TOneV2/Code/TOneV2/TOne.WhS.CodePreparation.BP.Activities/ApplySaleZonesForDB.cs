using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplySaleZonesForDB : CodeActivity
    {
        public InArgument<Dictionary<string,SaleZone>> AllZones { get; set; }
       // public InArgument<Dictionary<string, SaleZone>> ZonesToDelete { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;

            CodePreparationManager codePreparationManager = new CodePreparationManager();
            Dictionary<string,SaleZone> allZones=AllZones.Get(context);

            List<SaleZone> zonesToDelete = new List<SaleZone>();
            List<SaleZone> zonesToAdd = new List<SaleZone>();

            foreach (var saleZone in allZones)
            {
                if (saleZone.Value.Status == Status.New)
                {
                    zonesToAdd.Add(saleZone.Value);
                }
                else if (saleZone.Value.Status == Status.Deleted)
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
