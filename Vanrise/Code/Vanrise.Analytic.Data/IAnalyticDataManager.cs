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
        AnalyticSummaryBigResult<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);

        AnalyticSummaryBigResult<AnalyticRecord> GetFilteredAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);

        AnalyticTable Table { set; }

        Dictionary<string, AnalyticDimension> Dimensions { set; }

        Dictionary<string, AnalyticMeasure> Measures { set; }

        Dictionary<string, AnalyticJoin> Joins { set; }
    }
}
