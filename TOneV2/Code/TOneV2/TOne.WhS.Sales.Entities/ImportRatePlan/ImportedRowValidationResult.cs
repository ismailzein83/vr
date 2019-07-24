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

		public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }

		public bool FileIsEmpty;

        public ImportedDataValidationResult()
        {
			ApplicableZoneIds = new HashSet<long>();
			ValidDataByZoneId = new Dictionary<long, ImportedRow>();
            InvalidDataByRowIndex = new Dictionary<int, InvalidImportedRow>();
			AdditionalCountryBEDsByCountryId = new Dictionary<int, DateTime>();
		}
    }

    public class CustomerTargetMatchImportedDataValidationResult
    {
        public HashSet<long> ApplicableZoneIds { get; set; }

        public Dictionary<long, CustomerTargetMatchImportedRow> ValidDataByZoneId { get; set; }

        public Dictionary<int, CustomerTargetMatchInvalidImportedRow> InvalidDataByRowIndex { get; set; }

        //public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }

        public bool FileIsEmpty;

        public CustomerTargetMatchImportedDataValidationResult()
        {
            ApplicableZoneIds = new HashSet<long>();
            ValidDataByZoneId = new Dictionary<long, CustomerTargetMatchImportedRow>();
            InvalidDataByRowIndex = new Dictionary<int, CustomerTargetMatchInvalidImportedRow>();
            //AdditionalCountryBEDsByCountryId = new Dictionary<int, DateTime>();
        }
    }
}
