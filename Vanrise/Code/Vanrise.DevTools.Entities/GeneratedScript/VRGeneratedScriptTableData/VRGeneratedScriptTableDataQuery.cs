﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.DevTools.Entities
{
    public class VRGeneratedScriptTableDataQuery
    {
        public Guid ConnectionId { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public List<IdentifierColumn> IdentifierColumns { get; set; }
        public string WhereCondition { get; set; }
        public string JoinStatement { get; set; }
        public BulkActionState BulkActionState { get; set; }
        public BulkActionFinalState BulkActionFinalState { get; set; }
    }
    public class IdentifierColumn
    {
        public string ColumnName { get; set; }
    }
}