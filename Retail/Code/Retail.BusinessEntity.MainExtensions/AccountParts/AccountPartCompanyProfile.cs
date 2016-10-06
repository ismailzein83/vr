using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartCompanyProfile : AccountPartSettings
    {
        public override Guid ConfigId { get { return _ConfigId; } }
        public static Guid _ConfigId = new Guid("B0717C4F-E409-4AE2-8C00-5ADD4CA828C5");


        public const int ExtensionConfigId = 21;
        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string POBox { get; set; }

        public string Website { get; set; }
        
        public List<string> PhoneNumbers { get; set; }

        public List<string> Faxes { get; set; }

        public List<AccountCompanyContact> Contacts { get; set; }
    }

    public class AccountCompanyContact
    {
        public string ContactName { get; set; }

        public string ContactType { get; set; }

        public string Email { get; set; }

        public List<string> PhoneNumbers { get; set; }
    }
}
