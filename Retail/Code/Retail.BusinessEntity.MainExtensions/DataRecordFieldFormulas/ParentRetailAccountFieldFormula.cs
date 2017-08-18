using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class ParentRetailAccountFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("B3D9B0A4-B751-4544-8A7A-6764687059ED"); } }

        public string AccountFieldName { get; set; } 

        public Guid ParentAccountTypeId { get; set; }


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
