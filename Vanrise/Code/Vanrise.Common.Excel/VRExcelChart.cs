using Aspose.Cells;
using Aspose.Cells.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{
    public enum VRExcelChartType {
        Area = 0,
        Bar = 6,
        Bubble = 12,
        Column = 14,
        Cone = 21,
        Cylinder = 28,
        Doughnut = 35,
        Line = 37,
        Pie = 44,
        Pyramid = 50,
        Radar = 57,
        Scatter = 60,
    };
    public class VRExcelChart
    {
        VRExcelChartType _chartType;
        int _startingRow;
        int _endingRow;
        int _startingColumn;
        int _endingColumn;
        string _title;
        string _pivoteTableName;
        int? _pivoteTableSheetIndex;
        public VRExcelChart(VRExcelChartType chartType,int startingRow,int endingRow, int startingColumn, int endingColumn)
        {
            _chartType = chartType;
            _startingRow = startingRow;
            _endingRow = endingRow;
            _startingColumn = startingColumn;
            _endingColumn = endingColumn;
        }
        public void SetTitle(string title)
        {
            _title = title;
        }
        public void AddSeries(string title)
        {
        }
        public void SetPivotSource(int pivoteTableSheetIndex,string pivoteTableName)
        {
            _pivoteTableName = pivoteTableName;
            _pivoteTableSheetIndex = pivoteTableSheetIndex;
        }


        internal void GenerateChart(IVRExcelFileGenerateContext context, Worksheet worksheet)
        {
            var chartType = ChartType.Column;
            switch(_chartType)
            {
                case VRExcelChartType.Area:
                    chartType = ChartType.Area;
                    break;
                case VRExcelChartType.Bar:
                    chartType = ChartType.Bar;

                    break;
                case VRExcelChartType.Bubble:
                    chartType = ChartType.Bubble;

                    break;
                case VRExcelChartType.Column:
                    chartType = ChartType.Column;

                    break;
                case VRExcelChartType.Cone:
                    chartType = ChartType.Cone;

                    break;
                case VRExcelChartType.Cylinder:
                    chartType = ChartType.Cylinder;

                    break;
                case VRExcelChartType.Doughnut:
                    chartType = ChartType.Doughnut;

                    break;
                case VRExcelChartType.Line:
                    chartType = ChartType.Line;

                    break;
                case VRExcelChartType.Pie:
                    chartType = ChartType.Pie;

                    break;
                case VRExcelChartType.Pyramid:
                    chartType = ChartType.Pyramid;

                    break;
                case VRExcelChartType.Radar:
                    chartType = ChartType.Radar;

                    break;
                case VRExcelChartType.Scatter:
                    chartType = ChartType.Scatter;

                    break;
                
            }

            int chartIndex = worksheet.Charts.Add(chartType, _startingRow, _startingColumn, _endingRow, _endingColumn);
            Chart chart = worksheet.Charts[chartIndex];
            chart.ShowLegend = true;
            chart.Title.Text = _title;

            if(_pivoteTableSheetIndex.HasValue && _pivoteTableName  != null)
            {
                chart.PivotSource = string.Format("{0}!{1}",context.GetSheetName(_pivoteTableSheetIndex.Value),_pivoteTableName);
                chart.HidePivotFieldButtons = false;
            }
            //chart.NSeries.Add("B2:B4", true);
            //chart.NSeries.CategoryData = "A2:A4";
            //Series series = chart.NSeries[0];
            //series.Name = "=B1";
        }
    }
}
