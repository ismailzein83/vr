using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class ServicePackage
    {
        public string PackageId { get; set; }

        public string PackageName { get; set; }

        public List<Service> Services { get; set; }

    }
}
