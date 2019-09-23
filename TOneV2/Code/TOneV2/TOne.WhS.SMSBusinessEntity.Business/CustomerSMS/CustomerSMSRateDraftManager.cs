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
    public class CustomerSMSRateDraftManager
    {
        IProcessDraftDataManager _processDraftDataManager = SMSBEDataFactory.GetDataManager<IProcessDraftDataManager>();

        #region Public Methods
        public List<CustomerSMSRateChangesDetail> GetFilteredChanges(CustomerSMSRateChangesQuery input)
        {
            input.ThrowIfNull("input");

            Dictionary<string, List<MobileNetwork>> mobileNetworksByMobileCountryNames = new Dictionary<string, List<MobileNetwork>>();
            if (input.Filter != null)
                mobileNetworksByMobileCountryNames = new MobileNetworkManager().GetMobileNetworkByMobileCountryNames(input.Filter.CountryChar);

            if (mobileNetworksByMobileCountryNames.Count > 0)
            {
                List<CustomerSMSRateChangesDetail> customerSMSRateChangesDetails = GetInitialCustomerSMSRateChangesDetails(mobileNetworksByMobileCountryNames);

                CustomerSMSRateDraft customerSMSRateDraft = GetCustomerSMSRateDraft(input.ProcessDraftID);
                Dictionary<int, CustomerSMSRateChange> CustomerSMSRateChanges = new Dictionary<int, CustomerSMSRateChange>();
                if (customerSMSRateDraft != null && customerSMSRateDraft.SMSRates != null)
                    CustomerSMSRateChanges = customerSMSRateDraft.SMSRates;

                Dictionary<int, CustomerSMSRateItem> mobileNetworkRates = new CustomerSMSRateManager().GetEffectiveMobileNetworkRates(input.CustomerID, DateTime.Now);

                SetSMSRateDetails(customerSMSRateChangesDetails, CustomerSMSRateChanges, mobileNetworkRates);
                return customerSMSRateChangesDetails;
            }

            return null;
        }

        public DraftStateResult InsertOrUpdateChanges(CustomerSMSRateDraftToUpdate customerDraftToUpdate)
        {
            CustomerSMSRateDraft customerSMSRateDraft = MergeImportedDraft(customerDraftToUpdate);

            string serializedChanges = customerSMSRateDraft != null ? Serializer.Serialize(customerSMSRateDraft) : null;

            long? result;
            bool isInsertedOrUpdated = _processDraftDataManager.InsertOrUpdateChanges(ProcessEntityType.Customer, serializedChanges, customerSMSRateDraft.CustomerID.ToString(), SecurityContext.Current.GetLoggedInUserId(), out result);

            return isInsertedOrUpdated ? new DraftStateResult() { ProcessDraftID = result } : null;
        }

        public bool UpdateSMSRateChangesStatus(UpdateCustomerSMSDraftStatusInput input, int userID)
        {
            input.ThrowIfNull("input");
            return _processDraftDataManager.UpdateProcessStatus(input.ProcessDraftID, input.NewStatus, userID);
        }

        public DraftData GetDraftData(CustomerDraftDataInput input)
        {
            input.ThrowIfNull("input");

            ProcessDraft processDraft = _processDraftDataManager.GetChangesByEntityID(ProcessEntityType.Customer, input.CustomerID.ToString());

            CustomerSMSRateDraft customerSMSRateDraft = processDraft != null && processDraft.Changes != null && !string.IsNullOrEmpty(processDraft.Changes) ? Serializer.Deserialize<CustomerSMSRateDraft>(processDraft.Changes) : null;

            return new DraftData()
            {
                ProcessDraftID = processDraft != null && processDraft.Changes != null ? processDraft.ID : (long?)null,
                DraftEffectiveDate = customerSMSRateDraft != null ? customerSMSRateDraft.EffectiveDate : default(DateTime?),
                CountryLetters = new MobileNetworkManager().GetDistinctMobileCountryLetters(),
                PendingChanges = customerSMSRateDraft != null && customerSMSRateDraft.SMSRates != null ? customerSMSRateDraft.SMSRates.Count : 0
            };
        }

        public CustomerSMSRateDraft GetCustomerSMSRateDraft(long processDraftID)
        {
            ProcessDraft processDraft = _processDraftDataManager.GetChangesByProcessDraftID(processDraftID);
            if (processDraft == null || processDraft.Changes == null)
                return null;

            return !string.IsNullOrEmpty(processDraft.Changes) ? Serializer.Deserialize<CustomerSMSRateDraft>(processDraft.Changes) : null;
        }

        public UploadCustomerSMSRateChangesLog UploadSMSRateChanges(UploadCustomerSMSRateChangesInput input)
        {
            int customerId = input.CustomerID;
            int currencyId = input.CurrencyId;
            long fileId = input.FileId;

            Style outputCellStyle;
            Worksheet worksheet = GetExcelWorkSheet(fileId);
            Worksheet outputWorksheet = GetExcelOutputWorkSheet(fileId, out outputCellStyle);

            var uploadOutput = new UploadCustomerSMSRateChangesLog
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

            var draftData = GetDraftData(new CustomerDraftDataInput { CustomerID = customerId });

            CustomerSMSRateDraftToUpdate customerSMSRateDraftToUpdate = new CustomerSMSRateDraftToUpdate()
            {
                CurrencyId = currencyId,
                CustomerID = customerId,
                EffectiveDate = draftData.DraftEffectiveDate.HasValue ? draftData.DraftEffectiveDate.Value : DateTime.Today,
                ProcessDraftID = draftData.ProcessDraftID,
                SMSRates = new List<CustomerSMSRateChangeToUpdate>()
            };

            var mobileNetworksByLowerName = new MobileNetworkManager().GetCachedMobileNetworksByLowerName();
            if (mobileNetworksByLowerName == null || mobileNetworksByLowerName.Count == 0)
            {
                uploadOutput.ErrorMessage = "Any Mobile Network is defined";
                return uploadOutput;
            }

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
                if (!mobileNetworksByLowerName.TryGetValue(importedMobileNetwork.ToLower(), out mobileNetwork))
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

                var customerSMSRateChangeToUpdate = new CustomerSMSRateChangeToUpdate { MobileNetworkID = mobileNetworkId, NewRate = smsRate };

                SetMessageToOutputWorksheetRow(outputWorksheet, i, outputResultColumnIndex, "Succeeded", outputErrorMessageColumnIndex, null, outputCellStyle);
                uploadOutput.NumberOfItemsAdded++;

                customerSMSRateDraftToUpdate.SMSRates.Add(customerSMSRateChangeToUpdate);
                smsRateByModileNetworkId.Add(mobileNetworkId, smsRate);
            }

            if (uploadOutput.NumberOfItemsAdded > 0)
            {
                var draftStateResult = InsertOrUpdateChanges(customerSMSRateDraftToUpdate);
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

        public byte[] DownloadImportedCustomerSMSRateLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }

        #endregion

        #region Private Methods 

        private List<CustomerSMSRateChangesDetail> GetInitialCustomerSMSRateChangesDetails(Dictionary<string, List<MobileNetwork>> mobileNetworksByCountryNames)
        {
            if (mobileNetworksByCountryNames != null)
            {
                List<CustomerSMSRateChangesDetail> customerSMSRateChangesDetails = new List<CustomerSMSRateChangesDetail>();
                foreach (var mobileNetworkKvp in mobileNetworksByCountryNames)
                {
                    string mobileCountryName = mobileNetworkKvp.Key;
                    List<MobileNetwork> mobileNetworks = mobileNetworkKvp.Value;
                    foreach (var mobileNetwork in mobileNetworks)
                    {
                        customerSMSRateChangesDetails.Add(new CustomerSMSRateChangesDetail()
                        {
                            MobileCountryName = mobileCountryName,
                            MobileNetworkName = mobileNetwork.NetworkName,
                            MobileNetworkID = mobileNetwork.Id,
                        });
                    }
                }

                return customerSMSRateChangesDetails.OrderBy(item => item.MobileCountryName).ThenBy(item => item.MobileNetworkName).ToList();
            }

            return null;
        }

        private void SetSMSRateDetails(List<CustomerSMSRateChangesDetail> customerSMSRateChangesDetails, Dictionary<int, CustomerSMSRateChange> customerSMSRateChanges, Dictionary<int, CustomerSMSRateItem> customerSMSRates)
        {
            DateTime dateTime = DateTime.Now;
            bool haveChanges = (customerSMSRateChanges != null);
            bool haveRates = (customerSMSRates != null);

            foreach (CustomerSMSRateChangesDetail customerSMSRateChangesDetail in customerSMSRateChangesDetails)
            {
                int mobileNetworkID = customerSMSRateChangesDetail.MobileNetworkID;
                if (haveChanges)
                {
                    CustomerSMSRateChange customerSMSRateChange = customerSMSRateChanges.GetRecord(mobileNetworkID);
                    if (customerSMSRateChange != null)
                        customerSMSRateChangesDetail.NewRate = customerSMSRateChange.NewRate;
                }

                if (haveRates)
                {
                    CustomerSMSRateItem customerEffectiveSMSRate = customerSMSRates.GetRecord(mobileNetworkID);

                    if (customerEffectiveSMSRate != null)
                    {
                        var futureRate = customerEffectiveSMSRate.FutureRate;
                        customerSMSRateChangesDetail.CurrentRate = customerEffectiveSMSRate.CurrentRate != null ? customerEffectiveSMSRate.CurrentRate.Rate : (decimal?)null;
                        customerSMSRateChangesDetail.FutureRate = futureRate != null ? new SMSFutureRate() { Rate = futureRate.Rate, BED = futureRate.BED, EED = futureRate.EED } : null;
                    }
                }
            }
        }

        private CustomerSMSRateDraft MergeImportedDraft(CustomerSMSRateDraftToUpdate customerDraftToUpdate)
        {

            CustomerSMSRateDraft customerSMSRateChanges = customerDraftToUpdate.ProcessDraftID.HasValue ? GetCustomerSMSRateDraft(customerDraftToUpdate.ProcessDraftID.Value) : null;

            if (customerDraftToUpdate == null)
                return customerSMSRateChanges;

            if (customerSMSRateChanges == null || customerSMSRateChanges.SMSRates == null || customerSMSRateChanges.SMSRates.Count == 0)
            {
                return new CustomerSMSRateDraft()
                {
                    CustomerID = customerDraftToUpdate.CustomerID,
                    CurrencyId = customerDraftToUpdate.CurrencyId,
                    EffectiveDate = customerDraftToUpdate.EffectiveDate,
                    SMSRates = BuildCustomerSMSRateChangesMapper(customerDraftToUpdate.SMSRates),
                    Status = ProcessStatus.Draft
                };
            }

            customerSMSRateChanges.EffectiveDate = customerDraftToUpdate.EffectiveDate;
            customerSMSRateChanges.Status = ProcessStatus.Draft;

            Dictionary<int, CustomerSMSRateChange> oldCustomerSMSRates = customerSMSRateChanges.SMSRates;

            List<CustomerSMSRateChangeToUpdate> newCustomerSMSRates = customerDraftToUpdate.SMSRates;

            foreach (var newCustomerSMSRateHistory in newCustomerSMSRates)
            {
                int mobileNetworkID = newCustomerSMSRateHistory.MobileNetworkID;

                if (!oldCustomerSMSRates.ContainsKey(mobileNetworkID) && newCustomerSMSRateHistory.NewRate.HasValue)
                {
                    oldCustomerSMSRates.Add(mobileNetworkID, new CustomerSMSRateChange() { MobileNetworkID = mobileNetworkID, NewRate = newCustomerSMSRateHistory.NewRate.Value });
                    continue;
                }

                if (oldCustomerSMSRates.ContainsKey(mobileNetworkID) && newCustomerSMSRateHistory.NewRate.HasValue)
                {
                    oldCustomerSMSRates[mobileNetworkID] = new CustomerSMSRateChange() { MobileNetworkID = mobileNetworkID, NewRate = newCustomerSMSRateHistory.NewRate.Value };
                    continue;
                }

                oldCustomerSMSRates.Remove(mobileNetworkID);
            }

            customerSMSRateChanges.SMSRates = oldCustomerSMSRates;

            return customerSMSRateChanges;
        }

        private Dictionary<int, CustomerSMSRateChange> BuildCustomerSMSRateChangesMapper(List<CustomerSMSRateChangeToUpdate> customerSMSRatesChangeToUpdate)
        {
            if (customerSMSRatesChangeToUpdate == null)
                return null;

            Dictionary<int, CustomerSMSRateChange> customerSMSRateChanges = new Dictionary<int, CustomerSMSRateChange>();
            foreach (var customerSMSRateChangeToUpdate in customerSMSRatesChangeToUpdate)
            {
                if (customerSMSRateChangeToUpdate.NewRate.HasValue)
                    customerSMSRateChanges.Add(customerSMSRateChangeToUpdate.MobileNetworkID, new CustomerSMSRateChange()
                    {
                        NewRate = customerSMSRateChangeToUpdate.NewRate.Value,
                        MobileNetworkID = customerSMSRateChangeToUpdate.MobileNetworkID
                    });
            }

            return customerSMSRateChanges;
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
