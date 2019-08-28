﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum RDBDataType
    {
        [RDBDataTypeAttribute(Description = "Varchar", RequireSize = true, HasSizeRequired = false)]
        Varchar = 0,
        [RDBDataTypeAttribute(Description = "NVarchar", RequireSize = true, HasSizeRequired = false)]
        NVarchar = 1,
        [RDBDataTypeAttribute(Description = "Int")]
        Int = 2,
        [RDBDataTypeAttribute(Description = "BigInt")]
        BigInt = 3,
        [RDBDataTypeAttribute(Description = "Decimal", RequireSize = true, RequirePrecision = true, HasSizeRequired = true, HasPrecisionRequired = true)]
        Decimal = 4,
        [RDBDataTypeAttribute(Description = "DateTime")]
        DateTime = 5,
        [RDBDataTypeAttribute(Description = "UniqueIdentifier")]
        UniqueIdentifier = 6,
        [RDBDataTypeAttribute(Description = "Boolean")]
        Boolean = 7,
        [RDBDataTypeAttribute(Description = "VarBinary", RequireSize = true, HasSizeRequired = false)]
        VarBinary = 8,
        [RDBDataTypeAttribute(Description = "Cursor")]
        Cursor = 9,
        [RDBDataTypeAttribute(Description = "Date")]
        Date = 10,
        [RDBDataTypeAttribute(Description = "Time", RequireSize = true, HasSizeRequired = false)]
        Time = 11
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
        public bool HasSizeRequired { get; set; }
        public bool HasPrecisionRequired { get; set; }
        public static RDBDataTypeAttribute GetAttribute(RDBDataType rdbDataType)
        {
            return _cachedAttributes[rdbDataType];
        }
    }
}
