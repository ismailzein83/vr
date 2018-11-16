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

    }
    public class ReportGenerationCustomCodeContext : IReportGenerationCustomCodeContext
    {
        Func<string, string, VRAutomatedReportResolvedDataList> _dataListFunc;
        public ReportGenerationCustomCodeContext(Func<string , string , VRAutomatedReportResolvedDataList> dataListFunc)
        {
            _dataListFunc = dataListFunc;
        }
        public VRExcelFile CreateExcelFile()
        {
            return new VRExcelFile();
        }
        public VRAutomatedReportResolvedDataList GetDataList(string queryTitle, string listName)
        {
            _dataListFunc.ThrowIfNull("_dataListFunc");
            return _dataListFunc(queryTitle, listName);
        }
    }
}
