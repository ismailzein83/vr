using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class PackageDefinitionServiceTypeFilter : IServiceTypeFilter
    {
        public Guid PackageDefinitionId { get; set; }
        public bool IsMatched(IPackageDefinitionServiceTypeFilterContext context)
        {
            if (context.entityDefinition == null || context.entityDefinition.Settings == null)
                return false;

            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            var packageDefinition = packageDefinitionManager.GetPackageDefinitionById(this.PackageDefinitionId);
            if (packageDefinition == null || packageDefinition.Settings == null )
                return false;

            if (packageDefinition.Settings.AccountBEDefinitionId != context.entityDefinition.AccountBEDefinitionId)
                return false;

            return true;
        }
    }
  
  
    
}
