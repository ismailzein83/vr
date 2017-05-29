using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Data;

namespace Retail.BusinessEntity.Business
{
    public class ServiceTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("4F2A2B2F-CAA6-423A-A08F-39DE8587E3BA"); }
        }

        public Guid ServiceTypeId { get; set; }

        public string OverriddenTitle { get; set; }

        public ServiceTypeSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(ServiceTypeOverriddenConfigurationBehavior);
        }

        #region Private Classes

        public class ServiceTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                ServiceTypeManager serviceTypeManager = new ServiceTypeManager();
                List<ServiceType> serviceTypes = new List<ServiceType>();
                foreach (var config in context.Configs)
                {
                    ServiceTypeOverriddenConfiguration serviceTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<ServiceTypeOverriddenConfiguration>("serviceTypeConfig", config.OverriddenConfigurationId);

                    var serviceType = serviceTypeManager.GetServiceType(serviceTypeConfig.ServiceTypeId);
                    serviceType.ThrowIfNull("serviceType", serviceTypeConfig.ServiceTypeId);
                    serviceType = serviceType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(serviceTypeConfig.OverriddenTitle))
                    {
                        serviceType.Name = serviceTypeConfig.OverriddenTitle.Replace(" ", "");
                        serviceType.Title = serviceTypeConfig.OverriddenTitle;
                    }
                    if (serviceTypeConfig.OverriddenSettings != null)
                        serviceType.Settings = serviceTypeConfig.OverriddenSettings;
                    serviceTypes.Add(serviceType);                    
                }
                GenerateScript(serviceTypes, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<ServiceTypeOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).ServiceTypeId).Distinct();
                ServiceTypeManager serviceTypeManager = new ServiceTypeManager();
                List<ServiceType> serviceTypes = new List<ServiceType>();
                foreach (var id in ids)
                {
                    var serviceType = serviceTypeManager.GetServiceType(id);
                    serviceType.ThrowIfNull("serviceType", id);
                    serviceTypes.Add(serviceType);
                }
                GenerateScript(serviceTypes, context.AddEntityScript);
            }

            private void GenerateScript(List<ServiceType> serviceTypes, Action<string, string> addEntityScript)
            {
                IServiceTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IServiceTypeDataManager>();
                dataManager.GenerateScript(serviceTypes, addEntityScript);
            }
        }

        #endregion
    }
}
