using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

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

        public List<POSServiceDetail> GetPOSServiceDetail(string catalogId, string catalogName, string sourceSchemaName)
        {
            List<string> POSServicesids = new List<string>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, sourceSchemaName);
            var POSId = esq.AddColumn("StPOSServiceID");
            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, catalogName, catalogId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    POSServicesids.Add(item.GetTypedColumnValue<string>(POSId.Name));
                }
            }


            var POSServiceDetailItems = new List<POSServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Get<List<ServiceDefinition>>(String.Format("api/SOM.ST/Billing/GetPOSServices"));
                foreach (var item in items)
                {
                    var POSServiceDetailItem = POSServiceDefinitionToDetailMapper(item);
                    POSServiceDetailItems.Add(POSServiceDetailItem);
                }
            }

            var posServices = new List<POSServiceDetail>();
            posServices = POSServiceDetailItems.Where(p => !POSServicesids.Any(p2 => p2.ToString() == p.Id.ToString())).ToList();
            return posServices;
        }

        public List<ServiceDetail> GetServiceDetail(string catalogId, string catalogName, string sourceSchemaName)
        {
            List<string> Servicesids = new List<string>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, sourceSchemaName);
            var Id = esq.AddColumn("StServiceID");
            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, catalogName, catalogId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    Servicesids.Add(item.GetTypedColumnValue<string>(Id.Name));
                }
            }


            var serviceDetailItems = new List<ServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Get<List<ServiceDefinition>>(String.Format("api/SOM.ST/Billing/GetServicesDefinition"));
                foreach (var item in items)
                {
                    var serviceDetailItem = ServiceDefinitionToDetailMapper(item);
                    serviceDetailItems.Add(serviceDetailItem);
                }
            }

            var services = new List<ServiceDetail>();
            services = serviceDetailItems.Where(p => !serviceDetailItems.Any(p2 => p2.ToString() == p.ServiceId.ToString())).ToList();
            return services;
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
        public ServiceDetail ServiceDefinitionToDetailMapper(ServiceDefinition item)
        {
            return new ServiceDetail
            {
                Name = item.Title,
                ServiceId = item.PublicId

            };
        }

        public POSServiceDetail POSServiceDefinitionToDetailMapper(ServiceDefinition item)
        {
            return new POSServiceDetail
            {
                Name = item.Title,
                Id = item.PublicId

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
