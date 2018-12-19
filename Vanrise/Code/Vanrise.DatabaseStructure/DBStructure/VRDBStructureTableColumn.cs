using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DatabaseStructure
{
    public class VRDBStructureTableColumn
    {
        public string ColumnName { get; set; }

        public VRDBStructureDataType DataType { get; set; }

        public int? Size { get; set; }

        public int? Precision { get; set; }

        public bool IsNullable { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsDefaultDateTime { get; set; }
    }
}
