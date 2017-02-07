using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcRecordTypeExtraFields : DataRecordTypeExtraField
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }

        public Guid DataAnalysisDefinitionId { get; set; }

        public override Guid ConfigId { get { return new Guid("93F44A29-235D-4C3F-900E-6D7FE780CEF3"); } }

        public override List<DataRecordField> GetFields(IDataRecordExtraFieldContext context)
        {
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            DataAnalysisItemDefinition dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(this.DataAnalysisItemDefinitionId);

            if (dataAnalysisItemDefinition == null)
                throw new NullReferenceException(string.Format("dataAnalysisItemDefinition. DataAnalysisItemDefinitionId: {0}", this.DataAnalysisItemDefinitionId));

            if (dataAnalysisItemDefinition.Settings == null)
                throw new NullReferenceException(string.Format("dataAnalysisItemDefinition.Settings. DataAnalysisItemDefinitionId: {0}", this.DataAnalysisItemDefinitionId));

            DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinition.Settings as DAProfCalcOutputSettings;
            if (daProfCalcOutputSettings == null)
                throw new Exception(String.Format("daProfCalcOutputSettings is not of type DAProfCalcOutputSettings. it is of type {0}", dataAnalysisItemDefinition.Settings.GetType()));

            IDAProfCalcOutputSettingsGetOutputFieldsContext outputFieldContext = null;
            return daProfCalcOutputSettings.GetOutputFields(outputFieldContext);
        }
    }
}
