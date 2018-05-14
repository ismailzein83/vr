using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class DBReplicationDefinitionSettings : VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("BB07A3B5-E519-4A6C-B4C6-695069BBB64A"); }
        }

        public Dictionary<Guid, DBReplicationSourceDBConnectionDefinition> SourceDBConnectionDefinitions { get; set; }

        public Dictionary<Guid, DBReplicationTargetDBConnectionDefinition> TargetDBConnectionDefinitions { get; set; }

        public List<DBReplicationTableDefinitionSettings> Tables { get; set; }
    }

    public class DBReplicationSourceDBConnectionDefinition
    {
        public string Name { get; set; }
    }

    public class DBReplicationTargetDBConnectionDefinition
    {
        public string Name { get; set; }

        public string ConnectionStringName { get; set; }

        public string ConnectionStringAppSettingKey { get; set; }
    }

    public class DBReplicationTableDefinitionSettings
    {
        public string TableName { get; set; }

        public Guid SourceDBConnectionDefinitionId { get; set; }

        public Guid TargetDBConnectionDefinitionId { get; set; }

        public string FilterDateTimeColumn { get; set; }
    }
}
