using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public abstract class BulkActionZoneFilter
	{
		public abstract Guid ConfigId { get; }

		public abstract IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context);
	}

	public interface IApplicableZoneIdsContext
	{
		IEnumerable<long> SaleZoneIds { get; }

		Changes DraftData { get; }

		BulkActionType BulkAction { get; }
	}
}
