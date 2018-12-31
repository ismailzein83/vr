using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TOne.WhS.Deal.Entities
{
    public enum DealStatus
    {
        [Description("Active")]
        Active = 0,
        [Description("Inactive")]
        Inactive = 1,
        [Description("Draft")]
        Draft = 2
    }

    public enum DealZoneGroupPart { Both, Sale, Cost }
    public abstract class DealSettings
    {
        public abstract Guid ConfigId { get; }
        public DealStatus Status { get; set; }
        public DateTime? DeActivationDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EEDToStore { get; set; }
        public DateTime? EndDate
        {
            get { return EEDToStore.HasValue ? EEDToStore.Value.AddDays(1) : EEDToStore; }
        }
        public DateTime BEDToDisplay
        {
            get { return OffSet.HasValue ? BeginDate.Subtract(OffSet.Value) : BeginDate; }

        }
        public DateTime? EEDToDisplay
        {
            get
            {
                if (EEDToStore.HasValue)
                {
                    return OffSet.HasValue ? EEDToStore.Value.Subtract(OffSet.Value) : EEDToStore;
                }
                return EEDToStore;
            }
        }

        public abstract bool IsBuyDeal { get; }
        public abstract bool IsSellDeal { get;}
        public abstract DateTime? RealEED { get; }
        public abstract DateTime RealBED { get; }
        public abstract int GetCarrierAccountId();
        public abstract void GetZoneGroups(IDealGetZoneGroupsContext context);
        public abstract void GetRoutingZoneGroups(IDealGetRoutingZoneGroupsContext context);
        public abstract List<long> GetDealSaleZoneIds();
        public abstract List<long> GetDealSupplierZoneIds();
        public abstract bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext);
        public abstract string GetSaleZoneGroupName(int dealGroupNumber);
        public abstract string GetSupplierZoneGroupName(int dealGroupNumber);
        public abstract TimeSpan? GetCarrierOffSet(TimeSpan? currentOffSet);
        public abstract DealZoneGroupPart GetDealZoneGroupPart();
        public bool IsRecurrable { get; set; }
        public TimeSpan? OffSet { get; set; }
    }

    public interface IDealGetZoneGroupsContext
    {
        int DealId { get; }

        DealZoneGroupPart DealZoneGroupPart { get; }

        bool EvaluateRates { get; }

        List<BaseDealSaleZoneGroup> SaleZoneGroups { set; }

        List<BaseDealSupplierZoneGroup> SupplierZoneGroups { set; }
    }

    public class DealGetZoneGroupsContext : IDealGetZoneGroupsContext
    {
        public DealGetZoneGroupsContext(int dealId, DealZoneGroupPart dealZoneGroupPart, bool evaluateRates)
        {
            this.DealId = dealId;
            this.DealZoneGroupPart = dealZoneGroupPart;
            this.EvaluateRates = evaluateRates;
        }

        public int DealId { get; set; }

        public DealZoneGroupPart DealZoneGroupPart { get; set; }

        public bool EvaluateRates { get; set; }

        public List<BaseDealSaleZoneGroup> SaleZoneGroups { get; set; }

        public List<BaseDealSupplierZoneGroup> SupplierZoneGroups { get; set; }

    }

    public interface IDealGetRoutingZoneGroupsContext
    {
        DealZoneGroupPart DealZoneGroupPart { get; }

        List<DealRoutingSaleZoneGroup> SaleZoneGroups { set; }

        List<DealRoutingSupplierZoneGroup> SupplierZoneGroups { set; }
    }

    public class DealGetRoutingZoneGroupsContext : IDealGetRoutingZoneGroupsContext
    {
        public DealZoneGroupPart DealZoneGroupPart { get; set; }
        public List<DealRoutingSaleZoneGroup> SaleZoneGroups { get; set; }
        public List<DealRoutingSupplierZoneGroup> SupplierZoneGroups { get; set; }
    }

    public class DealRoutingSaleZoneGroup
    {
        public int DealSaleZoneGroupNb { get; set; }
        public int CurrencyId { get; set; }
        public List<DealRoutingSaleZoneGroupTier> Tiers { get; set; }
    }

    public class DealRoutingSaleZoneGroupTier
    {
        public int TierNumber { get; set; }

        public decimal? SubstituteRate { get; set; }
    }

    public class DealRoutingSupplierZoneGroup
    {
        public int DealSupplierZoneGroupNb { get; set; }
        public int CurrencyId { get; set; }
        public List<DealRoutingSupplierZoneGroupTier> Tiers { get; set; }
    }

    public class DealRoutingSupplierZoneGroupTier
    {
        public int TierNumber { get; set; }

        public decimal? SubstituteRate { get; set; }
    }
}