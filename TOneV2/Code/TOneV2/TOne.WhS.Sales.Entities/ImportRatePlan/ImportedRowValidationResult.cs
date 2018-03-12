using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ImportedDataValidationResult
    {
		public HashSet<long> ApplicableZoneIds { get; set; }

		public Dictionary<long, ImportedRow> ValidDataByZoneId { get; set; }

        public Dictionary<int, InvalidImportedRow> InvalidDataByRowIndex { get; set; }

		public bool FileIsEmpty;

        public ImportedDataValidationResult()
        {
			ApplicableZoneIds = new HashSet<long>();
			ValidDataByZoneId = new Dictionary<long, ImportedRow>();
            InvalidDataByRowIndex = new Dictionary<int, InvalidImportedRow>();
        }
    }
}
