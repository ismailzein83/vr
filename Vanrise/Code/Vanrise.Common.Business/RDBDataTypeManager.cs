using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Vanrise.Data.RDB;
namespace Vanrise.Common
{
    public class RDBDataTypeManager
    {
        public List<RDBDataTypeInfo> GetRDBDataTypeInfo()
        {
            var enumAttributes = Utilities.GetEnumAttributes<RDBDataType, RDBDataTypeAttribute>();
            List<RDBDataTypeInfo> rdbDataTypeInfo = null;
          
            if (enumAttributes != null && enumAttributes.Count > 0)
            {
                rdbDataTypeInfo = new List<RDBDataTypeInfo>();
                foreach (var attribute in enumAttributes)
                {
                    rdbDataTypeInfo.Add(new RDBDataTypeInfo()
                    {
                        Description = attribute.Value.Description,
                        RequireSize = attribute.Value.RequireSize,
                        RequirePrecision = attribute.Value.RequirePrecision,
                        Value = attribute.Key
                    });
                }
            }
            return rdbDataTypeInfo;
        }
    }
    public class RDBDataTypeInfo
    {
        public RDBDataType Value { get; set; }
        public string Description { get; set; }
        public bool RequireSize { get; set; }
        public bool RequirePrecision { get; set; }
    }
}
