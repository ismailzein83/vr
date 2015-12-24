using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class CloseCodesInput
    {
        public int SellingNumberPlanId { get; set; }
        public string ZoneName { get; set; }
        public List<string> Codes { get; set; }
        public DateTime CloseDate { get; set; }
    }
}
