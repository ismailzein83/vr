﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public enum ZoneItemStatus { ExistingNotChanged = 0, New = 1, ExistingClosed = 2 }

    public class ZoneItem
    {
        public long? ZoneId { get; set; }

        public string Name { get; set; }

        public DateTime? BED { get; set; }

        public DateTime? EED { get; set; }

        public ZoneItemStatus Status { get; set; }

        public string Message { get; set; }

        public int CountryId { get; set; }
    }
}
