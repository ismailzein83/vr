using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class ImportBulkActionValidationResult : BulkActionValidationResult
    {
		public string ErrorMessage { get; set; }

		public List<InvalidImportedRow> InvalidImportedRows { get; set; }

        public ImportBulkActionValidationResult()
        {
            base.ExcludedZoneIds = new List<long>();
            InvalidImportedRows = new List<InvalidImportedRow>();
        }
    }
}
