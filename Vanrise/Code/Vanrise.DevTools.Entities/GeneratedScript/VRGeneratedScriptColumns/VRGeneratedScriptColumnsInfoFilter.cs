﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DevTools.Entities
{
    public class VRGeneratedScriptColumnsInfoFilter
    {
        public Guid ConnectionId { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public List<VRGeneratedScriptColumns> ColumnNames { get; set; }

    }
}