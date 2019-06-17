using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
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


        public RatePlanChangeResponse ReadProductChangeServices(string contractId , string newRatePlanId)
        {

            var item = new RatePlanChangeResponse();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<RatePlanChangeResponse>(String.Format("api/SOM.ST/Billing/ReadProductChangeServices?ContractId={0}&NewRatePlanId={1}", contractId, newRatePlanId));

            }
            return item;

        }

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
        public List<ServiceHistoryRecordDetail> GetServiceHistory(string contractId, string serviceId)
        {
            var serviceHistoryItems = new List<ServiceHistoryRecordDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceHistoryRecord> items = client.Get<List<ServiceHistoryRecord>>(String.Format("api/SOM.ST/Billing/GetContractServiceHistory?ContractId={0}&ServiceId={1}", contractId, serviceId));
                foreach (var item in items)
                {
                    var serviceHistoryItem = ServiceHistoryToDetailMapper(item);
                    serviceHistoryItems.Add(serviceHistoryItem);
                }
            }
            return serviceHistoryItems;
        }

        public List<CustomerContractServiceDetail> GetContractServicesDetail(string contractId)
        {
            var customerContractServiceList = new List<CustomerContractServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                var items = client.Get<List<CustomerContractService>>(String.Format("api/SOM.ST/Billing/GetContractServices?ContractId={0}", contractId));
                foreach (var item in items)
                {
                    var customerContractServiceDetailItem = CustomerContractServiceToDetailMapper(item);
                    customerContractServiceList.Add(customerContractServiceDetailItem);
                }
            }
            return customerContractServiceList;
        }
        public List<ServiceDetail> GetCoreServices(string ratePlanId)
        {
            //var ratePlan = RatePlanMockDataGenerator.GetRatePlan(ratePlanId);
            //return ratePlan.CorePackage.Services.MapRecords(ServiceMapper).ToList();
            List<string> packagesIds = new List<string>();

            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            packagesIds.Add(packages.Core);
            packagesIds.Add(packages.Telephony);

            //var corePackageName = packages.Core;

           var coreServices = GetServicesDetailByRateplanAndPackage(ratePlanId, packagesIds,true);
            return coreServices;
        }

        public List<ServiceDetail> GetOptionalServices(string ratePlanId , string switchId, string contactId)
        {
            //var ratePlan = RatePlanMockDataGenerator.GetRatePlan(ratePlanId);
            //return ratePlan.CorePackage.Services.MapRecords(ServiceMapper).ToList();
            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();

            var excludedPackages = new List<string>();
            excludedPackages.Add(packages.Core);
            excludedPackages.Add(packages.Telephony);
            CRMCustomerManager manager = new CRMCustomerManager();
            bool isForeigner = false;
            if(contactId!=null && contactId != "")
                isForeigner = manager.IsContactForeigner(new Guid(contactId));
            var services = GetServicesDetailByRateplanAndPackages(ratePlanId, switchId, excludedPackages,isForeigner);
            return services;
        }
        public List<ServiceDetail> GetServicesDetailByRateplanAndPackage(string rateplanId, List<string> packagesIds, bool onlyCoreServices)
        {

            var serviceInput = new ServiceInput()
            {
                RatePlanId = rateplanId,
                Packages = packagesIds,
                OnlyCoreServices = onlyCoreServices
            };
            var servicesDetailItems = new List<ServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Post<ServiceInput, List<ServiceDefinition>>("api/SOM.ST/Billing/GetServicesByRateplanAndPackages", serviceInput);
               // List<ServiceDefinition> items = client.Get<List<ServiceDefinition>>(String.Format("api/SOM.ST/Billing/GetServicesByRateplanAndPackage?ratePlanId=TM006&packageId=SP005"));
                foreach (var item in items)
                {
                    var serviceDetailItem = ServiceDefinitionToDetailMapper(item);
                    servicesDetailItems.Add(serviceDetailItem);
                }
            }
            return servicesDetailItems;
        }

        public List<ServiceDetail> GetServicesDetailByRateplanAndPackages(string rateplanId,string switchId, List<string> packages,bool isForeigner)
        {

            var multiplePackagesServiceInput = new MultiplePackagesServiceInput()
            {
                RatePlanId = rateplanId,
                SwitchId = switchId,
                ExcludedPackages = packages
            };

            var servicesDetailItems = new List<ServiceDetail>();
            var filteredServicesDetailItems = new List<ServiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<ServiceDefinition> items = client.Post<MultiplePackagesServiceInput, List<ServiceDefinition>>("api/SOM.ST/Billing/GetRatePlanServices", multiplePackagesServiceInput);
                foreach (var item in items)
                {
                    var serviceDetailItem = ServiceDefinitionToDetailMapper(item);
                    servicesDetailItems.Add(serviceDetailItem);
                }

                //Get special services from service definition catalog 
                List<string> specialServicesIds = new CatalogManager().GetSpecialServicesIds();

                //filter the optional services (servicesDetailItems - special services)
                filteredServicesDetailItems = (from item in servicesDetailItems
                                               where !specialServicesIds.Contains(item.Id)
                                                select item).ToList();
                if (isForeigner)
                {
                    List<string> serviceIds = new List<string>();
                    EntitySchemaQuery esq;
                    EntityCollection entities;
                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceInGeneralSettings");
                    esq.AddColumn("Id");
                    var Idcol = esq.AddColumn("StServiceID");

                    entities = esq.GetEntityCollection(BPM_UserConnection);
                    foreach (Entity entity in entities)
                    {
                        serviceIds.Add(entity.GetTypedColumnValue<string>(Idcol.Name));
                    }

                    filteredServicesDetailItems = (from item in filteredServicesDetailItems
                                                   where !serviceIds.Contains(item.Id)
                                                   select item).ToList();
                }
            }
            return filteredServicesDetailItems;
        }
        public List<ServiceDetail> AddForeignerServicesToSelectedServices(string optionalServices)
        {
            List<ServiceDetail> listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);
            if (listOfOptionalServices == null) listOfOptionalServices = new List<ServiceDetail>();

            List<string> serviceIds = new List<string>();
            EntitySchemaQuery esq;
            EntityCollection entities;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceInGeneralSettings");
            esq.AddColumn("Id");
            var Idcol = esq.AddColumn("StServiceID");

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                ServiceDetail det = new ServiceDetail() { Id = entity.GetTypedColumnValue<string>(Idcol.Name), PackageId=""};
                listOfOptionalServices.Add(det);
            }

            return listOfOptionalServices;
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
            services = serviceDetailItems.Where(p => !Servicesids.Any(p2 => p2.ToString() == p.Id.ToString())).ToList();
            return services;
        }



        #endregion



        #region Mappers
        public ServiceInfo ServiceDefinitionToInfoMapper(ServiceDefinition item)
        {
            return new ServiceInfo
            {
                Name = item.Name,
                ServiceId = item.Id

            };
        }
        public ServiceDetail ServiceDefinitionToDetailMapper(ServiceDefinition item)
        {
            return new ServiceDetail
            {
                Id = item.Id,
                PackageId = item.PackageId,
                Name = item.Name,
                NeedsProvisioning = item.NeedsProvisioning,
                IsNetwork =item.IsNetwork,
                IsServiceResource=item.IsServiceResource
            };
        }

        public POSServiceDetail POSServiceDefinitionToDetailMapper(ServiceDefinition item)
        {
            return new POSServiceDetail
            {
                Name = item.Name,
                Id = item.Id

            };
        }

        public POSServiceInfo POSServiceDefinitionToInfoMapper(ServiceDefinition item)
        {
            return new POSServiceInfo
            {
                Name = item.Name,
                Id = item.Id

            };
        }

        public CustomerContractServiceDetail CustomerContractServiceToDetailMapper(CustomerContractService customerContractService)
        {
            var contractManager = new ContractManager();
            return new CustomerContractServiceDetail
            {
                Id = customerContractService.Id,
                Name = customerContractService.Name,
                //Status = contractManager.GetContractStatusByEnumValue(customerContractService.Status.ToString()),
                Status = customerContractService.Status,
                ActivateDate = customerContractService.ActivateDate,
            };
        }

        public ServiceHistoryRecordDetail ServiceHistoryToDetailMapper(ServiceHistoryRecord item)
        {
            var contractManager = new ContractManager();
            return new ServiceHistoryRecordDetail()
            {
                ServiceId = item.ServiceId,
                Status = contractManager.GetContractStatusByEnumValue(item.Status.ToString()),
                Name = item.Name,
                EntryDate = item.EntryDate,
                ValidFromDate = item.ValidFromDate
            };
        }
        #endregion

    }
}
