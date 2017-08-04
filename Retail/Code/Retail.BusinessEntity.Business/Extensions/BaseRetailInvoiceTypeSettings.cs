using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.Business
{
    public abstract class BaseRetailInvoiceTypeSettings : InvoiceTypeExtendedSettings
    {
        public Guid AccountBEDefinitionId { get; set; }
    }
}
