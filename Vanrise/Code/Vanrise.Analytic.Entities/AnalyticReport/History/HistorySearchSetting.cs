﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class HistorySearchSetting : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Analytic_AnalyticHistoryReportSettings";

        public string Editor { get; set; }
        public string RuntimeEditor { get; set; }
    }
}
