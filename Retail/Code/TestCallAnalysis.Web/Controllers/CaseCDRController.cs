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
using TestCallAnalysis.Entities;

namespace TestCallAnalysis.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(TestCallAnalysis.Web.Constants.ROUTE_PREFIX + "CaseCDR")]
    public class CaseCDRController: BaseAPIController
    {
        static Guid statusBusinessEntityDefinitionId = new Guid("1264c992-479e-45fb-8e8a-7edd54a9bc18");
        CaseCDRManager _manager = new CaseCDRManager();

        [HttpGet]
        [Route("GetCaseCDREntity")]
        public TCAnalCaseCDR GetCaseCDREntity(Guid businessEntityDefinitionId, [FromUri] Object genericBusinessEntityId)
        {
            genericBusinessEntityId = genericBusinessEntityId as System.IConvertible != null ? genericBusinessEntityId : null;
            return _manager.GetCaseCDREntity(businessEntityDefinitionId,genericBusinessEntityId);
        }

        [HttpPost]
        [Route("UpdateCaseCDRStatus")]
        public object UpdateCaseCDRStatus(CaseCDRToUpdate caseCDRToUpdate)
        {
            if (!DoesUserHaveEditAccess(statusBusinessEntityDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UpdateCaseCDRStatus(caseCDRToUpdate);
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