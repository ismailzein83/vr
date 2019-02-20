using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.Business
{
    public class SMSInvoiceUIGridColumnFilter : InvoiceUIGridColumnFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("15ceed9b-0c8f-4446-8bbe-92e84857ae31"); }
        }

        public override bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context)
        {
            Guid smsAnalyticTableId = new Guid("c1bd3f2f-6213-44d1-9d58-99f81e169930");
            RetailModuleManager retailModuleManager = new RetailModuleManager();
            return retailModuleManager.IsSMSModuleEnabled(smsAnalyticTableId);
        }
    }
}
