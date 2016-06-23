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
        public List<CompanyProfileContactType> ContactTypes { get; set; }
    }
    public class CompanyProfileContactType
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
