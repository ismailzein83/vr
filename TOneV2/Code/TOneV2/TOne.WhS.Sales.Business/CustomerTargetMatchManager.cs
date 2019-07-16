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
            var excelFile = new Vanrise.Common.Excel.VRExcelFile();
            var excelSheet = excelFile.CreateSheet();

            var excelTable = excelSheet.CreateTable(1, 0);

            var headerRow = excelTable.CreateHeaderRow();
            headerRow.CreateStyle();

            var headerCellStyle = new VRExcelTableRowCellStyle { FontColor = "Red", FontSize = 11 };
            CreateCell("Zone", headerRow, headerCellStyle);
            CreateCell("Rate", headerRow, headerCellStyle);
            CreateCell("Volume", headerRow, headerCellStyle);

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
    }

}
