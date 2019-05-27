﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityQuery
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public Dictionary<string, object> FilterValuesByFieldPath { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
        public int? LimitResult { get; set; }
        public List<GenericBusinessEntityFilter> Filters { get; set; }
        public BulkActionState BulkActionState { get; set; }
        public OrderType? OrderType { get; set; }
        public AdvancedOrderOptionsBase AdvancedOrderOptions { get; set; }
    }
    public class GenericBusinessEntityFilter
    {
        public string FieldName { get; set; }
        public List<Object> FilterValues { get; set; }
    }
}
