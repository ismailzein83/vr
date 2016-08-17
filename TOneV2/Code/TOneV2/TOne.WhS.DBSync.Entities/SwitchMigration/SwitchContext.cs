using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SwitchContext
    {
        public string ConnectionString { get; set; }
        public string SwitchId { get; set; }
        public string Parser { get; set; }
        public DateTime BED { get; set; }
    }
}
