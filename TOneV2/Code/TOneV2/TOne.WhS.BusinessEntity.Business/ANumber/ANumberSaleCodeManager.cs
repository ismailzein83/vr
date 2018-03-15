using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;
using System.IO;
using Aspose.Cells;
using System.Drawing;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ANumberSaleCodeManager
    {
        public ANumberSaleCode GetMatchSaleCode(string number, int customerId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
        public Vanrise.Entities.IDataRetrievalResult<ANumberSaleCodeDetail> GetFilteredANumberSaleCodes(Vanrise.Entities.DataRetrievalInput<ANumberSaleCodeQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ANumberSaleCodeRequestHandler());
        }
        public IEnumerable<ANumberSaleCode> GetEffectiveAfterBySellingNumberPlanId(int sellingNumberPlanId, DateTime effectiveOn)
        {
            IANumberSaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSaleCodeDataManager>();
            return dataManager.GetEffectiveAfterBySellingNumberPlanId(sellingNumberPlanId, effectiveOn);
        }

        public ANumberSaleCodesInsertResult AddANumberSaleCodes(ANumberSaleCodesInsertInput input)
        {
            IANumberSaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSaleCodeDataManager>();

            var matchedSaleCodes = GetEffectiveAfterBySellingNumberPlanId(input.SellingNumberPlanId, input.EffectiveOn);

            Dictionary<string, List<ANumberSaleCode>> structuredANumberSaleCodes = StructureANumberSaleCodesByCode(matchedSaleCodes);

            ANumberSaleCodesInsertResult result = new ANumberSaleCodesInsertResult();

            result.InvalidImportedSaleCodes = ValidateImportedSaleCodes(input.Codes, structuredANumberSaleCodes, input.ANumberGroupId, input.SellingNumberPlanId, input.EffectiveOn);
            
            if (result.InvalidImportedSaleCodes.Count() > 0)
            {
                result.ResultMessage = "Import ANumber Sale Codes failed.";
            }
            else 
            {
                long startingId = this.ReserveIDRange(input.Codes.Count());
                foreach (var code in input.Codes)
                {
                    var saleCodeInsertObject = new ANumberSaleCode
                    {
                        BED = input.EffectiveOn
                    };
                    List<ANumberSaleCodeToClose> codesToClose = new List<ANumberSaleCodeToClose>();
                    var exstingMatchedCodes = structuredANumberSaleCodes.GetRecord(code);
                    if (exstingMatchedCodes != null && exstingMatchedCodes.Count > 0)
                    {
                        foreach (var matchedCode in exstingMatchedCodes)
                        {

                            if (matchedCode.IsOverlappedWith(saleCodeInsertObject))
                            {
                                DateTime closeDate = Utilities.Max(input.EffectiveOn, matchedCode.BED);
                                codesToClose.Add(new ANumberSaleCodeToClose() { ANumberSaleCodeId = matchedCode.ANumberSaleCodeId, CloseDate = closeDate });
                            }

                        }
                    }
                    bool updateActionSucc = dataManager.Insert(codesToClose, startingId++, input.ANumberGroupId, input.SellingNumberPlanId, code, input.EffectiveOn);
                }
                result.ResultMessage = "Import ANumber Sale Codes Succeed.";
            }

            return result;

        }

        public ANumberSaleCode GetANumberSaleCode(long aNumberSaleCodeId)
        {
            IANumberSaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSaleCodeDataManager>();
            return dataManager.GetANumberSaleCode(aNumberSaleCodeId);
        }
        public UpdateOperationOutput<ANumberSaleCodeDetail> CloseANumberSaleCode(long aNumberSaleCodeId , DateTime effectiveOn)
        {
            UpdateOperationOutput<ANumberSaleCodeDetail> updateOperationOutput = new UpdateOperationOutput<ANumberSaleCodeDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IANumberSaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSaleCodeDataManager>();

            if (dataManager.Close(aNumberSaleCodeId, effectiveOn))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ANumberSaleCodeDetailMapper(this.GetANumberSaleCode(aNumberSaleCodeId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;

            return updateOperationOutput;
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetANumberSaleCodesType(), numberOfIDs, out startingId);
            return startingId;
        }
        public Type GetANumberSaleCodesType()
        {
            return this.GetType();
        }
        public List<string> GetUploadedSaleCodes(long fileId)
        {
            List<string> uploadedCodes = new List<string>();
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileId);
            byte[] bytes = file.Content;
            MemoryStream memStreamRate = new MemoryStream(bytes);
            Workbook objExcel = new Workbook(memStreamRate);
            Worksheet worksheet = objExcel.Worksheets[0];
            int count = 1;

            while (count < worksheet.Cells.Rows.Count)
            {
                string saleCode = worksheet.Cells[count, 0].StringValue.Trim();
                if (!uploadedCodes.Contains(saleCode) && !string.IsNullOrEmpty(saleCode) && Vanrise.Common.Utilities.IsNumeric(saleCode, 0))
                {
                    uploadedCodes.Add(saleCode);

                }
                count++;
            }

            return uploadedCodes;
        }

        #region private methodes

        private List<InvalidImportedSaleCode> ValidateImportedSaleCodes(List<string> codes, Dictionary<string, List<ANumberSaleCode>> structuredANumberSaleCodes, int aNumberGroupId, int sellingNumberPlanId, DateTime effectiveOn)
        {
            List<InvalidImportedSaleCode> invalidImportedSaleCodes = new List<InvalidImportedSaleCode>();

            foreach (var code in codes)
            {
                InvalidImportedSaleCode invalidImportedSaleCode = new InvalidImportedSaleCode();
                invalidImportedSaleCode.Code = code;

                if (String.IsNullOrEmpty(code))
                {
                    invalidImportedSaleCode.ErrorMessage = "Code Is Empty";
                    invalidImportedSaleCodes.Add(invalidImportedSaleCode);
                }
                else if (!Vanrise.Common.Utilities.IsNumeric(code, 0))
                {
                    invalidImportedSaleCode.ErrorMessage = "Code must be a positive number";
                    invalidImportedSaleCodes.Add(invalidImportedSaleCode);
                }
                else
                {
                    var exstingMatchedCodes = structuredANumberSaleCodes.GetRecord(code);
                    if (exstingMatchedCodes != null && exstingMatchedCodes.Count > 0)
                    {
                        foreach (var matchedCode in exstingMatchedCodes)
                        {
                            if (matchedCode.ANumberGroupId != aNumberGroupId)
                            {
                                invalidImportedSaleCode.ErrorMessage = "Code already exists in another ANumber group";
                                invalidImportedSaleCodes.Add(invalidImportedSaleCode);
                                break;
                            }
                        }
                    }
                }

            }

            return invalidImportedSaleCodes;
        }
        private Dictionary<string, List<ANumberSaleCode>> StructureANumberSaleCodesByCode(IEnumerable<ANumberSaleCode> filterdAnumberSaleCodes)
        {
            var structuredANumberSaleCodes = new Dictionary<string, List<ANumberSaleCode>>();
            foreach (ANumberSaleCode aNumberSaleCode in filterdAnumberSaleCodes)
            {
                List<ANumberSaleCode> aNumberSaleCodes = structuredANumberSaleCodes.GetOrCreateItem(aNumberSaleCode.Code);
                aNumberSaleCodes.Add(aNumberSaleCode);
            }

            return structuredANumberSaleCodes;

        }

        public ANumberSaleCodeDetail ANumberSaleCodeDetailMapper(ANumberSaleCode entity)
        {
            SellingNumberPlanManager _sellingNumberPlanManager = new SellingNumberPlanManager();

            return new ANumberSaleCodeDetail()
            {
                ANumberSaleCodeId = entity.ANumberSaleCodeId,
                SellingNumberPlanName = _sellingNumberPlanManager.GetSellingNumberPlanName(entity.SellingNumberPlanId),
                Code = entity.Code,
                BED = entity.BED,
                EED = entity.EED
            };
        }
         
        #endregion

        #region private class
        private class ANumberSaleCodeRequestHandler : BigDataRequestHandler<ANumberSaleCodeQuery, ANumberSaleCode, ANumberSaleCodeDetail>
        {

            
            public override ANumberSaleCodeDetail EntityDetailMapper(ANumberSaleCode entity)
            {
                return new ANumberSaleCodeManager().ANumberSaleCodeDetailMapper(entity);
            }

            public override IEnumerable<ANumberSaleCode> RetrieveAllData(DataRetrievalInput<ANumberSaleCodeQuery> input)
            {
                IANumberSaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSaleCodeDataManager>();
                var filtredAnumberSaleCodes = dataManager.GetFilteredANumberSaleCodes(input.Query.ANumberGroupId, input.Query.SellingNumberPlanIds);
                Func<ANumberSaleCode, bool> filterExpression = (x) =>
                {
                    if (!x.IsEffective(input.Query.EffectiveOn))
                        return false;
                    return true;
                };

                return filtredAnumberSaleCodes.FindAllRecords(filterExpression);
            }

            protected override ResultProcessingHandler<ANumberSaleCodeDetail> GetResultProcessingHandler(DataRetrievalInput<ANumberSaleCodeQuery> input, BigResult<ANumberSaleCodeDetail> bigResult)
            {
                return new ResultProcessingHandler<ANumberSaleCodeDetail>
                {
                    ExportExcelHandler = new ANumberSaleCodesExcelExportHandler()
                };
            }


        }
        private class ANumberSaleCodesExcelExportHandler : ExcelExportHandler<ANumberSaleCodeDetail>
        {

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ANumberSaleCodeDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Sale Codes",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Number Plan" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });


                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.ANumberSaleCodeId });
                            row.Cells.Add(new ExportExcelCell { Value = record.SellingNumberPlanName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Code });
                            row.Cells.Add(new ExportExcelCell { Value = record.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.EED });

                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

    }
}
