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
                        customerSMSRateChangesDetail.CurrentRate = customerEffectiveSMSRate.CurrentRate != null ? customerEffectiveSMSRate.CurrentRate.Rate : (decimal?) null;
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
        #endregion
    }
}
