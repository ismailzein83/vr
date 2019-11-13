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
        public bool HasDeposit { get; set; }
        public bool IsServiceResource { get; set; }
        public decimal SubscriptionFee { get; set; }
        public decimal AccessFee { get; set; }
        public bool CanDiscount { get; set; }
        public bool HasDiscount { get; set; }
        public List<ServiceParameterInfo> ServiceParameters { get; set; }
    }
    public class ServiceParameterInfo {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterDisplayValue { get; set; }
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
