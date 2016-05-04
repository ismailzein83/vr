using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Entities
{
    public class ApplicationTenantRegistration
    {
        public int ApplicationId { get; set; }

        public int TenantId { get; set; }

        public ApplicationTenantLicense License { get; set; }
    }

    public class ApplicationTenantLicense
    {
        public DateTime? ExpiresAt { get; set; }
    }
}
