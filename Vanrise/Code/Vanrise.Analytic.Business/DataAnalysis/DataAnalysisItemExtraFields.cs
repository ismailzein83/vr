using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using System.Linq;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcRecordTypeExtraFields : DataRecordTypeExtraField
    {
        public override Guid ConfigId { get { return new Guid("93F44A29-235D-4C3F-900E-6D7FE780CEF3"); } }

        public Guid DataAnalysisDefinitionId { get; set; }

        public Guid DataAnalysisItemDefinitionId { get; set; }


        public override List<DataRecordField> GetFields(IDataRecordExtraFieldContext context)
        {
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(this.DataAnalysisItemDefinitionId);

            IDAProfCalcOutputSettingsGetOutputFieldsContext outputFieldContext = null;
            List<DAProfCalcOutputField> daProfCalcOutputFields = daProfCalcOutputSettings.GetOutputFields(outputFieldContext);
            return BuildDataRecordFields(daProfCalcOutputFields);
        }

        private List<DataRecordField> BuildDataRecordFields(List<DAProfCalcOutputField> daProfCalcOutputFields)
        {
            if (daProfCalcOutputFields == null)
                return null;

            return daProfCalcOutputFields.Select(itm => new DataRecordField() { Name = itm.Name, Title = itm.Title, Type = itm.Type }).ToList();
        }
    }
}