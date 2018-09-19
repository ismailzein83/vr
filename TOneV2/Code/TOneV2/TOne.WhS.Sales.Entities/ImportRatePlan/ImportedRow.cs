using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class ImportedRow
	{
		public string Zone { get; set; }

		public string Rate { get; set; }

		public string EffectiveDate { get; set; }

        public List<ImportedOtherRate> OtherRates { get; set; }

        public ImportedRowStatus Status { get; set; }
    }
    public class ImportedOtherRate
    {
        public string Value { get; set; }
        public string TypeName { get; set; }
        public int TypeId { get; set; }
    }
}
