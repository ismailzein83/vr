using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBDataRecordStorageSettings : DataRecordStorageSettings
    {
        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public List<RDBDataRecordStorageColumn> Columns { get; set; }

        public List<RDBNullableField> NullableFields { get; set; }

        public bool IncludeQueueItemId { get; set; }

    }

    public class RDBDataRecordStorageColumn
    {
        public string FieldName { get; set; }
        
        public RDBTableColumnDefinition RDBColumnDefinition { get; set; }
        
        public bool IsUnique { get; set; }

        public bool IsIdentity { get; set; }
    }

    public class RDBNullableField
    {
        public string Name { get; set; }
    }
}
