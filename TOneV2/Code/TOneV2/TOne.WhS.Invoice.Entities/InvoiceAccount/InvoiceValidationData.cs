using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceValidationData
    {
        public IEnumerable<InvoiceType> InvoiceTypes { get; set; }
        public IEnumerable<InvoiceAccountData> ProfileInvoiceAccounts { get; set; }
        public InvoiceCarrierProfile InvoiceCarrierProfile { get; set; }
        public InvoiceCarrierAccount InvoiceCarrierAccount { get; set; }
    }
    public class InvoiceCarrierProfile
    {
        public CarrierProfile CarrierProfile { get; set; }
        public IEnumerable<CarrierAccount> ProfileCarrierAccounts { get; set; }
        public Dictionary<int, IEnumerable<InvoiceAccountData>> InvoiceAccountsByAccount { get; set; }

    }
    public class InvoiceAccountData
    {
        public InvoiceAccount InvoiceAccount { get; set; }
        public bool IsApplicableToCustomer { get; set; }
        public bool IsApplicableToSupplier { get; set; }

    }
    public class InvoiceCarrierAccount
    {
        public IEnumerable<InvoiceAccountData> InvoiceAccounts { get; set; }
        public CarrierAccount CarrierAccount { get; set; }

    }
}
