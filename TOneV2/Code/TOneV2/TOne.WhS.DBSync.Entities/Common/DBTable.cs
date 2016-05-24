using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;

namespace TOne.WhS.DBSync.Entities
{
    public class DBTable
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Database { get; set; }
        public string ScriptedIndexes { get; set; }
        public Table Info { get; set; }
        public object Records { get; set; }
        public List<DBForeignKey> DBFKs { get; set; }
        public bool Migrated { get; set; }
        public bool MigrationRequested { get; set; }
    }
}
