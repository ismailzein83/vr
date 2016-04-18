using CDRComparison.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace CDRComparison.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "FileCDRSource")]
    public class FileCDRSourceController : BaseAPIController
    {
        FileCDRSourceManager _manager = new FileCDRSourceManager();

        [HttpGet]
        [Route("GetMaxUncompressedFileSizeInMegaBytes")]
        public decimal? GetMaxUncompressedFileSizeInMegaBytes()
        {
            return _manager.GetMaxUncompressedFileSizeInMegaBytes();
        }
    }
}