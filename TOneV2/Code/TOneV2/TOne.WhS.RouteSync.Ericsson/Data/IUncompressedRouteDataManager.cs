using System;
using System.Collections.Generic;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
	public interface IUncompressedRouteDataManager : IBulkApplyDataManager<EricssonConvertedRoute>, IDataManager
	{
		string SwitchId { get; set; }
		void ApplyRouteForDB(object preparedRoute);
		IEnumerable<EricssonConvertedRoute> GetUncompressedRoutes();
	}
}
