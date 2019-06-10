using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealTimePeriodConfig : Vanrise.Entities.ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Deal_DealTimePeriodConfig";

        public string Editor { get; set; }
    }
}