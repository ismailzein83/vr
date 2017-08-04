using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldFormulaCalculateValueContext : IDataRecordFieldFormulaCalculateValueContext
    {
        public DataRecordFieldType FieldType { get; set; }

        Dictionary<string, DataRecordFieldType> DataRecordFieldTypeDict;

        Dictionary<string, dynamic> DataRecordFieldValueDict;

        public DataRecordFieldFormulaCalculateValueContext(Dictionary<string, DataRecordFieldType> dataRecordFieldTypeDict, Dictionary<string, dynamic> dataRecordFieldValueDict, DataRecordFieldType fieldType)
        {
            DataRecordFieldTypeDict = dataRecordFieldTypeDict;
            DataRecordFieldValueDict = dataRecordFieldValueDict;
            FieldType = fieldType;
        }

        public DataRecordFieldType GetFieldType(string fieldName)
        {
            return DataRecordFieldTypeDict.GetRecord(fieldName);
        }

        public dynamic GetFieldValue(string fieldName)
        {
            return DataRecordFieldValueDict.GetRecord(fieldName);
        }
    }
}