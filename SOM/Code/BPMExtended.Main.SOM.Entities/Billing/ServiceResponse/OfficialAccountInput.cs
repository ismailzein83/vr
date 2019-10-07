using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class OfficialAccountInput
    {
        public string CustomerId { get; set; }
        public string CustomerCategoryId { get; set; }
        public string DefaultRatePlan { get; set; }
        public string Name { get; set; }
        public string MinistryName { get; set; }
        public string EndCustomerName { get; set; }
        public string FaxNumber { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Email { get; set; }
        public string BusninessType { get; set; }
        public string Nationality { get; set; }
        public string DocumentId { get; set; }
        public string DocumentIdType { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string ExternalCustomerSetId { get; set; }
        public string AddressNotes { get; set; }
        public string CSO { get; set; }
        public bool PaymentResponsibility { get; set; }
        public string PaymentMethodId { get; set; }
        public bool BillInformation { get; set; }
        public string BillCycle { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string DebitAccountOwner { get; set; }
        public string IBAN { get; set; }
        public string BankSwiftCode { get; set; }
        public DateTime ValidFromDate { get; set; }
    }
}
