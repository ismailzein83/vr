using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Ericsson.Business;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Ericsson.Controller
{
	[RoutePrefix(Constants.ROUTE_PREFIX + "EricssonBranchRouteSettings")]
	[JSONWithTypeAttribute]
	public class EricssonBranchRouteSettingsController : BaseAPIController
	{
		BranchRouteSettingsManager _manager = new BranchRouteSettingsManager();

		[HttpGet]
		[Route("GetEricssonBranchRouteSettingsConfigs")]
		public IEnumerable<BranchRouteSettingsConfig> GetSwitchLoggerTemplates()
		{
			return _manager.GetEricssonBranchRouteSettingsConfigs();
		}
	}
}