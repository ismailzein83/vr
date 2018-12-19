using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities
{
    #region Argument Classes

    public class ExecuteGenericBEActionsInput
    {
        public BaseQueue<GenericBusinessEntity> InputQueue { get; set; }
        public List<GenericBEBulkActionRuntime> GenericBEBulkActions { get; set; }
        public Guid BEDefinitionId { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }

    }
    #endregion
    public class ExecuteGenericBEActions : DependentAsyncActivity<ExecuteGenericBEActionsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<GenericBusinessEntity>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<GenericBEBulkActionRuntime>> GenericBEBulkActions { get; set; }

        [RequiredArgument]
        public InArgument<Guid> BEDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<HandlingErrorOption> HandlingErrorOption { get; set; }
        #endregion

        protected override void DoWork(ExecuteGenericBEActionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
          
        }
        protected override ExecuteGenericBEActionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteGenericBEActionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                GenericBEBulkActions = this.GenericBEBulkActions.Get(context),
                BEDefinitionId = this.BEDefinitionId.Get(context),
                HandlingErrorOption = this.HandlingErrorOption.Get(context),
            };
        }
    }
}
