using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyChangedCountriesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyChangedCountriesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyChangedCountriesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangedCountriesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            IChangedCustomerCountryDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedCustomerCountryDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCountryMatch) =>
                    {
                        dataManager.ApplyChangedCustomerCountriesToDB(preparedCountryMatch);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedCountriesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedCountriesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
