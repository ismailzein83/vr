using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class MoveCodeInput
    {
        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }

        public int ZoneId { get; set; }
        public string CurrentZoneName { get; set; }
        public string NewZoneName { get; set; }
        public List<string> Codes { get; set; }
    }
}
