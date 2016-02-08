﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordStorageQuery
    {
        public string Name { get; set; }

        public IEnumerable<int> DataRecordTypeIds { get; set; }

        public IEnumerable<int> DataStoreIds { get; set; }
    }
}
