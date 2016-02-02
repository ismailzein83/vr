﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordFieldTypeConfig
    {
        public int DataRecordFieldTypeConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Editor { get; set; }

        public string DynamicGroupUIControl { get; set; }

        public string SelectorControl { get; set; }

        public bool IsSupportedInGenericRule { get; set; }
    }
}
