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
        
        public abstract bool ValidateDataBeforeSave();
    }

    public interface IDealGetZoneGroupsContext
    {
        int DealId { get; }

        List<DealSaleZoneGroup> SaleZoneGroups { set; }

        List<DealSupplierZoneGroup> SupplierZoneGroups { set; }
    }

    public class DealGetZoneGroupsContext : IDealGetZoneGroupsContext
    {
        public int DealId { get; set; }

        public List<DealSaleZoneGroup> SaleZoneGroups { get; set; }

        public List<DealSupplierZoneGroup> SupplierZoneGroups { get; set; }
    }
}