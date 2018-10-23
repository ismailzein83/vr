using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class GetCustomerRoutingQualityDataInput
    {
        public int RoutingDatabaseId { get; set; }
    }

    public class GetCustomerRoutingQualityDataOutput
    {
        public CustomerRouteQualityDataBySupplierZone CustomerRouteQualityDataBySupplierZone { get; set; }
    }

    public sealed class GetCustomerRoutingQualityData : BaseAsyncActivity<GetCustomerRoutingQualityDataInput, GetCustomerRoutingQualityDataOutput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public OutArgument<CustomerRouteQualityDataBySupplierZone> CustomerRouteQualityDataBySupplierZone { get; set; }

        protected override GetCustomerRoutingQualityDataOutput DoWorkWithResult(GetCustomerRoutingQualityDataInput inputArgument, AsyncActivityHandle handle)
        {
            CustomerRouteQualityDataBySupplierZone qualityConfigurationBySupplierZone = null;
            ICustomerQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
            dataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            var qualityConfigurationData = dataManager.GetCustomerRouteQualityConfigurationsData();

            if (qualityConfigurationData != null)
            {
                qualityConfigurationBySupplierZone = new CustomerRouteQualityDataBySupplierZone();

                foreach (var qualityConfiguration in qualityConfigurationData)
                {
                    var customerRouteQualityConfigurationDataByQualityConfiguration = qualityConfigurationBySupplierZone.GetOrCreateItem(qualityConfiguration.SupplierZoneId);
                    customerRouteQualityConfigurationDataByQualityConfiguration.Add(qualityConfiguration.QualityConfigurationId, qualityConfiguration);
                }
            }

            return new GetCustomerRoutingQualityDataOutput()
            {
                CustomerRouteQualityDataBySupplierZone = qualityConfigurationBySupplierZone
            };
        }

        protected override GetCustomerRoutingQualityDataInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCustomerRoutingQualityDataInput()
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCustomerRoutingQualityDataOutput result)
        {
            this.CustomerRouteQualityDataBySupplierZone.Set(context, result.CustomerRouteQualityDataBySupplierZone);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Customer Routing Quality Data is done", null);
        }
    }
}
