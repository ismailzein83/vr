using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.MainExtensions.TenantTypes
{
    public class DefaultTenantType : TenantTypeSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("649495CA-172E-4AF5-AB09-ABFE908E3CA4"); }
        }

        public override bool IsViewIncluded(ITenantTypeIsViewIncludedContext context)
        {
            return true;
        }

        public override bool IsEntityIncluded(ITenantTypeIsBusinessEntityIncludedContext context)
        {
            return true;
        }
    }
}
