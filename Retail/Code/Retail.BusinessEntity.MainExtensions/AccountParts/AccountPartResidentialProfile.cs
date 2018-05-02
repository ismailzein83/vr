using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartResidentialProfile : AccountPartSettings, IAccountProfile
    {
        private static CountryManager s_countryManager = new CountryManager();
        private static CityManager s_cityManager = new CityManager();

        public static Guid _ConfigId = new Guid("05FECF19-6413-402F-BD65-64B0EEF1FB52");
        public override Guid ConfigId { get { return _ConfigId; } }

        // public const int ExtensionConfigId = 22;
        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string Email { get; set; }

        public string ZipCode { get; set; }

        public string Provence { get; set; }

        #region IAccountProfile Memebers

        public List<string> PhoneNumbers { get; set; }

        public List<string> Faxes { get; set; }

        public string Address
        {
            get
            {
                string countryName = string.Empty;
                string cityName = string.Empty;

                if (this.CountryId != null)
                    countryName = s_countryManager.GetCountryName(this.CountryId.Value);

                if (this.CityId != null)
                    cityName = s_cityManager.GetCityName(this.CityId.Value);

                List<string> address = new List<string>();

                if (string.IsNullOrEmpty(countryName))
                    address.Add(countryName);

                if (string.IsNullOrEmpty(cityName))
                    address.Add(cityName);

                address.Add(this.Town);
                address.Add(this.Street);

                return string.Join(",", address.ToArray());
            }
        }

        #endregion


        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "Email": return this.Email;

                default: return null;
            }
        }
        public bool TryGetContact(string contactType, out AccountContact accountContact)
        {
            accountContact=null;
            return false;
        }
    }
}
