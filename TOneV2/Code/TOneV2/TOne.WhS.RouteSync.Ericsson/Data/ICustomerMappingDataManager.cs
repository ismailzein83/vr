using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
	public interface ICustomerMappingDataManager : IDataManager, IBulkApplyDataManager<CustomerMappingSerialized>
	{
		string SwitchId { get; set; }
		void Initialize(ICustomerMappingInitializeContext context);
		void Finalize(ICustomerMappingFinalizeContext context);
		void ApplyCustomerMappingForDB(object preparedCustomerMapping);
		void CompareTables(ICustomerMappingTablesContext context);
		void RemoveCutomerMappingsFromTempTable(IEnumerable<string> failedRecordsBO);
		void UpdateCustomerMappingsInTempTable(IEnumerable<CustomerMapping> customerMappings);
		void InsertCutomerMappingsToTempTable(IEnumerable<CustomerMapping> customerMappings);
	}

	public interface ICustomerMappingSucceededDataManager : IDataManager, IBulkApplyDataManager<CustomerMappingWithActionType>
	{
		string SwitchId { get; set; }
		void SaveCustomerMappingsSucceededToDB(IEnumerable<CustomerMappingWithActionType> customerMappingsSucceeded);
	}
}