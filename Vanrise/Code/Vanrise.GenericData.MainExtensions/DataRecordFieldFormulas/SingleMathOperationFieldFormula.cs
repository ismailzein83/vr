using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class SingleMathOperationFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("5B8471BA-3D9A-412D-A158-27BDEC9C4094"); } }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            throw new NotImplementedException();
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
