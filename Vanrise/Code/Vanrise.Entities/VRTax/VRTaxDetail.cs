using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRTaxItemDetail
    {
        public Decimal Value { get; set; }
        public string TaxName { get; set; }
        public bool IsVAT { get; set; }
    }
}
