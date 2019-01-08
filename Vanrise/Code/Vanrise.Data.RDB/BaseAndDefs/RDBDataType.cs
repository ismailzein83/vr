using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBDataType
    {
        [RDBDataTypeAttribute(Description = "Varchar", RequireSize = true)]
        Varchar = 0,
        [RDBDataTypeAttribute(Description = "NVarchar", RequireSize = true)]
        NVarchar = 1,
        [RDBDataTypeAttribute(Description = "Int")]
        Int = 2,
        [RDBDataTypeAttribute(Description = "BigInt")]
        BigInt = 3,
        [RDBDataTypeAttribute(Description = "Decimal", RequireSize = true, RequirePrecision =true)]
        Decimal = 4,
        [RDBDataTypeAttribute(Description = "DateTime")]
        DateTime = 5,
        [RDBDataTypeAttribute(Description = "UniqueIdentifier")]
        UniqueIdentifier = 6,
        [RDBDataTypeAttribute(Description = "Boolean")]
        Boolean = 7,
        [RDBDataTypeAttribute(Description = "VarBinary", RequireSize = true)]
        VarBinary = 8,
        [RDBDataTypeAttribute(Description = "Cursor")]
        Cursor = 9
    }
    public class RDBDataTypeAttribute : Attribute
    {
        static Dictionary<RDBDataType, RDBDataTypeAttribute> _cachedAttributes;
        static RDBDataTypeAttribute()
        {
            _cachedAttributes = new Dictionary<RDBDataType, RDBDataTypeAttribute>();
            foreach (var member in typeof(RDBDataType).GetFields())
            {
                RDBDataTypeAttribute mbrAttribute = member.GetCustomAttributes(typeof(RDBDataTypeAttribute), true).FirstOrDefault() as RDBDataTypeAttribute;
                if (mbrAttribute != null)
                    _cachedAttributes.Add((RDBDataType)Enum.Parse(typeof(RDBDataType), member.Name), mbrAttribute);
            }
        }
        public string Description { get; set; }
        public bool RequireSize { get; set; }
        public bool RequirePrecision { get; set; }
        public static RDBDataTypeAttribute GetAttribute(RDBDataType rdbDataType)
        {
            return _cachedAttributes[rdbDataType];
        }
    }

}
