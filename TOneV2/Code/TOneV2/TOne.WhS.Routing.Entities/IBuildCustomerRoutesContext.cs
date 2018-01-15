using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public BuildCustomerRoutesContext(RoutingCodeMatches routingCodeMatches, CustomerZoneDetailByZone customerZoneDetails, DateTime? effectiveDate, bool isFuture,
            IEnumerable<RoutingCustomerInfo> activeRoutingCustomerInfos, Dictionary<int, HashSet<int>> customerCountries, int versionNumber, bool isFullRouteBuild, RoutingDatabase routingDatabase)
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
        }
    }
}
