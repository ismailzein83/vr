using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleDetail
    {
        public RouteRule Entity { get; set; }

        public string CssClass { get; set; }

        public string RouteRuleSettingsTypeName { get; set; }

        public string Destinations
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                CodeCriteriaGroupSettings codeCriteriaGroupSettings = this.Entity.Criteria.GetCodeCriteriaGroupSettings();
                if (codeCriteriaGroupSettings != null)
                {
                    string codesDesription = codeCriteriaGroupSettings.GetDescription(this.Entity.GetCodeCriteriaGroupContext());
                    return !string.IsNullOrEmpty(codesDesription) ? string.Concat("Codes: ", codesDesription) : string.Empty;
                }

                SaleZoneGroupSettings saleZoneGroupSettings = this.Entity.Criteria.GetSaleZoneGroupSettings();
                if (saleZoneGroupSettings != null)
                {
                    string saleZonesDesription = saleZoneGroupSettings.GetDescription(this.Entity.GetSaleZoneGroupContext());
                    return !string.IsNullOrEmpty(saleZonesDesription)
                        ? !saleZonesDesription.Contains("Sale Zones")
                                ? string.Concat("Sale Zones: ", saleZonesDesription)
                                : saleZonesDesription
                        : string.Empty;
                }

                CountryCriteriaGroupSettings countryCriteriaGroupSettings = this.Entity.Criteria.GetCountryCriteriaGroupSettings();
                if (countryCriteriaGroupSettings != null)
                {
                    string countriesDesription = countryCriteriaGroupSettings.GetDescription(this.Entity.GetCountryCriteriaGroupContext());
                    return !string.IsNullOrEmpty(countriesDesription) ? string.Concat("Countries: ", countriesDesription) : string.Empty;
                }

                return string.Empty;
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

        public string Excluded
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                RoutingExcludedDestinations routingExcludedDestinations = this.Entity.Criteria.GetExcludedDestinations();
                if (routingExcludedDestinations == null)
                    return string.Empty;

                return routingExcludedDestinations.GetDescription();
            }
        }
    }
}