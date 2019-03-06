using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Data
{
   public interface IDraftReportDataManager : IDataManager
    {
        List<ERPDraftReport> GetTransactionsReports(long processInstanceId);
        void Insert(JazzTransactionsReport transactionReport,long processInstanceId, out long insertedId);
        void Delete(long processInstanceId);
    }
}
