using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Excel;

namespace Vanrise.Analytic.Business
{
    public interface IReportGenerationCustomCodeContext
    {
        VRExcelFile CreateExcelFile();
     //   VRAutomatedReportResolvedDataList GetDataList();

    }
    public class ReportGenerationCustomCodeContext : IReportGenerationCustomCodeContext
    {
        VRAutomatedReportResolvedDataList _dataListResolver;
        public ReportGenerationCustomCodeContext()
        {

        }
        public ReportGenerationCustomCodeContext(VRAutomatedReportResolvedDataList dataListResolver)
        {
            _dataListResolver = dataListResolver;
        }
        public VRExcelFile CreateExcelFile()
        {
            return new VRExcelFile();
        }

        //public VRAutomatedReportResolvedDataList GetDataList()
        //{
        //    return _dataListResolver;
        //}


    }
}
