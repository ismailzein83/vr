using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldArrayType : DataRecordFieldType
    {
        public int DataRecordFieldTypeConfigId { get; set; }
        public List<DataRecordFieldType> Items { get; set; }

        public override Type GetRuntimeType()
        {
            //DataRecordFieldTypeConfig dataRecordFieldTypeConfig = new DataRecordFieldTypeConfigManager().GetDataRecordFieldTypeConfig(DataRecordFieldTypeConfigId);
            //if (dataRecordFieldTypeConfig == null)
            //    throw new NullReferenceException("dataRecordFieldTypeConfig");
            throw new NotImplementedException();
        }

        public override string GetDescription(object value)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }
    }
}
