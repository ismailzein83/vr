using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class RatePlanManager
    {
        private List<CustomerCategoryInfo> GetCustomerCategoryInfo(BPMCustomerType customerType)
        {
            var customerCategories = MockDataGenerator.GetCustomerCategories(customerType);
            return customerCategories.MapRecords(CustomerCategroryInfoMapper).ToList();
        }
        
        private List<CustomerCategoryDetail> GetCustomerCategories(BPMCustomerType customerType)
        {
            return MockDataGenerator.GetCustomerCategories(customerType);
        }

        public List<RatePlanInfo> GetRatePlanInfo(BPMCustomerType customerType, string customerCategoryId, OperationType operationType, string subType)
        {
            var lobAttr = Utilities.GetEnumAttribute<OperationType, SOM.Main.Entities.LineOfBusinessAttribute>(operationType);
            var ratePlans = MockDataGenerator.GetRatePlans(lobAttr.LOB, customerCategoryId, subType);

            return ratePlans.MapRecords(RatePlanInfoMapper).ToList();
        }

        public List<ServiceDetail> GetCoreServices(string ratePlanId)
        {
            var ratePlan = MockDataGenerator.GetRatePlan(ratePlanId);
            return ratePlan.CorePackage.Services.MapRecords(ServiceMapper).ToList();
        }

        public List<ServiceDetail> GetOptionalServices(string ratePlanId)
        {
            var ratePlan = MockDataGenerator.GetRatePlan(ratePlanId);
            var services = new List<ServiceDetail>();

            foreach (var pckg in ratePlan.OptionalPackages)
            {
                services.AddRange(pckg.Services.MapRecords(ServiceMapper).ToList());
            }

            return services;
        }
        
        public List<ServiceDetail> GetAllCoreServices()
        {
            return MockDataGenerator.GetAllServices().FindAllRecords(x => x.IsCore).MapRecords(ServiceMapper).ToList();
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
                PackageName = MockDataGenerator.GetServicePackage(service.PackageId).PackageName,
                ServiceParams = service.ServiceParams != null ? service.ServiceParams.MapRecords(ServiceParameterMapper).ToList() : null
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
