using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using System.IO;
using System.Web;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodePreparationPreview")]
    public class NP_CodePreparationPreviewController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCountryPreview")]
        public object GetFilteredCountryPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            CountryPreviewManager manager = new CountryPreviewManager();
            return GetWebResponse(input, manager.GetFilteredCountryPreview(input), "Country Preview");
        }

        [HttpPost]
        [Route("GetFilteredZonePreview")]
        public object GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ZonePreviewManager manager = new ZonePreviewManager();
            return GetWebResponse(input, manager.GetFilteredZonePreview(input), "Zone Preview");
        }

        [HttpPost]
        [Route("GetFilteredCodePreview")]
        public object GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            CodePreviewManager manager = new CodePreviewManager();
            return GetWebResponse(input, manager.GetFilteredCodePreview(input), "Code Preview");
        }

    }
}