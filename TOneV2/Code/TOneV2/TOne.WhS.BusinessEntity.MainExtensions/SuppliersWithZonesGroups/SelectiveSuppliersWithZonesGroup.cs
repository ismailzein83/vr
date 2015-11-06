using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups
{
    public class SelectiveSuppliersWithZonesGroup : SuppliersWithZonesGroupSettings
    {
        public List<SupplierWithZones> SuppliersWithZones { get; set; }

        public override IEnumerable<SupplierWithZones> GetSuppliersWithZones(ISuppliersWithZonesGroupContext context)
        {
            return this.SuppliersWithZones;
        }

        public override string GetDescription(ISuppliersWithZonesGroupContext context)
        {
            var validZoneIds = context.GetSuppliersWithZones(this);
            if (validZoneIds != null)
            {
                CarrierAccountManager manager = new CarrierAccountManager();
                List<int> carrierAccountsIds=new List<int>();
                foreach (SupplierWithZones validZoneId in validZoneIds)
                {
                    carrierAccountsIds.Add(validZoneId.SupplierId);
                }
                return String.Format("{0}:(Supplier With Zones)", manager.GetDescription(carrierAccountsIds,false,true));
            }
            else
                return null;
        }
    }
}
