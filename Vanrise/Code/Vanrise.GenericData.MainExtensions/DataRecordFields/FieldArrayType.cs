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
        public DataRecordFieldType FieldType { get; set; }

        public override Type GetRuntimeType()
        {
            Type fieldType = FieldType.GetRuntimeType();
            return typeof(List<>).MakeGenericType(fieldType);
        }

        public override string GetDescription(object value)
        {
            return null;
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return true;
        }
    }
}
