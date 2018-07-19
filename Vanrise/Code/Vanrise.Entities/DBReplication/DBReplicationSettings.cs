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

        public string TargetConnectionString { get; set; }

        public List<DBConnectionSettings> Settings { get; set; }
    }

    public class DBConnectionSettings
    {
        public Guid DatabaseDefinitionId { get; set; }
    }
}