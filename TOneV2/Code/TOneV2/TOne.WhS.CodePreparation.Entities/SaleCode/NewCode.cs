using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class NewCode
    {
        public string Code { get; set; }
        public long? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string OldZoneName { get; set; }
        public DateTime OldCodeBED { get; set; }
        public int CountryId { get; set; }
    }
}
