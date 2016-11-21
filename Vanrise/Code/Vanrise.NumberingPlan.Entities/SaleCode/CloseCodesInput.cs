using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class CloseCodesInput
    {
        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }
        public string ZoneName { get; set; }
        public List<string> Codes { get; set; }
    }
}
