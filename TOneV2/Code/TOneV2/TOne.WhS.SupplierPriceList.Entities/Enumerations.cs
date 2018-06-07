using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		RateChange = 0,
		Country = 1,
		Full = 2
	}

	public enum ReceivedPricelistStatus
	{
		Received = 0,
		Processing = 1,
		Succeeded = 2,
		CompletedWithNoChanges = 3,
		FailedDueToBusinessRuleError = 4,
		FailedDueToProcessingError = 5,
		FailedDueToConfigurationError = 6,
		FailedDueToReceivedMailError = 7
	}
}
