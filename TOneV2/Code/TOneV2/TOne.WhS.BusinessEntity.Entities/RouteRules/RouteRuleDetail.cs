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
                    return this.Entity.Criteria.CodeCriteriaGroupSettings.GetDescription();

                return string.Empty;
            }
        }

        public string Customers
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.CustomerGroupSettings != null)
                    return this.Entity.Criteria.CustomerGroupSettings.GetDescription();

                return string.Empty;
            }
        }

        public string SaleZones
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.SaleZoneGroupSettings != null)
                    return this.Entity.GetSaleZoneGroupContext().GetGroupDescription(this.Entity.Criteria.SaleZoneGroupSettings);

                return string.Empty;
            }
        }
       
    }
}
