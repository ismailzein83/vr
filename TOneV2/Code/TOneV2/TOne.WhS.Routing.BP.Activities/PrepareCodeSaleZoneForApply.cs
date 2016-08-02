using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class PrepareCodeSaleZoneForApplyInput
    {
        public BaseQueue<CodeMatchesBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareCodeSaleZoneForApply : DependentAsyncActivity<PrepareCodeSaleZoneForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue { get; set; }


        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareCodeSaleZoneForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeSaleZoneDataManager CodeSaleZoneDataManager = RoutingDataManagerFactory.GetDataManager<ICodeSaleZoneDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, CodeSaleZoneDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, GetCodeSaleZoneList);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Code Sale Zone For Apply is done", null);
        }

        protected override PrepareCodeSaleZoneForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeSaleZoneForApplyInput
            {
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
