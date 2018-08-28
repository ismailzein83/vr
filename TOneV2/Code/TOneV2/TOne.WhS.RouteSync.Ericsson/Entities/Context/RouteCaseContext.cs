using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
	public interface IRouteCaseInitializeContext
	{
		int FirstRCNumber { get; }
		BranchRouteSettings BranchRouteSettings { get; }

	}

	public class RouteCaseInitializeContext : IRouteCaseInitializeContext
	{
		public int FirstRCNumber { get; set; }
		public BranchRouteSettings BranchRouteSettings { get; set; }
	}

	public interface IRouteCaseFinalizeContext
	{

	}

	public class RouteCaseFinalizeContext : IRouteCaseFinalizeContext
	{

	}
}
