using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ServicePackageItem
{
    public class ServicePackageChargingPolicyItem : Entities.PackageItemSettings
    {
        public override Guid ConfigId { get { return new Guid("f3cee2a7-1d63-4804-b9c0-9aba4f43f480"); } }

        public ChargingPolicySettings ChargingPolicySettings { get; set; }
    }
}
