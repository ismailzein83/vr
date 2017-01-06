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
                Guid packageDefinitionBEDefinitonId = GetPackageDefinitionBEDefinitonId(context.Package.Settings.PackageDefinitionId);
                Guid productDefinitionBEDefinitonId = GetProductDefinitionBEDefinitonId(this.ProductDefinitionId);
                
                if (packageDefinitionBEDefinitonId != productDefinitionBEDefinitonId)
                    return false;
            }
            return true;
        }

        private Guid GetPackageDefinitionBEDefinitonId(Guid packageDefinitionId)
        {
            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            PackageDefinition packageDefinition = packageDefinitionManager.GetPackageDefinitionById(packageDefinitionId);
            if (packageDefinition == null)
                throw new NullReferenceException(string.Format("packageDefinition of packageDefinitionId: {0}", packageDefinitionId));

            if (packageDefinition.Settings == null)
                throw new NullReferenceException(string.Format("packageDefinition.Settings of packageDefinitionId: {0}", packageDefinitionId));

            return packageDefinition.Settings.AccountBEDefinitionId;
        }
        private Guid GetProductDefinitionBEDefinitonId(Guid productDefinitionId)
        {
            ProductDefinitionManager productDefinitionManager = new ProductDefinitionManager();
            ProductDefinition productDefinition = productDefinitionManager.GetPackageDefinitionById(productDefinitionId);
            if (productDefinition == null)
                throw new NullReferenceException(string.Format("productDefinition of packageDefinitionId: {0}", productDefinitionId));

            if (productDefinition.Settings == null)
                throw new NullReferenceException(string.Format("productDefinition.Settings of packageDefinitionId: {0}", productDefinitionId));

            return productDefinition.Settings.AccountBEDefinitionId;
        }
    }
}
