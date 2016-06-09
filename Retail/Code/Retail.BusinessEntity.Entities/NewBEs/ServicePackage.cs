using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServicePackage
    {
        public int ServicePackageId { get; set; }

        public string Name { get; set; }

        public ServicePackageSettings Settings { get; set; }
    }
}
