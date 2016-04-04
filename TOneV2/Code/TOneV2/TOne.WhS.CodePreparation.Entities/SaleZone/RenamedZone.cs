using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class RenamedZone
    {
        public int CountryId { get; set; }
        public int ZoneId { get; set; }

        public string OldZoneName { get; set; }

        public string NewZoneName { get; set; }
    }
}
