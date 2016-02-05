using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.MainExtensions.DataStorages
{    
    public class SQLDataRecordStorageState
    {
        public List<SQLDataRecordStorageStateColumn> Columns { get; set; }
    }

    public class SQLDataRecordStorageStateColumn
    {
        public string ColumnName { get; set; }

        public string SQLDataType { get; set; }
    }
}
