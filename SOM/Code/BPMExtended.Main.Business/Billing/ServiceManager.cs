using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using System;
using System.Collections.Generic;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class ServiceManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        public List<ServiceInfo> GetServicesInfo()
        {
            var servicesInfoItems = new List<ServiceInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items= client.Get<List<ServiceDefinition>>(String.Format("api/SOM.ST/Billing/GetServicesDefinition"));
                foreach (var item in items)
                {
                    var serviceInfoItem = ServiceDefinitionToInfoMapper(item);
                    servicesInfoItems.Add(serviceInfoItem);
                }
            }
            return servicesInfoItems;
        }
        public List<POSServiceInfo> GetPOSServicesInfo()
        {
            var servicesInfoItems = new List<POSServiceInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Get<List<ServiceDefinition>>(String.Format("api/SOM.ST/Billing/GetPOSServices"));
                foreach (var item in items)
                {
                    var serviceInfoItem = POSServiceDefinitionToInfoMapper(item);
                    servicesInfoItems.Add(serviceInfoItem);
                }
            }
            return servicesInfoItems;
        }

        #region Mappers
        public ServiceInfo ServiceDefinitionToInfoMapper(ServiceDefinition item)
        {
            return new ServiceInfo
            {
                Name = item.Title,
                ServiceId = item.PublicId

            };
        }

        public POSServiceInfo POSServiceDefinitionToInfoMapper(ServiceDefinition item)
        {
            return new POSServiceInfo
            {
                Name = item.Title,
                Id = item.PublicId

            };
        }
        #endregion

    }
}
