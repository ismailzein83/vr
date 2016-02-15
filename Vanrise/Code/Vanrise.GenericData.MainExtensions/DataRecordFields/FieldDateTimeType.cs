using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldDateTimeType : DataRecordFieldType
    {
        public override Type GetRuntimeType()
        {
            return typeof(DateTime);
        }

        public override string GetDescription(Object value)
        {
            return value.ToString();
        }
    }
}
