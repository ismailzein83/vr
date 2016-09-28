using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldFormulaConvertFilterContext : IDataRecordFieldFormulaConvertFilterContext
    {
        string fieldName;
        Dictionary<string, DataRecordField> _recordTypeFieldsByName;

        public DataRecordFieldFormulaConvertFilterContext(Guid dataRecordTypeId,string fieldName)
        {
            _recordTypeFieldsByName = (new DataRecordTypeManager()).GetDataRecordTypeFields(dataRecordTypeId);
            if (_recordTypeFieldsByName == null)
                throw new NullReferenceException("_recordTypeFieldsByName");
            if (fieldName == null)
                throw new NullReferenceException("fieldName");
            this.fieldName = fieldName;
        }
        public DataRecordFieldType FieldType
        {
            get
            {
                DataRecordField field;
                if (!_recordTypeFieldsByName.TryGetValue(this.fieldName, out field))
                    throw new NullReferenceException(String.Format("field. fieldName '{0}'", fieldName));
                return field.Type;
            }
        }

        public DataRecordFieldType GetFieldType(string fieldName)
        {
            DataRecordField field;
            if (!_recordTypeFieldsByName.TryGetValue(fieldName, out field))
                throw new NullReferenceException(String.Format("field. fieldName '{0}'", fieldName));
            return field.Type;
        }

        public RecordFilter InitialFilter { get; set; }
    }
}
