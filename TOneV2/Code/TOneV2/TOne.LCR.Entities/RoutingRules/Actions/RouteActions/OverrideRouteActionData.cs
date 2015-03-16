using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class OverrideRouteActionData
    {
        public List<OverrideOption> Options { get; set; }

        public OverrideRouteNoOptionAction NoOptionAction { get; set; }

        public List<OverrideOption> BackupOptions { get; set; }
    }

    public class OverrideOption
    {
        public string SupplierId { get; set; }

        public short? Percentage { get; set; }
    }

    public enum OverrideRouteNoOptionAction { None = 0, SwitchToLCR = 1, BackupRoute = 2}    
}
