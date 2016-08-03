using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface ISummaryBatchActivatorDataManager : IDataManager
    {
        List<SummaryBatchActivator> GetAllSummaryBatchActivators();


        List<SummaryBatchActivator> GetSummaryBatchActivators(Guid activatorId);

        void Delete(int queueId, DateTime batchStart);

        void Insert(List<SummaryBatchActivator> summaryBatchActivators);
    }
}
