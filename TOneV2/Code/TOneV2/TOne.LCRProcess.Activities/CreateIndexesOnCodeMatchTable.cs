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

    public class CreateIndexesOnCodeMatchTableInput
    {
         public int RoutingDatabaseId { get; set; }
    }

    #endregion

    public sealed class CreateIndexesOnCodeMatchTable : BaseAsyncActivity<CreateIndexesOnCodeMatchTableInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }        
       
        protected override CreateIndexesOnCodeMatchTableInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CreateIndexesOnCodeMatchTableInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }

        protected override void DoWork(CreateIndexesOnCodeMatchTableInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            dataManager.CreateIndexesOnTable();
            Console.WriteLine("{0}: CreateIndexesOnCodeMatchTable is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
