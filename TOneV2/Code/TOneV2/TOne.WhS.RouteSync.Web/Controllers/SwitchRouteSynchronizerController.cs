using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchRouteSynchronizer")]
    public class SwitchRouteSynchronizerController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSwitchRouteSynchronizerExtensionConfigs")]
        public IEnumerable<SwitchRouteSynchronizerConfig> GetSwitchRouteSynchronizerExtensionConfigs()
        {
            SwitchRouteSynchronizerManager manager = new SwitchRouteSynchronizerManager();
            return manager.GetSwitchRouteSynchronizerExtensionConfigs();
        }

		[HttpGet]
		[Route("GetSwitchRouteSynchronizerHaveSettingsExtensionConfigs")]
		public IEnumerable<SwitchRouteSynchronizerConfig> GetSwitchRouteSynchronizerHaveSettingsExtensionConfigs()
		{
			SwitchRouteSynchronizerManager manager = new SwitchRouteSynchronizerManager();
			return manager.GetSwitchRouteSynchronizerHaveSettingsExtensionConfigs();
		}
	}
}