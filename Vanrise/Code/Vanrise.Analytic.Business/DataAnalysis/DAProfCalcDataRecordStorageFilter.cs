using System;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities.DataStorage.DataRecordStorage;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcDataRecordStorageFilter : IDataRecordStorageFilter
    {
        public Guid DataAnalysisDefinitionId { get; set; }

        public bool IsMatched(GenericData.Entities.DataRecordStorage dataRecordStorage)
        {
            if (dataRecordStorage == null)
                throw new NullReferenceException("dataRecordStorage");

            DataAnalysisDefinitionManager dataAnalysisDefinitionManager = new DataAnalysisDefinitionManager();
            DataAnalysisDefinition dataAnalysisDefinition = dataAnalysisDefinitionManager.GetDataAnalysisDefinition(DataAnalysisDefinitionId);

            if (dataAnalysisDefinition == null)
                throw new NullReferenceException(string.Format("dataAnalysisDefinition {0}", DataAnalysisDefinitionId));

            if (dataAnalysisDefinition.Settings == null)
                throw new NullReferenceException(string.Format("dataAnalysisDefinition.Settings {0}", DataAnalysisDefinitionId));

            DAProfCalcSettings daProfCalcSettings = dataAnalysisDefinition.Settings as DAProfCalcSettings;
            if (daProfCalcSettings == null)
                throw new Exception(String.Format("dataAnalysisDefinition.Settings is not of type DAProfCalcSettings. It is of type '{0}'", dataAnalysisDefinition.Settings.GetType()));

            if (daProfCalcSettings.DataRecordTypeId != dataRecordStorage.DataRecordTypeId)
                return false;

            return true;
        }
    }
}