using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DatabaseStructure
{
    public class VRDBStructureTableForeignKey
    {
        public string ForeignKeyName { get; set; }

        public string ColumnName { get; set; }

        public string ParentTableSchema { get; set; }

        public string ParentTable { get; set; }

        public string ParentColumnName { get; set; }
    }
}
