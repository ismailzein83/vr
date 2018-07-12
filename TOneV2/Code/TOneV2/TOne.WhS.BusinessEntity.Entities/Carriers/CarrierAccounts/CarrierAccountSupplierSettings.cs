using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
	public enum RoutingStatus { Enabled = 0, Blocked = 1 }
	public class CarrierAccountSupplierSettings
	{
		public RoutingStatus RoutingStatus { get; set; }

		public List<ZoneService> DefaultServices { get; set; }

		public int? TimeZoneId { get; set; }

		public int? EffectiveDateDayOffset { get; set; }

		public int? BPBusinessRuleSetId { get; set; }

		public SupplierAutoImportSettings AutoImportSettings { get; set; }
	}

	public class SupplierAutoImportSettings
	{
		public bool IsAutoImportActive { get; set; }
		public string Email { get; set; }
		public string SubjectCode { get; set; }
		public string AttachmentCode { get; set; }
	}
}
