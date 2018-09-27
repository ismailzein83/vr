using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class VolumePackageDefinitionSettings : PackageDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("959D230D-3FEA-44FE-9231-6698642F48CB"); }
        }

        public List<VolumePackageDefinitionItem> Items { get; set; }
    }

    public class VolumePackageDefinitionItem
    {
        public Guid VolumePackageDefinitionItemId { get; set; }

        public Guid RecordTypeId { get; set; }

        public List<Guid> ServiceTypeIds { get; set; }

        public string Name { get; set; }
    }
}
