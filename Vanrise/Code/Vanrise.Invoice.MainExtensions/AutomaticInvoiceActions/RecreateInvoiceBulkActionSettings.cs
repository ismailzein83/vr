using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions
{
    public class RecreateInvoiceBulkActionSettings : AutomaticInvoiceActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FFECCE75-D9CA-4357-94AD-7BD643B633F8"); }
        }
        public override void Execute(IAutomaticInvoiceActionSettingsContext contex)
        {
            throw new NotImplementedException();
        }
        public override string RuntimeEditor
        {
            get { return "vr-invoicetype-invoicebulkaction-recreateinvoice-runtime"; }
        }
    }
}
