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
		void Swap(ICustomerMappingFinalizeContext context);
		void Finalize(ICustomerMappingFinalizeContext context);
		void ApplyCustomerMappingForDB(object preparedCustomerMapping);
		void CompareTables(ICustomerMappingTablesContext context);
	}

	public interface ICustomerMappingSucceededDataManager : IDataManager, IBulkApplyDataManager<CustomerMappingWithActionType>
	{
		string SwitchId { get; set; }
		void SaveCustomerMappingsSucceededToDB(IEnumerable<CustomerMappingWithActionType> customerMappingsSucceeded);
	}
}