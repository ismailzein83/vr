using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartResidentialProfile : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("05FECF19-6413-402F-BD65-64B0EEF1FB52");
        public override Guid ConfigId { get { return _ConfigId; } }

       // public const int ExtensionConfigId = 22;
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }        
        public string Email { get; set; }
        public string ZipCode { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public List<string> Faxes { get; set; }
        public string Provence { get; set; }
    }
}
