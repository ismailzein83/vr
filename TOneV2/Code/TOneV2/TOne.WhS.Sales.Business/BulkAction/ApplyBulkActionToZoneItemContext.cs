using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
	public class ApplyBulkActionToZoneItemContext : IApplyBulkActionToZoneItemContext
	{
		private Func<Dictionary<long, RPRouteDetail>> _getRPRouteDetails;

		public ApplyBulkActionToZoneItemContext(Func<Dictionary<long, RPRouteDetail>> getRPRouteDetails)
		{
			this._getRPRouteDetails = getRPRouteDetails;
		}

		public ZoneItem ZoneItem { get; set; }

		public ZoneChanges ZoneDraft { get; set; }

		public RPRouteDetail GetRPRouteDetail(long zoneId)
		{
			Dictionary<long, RPRouteDetail> rpRouteDetailsByZone = this._getRPRouteDetails();
			return (rpRouteDetailsByZone != null) ? rpRouteDetailsByZone.GetRecord(zoneId) : null;
		}
	}
}
