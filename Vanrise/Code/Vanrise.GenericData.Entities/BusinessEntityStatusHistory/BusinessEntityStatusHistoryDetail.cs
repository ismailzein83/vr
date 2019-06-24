﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BusinessEntityStatusHistoryDetail
    {
        public long BusinessEntityStatusHistoryId { get; set; }
        public string FieldName { get; set; }
        public string StatusName { get; set; }
        public string PreviousStatusName { get; set; }
        public DateTime StatusChangedDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
