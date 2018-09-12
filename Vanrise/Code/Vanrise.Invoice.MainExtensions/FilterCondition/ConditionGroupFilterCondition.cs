using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public enum LogicalOperator
    {
        [Description("AND")]
        And = 0,
        [Description("OR")]
        Or = 1,
    }
    public class ConditionGroupFilterCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("228497AE-5DBE-4A00-88A5-6DC6C5B8535A"); }
        }

        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            if (this.FilterConditionItems != null)
            {
                bool result = false;
                foreach (var filterConditionItem in this.FilterConditionItems)
                {
                    InvoiceGridActionFilterConditionContext invoiceGridActionFilterConditionContext = new InvoiceGridActionFilterConditionContext
                    {
                        Invoice =context.Invoice,
                        InvoiceAccount=context.InvoiceAccount,
                        InvoiceType = context.InvoiceType,
                        InvoiceAction= context.InvoiceAction
                    };
                    result = filterConditionItem.FilterCondition.IsFilterMatch(invoiceGridActionFilterConditionContext);
                    switch (this.LogicalOperator)
                    {
                        case LogicalOperator.And:
                            if (!result)
                                return false;
                            break;
                        case LogicalOperator.Or:
                            if (result)
                                return true;
                            break;
                    }
                }
            }
            return true;
        }
        public List<FilterConditionItem> FilterConditionItems { get; set; }
        public LogicalOperator LogicalOperator { get; set; }
    }
    public class FilterConditionItem
    {
        public string Name { get; set; }
        public InvoiceGridActionFilterCondition FilterCondition { get; set; }
    }
}
