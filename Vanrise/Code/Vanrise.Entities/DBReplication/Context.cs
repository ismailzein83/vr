using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public interface IDBReplicationInitializeContext
    {
        Dictionary<string, List<DBReplicationTableDetails>> DBReplicationTableDetailsListBySourceConnection { get; }
    }
    public class DBReplicationInitializeContext : IDBReplicationInitializeContext
    {
        public Dictionary<string, List<DBReplicationTableDetails>> DBReplicationTableDetailsListBySourceConnection { get; set; }
    }

    public interface IDBReplicationMigrateDataContext
    {
        DateTime FromTime { get; }

        DateTime ToTime { get; }
    }

    public class DBReplicationMigrateDataContext : IDBReplicationMigrateDataContext
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
    }

    public interface IDBReplicationFinalizeContext
    {
        Action<string> WriteInformation { get; }
    }

    public class DBReplicationFinalizeContext : IDBReplicationFinalizeContext
    {
        public Action<string> WriteInformation { get; set; }
    }
}
