using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{

    #region Argument Classes
    public class StoreStatisticsToDBInput
    {
        public int SwitchId { get; set; }
    }

    #endregion

    public sealed class StoreStatisticsToDB : DependentAsyncActivity<StoreStatisticsToDBInput>
    {

        [RequiredArgument]
        public InArgument<int> SwitchId { get; set; }

        protected override void DoWork(StoreStatisticsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override StoreStatisticsToDBInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new StoreStatisticsToDBInput
            {
                SwitchId = this.SwitchId.Get(context),
            };
        }
    }
}
