using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class CodeCompareManager
    {

        private Dictionary<string, List<CodeSupplierZoneMatch>> StructureCodeSupplierZoneMatchDictionary(IEnumerable<CodeSupplierZoneMatch> codeZoneMatches)
        {
            var codeZoneMatchByCode = new Dictionary<string, List<CodeSupplierZoneMatch>>();
            foreach (var codeZoneMatch in codeZoneMatches)
            {
                List<CodeSupplierZoneMatch> codes = codeZoneMatchByCode.GetOrCreateItem(codeZoneMatch.Code);
                codes.Add(codeZoneMatch);
            }
            return codeZoneMatchByCode;
        }

        private Dictionary<string, CodeSaleZoneMatch> StructureCodeSaleZoneMatchDictionary(IEnumerable<CodeSaleZoneMatch> codeZoneMatches)
        {
            var codeZoneMatchByCode = new Dictionary<string, CodeSaleZoneMatch>();

            foreach (var codeZoneMatch in codeZoneMatches)
            {
                CodeSaleZoneMatch codeSaleZoneMatch;
                if (!codeZoneMatchByCode.TryGetValue(codeZoneMatch.Code, out codeSaleZoneMatch))
                {
                    codeZoneMatchByCode.Add(codeZoneMatch.Code, codeZoneMatch);
                }
            }
            return codeZoneMatchByCode;
        }
        public Vanrise.Entities.IDataRetrievalResult<CodeCompareItemDetail> GetFilteredCodeCompare(Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CodeCompareRequestHandler());
        }
        public IEnumerable<CodeCompareItem> GetFilteredData(CodeCompareQuery query)
        {
            var codeCompareManager = new CodeCompareManager();
            var codeCompareItems = new List<CodeCompareItem>();
            var codeZoneMatchManager = new CodeZoneMatchManager();
            var saleZoneManager = new SaleZoneManager();
            var supplierZoneManager = new SupplierZoneManager();

            IEnumerable<CodeSupplierZoneMatch> codeMatchBySupplier = codeZoneMatchManager.GetSupplierZoneMatchBysupplierIdsAndSellingNumberPanId(query.sellingNumberPlanId, query.supplierIds, query.codeStartWith);
            IEnumerable<CodeSaleZoneMatch> saleCodeMatch = codeZoneMatchManager.GetSaleCodeMatchBySellingNumberPlanId(query.sellingNumberPlanId, query.codeStartWith);

            if (codeMatchBySupplier == null || !codeMatchBySupplier.Any())
                return null;

            Dictionary<string, List<CodeSupplierZoneMatch>> codeSupplierZoneMatchDictionary = codeCompareManager.StructureCodeSupplierZoneMatchDictionary(codeMatchBySupplier);
            Dictionary<string, CodeSaleZoneMatch> codeSaleZoneMatchDictionary = codeCompareManager.StructureCodeSaleZoneMatchDictionary(saleCodeMatch);

            HashSet<string> distinctCode = codeSupplierZoneMatchDictionary.Keys.ToHashSet<string>();
            foreach (var code in codeSaleZoneMatchDictionary.Keys)
            {
                distinctCode.Add(code);
            }

            foreach (var code in distinctCode)
            {
                CodeSaleZoneMatch codeSaleZoneMatch;
                List<CodeSupplierZoneMatch> codeSupplierZoneMatches;
                var codeCompareItem = new CodeCompareItem { Code = code };

                if (codeSaleZoneMatchDictionary.TryGetValue(code, out codeSaleZoneMatch))
                {
                    codeCompareItem.SaleZone = saleZoneManager.GetSaleZoneName(codeSaleZoneMatch.SaleZoneId);
                    codeCompareItem.SaleCode = codeSaleZoneMatch.CodeMatch;

                    codeCompareItem.SaleCodeIndicator = codeSaleZoneMatch.CodeMatch == code
                        ? CodeCompareIndicator.None
                        : CodeCompareIndicator.Highlight;
                }
                if (codeSupplierZoneMatchDictionary.TryGetValue(code, out codeSupplierZoneMatches))
                {
                    codeCompareItem.SupplierItems = new List<CodeCompareSupplierItem>();
                    foreach (var codeSupplierZoneMatch in codeSupplierZoneMatches)
                    {
                        CodeCompareSupplierItem codeCompareSupplierItem = new CodeCompareSupplierItem
                        {
                            SupplierId = codeSupplierZoneMatch.SupplierId,
                            SupplierZone = supplierZoneManager.GetSupplierZoneName(codeSupplierZoneMatch.SupplierZoneId),
                            SupplierCode = codeSupplierZoneMatch.CodeMatch
                        };
                        if (!string.IsNullOrEmpty(codeSupplierZoneMatch.CodeMatch) && codeSupplierZoneMatch.CodeMatch == code)
                        {
                            codeCompareSupplierItem.SupplierCodeIndicator = CodeCompareIndicator.None;
                            codeCompareItem.OccurrenceInSuppliers++;
                        }
                        else
                            codeCompareSupplierItem.SupplierCodeIndicator = CodeCompareIndicator.Highlight;

                        codeCompareItem.SupplierItems.Add(codeCompareSupplierItem);
                    }
                }
                codeCompareItem.AbsenceInSuppliers = query.supplierIds.Count() - codeCompareItem.OccurrenceInSuppliers;
                if (codeCompareItem.OccurrenceInSuppliers >= query.threshold && codeCompareItem.SaleCode != code)
                {
                    codeCompareItem.OccurrenceInSuppliersIndicator = CodeCompareIndicator.Highlight;
                    codeCompareItem.AbsenceInSuppliersIndicator = CodeCompareIndicator.None;
                    codeCompareItem.Action = CodeCompareAction.New;
                }
                else if (codeCompareItem.AbsenceInSuppliers >= query.threshold && codeCompareItem.SaleCode == code)
                {
                    codeCompareItem.OccurrenceInSuppliersIndicator = CodeCompareIndicator.None;
                    codeCompareItem.AbsenceInSuppliersIndicator = CodeCompareIndicator.Highlight;
                    codeCompareItem.Action = CodeCompareAction.Delete;
                }
                codeCompareItems.Add(codeCompareItem);

            }
            return codeCompareItems;
        }
        public byte[] ExportCodeCompareTemplate(byte[] buffer, CodeCompareQuery query)
        {
            IEnumerable<CodeCompareItem> codeCompareItems = new CodeCompareManager().GetFilteredData(query);
            if (codeCompareItems == null)
            {
                return null;
            }

            Vanrise.Common.Utilities.ActivateAspose();
            MemoryStream stream = new MemoryStream(buffer);
            Workbook workbook = new Workbook(stream);
            Worksheet worksheet = workbook.Worksheets[0];
            var i = 0;
            var cellCounter = 1;
            var countOfItems = codeCompareItems.Count();

            worksheet.Cells.DeleteRows(1, 3);

            for (i = 0; i < countOfItems; i++)
            {
                var codeCompareItem = codeCompareItems.ElementAt(i);
                if (codeCompareItem.Action != null)
                {
                    if (codeCompareItem.SaleCode == null)
                    {
                        worksheet.Cells[cellCounter, 0].PutValue(codeCompareItem.SupplierItems[0].SupplierZone);
                    }
                    else
                    {
                        worksheet.Cells[cellCounter, 0].PutValue(codeCompareItem.SaleZone);
                    }
                    worksheet.Cells[cellCounter, 1].PutValue(codeCompareItem.Code);
                    if (codeCompareItem.Action == CodeCompareAction.New)
                    {
                        worksheet.Cells[cellCounter, 2].PutValue("N");
                    }
                    else
                    {
                        if (codeCompareItem.Action == CodeCompareAction.Delete)
                        {
                            worksheet.Cells[cellCounter, 2].PutValue("D");
                        }
                    }
                    cellCounter++;
                }

            }
            byte[] array;

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Excel97To2003);
                array = ms.ToArray();
            }
            return array;
        }
        private class CodeCompareRequestHandler : BigDataRequestHandler<CodeCompareQuery, CodeCompareItem, CodeCompareItemDetail>
        {

            public override CodeCompareItemDetail EntityDetailMapper(CodeCompareItem entity)
            {
                return new CodeCompareItemDetail()
                {
                    Code = entity.Code,
                    SaleZone = entity.SaleZone,
                    SaleCode = entity.SaleCode,
                    SaleCodeIndicator = entity.SaleCodeIndicator,
                    SupplierItems = entity.SupplierItems,
                    OccurrenceInSuppliers = entity.OccurrenceInSuppliers,
                    OccurrenceInSuppliersIndicator = entity.OccurrenceInSuppliersIndicator,
                    AbsenceInSuppliers = entity.AbsenceInSuppliers,
                    AbsenceInSuppliersIndicator = entity.AbsenceInSuppliersIndicator,
                    Action = entity.Action
                };
            }

            public override IEnumerable<CodeCompareItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input)
            {
                return new CodeCompareManager().GetFilteredData(input.Query);
            }

            protected override ResultProcessingHandler<CodeCompareItemDetail> GetResultProcessingHandler(DataRetrievalInput<CodeCompareQuery> input, BigResult<CodeCompareItemDetail> bigResult)
            {
                return new ResultProcessingHandler<CodeCompareItemDetail>
                {
                    ExportExcelHandler = new CodeCompareItemDetailExportExcelHandler()
                };
            }
        }
        private class CodeCompareItemDetailExportExcelHandler : ExcelExportHandler<CodeCompareItemDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CodeCompareItemDetail> context)
            {
                Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input = context.Input as Vanrise.Entities.DataRetrievalInput<CodeCompareQuery>;
                IEnumerable<long> selectedSupplierIds = input.Query.supplierIds;

                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Code Compare",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Sale Zone Name" });
                foreach (var supplierId in selectedSupplierIds)
                {
                    string supplierName = new CarrierAccountManager().GetCarrierAccountName((int)supplierId);
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = supplierName, Width = 30 });
                }
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Sale Zone Code" });
                foreach (var supplierId in selectedSupplierIds)
                {
                    string supplierName = new CarrierAccountManager().GetCarrierAccountName((int)supplierId);
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = supplierName, Width = 30 });
                }
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Occurrence Code In Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Absence Code In Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Action" });
                sheet.Rows = new List<ExportExcelRow>();

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    int supplierCount = context.BigResult.Data.First().AbsenceInSuppliers + context.BigResult.Data.First().OccurrenceInSuppliers;

                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Code });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SaleZone });

                            foreach (var supplierId in selectedSupplierIds)
                            {
                                var matchedSupplierItem = new CodeCompareSupplierItem();
                                if (record.SupplierItems != null)
                                {
                                    matchedSupplierItem = record.SupplierItems.FirstOrDefault(sup => sup.SupplierId == supplierId);
                                }
                                if (matchedSupplierItem != null)
                                    row.Cells.Add(new ExportExcelCell() { Value = matchedSupplierItem.SupplierZone });
                                else
                                    row.Cells.Add(new ExportExcelCell() { Value = "" });
                            }
                            row.Cells.Add(new ExportExcelCell() { Value = record.SaleCode });

                            foreach (var supplierId in selectedSupplierIds)
                            {
                                var matchedSupplierItem = new CodeCompareSupplierItem();
                                if (record.SupplierItems != null)
                                {
                                    matchedSupplierItem = record.SupplierItems.FirstOrDefault(sup => sup.SupplierId == supplierId);
                                }
                                if (matchedSupplierItem != null)
                                    row.Cells.Add(new ExportExcelCell() { Value = matchedSupplierItem.SupplierCode });
                                else
                                    row.Cells.Add(new ExportExcelCell() { Value = "" });
                            }
                            
                            row.Cells.Add(new ExportExcelCell() { Value = record.OccurrenceInSuppliers });
                            row.Cells.Add(new ExportExcelCell() { Value = record.AbsenceInSuppliers });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Action });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }
    }
}
