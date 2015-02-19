using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class CreateIndexesOnZoneMatchTableInput
    {
        public int RoutingDatabaseId { get; set; }
    }

    #endregion

    public sealed class CreateIndexesOnZoneMatchTable : BaseAsyncActivity<CreateIndexesOnZoneMatchTableInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
       
        protected override CreateIndexesOnZoneMatchTableInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CreateIndexesOnZoneMatchTableInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }

        protected override void DoWork(CreateIndexesOnZoneMatchTableInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            IZoneMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneMatchDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            dataManager.CreateIndexesOnTable();
            Console.WriteLine("{0}: CreateIndexesOnZoneMatchTable is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
