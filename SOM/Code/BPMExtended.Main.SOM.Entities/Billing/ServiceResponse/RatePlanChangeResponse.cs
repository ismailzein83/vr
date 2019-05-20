using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class RatePlanChangeResponse
    {
        public List<ServicePackage> ServicesAreUnavailableInNewRP { get; set; }
        public List<ServicePackage> ServicesAreInDifferentPKGInNewRP { get; set; }
        public List<ServicePackage> ServicesAreCoreInNewRP { get; set; }
    }

    public class ServicePackage
    {
        public string PackageId { get; set; }

        public List<Service> Services { get; set; }
    }

    public class Service
    {
        public string Id { get; set; }
        public bool IsCore { get; set; }
    }

}
