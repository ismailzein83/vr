using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class BasicSalePriceListTemplateSettings : SalePriceListTemplateSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9da89204-193a-4fe1-8416-98f1620848b0"); }
        }

        public long TemplateFileId { get; set; }

        public List<MappedTable> MappedTables { get; set; }

        public List<MappedCell> MappedCells { get; set; }

        public string DateTimeFormat { get; set; }

        public override byte[] Execute(ISalePriceListTemplateSettingsContext context)
        {
            var fileManager = new Vanrise.Common.Business.VRFileManager();
            Vanrise.Entities.VRFile file = fileManager.GetFile(TemplateFileId);
            if (file == null)
                throw new NullReferenceException(String.Format("File '{0}' was not found", TemplateFileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("File '{0}' is empty", TemplateFileId));

            IEnumerable<SalePLZoneNotification> orderedListofZones = context.Zones.OrderBy(x => x.ZoneName);

            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);

            Vanrise.Common.Utilities.ActivateAspose();

            SetWorkbookData(orderedListofZones, workbook, context.CustomerId, context.PricelistDate, context.PricelistType, context.PricelistCurrencyId);

            byte[] array;

            using (MemoryStream ms = new MemoryStream())
            {
                switch (context.PriceListExtensionFormat)
                {
                    case PriceListExtensionFormat.XLS:
                        workbook.Save(ms, SaveFormat.Excel97To2003);

                        break;
                    case PriceListExtensionFormat.XLSX:
                        workbook.Save(ms, SaveFormat.Xlsx);

                        break;
                }
                array = ms.ToArray();
            }
            return array;
        }

        public override void OnBeforeSave(IPriceListTemplateOnBeforeSaveContext context)
        {
            VRFileManager fileManager = new VRFileManager();

            if (context.TemplateId.HasValue)
            {
                var fileSettings = new VRFileSettings { ExtendedSettings = new BasicSalePricelistTemplateFileSettings { SalePricelistTemplateId = context.TemplateId } };
                fileManager.SetFileUsedAndUpdateSettings(TemplateFileId, fileSettings);
            }
            else fileManager.SetFileUsed(TemplateFileId);
        }

        public override void OnAfterSave(IPriceListTemplateOnAfterSaveContext context)
        {
            if (context.SaveOperationType == SaveOperationType.Update)
                return;
            VRFileManager fileManager = new VRFileManager();
            var fileSettings = new VRFileSettings { ExtendedSettings = new BasicSalePricelistTemplateFileSettings { SalePricelistTemplateId = context.TemplateId } };
            fileManager.SetFileUsedAndUpdateSettings(TemplateFileId, fileSettings);
        }

        #region Private Methods

        private void SetWorkbookData(IEnumerable<SalePLZoneNotification> zoneNotificationList, Workbook workbook, int customerId, DateTime pricelistDate, SalePriceListType pricelistType, int pricelistCurrencyId)
        {
            if (MappedCells != null)
                SetWorkbookMappedCells(workbook, customerId, pricelistDate, pricelistType, pricelistCurrencyId);

            if (MappedTables != null)
                SetWorkbookMappedTables(zoneNotificationList, workbook, customerId);
        }
        private void SetWorkbookMappedCells(Workbook workbook, int customerId, DateTime pricelistDate, SalePriceListType pricelistType, int pricelistCurrencyId)
        {
            IMappedCellContext mappedCellContext = new MappedCellContext
            {
                CustomerId = customerId,
                PricelistDate = pricelistDate,
                PricelistType = pricelistType,
                PricelistCurrencyId = pricelistCurrencyId,
            };

            foreach (MappedCell mappedCell in MappedCells)
            {
                Worksheet worksheet = workbook.Worksheets[mappedCell.SheetIndex];
                object cellValue = getValueOfMappedCell(mappedCell, mappedCellContext);
                if (cellValue != null)
                    worksheet.Cells[mappedCell.RowIndex, mappedCell.CellIndex].PutValue(cellValue);
            }
        }

        private void SetWorkbookMappedTables(IEnumerable<SalePLZoneNotification> zoneNotificationList, Workbook workbook, int customerId)
        {
            foreach (MappedTable mappedSheet in MappedTables)
            {
                int firstRow = mappedSheet.FirstRowIndex;
                IEnumerable<SalePricelistTemplateTableRow> sheet = mappedSheet.BuildSheet(zoneNotificationList, DateTimeFormat, customerId);
                Worksheet worksheet = workbook.Worksheets[mappedSheet.SheetIndex];
                SetSheetData(worksheet, sheet, firstRow);
            }
        }

        private void SetSheetData(Worksheet worksheet, IEnumerable<SalePricelistTemplateTableRow> sheet, int currentRow)
        {
            int rowCount = sheet.Count();
            if (rowCount > 1)
                worksheet.Cells.InsertRows(currentRow + 1, rowCount - 1);

            foreach (SalePricelistTemplateTableRow row in sheet)
            {
                SetRowData(worksheet, row, currentRow);
                currentRow++;
                //worksheet.Cells.InsertRow(++currentRow);
            }
        }
        private void SetRowData(Worksheet worksheet, SalePricelistTemplateTableRow row, int currentRow)
        {
            if (row != null)
            {
                foreach (SalePriceListTemplateTableCell cell in row.RowCells)
                {
                    worksheet.Cells[currentRow, cell.ColumnIndex].PutValue(cell.Value);
                }
            }
            else
                throw new ArgumentNullException("row");
        }


        private object getValueOfMappedCell(MappedCell mappedCell, IMappedCellContext mappedCellContext)
        {
            mappedCell.Execute(mappedCellContext);

            if (mappedCellContext.Value is DateTime)
                mappedCellContext.Value = ((DateTime)mappedCellContext.Value).ToString(DateTimeFormat);

            return mappedCellContext.Value;
        }
        #endregion
    }
}