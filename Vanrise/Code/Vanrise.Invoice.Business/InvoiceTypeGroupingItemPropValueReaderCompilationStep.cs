using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Invoice.Business
{
    public class InvoiceTypeGroupingItemPropValueReaderCompilationStep : IPropValueReaderCompilationStep
    {
        public HashSet<string> GetPropertiesToCompile(IPropValueReaderCompilationStepContext context)
        {

            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceTypes = invoiceTypeManager.GetInvoiceTypes();
            HashSet<string> hashSet = new HashSet<string>();
            foreach(var invoiceType in invoiceTypes)
            {
                if(invoiceType.Settings.ItemGroupings != null)
                {
                    foreach(var itemGrouping  in invoiceType.Settings.ItemGroupings)
                    {
                        if(itemGrouping.DimensionItemFields != null)
                        {
                            foreach(var dimensionItemField in itemGrouping.DimensionItemFields)
                            {
                                hashSet.Add(dimensionItemField.FieldName);
                            }
                        }
                        if (itemGrouping.AggregateItemFields != null)
                        {
                            foreach (var aggregateItemField in itemGrouping.AggregateItemFields)
                            {
                                hashSet.Add(aggregateItemField.FieldName);
                            }
                        }
                    }
                }
            }
            return hashSet;
        }
    }
}
