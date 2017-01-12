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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CreditClass")]
    [JSONWithTypeAttribute]
    public class CreditClassController : BaseAPIController
    {
        CreditClassManager _manager = new CreditClassManager();

        [HttpPost]
        [Route("GetFilteredCreditClasses")]
        public object GetFilteredCreditClasses(Vanrise.Entities.DataRetrievalInput<CreditClassQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredCreditClasses(input));
        }

        [HttpGet]
        [Route("GetCreditClass")]
        public CreditClass GetCreditClass(int creditClassId)
        {
            return _manager.GetCreditClass(creditClassId);
        }

        [HttpPost]
        [Route("AddCreditClass")]
        public Vanrise.Entities.InsertOperationOutput<CreditClassDetail> AddCreditClass(CreditClass creditClassItem)
        {
            return _manager.AddCreditClass(creditClassItem);
        }

        [HttpPost]
        [Route("UpdateCreditClass")]
        public Vanrise.Entities.UpdateOperationOutput<CreditClassDetail> UpdateCreditClass(CreditClass creditClassItem)
        {
            return _manager.UpdateCreditClass(creditClassItem);
        }

        [HttpGet]
        [Route("GetCreditClassesInfo")]
        public IEnumerable<CreditClassInfo> GetCreditClassesInfo(string filter = null)
        {
            CreditClassFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<CreditClassFilter>(filter) : null;
            return _manager.GetCreditClassesInfo(deserializedFilter);
        }
    }
}