using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PrepareChangedCustomerCountriesForApplyInput
    {
        public BaseQueue<IEnumerable<ChangedCustomerCountry>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareChangedCustomerCountriesForApply : DependentAsyncActivity<PrepareChangedCustomerCountriesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ChangedCustomerCountry>>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareChangedCustomerCountriesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedCustomerCountryDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedCustomerCountryDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ChangedCustomerCountriesList => ChangedCustomerCountriesList);
        }

        protected override PrepareChangedCustomerCountriesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareChangedCustomerCountriesForApplyInput()
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
    }
}
