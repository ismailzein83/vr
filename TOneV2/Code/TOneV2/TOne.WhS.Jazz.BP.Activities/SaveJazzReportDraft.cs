using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Jazz.Data;

namespace TOne.WhS.Jazz.BP.Activities
{

    public sealed class SaveJazzReportDraft : CodeActivity
    {
        public InArgument<List<JazzTransactionsReport>> TransactionsReport { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            var transactionsReport = TransactionsReport.Get(context);
            if(transactionsReport!=null && transactionsReport.Count > 0)
            {
                IDraftReportDataManager draftReportDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportDataManager>();
                IDraftReportTransactionDataManager draftReportTransactionDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportTransactionDataManager>();

                foreach(var transactionReport in transactionsReport)
                {
                    long transactionReportId = 0; ;
                    draftReportDataManager.Insert(transactionReport, processInstanceId, out transactionReportId);
                    if(transactionReportId > 0)
                    {
                        if(transactionReport.ReportData!=null && transactionReport.ReportData.Count > 0)
                        {
                            
                                draftReportTransactionDataManager.Insert(transactionReport.ReportData, transactionReportId);
                            
                        }

                    }
                }
            }
        }
    }
}
