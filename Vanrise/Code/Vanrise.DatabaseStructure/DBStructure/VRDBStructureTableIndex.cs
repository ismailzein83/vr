using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DatabaseStructure
{
    public class VRDBStructureTableIndex
    {
        public string IndexName { get; set; }

        public List<VRDBStructureTableIndexColumn> Columns { get; set; }

        public bool IsClustered { get; set; }

        public bool IsUnique { get; set; }
    }

    public class VRDBStructureTableIndexColumn
    {
        public string ColumnName { get; set; }

        public bool IsAscending { get; set; }
    }
}
