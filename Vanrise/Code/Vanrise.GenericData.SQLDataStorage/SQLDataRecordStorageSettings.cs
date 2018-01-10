using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    public class SQLDataRecordStorageSettings : DataRecordStorageSettings
    {
        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public List<SQLDataRecordStorageColumn> Columns { get; set; }

        public List<NullableField> NullableFields { get; set; }

        public bool IncludeQueueItemId { get; set; }
        
        /// <summary>
        /// this property is needed only in case the current storage (SQL table) is used to store Summary data (Statistic/Billing)
        /// </summary>
        //public SQLDataRecordStorageSummarySettings SummarySettings { get; set; }
    }

    public class SQLDataRecordStorageColumn
    {
        public string ColumnName { get; set; }

        public string SQLDataType { get; set; }

        public string ValueExpression { get; set; }
        public bool IsUnique { get; set; }
    }

    public class NullableField
    {
        public string Name { get; set; }
    }

    //public class SQLDataRecordStorageSummarySettings
    //{
    //    public string IdColumnName { get; set; }
    //}
}
