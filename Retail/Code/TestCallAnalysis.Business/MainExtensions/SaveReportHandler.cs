using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Business
{
    public enum ReportType { ReportType1 = 1, ReportType2 = 2, ReportType3 = 3}
    public class SaveReportHandler : GenerateFilesActionType
    {
        static Guid TCAnalReportDataRecordStorage = new Guid("45B7E9F3-D783-4A6A-B6C4-7BE1AA62A679");

        static Guid TCAnalReportRecordsDataRecordStorage = new Guid("6EBE2370-E4A8-4625-A29A-AB00559A831F");
        public override Guid ConfigId
        {
            get { return new Guid("2884E0F4-6FB0-4CC2-8F18-EE73184AB9AB"); }
        }

        public ReportType ReportType { get; set; }
        public Guid ReportQueryId { get; set; }
        public string ListName { get; set; }
        public string RecordId { get; set; }

        public override void Execute(IGenerateFilesActionTypeContext context)
        {
        }
    }
}
