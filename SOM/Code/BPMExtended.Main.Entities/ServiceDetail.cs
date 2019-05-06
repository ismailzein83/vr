using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ServiceDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PackageId { get; set; }
        public bool NeedsProvisioning { get; set; }
        public bool IsNetwork { get; set; }

        //public string ServiceResource { get; set; }

        //public string ServiceId { get; set; }


        //public string Name { get; set; }

        //public string Description { get; set; }

        //public bool IsCore { get; set; }

        //public string PackageName { get; set; }

        //public decimal SubscriptionFee { get; set; }

        //public decimal AccessFee { get; set; }

        //public List<ServiceParameterDetail> ServiceParams { get; set; }
    }

    public class ServiceInfo
    {

        public string ServiceId { get; set; }

        public string Name { get; set; }

    }

    public class ServiceParameterDetail
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
