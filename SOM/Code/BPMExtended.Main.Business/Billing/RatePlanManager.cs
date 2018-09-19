using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;

namespace BPMExtended.Main.Business
{
    public class RatePlanManager
    {
        CPTManager _cptManager = new CPTManager();

        public List<CustomerCategoryInfo> GetCustomerCategoryInfo(BPMCustomerType customerType)
        {
            var customerCategories = RatePlanMockDataGenerator.GetCustomerCategories(customerType);
            return customerCategories.MapRecords(CustomerCategroryInfoMapper).ToList();
        }
        
        public List<CustomerCategoryDetail> GetCustomerCategories(BPMCustomerType customerType)
        {
            return RatePlanMockDataGenerator.GetCustomerCategories(customerType);
        }

        public List<RatePlanInfo> GetRatePlanInfo(BPMCustomerType customerType, string customerCategoryId, OperationType operationType, string subType)
        {
            var lobAttr = Utilities.GetEnumAttribute<OperationType, SOM.Main.Entities.LineOfBusinessAttribute>(operationType);
            var ratePlans = RatePlanMockDataGenerator.GetRatePlans(lobAttr.LOB, customerCategoryId, subType);

            return ratePlans.MapRecords(RatePlanInfoMapper).ToList();
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
        
        public List<ServiceDetail> GetAllCoreServices()
        {
            return RatePlanMockDataGenerator.GetAllServices().FindAllRecords(x => x.IsCore).MapRecords(ServiceMapper).ToList();
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

        public List<ServiceDetail> GetADSLOptionalServicesByContractId(string telephonycontractid)
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
