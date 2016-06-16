using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ServicePackageItem
{
    public class ServicePackageVolumeItem : Entities.PackageItemSettings
    {
        public VolumeSettings VolumeSettings { get; set; }
    }
}
