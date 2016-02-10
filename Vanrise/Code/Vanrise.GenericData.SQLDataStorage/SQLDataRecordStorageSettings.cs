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

        public List<SQLDataRecordStorageColumn> Columns { get; set; }
    }

    public class SQLDataRecordStorageColumn
    {
        public string ColumnName { get; set; }

        public string SQLDataType { get; set; }

        public string ValueExpression { get; set; }
    }
}
