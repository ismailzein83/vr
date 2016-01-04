using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class MovedCode
    {
      
        public string Code { get; set; }
        public string ZoneName { get; set; }
        public string OldZoneName { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public bool IsExcluded { get; set; }
    }
}
