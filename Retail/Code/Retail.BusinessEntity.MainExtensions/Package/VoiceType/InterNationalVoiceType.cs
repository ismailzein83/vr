using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public class InterNationalVoiceType:VoiceType
    {
        public List<TargetZone> TargetZones { get; set; }
    }
    public class TargetZone
    {
        public int CountryId { get; set; }
        public List<int> ZonesIds { get; set; }
        public decimal Rate { get; set; }
    }
}
