using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class InvalidCDRQuery
    {
        public string TableKey { get; set; }
        public bool IsPartnerCDRs { get; set; }
    }
}
