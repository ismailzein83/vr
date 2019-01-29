using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.IO;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;
using TestCallAnalysis.Business;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(TestCallAnalysis.Web.Constants.ROUTE_PREFIX + "CaseCDR")]
    public class CaseCDRController: BaseAPIController
    {
        CaseCDREntityManager _manager = new CaseCDREntityManager();

        [HttpGet]
        [Route("GetCaseCDREntity")]
        public GenericBusinessEntity GetCaseCDREntity(Guid businessEntityDefinitionId, [FromUri] Object genericBusinessEntityId)
        {
            genericBusinessEntityId = genericBusinessEntityId as System.IConvertible != null ? genericBusinessEntityId : null;
            return _manager.GetCaseCDREntity(businessEntityDefinitionId,genericBusinessEntityId);
        }

        [HttpPost]
        [Route("UpdateCaseCDRStatus")]
        public object UpdateCaseCDRStatus(GenericBusinessEntityToUpdate caseCDREntity)
        {
            if (!DoesUserHaveEditAccess(caseCDREntity.BusinessEntityDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UpdateCaseCDRStatus(caseCDREntity);
        }

        [HttpGet]
        [Route("DoesUserHaveEditAccess")]
        public bool DoesUserHaveEditAccess(Guid businessEntityDefinitionId)
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.DoesUserHaveEditAccess(businessEntityDefinitionId);
        }
    }
}