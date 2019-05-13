using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.RDBDataStorage
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RDBDataRecordStorage")]
    public class RDBDataRecordStorageController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRDBDataRecordStorageSettingsFilterConfigs")]
        public IEnumerable<RDBDataRecordStorageSettingsFilterConfig> GetRDBDataRecordStorageSettingsFilterConfigs()
        {
            RDBDataRecordStorageManager _manager = new RDBDataRecordStorageManager();
            return _manager.GetRDBDataRecordStorageSettingsFilterConfigs();
        }
    }
}
