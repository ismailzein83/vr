using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DatabaseStructure
{
    public class VRDBStructureTable
    {
        public string SchemaName { get; set; }

        public string TableName { get; set; }

        public List<VRDBStructureTableColumn> Columns { get; set; }

        public List<VRDBStructureTablePrimaryKey> PrimaryKeys { get; set; }

        public List<VRDBStructureTableIndex> Indexes { get; set; }

        public List<VRDBStructureTableForeignKey> ForeignKeys { get; set; }
    }

    public class VRDBStructureTablePrimaryKey
    {
        public string ColumnName { get; set; }
    }
}
