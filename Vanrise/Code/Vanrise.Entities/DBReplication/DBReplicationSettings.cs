using System;
using System.Collections.Generic;

namespace Vanrise.Entities
{
    public class DBReplicationSettings
    {
        public List<DBReplicationDBConnection> DBConnections { get; set; }
    }

    public class DBReplicationDBConnection
    {
        public string SourceConnectionStringName { get; set; }

        public Guid TargetConnectionId { get; set; }

        public List<DBConnectionSettings> Settings { get; set; }
    }

    public class DBConnectionSettings
    {
        public Guid DatabaseDefinitionId { get; set; }
    }
}