using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class PreparedCdrBatch
    {
        public List<dynamic> Cdrs { get; set; }
        public PreparedCdrBatch()
        {
            this.Cdrs = new List<dynamic>();
        }
    }
}
