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
		public DateTime? EndDate { get; set; }
		public abstract int GetCarrierAccountId();
		public abstract void GetZoneGroups(IDealGetZoneGroupsContext context);
        public abstract void GetRoutingZoneGroups(IDealGetRoutingZoneGroupsContext context);
        public abstract List<long> GetDealSaleZoneIds();
        public abstract List<long> GetDealSupplierZoneIds();
		public abstract bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext);
        public abstract string GetSaleZoneGroupName(int dealGroupNumber);
        public abstract string GetSupplierZoneGroupName(int dealGroupNumber);
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
		public DealGetZoneGroupsContext(int dealId, DealZoneGroupPart dealZoneGroupPart,bool evaluateRates)
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
        int DealId { get; }

        DealZoneGroupPart DealZoneGroupPart { get; }

        List<DealRoutingSaleZoneGroup> SaleZoneGroups { set; }

        List<DealRoutingSupplierZoneGroup> SupplierZoneGroups { set; }
    }

    public class DealGetRoutingZoneGroupsContext : IDealGetRoutingZoneGroupsContext
    {
        public int DealId { get; set; }
        public DealZoneGroupPart DealZoneGroupPart { get; set; }
        public List<DealRoutingSaleZoneGroup> SaleZoneGroups { get; set; }
        public List<DealRoutingSupplierZoneGroup> SupplierZoneGroups { get; set; }

    }

    public class DealRoutingSaleZoneGroup
    {
        public List<DealRoutingSaleZoneGroupTier> Tiers { get; set; } 
    }

    public class DealRoutingSaleZoneGroupTier
    {
        public int TierNumber { get; set; }

        public decimal? SubstituteRate { get; set; }
    }

    public class DealRoutingSupplierZoneGroup
    {
        public List<DealRoutingSupplierZoneGroupTier> Tiers { get; set; }
    }

    public class DealRoutingSupplierZoneGroupTier
    {
        public int TierNumber { get; set; }

        public decimal? SubstituteRate { get; set; }
    }
}