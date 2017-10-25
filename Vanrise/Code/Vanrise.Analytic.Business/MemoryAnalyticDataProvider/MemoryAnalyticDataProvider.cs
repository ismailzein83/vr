using System;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class MemoryAnalyticDataProvider : AnalyticDataProvider
    {
        public override Guid ConfigId { get { return new Guid("4F72F80C-3928-460F-AAF5-CC18A78C8265"); } }

        public MemoryAnalyticDataManager DataManager { get; set; }

        public override IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext)
        {
            this.DataManager.ThrowIfNull("DataManager");
            var dataManagerCopy = Serializer.Deserialize(Serializer.Serialize(this.DataManager)) as MemoryAnalyticDataManager;
            dataManagerCopy.QueryContext = queryContext;
            return dataManagerCopy;
        }
    }
}
