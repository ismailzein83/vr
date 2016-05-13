using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplicationUserToAssign
    {
        public int CloudApplicationTenantID { get; set; }

        public List<int> UserIds { get; set; }
    }
}
