using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleDetail
    {
        public RouteRule Entity { get; set; }

        public string CssClass { get; set; }

        public string RouteRuleSettingsTypeName { get; set; }

        public string IncludedCodes
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                CodeCriteriaGroupSettings codeCriteriaGroupSettings = this.Entity.Criteria.GetCodeCriteriaGroupSettings();
                if (codeCriteriaGroupSettings == null)
                    return string.Empty;

                return codeCriteriaGroupSettings.GetDescription(this.Entity.GetCodeCriteriaGroupContext());
            }
        }

        public string Customers
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                CustomerGroupSettings customerGroupSettings = this.Entity.Criteria.GetCustomerGroupSettings();
                if (customerGroupSettings == null)
                    return string.Empty;

                return customerGroupSettings.GetDescription(this.Entity.GetCustomerGroupContext());
            }
        }

        public string SaleZones
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                SaleZoneGroupSettings saleZoneGroupSettings = this.Entity.Criteria.GetSaleZoneGroupSettings();
                if (saleZoneGroupSettings == null)
                    return string.Empty;

                return saleZoneGroupSettings.GetDescription(this.Entity.GetSaleZoneGroupContext());
            }
        }
    }
}