using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TaxCode")]
    public class TaxCodeController : BaseAPIController
    {
        TaxCodeManager _manager = new TaxCodeManager();

        [HttpGet]
        [Route("GetTaxCodesInfo")]
        public IEnumerable<TaxCodeDetail> GetTaxCodesInfo(string filter=null)
        {
            TaxCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<TaxCodeInfoFilter>(filter) : null;
            return _manager.GetTaxCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllTaxCodes")]
        public IEnumerable<TaxCode> GetAllTaxCodes()
        {
            return _manager.GetAllTaxCodes();
        }
    }
}