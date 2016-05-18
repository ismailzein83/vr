using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IAnalyticDataManager : IDataManager
    {
        IEnumerable<DBAnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, out  HashSet<string> includedSQLDimensions);
        AnalyticRecord GetAnalyticSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);
        IEnumerable<TimeVariationAnalyticRecord> GetTimeVariationAnalyticRecords(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input);
        
        IAnalyticTableQueryContext AnalyticTableQueryContext { set; }
    }
}
