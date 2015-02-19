using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class UpdateZoneMatchInDBInput
    {
        public int RoutingDatabaseId { get; set; }
    }

    #endregion

    public sealed class UpdateZoneMatchInDB : BaseAsyncActivity<UpdateZoneMatchInDBInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override UpdateZoneMatchInDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new UpdateZoneMatchInDBInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
        protected override void DoWork(UpdateZoneMatchInDBInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            IZoneMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneMatchDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            dataManager.UpdateAll();
            Console.WriteLine("{0}: UpdateZoneMatchInDB is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
