using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;
using System.Drawing;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ANumberSupplierCodeManager
    {
        public ANumberSupplierCode GetMatchSupplierCode(string number, int supplierId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
        public Vanrise.Entities.IDataRetrievalResult<ANumberSupplierCodeDetail> GetFilteredANumberSupplierCodes(Vanrise.Entities.DataRetrievalInput<ANumberSupplierCodeQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ANumberSupplierCodeRequestHandler());
        }
       
        public IEnumerable<ANumberSupplierCode> GetEffectiveAfterBySupplierId(int supplierId, DateTime effectiveOn)
        {
            IANumberSupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSupplierCodeDataManager>();
            return dataManager.GetEffectiveAfterBySupplierId(supplierId, effectiveOn);
        }


        public ANumberSupplierCodesInsertResult AddANumberSupplierCodes(ANumberSupplierCodesInsertInput input)
        {
            IANumberSupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSupplierCodeDataManager>();

            var matchedSupplierCodes = GetEffectiveAfterBySupplierId(input.SupplierId, input.EffectiveOn);

            Dictionary<string, List<ANumberSupplierCode>> structuredANumberSupplierCodes = StructureANumberSupplierCodesByCode(matchedSupplierCodes);

            ANumberSupplierCodesInsertResult result = new ANumberSupplierCodesInsertResult();

            result.InvalidImportedSupplierCodes = ValidateImportedSupplierCodes(input.Codes, structuredANumberSupplierCodes, input.ANumberGroupId, input.EffectiveOn);

            if (result.InvalidImportedSupplierCodes.Count() > 0)
            {
                result.ResultMessage = "Import ANumber Supplier Codes failed.";
            }
            else
            {
                long startingId = this.ReserveIDRange(input.Codes.Count());
                foreach (var code in input.Codes)
                {
                    var SupplierCodeInsertObject = new ANumberSupplierCode
                    {
                        BED = input.EffectiveOn
                    };
                    List<ANumberSupplierCodeToClose> codesToClose = new List<ANumberSupplierCodeToClose>();
                    var exstingMatchedCodes = structuredANumberSupplierCodes.GetRecord(code);
                    if (exstingMatchedCodes != null && exstingMatchedCodes.Count > 0)
                    {
                        foreach (var matchedCode in exstingMatchedCodes)
                        {

                            if (matchedCode.IsOverlappedWith(SupplierCodeInsertObject))
                            {
                                DateTime closeDate = Utilities.Max(input.EffectiveOn, matchedCode.BED);
                                codesToClose.Add(new ANumberSupplierCodeToClose() { ANumberSupplierCodeId = matchedCode.ANumberSupplierCodeId, CloseDate = closeDate });
                            }

                        }
                    }
                    bool updateActionSucc = dataManager.Insert(codesToClose, startingId++, input.ANumberGroupId, input.SupplierId, code, input.EffectiveOn);
                }
                result.ResultMessage = "Import ANumber Supplier Codes Succeed.";
            }

            return result;

        }

        public ANumberSupplierCode GetANumberSupplierCode(long aNumberSupplierCodeId)
        {
            IANumberSupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSupplierCodeDataManager>();
            return dataManager.GetANumberSupplierCode(aNumberSupplierCodeId);
        }
        public byte[] DownloadSupplierCodesLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetANumberSupplierCodesType(), numberOfIDs, out startingId);
            return startingId;
        }
        public Type GetANumberSupplierCodesType()
        {
            return this.GetType();
        }
        public List<string> GetUploadedSupplierCodes(long fileId)
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
                string supplierCode = worksheet.Cells[count, 0].StringValue.Trim();
                if (!uploadedCodes.Contains(supplierCode) && !string.IsNullOrEmpty(supplierCode) && Vanrise.Common.Utilities.IsNumeric(supplierCode, 0))
                {
                    uploadedCodes.Add(supplierCode);

                }
                count++;
            }

            return uploadedCodes;
        }


        #region private methodes

        private List<InvalidImportedSupplierCode> ValidateImportedSupplierCodes(List<string> codes, Dictionary<string, List<ANumberSupplierCode>> structuredANumberSupplierCodes, int aNumberGroupId, DateTime effectiveOn)
        {
            List<InvalidImportedSupplierCode> invalidImportedSupplierCodes = new List<InvalidImportedSupplierCode>();

            foreach (var code in codes)
            {
                InvalidImportedSupplierCode invalidImportedSupplierCode = new InvalidImportedSupplierCode();
                invalidImportedSupplierCode.Code = code;

                if (String.IsNullOrEmpty(code))
                {
                    invalidImportedSupplierCode.ErrorMessage = "Code Is Empty";
                    invalidImportedSupplierCodes.Add(invalidImportedSupplierCode);
                }
                else if (!Vanrise.Common.Utilities.IsNumeric(code, 0))
                {
                    invalidImportedSupplierCode.ErrorMessage = "Code must be a positive number";
                    invalidImportedSupplierCodes.Add(invalidImportedSupplierCode);
                }
                else
                {
                    var exstingMatchedCodes = structuredANumberSupplierCodes.GetRecord(code);
                    if (exstingMatchedCodes != null && exstingMatchedCodes.Count > 0)
                    {
                        foreach (var matchedCode in exstingMatchedCodes)
                        {
                            if (matchedCode.ANumberGroupId != aNumberGroupId)
                            {
                                invalidImportedSupplierCode.ErrorMessage = "Code already exists in another ANumber group";
                                invalidImportedSupplierCodes.Add(invalidImportedSupplierCode);
                                break;
                            }
                        }
                    }
                }

            }

            return invalidImportedSupplierCodes;
        }
        private Dictionary<string, List<ANumberSupplierCode>> StructureANumberSupplierCodesByCode(IEnumerable<ANumberSupplierCode> filterdAnumberSupplierCodes)
        {
            var structuredANumberSupplierCodes = new Dictionary<string, List<ANumberSupplierCode>>();
            foreach (ANumberSupplierCode aNumberSupplierCode in filterdAnumberSupplierCodes)
            {
                List<ANumberSupplierCode> aNumberSupplierCodes = structuredANumberSupplierCodes.GetOrCreateItem(aNumberSupplierCode.Code);
                aNumberSupplierCodes.Add(aNumberSupplierCode);
            }

            return structuredANumberSupplierCodes;

        }

        #endregion

        #region private class
        private class ANumberSupplierCodeRequestHandler : BigDataRequestHandler<ANumberSupplierCodeQuery, ANumberSupplierCode, ANumberSupplierCodeDetail>
        {

            private CarrierAccountManager _carrierAccountManager;
            public ANumberSupplierCodeRequestHandler()
            {
                _carrierAccountManager = new CarrierAccountManager();
            }
            public override ANumberSupplierCodeDetail EntityDetailMapper(ANumberSupplierCode entity)
            {
                return new ANumberSupplierCodeDetail()
                {
                    ANumberSupplierCodeId = entity.ANumberSupplierCodeId,
                    SupplierName = _carrierAccountManager.GetCarrierAccountName(entity.SupplierId),
                    Code = entity.Code,
                    BED = entity.BED,
                    EED = entity.EED
                };
            }

            public override IEnumerable<ANumberSupplierCode> RetrieveAllData(DataRetrievalInput<ANumberSupplierCodeQuery> input)
            {
                IANumberSupplierCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<IANumberSupplierCodeDataManager>();
                var filtredAnumberSupplierCodes = dataManager.GetFilteredANumberSupplierCodes(input.Query.ANumberGroupId, input.Query.SupplierIds);
                Func<ANumberSupplierCode, bool> filterExpression = (x) =>
                {
                    if (!x.IsEffective(input.Query.EffectiveOn))
                        return false;
                    return true;
                };
                return filtredAnumberSupplierCodes.FindAllRecords(filterExpression);
            }

            protected override ResultProcessingHandler<ANumberSupplierCodeDetail> GetResultProcessingHandler(DataRetrievalInput<ANumberSupplierCodeQuery> input, BigResult<ANumberSupplierCodeDetail> bigResult)
            {
                return new ResultProcessingHandler<ANumberSupplierCodeDetail>
                {
                    ExportExcelHandler = new ANumberSupplierCodesExcelExportHandler()
                };
            }

        }

      
        private class ANumberSupplierCodesExcelExportHandler : ExcelExportHandler<ANumberSupplierCodeDetail>
        {

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ANumberSupplierCodeDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Codes",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier" });
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
                            row.Cells.Add(new ExportExcelCell { Value = record.ANumberSupplierCodeId });
                            row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
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
