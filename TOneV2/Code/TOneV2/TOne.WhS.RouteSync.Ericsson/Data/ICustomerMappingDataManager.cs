using System;
using System.Collections.Generic;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
	public interface ICustomerMappingDataManager : IDataManager, IBulkApplyDataManager<CustomerMappingSerialized>
	{
		string SwitchId { get; set; }
		void ApplyCustomerMappingForDB(object preparedCustomerMapping);
		void Initialize(ICustomerMappingInitializeContext context);
		/*Dictionary<string, List<CustomerMappingByCompare>> GetCustomerMappingDifferences();*/
		void CompareTables(ICustomerMappingTablesContext context);
	}
}