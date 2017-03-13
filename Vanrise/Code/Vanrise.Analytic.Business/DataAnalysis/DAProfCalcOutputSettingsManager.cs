using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using System.Linq;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcOutputSettingsManager
    {
        public List<DataRecordField> GetOutputFields(Guid dataAnalysisItemDefinitionId)
        {
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            IDAProfCalcOutputSettingsGetOutputFieldsContext context = null;
            return dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(dataAnalysisItemDefinitionId).GetOutputFields(context);
        }

        public List<DataRecordField> GetInputFields(Guid dataAnalysisDefinitionId)
        {
            DataAnalysisDefinitionManager dataAnalysisDefinitionManager = new DataAnalysisDefinitionManager();
            DAProfCalcSettings settings = dataAnalysisDefinitionManager.GetDataAnalysisDefinitionSettings<DAProfCalcSettings>(dataAnalysisDefinitionId);
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return dataRecordTypeManager.GetDataRecordTypeFields(settings.DataRecordTypeId).Values.ToList();
        }
    }
}