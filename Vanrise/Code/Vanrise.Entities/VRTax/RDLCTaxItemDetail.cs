using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class RDLCTaxItemDetail
    {
        public Decimal Value { get; set; }
        public string TaxName { get; set; }
        public RDLCTaxItemDetail() { }
        public IEnumerable<RDLCTaxItemDetail> GetRDLCTaxItemDetailsSchema()
        {
            return null;
        }
    }
}
