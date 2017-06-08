using System;
using System.Collections.Generic;
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

        public Dictionary<string, List<CodeSupplierZoneMatch>> StructureCodeSupplierZoneMatchDictionary(List<CodeSupplierZoneMatch> codeZoneMatches)
        {
            var codeZoneMatchByCode = new Dictionary<string, List<CodeSupplierZoneMatch>>();
            foreach (var codeZoneMatch in codeZoneMatches)
            {
                List<CodeSupplierZoneMatch> codes = codeZoneMatchByCode.GetOrCreateItem(codeZoneMatch.Code);
                codes.Add(codeZoneMatch);
            }
            return codeZoneMatchByCode;
        }

        public Dictionary<string, CodeSaleZoneMatch> StructureCodeSaleZoneMatchDictionary(List<CodeSaleZoneMatch> codeZoneMatches)
        {
            var codeZoneMatchByCode = new Dictionary<string, CodeSaleZoneMatch>();

            foreach (var codeZoneMatch in codeZoneMatches)
            {CodeSaleZoneMatch codeSaleZoneMatch;
                if(!codeZoneMatchByCode.TryGetValue(codeZoneMatch.Code,out codeSaleZoneMatch))
                {
                codeZoneMatchByCode.Add(codeZoneMatch.Code,codeZoneMatch);
                }
            }
            return codeZoneMatchByCode;
        }
        public Vanrise.Entities.IDataRetrievalResult<CodeCompareItemDetail> GetFilteredCodeCompare(Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CodeCompareRequestHandler());
        }

        private class CodeCompareRequestHandler : BigDataRequestHandler<CodeCompareQuery, CodeCompareItem, CodeCompareItemDetail>
        {

            public override CodeCompareItemDetail EntityDetailMapper(CodeCompareItem entity)
            {
                return new CodeCompareItemDetail()
                {
                      Code =entity.Code,
                      SaleZone=entity.SaleZone,
                      SaleCode=entity.SaleCode,
                      SaleCodeIndicator=entity.SaleCodeIndicator,
                      SupplierItems=entity.SupplierItems,
                      OccurrenceInSuppliers=entity.OccurrenceInSuppliers,
                      OccurrenceInSuppliersIndicator=entity.OccurrenceInSuppliersIndicator,
                      AbsenceInSuppliers=entity.AbsenceInSuppliers,
                      AbsenceInSuppliersIndicator=entity.AbsenceInSuppliersIndicator,
                      Action = entity.Action
                };
            }

            public override IEnumerable< CodeCompareItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CodeCompareQuery> input)
            {
                CodeCompareManager codeCompareManager=new CodeCompareManager();
                List<CodeCompareItem> CodeCompareItems = new List<CodeCompareItem>();
                CodeZoneMatchManager codeZoneMatchManager = new CodeZoneMatchManager();
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
                Dictionary<string, List<CodeSupplierZoneMatch>> codeSupplierZoneMatchDictionary = codeCompareManager.StructureCodeSupplierZoneMatchDictionary((List<CodeSupplierZoneMatch>)codeZoneMatchManager.GetSupplierZoneMatchBysupplierIds(input.Query.supplierIds, input.Query.codeStartWith));
                Dictionary<string, CodeSaleZoneMatch> codeSaleZoneMatchDictionary = codeCompareManager.StructureCodeSaleZoneMatchDictionary((List<CodeSaleZoneMatch>)codeZoneMatchManager.GetSaleZoneMatchBySellingNumberPlanId(input.Query.sellingNumberPlanId, input.Query.codeStartWith));

                HashSet<string> distinctCode = new HashSet<string>();
                distinctCode = codeSupplierZoneMatchDictionary.Keys.ToHashSet<string>();
                foreach (var code in codeSaleZoneMatchDictionary.Keys)
                {
                    distinctCode.Add(code);
                }
                foreach (var code in distinctCode)
                {

                    CodeSaleZoneMatch codeSaleZoneMatch;
                    List<CodeSupplierZoneMatch> codeSupplierZoneMatches;
                    CodeCompareItem codeCompareItem = new CodeCompareItem();
                    codeCompareItem.Code = code;
                    if (codeSaleZoneMatchDictionary.TryGetValue(code, out codeSaleZoneMatch))
                    {
                        codeCompareItem.SaleZone = saleZoneManager.GetSaleZoneName(codeSaleZoneMatch.SaleZoneId);
                        codeCompareItem.SaleCode = codeSaleZoneMatch.CodeMatch;
                        if (codeSaleZoneMatch.CodeMatch == code)
                            codeCompareItem.SaleCodeIndicator = CodeCompareIndicator.None;
                        else
                            codeCompareItem.SaleCodeIndicator = CodeCompareIndicator.Highlight;
                    }
                    if (codeSupplierZoneMatchDictionary.TryGetValue(code, out codeSupplierZoneMatches))
                    {
                        codeCompareItem.SupplierItems = new List<CodeCompareSupplierItem>();
                        foreach (var codeSupplierZoneMatch in codeSupplierZoneMatches)
                        {
                            CodeCompareSupplierItem codeCompareSupplierItem = new CodeCompareSupplierItem();
                            codeCompareSupplierItem.SupplierZone = supplierZoneManager.GetSupplierZoneName(codeSupplierZoneMatch.SupplierZoneId);
                            codeCompareSupplierItem.SupplierCode = codeSupplierZoneMatch.CodeMatch;
                            if (codeSupplierZoneMatch.CodeMatch == code)
                            {
                                codeCompareSupplierItem.SupplierCodeIndicator = CodeCompareIndicator.None;
                                codeCompareItem.OccurrenceInSuppliers = codeCompareItem.OccurrenceInSuppliers + 1;
                            }

                            else
                            {
                                codeCompareSupplierItem.SupplierCodeIndicator = CodeCompareIndicator.Highlight;
                                codeCompareItem.AbsenceInSuppliers = codeCompareItem.AbsenceInSuppliers + 1;
                            }
                            codeCompareItem.SupplierItems.Add(codeCompareSupplierItem);

                        }
                    }
                    if (codeCompareItem.OccurrenceInSuppliers >= input.Query.threshold && codeCompareItem.SaleCode != code)
                    {
                        codeCompareItem.OccurrenceInSuppliersIndicator = CodeCompareIndicator.Highlight;
                        codeCompareItem.AbsenceInSuppliersIndicator = CodeCompareIndicator.None;
                        codeCompareItem.Action = CodeCompareAction.New;
                    }
                    else
                    {
                        if (codeCompareItem.AbsenceInSuppliers >= input.Query.threshold && codeCompareItem.SaleCode == code)
                        {
                            codeCompareItem.OccurrenceInSuppliersIndicator = CodeCompareIndicator.None;
                            codeCompareItem.AbsenceInSuppliersIndicator = CodeCompareIndicator.Highlight;
                            codeCompareItem.Action = CodeCompareAction.Delete;
                        }


                    }
                    CodeCompareItems.Add(codeCompareItem);

                }
                return CodeCompareItems;
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
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Code Compare",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Sale Zone Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Sale Zone Code"});
                for (var i = 0; i < context.BigResult.Data.ElementAt(0).SupplierItems.Count(); i++)
                {var j=i+1;
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Supplier " + j+" Zone Name", Width = 30});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Supplier " + j + " Zone Code",Width = 30 });
                }
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Occurrence Code In Suppliers" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Absence Code In Suppliers" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Action" });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Code });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SaleZone });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SaleCode });
                            for (var f = 0; f < record.SupplierItems.Count(); f++)
                            {
                                row.Cells.Add(new ExportExcelCell() { Value = record.SupplierItems[f].SupplierZone });
                                row.Cells.Add(new ExportExcelCell() { Value = record.SupplierItems[f].SupplierCode });
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
