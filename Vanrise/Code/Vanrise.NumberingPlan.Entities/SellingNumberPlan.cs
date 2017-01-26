using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class BaseSellingNumberPlan
    {
        public int SellingNumberPlanId { get; set; }

        public string Name { get; set; }
    }

    public class SellingNumberPlan : BaseSellingNumberPlan
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "VR_NumberingPlan_SellingNumberPlan";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("2EC2FB2D-2343-40EB-B72A-9A90F99DF0C7");
    }

    public class SellingNumberPlanToEdit : BaseSellingNumberPlan
    {

    }
}
