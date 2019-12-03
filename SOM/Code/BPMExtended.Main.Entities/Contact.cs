using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class Contact
    {
        public string DocumentID { get; set; }
        public string CSOId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCategoryID { get; set; }
        public string CustomerCategoryName { get; set; }
        public string DocumentIdTypeId { get; set;  }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string SurName { get; set; }
        public string Title { get; set; }
        public string MotherName { get; set; }
        public string BirthDate { get; set; }
        public string Career { get; set; }
        public string LanguageId { get; set; }
        public string NationalityId { get; set; }
        public string CountryId { get; set; }
        public string RegionId { get; set; }
        public string CityId { get; set; }
        public string TownId { get; set; }
        public string DistrictId { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNumber { get; set; }
        public string Mailbox { get; set; }
        public string AddressNotes { get; set; }
        public string HomePhone { get; set; }
        public string FaxNumber { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Email { get; set; }
        public string CustomerBillCycle { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string BankAccountID { get; set; }
        public string IBAN { get; set; }
        public string Street { get; set; }
    }
}
