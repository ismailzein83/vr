using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.Entities
{
    public class TCRRingoReportFilter
    {
        public TCRReportType ReportType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<string> Operator { get; set; }
    }
}
