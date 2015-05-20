using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Switch
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }
        public string Symbol { get; set; }
        public string SwitchManagerName { get; set; }
        public virtual string Description { get; set; }
        public bool Enable_CDR_Import { get; set; }
        public bool Enable_Routing { get; set; }
        public DateTime? LastImport { get; set; }
        public DateTime? LastAttempt { get; set; }
        public DateTime? LastRouteUpdate { get; set; }
        public string LastCDRImportTag { get; set; }
    }
}
