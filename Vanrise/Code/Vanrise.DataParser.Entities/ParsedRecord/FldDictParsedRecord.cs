﻿using System;
using System.Collections.Generic;

namespace Vanrise.DataParser.Entities
{
    public class FldDictParsedRecord : ParsedRecord
    {
        public string RecordName { get; set; }

        public Dictionary<string, Object> FieldValues { get; set; }

        public HashSet<string> TempFieldNames { get; set; }

        public Dictionary<string, Object> TempFieldValues { get; set; }

        public FldDictParsedRecord()
        {
            this.FieldValues = new Dictionary<string, object>();
            this.TempFieldValues = new Dictionary<string, object>();
        }

        public override void SetFieldValue(string fieldName, object value)
        {
            Dictionary<string, Object> fieldValues = this.TempFieldNames != null && this.TempFieldNames.Contains(fieldName) ? this.TempFieldValues : this.FieldValues;

            if (fieldValues.ContainsKey(fieldName))
                fieldValues[fieldName] = value;
            else
                fieldValues.Add(fieldName, value);
        }

        public override object GetFieldValue(string fieldName)
        {
            Dictionary<string, Object> fieldValues = this.TempFieldNames != null && this.TempFieldNames.Contains(fieldName) ? this.TempFieldValues : this.FieldValues;
            
            Object value;
            fieldValues.TryGetValue(fieldName, out value);
            return value;
        }
    }
}