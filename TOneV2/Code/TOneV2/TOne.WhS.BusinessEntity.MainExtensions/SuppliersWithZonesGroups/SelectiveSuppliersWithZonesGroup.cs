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
        public override Guid ConfigId { get { return  new Guid("11eaa8de-6175-47fb-9c16-c428b735d9e2"); } }
        public List<SupplierWithZones> SuppliersWithZones { get; set; }

        public override IEnumerable<SupplierWithZones> GetSuppliersWithZones(ISuppliersWithZonesGroupContext context)
        {
            return this.SuppliersWithZones;
        }

        public override string GetDescription(ISuppliersWithZonesGroupContext context)
        {
            var validSuppliersWithZoneIds = context != null ? context.GetSuppliersWithZones(this) : this.SuppliersWithZones;
            if (validSuppliersWithZoneIds != null)
            {
                CarrierAccountManager manager = new CarrierAccountManager();
                List<int> carrierAccountsIds=new List<int>();
                foreach (SupplierWithZones validZoneId in validSuppliersWithZoneIds)
                {
                    carrierAccountsIds.Add(validZoneId.SupplierId);
                }
                return String.Format("{0}:(Supplier With Zones)", manager.GetDescription(carrierAccountsIds,false,true));
            }
            else
                return null;
        }

        public override void CleanDeletedZoneIds(ISupplierZoneGroupCleanupContext context)
        {
            context.Result = SupplierZoneGroupCleanupResult.NoChange;

            if (this.SuppliersWithZones != null && this.SuppliersWithZones.Count > 0)
            {
                List<int> supplierZoneIdstoDelete = new List<int>();
                foreach (SupplierWithZones supplier in this.SuppliersWithZones)
                {
                    if (supplier.SupplierZoneIds != null && supplier.SupplierZoneIds.Count > 0)
                    {
                        foreach (int deletedZoneId in context.DeletedSupplierZoneIds)
                        {
                            if (supplier.SupplierZoneIds.Contains(deletedZoneId))
                            {
                                context.Result = SupplierZoneGroupCleanupResult.ZonesUpdated;
                                supplier.SupplierZoneIds.Remove(deletedZoneId);
                            }
                        }

                        if (supplier.SupplierZoneIds.Count == 0)
                            supplierZoneIdstoDelete.Add(supplier.SupplierId);
                    }
                }

                this.SuppliersWithZones.RemoveAll(x => supplierZoneIdstoDelete.Contains(x.SupplierId));
                if (this.SuppliersWithZones.Count == 0)
                    context.Result = SupplierZoneGroupCleanupResult.AllZonesRemoved;
            }
        }
    }
}
