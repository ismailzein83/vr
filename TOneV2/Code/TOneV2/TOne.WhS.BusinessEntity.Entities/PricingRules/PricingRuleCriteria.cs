using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CriteriaType { Sale=0,Purchase=1}
    public class PricingRuleCriteria
    {
        public CriteriaType CriteriaType { get; set; }
    }
}
