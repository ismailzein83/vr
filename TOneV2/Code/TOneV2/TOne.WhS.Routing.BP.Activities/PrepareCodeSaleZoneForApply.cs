using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCodeSaleZoneForApplyInput
    {
        public int RoutingDatabaseId { get; set; }
        public BaseQueue<CodeMatchesBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareCodeSaleZoneForApply : DependentAsyncActivity<PrepareCodeSaleZoneForApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareCodeSaleZoneForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeSaleZoneDataManager codeSaleZoneDataManager = RoutingDataManagerFactory.GetDataManager<ICodeSaleZoneDataManager>();
            codeSaleZoneDataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            PrepareDataForDBApply(previousActivityStatus, handle, codeSaleZoneDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, GetCodeSaleZoneList);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Code Sale Zone For Apply is done", null);
        }

        protected override PrepareCodeSaleZoneForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeSaleZoneForApplyInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());

            base.OnBeforeExecute(context, handle);
        }

        private IEnumerable<CodeSaleZone> GetCodeSaleZoneList(CodeMatchesBatch codeMatchesBatch)
        {
            List<CodeSaleZone> codeSaleZone = new List<CodeSaleZone>();

            foreach (CodeMatches codeMatch in codeMatchesBatch.CodeMatches)
            {
                foreach (var saleCodeMatch in codeMatch.SaleCodeMatches)
                {
                    codeSaleZone.Add(new CodeSaleZone() { Code = codeMatch.Code, SaleZoneId = saleCodeMatch.SaleZoneId });
                }
            }

            return codeSaleZone;
        }
    }
}