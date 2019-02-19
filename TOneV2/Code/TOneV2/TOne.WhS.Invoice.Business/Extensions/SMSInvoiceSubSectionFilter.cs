using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class SMSInvoiceSubSectionFilter : InvoiceSubSectionFilter
    {
        public override Guid ConfigId { get { return new Guid("C3240788-4078-4F24-83E4-51B36B1E42EC"); } }

        public override bool IsFilterMatch(IInvoiceSubSectionFilterContext context)
        {
            TOneModuleManager toneModuleManager = new TOneModuleManager();
            return toneModuleManager.IsSMSModuleEnabled();
        }
    }
}
