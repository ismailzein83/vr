using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceGeneratorActionFilterGroupCondition : InvoiceGeneratorActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("A72E2AAA-E837-47BD-8BF4-C41EA07893EA"); }
        }
        public Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public override bool IsFilterMatch(IInvoiceGeneratorActionFilterConditionContext context)
        {
            return true;
            //RecordFilterManager recordFilterManager = new RecordFilterManager();
            //DataRecordFilterGenericFieldMatchContext invoiceGeneratorActionsContext = new DataRecordFilterGenericFieldMatchContext(context.Invoice.Details, context.InvoiceType.Settings.InvoiceDetailsRecordTypeId);
            //return recordFilterManager.IsFilterGroupMatch(this.FilterGroup, invoiceGridActionsContext);
        }
    }
}
