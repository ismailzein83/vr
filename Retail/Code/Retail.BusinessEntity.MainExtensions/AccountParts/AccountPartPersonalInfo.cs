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
        public static Guid _ConfigId = new Guid("A46ABF18-DEEC-4F78-9178-F433790B5AEB");
        public override Guid ConfigId { get { return _ConfigId; } }

       // public const int ExtensionConfigId = 27;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
    }
}
