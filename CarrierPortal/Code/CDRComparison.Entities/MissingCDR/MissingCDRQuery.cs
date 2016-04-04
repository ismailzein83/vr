using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class MissingCDRQuery
    {
        public bool IsPartnerCDRs { get; set; }
        public string TableKey { get; set; }
    }
}
