using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Ericsson
{
	public interface ICustomerMappingInitializeContext
	{ }
	public class CustomerMappingInitializeContext : ICustomerMappingInitializeContext
	{ }

	public class ICustomerMappingFinalizeContext
	{ }

	public class CustomerMappingFinalizeContext : ICustomerMappingFinalizeContext
	{ }

	public interface ICustomerMappingTablesContext
	{
		List<CustomerMappingSerialized> CustomerMappingsToAdd { set; }
		List<CustomerMappingSerialized> CustomerMappingsToUpdate { set; }
		List<CustomerMappingSerialized> CustomerMappingsToDelete { set; }
	}

	public class CustomerMappingTablesContext : ICustomerMappingTablesContext
	{
		public List<CustomerMappingSerialized> CustomerMappingsToAdd { get; set; }
		public List<CustomerMappingSerialized> CustomerMappingsToUpdate { get; set; }
		public List<CustomerMappingSerialized> CustomerMappingsToDelete { get; set; }
	}


	public interface ICustomerMappingSucceededInitializeContext
	{ }
	public class CustomerMappingSucceededInitializeContext : ICustomerMappingInitializeContext
	{ }

	public class ICustomerMappingSucceededFinalizeContext
	{ }
	public class CustomerMappingSucceededFinalizeContext : ICustomerMappingFinalizeContext
	{ }
}
