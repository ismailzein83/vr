using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Data;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Jazz.BP.Activities
{
    public class ApplyJazzReportTransactions : CodeActivity
    {
        public InArgument<DateTime> FromDate { get; set; }

        public InArgument<DateTime> ToDate { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            IFinalTransactionDataManager finalTransactionDataManager = JazzDataManagerFactory.GetDataManager<IFinalTransactionDataManager>();

            finalTransactionDataManager.Insert(FromDate.Get(context), ToDate.Get(context),processInstanceId);

        }
    }
}
