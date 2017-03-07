using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageDefinitionFilter
    {
        public Guid? AccountBEDefinitionId { get; set; }

        public bool IncludeHiddenPackageDefinitions { get; set; }

        public List<IPackageDefinitionFilter> Filters { get; set; }
    }

    public interface IPackageDefinitionFilter
    {
        bool IsMatched(IPackageDefinitionFilterContext context);
    }

    public interface IPackageDefinitionFilterContext
    {
        Guid PakageDefinitionId { get; }
    }

    public class PackageDefinitionFilterContext : IPackageDefinitionFilterContext
    {
        public Guid PakageDefinitionId { get; set; }
    }
}
