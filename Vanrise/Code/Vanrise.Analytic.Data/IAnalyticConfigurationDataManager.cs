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
        List<AnalyticConfiguration<MeasureConfiguration>> GetMeasures();
        List<AnalyticConfiguration<DimensionConfiguration>> GetDimensions();
    }
}
