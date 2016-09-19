﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public class AnalyticChartWidget : AnalyticHistoryReportWidget
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("D050DEB3-700E-437B-86D1-510A81C0C14C"); } }
        public List<AnalyticChartWidgetDimension> Dimensions { get; set; }
        public List<AnalyticChartWidgetMeasure> Measures { get; set; }
        public AnalyticQueryOrderType OrderType { get; set; }
        public int? TopRecords { get; set; }
        public string ChartType { get; set; }
        public bool RootDimensionsFromSearch { get; set; }

        public override List<string> GetMeasureNames()
        {
            return this.Measures.Select(measure => measure.MeasureName).ToList();
        }
    }

    public class AnalyticChartWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
    }

    public class AnalyticChartWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
    }
}
