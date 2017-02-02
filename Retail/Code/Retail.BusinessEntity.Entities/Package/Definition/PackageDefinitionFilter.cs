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
    }
}
