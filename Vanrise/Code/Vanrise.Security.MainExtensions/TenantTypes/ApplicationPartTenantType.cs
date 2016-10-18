using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.MainExtensions.TenantTypes
{
    public class ApplicationPartTenantType : TenantTypeSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("5C68DE43-E662-4BEF-B8DE-A099B8A4302E"); }
        }

        public List<Guid> ViewIds { get; set; }

        public List<Guid> EntityIds { get; set; }

        public override bool IsViewIncluded(ITenantTypeIsViewIncludedContext context)
        {
            return this.ViewIds != null && this.ViewIds.Contains(context.ViewId);
        }

        public override bool IsEntityIncluded(ITenantTypeIsBusinessEntityIncludedContext context)
        {
            return this.EntityIds != null && this.EntityIds.Contains(context.EntityId);
        }
    }
}
