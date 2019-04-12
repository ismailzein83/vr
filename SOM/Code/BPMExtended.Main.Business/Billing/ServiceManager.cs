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

        #region Public

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
        public List<ServiceDetail> GetCoreServices(string ratePlanId)
        {
            //var ratePlan = RatePlanMockDataGenerator.GetRatePlan(ratePlanId);
            //return ratePlan.CorePackage.Services.MapRecords(ServiceMapper).ToList();
            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            var corePackageName = packages.Core;

           var coreServices = GetServicesDetailByRateplanAndPackage(ratePlanId, corePackageName);
            return coreServices;
        }
        public List<ServiceDetail> GetServicesDetailByRateplanAndPackage(string rateplanId, string package)
        {

            var serviceInput = new ServiceInput()
            {
                RatePlanId = rateplanId,
                Package = package
            };
            var servicesDetailItems = new List<ServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Post<ServiceInput, List<ServiceDefinition>>("api/SOM.ST/Billing/GetServicesByRateplanAndPackage", serviceInput);
               // List<ServiceDefinition> items = client.Get<List<ServiceDefinition>>(String.Format("api/SOM.ST/Billing/GetServicesByRateplanAndPackage?ratePlanId=TM006&packageId=SP005"));
                foreach (var item in items)
                {
                    var serviceDetailItem = ServiceDefinitionToDetailMapper(item);
                    servicesDetailItems.Add(serviceDetailItem);
                }
            }
            return servicesDetailItems;
        }

        public List<ServiceDetail> GetServicesDetailByRateplanAndPackages(string rateplanId, List<string> packages)
        {

            var multiplePackagesServiceInput = new MultiplePackagesServiceInput()
            {
                RatePlanId = rateplanId,
                Packages = packages
            };

            var servicesDetailItems = new List<ServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Post<MultiplePackagesServiceInput,List<ServiceDefinition>>("api/SOM.ST/Billing/GetServicesByRateplanAndPackages",multiplePackagesServiceInput);
                foreach (var item in items)
                {
                    var serviceDetailItem = ServiceDefinitionToDetailMapper(item);
                    servicesDetailItems.Add(serviceDetailItem);
                }
            }
            return servicesDetailItems;
        }
        public List<ServiceDetail> GetRatePlanServicesDetail(string rateplanId, List<string> excludedPackages)
        {
            var excludePackagesServiceInput = new ExcludePackagesServiceInput()
            {
                RatePlanId = rateplanId,
                ExcludePackages = excludedPackages
            };

            var servicesDetailItems = new List<ServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Post< ExcludePackagesServiceInput,List<ServiceDefinition>>("api/SOM.ST/Billing/GetRatePlanServices", excludePackagesServiceInput);
                foreach (var item in items)
                {
                    var serviceDetailItem = ServiceDefinitionToDetailMapper(item);
                    servicesDetailItems.Add(serviceDetailItem);
                }
            }
            return servicesDetailItems;
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
            services = serviceDetailItems.Where(p => !Servicesids.Any(p2 => p2.ToString() == p.ServiceId.ToString())).ToList();
            return services;
        }



        #endregion



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
