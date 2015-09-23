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
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplySaleCodesForDB : CodeActivity
    {
        public InArgument<List<SaleCode>> CodesToAdd { get; set; }
        public InArgument<List<SaleCode>> CodesToDelete { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime startReading = DateTime.Now;
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            saleCodeManager.DeleteSaleCodes(CodesToDelete.Get(context));
            saleCodeManager.InsertSaleCodes(CodesToAdd.Get(context));
            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply SaleCodes For DB done and Takes: {0}", spent);
        }
    }
}
