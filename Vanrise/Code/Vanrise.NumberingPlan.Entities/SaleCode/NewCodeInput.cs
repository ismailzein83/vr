using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class NewCodeInput
    {
        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }
        public List<NewCode> NewCodes { get; set; }
    }
}
