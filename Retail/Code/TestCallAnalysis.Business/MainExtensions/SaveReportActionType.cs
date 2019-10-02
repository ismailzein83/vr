using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Business
{
    public enum ReportType { ReportTypeA = 1, ReportTypeB = 2, ReportTypeC = 3 }
    public class SaveReportActionType : GenerateFilesActionType
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
            var fileManager = new VRFileManager();
            var recordStorageManager = new DataRecordStorageManager();

            var attachementEntity = new Vanrise.GenericData.Entities.AttachmentFieldTypeEntityCollection();
            if (context.GeneratedFileItems != null && context.GeneratedFileItems.Count > 0)
            {
                foreach (var generator in context.GeneratedFileItems)
                {
                    VRFile attachment = new VRFile()
                    {
                        Name = generator.FileName,
                        Content = generator.FileContent
                    };

                    var fileId = fileManager.AddFile(attachment);
                    if (fileId != -1)
                    {
                        attachementEntity.Add(new Vanrise.GenericData.Entities.AttachmentFieldTypeEntity()
                        {
                            FileId = fileId,
                            Notes = "",
                            CreatedTime = DateTime.Now
                        });
                    }
                }
            }

            Dictionary<string, Object> fieldReportValues = new Dictionary<string, object>();
            fieldReportValues.Add("Type", (int)this.ReportType);
            fieldReportValues.Add("FileIds", attachementEntity);
            fieldReportValues.Add("SentTime", DateTime.Now);
            recordStorageManager.AddDataRecord(TCAnalReportDataRecordStorage, fieldReportValues, out object insertedReportId, out bool hasSucceededReportInserted);

            if (hasSucceededReportInserted)
            {
                context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("New Report with Id {0} has been added", insertedReportId);
                VRAutomatedReportResolvedDataList dataList = context.HandlerContext.GetDataList(this.ReportQueryId, this.ListName);
                if (dataList != null && dataList.Items != null && dataList.Items.Count > 0)
                {
                    foreach (VRAutomatedReportResolvedDataItem item in dataList.Items)
                    {
                        Dictionary<string, VRAutomatedReportResolvedDataFieldValue> Fields = item.Fields;

                        Dictionary<string, Object> fieldReportRecordValues = new Dictionary<string, object>();
                        fieldReportRecordValues.Add("ReportId", (int)insertedReportId);
                        fieldReportRecordValues.Add("CaseId", Fields[this.RecordId].Value);
                        recordStorageManager.AddDataRecord(TCAnalReportRecordsDataRecordStorage, fieldReportRecordValues, out object insertedReportRecordId, out bool hasSuccessfulReportRecordInserted);
                    }
                    context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("The number of report records that have been added is {0}", dataList.Items.Count);
                }
            }
        }
    }
}
