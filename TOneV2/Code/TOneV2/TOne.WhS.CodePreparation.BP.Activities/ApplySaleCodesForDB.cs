using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Business;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplySaleCodesForDB : CodeActivity
    {
        public InArgument<Dictionary<string, SaleZone>> AllZones { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            CodePreparationManager codePreparationManager = new CodePreparationManager();
            List<SaleCode> codesToDelete = new List<SaleCode>();
            List<SaleCode> codesToAdd = new List<SaleCode>();

            Dictionary<string, SaleZone> allZones = AllZones.Get(context);

            foreach (var saleZone in allZones)
            {
                if (saleZone.Value.Codes != null)
                {
                    foreach (var code in saleZone.Value.Codes)
                    {
                        if (code.Status == Status.New)
                        {
                            codesToAdd.Add(code);
                        }
                        else if (code.Status == Status.Deleted)
                        {
                            codesToDelete.Add(code);
                        }
                    }
                }
               
               
            }
            codePreparationManager.DeleteSaleCodes(codesToDelete);
            codePreparationManager.InsertSaleCodes(codesToAdd);
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply SaleCodes For DB done and Takes: {0}", spent);
        }
    }
}
