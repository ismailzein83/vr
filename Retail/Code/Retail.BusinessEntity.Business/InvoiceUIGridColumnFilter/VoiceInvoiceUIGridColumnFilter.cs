using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.Business
{
    public class VoiceInvoiceUIGridColumnFilter : InvoiceUIGridColumnFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("0fa2e4e4-3468-4a32-bedc-2c302300ae6b"); }
        }

        public override bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context)
        {
            RetailModuleManager retailModuleManager = new RetailModuleManager();
            Guid voiceAnalyticTableId = new Guid("6cd535c0-ac49-46bb-aecf-0eae33823b20");
            return retailModuleManager.IsVoiceModuleEnabled(voiceAnalyticTableId);
        }
    }
}
