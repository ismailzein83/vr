using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public enum DealStatus { Active, Inactive, Draft }
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
        public abstract List<long> GetDealSaleZoneIds();
        public abstract List<long> GetDealSupplierZoneIds();
		public abstract bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext);
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
}