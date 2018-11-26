using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Excel;
using Vanrise.Common;
namespace Vanrise.Analytic.Business
{
    public interface IReportGenerationCustomCodeContext
    {
        VRExcelFile CreateExcelFile();
        VRAutomatedReportResolvedDataList GetDataList(string queryTitle, string listName);
        Guid? GetSubTableIdByGroupingFields(List<string> groupingFields, string queryTitle, string listName);
        void SetCellStyle(VRExcelTableRowCell cell, string bgColor, string FontColor, int FontSize, bool isBold, VRExcelContainerHorizontalAlignment horizontalAlignment, bool setBorder = false);
    }
    public class ReportGenerationCustomCodeContext : IReportGenerationCustomCodeContext
    {
        Func<string, string, VRAutomatedReportResolvedDataList> _dataListFunc;
        Func<List<string>, string, string, Guid?> _getSubTableIdByGroupingFieldsFunc;
        VRExcelFile excelFile;

        public ReportGenerationCustomCodeContext(Func<string , string , VRAutomatedReportResolvedDataList> dataListFunc, Func<List<string>, string, string, Guid?> getSubTableIdByGroupingFieldsFunc)
        {
            _dataListFunc = dataListFunc;
            _getSubTableIdByGroupingFieldsFunc = getSubTableIdByGroupingFieldsFunc;
        }
        public VRExcelFile CreateExcelFile()
        {
            excelFile = new VRExcelFile();
            return excelFile;
        }
        public void SetCellStyle(VRExcelTableRowCell cell,string bgColor,string fontColor, int fontSize, bool isBold, VRExcelContainerHorizontalAlignment horizontalAlignment, bool setBorder = false)
        {
            var style = cell.CreateStyle();
            style.BGColor = bgColor;
            style.FontColor = fontColor;
            style.FontSize = fontSize;
            style.IsBold = isBold;
            style.HorizontalAlignment = horizontalAlignment;
            style.SetBorder = setBorder;
        }

        public VRAutomatedReportResolvedDataList GetDataList(string queryTitle, string listName)
        {
            _dataListFunc.ThrowIfNull("_dataListFunc");
            return _dataListFunc(queryTitle, listName);
        }

        public Guid? GetSubTableIdByGroupingFields(List<string> groupingFields, string queryTitle, string listName)
        {
            return _getSubTableIdByGroupingFieldsFunc(groupingFields, queryTitle, listName);
        }
    }
}
