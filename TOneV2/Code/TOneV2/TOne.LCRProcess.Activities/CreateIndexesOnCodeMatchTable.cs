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
        public bool IsFuture { get; set; }
    }

    #endregion

    public sealed class CreateIndexesOnCodeMatchTable : BaseAsyncActivity<CreateIndexesOnCodeMatchTableInput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }
        
       
        protected override CreateIndexesOnCodeMatchTableInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CreateIndexesOnCodeMatchTableInput
            {
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void DoWork(CreateIndexesOnCodeMatchTableInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();            
            dataManager.CreateIndexesOnTable(inputArgument.IsFuture);
            Console.WriteLine("{0}: CreateIndexesOnCodeMatchTable is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
