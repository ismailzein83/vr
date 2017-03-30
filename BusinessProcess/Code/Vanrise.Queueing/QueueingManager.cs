using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Queueing
{
    public class QueueingManager
    {

        #region ctor
        private readonly IQueueDataManager _dataManager;
        private readonly IQueueItemDataManager _itemDataManager;

        public QueueingManager()
        {
            _dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _itemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
        }
        #endregion

        #region Public Methods

        
       
        public List<QueueItemType> GetQueueItemTypes()
        {
            return _dataManager.GetQueueItemTypes();
        }
       
        
        public List<QueueItemHeader> GetHeaders(IEnumerable<int> queueIds, IEnumerable<QueueItemStatus> statuses, DateTime dateFrom, DateTime dateTo)
        {
            return _itemDataManager.GetHeaders(queueIds, statuses, dateFrom, dateTo);
        }
        public Dictionary<long, ItemExecutionFlowInfo> GetItemsExecutionFlowStatus(List<long> itemIds)
        {
            List<ItemExecutionFlowInfo> originalList = _itemDataManager.GetItemExecutionFlowInfo(itemIds);

            var subLists = originalList.GroupBy(p => p.ItemId)
                        .Select(g => g.ToList());

            Dictionary<long, ItemExecutionFlowInfo> itemsExectionStatusDic = new Dictionary<long, ItemExecutionFlowInfo>();

            foreach (List<ItemExecutionFlowInfo> list in subLists)
            {
                ItemExecutionFlowInfo resultItem =
                        new ItemExecutionFlowInfo() { ItemId = list[0].ItemId, ExecutionFlowTriggerItemId = list[0].ExecutionFlowTriggerItemId };

                resultItem.Status = this.GetExecutionFlowStatus(list);
                itemsExectionStatusDic.Add(list[0].ItemId, resultItem);
            }

            return itemsExectionStatusDic;
        }
        public ItemExecutionFlowStatus GetExecutionFlowStatus(List<ItemExecutionFlowInfo> list)
        {
            ItemExecutionFlowStatus result = ItemExecutionFlowStatus.New;

            int statusNewCount = 0;
            int statusProcessedCount = 0;
            int statusItemFailedCount = 0;
            int statusItemProcessingCount = 0;
            int statusItemSuspendedCount = 0;

            foreach (ItemExecutionFlowInfo item in list)
            {
                switch (item.Status)
                {
                    case ItemExecutionFlowStatus.New:
                        statusNewCount++;
                        break;
                    case ItemExecutionFlowStatus.Processing:
                        statusItemProcessingCount++;
                        break;
                    case ItemExecutionFlowStatus.Failed:
                        statusItemFailedCount++;
                        break;
                    case ItemExecutionFlowStatus.Suspended:
                        statusItemSuspendedCount++;
                        break;
                    default:
                        statusProcessedCount++;
                        break;
                }
            }

            if (statusNewCount == list.Count)
                result = ItemExecutionFlowStatus.New;
            else if ((statusItemSuspendedCount + statusProcessedCount) == list.Count)
            {
                result = statusItemSuspendedCount > 0 ? ItemExecutionFlowStatus.Suspended : ItemExecutionFlowStatus.Processed;
            }
            else if (statusItemFailedCount > 0)
                result = ItemExecutionFlowStatus.Failed;
            else
                result = ItemExecutionFlowStatus.Processing;

            return result;
        }
        public Vanrise.Entities.IDataRetrievalResult<QueueItemHeaderDetails> GetQueueItemsHeader(Vanrise.Entities.DataRetrievalInput<List<long>> input)
        {
            var data = _itemDataManager.GetQueueItemsHeader(input);
            Vanrise.Entities.BigResult<QueueItemHeaderDetails> returnedResult = new Vanrise.Entities.BigResult<QueueItemHeaderDetails>();
            returnedResult.ResultKey = data.ResultKey;
            returnedResult.TotalCount = data.TotalCount;
            returnedResult.Data = data.Data.MapRecords(QueueItemHeaderDetailsMapper);

            ResultProcessingHandler<QueueItemHeaderDetails> handler = new ResultProcessingHandler<QueueItemHeaderDetails>()
            {
                ExportExcelHandler = new QueueingExcelExportHandler()
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, returnedResult, handler);
        }
       
              
        #endregion

        #region Private Classes
        private QueueItemStatus GetSubStatus(List<ItemExecutionFlowInfo> subList)
        {
            return QueueItemStatus.Processed;
        }

        private class QueueingExcelExportHandler : ExcelExportHandler<QueueItemHeaderDetails>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<QueueItemHeaderDetails> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Queue Items",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Item Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Stage Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Retry Count" });
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
                            row.Cells.Add(new ExportExcelCell { Value = record.StageName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Description });
                            row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(record.Entity.Status) });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.RetryCount });
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

        #region Mappers
        QueueItemHeaderDetails QueueItemHeaderDetailsMapper(QueueItemHeader queueItemHeader)
        {
            QueueInstanceManager queueManager = new QueueInstanceManager();
            var instance = queueManager.GetQueueInstanceById(queueItemHeader.QueueId);
            return new QueueItemHeaderDetails
            {
                Entity = queueItemHeader,
                StageName = instance != null ? instance.StageName : "",

            };
        }

        #endregion
    }
}
