using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public abstract class TenantTypeSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool IsViewIncluded(ITenantTypeIsViewIncludedContext context);

        public abstract bool IsEntityIncluded(ITenantTypeIsBusinessEntityIncludedContext context);
    }

    public interface ITenantTypeIsViewIncludedContext
    {
        Guid ViewId { get; }
    }

    public interface ITenantTypeIsBusinessEntityIncludedContext
    {
        Guid EntityId { get; }
    }
}
