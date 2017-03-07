using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class PackageDefinitionAddOrEditFilter : IPackageDefinitionFilter
    {
        public bool EditMode { get; set; }
        public bool IsMatched(IPackageDefinitionFilterContext context)
        {
            PackageDefinitionManager pdmanager = new PackageDefinitionManager();
            bool matched = (this.EditMode) ? pdmanager.DoesUserHaveAddPackageDefinitions(context.PakageDefinitionId) || pdmanager.DoesUserHaveEditPackageDefinitions(context.PakageDefinitionId) : pdmanager.DoesUserHaveAddPackageDefinitions(context.PakageDefinitionId);
            return matched;
        }
    }
}
