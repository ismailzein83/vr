using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseSellingNumberPlan
    {
        public int SellingNumberPlanId { get; set; }

        public string Name { get; set; }
    }

    public class SellingNumberPlan : BaseSellingNumberPlan
    {
        
    }

    public class SellingNumberPlanToEdit : BaseSellingNumberPlan
    {

    }
}
