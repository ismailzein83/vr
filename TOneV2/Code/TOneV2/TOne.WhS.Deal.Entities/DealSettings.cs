using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealSettings
    {
        public abstract Guid ConfigId { get; }

        public DateTime BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public abstract void GetZoneGroups(IDealGetZoneGroupsContext context);
    }

    public interface IDealGetZoneGroupsContext
    {
        List<DealSaleZoneGroup> SaleZoneGroups { set; }

        List<DealSupplierZoneGroup> SupplierZoneGroups { set; }
    }

    public class DealGetZoneGroupsContext : IDealGetZoneGroupsContext
    {
        public List<DealSaleZoneGroup> SaleZoneGroups { get; set; }

        public List<DealSupplierZoneGroup> SupplierZoneGroups { get; set; }
    }
}