using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountPartDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C01FA066-28C8-4225-9F59-39F5EECF86ED"); }
        }

        public Guid AccountPartDefinitionId { get; set; }

        public string OverriddenTitle { get; set; }

        public AccountPartDefinitionSettings OverriddenSettings { get; set; }
    }
}
