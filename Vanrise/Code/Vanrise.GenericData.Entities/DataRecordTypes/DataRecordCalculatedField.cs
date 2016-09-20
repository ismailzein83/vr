using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordFieldFormula
    {
        public  abstract Guid ConfigId { get; }

        public abstract dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context);

        public abstract RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context);
    }

    public interface IDataRecordFieldFormulaContext
    {
        DataRecordFieldType FieldType { get; }

        DataRecordFieldType GetFieldType(string fieldName);
    }

    public interface IDataRecordFieldFormulaCalculateValueContext : IDataRecordFieldFormulaContext
    {
        dynamic GetFieldValue(string fieldName);
    }


    public interface IDataRecordFieldFormulaConvertFilterContext : IDataRecordFieldFormulaContext
    {
        RecordFilter InitialFilter { get; }
    }
}
