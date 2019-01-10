using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Vanrise.Entities;
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
   
}
