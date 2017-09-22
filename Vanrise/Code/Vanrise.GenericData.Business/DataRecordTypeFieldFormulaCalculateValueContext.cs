using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class DataRecordTypeFieldFormulaCalculateValueContext : IDataRecordFieldFormulaCalculateValueContext
    {
        public DataRecordFieldType FieldType { get; set; }

        Dictionary<string, DataRecordFieldType> _dataRecordFieldTypeDict;

        Func<string, dynamic> _getFieldValue;

        public DataRecordTypeFieldFormulaCalculateValueContext(Dictionary<string, DataRecordFieldType> dataRecordFieldTypeDict, Func<string, dynamic> getFieldValue, DataRecordFieldType fieldType)
        {
            FieldType = fieldType;
            _dataRecordFieldTypeDict = dataRecordFieldTypeDict;
            _getFieldValue = getFieldValue;
        }

        public DataRecordFieldType GetFieldType(string fieldName)
        {
            return _dataRecordFieldTypeDict.GetRecord(fieldName);
        }

        public dynamic GetFieldValue(string fieldName)
        {
            return _getFieldValue(fieldName);
        }
    }
}
