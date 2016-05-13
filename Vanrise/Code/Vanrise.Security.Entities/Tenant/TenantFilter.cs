using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class TenantFilter
    {
        public int? CanBeParentOfTenantId { get; set; }

        public List<ITenantFilter> Filters { get; set; }
    }

    public interface ITenantFilter
    {
        bool IsExcluded(Tenant tenant);
    }
}
