using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Ericsson
{
	public class EricssonConvertedRoutesPayload
	{
		public EricssonConvertedRoutesPayload()
		{
			this.ConvertedRoutes = new List<EricssonConvertedRoute>();
		}

		public List<EricssonConvertedRoute> ConvertedRoutes { get; set; }
	}
}
