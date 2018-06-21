using System;
using System.ComponentModel;

namespace TOne.WhS.SupplierPriceList.Entities
{
	public enum Status : short
	{
		NotChanged = 0,
		New = 1,
		Updated = 2,
	}

	/*public enum SupplierPriceListType
	{
		[Description("Rate Change")]
		RateChange = 0,

		[Description("Country")]
		Country = 1,

		[Description("Full")]
		Full = 2
	}*/

	public enum ReceivedPricelistStatus
	{
		[Description("Received")]
		Received = 0,

		[Description("Processing")]
		Processing = 10,

		[Description("Succeeded")]
		Succeeded = 60,

		[Description("Completed With No Changes")]
		CompletedWithNoChanges = 65,

		[Description("Failed Due To Business Rule Error")]
		FailedDueToBusinessRuleError = 70,

		[Description("Failed Due To Processing Error")]
		FailedDueToProcessingError = 75,

		[Description("Failed Due To Configuration Error")]
		FailedDueToConfigurationError = 80,

		[Description("Failed Due To Received Mail Error")]
		FailedDueToReceivedMailError = 85
	}
}
