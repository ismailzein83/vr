using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.ExcelConversion.Business
{
    public class MappingFilterMatchContext : IRecordFilterGenericFieldMatchContext
    {
        public Dictionary<string, Vanrise.GenericData.Entities.DataRecordFieldType> fieldTypeByFieldName;
        public Dictionary<string, Object> fieldValueByFieldName;
        public object GetFieldValue(string fieldName, out Vanrise.GenericData.Entities.DataRecordFieldType fieldType)
        {
            if (this.fieldTypeByFieldName == null)
                throw new NullReferenceException("fieldTypeByFieldName");
            fieldTypeByFieldName.TryGetValue(fieldName, out fieldType);
            if (this.fieldValueByFieldName == null)
                throw new NullReferenceException("fieldValueByFieldName");

            object fieldValue = null;
            fieldValueByFieldName.TryGetValue(fieldName, out fieldValue);
            return fieldValue;
        }
    }
}
