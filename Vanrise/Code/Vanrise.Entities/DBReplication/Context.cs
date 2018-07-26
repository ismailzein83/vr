using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public interface IDBReplicationInitializeContext
    {
        Dictionary<string, List<DBReplicationTableDetails>> DBReplicationTableDetailsListByTargetServer { get; }

        Action<string> WriteInformation { get; }
    }
    public class DBReplicationInitializeContext : IDBReplicationInitializeContext
    {
        public Dictionary<string, List<DBReplicationTableDetails>> DBReplicationTableDetailsListByTargetServer { get; set; }

        public Action<string> WriteInformation { get; set; }
    }

    public interface IDBReplicationMigrateDataContext
    {
        DateTime FromTime { get; }

        DateTime ToTime { get; }

        int NumberOfDaysPerInterval { get; }

        Action<string> WriteInformation { get; }
    }

    public class DBReplicationMigrateDataContext : IDBReplicationMigrateDataContext
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public int NumberOfDaysPerInterval { get; set; }

        public Action<string> WriteInformation { get; set; }
    }

    public interface IDBReplicationFinalizeContext
    {
        Action<string> WriteInformation { get; }
    }

    public class DBReplicationFinalizeContext : IDBReplicationFinalizeContext
    {
        public Action<string> WriteInformation { get; set; }
    }

    public interface IDBReplicationTableMigrateDataContext
    {
        List<string> Columns { get; }
        string TargetConnectionString { get; }
        string TableSchema { get; }
        string TargetTempTableName { get; }
        string SourceTableName { get; }
        DateTime FromTime { get; }
        DateTime ToTime { get; }
        string FilterDateTimeColumn { get; }
        int NumberOfDaysPerInterval { get; }
        string IdColumn { get; }
        int? ChunkSize { get; }
        Action<string> WriteInformation { get; }
        DBReplicationPreInsert DbReplicationPreInsert { get; }
    }

    public class DBReplicationTableMigrateDataContext : IDBReplicationTableMigrateDataContext
    {
        public List<string> Columns { get; set; }

        public string TargetConnectionString { get; set; }

        public string TableSchema { get; set; }

        public string TargetTempTableName { get; set; }

        public string SourceTableName { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public string FilterDateTimeColumn { get; set; }

        public int NumberOfDaysPerInterval { get; set; }

        public string IdColumn { get; set; }

        public int? ChunkSize { get; set; }

        public Action<string> WriteInformation { get; set; }

        public DBReplicationPreInsert DbReplicationPreInsert { get; set; }
    }
}