using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class AutomaticInvoiceActionsPart : InvoiceSettingPart
    {
        public List<AutomaticInvoiceActionRuntime> Actions { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("A3C1239E-2C00-4236-96EC-690A5B294586"); }
        }
    }
    public class AutomaticInvoiceActionRuntime
    {
        public Guid AutomaticInvoiceActionId { get; set; }
        public AutomaticInvoiceActionRuntimeSettings Settings { get; set; }
    }
    public abstract class AutomaticInvoiceActionRuntimeSettings
    {
        public abstract void Execute(IAutomaticSendEmailActionRuntimeSettingsContext context);
    }
}
