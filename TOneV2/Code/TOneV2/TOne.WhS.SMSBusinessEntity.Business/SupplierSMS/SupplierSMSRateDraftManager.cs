using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
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
        #endregion
    }
}
