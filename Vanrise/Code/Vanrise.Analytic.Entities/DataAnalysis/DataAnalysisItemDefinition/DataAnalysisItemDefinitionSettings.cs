using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class DataAnalysisItemDefinitionSettings
    {
        public string Title { get; set; }

        public abstract Guid ItemDefinitionTypeId { get; }

        public virtual bool HasParameters(IDataAnalysisItemDefinitionSettingsHasParametersContext context) { return false; }
    }

    public enum DataAnalysisParametersType { All = 0, Global = 1, Overridden = 2 }

    public interface IDataAnalysisItemDefinitionSettingsHasParametersContext
    {
        DataAnalysisParametersType ParametersType { get; }
    }

    public class DataAnalysisItemDefinitionSettingsHasParametersContext: IDataAnalysisItemDefinitionSettingsHasParametersContext
    {
        public DataAnalysisParametersType ParametersType { get; set; }
    }
}
