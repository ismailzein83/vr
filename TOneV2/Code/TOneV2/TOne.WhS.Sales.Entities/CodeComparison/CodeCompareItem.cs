using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public enum CodeCompareIndicator { None = 0, Highlight = 1 };

    public enum CodeCompareAction { New = 0, Delete = 1 };

    public class CodeCompareItem
    {
        public string Code { get; set; }

        public string SaleZone { get; set; }

        public string SaleCode { get; set; }

        public CodeCompareIndicator SaleCodeIndicator { get; set; }

        public List<CodeCompareSupplierItem> SupplierItems { get; set; }

        public int OccurrenceInSuppliers { get; set; }

        public CodeCompareIndicator OccurrenceInSuppliersIndicator { get; set; }

        public int AbsenceInSuppliers { get; set; }

        public CodeCompareIndicator AbsenceInSuppliersIndicator { get; set; }

        public CodeCompareAction Action { get; set; }
    }

    public class CodeCompareSupplierItem
    {
        public string SupplierZone { get; set; }

        public string SupplierCode { get; set; }

        public CodeCompareIndicator SupplierCodeIndicator { get; set; }
    }
}
