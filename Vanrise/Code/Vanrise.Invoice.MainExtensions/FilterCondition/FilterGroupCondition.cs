using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class FilterGroupCondition : InvoiceFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("3764B09D-3F5A-4121-91D3-4AEF308AD437"); }
        }
        public  Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public override bool IsFilterMatch(IInvoiceFilterConditionContext context)
        {
           RecordFilterManager recordFilterManager = new RecordFilterManager();
           DataRecordFilterGenericFieldMatchContext invoiceGridActionsContext = new DataRecordFilterGenericFieldMatchContext(context.Invoice.Details, context.InvoiceType.Settings.InvoiceDetailsRecordTypeId);
           return recordFilterManager.IsFilterGroupMatch(this.FilterGroup, invoiceGridActionsContext);
        }
    }
}
