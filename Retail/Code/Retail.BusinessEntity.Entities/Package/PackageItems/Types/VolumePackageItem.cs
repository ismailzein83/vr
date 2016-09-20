using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class VolumePackageItem : PackageItemSettings
    {
        public override Guid ConfigId { get { return new Guid("e548dc54-6664-45e6-b5cf-9b84d046d782"); } }
        public List<ConditionalServiceVolume> ConditionalVolumes { get; set; }
    }

    public class ConditionalServiceVolume
    {
        public Vanrise.GenericData.Entities.DataRecordCondition EventCondition { get; set; }

        public VolumeSettings VolumeSettings { get; set; }
    }
}
