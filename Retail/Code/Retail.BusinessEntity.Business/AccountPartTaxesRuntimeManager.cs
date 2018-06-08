using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Invoice.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountPartTaxesRuntimeManager
    {
        public Dictionary<Guid, InvoiceTypesTaxesRuntime> GetInvoiceTypesTaxesRuntime(List<Guid> InvoiceTypesIds)
        {
            Dictionary<Guid, InvoiceTypesTaxesRuntime> invoiceTypesTaxes = new Dictionary<Guid, InvoiceTypesTaxesRuntime>();
           
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            ConfigManager configManager = new ConfigManager();

            foreach (var invoiceTypeId in InvoiceTypesIds){
                InvoiceTypesTaxesRuntime invoiceTypesTaxesRuntime = new InvoiceTypesTaxesRuntime();

                invoiceTypesTaxesRuntime.InvoiceTypeTitle = invoiceTypeManager.GetInvoiceType(invoiceTypeId).Name;
                invoiceTypesTaxesRuntime.TaxesDefinitions = configManager.GetRetailTaxesDefinitions();
                invoiceTypesTaxes.Add(invoiceTypeId, invoiceTypesTaxesRuntime);
            }
            return invoiceTypesTaxes;
        }
    }
}
