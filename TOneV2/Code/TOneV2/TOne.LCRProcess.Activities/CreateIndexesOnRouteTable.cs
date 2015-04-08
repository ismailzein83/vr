using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    public class CreateIndexesOnRouteTableInput
    {
        public int RoutingDatabaseId { get; set; }
    }

    public sealed class CreateIndexesOnRouteTable : BaseAsyncActivity<CreateIndexesOnRouteTableInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }  

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.

        protected override void DoWork(CreateIndexesOnRouteTableInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            IRouteDetailDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteDetailDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            dataManager.CreateIndexesOnTable();
            Console.WriteLine("{0}: CreateIndexesOnRouteTable is done in {1}", DateTime.Now, (DateTime.Now - start));
        }

        protected override CreateIndexesOnRouteTableInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CreateIndexesOnRouteTableInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
