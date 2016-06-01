using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Package
    {
        public int PackageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PackageSettings Settings { get; set; }
    }
    public class PackageSettings
    {
        public List<PackageService> Services { get; set; }
    }
    public abstract class PackageService
    {
        public int ConfigId { get; set; }
    }
}
