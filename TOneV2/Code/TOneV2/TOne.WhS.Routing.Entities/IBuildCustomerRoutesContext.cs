using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface IBuildCustomerRoutesContext
    {
        List<SaleZoneDefintion> SaleZoneDefintions { get; }

        CustomerZoneDetailByZone CustomerZoneDetails { get; }

        List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; }

        SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; }

        DateTime? EntitiesEffectiveOn { get; }

        bool EntitiesEffectiveInFuture { get; }

        IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; }

        Dictionary<int, HashSet<int>> CustomerCountries { get; set; }

        int VersionNumber { get; }

        bool IsFullRouteBuild { get; }

        RoutingDatabase RoutingDatabase { get; }

        bool GenerateAnalysisData { get; }

        Dictionary<CustomerSaleZone, SaleZoneOptionsMarginStaging> SaleZoneOptionsMarginStagingByCustomerSaleZone { get; }
    }

    public class BuildCustomerRoutesContext : IBuildCustomerRoutesContext
    {
        public List<SaleZoneDefintion> SaleZoneDefintions { get; set; }

        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }

        public DateTime? EntitiesEffectiveOn { get; set; }

        public bool EntitiesEffectiveInFuture { get; set; }

        public IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; set; }

        public Dictionary<int, HashSet<int>> CustomerCountries { get; set; }

        public int VersionNumber { get; set; }

        public bool IsFullRouteBuild { get; set; }

        public RoutingDatabase RoutingDatabase { get; set; }

        public bool GenerateAnalysisData { get; set; }

        public Dictionary<CustomerSaleZone, SaleZoneOptionsMarginStaging> SaleZoneOptionsMarginStagingByCustomerSaleZone { get; set; }

        public BuildCustomerRoutesContext(RoutingCodeMatches routingCodeMatches, CustomerZoneDetailByZone customerZoneDetails, DateTime? effectiveDate, bool isFuture,
            IEnumerable<RoutingCustomerInfo> activeRoutingCustomerInfos, Dictionary<int, HashSet<int>> customerCountries, int versionNumber, bool isFullRouteBuild,
            RoutingDatabase routingDatabase, bool generateAnalysisData, Dictionary<CustomerSaleZone, SaleZoneOptionsMarginStaging> saleZoneOptionsMarginStagingByCustomerSaleZone)
        {
            this.SaleZoneDefintions = routingCodeMatches.SaleZoneDefintions;
            this.SupplierCodeMatches = routingCodeMatches.SupplierCodeMatches;
            this.SupplierCodeMatchesBySupplier = routingCodeMatches.SupplierCodeMatchesBySupplier;
            this.CustomerZoneDetails = customerZoneDetails;
            this.EntitiesEffectiveOn = effectiveDate;
            this.EntitiesEffectiveInFuture = isFuture;
            this.ActiveRoutingCustomerInfos = activeRoutingCustomerInfos;
            this.CustomerCountries = customerCountries;
            this.VersionNumber = versionNumber;
            this.IsFullRouteBuild = isFullRouteBuild;
            this.RoutingDatabase = routingDatabase;
            this.GenerateAnalysisData = generateAnalysisData;
            this.SaleZoneOptionsMarginStagingByCustomerSaleZone = saleZoneOptionsMarginStagingByCustomerSaleZone;
        }
    }
}