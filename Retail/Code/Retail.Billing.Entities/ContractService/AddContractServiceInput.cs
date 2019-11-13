using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class AddContractServiceInput
    {
        public long ContractId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        public long? BillingAccountId { get; set; }

        public DateTime BET { get; set; }

        public Guid? TechnologyId { get; set; }

        public Guid? SpecialNumberCategoryId { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }

        public int? VoiceVolumeFixed { get; set; }

        public int? VoiceVolumeMobile { get; set; }

        public int? VoiceVolumePreferredNb { get; set; }
    }    
}
