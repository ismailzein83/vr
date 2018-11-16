using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Tools.Entities
{
    public class TableDataQuery
    {
        public Guid ConnectionId { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public List<IdentifierColumn> IdentifierColumns { get; set; }
        public string WhereCondition { get; set; }
        public BulkActionState BulkActionState { get; set; }
        public BulkActionFinalState BulkActionFinalState { get; set; }
    }
    public class IdentifierColumn
    {
        public string ColumnName { get; set; }
    }
}