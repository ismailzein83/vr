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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ANumberSaleCode")]
    public class ANumberSaleCodeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredANumberSaleCodes")]
        public object GetFilteredANumberSaleCodes(Vanrise.Entities.DataRetrievalInput<ANumberSaleCodeQuery> input)
        {
            return GetWebResponse(input, new ANumberSaleCodeManager().GetFilteredANumberSaleCodes(input));
        }
        [HttpGet]
        [Route("GetUploadedSaleCodes")]
        public IEnumerable<string> GetUploadedSaleCodes(long fileId)
        {
            return new ANumberSaleCodeManager().GetUploadedSaleCodes(fileId);
        }


        [HttpPost]
        [Route("AddANumberSaleCodes")]
        public ANumberSaleCodesInsertResult AddANumberSaleCodes(ANumberSaleCodesInsertInput input)
        {
            return new ANumberSaleCodeManager().AddANumberSaleCodes(input);
        }

    }
}