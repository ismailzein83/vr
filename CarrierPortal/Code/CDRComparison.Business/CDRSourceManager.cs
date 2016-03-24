﻿using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Business
{
    public class CDRSourceManager
    {
        public CDRSample ReadSample(CDRSource cdrSource)
        {
            return cdrSource.ReadSample(new ReadSampleFromSourceContext());
        }
    }
}
