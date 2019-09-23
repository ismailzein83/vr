using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    //public class CreateCustomerInput
    //{
    //    public string CustomerId { get; set; }
    //    public string CustomerCategoryId { get; set; }
    //    public string PaymentMethodId { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string City { get; set; }
    //    public string CSO { get; set; }
    //    public string BankCode { get; set; }
    //    public string BankName { get; set; }
    //    public string AccountNumber { get; set; }
    //}
    public class CreateCustomerInput
    {
        public string CustomerId { get; set; }
        public string CustomerCategoryId { get; set; }
        public string DefaultRatePlan { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Career { get; set; }
        public string Language { get; set; }
        public string Nationality { get; set; }
        public string ExternalCustomerSetId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string HomePhone { get; set; }
        public string FaxNumber { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Email { get; set; }
        public string CSO { get; set; }
        public bool PaymentResponsibility { get; set; }
        public string PaymentMethodId { get; set; }
        public string BillCycle { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string DebitAccountOwner { get; set; }
        public string IBAN { get; set; }
        public string BankSwiftCode { get; set; }
        public string DocumentId { get; set; }
        public string DocumentTypeId { get; set; }
        public DateTime ValidFromDate { get; set; }
        public bool IsEmployee { get; set; }
        public string AddressNotes { get; set; }
    }



}
