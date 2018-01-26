using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBTableColumnDefinition
    {
        public string DBColumnName { get; set; }

        public RDBDataType DataType { get; set; }

        public int? Size { get; set; }
    }
}
