using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class SMSInvoiceUIGridColumnFilter : InvoiceUIGridColumnFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("15ceed9b-0c8f-4446-8bbe-92e84857ae31"); }
        }

        public override bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context)
        {
            TOneModuleManager toneModuleManager = new TOneModuleManager();
            return toneModuleManager.IsSMSModuleEnabled();
        }
    }
}
