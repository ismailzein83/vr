using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class SOMRequestQuery
    {
        public string EntityId { get; set; }

        public DateTime? FromTime { get; set; }

        public DateTime? ToTime { get; set; }
    }
}
