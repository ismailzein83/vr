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
         
        [HttpGet]
        [Route("GetSettingHistoryDetailbyHistoryId")]
        public Setting GetSettingHistoryDetailbyHistoryId(int settingHistoryId)
        {
            SettingManager manager = new SettingManager();
            return manager.GetSettingHistoryDetailbyHistoryId(settingHistoryId);
        }
        [HttpPost]
        [Route("UpdateSetting")]
        public object UpdateSetting(SettingToEdit setting)
        {
           
            SettingManager manager = new SettingManager();
            if (!manager.DoesUserHaveUpdatePermission(setting.SettingId))
                return GetUnauthorizedResponse();
            return manager.UpdateSetting(setting);
                
        }

        [HttpPost]
        [Route("SendTestEmail")]
        public void SendTestEmail(EmailSettingDetail setting)
        {
            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendTestMail(setting.EmailSettingData, setting.FromEmail, setting.ToEmail, setting.Subject, setting.Body);
        }

        [HttpGet]
        [Route("GetSetting")]
        public object GetSetting(Guid settingId)
        {
            SettingManager manager = new SettingManager();
            if (!manager.DoesUserHaveGetSettings(settingId))
                return GetUnauthorizedResponse();
            return manager.GetSetting(settingId,true);
        }

        [HttpGet]
        [Route("GetDistinctSettingCategories")]
        public List<string> GetDistinctSettingCategories()
        {
            SettingManager manager = new SettingManager();
            return manager.GetDistinctSettingCategories();
        }

        [HttpGet]
        [Route("GetSettingsInfo")]
        public IEnumerable<SettingInfo> GetSettingsInfo(string filter = null)
        {
            SettingManager manager = new SettingManager();
            SettingInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<SettingInfoFilter>(filter) : null;
            return manager.GetSettingsInfo(deserializedFilter);
        }
    }
}
