using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateIDs
{
    public class MiratorParameter
    {
        public string MainTableName { get; set; }
        public string SchemaName { get; set; }
        public string IDColumnName { get; set; }
        public string OldColumnName { get; set; }
        public string IDColumnType { get; set; }
        public bool IsPrimaryColumn { get; set; }
        public List<MiratorTableInfo> UpdatedTables { get; set; }
    }
    public class MiratorTableInfo {
        public string TableName { get; set; }
        public bool UseQuotationIdentifier { get; set; }
        public string ColumnName { get; set; }
        public string Identifier { get; set; }
        public string ConnectionStringKey { get; set; }
    }
}
