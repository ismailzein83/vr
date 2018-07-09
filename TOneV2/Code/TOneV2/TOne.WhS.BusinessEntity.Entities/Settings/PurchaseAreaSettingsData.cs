using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class PurchaseAreaSettingsData : Vanrise.Entities.SettingData
	{
		public int EffectiveDateDayOffset { get; set; }
		public int RetroactiveDayOffset { get; set; }
		public decimal MaximumRate { get; set; }
		public long MaximumCodeRange { get; set; }
        public int AcceptableIncreasedRate { get; set; }
        public int AcceptableDecreasedRate { get; set; }
        public int AcceptableZoneClosingPercentage { get; set; }
        public bool CodeGroupVerfifcation { get; set; }
	}

	public class PricelistTypeMapping
	{
		public string Subject { get; set; }
		public SupplierPricelistType PricelistType { get; set; } // when remove enum class change SupplierPricelistType to SupplierPriceListType
	}

	public enum SupplierPricelistType // to be removed when the reference will be added
	{
		[Description("Rate Change")]
		RateChange = 0,

		[Description("Country")]
		Country = 1,

		[Description("Full")]
		Full = 2
	}

	public class SupplierAutoImportTemplate
	{
		public AutoImportEmailTypeEnum TemplateType { get; set; }
		public Guid EmailTemplateId { get; set; }
		public bool AttachPricelist { get; set; }
	}

	public class InternalAutoImportTemplate
	{
		public AutoImportEmailTypeEnum TemplateType { get; set; }
		public Guid? EmailTemplateId { get; set; }
		public bool AttachPricelist { get; set; }
	}


	public enum AutoImportEmailTypeEnum
	{
		[Description("Received")]
		Received = 0,

		[Description("Succeeded")]
		Succeeded = 1,

		[Description("Failed")]
		Failed = 2,
	}
}
