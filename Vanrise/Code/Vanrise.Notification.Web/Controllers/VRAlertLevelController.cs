using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRAlertLevel")]
    [JSONWithTypeAttribute]
    public class VRAlertLevelController : BaseAPIController
    {
        VRAlertLevelManager _manager = new VRAlertLevelManager();

        [HttpPost]
        [Route("GetFilteredAlertLevels")]
        public object GetFilteredAlertLevels(Vanrise.Entities.DataRetrievalInput<VRAlertLevelQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAlertLevels(input));
        }

        [HttpPost]
        [Route("AddAlertLevel")]
        public Vanrise.Entities.InsertOperationOutput<VRAlertLevelDetail> AddAlertLevel(VRAlertLevel alertLevelItem)
        {
            return _manager.AddAlertLevel(alertLevelItem);
        }

        [HttpPost]
        [Route("UpdateAlertLevel")]
        public Vanrise.Entities.UpdateOperationOutput<VRAlertLevelDetail> UpdateAlertLevel(VRAlertLevel alertLevelItem)
        {
            return _manager.UpdateAlertLevel(alertLevelItem);
        }

        [HttpGet]
        [Route("GetAlertLevel")]
        public VRAlertLevel GetAlertLevel(Guid alertLevelId)
        {
            return _manager.GetAlertLevel(alertLevelId);
        }
        [HttpGet]
        [Route("GetAlertLevelsInfo")]
        public IEnumerable<VRAlertLevelInfo> GetAlertLevelsInfo(string filter = null)
        {
            VRAlertLevelInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRAlertLevelInfoFilter>(filter) : null;
            return _manager.GetAlertLevelsInfo(deserializedFilter);
        }

       
    }
}