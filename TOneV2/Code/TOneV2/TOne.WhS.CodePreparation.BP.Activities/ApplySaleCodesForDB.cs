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
using TOne.WhS.CodePreparation.Entities;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplySaleCodesForDB : CodeActivity
    {
        public InArgument<Dictionary<string, Zone>> AffectedZonesWithCodes { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            CodePreparationManager codePreparationManager = new CodePreparationManager();
            List<Code> codesToDelete = new List<Code>();
            List<Code> codesToAdd = new List<Code>();

            Dictionary<string, Zone> affectedZonesWithCodes = AffectedZonesWithCodes.Get(context);

            foreach (var saleZone in affectedZonesWithCodes)
            {
                if (saleZone.Value.Codes != null)
                {
                    foreach (var code in saleZone.Value.Codes)
                    {
                        if (code.Status == Status.New)
                        {
                            codesToAdd.Add(code);
                        }
                        else if (code.Status == Status.Changed)
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
