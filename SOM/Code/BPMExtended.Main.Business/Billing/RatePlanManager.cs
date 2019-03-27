﻿using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class RatePlanManager
    {
        CPTManager _cptManager = new CPTManager();

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public List<CustomerCategoryInfo> GetCustomerCategoryInfo(BPMCustomerType customerType)
        {

            var customerCategories = RatePlanMockDataGenerator.GetCustomerCategories(customerType);
            return customerCategories.MapRecords(CustomerCategroryInfoMapper).ToList();
            
        }
        
        public List<CustomerCategoryDetail> GetCustomerCategories(BPMCustomerType customerType)
        {
            return RatePlanMockDataGenerator.GetCustomerCategories(customerType);
        }

        public SOM.Main.Entities.CustomerCategory GetCustomerCategoryById(string customerCategoryId)
        {
            return RatePlanMockDataGenerator.GetCustomerCategory(customerCategoryId);
        }

        public List<RatePlanInfo> GetRatePlanInfo(BPMCustomerType customerType, string customerCategoryId, OperationType operationType, string subType , string filter = null)
        {
            List<string> ratePlanIds = null;

            var lobAttr = Utilities.GetEnumAttribute<OperationType, SOM.Main.Entities.LineOfBusinessAttribute>(operationType);
            List<SOM.Main.Entities.RatePlan> ratePlans = RatePlanMockDataGenerator.GetRatePlans(lobAttr.LOB, customerCategoryId, subType);


            Func<SOM.Main.Entities.RatePlan, bool> filterExpression = (item) =>
            {
                if (ratePlanIds.Find(x => x == item.RatePlanId) != null)
                    return false;


                return true;
            };

            if (filter == null)
            {
                return ratePlans.MapRecords(RatePlanInfoMapper).ToList();

            }
            else
            {
                //deserialize filter
                ratePlanIds = JsonConvert.DeserializeObject<Filter>(filter).ExcludedRatePlanIds;

                return ratePlans.MapRecords(RatePlanInfoMapper, filterExpression).ToList();
            }

        }

        public List<RatePlanInfo> GetRatePlansInfo()
        {
            var ratePlanInfoItems = new List<RatePlanInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<BPMExtended.Main.SOMAPI.RatePlan> items = client.Get<List<BPMExtended.Main.SOMAPI.RatePlan>>(String.Format("api/SOM.ST/Billing/GetRatePlans"));
                foreach (var item in items)
                {
                    var ratePlanInfoItem = RatePlanToInfoMapper(item);
                    ratePlanInfoItems.Add(ratePlanInfoItem);
                }
            }
            return ratePlanInfoItems;
        }

        public RatePlanInfo RatePlanToInfoMapper(BPMExtended.Main.SOMAPI.RatePlan item)
        {
            return new RatePlanInfo
            {
                Name = item.Name,
                RatePlanId = item.RatePlanId

            };
        }

        public List<ServiceDetail> ValidateServices(string selectedRatePlanId , string oldRatePlanId)
        {
            var selectedRatePlan = RatePlanMockDataGenerator.GetRatePlan(selectedRatePlanId);
            var services = new List<ServiceDetail>();

            foreach (var pckg in selectedRatePlan.OptionalPackages)
            {
                services.AddRange(pckg.Services.MapRecords(ServiceMapper).ToList());
            }


            var ratePlan2 = RatePlanMockDataGenerator.GetRatePlan(oldRatePlanId);
            var oldServices = new List<ServiceDetail>();

            foreach (var pckg in ratePlan2.OptionalPackages)
            {
                oldServices.AddRange(pckg.Services.MapRecords(ServiceMapper).ToList());
            }

            var listOfServices = oldServices.Except(services).ToList();

            return listOfServices;
        }

        public bool UpdateRatePlan(string contractId, string ratePlanId, string servicesIds)
        {
            if (servicesIds == null)
            {
                //update ratePlan
            }
            else
            {
                //change services

                //update ratePlan
            }
            
            return true;
        }

        public void ChangeServices(string contractId, string ratePlanId, string servicesIds)
        {
            
        }

        public List<ServiceDetail> GetCoreServices(string ratePlanId)
        {
            var ratePlan = RatePlanMockDataGenerator.GetRatePlan(ratePlanId);
            return ratePlan.CorePackage.Services.MapRecords(ServiceMapper).ToList();
        }

        public List<ServiceDetail> GetOptionalServices(string ratePlanId)
        {
            var ratePlan = RatePlanMockDataGenerator.GetRatePlan(ratePlanId);
            var services = new List<ServiceDetail>();

            foreach (var pckg in ratePlan.OptionalPackages)
            {
                services.AddRange(pckg.Services.MapRecords(ServiceMapper).ToList());
            }

            return services;
        }

        public List<ServiceDetail> GetAdditionServices(string ratePlanId)
        {
            //TODO :  Get the addition services

            List<ServiceDetail> allOptionalServices = this.GetOptionalServices(ratePlanId);

            return allOptionalServices;

        }

        public bool CheckServicesConsistency(List<ServiceParameter> services)
        {
           //TODO : validate services 

            return true;
        }

        public List<ServiceDetail> checkIfSwitchSupportedServices(string services, string switchId)
        {
            List<ServiceDetail> servicesList = null;
            List<ServiceDetail> nonSupportedServices =new List<ServiceDetail>();

            //
            servicesList = JsonConvert.DeserializeObject<List<ServiceDetail>>(services);

            //TODO:check if these services are  supported or not by the switch

            return nonSupportedServices;
        }

        public List<ServiceDetail> GetAllCoreServices()
        {
            return RatePlanMockDataGenerator.GetAllServices().FindAllRecords(x => x.IsCore).MapRecords(ServiceMapper).ToList();
        }

        public List<ServiceDetail> GetAllOptionalServices()
        {
            return RatePlanMockDataGenerator.GetAllServices().FindAllRecords(x => x.IsCore != true).MapRecords(ServiceMapper).ToList();
        }

        public List<ServiceDetail> GetOptionalServicesByContractId(string telephonycontractid)
        {
            ContractManager contractManager = new ContractManager();
            string ratePlanId = contractManager.GetTelephonyContract(telephonycontractid).RatePlanId;
            List<ServiceDetail> allOptionalServices = this.GetOptionalServices(ratePlanId);
            if (allOptionalServices.Count > 0)
                return allOptionalServices.Take(2).ToList();

            return new List<ServiceDetail>();
        }

        public List<ServiceDetail> GetLeasedLineOptionalServicesByContractId(string contractid)
        {
            ContractManager contractManager = new ContractManager();
            string ratePlanId = contractManager.GetLeasedLineContract(contractid).RatePlanId;
            List<ServiceDetail> allOptionalServices = this.GetOptionalServices(ratePlanId);
            if (allOptionalServices.Count > 0)
                return allOptionalServices.Take(2).ToList();

            return new List<ServiceDetail>();
        }

        public List<ServiceDetail> GetGSHDSLOptionalServicesByContractId(string contractid)
        {
            ContractManager contractManager = new ContractManager();
            string ratePlanId = contractManager.GetGSHDSLContract(contractid).RatePlanId;
            List<ServiceDetail> allOptionalServices = this.GetOptionalServices(ratePlanId);
            if (allOptionalServices.Count > 0)
                return allOptionalServices.Take(2).ToList();

            return new List<ServiceDetail>();
        }



        public List<ServiceDetail> GetADSLOptionalServicesByContractId(string telephonycontractid)//adsl contractId
        {
            ContractManager contractManager = new ContractManager();
            string ratePlanId = contractManager.GetADSLContract(telephonycontractid).RatePlanId;
            List<ServiceDetail> allOptionalServices = this.GetOptionalServices(ratePlanId);
            if (allOptionalServices.Count > 0)
                return allOptionalServices.Take(2).ToList();

            return new List<ServiceDetail>();
        }

        public bool ActivateCPTService(string requestId , string contractId, string cptNumber)
        {

            //TODO: call BSCS to activate CPT service


            //
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            //var update = new Update(connection, "Contact").Set("StCustomerId", Func.IsNull(Column.SourceColumn("StCustomerId"), Column.Parameter(contactId)));
            var update = new Update(connection, "StCpt").Set("StStepId", Column.Parameter("D9BCF65C-8311-4C85-84ED-972EB09C46BB"))
                .Where("Id").IsEqual(Column.Parameter(requestId));
            update.Execute();

            return true;
        }

        public bool DeactivateCPTService(string contractId)
        {
            this._cptManager.UnRegisterCPTNumber(contractId);

            //TODO: call BSCS to deactivate the service
            return true;
        }

        public List<ServiceInfo> GetServicesWithPasswordResetSupport(string contractId)
        {
            ContractManager contractManager = new ContractManager();
            string ratePlanId = contractManager.GetTelephonyContract(contractId).RatePlanId;

            var services = GetOptionalServices(ratePlanId);
            return services.MapRecords(ServiceInfoMapper).ToList();

        }


        public List<ADSLSpeedInfo> GetAllADSLSpeedInfo()
        {

            return RatePlanMockDataGenerator.GetAllADSLSpeedInfo();

        }

        #region Mappers

        private RatePlanInfo RatePlanInfoMapper(SOM.Main.Entities.RatePlan ratePlan)
        {
            return new RatePlanInfo
            {
                RatePlanId = ratePlan.RatePlanId,
                Name = ratePlan.Name
            };
        }

        private ServiceInfo ServiceInfoMapper(ServiceDetail service)
        {
            return new ServiceInfo
            {
                ServiceId = service.ServiceId,
                Name = service.Name
            };
        }


        private ServiceDetail ServiceMapper(SOM.Main.Entities.Service service)
        {
            return new ServiceDetail
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Description = service.Description,
                IsCore = service.IsCore,
                SubscriptionFee = service.SubscriptionFee,
                AccessFee = service.AccessFee,
                PackageName = RatePlanMockDataGenerator.GetServicePackage(service.PackageId)!=null?RatePlanMockDataGenerator.GetServicePackage(service.PackageId).PackageName : null,
                ServiceParams = service.ServiceParams != null ?service.ServiceParams.MapRecords(ServiceParameterMapper).ToList():null
            };
        }

        private ServiceParameterDetail ServiceParameterMapper(SOM.Main.Entities.ServiceParameter serviceParameter)
        {
            return new ServiceParameterDetail
            {
                Id = serviceParameter.Id,
                Name = serviceParameter.Name
            };
        }

        //TODO: to be removed after real installation
        private CustomerCategoryInfo CustomerCategroryInfoMapper(CustomerCategoryDetail customerCategory)
        {
            return new CustomerCategoryInfo {
                CategoryId = customerCategory.CategoryId,
                Name = customerCategory.Name
            };
        }

        #endregion
    }
}
