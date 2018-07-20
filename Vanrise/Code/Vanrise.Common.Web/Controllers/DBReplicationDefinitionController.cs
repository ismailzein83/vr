using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DBReplicationDefinition")]
    [JSONWithTypeAttribute]
    public class DBReplicationDefinitionController : BaseAPIController
    {
        DBReplicationDefinitionManager _manager = new DBReplicationDefinitionManager();

        [HttpGet]
        [Route("GetDBReplicationDefinitionsInfo")]
        public IEnumerable<DBReplicationDefinitionInfo> GetDBReplicationDefinitionsInfo(string filter = null)
        {
            DBReplicationDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DBReplicationDefinitionInfoFilter>(filter) : null;
            return _manager.GetDBReplicationDefinitionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetDBDefinitionsInfo")]
        public IEnumerable<DBDefinitionInfo> GetDBDefinitionsInfo(Guid dbReplicationDefinitionId, string filter = null)
        {
            DBDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DBDefinitionInfoFilter>(filter) : null;
            return _manager.GetDBDefinitionsInfo(dbReplicationDefinitionId, deserializedFilter);
        }
    }
}