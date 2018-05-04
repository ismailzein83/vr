using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson
{
    public interface IRouteCaseInitializeContext
    {
		int FirstRCNumber { get; }
    }

    public class RouteCaseInitializeContext : IRouteCaseInitializeContext
    {
		public int FirstRCNumber { get; set; }
	}

    public interface IRouteCaseFinalizeContext
    {

    }

    public class RouteCaseFinalizeContext : IRouteCaseFinalizeContext
    {

    }
}
