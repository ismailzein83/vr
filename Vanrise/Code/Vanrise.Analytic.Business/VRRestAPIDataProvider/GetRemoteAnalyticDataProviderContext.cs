﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class GetRemoteAnalyticDataProviderContext : IGetRemoteAnalyticDataProviderContext
    {
        public Guid AnalyticTableId { get; set; }
    }
}
