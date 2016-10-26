using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SupplierZoneGroups
{
    public class SelectiveSupplierZoneGroup :SupplierZoneGroup
    {
        public override Guid ConfigId { get { return  new Guid("65728603-d622-4bc3-a03c-495503e5ed4e"); } }
        public List<SupplierWithZones> SuppliersWithZones { get; set; }

        public override IEnumerable<long> GetSupplierZoneIds(ISupplierZoneGroupContext context)
        {

            List<long> supplierZoneIds = new List<long>();
            SupplierZoneManager manager = new SupplierZoneManager();

            foreach (var item in this.SuppliersWithZones)
            {
                if(item.SupplierZoneIds !=null && item.SupplierZoneIds.Count>0)
                {
                    supplierZoneIds.AddRange(item.SupplierZoneIds);
                }else
                {
                    var supplierZones = manager.GetSupplierZones(item.SupplierId,DateTime.Now);
                    foreach(var supplierZone in supplierZones)
                    {
                        supplierZoneIds.Add(supplierZone.SupplierZoneId);
                    }
                  
                }
            }
            return supplierZoneIds;
        }

        public override string GetDescription(ISupplierZoneGroupContext context)
        {
            var validSupplierZonesIds = context != null ? context.GetGroupSupplierZoneIds(this) : null;
            if (validSupplierZonesIds != null)
            {
                SupplierZoneManager manager = new SupplierZoneManager();
                return manager.GetDescription(validSupplierZonesIds);
            }
            return null;
        }


        public override void CleanDeletedZoneIds(ISupplierZoneGroupCleanupContext context)
        {
            context.Result = SupplierZoneGroupCleanupResult.NoChange;

            if(this.SuppliersWithZones != null && this.SuppliersWithZones.Count > 0)
            {
                List<int> supplierZoneIdstoDelete = new List<int>();
                foreach (SupplierWithZones supplier in this.SuppliersWithZones)
                {
                    if(supplier.SupplierZoneIds != null && supplier.SupplierZoneIds.Count > 0)
                    {
                        foreach (int deletedZoneId in context.DeletedSupplierZoneIds)
                        {
                            if(supplier.SupplierZoneIds.Contains(deletedZoneId))
                            {
                                context.Result = SupplierZoneGroupCleanupResult.ZonesUpdated;
                                supplier.SupplierZoneIds.Remove(deletedZoneId);
                            }
                        }

                        if(supplier.SupplierZoneIds.Count == 0)
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
