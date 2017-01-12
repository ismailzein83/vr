using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public interface ISalePricelistFileContext
	{
		int SellingNumberPlanId { get; }

		long ProcessInstanceId { get; set; }

		IEnumerable<int> CustomerIds { get; }

		IEnumerable<SalePLZoneChange> ZoneChanges { get; }

		DateTime EffectiveDate { get; }

		SalePLChangeType ChangeType { get; }

		IEnumerable<int> EndedCountryIds { get; }

		DateTime? CountriesEndedOn { get; }
	}
}
