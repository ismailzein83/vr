﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceBeforeIdInput
    {
        public long LessThanID { get; set; }
        public int NbOfRows { get; set; }
        public List<int> DefinitionsId { get; set; }
        public int ParentId { get; set; }
        public string EntityId { get; set; }
    }
}
