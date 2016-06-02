using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public enum ChargeType { PerMinute = 0, PerUnit = 1, Flat = 2 }
    public class VoiceService : PackageServiceSettings
    {
        public VoiceType VoiceType { get; set; }
        public ChargeType ChargeType { get; set; }
    }

    public abstract class VoiceType
    {
        public int ConfigId { get; set; }

    }
}
