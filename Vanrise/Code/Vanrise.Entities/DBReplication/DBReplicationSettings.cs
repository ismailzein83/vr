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
        public string LinkedServerName { get; set; }

        public List<DBReplicationDBConnectionSourceDefinition> SourceConnectionDefinitions { get; set; }
    }

    public class DBReplicationDBConnectionSourceDefinition
    {
        public Guid SourceDBConnectionDefinitionId { get; set; }
    }
}
