using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;


namespace Demo.Module.Data
{
    public interface IGenericAnalyticDataManager : IDataManager
    {
        AnalyticSummaryBigResult<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input);
    }
}
