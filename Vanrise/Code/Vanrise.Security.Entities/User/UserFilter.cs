using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserFilter
    {
        public EntityType? EntityType { get; set; }
        public string EntityId { get; set; }

        public bool IncludeSystemUsers { get; set; }

        public bool ExcludeInactive { get; set; }
        public bool GetOnlyTenantUsers { get; set; }
        public int? TenantId { get; set; }
        public List<IUserFilter> Filters { get; set; }
    }

    public interface IUserFilter
    {
        bool IsExcluded(User user);
    }
}
