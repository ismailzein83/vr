using CP.WhS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReleaseCodeStatistics")]
    [JSONWithTypeAttribute]
    public class ReleaseCodeStatisticsController : BaseAPIController
    {
        ReleaseCodeStatisticsManager _releaseCodeStatisticsManager = new ReleaseCodeStatisticsManager();
        [HttpPost]
        [Route("GetFilteredReleaseCodeStatistics")]
        public object GetFilteredReleaseCodeStatistics(DataRetrievalInput<ReleaseCodeQuery> input)
        {
            return GetWebResponse(input, _releaseCodeStatisticsManager.GetFilteredReleaseCodeStatistics(input));
        }
    }
}