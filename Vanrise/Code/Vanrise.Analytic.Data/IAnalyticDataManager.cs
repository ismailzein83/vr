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
        IEnumerable<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);
        AnalyticRecord GetAnalyticSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);
        IEnumerable<TimeVariationAnalyticRecord> GetTimeVariationAnalyticRecords(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input);
        AnalyticTable Table { set; }

        Dictionary<string, AnalyticDimension> Dimensions { set; }

        Dictionary<string, AnalyticAggregate> Aggregates { set; }

        Dictionary<string, AnalyticMeasure> Measures { set; }

        Dictionary<string, AnalyticJoin> Joins { set; }
    }
}
