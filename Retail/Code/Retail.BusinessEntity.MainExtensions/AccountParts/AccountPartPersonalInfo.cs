using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public enum Gender { Male = 0, Female = 1 }
    public class AccountPartPersonalInfo : AccountPartSettings
    {
        public const int ExtensionConfigId = 27;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int? BirthCountryId { get; set; }
        public int? BirthCityId { get; set; }
    }
}
