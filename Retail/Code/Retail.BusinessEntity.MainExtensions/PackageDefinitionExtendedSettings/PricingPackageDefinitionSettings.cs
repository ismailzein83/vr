using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions
{
    public class PricingPackageDefinitionSettings : PackageDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("76A889A4-9F93-4327-91C4-EE2F1EF2026E"); } }

        public override string RuntimeEditor { get { return "retail-be-pricingpackagesettings-management"; } }
    }
}
