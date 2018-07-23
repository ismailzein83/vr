using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class DBReplicationDefinitionSettings : VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("BB07A3B5-E519-4A6C-B4C6-695069BBB64A"); } }

        public Dictionary<Guid, DBReplicationDatabaseDefinition> DatabaseDefinitions { get; set; }
    }

    public class DBReplicationDatabaseDefinition
    {
        public string Name { get; set; }

        public List<DBReplicationTableDefinition> Tables { get; set; }
    }

    public class DBReplicationTableDefinition
    {
        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public string FilterDateTimeColumn { get; set; }

        public string IdColumn { get; set; }

        public int? ChunkSize { get; set; }
        public DBReplicationPreInsert DBReplicationPreInsert { get; set; }
    }

    public abstract class DBReplicationPreInsert
    {
        public abstract void Execute(IDBReplicationPreInsertExecuteContext context);
    }

    public interface IDBReplicationPreInsertExecuteContext
    {
        Object DataToInsert { get; set; }
    }
}