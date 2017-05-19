using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

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

            SetWorkbookData(orderedListofZones, workbook);

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

        #region Private Methods

        private void SetWorkbookData(IEnumerable<SalePLZoneNotification> zoneNotificationList, Workbook workbook)
        {
            if (MappedTables != null)
            {
                foreach (MappedTable mappedSheet in MappedTables)
                {
                    IEnumerable<SalePriceListTemplateTableCell> sheets = mappedSheet.FillSheet(zoneNotificationList, DateTimeFormat);
                    Worksheet worksheet = workbook.Worksheets[mappedSheet.SheetIndex];
                    SetCellData(worksheet, sheets);
                }
            }
        }
        private void SetCellData(Worksheet worksheet, IEnumerable<SalePriceListTemplateTableCell> sheets)
        {
            if (sheets != null)
            {
                foreach (SalePriceListTemplateTableCell sheet in sheets)
                {
                    worksheet.Cells[sheet.RowIndex, sheet.ColumnIndex].PutValue(sheet.Value);
                }
            }
        }

        #endregion
    }
}
