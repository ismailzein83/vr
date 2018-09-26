using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleCode")]
    public class WhSBE_SaleCodeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleCodes")]
        public object GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<BaseSaleCodeQueryHandler> input)
        {
            SaleCodeManager manager = new SaleCodeManager();
            return GetWebResponse(input, manager.GetFilteredSaleCodes(input), "Sale Codes");
        }

        [HttpPost]
        [Route("GetSaleCodesByCodeGroups")]
        public List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds)
        {
            SaleCodeManager manager = new SaleCodeManager();
            return manager.GetSaleCodesByCodeGroups(codeGroupsIds);
        }
    }

}