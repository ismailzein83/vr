using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageItem
    {
        public int ServiceTypeId { get; set; }
        public PackageItemSettings Settings { get; set; }
    }

    public abstract class PackageItemSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
