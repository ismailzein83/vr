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

        public DateTime? CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }

    }

    public class SellingNumberPlan : BaseSellingNumberPlan
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "WHS_BE_SellingNumberPlan";        
    }

    public class SellingNumberPlanToEdit : BaseSellingNumberPlan
    {

    }
}
