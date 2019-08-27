using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.MobileNetwork.Business;
using Vanrise.MobileNetwork.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SupplierSMSRateDraftManager
    {
        IProcessDraftDataManager _processDraftDataManager = SMSBEDataFactory.GetDataManager<IProcessDraftDataManager>();

        #region Public Methods
        public List<SupplierSMSRateChangesDetail> GetFilteredChanges(SupplierSMSRateChangesQuery input)
        {
            input.ThrowIfNull("input");

            Dictionary<string, List<MobileNetwork>> mobileNetworksByMobileCountryNames = new Dictionary<string, List<MobileNetwork>>();
            if (input.Filter != null)
                mobileNetworksByMobileCountryNames = new MobileNetworkManager().GetMobileNetworkByMobileCountryNames(input.Filter.CountryChar);

            if (mobileNetworksByMobileCountryNames.Count > 0)
            {
                List<SupplierSMSRateChangesDetail> supplierSMSRateChangesDetails = GetInitialSupplierSMSRateChangesDetails(mobileNetworksByMobileCountryNames);

                SupplierSMSRateDraft supplierSMSRateDraft = GetSupplierSMSRateDraft(input.ProcessDraftID);
                Dictionary<int, SupplierSMSRateChange> SupplierSMSRateChanges = new Dictionary<int, SupplierSMSRateChange>();
                if (supplierSMSRateDraft != null && supplierSMSRateDraft.SMSRates != null)
                    SupplierSMSRateChanges = supplierSMSRateDraft.SMSRates;

                Dictionary<int, SupplierSMSRateItem> mobileNetworkRates = new SupplierSMSRateManager().GetEffectiveMobileNetworkRates(input.SupplierID, DateTime.Now);

                SetSMSRateDetails(supplierSMSRateChangesDetails, SupplierSMSRateChanges, mobileNetworkRates);
                return supplierSMSRateChangesDetails;
            }

            return null;
        }

        public DraftStateResult InsertOrUpdateChanges(SupplierSMSRateDraftToUpdate supplierDraftToUpdate)
        {
            SupplierSMSRateDraft supplierSMSRateDraft = MergeImportedDraft(supplierDraftToUpdate);

            string serializedChanges = supplierSMSRateDraft != null ? Serializer.Serialize(supplierSMSRateDraft) : null;

            long? result;
            bool isInsertedOrUpdated = _processDraftDataManager.InsertOrUpdateChanges(ProcessEntityType.Supplier, serializedChanges, supplierSMSRateDraft.SupplierID.ToString(), SecurityContext.Current.GetLoggedInUserId(), out result);

            return isInsertedOrUpdated ? new DraftStateResult() { ProcessDraftID = result } : null;
        }

        public bool UpdateSMSRateChangesStatus(UpdateSupplierSMSDraftStatusInput input, int userID)
        {
            input.ThrowIfNull("input");
            return _processDraftDataManager.UpdateProcessStatus(input.ProcessDraftID, input.NewStatus, userID);
        }

        public DraftData GetDraftData(SupplierDraftDataInput input)
        {
            input.ThrowIfNull("input");

            ProcessDraft processDraft = _processDraftDataManager.GetChangesByEntityID(ProcessEntityType.Supplier, input.SupplierID.ToString());

            SupplierSMSRateDraft supplierSMSRateDraft = processDraft != null && processDraft.Changes != null && !string.IsNullOrEmpty(processDraft.Changes) ? Serializer.Deserialize<SupplierSMSRateDraft>(processDraft.Changes) : null;

            return new DraftData()
            {
                ProcessDraftID = processDraft != null && processDraft.Changes != null ? processDraft.ID : (long?)null,
                DraftEffectiveDate = supplierSMSRateDraft != null ? supplierSMSRateDraft.EffectiveDate : default(DateTime?),
                CountryLetters = new MobileNetworkManager().GetDistinctMobileCountryLetters(),
                PendingChanges = supplierSMSRateDraft != null && supplierSMSRateDraft.SMSRates != null ? supplierSMSRateDraft.SMSRates.Count : 0
            };
        }

        public SupplierSMSRateDraft GetSupplierSMSRateDraft(long processDraftID)
        {
            ProcessDraft processDraft = _processDraftDataManager.GetChangesByProcessDraftID(processDraftID);
            if (processDraft == null || processDraft.Changes == null)
                return null;

            return !string.IsNullOrEmpty(processDraft.Changes) ? Serializer.Deserialize<SupplierSMSRateDraft>(processDraft.Changes) : null;
        }

        public UploadSupplierSMSRateChangesLog UploadSMSRateChanges(UploadSupplierSMSRateChangesInput input)
        {
            int supplierId = input.SupplierID;
            int currencyId = input.CurrencyId;
            long fileId = input.FileId;

            Style outputCellStyle;
            Worksheet worksheet = GetExcelWorkSheet(fileId);
            Worksheet outputWorksheet = GetExcelOutputWorkSheet(fileId, out outputCellStyle);

            var uploadOutput = new UploadSupplierSMSRateChangesLog
            {
                NumberOfItemsAdded = 0,
                NumberOfItemsFailed = 0,
            };

            string errorMessage;
            if (!CheckMatchingWithExcelTemplate(worksheet, out errorMessage))
            {
                uploadOutput.ErrorMessage = errorMessage;
                return uploadOutput;
            }

            var draftData = GetDraftData(new SupplierDraftDataInput { SupplierID = supplierId });

            SupplierSMSRateDraftToUpdate supplierSMSRateDraftToUpdate = new SupplierSMSRateDraftToUpdate()
            {
                CurrencyId = currencyId,
                SupplierID = supplierId,
                EffectiveDate = draftData.DraftEffectiveDate.HasValue ? draftData.DraftEffectiveDate.Value : DateTime.Today,
                ProcessDraftID = draftData.ProcessDraftID,
                SMSRates = new List<SupplierSMSRateChangeToUpdate>()
            };

            var mobileNetworksByName = new MobileNetworkManager().GetCachedMobileNetworksByName();

            int outputResultColumnIndex = outputWorksheet.Cells.MaxColumn - 1;
            int outputErrorMessageColumnIndex = outputWorksheet.Cells.MaxColumn;

            Dictionary<int, decimal> smsRateByModileNetworkId = new Dictionary<int, decimal>();

            for (int i = 1; i < worksheet.Cells.Rows.Count; i++)
            {
                Cell mobileNetworkCell = worksheet.Cells[i, 0];
                Cell rateCell = worksheet.Cells[i, 1];

                string importedMobileNetwork;
                string importedRate;

                if (IsExcelRowEmpty(mobileNetworkCell, rateCell, out importedMobileNetwork, out importedRate))
                    continue;

                if (string.IsNullOrEmpty(importedMobileNetwork))
                {
                    SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Failed", outputErrorMessageColumnIndex, "Empty Mobile Network Name", outputCellStyle);
                    uploadOutput.NumberOfItemsFailed++;
                    continue;
                }

                if (string.IsNullOrEmpty(importedRate))
                {
                    SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Failed", outputErrorMessageColumnIndex, "Empty SMS Rate", outputCellStyle);
                    uploadOutput.NumberOfItemsFailed++;
                    continue;
                }

                decimal smsRate;
                if (!decimal.TryParse(importedRate, out smsRate) || smsRate < 0)
                {
                    SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Failed", outputErrorMessageColumnIndex, "Invalid SMS Rate Format", outputCellStyle);
                    uploadOutput.NumberOfItemsFailed++;
                    continue;
                }

                MobileNetwork mobileNetwork;
                if (!mobileNetworksByName.TryGetValue(importedMobileNetwork, out mobileNetwork))
                {
                    SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Failed", outputErrorMessageColumnIndex, "Invalid Mobile Network Name", outputCellStyle);
                    uploadOutput.NumberOfItemsFailed++;
                    continue;
                }

                int mobileNetworkId = mobileNetwork.Id;
                decimal existedSMSRate;
                if (smsRateByModileNetworkId.TryGetValue(mobileNetworkId, out existedSMSRate))
                {
                    string errorDuplicationMessage = existedSMSRate == smsRate ? "Same row already exists in this sheet" : "Same Mobile Network already has a rate in this sheet";
                    SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Failed", outputErrorMessageColumnIndex, errorDuplicationMessage, outputCellStyle);
                    uploadOutput.NumberOfItemsFailed++;
                    continue;
                }

                var supplierSMSRateChangeToUpdate = new SupplierSMSRateChangeToUpdate { MobileNetworkID = mobileNetworkId, NewRate = smsRate };

                SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Succeeded", outputErrorMessageColumnIndex, null, outputCellStyle);
                uploadOutput.NumberOfItemsAdded++;

                supplierSMSRateDraftToUpdate.SMSRates.Add(supplierSMSRateChangeToUpdate);
                smsRateByModileNetworkId.Add(mobileNetworkId, smsRate);
            }

            if (uploadOutput.NumberOfItemsAdded > 0)
            {
                var draftStateResult = InsertOrUpdateChanges(supplierSMSRateDraftToUpdate);
                if (!draftStateResult.ProcessDraftID.HasValue)
                {
                    uploadOutput.ErrorMessage = "Draft Doesn't Saved !";
                    return uploadOutput;
                }

                uploadOutput.ProcessDraftID = draftStateResult.ProcessDraftID;
            }

            long outputFileId;
            CreateOutputFileFromExcelWorksheet(outputWorksheet, out outputFileId);
            uploadOutput.FileID = outputFileId;

            return uploadOutput;
        }

        public byte[] DownloadImportedSupplierSMSRateLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }

        #endregion

        #region Private Methods 

        private List<SupplierSMSRateChangesDetail> GetInitialSupplierSMSRateChangesDetails(Dictionary<string, List<MobileNetwork>> mobileNetworksByCountryNames)
        {
            if (mobileNetworksByCountryNames != null)
            {
                List<SupplierSMSRateChangesDetail> supplierSMSRateChangesDetails = new List<SupplierSMSRateChangesDetail>();
                foreach (var mobileNetworkKvp in mobileNetworksByCountryNames)
                {
                    string mobileCountryName = mobileNetworkKvp.Key;
                    List<MobileNetwork> mobileNetworks = mobileNetworkKvp.Value;
                    foreach (var mobileNetwork in mobileNetworks)
                    {
                        supplierSMSRateChangesDetails.Add(new SupplierSMSRateChangesDetail()
                        {
                            MobileCountryName = mobileCountryName,
                            MobileNetworkName = mobileNetwork.NetworkName,
                            MobileNetworkID = mobileNetwork.Id,
                        });
                    }
                }

                return supplierSMSRateChangesDetails.OrderBy(item => item.MobileCountryName).ThenBy(item => item.MobileNetworkName).ToList();
            }

            return null;
        }

        private void SetSMSRateDetails(List<SupplierSMSRateChangesDetail> supplierSMSRateChangesDetails, Dictionary<int, SupplierSMSRateChange> supplierSMSRateChanges, Dictionary<int, SupplierSMSRateItem> supplierSMSRates)
        {
            DateTime dateTime = DateTime.Now;
            bool haveChanges = (supplierSMSRateChanges != null);
            bool haveRates = (supplierSMSRates != null);

            foreach (SupplierSMSRateChangesDetail supplierSMSRateChangesDetail in supplierSMSRateChangesDetails)
            {
                int mobileNetworkID = supplierSMSRateChangesDetail.MobileNetworkID;
                if (haveChanges)
                {
                    SupplierSMSRateChange supplierSMSRateChange = supplierSMSRateChanges.GetRecord(mobileNetworkID);
                    if (supplierSMSRateChange != null)
                        supplierSMSRateChangesDetail.NewRate = supplierSMSRateChange.NewRate;
                }

                if (haveRates)
                {
                    SupplierSMSRateItem supplierEffectiveSMSRate = supplierSMSRates.GetRecord(mobileNetworkID);

                    if (supplierEffectiveSMSRate != null)
                    {
                        var futureRate = supplierEffectiveSMSRate.FutureRate;
                        supplierSMSRateChangesDetail.CurrentRate = supplierEffectiveSMSRate.CurrentRate != null ? supplierEffectiveSMSRate.CurrentRate.Rate : (decimal?)null;
                        supplierSMSRateChangesDetail.FutureRate = futureRate != null ? new SMSFutureRate() { Rate = futureRate.Rate, BED = futureRate.BED, EED = futureRate.EED } : null;
                    }
                }
            }
        }

        private SupplierSMSRateDraft MergeImportedDraft(SupplierSMSRateDraftToUpdate supplierDraftToUpdate)
        {

            SupplierSMSRateDraft supplierSMSRateChanges = supplierDraftToUpdate.ProcessDraftID.HasValue ? GetSupplierSMSRateDraft(supplierDraftToUpdate.ProcessDraftID.Value) : null;

            if (supplierDraftToUpdate == null)
                return supplierSMSRateChanges;

            if (supplierSMSRateChanges == null || supplierSMSRateChanges.SMSRates == null || supplierSMSRateChanges.SMSRates.Count == 0)
            {
                return new SupplierSMSRateDraft()
                {
                    SupplierID = supplierDraftToUpdate.SupplierID,
                    CurrencyId = supplierDraftToUpdate.CurrencyId,
                    EffectiveDate = supplierDraftToUpdate.EffectiveDate,
                    SMSRates = BuildSupplierSMSRateChangesMapper(supplierDraftToUpdate.SMSRates),
                    Status = ProcessStatus.Draft
                };
            }

            supplierSMSRateChanges.EffectiveDate = supplierDraftToUpdate.EffectiveDate;
            supplierSMSRateChanges.Status = ProcessStatus.Draft;

            Dictionary<int, SupplierSMSRateChange> oldSupplierSMSRates = supplierSMSRateChanges.SMSRates;

            List<SupplierSMSRateChangeToUpdate> newSupplierSMSRates = supplierDraftToUpdate.SMSRates;

            foreach (var newSupplierSMSRateHistory in newSupplierSMSRates)
            {
                int mobileNetworkID = newSupplierSMSRateHistory.MobileNetworkID;

                if (!oldSupplierSMSRates.ContainsKey(mobileNetworkID) && newSupplierSMSRateHistory.NewRate.HasValue)
                {
                    oldSupplierSMSRates.Add(mobileNetworkID, new SupplierSMSRateChange() { MobileNetworkID = mobileNetworkID, NewRate = newSupplierSMSRateHistory.NewRate.Value });
                    continue;
                }

                if (oldSupplierSMSRates.ContainsKey(mobileNetworkID) && newSupplierSMSRateHistory.NewRate.HasValue)
                {
                    oldSupplierSMSRates[mobileNetworkID] = new SupplierSMSRateChange() { MobileNetworkID = mobileNetworkID, NewRate = newSupplierSMSRateHistory.NewRate.Value };
                    continue;
                }

                oldSupplierSMSRates.Remove(mobileNetworkID);
            }

            supplierSMSRateChanges.SMSRates = oldSupplierSMSRates;

            return supplierSMSRateChanges;
        }

        private Dictionary<int, SupplierSMSRateChange> BuildSupplierSMSRateChangesMapper(List<SupplierSMSRateChangeToUpdate> supplierSMSRatesChangeToUpdate)
        {
            if (supplierSMSRatesChangeToUpdate == null)
                return null;

            Dictionary<int, SupplierSMSRateChange> supplierSMSRateChanges = new Dictionary<int, SupplierSMSRateChange>();
            foreach (var supplierSMSRateChangeToUpdate in supplierSMSRatesChangeToUpdate)
            {
                if (supplierSMSRateChangeToUpdate.NewRate.HasValue)
                    supplierSMSRateChanges.Add(supplierSMSRateChangeToUpdate.MobileNetworkID, new SupplierSMSRateChange()
                    {
                        NewRate = supplierSMSRateChangeToUpdate.NewRate.Value,
                        MobileNetworkID = supplierSMSRateChangeToUpdate.MobileNetworkID
                    });
            }

            return supplierSMSRateChanges;
        }

        #region Upload Excel Methods
        private bool IsExcelRowEmpty(Cell mobileNetworkCell, Cell rateCell, out string importedMobileNetwork, out string importedRate)
        {
            bool isUploadedModileNetworkEmpty = IsExcelStringEmpty(mobileNetworkCell.StringValue, out importedMobileNetwork);
            bool isUploadedRateEmpty = IsExcelStringEmpty(rateCell.StringValue, out importedRate);

            return (isUploadedModileNetworkEmpty && isUploadedRateEmpty);
        }

        private bool IsExcelStringEmpty(string originalString, out string trimmedString)
        {
            trimmedString = null;

            if (string.IsNullOrEmpty(originalString))
                return true;

            trimmedString = originalString.Trim();
            return string.IsNullOrEmpty(trimmedString);
        }

        private bool CheckMatchingWithExcelTemplate(Worksheet worksheet, out string errorMessage)
        {
            errorMessage = null;
            var nbOfRows = worksheet.Cells.MaxRow + 1;
            var nbOfCols = worksheet.Cells.MaxColumn + 1;

            if (nbOfRows == 1)
            {
                errorMessage = "Empty File";
                return false;
            }

            HashSet<string> templateHeaders = new HashSet<string>() { "MOBILE NETWORK", "RATE" };

            int templateNbOfCols = templateHeaders.Count;

            if (nbOfCols < templateNbOfCols)
            {
                errorMessage = "Invalid Number Of Columns";
                return false;
            }

            HashSet<string> invalidColumns = new HashSet<string>();

            for (int headerIndex = 0; headerIndex < templateNbOfCols; headerIndex++)
            {
                var templateHeader = templateHeaders.ElementAt(headerIndex);
                var header = worksheet.Cells[0, headerIndex];

                if (string.Compare(templateHeader, header.StringValue, true) != 0)
                {
                    invalidColumns.Add(header.StringValue);
                }
            }

            if (invalidColumns.Count > 0)
            {
                errorMessage = $"Invalid Columns: { string.Join(", ", invalidColumns)}";
                return false;
            }

            return true;
        }

        private Worksheet GetExcelWorkSheet(long fileId)
        {
            var fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileId);

            if (file == null)
                throw new Vanrise.Entities.DataIntegrityValidationException($"File '{fileId}' was not found");
            if (file.Content == null)
                throw new Vanrise.Entities.DataIntegrityValidationException($"File '{fileId}' is empty");

            byte[] bytes = file.Content;
            var fileStream = new MemoryStream(bytes);

            Vanrise.Common.Utilities.ActivateAspose();

            var workbook = new Workbook(fileStream);
            if (workbook.Worksheets == null || workbook.Worksheets.Count == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException($"Workbook created from file '{fileId}' does not contain any worksheets");

            return workbook.Worksheets.ElementAt(0);
        }

        private Worksheet GetExcelOutputWorkSheet(long fileId, out Style cellStyle)
        {
            var outputWorkSheet = GetExcelWorkSheet(fileId);

            outputWorkSheet.Name = "Result";
            int colnum = outputWorkSheet.Cells.MaxColumn + 1;
            outputWorkSheet.Cells.SetColumnWidth(colnum, 20);
            outputWorkSheet.Cells.SetColumnWidth(colnum + 1, 40);
            outputWorkSheet.Cells[0, colnum].PutValue("Result");
            outputWorkSheet.Cells[0, colnum + 1].PutValue("Error Message");

            Style headerStyle = new Style();
            headerStyle.Font.Name = "Times New Roman";
            headerStyle.Font.Color = Color.Red;
            headerStyle.Font.Size = 14;
            headerStyle.Font.IsBold = true;

            outputWorkSheet.Cells[0, colnum].SetStyle(headerStyle);
            outputWorkSheet.Cells[0, colnum + 1].SetStyle(headerStyle);

            cellStyle = new Style();
            cellStyle.Font.Name = "Times New Roman";
            cellStyle.Font.Color = Color.Black;
            cellStyle.Font.Size = 12;

            return outputWorkSheet;
        }

        private void CreateOutputFileFromExcelWorksheet(Worksheet outputWorksheet, out long fileId)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = outputWorksheet.Workbook.SaveToStream();

            VRFile returnedFile = new VRFile() { Content = memoryStream.ToArray(), Name = "UploadSMSSaleRatesOutput", Extension = ".xlsx", IsTemp = true };

            VRFileManager fileManager = new VRFileManager();
            var outputFileId = fileManager.AddFile(returnedFile);
            fileId = outputFileId;
        }

        private void SetMessageToOutputWorksheetRow(Worksheet outputWorksheet, int rowIndex, int outputResultColumnIndex, string resultMessage, int outputErrorMessageColumnIndex, string errorMessage, Style outputCellStyle)
        {
            if (!string.IsNullOrEmpty(resultMessage))
            {
                outputWorksheet.Cells[rowIndex, outputResultColumnIndex].PutValue(resultMessage);
                outputWorksheet.Cells[rowIndex, outputResultColumnIndex].SetStyle(outputCellStyle);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                outputWorksheet.Cells[rowIndex, outputErrorMessageColumnIndex].PutValue(errorMessage);
                outputWorksheet.Cells[rowIndex, outputErrorMessageColumnIndex].SetStyle(outputCellStyle);
            }
        }

        #endregion

        #endregion
    }
}
