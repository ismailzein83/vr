using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class DBReplicationSettings
    {
        public List<DBReplicationDBConnection> DBConnections { get; set; }
    }

    public class DBReplicationDBConnection
    {
        public string SourceConnectionStringName { get; set; }

        public string TargetLinkedServerName { get; set; }

        public string TargetDatabaseName { get; set; }

        public List<DBConnectionDefinition> DBConnectionDefinitions { get; set; }
    }

    public class DBConnectionDefinition
    {
        public Guid DBReplicationDBConnectionDefinitionId { get; set; }
    }
}