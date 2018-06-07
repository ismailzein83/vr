using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCodePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CodePreviewDetail> GetFilteredCodePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierCodePreviewRequestHandler());
        }


        #region Private Classes

        private class SupplierCodePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, CodePreview, CodePreviewDetail>
        {
            public override CodePreviewDetail EntityDetailMapper(CodePreview entity)
            {
                SupplierCodePreviewManager manager = new SupplierCodePreviewManager();
                return manager.CodePreviewDetailMapper(entity);
            }

            public override IEnumerable<CodePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierCodePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCodePreviewDataManager>();
                return dataManager.GetFilteredCodePreview(input.Query);
            }
            protected override ResultProcessingHandler<CodePreviewDetail> GetResultProcessingHandler(DataRetrievalInput<SPLPreviewQuery> input, BigResult<CodePreviewDetail> bigResult)
            {
                return new ResultProcessingHandler<CodePreviewDetail>
                {
                    ExportExcelHandler = new CodePreviewDetailExportExcelHandler()
                };
            }
            private class CodePreviewDetailExportExcelHandler : ExcelExportHandler<CodePreviewDetail>
            {
                public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CodePreviewDetail> context)
                {

                    var sheet = new ExportExcelSheet()
                    {
                        SheetName = "Sale Codes",
                        Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                    };

                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code", Width = 30 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone Name", Width = 30 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code Change Type", Width = 30 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Moved From", Width = 30 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Moved To", Width = 30 });

                    sheet.Rows = new List<ExportExcelRow>();
                    if (context.BigResult != null && context.BigResult.Data != null)
                    {
                        CountryManager countryManager = new CountryManager();
                        foreach (var record in context.BigResult.Data)
                        {
                            if (record.Entity != null)
                            {
                                var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                                row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Code });
                                row.Cells.Add(new ExportExcelCell() { Value = record.Entity.ZoneName });
                                row.Cells.Add(new ExportExcelCell() { Value = record.ChangeTypeDecription });
                                row.Cells.Add(new ExportExcelCell() { Value = record.Entity.BED });
                                row.Cells.Add(new ExportExcelCell() { Value = record.Entity.EED });
                                string movedFrom = null;
                                string movedTo = null;

                                if (record.Entity.ChangeType == CodeChangeType.Moved)
                                {
                                    movedFrom = record.Entity.RecentZoneName;
                                    movedTo = record.Entity.ZoneName;
                                }

                                row.Cells.Add(new ExportExcelCell() { Value = movedFrom });
                                row.Cells.Add(new ExportExcelCell() { Value = movedTo });

                                sheet.Rows.Add(row);
                            }
                        }
                    }

                    context.MainSheet = sheet;
                }
            }
        }

        #endregion


        #region Private Mappers
        private CodePreviewDetail CodePreviewDetailMapper(CodePreview codePreview)
        {
            CodePreviewDetail codePreviewDetail = new CodePreviewDetail();
            codePreviewDetail.Entity = codePreview;
            codePreviewDetail.ChangeTypeDecription = Utilities.GetEnumAttribute<CodeChangeType, DescriptionAttribute>(codePreview.ChangeType).Description;
            return codePreviewDetail;
        }

        #endregion
    }
}
