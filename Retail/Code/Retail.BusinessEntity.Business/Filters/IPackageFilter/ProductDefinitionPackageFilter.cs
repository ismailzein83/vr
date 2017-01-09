using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    class ProductDefinitionPackageFilter : IPackageFilter
    {
        public Guid ProductDefinitionId { get; set; }

        public bool IsMatched(IPackageFilterContext context)
        {
            if (context != null && context.Package != null && context.Package.Settings != null)
            {
                Guid packageDefinitionBEDefinitonId = new PackageDefinitionManager().GetPackageDefinitionAccountBEDefId(context.Package.Settings.PackageDefinitionId);
                Guid productDefinitionBEDefinitonId = new ProductDefinitionManager().GetProductDefinitionAccountBEDefId(this.ProductDefinitionId);
                
                if (packageDefinitionBEDefinitonId != productDefinitionBEDefinitonId)
                    return false;
            }
            return true;
        }
    }
}
