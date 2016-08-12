using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions
{
    public class RecordProfilingOutputSettings : DAProfCalcOutputSettings
    {
        public override Guid DefinitionTypeId
        {
            get { return new Guid("39E04643-3C5C-4D11-9D3C-41611C34F7B3"); }
        }

        public RecordFilterGroup RecordFilter { get; set; }

        public TimeRangeFilter TimeRangeFilter { get; set; }

        public List<DAProfCalcGroupingField> GroupingFields { get; set; }

        public List<DAProfCalcAggregationField> AggregationFields { get; set; }

        public List<DAProfCalcCalculationField> CalculationFields { get; set; }

        public override List<DataRecordField> GetOutputFields(IDAProfCalcOutputSettingsGetOutputFieldsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
