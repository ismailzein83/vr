using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleDetail
    {
        public RouteOptionRule Entity { get; set; }

        public string CssClass { get; set; }

        public string RouteOptionRuleSettingsTypeName { get; set; }
        
        public string Destinations
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                CodeCriteriaGroupSettings codeCriteriaGroupSettings = this.Entity.Criteria.CodeCriteriaGroupSettings;
                if (codeCriteriaGroupSettings != null)
                {
                    string codesDesription = codeCriteriaGroupSettings.GetDescription(this.Entity.GetCodeCriteriaGroupContext());
                    return !string.IsNullOrEmpty(codesDesription) ? string.Concat("Codes: ", codesDesription) : string.Empty;
                }

                SaleZoneGroupSettings saleZoneGroupSettings = this.Entity.Criteria.SaleZoneGroupSettings;
                if (saleZoneGroupSettings != null)
                {
                    string saleZonesDesription = saleZoneGroupSettings.GetDescription(this.Entity.GetSaleZoneGroupContext());
                    return !string.IsNullOrEmpty(saleZonesDesription)
                        ? !saleZonesDesription.Contains("Sale Zones")
                                ? string.Concat("Sale Zones: ", saleZonesDesription)
                                : saleZonesDesription
                        : string.Empty;
                }

                CountryCriteriaGroupSettings countryCriteriaGroupSettings = this.Entity.Criteria.CountryCriteriaGroupSettings;
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
                if (this.Entity.Criteria != null && this.Entity.Criteria.CustomerGroupSettings != null)
                    return this.Entity.Criteria.CustomerGroupSettings.GetDescription(this.Entity.GetCustomerGroupContext());

                return string.Empty;
            }
        }

        public string Suppliers
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.SuppliersWithZonesGroupSettings != null)
                    return this.Entity.Criteria.SuppliersWithZonesGroupSettings.GetDescription(this.Entity.GetSuppliersWithZonesGroupContext());

                return string.Empty;
            }
        }

        public string Excluded
        {
            get
            {
                if (this.Entity.Criteria == null)
                    return string.Empty;

                RoutingExcludedDestinations routingExcludedDestinations = this.Entity.Criteria.ExcludedDestinations;
                if (routingExcludedDestinations == null)
                    return string.Empty;

                return routingExcludedDestinations.GetDescription();
            }
        }
    }
}