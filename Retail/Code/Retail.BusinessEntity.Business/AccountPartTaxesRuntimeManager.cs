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
        public Dictionary<Guid, InvoiceTypesTaxesRuntime> GetInvoiceTypesTaxesRuntime(List<Guid> invoiceTypesIds)
        {
            Dictionary<Guid, InvoiceTypesTaxesRuntime> invoiceTypesTaxes = new Dictionary<Guid, InvoiceTypesTaxesRuntime>();
           
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            if (invoiceTypesIds != null)
            {
                ConfigManager configManager = new ConfigManager();
                var taxesDefinitions = configManager.GetRetailTaxesDefinitions();
                foreach (var invoiceTypeId in invoiceTypesIds)
                {
                    var invoiceTypesTaxesRuntime = new InvoiceTypesTaxesRuntime
                    {
                        InvoiceTypeId = invoiceTypeId,
                        InvoiceTypeTitle = invoiceTypeManager.GetInvoiceType(invoiceTypeId).Name,
                        TaxesDefinitions = taxesDefinitions
                    };
                    invoiceTypesTaxes.Add(invoiceTypeId, invoiceTypesTaxesRuntime);
                }
            }
            return invoiceTypesTaxes;
        }
    }
}
