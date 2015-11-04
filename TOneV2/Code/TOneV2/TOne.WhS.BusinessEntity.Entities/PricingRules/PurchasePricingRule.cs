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

        public ISuppliersWithZonesGroupContext GetSuppliersWithZonesGroupContext()
        {
            ISuppliersWithZonesGroupContext groupContext = ContextFactory.CreateContext<ISuppliersWithZonesGroupContext>();
            groupContext.FilterSettings = new SupplierFilterSettings
            {
            };
            return groupContext;
        }

        IEnumerable<int> IRuleSupplierCriteria.SupplierIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SuppliersWithZonesGroupSettings != null)
                {
                    var suppliersWithZones = this.GetSuppliersWithZonesGroupContext().GetSuppliersWithZones(this.Criteria.SuppliersWithZonesGroupSettings);
                    if (suppliersWithZones != null)
                        return suppliersWithZones.Select(itm => itm.SupplierId);
                }
                return null;
            }
        }

        IEnumerable<long> IRuleSupplierZoneCriteria.SupplierZoneIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SuppliersWithZonesGroupSettings != null)
                {
                    var suppliersWithZones = this.GetSuppliersWithZonesGroupContext().GetSuppliersWithZones(this.Criteria.SuppliersWithZonesGroupSettings);
                    if (suppliersWithZones != null)
                        return suppliersWithZones.SelectMany(itm => itm.SupplierZoneIds != null ? itm.SupplierZoneIds : new List<long>());
                }
                return null;
            }
        }
    }
}
