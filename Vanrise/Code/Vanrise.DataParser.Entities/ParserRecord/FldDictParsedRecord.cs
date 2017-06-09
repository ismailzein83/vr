using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.DataParser.Entities
{
    public class FldDictParsedRecord : ParsedRecord
    {
        public FldDictParsedRecord()
        {
            this.FieldValues = new Dictionary<string, object>();
        }
        public string RecordName { get; set; }
        public Dictionary<string, Object> FieldValues { get; set; }

        public override void SetFieldValue(string fieldName, object value)
        {
            this.FieldValues.GetOrCreateItem(fieldName);
            this.FieldValues[fieldName] = value;
        }

        public override object GetFieldValue(string fieldName)
        {
            Object value;
            FieldValues.TryGetValue(fieldName, out value);
            return value;
        }
    }
}
