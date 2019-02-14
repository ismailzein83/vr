using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Data;
using TOne.WhS.Jazz.Data.RDB;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Jazz.BP.Activities
{
    public sealed class DeleteJazzReportFile : CodeActivity
    {
        public InArgument<long> FileId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            IDraftReportDataManager draftReportDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportDataManager>();
            IDraftReportTransactionDataManager draftReportTransactionDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportTransactionDataManager>();

            draftReportTransactionDataManager.Delete(processInstanceId);
            draftReportDataManager.Delete(processInstanceId);

            VRFileManager fileManager = new VRFileManager();
            fileManager.DeleteFile(FileId.Get(context));
        }
    }
}
