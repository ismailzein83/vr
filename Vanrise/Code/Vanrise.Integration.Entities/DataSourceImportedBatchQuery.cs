﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceImportedBatchQuery
    {
        public int? DataSourceId { get; set; }
        public string BatchName { get; set; } // string is nullable by default
        public MappingResult MappingResult { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
