using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;
using System.Web;
using System.IO;
using Vanrise.Entities;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBEEmailAction")]
    public class GenericBEEmailActionController : BaseAPIController
    {
        GenericBusinessEntityManager _manager = new GenericBusinessEntityManager();

        [HttpPost]
        [Route("SendEmail")]
        public object SendEmail(SendEmailActionInput input)
        {
            if (!_manager.DoesUserHaveActionAccess("SendEmail",input.BusinessEntityDefinitionId, input.GenericBEActionId))
                return GetUnauthorizedResponse();

            GenericBEEmailActionManager manager = new GenericBEEmailActionManager();
            return manager.SendEmail(input);
        }
        [HttpGet]
        [Route("GetEmailTemplate")]
        public EmailTemplateRuntimeEditor GetEmailTemplate([FromUri]object genericBusinessEntityId, Guid businessEntityDefinitionId, Guid genericBEMailTemplateId, Guid genericBEActionId)
        {
            GenericBEEmailActionManager manager = new GenericBEEmailActionManager();
            return manager.GetEmailTemplate(genericBusinessEntityId, businessEntityDefinitionId, genericBEMailTemplateId,genericBEActionId);
        }
        [HttpGet]
        [Route("GetMailTemplateIdByInfoType")]
        public object GetMailTemplateIdByInfoType([FromUri]object genericBusinessEntityId, Guid businessEntityDefinitionId, string infoType)
        {
            GenericBEEmailActionManager manager = new GenericBEEmailActionManager();
            return manager.GetMailTemplateIdByInfoType(genericBusinessEntityId, businessEntityDefinitionId, infoType);
        }
    }
}
