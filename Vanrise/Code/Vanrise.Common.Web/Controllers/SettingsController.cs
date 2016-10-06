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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Settings")]
    [JSONWithTypeAttribute]
    public class SettingsController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSettings")]
        public object GetFilteredSettings(Vanrise.Entities.DataRetrievalInput<SettingQuery> input)
        {
            SettingManager manager = new SettingManager();
            return GetWebResponse(input, manager.GetFilteredSettings(input));
        }

        [HttpPost]
        [Route("UpdateSetting")]
        public Vanrise.Entities.UpdateOperationOutput<SettingDetail> UpdateSetting(Setting setting)
        {
            SettingManager manager = new SettingManager();
            return manager.UpdateSetting(setting);
        }

        [HttpGet]
        [Route("GetSetting")]
        public Setting GetSetting(Guid settingId)
        {
            SettingManager manager = new SettingManager();
            return manager.GetSetting(settingId);
        }

        [HttpGet]
        [Route("GetDistinctSettingCategories")]
        public List<string> GetDistinctSettingCategories()
        {
            SettingManager manager = new SettingManager();
            return manager.GetDistinctSettingCategories();
        }
    }
}
