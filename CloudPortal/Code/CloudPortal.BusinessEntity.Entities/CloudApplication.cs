using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplication
    {
        public int CloudApplicationId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public CloudApplicationSettings Settings { get; set; }

        public CloudApplicationIdentification ApplicationIdentification { get; set; }
    }

    public class CloudApplicationSettings
    {
        public string OnlineURL { get; set; }

        public string InternalURL { get; set; }
    }
}
