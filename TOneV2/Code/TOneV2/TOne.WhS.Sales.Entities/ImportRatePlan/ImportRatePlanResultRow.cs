using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class ValidatedImportedRow
	{
		public ImportedRow ImportedRow { get; set; }

		public ImportedRowStatus Status { get; set; }

		public string ErrorMessage { get; set; }
	}

	public enum ImportedRowStatus
	{
		Valid = 0,
		Invalid = 1,
        OnlyNormalRateValid = 2
	}
}
