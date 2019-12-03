using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcOutputSettingsManager
    {
        public IEnumerable<DAProfCalcOutputField> GetFilteredOutputFields(Guid dataAnalysisItemDefinitionId, DAProfCalcOutputFieldFilter filter)
        {
            List<DAProfCalcOutputField> daProfCalcOutputFields = GetOutputFields(dataAnalysisItemDefinitionId);

            Func<DAProfCalcOutputField, bool> predicate = (itm) =>
            {
                if (filter == null && itm.IsTechnical)
                    return false;

                if (filter != null)
                {
                    if (!filter.IncludeTechnicalField && itm.IsTechnical)
                        return false;

                    if (filter.DAProfCalcOutputFieldType.HasValue && itm.DAProfCalcOutputFieldType != filter.DAProfCalcOutputFieldType.Value)
                        return false;
                }

                return true;
            };

            return daProfCalcOutputFields.FindAllRecords(predicate);
        }

        public List<DataRecordField> GetInputFields(Guid dataAnalysisDefinitionId)
        {
            DataAnalysisDefinitionManager dataAnalysisDefinitionManager = new DataAnalysisDefinitionManager();
            DAProfCalcSettings settings = dataAnalysisDefinitionManager.GetDataAnalysisDefinitionSettings<DAProfCalcSettings>(dataAnalysisDefinitionId);
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            return dataRecordTypeManager.GetDataRecordTypeFields(settings.DataRecordTypeId).Values.ToList();
        }

        private List<DAProfCalcOutputField> GetOutputFields(Guid dataAnalysisItemDefinitionId)
        {
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            IDAProfCalcOutputSettingsGetOutputFieldsContext context = null;
            return dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(dataAnalysisItemDefinitionId).GetOutputFields(context);
        }
    }
}