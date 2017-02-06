using System;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcDataAnalysisItemDefinitionFilter : IDataAnalysisItemDefinitionFilter
    {
        public bool IsMatch(DataAnalysisItemDefinition dataAnalysisItemDefinition)
        {
            if (dataAnalysisItemDefinition == null)
                throw new NullReferenceException("dataAnalysisItemDefinition");

            if (dataAnalysisItemDefinition.Settings == null)
                throw new NullReferenceException("dataAnalysisItemDefinition.Settings");

            if (dataAnalysisItemDefinition.Settings as DAProfCalcOutputSettings == null)
                return false;

            return true;
        }
    }
}
