using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (!this.FieldValues.ContainsKey(fieldName))
                this.FieldValues.Add(fieldName, value);
        }
    }
}
