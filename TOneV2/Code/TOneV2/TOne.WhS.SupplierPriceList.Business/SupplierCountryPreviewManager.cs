using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCountryPreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CountryPreviewDetail> GetFilteredCountryPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierCountryPreviewRequestHandler());
        }


        private CountryPreviewDetail CountryPreviewDetailMapper(CountryPreview countryPreview)
        {
            CountryPreviewDetail countryPreviewDetail = new CountryPreviewDetail();
            countryPreviewDetail.Entity = countryPreview;

            CountryManager manager = new CountryManager();
            countryPreviewDetail.CountryName = manager.GetCountryName(countryPreview.CountryId);
            return countryPreviewDetail;
        }


        #region Private Classes

        private class SupplierCountryPreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, CountryPreview, CountryPreviewDetail>
        {
            public override CountryPreviewDetail EntityDetailMapper(CountryPreview entity)
            {
                SupplierCountryPreviewManager manager = new SupplierCountryPreviewManager();
                return manager.CountryPreviewDetailMapper(entity);
            }

            public override IEnumerable<CountryPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierCountryPreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCountryPreviewDataManager>();
                return dataManager.GetFilteredCountryPreview(input.Query);
            }
            protected override ResultProcessingHandler<CountryPreviewDetail> GetResultProcessingHandler(DataRetrievalInput<SPLPreviewQuery> input, BigResult<CountryPreviewDetail> bigResult)
            {
                return new ResultProcessingHandler<CountryPreviewDetail>
                {
                    ExportExcelHandler = new CountryPreviewExportExcelHandler()
                };
            }
        }

        private class CountryPreviewExportExcelHandler : ExcelExportHandler<CountryPreviewDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CountryPreviewDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Countries",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Country", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "New Zones", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Renamed Zones", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Deleted Zones", Width = 30 });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    CountryManager countryManager = new CountryManager();
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.CountryName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.NewZones });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.RenamedZones });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.DeletedZones });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        #endregion


     
    }
}

