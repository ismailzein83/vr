using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecord
    {
        public int DataRecordTypeId { get; set; }

        public Dictionary<string, Object> FieldsValues { get; set; }

        public DateTime Time { get; set; }

        public Object this[string fieldName]
        {
            get
            {
                Object value;
                if (this.FieldsValues.TryGetValue(fieldName, out value))
                    return value;
                else
                    throw new Exception(String.Format("Field '{0}' not found", fieldName));
            }
            set
            {
                if (this.FieldsValues.ContainsKey(fieldName))
                    this.FieldsValues[fieldName] = value;
                else
                    this.FieldsValues.Add(fieldName, value);
            }
        }

        public T GetFieldValue<T>(string fieldName)
        {
            Object value = this[fieldName];
            if (value is T)
                return (T)value;
            else

                throw new Exception(String.Format("Field '{0}' is not of type '{1}'", fieldName, typeof(T)));
        }
    }
}
