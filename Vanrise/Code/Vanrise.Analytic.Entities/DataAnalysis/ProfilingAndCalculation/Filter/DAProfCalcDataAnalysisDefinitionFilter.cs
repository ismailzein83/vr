using System;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcDataAnalysisDefinitionFilter : IDataAnalysisDefinitionFilter
    {
        Guid _configId { get { return new Guid("B3AF681B-72CE-4DD8-9090-CC727690F7E0"); } }
        public bool IsMatch(DataAnalysisDefinition dataAnalysisDefinition)
        {
            if (dataAnalysisDefinition == null)
                throw new NullReferenceException("dataAnalysisDefinition");

            if(dataAnalysisDefinition.Settings==null)
                throw new NullReferenceException("dataAnalysisDefinition.Settings");

            if (dataAnalysisDefinition.Settings.ConfigId != _configId)
                return false;

            return true;
        }
    }
}
