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
		New = 0,
		Processing = 1,
		Failed = 2,
		Succeeded = 3
	}
}
