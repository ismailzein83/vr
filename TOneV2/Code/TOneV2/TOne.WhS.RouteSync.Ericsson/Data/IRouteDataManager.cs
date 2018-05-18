using System;
using System.Collections.Generic;
using Vanrise.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
	public interface IRouteDataManager : IBulkApplyDataManager<EricssonConvertedRoute>, IDataManager
    {
        string SwitchId { get; set; }
        void Initialize(IRouteInitializeContext context);
		void Finalize(IRouteFinalizeContext context);
		void ApplyRouteForDB(object preparedRoute);
		void CompareTables(IRouteCompareTablesContext context);
		void InsertRoutesToTempTable(IEnumerable<EricssonConvertedRoute> routes);
		void RemoveRoutesFromTempTable(IEnumerable<EricssonConvertedRoute> routes);
		void UpdateRoutesInTempTable(IEnumerable<EricssonConvertedRoute> routes);
		void CopyCustomerRoutesToTempTable(IEnumerable<string> customerBOs);
	}
	public interface IRouteSucceededDataManager : IBulkApplyDataManager<EricssonConvertedRoute>, IDataManager
	{
		string SwitchId { get; set; }
		void SaveRoutesSucceededToDB(Dictionary<string, List<EricssonRouteWithCommands>> routesWithCommandsByBO);
	}
}