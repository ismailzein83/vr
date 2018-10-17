using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ExcludedItemsManager
    {
        //#region Public Methods
        //public IDataRetrievalResult<ExcludedItemDetail> GetFilteredExcludedItems(DataRetrievalInput<ExcludedItemsQuery> input)
        //{
        //    return BigDataManager.Instance.RetrieveData(input, new ExcludedItemsRequestHandler());
        //}

        //public void BulkInsertExcludedItems(List<ExcludedItem> excludedItems)
        //{
        //    var dataManager = SalesDataManagerFactory.GetDataManager<IExcludedItemsDataManager>();
        //    dataManager.BulkInsertExcludedItems(excludedItems);
        //}

        //#endregion

        //#region Private Methods
        //private class ExcludedItemsRequestHandler : BigDataRequestHandler<ExcludedItemsQuery, ExcludedItem, ExcludedItemDetail>
        //{
        //    public override ExcludedItemDetail EntityDetailMapper(ExcludedItem entity)
        //    {
        //        return ExcludedItemDetailMapper(entity);
        //    }

        //    public override IEnumerable<ExcludedItem> RetrieveAllData(DataRetrievalInput<ExcludedItemsQuery> input)
        //    {
        //        IExcludedItemsDataManager receivedPricelistDataManager = SalesDataManagerFactory.GetDataManager<IExcludedItemsDataManager>();
        //        return receivedPricelistDataManager.GetAllExcludedItems(input.Query);
        //    }
        //    private class ExcludedItemExcelExportHandler : ExcelExportHandler<ExcludedItemDetail>
        //    {
        //        public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ExcludedItemDetail> context)
        //        {
        //            ExportExcelSheet sheet = new ExportExcelSheet
        //            {
        //                SheetName = "Excluded Items",
        //                Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
        //                AutoFitColumns = true
        //            };
        //            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Item ID" });
        //            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Item Type" });
        //            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Item Name" });
        //            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Reason" });
        //            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Parent ID" });
        //            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Process Instance ID" });

        //            sheet.Rows = new List<ExportExcelRow>();

        //            if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Any())
        //            {
        //                foreach (var record in context.BigResult.Data)
        //                {
        //                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
        //                    row.Cells.Add(new ExportExcelCell { Value = record.ItemId });
        //                    row.Cells.Add(new ExportExcelCell { Value = record.ItemTypeDescription });
        //                    row.Cells.Add(new ExportExcelCell { Value = record.ItemName });
        //                    row.Cells.Add(new ExportExcelCell { Value = record.Reason});
        //                    row.Cells.Add(new ExportExcelCell { Value = record.ParentId });
        //                    row.Cells.Add(new ExportExcelCell { Value = record.ProcessInstanceId });

        //                    sheet.Rows.Add(row);

        //                }
        //            }
        //            context.MainSheet = sheet;
        //        }
        //    }
        //    protected override ResultProcessingHandler<ExcludedItemDetail> GetResultProcessingHandler(DataRetrievalInput<ExcludedItemsQuery> input, BigResult<ExcludedItemDetail> bigResult)
        //    {
        //        var resultProcessingHandler = new ResultProcessingHandler<ExcludedItemDetail>
        //        {
        //            ExportExcelHandler = new ExcludedItemExcelExportHandler()
        //        };
        //        return resultProcessingHandler;
        //    }
        //}
        //private static ExcludedItemDetail ExcludedItemDetailMapper(ExcludedItem excludedItem)
        //{
        //    return new ExcludedItemDetail()
        //    {
        //        ItemId = excludedItem.ItemId,
        //        ItemTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(excludedItem.ItemType),
        //        ItemName = excludedItem.ItemName,
        //        Reason = excludedItem.Reason,
        //        ParentId = excludedItem.ParentId,
        //        ProcessInstanceId = excludedItem.ProcessInstanceId,
        //        Entity = excludedItem
        //    };
        //}
        //#endregion
    }
}
