using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplicationUserDetail
    {
        public CloudApplicationUser Entity { get; set; }

        public string UserName { get; set; }
    }
}
