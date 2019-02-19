using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class VoiceInvoiceUIGridColumnFilter : InvoiceUIGridColumnFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("0fa2e4e4-3468-4a32-bedc-2c302300ae6b"); }
        }

        public override bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context)
        {
            TOneModuleManager toneModuleManager = new TOneModuleManager();
            return toneModuleManager.IsVoiceModuleEnabled();
        }
    }
}
