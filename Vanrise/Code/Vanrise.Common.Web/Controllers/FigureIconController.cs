using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "FigureIcon")]
    public class VRCommon_FigureIconController : BaseAPIController
    {
        [HttpGet]
        [Route("GetFigureIconsInfo")]
        public IEnumerable<VRFigureIcon> GetFigureIconsInfo()
        {
            FigureIconManger manager = new FigureIconManger();
            return manager.GetFigureIconsInfo();
        }

    }
}