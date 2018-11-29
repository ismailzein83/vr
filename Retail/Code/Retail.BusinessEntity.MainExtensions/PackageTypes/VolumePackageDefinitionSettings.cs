using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class VolumePackageDefinitionSettings : PackageDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("959D230D-3FEA-44FE-9231-6698642F48CB"); } }

        public override string RuntimeEditor { get { return "retail-be-packagesettings-extendedsettings-volume"; } }

        public List<VolumePackageDefinitionItem> Items { get; set; }
    }

    public class VolumePackageDefinitionItem
    {
        public Guid VolumePackageDefinitionItemId { get; set; }

        public CompositeRecordConditionDefinitionGroup CompositeRecordConditionDefinitionGroup { get; set; }

        public List<Guid> ServiceTypeIds { get; set; }

        public string Name { get; set; }
    }
}