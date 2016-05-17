using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldManager
    {
        public IEnumerable<DataRecordFieldInfo> GetDataRecordFieldsInfo(DataRecordFieldInfoFilter filter)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            List<DataRecordField> dataRecordFields = dataRecordTypeManager.GetDataRecordTypeFields(filter.DataRecordTypeId);
            if (dataRecordFields == null || dataRecordFields.Count == 0)
                return null;
            List<DataRecordFieldInfo> result = new List<DataRecordFieldInfo>();
            foreach (DataRecordField dataRecordField in dataRecordFields)
            {
                result.Add(DataRecordFieldInfoMapper(dataRecordField));
            }
            return result;
        }

        private DataRecordFieldInfo DataRecordFieldInfoMapper(DataRecordField dataRecordField)
        {
            return new DataRecordFieldInfo()
            {
                Entity = dataRecordField,
            };
        }
    }
}