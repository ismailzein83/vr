using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class ReleaseCodeStatDetail
    {
        public ReleaseCodeStat Entity { get; set; }

        public string SwitchName { get; set; }
        public string SupplierName { get; set; }
        public string ZoneName { get; set; }
        public string ReleaseCodeDescription { get; set; }
    }
}
