using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IAnalyticConfigurationDataManager : IDataManager
    {
        IEnumerable<AnalyticConfiguration<MeasureConfiguration>> GetMeasures();
        IEnumerable<AnalyticConfiguration<DimensionConfiguration>> GetDimensions();

        List<AnalyticItemConfig<T>> GetItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class;

        bool AreAnalyticConfigurationUpdated(ref object updateHandle);
    }
}
