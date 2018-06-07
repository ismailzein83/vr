using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierZonePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<ZoneRatePreviewDetail> GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierZoneRatePreviewRequestHandler());
        }


        #region Private Classes

        private class SupplierZoneRatePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, ZoneRatePreviewDetail, ZoneRatePreviewDetail>
        {
            public override ZoneRatePreviewDetail EntityDetailMapper(ZoneRatePreviewDetail entity)
            {
                SupplierZonePreviewManager manager = new SupplierZonePreviewManager();
                return manager.ZoneRatePreviewDetailMapper(entity);
            }

            public override IEnumerable<ZoneRatePreviewDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();
                return dataManager.GetFilteredZonePreview(input.Query);
            }
            protected override ResultProcessingHandler<ZoneRatePreviewDetail> GetResultProcessingHandler(DataRetrievalInput<SPLPreviewQuery> input, BigResult<ZoneRatePreviewDetail> bigResult)
            {
                return new ResultProcessingHandler<ZoneRatePreviewDetail>
                {
                    ExportExcelHandler = new ZonePreviewExportExcelHandler()
                };
            }
        }
      
        private class ZonePreviewExportExcelHandler : ExcelExportHandler<ZoneRatePreviewDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ZoneRatePreviewDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Sale Zones",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone Change Type", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Current Rate", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Imported Rate", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Current Rate BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Imported Rate BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Current Rate EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Imported Services", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "New Codes", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Deleted Codes", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Codes Moved From", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Codes Moved To", Width = 30 });
                
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    string servicesSymbol = null;
                    CountryManager countryManager = new CountryManager();
                    foreach (var record in context.BigResult.Data)
                    {
                        ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
                        if (record.ImportedServiceIds != null)
                        {
                            var services = zoneServiceConfigManager.GetZoneServicesNames(record.ImportedServiceIds);
                             servicesSymbol = string.Join(",", services);
                        }
                        if (record != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell() { Value = Utilities.GetEnumAttribute<ZoneChangeType, DescriptionAttribute>(record.ChangeTypeZone).Description});
                            row.Cells.Add(new ExportExcelCell() { Value = record.ZoneBED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ZoneEED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SystemRate });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ImportedRate });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SystemRateBED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ImportedRateBED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SystemRateEED });
                            row.Cells.Add(new ExportExcelCell() { Value = servicesSymbol });
                            row.Cells.Add(new ExportExcelCell() { Value = record.NewCodes });
                            row.Cells.Add(new ExportExcelCell() { Value = record.DeletedCodes });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CodesMovedFrom });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CodesMovedTo });
                           

                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        #endregion


        #region Private Mappers

        private ZoneRatePreviewDetail ZoneRatePreviewDetailMapper(ZoneRatePreviewDetail zoneRatePreviewDetail)
        {
            return zoneRatePreviewDetail;
        }

        #endregion


    }
}
