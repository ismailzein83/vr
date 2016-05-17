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
        public string ScriptedTempTable { get; set; }
        public List<DBForeignKey> DBFKs { get; set; }
        public Table Info { get; set; }
        public bool DroppedOriginal { get; set; }
        public object Records { get; set; }
    }
}
