using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions
{
    public class MergingProfiledOutputSettings: DAProfCalcOutputSettings
    {
        public override Guid DefinitionTypeId
        {
            get { return new Guid("9086F241-2680-48CF-A1BD-B8538FF03801"); }
        }

        public List<MergingProfiledOutputSettingsSourceItem> SourceItems { get; set; }

        public List<MergingProfiledOutputSettingsJoinField> JoinsFields { get; set; }

        public List<DAProfCalcCalculationField> CalculationFields { get; set; }

        public override List<GenericData.Entities.DataRecordField> GetOutputFields(IDAProfCalcOutputSettingsGetOutputFieldsContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class MergingProfiledOutputSettingsSourceItem
    {
        public string Name { get; set; }

        public Guid RecordProfilingItemDefinitionId{ get; set; }
    }

    public class MergingProfiledOutputSettingsJoinField
    {
        public string FieldName { get; set; }

        public string FirstRecordName { get; set; }

        public string FirstRecordFieldName { get; set; }

        public string SecondRecordName { get; set; }

        public string SecondRecordFieldName { get; set; }
    }
}
