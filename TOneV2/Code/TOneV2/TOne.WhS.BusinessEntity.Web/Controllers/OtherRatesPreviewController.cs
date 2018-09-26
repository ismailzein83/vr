using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "OtherRatesPreview")]
    public class OtherRatesPreviewController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRatesPreview")]
        public object GetFilteredRatePreviews(Vanrise.Entities.DataRetrievalInput<BaseOtherRatesPreviewQueryHandler> input)
        {
            OtherRatesPreviewManager manager = new OtherRatesPreviewManager();
            return GetWebResponse(input, manager.GetFilteredRatesPreview(input), "Other Rates");
        }

    }
}