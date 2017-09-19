using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.APIEntities
{
    public class ClientRetailProfileAccountInfo
    {
        public string AccountName { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }

        public int? CityId { get; set; }
        public string CityName { get; set; }

        public string Town { get; set; }
        public string Street { get; set; }
        public string POBox { get; set; }
        public string Website { get; set; }
        public string ArabicName { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public List<string> PhoneNumbers { get; set; }
        public string PhoneNumbersDescription { get; set; }
        public List<string> MobileNumbers { get; set; }
        public string MobileNumbersDescription { get; set; }
        public List<string> Faxes { get; set; }
        public string FaxesDescription { get; set; }
        public string Address { get; set; }


        public List<AccountCompanyContact> Contacts { get; set; }
    }
    public class AccountCompanyContact
    {
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string PhoneNumbersDescription { get; set; }
        public List<string> MobileNumbers { get; set; }
        public string MobileNumbersDescription { get; set; }
        public SalutationType? Salutation { get; set; }
        public string SalutationDescription { get; set; }

        public string Notes { get; set; }
    }

}
