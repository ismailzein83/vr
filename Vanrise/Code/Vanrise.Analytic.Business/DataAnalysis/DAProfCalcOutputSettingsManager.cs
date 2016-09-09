using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcOutputSettingsManager
    {
        public List<DataRecordField> GetOutputFields(Guid dataAnalysisItemDefinitionId)
        {
            DataAnalysisItemDefinitionManager _manager = new DataAnalysisItemDefinitionManager();
            DataAnalysisItemDefinition dataAnalysisItemDefinition = _manager.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);

            IDAProfCalcOutputSettingsGetOutputFieldsContext context = null;

            return ((DAProfCalcOutputSettings)dataAnalysisItemDefinition.Settings).GetOutputFields(context);
        }
    }
}
