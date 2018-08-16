using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class Service
    {
        public string ServiceId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public bool IsCore { get; set; }
        
        public string PackageId { get; set; }
        
        public decimal SubscriptionFee { get; set; }
        
        public decimal AccessFee { get; set; }
        
        public List<ServiceParameter> ServiceParams { get; set; }

        public bool IsTelephony { get; set; }

        public bool NeedsProvisioning { get; set; }
    }

    public class ServiceParameter
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
