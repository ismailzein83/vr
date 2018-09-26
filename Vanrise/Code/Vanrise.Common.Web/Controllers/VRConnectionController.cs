using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRConnection")]
    [JSONWithTypeAttribute]
    public class VRConnectionController : BaseAPIController
    {
        VRConnectionManager _manager = new VRConnectionManager();

        [HttpPost]
        [Route("GetFilteredVRConnections")]
        public object GetFilteredVRConnections(Vanrise.Entities.DataRetrievalInput<VRConnectionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRConnections(input), "Connections");
        }

        [HttpGet]
        [Route("GetVRConnection")]
        public VRConnection GetVRConnection(Guid vrConnectionId)
        {
            return _manager.GetVRConnection(vrConnectionId,true);
        }

        [HttpPost]
        [Route("AddVRConnection")]
        public Vanrise.Entities.InsertOperationOutput<VRConnectionDetail> AddVRConnection(VRConnection vrConnectionItem)
        {
            return _manager.AddVRConnection(vrConnectionItem);
        }

        [HttpPost]
        [Route("UpdateVRConnection")]
        public Vanrise.Entities.UpdateOperationOutput<VRConnectionDetail> UpdateVRConnection(VRConnection vrConnectionItem)
        {
            return _manager.UpdateVRConnection(vrConnectionItem);
        }

        [HttpGet]
        [Route("GetVRConnectionInfos")]
        public IEnumerable<VRConnectionInfo> GetVRConnectionInfos(string filter = null)
        {
            VRConnectionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRConnectionFilter>(filter) : null;
            return _manager.GetVRConnectionInfos(deserializedFilter);
        }

        [HttpGet]
        [Route("GetVRConnectionConfigTypes")]
        public IEnumerable<VRConnectionConfig> GetVRConnectionConfigTypes()
        {
            return _manager.GetVRConnectionConfigTypes();
        }


    }
}