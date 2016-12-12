using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageGroup
    {
        public int PackageGroupId { get; set; }

        public string Name { get; set; }

        public PackageGroupSettings Settings { get; set; }
    }
}
