using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IGenericAnalyticDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<AnalyticRecord> GetAnalyticSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);
    }
}
