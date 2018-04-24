using System;
using System.Collections.Generic;
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
	}

	public interface ICustomerMappingSucceededDataManager : IDataManager, IBulkApplyDataManager<CustomerMappingSerialized>
	{
		string SwitchId { get; set; }
		void Finalize(ICustomerMappingSucceededFinalizeContext context);
		void ApplyCustomerMappingSucceededForDB(object preparedCustomerMapping);
	}
}