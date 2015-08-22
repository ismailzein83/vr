using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class BlockedLinesResultQuery
    {
        public string StrategiesList { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool GroupDaily { get; set; }

    }
}
