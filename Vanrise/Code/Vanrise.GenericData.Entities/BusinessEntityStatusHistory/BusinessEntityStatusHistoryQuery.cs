﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BusinessEntityStatusHistoryQuery
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public string BusinessEntityId { get; set; }
		public string FieldName { get; set; }
    }
}
