using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetRPCodeMatchesInput
    {
        public int RoutingDatabaseId { get; set; }
        public IEnumerable<long> SaleZoneIds { get; set; }
        public BaseQueue<RPCodeMatchesByZone> OutputQueue { get; set; }

    }

    public sealed class GetRPCodeMatches : BaseAsyncActivity<GetRPCodeMatchesInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<long>> SaleZoneIds { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<RPCodeMatchesByZone>> OutputQueue { get; set; }

        protected override void DoWork(GetRPCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            Dictionary<long, RPCodeMatchesByZone> codeMatchesByZone = new Dictionary<long, RPCodeMatchesByZone>();
            var codeMatches = dataManager.GetCodeMatches(inputArgument.SaleZoneIds);
            long currentZoneId = 0;
            foreach (var codeMatch in codeMatches.OrderBy(c => c.SaleZoneId))
            {
                RPCodeMatchesByZone codeMatchByZone = null;
                if (currentZoneId != codeMatch.SaleZoneId && codeMatchesByZone.ContainsKey(currentZoneId))
                {
                    inputArgument.OutputQueue.Enqueue(codeMatchByZone);
                }
                if (!codeMatchesByZone.TryGetValue(codeMatch.SaleZoneId, out codeMatchByZone))
                {
                    currentZoneId = codeMatch.SaleZoneId;
                    codeMatchByZone = new RPCodeMatchesByZone()
                    {
                        SaleZoneId = codeMatch.SaleZoneId,
                        SupplierCodeMatches = new List<SupplierCodeMatchWithRate>()
                    };
                    codeMatchesByZone.Add(codeMatch.SaleZoneId, codeMatchByZone);
                }
                codeMatchByZone.SupplierCodeMatches.AddRange(codeMatch.SupplierCodeMatches);
            }
        }

        protected override GetRPCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetRPCodeMatchesInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                SaleZoneIds = this.SaleZoneIds.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue == null)
                this.OutputQueue.Set(context, new MemoryQueue<RPCodeMatchesByZone>());
        }

    }
}
