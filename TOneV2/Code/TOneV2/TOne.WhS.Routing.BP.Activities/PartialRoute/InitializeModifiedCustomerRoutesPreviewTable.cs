using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public class InitializeModifiedCustomerRoutesPreviewTableInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
    }

    public sealed class InitializeModifiedCustomerRoutesPreviewTable : BaseAsyncActivity<InitializeModifiedCustomerRoutesPreviewTableInput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        protected override void DoWork(InitializeModifiedCustomerRoutesPreviewTableInput inputArgument, AsyncActivityHandle handle)
        {
            var dataManager = RoutingDataManagerFactory.GetDataManager<IModifiedCustomerRoutePreviewDataManager>();
            dataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            dataManager.InitializeTable();
        }

        protected override InitializeModifiedCustomerRoutesPreviewTableInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new InitializeModifiedCustomerRoutesPreviewTableInput()
            {
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }
    }
}