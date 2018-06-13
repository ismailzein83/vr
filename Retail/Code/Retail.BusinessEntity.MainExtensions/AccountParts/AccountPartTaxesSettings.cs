using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartTaxesSettings : AccountPartSettings, IAccountTaxes
    {
        public override Guid ConfigId { get { return _ConfigId; } }
        public static Guid _ConfigId = new Guid("028F122A-7EA4-4DA5-8D87-34676F55AB94");
        public List<TaxInvoiceTypeSetting> InvoiceTypes { get; set; }

        public List<TaxInvoiceTypeSetting> GetAccountTaxes()
        {
            return InvoiceTypes;
        }
    }
   
}
