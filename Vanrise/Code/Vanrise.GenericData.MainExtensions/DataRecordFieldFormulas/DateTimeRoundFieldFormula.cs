using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class DateTimeRoundFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("9A9E268C-0DC3-4488-8F11-CBEFF8D70E1D"); } }

        public string DateTimeFieldName { get; set; }

        public DateTimeRecordFilterComparisonPart ComparisonPart { get; set; } 
        
        public int? RoundingIntervalInMinutes { get; set; } 

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { DateTimeFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            throw new NotImplementedException();
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
