﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BusinessEntityDefinitionInfo
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public string Name { get; set; }
        public string SelectorFilterEditor { get; set; }
        public string WorkFlowAddBEActivityEditor { get; set; }
        public string WorkFlowUpdateBEActivityEditor { get; set; }
    }
}
