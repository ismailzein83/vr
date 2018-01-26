using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBTableDefinition : IRDBTableQuerySource
    {
        public string DBSchemaName { get; set; }

        public string DBTableName { get; set; }

        public string CreatedTimeColumnName { get; set; }

        public string ModifiedTimeColumnName { get; set; }

        public Dictionary<string, RDBTableColumnDefinition> Columns { get; set; }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            if (!String.IsNullOrEmpty(this.DBSchemaName))
                return String.Concat(this.DBSchemaName, ".", this.DBTableName);
            else
                return this.DBTableName;
        }

        public string GetDescription()
        {
            if (!String.IsNullOrEmpty(this.DBSchemaName))
                return String.Concat(this.DBSchemaName, ".", this.DBTableName);
            else
                return this.DBTableName;
        }
    }
}
