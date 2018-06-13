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

	public enum SupplierPriceListType
	{
		[Description("Rate Change")]
		RateChange = 0,

		[Description("Country")]
		Country = 1,

		[Description("Full")]
		Full = 2
	}

	public enum ReceivedPricelistStatus
	{
		[Description("Received")]
		Received = 0,

		[Description("Processing")]
		Processing = 10,

		[Description("Succeeded")]
		Succeeded = 60,

		[Description("Completed with no changes")]
		CompletedWithNoChanges = 65,

		[Description("Failed due to business rule error")]
		FailedDueToBusinessRuleError = 70,

		[Description("Failed due to processing error")]
		FailedDueToProcessingError = 75,

		[Description("Failed due to configuration error")]
		FailedDueToConfigurationError = 80,

		[Description("Failed due to received mail error")]
		FailedDueToReceivedMailError = 85
	}
}
