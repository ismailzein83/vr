using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceBaseRule
    {
        public string SourceId { get; set; }

        public string Code { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string ExcludedCodes { get; set; }

        public HashSet<string> ExcludedCodesList { get; set; }

        public bool IncludeSubCode { get; set; }

        public string Reason { get; set; }

    }
}
