using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IPackageFilter
    {
        bool IsMatched(IPackageFilterContext context);
    }

    public interface IPackageFilterContext
    {
        Package Package { get; }
    }
}
