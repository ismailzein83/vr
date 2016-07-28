using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Data.Entities
{
    public class DataChargingPolicySettings : ChargingPolicySettings
    {
        public int DownloadSpeedInKbps { get; set; }

        public int UploadSpeedInKbps { get; set; }

        public int DownloadQuotaInMB { get; set; }

        public int UploadQuotaInMB { get; set; }
    }
}
    