﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticHistoryReportWidget
    {
        public abstract Guid ConfigId { get; }

        public Guid AnalyticTableId { get; set; }
        public string WidgetTitle { get; set; }
        public int ColumnWidth { get; set; }
        public bool ShowTitle { get; set; }
        public RecordFilterGroup RecordFilter { get; set; }

        public abstract List<string> GetMeasureNames();
    }
}
