using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ApplySupplierPriceListForDB:CodeActivity
    {
        public InArgument<int> SupplierAccountId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startApplying= DateTime.Now;
            SupplierPriceListManager manager = new SupplierPriceListManager();
            manager.AddSupplierPriceList(SupplierAccountId.Get(context));
            TimeSpan spent = DateTime.Now.Subtract(startApplying);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply Supplier PriceList For DB done and Takes: {0}", spent);
        }
    }
}
