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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ANumberSupplierCode")]
    public class ANumberSupplierCodeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredANumberSupplierCodes")]
        public object GetFilteredANumberSupplierCodes(Vanrise.Entities.DataRetrievalInput<ANumberSupplierCodeQuery> input)
        {
            return GetWebResponse(input, new ANumberSupplierCodeManager().GetFilteredANumberSupplierCodes(input));
        }
        [HttpGet]
        [Route("GetUploadedSupplierCodes")]
        public IEnumerable<string> GetUploadedSupplierCodes(long fileId)
        {
            return new ANumberSupplierCodeManager().GetUploadedSupplierCodes(fileId);
        }


        [HttpPost]
        [Route("AddANumberSupplierCodes")]
        public ANumberSupplierCodesInsertResult AddANumberSupplierCodes(ANumberSupplierCodesInsertInput input)
        {
            return new ANumberSupplierCodeManager().AddANumberSupplierCodes(input);
        }

      
    }
}