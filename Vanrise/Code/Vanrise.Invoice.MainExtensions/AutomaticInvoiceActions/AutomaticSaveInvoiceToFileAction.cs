using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions
{
    public class AutomaticSaveInvoiceToFileAction : AutomaticInvoiceActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C5F9CDCC-FC61-4F0B-9B72-B5DE53D8520B"); }
        }
        public List<InvoiceToFileActionSet> InvoiceToFileActionSets { get; set; }
        public override void Execute(IAutomaticInvoiceActionSettingsContext contex)
        {
            throw new NotImplementedException();
        }
        public override string RuntimeEditor
        {
            get { return "vr-invoicetype-automaticinvoiceaction-savetofile-runtime"; }
        }
    }
    public class InvoiceToFileActionSet
    {
        public Guid InvoiceToFileActionSetId { get; set; }
        public string Name { get; set; }
        public List<Guid> AttachmentsIds { get; set; }
        public PartnerInvoiceFilterCondition FilterCondition { get; set; }

    }
}
