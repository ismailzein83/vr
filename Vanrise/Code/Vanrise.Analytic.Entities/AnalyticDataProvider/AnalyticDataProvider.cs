using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticDataProvider
    {
        public abstract Guid ConfigId { get; }
        public virtual Guid? ConnectionId { get; }
        public abstract IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext);

        public virtual IRemoteAnalyticDataProvider GetRemoteAnalyticDataProvider(IGetRemoteAnalyticDataProviderContext context)
        {
            return null;
        }
    }

    public interface IAnalyticDataManager
    {
        IEnumerable<DBAnalyticRecord> GetAnalyticRecords(IAnalyticDataManagerGetAnalyticRecordsContext context);
    }

    public interface IAnalyticDataManagerGetAnalyticRecordsContext
    {
        AnalyticQuery Query { get; }

        HashSet<string> AllQueryDBDimensions { get; }

        HashSet<string> AllQueryDBAggregates { get; }

        List<DimensionFilter> DBFilters { get; }

        RecordFilterGroup DBFilterGroup { get; }
    }

    public interface IGetRemoteAnalyticDataProviderContext
    {
        Guid AnalyticTableId { get; set; }  
    }

    public interface IRemoteAnalyticDataProvider
    {
        Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input);
    }
}
