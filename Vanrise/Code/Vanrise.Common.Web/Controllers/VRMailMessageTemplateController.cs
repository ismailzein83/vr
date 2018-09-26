using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRMailMessageTemplate")]
    [JSONWithTypeAttribute]
    public class VRMailMessageTemplateController : BaseAPIController
    {
        VRMailMessageTemplateManager _manager = new VRMailMessageTemplateManager();

        [HttpPost]
        [Route("GetFilteredMailMessageTemplates")]
        public object GetFilteredMailMessageTemplates(Vanrise.Entities.DataRetrievalInput<VRMailMessageTemplateQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredMailMessageTemplates(input), "Mail Message Templates");
        }

        [HttpGet]
        [Route("GetMailMessageTemplate")]
        public VRMailMessageTemplate GetMailMessageTemplate(Guid VRMailMessageTemplateId)
        {
            return _manager.GetMailMessageTemplate(VRMailMessageTemplateId,true);
        }

        [HttpPost]
        [Route("AddMailMessageTemplate")]
        public Vanrise.Entities.InsertOperationOutput<VRMailMessageTemplateDetail> AddMailMessageTemplate(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            return _manager.AddMailMessageTemplate(vrMailMessageTemplateItem);
        }

        [HttpPost]
        [Route("UpdateMailMessageTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<VRMailMessageTemplateDetail> UpdateMailMessageTemplate(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            return _manager.UpdateMailMessageTemplate(vrMailMessageTemplateItem);
        }

        [HttpGet]
        [Route("GetMailMessageTemplatesInfo")]
        public IEnumerable<VRMailMessageTemplateInfo> GetStyleDefinitionsInfo(string filter = null)
        {
            VRMailMessageTemplateFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRMailMessageTemplateFilter>(filter) : null;
            return _manager.GetMailMessageTemplatesInfo(deserializedFilter);
        }
    }
}