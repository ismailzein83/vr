using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRuleDetail
    {
        public RouteRule Entity { get; set; }

        public string IncludedCodes 
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.CodeCriteriaGroupSettings != null)
                    return string.Join(",", this.Entity.Criteria.CodeCriteriaGroupSettings.GetCodeCriterias(null).Select(item => item.Code));

                return string.Empty;
            }
        }
    }
}
