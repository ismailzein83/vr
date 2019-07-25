using System;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Common.Excel;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
    public class CustomerTargetMatchManager
    {
        public byte[] GetCustomerTargetMatchTemplateFileContent(int customerId)
        {
            var saleZoneManager = new SaleZoneManager();
            List<SaleZone> customerSaleZones = saleZoneManager.GetCustomerSaleZones(customerId, DateTime.Now, false).ToList();
            return GetCustomerZoneSheetBytes(customerSaleZones.Where(zone => !zone.EED.HasValue));
        }
        private byte[] GetCustomerZoneSheetBytes(IEnumerable<SaleZone> saleZones)
        {
            var excelFile = new VRExcelFile();
            var excelSheet = excelFile.CreateSheet();

            var zoneColumnConfig = new VRExcelColumnConfig { ColumnIndex = 0, ColumnWidth = 30 };
            excelSheet.SetColumnConfig(zoneColumnConfig);

            var excelTable = excelSheet.CreateTable(0, 0);

            var headerRow = excelTable.CreateHeaderRow();
            headerRow.CreateStyle();

            CreateHeaderCell("Zone", headerRow);
            CreateHeaderCell("Rate", headerRow);
            CreateHeaderCell("Volume", headerRow);

            foreach (var zone in saleZones)
            {
                var row = excelTable.CreateDataRow();
                CreateCell(zone.Name, row, null);
                CreateCell(string.Empty, row, null);
                CreateCell(string.Empty, row, null);
            }
            return excelFile.GenerateExcelFile();
        }
        private void CreateCell(object cellValue, VRExcelTableRow row, VRExcelTableRowCellStyle cellStyle)
        {
            var cell = row.CreateCell();
            cell.SetValue(cellValue);
            if (cellStyle != null)
            {
                var style = cell.CreateStyle();
                style.FontColor = cellStyle.FontColor;
                style.FontSize = cellStyle.FontSize;
            }
        }
        private void CreateHeaderCell(object cellValue, VRExcelTableRow row)
        {
            var cell = row.CreateCell();
            var headerCellStyle = cell.CreateStyle();

            headerCellStyle.FontColor = "Black";
            headerCellStyle.FontSize = 12;
            headerCellStyle.IsBold = true;
            headerCellStyle.VerticalAlignment = VRExcelContainerVerticalAlignment.Bottom;
            headerCellStyle.HorizontalAlignment = VRExcelContainerHorizontalAlignment.Center;

            cell.SetValue(cellValue);
        }
    }

}
