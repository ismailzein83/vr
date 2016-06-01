using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public class VoiceService : PackageService
    {
        public int DurationPerUnit { get; set; }
        public int FractionUnit { get; set; }
        public bool RoamingSupport { get; set; }
    }
}
