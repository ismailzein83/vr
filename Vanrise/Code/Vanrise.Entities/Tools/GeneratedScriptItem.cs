using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.Tools
{
    public class GeneratedScriptItem
    {
        public List<GeneratedScriptItemTable> Tables { get; set; }
    }

    public class GeneratedScriptItemTable
    {
        public Guid ConnectionId { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public List<GeneratedScriptItemTableColumn> InsertColumns { get; set; }
        public List<GeneratedScriptItemTableColumn> UpdateColumns { get; set; }
        public List<GeneratedScriptItemTableColumn> IdentifierColumns { get; set; }
        public List<GeneratedScriptItemTableRow> DataRows { get; set; }

    }
    public class GeneratedScriptItemTableColumn
    {
        public string ColumnName { get; set; }
    }

    public class GeneratedScriptItemTableRow
    {
        public Dictionary<string, Object> FieldValues { get; set; }
    }
}
