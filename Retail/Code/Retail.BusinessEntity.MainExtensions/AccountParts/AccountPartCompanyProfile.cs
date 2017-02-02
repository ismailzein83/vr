using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartCompanyProfile : AccountPartSettings, IAccountProfile
    {
        private static CountryManager s_countryManager = new CountryManager();
        private static CityManager s_cityManager = new CityManager();

        public override Guid ConfigId { get { return _ConfigId; } }
        public static Guid _ConfigId = new Guid("B0717C4F-E409-4AE2-8C00-5ADD4CA828C5");

        public const int ExtensionConfigId = 21;
        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string POBox { get; set; }

        public string Website { get; set; }

        public string ArabicName { get; set; }

        public List<AccountCompanyContact> Contacts { get; set; }

        #region IAccountProfile Memebers

        public List<string> PhoneNumbers { get; set; }

        public List<string> Faxes { get; set; }

        public string Address { get; set; }

        #endregion


        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "Emails": return this.Contacts != null ? this.Contacts.Select(itm => itm.Email) : null;
                case "ArabicName": return this.ArabicName;

                default: return null;
            }
        }
       
    }

    public class AccountCompanyContact
    {
        public string ContactName { get; set; }

        public string ContactType { get; set; }

        public string Email { get; set; }

        public List<string> PhoneNumbers { get; set; }
    }
}
