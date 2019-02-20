using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.Business
{
    public class SMSInvoiceSubSectionFilter : InvoiceSubSectionFilter
    {
        public override Guid ConfigId { get { return new Guid("C3240788-4078-4F24-83E4-51B36B1E42EC"); } }

        public override bool IsFilterMatch(IInvoiceSubSectionFilterContext context)
        {
            Guid smsAnalyticTableId = new Guid("c1bd3f2f-6213-44d1-9d58-99f81e169930");
            RetailModuleManager retailModuleManager = new RetailModuleManager();
            return retailModuleManager.IsSMSModuleEnabled(smsAnalyticTableId);
        }
    }
}
