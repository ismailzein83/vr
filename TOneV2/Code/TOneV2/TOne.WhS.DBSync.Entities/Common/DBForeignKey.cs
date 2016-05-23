using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class DBForeignKey
    {
        public string ReferencedKey { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedTableSchema { get; set; }
    }
}
