using System;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public interface IRouteDataManager : IBulkApplyDataManager<EricssonConvertedRoute>, IDataManager
    {
        string SwitchId { get; set; }
        
        void Initialize(IRouteInitializeContext context);

        void Finalize(IRouteFinalizeContext context);

		void ApplyRouteForDB(object preparedRoute);

		void CompareTables(IRouteCompareTablesContext context);
	}
}