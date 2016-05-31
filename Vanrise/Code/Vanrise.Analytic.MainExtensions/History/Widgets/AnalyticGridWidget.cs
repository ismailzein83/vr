﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public class AnalyticGridWidget : AnalyticHistoryReportWidget
    {
        public bool RootDimensionsFromSearchSection { get; set; }

        public List<AnalyticGridWidgetDimension> Dimensions { get; set; }

        public List<AnalyticGridWidgetMeasure> Measures { get; set; }

        public List<MeasureStyleRule> MeasureStyleRules { get; set; }
        public AnalyticQueryOrderType OrderType { get; set; }
        public bool WithSummary { get; set; }
    }

    public class AnalyticGridWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public bool IsRootDimension { get; set; }
        public string Width { get; set; }
    }

    public class AnalyticGridWidgetMeasure
    {
        public string MeasureName { get; set; }

        public string Title { get; set; }
        public string Width { get; set; }
    }
}
