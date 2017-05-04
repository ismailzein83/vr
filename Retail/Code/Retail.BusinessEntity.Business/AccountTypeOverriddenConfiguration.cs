using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("22C9E36D-D328-4220-83E8-E45AD1B005D8"); }
        }

        public Guid AccountTypeId { get; set; }

        public string OverriddenTitle { get; set; }

        public AccountTypeSettings OverriddenSettings { get; set; }
    }
}
