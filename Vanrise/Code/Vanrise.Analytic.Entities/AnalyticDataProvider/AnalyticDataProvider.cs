using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticDataProvider
    {
        public abstract Guid ConfigId { get; }

        public abstract IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext);

        public virtual IRemoteAnalyticDataProvider GetRemoteAnalyticDataProvider(IGetRemoteAnalyticDataProviderContext context)
        {
            return null;
        }
    }

    public interface IAnalyticDataManager
    {
        IEnumerable<DBAnalyticRecord> GetAnalyticRecords(AnalyticQuery query, out  HashSet<string> includedSQLDimensions);
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
