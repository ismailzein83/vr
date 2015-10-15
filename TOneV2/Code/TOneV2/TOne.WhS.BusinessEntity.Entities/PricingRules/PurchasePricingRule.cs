using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PurchasePricingRule : BasePricingRule, IRuleSupplierCriteria, IRuleSupplierZoneCriteria
    {
        public PurchasePricingRuleCriteria Criteria { get; set; }

        public IEnumerable<int> SupplierIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SuppliersWithZonesGroupSettings != null)
                {
                    var suppliersWithZones = this.Criteria.SuppliersWithZonesGroupSettings.GetSuppliersWithZones(null);
                    if (suppliersWithZones != null)
                        return suppliersWithZones.Select(itm => itm.SupplierId);
                }
                return null;
            }
        }

        public IEnumerable<long> SupplierZoneIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SuppliersWithZonesGroupSettings != null)
                {
                    var suppliersWithZones = this.Criteria.SuppliersWithZonesGroupSettings.GetSuppliersWithZones(null);
                    if (suppliersWithZones != null)
                        return suppliersWithZones.SelectMany(itm => itm.SupplierZoneIds != null ? itm.SupplierZoneIds : new List<long>());
                }
                return null;
            }
        }
    }
}
