using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum VolumeCommitmentType { Buy = 0, Sell = 1 }
    public class VolumeCommitment
    {
        public int VolumeCommitmentId { get; set; }

        public VolumeCommitmentSettings Settings { get; set; }
    }

    public class VolumeCommitmentSettings
    {
        public VolumeCommitmentType Type { get; set; }
        
        public int CarrierAccountId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int GracePeriod { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public List<VolumeCommitmentItem> Items { get; set; }
    }

    public class VolumeCommitmentItem
    {
        public string Name { get; set; }

        public List<long> ZoneIds { get; set; }

        public List<VolumeCommitmentItemRate> Rates { get; set; }
    }

    public class VolumeCommitmentItemRate
    {
        public int UpToVolume { get; set; }

        public Decimal Rate { get; set; }

        public bool IsRetroActive { get; set; }
    }
}
