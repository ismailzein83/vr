using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Entities.SMS;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SMSMessageTemplate")]
    [JSONWithTypeAttribute]
    public class SMSMessageTemplateController : BaseAPIController
    {
        SMSMessageTemplateManager _manager = new SMSMessageTemplateManager();

        [HttpPost]
        [Route("GetFilteredSMSMessageTemplates")]
        public object GetFilteredSMSMessageTemplates(Vanrise.Entities.DataRetrievalInput<SMSMessageTemplateQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSMSMessageTemplates(input));
        }

        [HttpGet]
        [Route("GetSMSMessageTemplate")]
        public SMSMessageTemplate GetSMSMessageTemplate(Guid SMSMessageTemplateId)
        {
            return _manager.GetSMSMessageTemplate(SMSMessageTemplateId, true);
        }

        [HttpGet]
        [Route("GetSMSMessageTemplatesInfo")]
        public IEnumerable<SMSMessageTemplateInfo> GetSMSMessageTemplatesInfo(string filter = null)
        {
            SMSMessageTemplateFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<SMSMessageTemplateFilter>(filter) : null;
            return _manager.GetSMSMessageTemplatesInfo(deserializedFilter);
        }

        [HttpPost]
        [Route("AddSMSMessageTemplate")]
        public InsertOperationOutput<SMSMessageTemplateDetail> AddSMSMessageTemplate(SMSMessageTemplate smsMessageTemplateItem)
        {
            return _manager.AddSMSMessageTemplate(smsMessageTemplateItem);
        }

        [HttpPost]
        [Route("UpdateSMSMessageTemplate")]
        public UpdateOperationOutput<SMSMessageTemplateDetail> UpdateSMSMessageTemplate(SMSMessageTemplate smsMessageTemplateItem)
        {
            return _manager.UpdateSMSMessageTemplate(smsMessageTemplateItem);
        }
    }
}