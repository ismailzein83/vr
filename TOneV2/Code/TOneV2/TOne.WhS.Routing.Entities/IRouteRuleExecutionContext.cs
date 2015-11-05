using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteRuleExecutionContext
    {
        RouteRule RouteRule { get; }

        int? NumberOfOptions { get; }

        bool TryAddOption(RouteOptionRuleTarget optionTarget);

        ReadOnlyCollection<RouteOptionRuleTarget> GetOptions();

        List<SupplierCodeMatch> GetSupplierCodeMatches(int supplierId);

        List<SupplierCodeMatch> GetAllSuppliersCodeMatches();

        SupplierZoneDetail GetSupplierZoneDetail(long supplierZoneId);
    }
}
