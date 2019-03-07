using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Data
{
   public interface IDraftReportTransactionDataManager : IDataManager
    {
        Dictionary<long, List<ERPDraftReportTransaction>> GetTransactionsReportsData(List<long> reportsIds);
        void Insert(List<JazzTransactionsReportData> transactionReportData,long reportId);
        void Delete(long processInstanceId);

    }
}
