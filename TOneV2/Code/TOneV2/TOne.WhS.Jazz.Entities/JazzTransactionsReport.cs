using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Jazz.Entities
{
    public class JazzTransactionsReport
    {
        public Guid ReportDefinitionId { get; set; }
        public string SheetName { get; set; }
        public Guid? TransactionTypeId { get; set; }
        public bool IsTaxTransaction { get; set; }
        public List<JazzTransactionsReportData> ReportData { get; set; }
    }
    public class JazzTransactionsReportData
    {
        public string TransationDescription { get; set; }
        public string TransactionCode { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
    }


}
