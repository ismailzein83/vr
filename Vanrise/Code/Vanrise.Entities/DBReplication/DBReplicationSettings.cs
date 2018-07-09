using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class DBReplicationSettings
    {
        public List<SourceDBReplicationDBConnection> SourceDBConnections { get; set; }

        public List<TargetDBReplicationDBConnection> TargetDBConnections { get; set; }
    }

    public class SourceDBReplicationDBConnection
    {
        public string ConnectionStringName { get; set; }

        public List<DBReplicationDBConnectionSourceDefinition> SourceConnectionDefinitions { get; set; }
    }

    public class DBReplicationDBConnectionSourceDefinition
    {
        public Guid SourceDBConnectionDefinitionId { get; set; }
    }

    public class TargetDBReplicationDBConnection
    {
        public string LinkedServerDBName { get; set; }


        public List<DBReplicationDBConnectionTargetDefinition> TargetConnectionDefinitions { get; set; }
    }

    public class DBReplicationDBConnectionTargetDefinition
    {
        public Guid TargetDBConnectionDefinitionId { get; set; }
    }
}