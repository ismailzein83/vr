using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class RatePlan
    {
        public string RatePlanId { get; set; }
        
        public string Name { get; set; }

        public LineOfBusiness LOB { get; set; }

        public string SubType { get; set; }

        public CustomerCategory Category { get; set; }

        public ServicePackage CorePackage { get; set; }

        public List<ServicePackage> OptionalPackages { get; set; }

    }
}
