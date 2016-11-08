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
        public object UpdateSetting(Setting setting)
        {
            SettingManager manager = new SettingManager();
            try {
                return manager.UpdateSetting(setting);
            }
            catch (UnauthorizedAccessException ex)
            {
                return GetUnauthorizedResponse();
            }
                
        }

        [HttpGet]
        [Route("GetSetting")]
        public object GetSetting(Guid settingId)
        {
            SettingManager manager = new SettingManager();
            try
            {
                return manager.GetSetting(settingId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return GetUnauthorizedResponse();
            }

           
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
