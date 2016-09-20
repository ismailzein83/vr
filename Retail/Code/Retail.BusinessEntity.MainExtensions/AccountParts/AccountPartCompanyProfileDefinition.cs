using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartCompanyProfileDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("1aff2bf7-1f15-4e0b-accf-457edf36a342"); } }

        public List<CompanyProfileContactType> ContactTypes { get; set; }
    }
    public class CompanyProfileContactType
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
