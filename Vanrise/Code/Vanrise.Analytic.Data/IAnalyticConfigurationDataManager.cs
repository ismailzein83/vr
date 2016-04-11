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
        bool AreAnalyticConfigurationUpdated(ref object updateHandle);
    }
}
