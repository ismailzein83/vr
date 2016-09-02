using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorDeclaredInfo")]
    public class OperatorDeclaredInfoController : BaseAPIController
    {
        OperatorDeclaredInfoManager _manager = new OperatorDeclaredInfoManager();

        [HttpPost]
        [Route("GetFilteredOperatorDeclaredInfos")]
        public object GetFilteredOperatorDeclaredInfos(Vanrise.Entities.DataRetrievalInput<OperatorDeclaredInfoQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredOperatorDeclaredInfos(input));
        }

        [HttpGet]
        [Route("GetOperatorDeclaredInfo")]
        public OperatorDeclaredInfo GetOperatorDeclaredInfo(int operatorDeclaredInfoId)
        {
            return _manager.GetOperatorDeclaredInfo(operatorDeclaredInfoId);
        }

        [HttpPost]
        [Route("AddOperatorDeclaredInfo")]
        public Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInfoDetail> AddOperatorDeclaredInfo(OperatorDeclaredInfo operatorDeclaredInfo)
        {
            return _manager.AddOperatorDeclaredInfo(operatorDeclaredInfo);
        }

        [HttpPost]
        [Route("UpdateOperatorDeclaredInfo")]
        public Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInfoDetail> UpdateOperatorDeclaredInfo(OperatorDeclaredInfo operatorDeclaredInfo)
        {
            return _manager.UpdateOperatorDeclaredInfo(operatorDeclaredInfo);
        }
    }
}