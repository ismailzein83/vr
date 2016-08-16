﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{

    public class SourceBatches
    {
        public List<SourceBEBatch> SorceBEBatches { get; set; }
    }

    public abstract class SourceBEBatch
    {
        public abstract string BatchName { get; }
    }
}
