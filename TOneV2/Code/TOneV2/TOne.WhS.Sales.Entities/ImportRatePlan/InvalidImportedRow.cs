using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class InvalidImportedRow
    {
        public ImportedRow ImportedRow { get; set; }

        public int RowIndex { get; set; }

        public long? ZoneId { get; set; }

        public string ErrorMessage { get; set; }
        public ImportedRowStatus Status { get; set; }
    }
}
