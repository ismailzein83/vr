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
        internal static Guid S_ItemDefinitionTypeId = new Guid("39E04643-3C5C-4D11-9D3C-41611C34F7B3");
        public override Guid ItemDefinitionTypeId { get { return S_ItemDefinitionTypeId; } }

        public RecordFilterGroup RecordFilter { get; set; }

        public TimeRangeFilter TimeRangeFilter { get; set; }

        public List<DAProfCalcGroupingField> GroupingFields { get; set; }

        public List<DAProfCalcAggregationField> AggregationFields { get; set; }

        public List<DAProfCalcCalculationField> CalculationFields { get; set; }

        public FilterParameterCollection FilterParameters { get; set; }

        public override List<DAProfCalcOutputField> GetOutputFields(IDAProfCalcOutputSettingsGetOutputFieldsContext context)
        {
            List<DAProfCalcOutputField> fields = new List<DAProfCalcOutputField>();
            fields.AddRange(this.GroupingFields.Select(itm => new DAProfCalcOutputField { Name = itm.FieldName, Title = itm.FieldTitle, Type = itm.FieldType, DAProfCalcOutputFieldType = DAProfCalcOutputFieldType.GroupingField }));
            fields.AddRange(this.AggregationFields.Select(itm => new DAProfCalcOutputField { Name = itm.FieldName, Title = itm.FieldTitle, Type = itm.RecordAggregate.FieldType, DAProfCalcOutputFieldType = DAProfCalcOutputFieldType.AggregationField }));
            fields.AddRange(this.CalculationFields.Select(itm => new DAProfCalcOutputField { Name = itm.FieldName, Title = itm.FieldTitle, Type = itm.FieldType, DAProfCalcOutputFieldType = DAProfCalcOutputFieldType.CalculationField }));
            return fields;
        }
    }
}
