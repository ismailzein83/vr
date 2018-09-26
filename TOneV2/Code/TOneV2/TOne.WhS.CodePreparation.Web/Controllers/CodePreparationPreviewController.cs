using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.CodePreparation.Business;
using Vanrise.Web.Base;
using TOne.WhS.CodePreparation.Entities;
using System.IO;
using System.Web;

namespace TOne.WhS.CodePreparation.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodePreparationPreview")]
    public class CodePreparationPreviewController : BaseAPIController
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

        [HttpPost]
        [Route("GetFilteredRatesPreview")]
        public object GetFilteredRatesPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            RatePreviewManager manager = new RatePreviewManager();
            return GetWebResponse(input, manager.GetFilteredRatesPreview(input), "Rates Preview");
        }

        [HttpPost]
        [Route("GetFilteredZonesRoutingProductsPreview")]
        public object GetFilteredZonesRoutingProductsPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ZoneRoutingProductPreviewManager manager = new ZoneRoutingProductPreviewManager();
            return GetWebResponse(input, manager.GetFilteredZonesRoutingProductsPreview(input), "Zones Routing Products Preview");
        }
    }
}