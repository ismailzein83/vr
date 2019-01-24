using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Jazz.Entities
{
    public class JazzTransactionsReport
    {
        public string ReportName { get; set; }
        public Guid ReportDefinitionId { get; set; }
        public List<JazzTransactionsReportData> ReportData { get; set; }
    }
    public class JazzTransactionsReportData
    {
        public string AccountDescription { get; set; }
        public string AccountCode { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
    }
}
