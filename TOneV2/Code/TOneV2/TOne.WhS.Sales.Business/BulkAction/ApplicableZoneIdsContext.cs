using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
	public class ApplicableZoneIdsContext : IApplicableZoneIdsContext
	{
		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public IEnumerable<long> SaleZoneIds { get; set; }

		public Changes DraftData { get; set; }

		public BulkActionType BulkAction { get; set; }
	}
}
