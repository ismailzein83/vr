using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Package
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_Package";

        public int PackageId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public PackageSettings Settings { get; set; }
    }

    public class PackageSettings
    {
        public List<PackageItem> Services { get; set; }

        public PackageExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class PackageExtendedSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
