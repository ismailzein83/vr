using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Notification
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordAlertRule")]
    [JSONWithTypeAttribute]
    public class DataRecordAlertRuleController : BaseAPIController
    {
        DataRecordAlertRuleManager _manager = new DataRecordAlertRuleManager();

        [HttpGet]
        [Route("GetDataRecordAlertRuleConfigs")]
        public IEnumerable<DataRecordAlertRuleConfig> GetDataRecordAlertRuleConfigs()
        {
            return _manager.GetDataRecordAlertRuleConfigs();
        }
    }
}
