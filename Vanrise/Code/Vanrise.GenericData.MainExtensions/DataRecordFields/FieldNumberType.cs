using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public enum FieldNumberDataType { Decimal = 0, Int = 1, BigInt = 2}
    public class FieldNumberType : DataRecordFieldType
    {
        public FieldNumberDataType DataType { get; set; }
    }
}
