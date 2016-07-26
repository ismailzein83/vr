using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Web.Online.Models;

namespace TOne.Web.Online.Controllers
{
    public class MobileAppController : ApiController
    {
        [HttpGet]
        public MobileAppConfig GetConfig()
        {
            return new MobileAppConfig
            {
                UserDisplayName = SecurityToken.Current.UserDisplayName
            };
        }

        [HttpGet]
        public IEnumerable<MobileConfiguration> GetMobileConfigurations(int? configId)
        {
            MobileConfigurationManager manager = new MobileConfigurationManager();
            return manager.GetConfigurations(configId);
        }

        [HttpGet]
        public bool AddMobileConfiguration(string configuration)
        {
            MobileConfigurationManager manager = new MobileConfigurationManager();
            MobileConfiguration mobileConfiguration = Vanrise.Common.Serializer.Deserialize<MobileConfiguration>(configuration);
            int configId;
            return manager.AddMobileConfiguration(mobileConfiguration, out configId);
        }
        [HttpGet]
        public bool UpdateMobileConfiguration(string configuration)
        {
            MobileConfigurationManager manager = new MobileConfigurationManager();
            MobileConfiguration mobileConfiguration = Vanrise.Common.Serializer.Deserialize<MobileConfiguration>(configuration);
            return manager.UpdateMobileConfiguration(mobileConfiguration);
        }
    }
}