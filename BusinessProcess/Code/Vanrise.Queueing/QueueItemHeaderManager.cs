using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using System.Configuration;

namespace Vanrise.Queueing
{
    public class QueueItemHeaderManager
    {


        #region ctor/Local Variables

        static Vanrise.Integration.Entities.IDataSourceManager s_dataSourceManager = Vanrise.Integration.Entities.BEManagerFactory.GetManager<Vanrise.Integration.Entities.IDataSourceManager>();

        static QueueItemHeaderManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Queueing_GetItemStatusSummaryTimeInterval"], out s_GetItemStatusSummaryTimeInterval))
                s_GetItemStatusSummaryTimeInterval = new TimeSpan(0, 0, 2);
        }

        #endregion

        #region Public Methods

        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            if (DateTime.Now - s_lastTimeCalled > s_GetItemStatusSummaryTimeInterval)
            {
                lock (s_thisLock)
                {
                    if (DateTime.Now - s_lastTimeCalled > s_GetItemStatusSummaryTimeInterval)
                    {
                        IQueueItemHeaderDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemHeaderDataManager>();
                        s_itemStatusSummary = manager.GetItemStatusSummary();
                        s_lastTimeCalled = DateTime.Now;
                    }
                }
            }
            return s_itemStatusSummary;

        }


        public IEnumerable<ExecutionFlowStatusSummary> GetExecutionFlowStatusSummary()
        {
            QueueInstanceManager queueInstanceManager = new QueueInstanceManager();
            List<QueueItemStatusSummary> queueItemStatusSummary = GetItemStatusSummary();
            IEnumerable<QueueInstance> queueInstances = queueInstanceManager.GetAllQueueInstances();

            IEnumerable<ExecutionFlowStatusSummary> result = from qInstances in queueInstances
                                                             join qItemStatusSummary in queueItemStatusSummary
                                                             on qInstances.QueueInstanceId equals qItemStatusSummary.QueueId
                                                             group new { qItemStatusSummary, qInstances, qItemStatusSummary.Count }
                                                             by new { qItemStatusSummary, qItemStatusSummary.Status, qInstances.ExecutionFlowId } into res
                                                             select new ExecutionFlowStatusSummary
                                                             {
                                                                 ExecutionFlowId = (Guid)res.Key.ExecutionFlowId,
                                                                 Status = res.Key.Status,
                                                                 Count = res.Key.qItemStatusSummary.Count
                                                             };



            IEnumerable<ExecutionFlowStatusSummary> filteredResult = from c in result
                                                                     group c by new { c.ExecutionFlowId, c.Status } into item
                                                                     select new ExecutionFlowStatusSummary
                                                                     {
                                                                         ExecutionFlowId = item.Key.ExecutionFlowId,
                                                                         Status = item.Key.Status,
                                                                         Count = item.Sum(x => x.Count)
                                                                     };

            return filteredResult;
        }

        public Vanrise.Entities.IDataRetrievalResult<QueueItemHeaderDetails> GetFilteredQueueItemHeader(Vanrise.Entities.DataRetrievalInput<QueueItemHeaderQuery> input)
        {

            IQueueItemHeaderDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueItemHeaderDataManager>();

            if (input.Query.QueueIds == null && input.Query.ExecutionFlowIds != null && input.Query.ExecutionFlowIds.Count() > 0)
            {
                QueueInstanceManager queueInstanceManger = new QueueInstanceManager();
                input.Query.QueueIds = queueInstanceManger.GetQueueExecutionFlows(input.Query.ExecutionFlowIds).Select(x => x.QueueInstanceId).ToList();
                input.Query.ExecutionFlowIds = null;
            }


            Vanrise.Entities.BigResult<QueueItemHeader> queueItemHeader = dataManager.GetFilteredQueueItemHeader(input);
            BigResult<QueueItemHeaderDetails> queueItemHeaderDetailResult = new BigResult<QueueItemHeaderDetails>()
            {
                ResultKey = queueItemHeader.ResultKey,
                TotalCount = queueItemHeader.TotalCount,
                Data = queueItemHeader.Data.MapRecords(QueueItemHeaderDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueItemHeaderDetailResult, new ResultProcessingHandler<QueueItemHeaderDetails> { ExportExcelHandler = new QueueItemHeaderExportHandler() });
        }

        #endregion


        #region Private Methods

        private static TimeSpan s_GetItemStatusSummaryTimeInterval;

        private static DateTime s_lastTimeCalled = new DateTime();

        private static List<QueueItemStatusSummary> s_itemStatusSummary;

        private static Object s_thisLock = new Object();

        internal static QueueItemHeaderDetails QueueItemHeaderDetailMapper(QueueItemHeader queueItemHeader)
        {
            QueueItemHeaderDetails queueItemHeaderDetail = new QueueItemHeaderDetails();
            QueueInstanceManager queueManager = new QueueInstanceManager();
            QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            var instance = queueManager.GetQueueInstanceById(queueItemHeader.QueueId);
            queueItemHeaderDetail.Entity = queueItemHeader;
            queueItemHeaderDetail.StageName = instance != null ? instance.StageName : "";
            queueItemHeaderDetail.StatusName = Vanrise.Common.Utilities.GetEnumDescription(queueItemHeader.Status);
            queueItemHeaderDetail.QueueTitle = instance.Title;
            queueItemHeaderDetail.ExecutionFlowName = executionFlowManager.GetExecutionFlowName((Guid)instance.ExecutionFlowId);
            if (queueItemHeader.DataSourceID.HasValue)
                queueItemHeaderDetail.DataSourceName = s_dataSourceManager.GetDataSourceName(queueItemHeader.DataSourceID.Value);
            return queueItemHeaderDetail;
        }

        #endregion

        #region Private Classes

        internal class QueueItemHeaderExportHandler : ExcelExportHandler<QueueItemHeaderDetails>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<QueueItemHeaderDetails> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Queue Items",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Item Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Execution Flow" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Queue" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Stage" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Data Source" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Imported Batch" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Error Message" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Created Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Last Updated Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        if (record.Entity != null)
                        {
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ItemId });
                            row.Cells.Add(new ExportExcelCell { Value = record.ExecutionFlowName });
                            row.Cells.Add(new ExportExcelCell { Value = record.QueueTitle });
                            row.Cells.Add(new ExportExcelCell { Value = record.StageName });
                            row.Cells.Add(new ExportExcelCell { Value = record.DataSourceName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BatchDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Description });
                            row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(record.Entity.Status) });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ErrorMessage });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CreatedTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.LastUpdatedTime });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

    }
}
