using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class NormalCDRQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string MSISDN { get; set; }

    }
}
