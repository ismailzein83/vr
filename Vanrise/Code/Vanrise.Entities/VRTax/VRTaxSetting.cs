using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRTaxSetting
    {
        public List<VRTaxItem> Items { get; set; }
        public Decimal? VAT { get; set; }
        public string VATId { get; set; }
    }
}
