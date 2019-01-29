using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Data.RDB;
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
        IProcessDraftDataManager _processDraftDataManager = ProcessDraftDataManagerFactory.GetDataManager<IProcessDraftDataManager>();

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CustomerSMSRateChangesDetail> GetFilteredChanges(DataRetrievalInput<CustomerSMSRateChangesQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerSMSRateChangesRequestHandler());
        }

        public DraftStateResult InsertOrUpdateChanges(CustomerSMSRateDraftToUpdate customerDraftToUpdate)
        {
            if (customerDraftToUpdate.SMSRates == null)
                return null;

            CustomerSMSRateDraft customerSMSRateDraft = MergeImportedDraft(customerDraftToUpdate);

            string serializedChanges = customerSMSRateDraft != null ? Serializer.Serialize(customerSMSRateDraft) : null;

            int? result;
            bool isInsertedOrUpdated = _processDraftDataManager.InsertOrUpdateChanges(ProcessEntityType.Customer, serializedChanges, customerSMSRateDraft.CustomerID.ToString(), SecurityContext.Current.GetLoggedInUserId(), out result);

            return isInsertedOrUpdated ? new DraftStateResult() { ProcessDraftID = result.Value } : null;
        }

        public bool UpdateSMSRateChangesStatus(DataRetrievalInput<UpdateCustomerSMSDraftStatusInput> input)
        {
            if (input != null && input.Query != null)
                return _processDraftDataManager.UpdateProcessStatus(ProcessEntityType.Customer, input.Query.ProcessDraftID, input.Query.NewStatus, SecurityContext.Current.GetLoggedInUserId());

            return false;
        }

        public DraftData GetDraftData(DataRetrievalInput<CustomerDraftDataInput> input)
        {
            if (input != null && input.Query != null)
            {
                return new DraftData()
                {
                    EffectiveDate = GetEffectiveDate(input.Query.CustomerID),
                    CountryLetters = new CustomerSMSRateManager().GetMobileCountryLetters()
                };
            }

            return null;
        }

        public DraftStateResult CheckIfDraftExist(int customerID)
        {
            return _processDraftDataManager.CheckIfDraftExist(ProcessEntityType.Customer, customerID.ToString());
        }

        #endregion

        #region Private Methods 
        private CustomerSMSRateDraft MergeImportedDraft(CustomerSMSRateDraftToUpdate customerDraftToUpdate)
        {
            CustomerSMSRateDraft customerSMSRateChanges = GetCustomerSMSRateChanges(customerDraftToUpdate.CustomerID);

            if (customerDraftToUpdate == null || customerDraftToUpdate.SMSRates == null)
                return customerSMSRateChanges;

            if (customerSMSRateChanges == null || customerSMSRateChanges.SMSRates == null)
            {
                return new CustomerSMSRateDraft()
                {
                    CustomerID = customerDraftToUpdate.CustomerID,
                    EffectiveDate = customerDraftToUpdate.EffectiveDate,
                    SMSRates = OnUpdateCustomerSMSRateChangeMapper(customerDraftToUpdate.SMSRates),
                    Status = ProcessStatus.Draft
                };
            }

            customerSMSRateChanges.EffectiveDate = customerDraftToUpdate.EffectiveDate;
            customerSMSRateChanges.Status = ProcessStatus.Draft;

            List<CustomerSMSRateChange> oldCustomerSMSRateHistories = customerSMSRateChanges.SMSRates;

            List<CustomerSMSRateChangeToUpdate> newCustomerSMSRateHistories = customerDraftToUpdate.SMSRates;
            List<int> oldCustomerSMSRateHistoryIds = oldCustomerSMSRateHistories.Select(item => item.MobileNetworkID).ToList();

            foreach (var newCustomerSMSRateHistory in newCustomerSMSRateHistories)
            {
                int oldCustomerSMSRateIndex;

                if (!DoesMobileNetworkExist(oldCustomerSMSRateHistoryIds, newCustomerSMSRateHistory.MobileNetworkID, out oldCustomerSMSRateIndex))
                {
                    if (newCustomerSMSRateHistory.NewRate.HasValue)
                        oldCustomerSMSRateHistories.Add(new CustomerSMSRateChange() { MobileNetworkID = newCustomerSMSRateHistory.MobileNetworkID, NewRate = newCustomerSMSRateHistory.NewRate.Value });
                }
                else if (newCustomerSMSRateHistory.NewRate.HasValue)
                {
                    oldCustomerSMSRateHistories[oldCustomerSMSRateIndex] = new CustomerSMSRateChange() { MobileNetworkID = newCustomerSMSRateHistory.MobileNetworkID, NewRate = newCustomerSMSRateHistory.NewRate.Value };
                }
                else
                {
                    oldCustomerSMSRateHistories.RemoveAt(oldCustomerSMSRateIndex);
                }
            }

            customerSMSRateChanges.SMSRates = oldCustomerSMSRateHistories;

            return customerSMSRateChanges;
        }

        private List<CustomerSMSRateChange> OnUpdateCustomerSMSRateChangeMapper(List<CustomerSMSRateChangeToUpdate> customerSMSRatesChangeToUpdate)
        {
            if (customerSMSRatesChangeToUpdate == null)
                return null;

            List<CustomerSMSRateChange> customerSMSRateChanges = new List<CustomerSMSRateChange>();
            foreach (var customerSMSRateChangeToUpdate in customerSMSRatesChangeToUpdate)
            {
                if (customerSMSRateChangeToUpdate.NewRate.HasValue)
                    customerSMSRateChanges.Add(new CustomerSMSRateChange()
                    {
                        NewRate = customerSMSRateChangeToUpdate.NewRate.Value,
                        MobileNetworkID = customerSMSRateChangeToUpdate.MobileNetworkID
                    });
            }

            return customerSMSRateChanges;
        }

        private bool DoesMobileNetworkExist(List<int> rateHistoryMobileNetworkIDs, int mobileNetworkID, out int indexToOut)
        {
            indexToOut = -1;
            for (int i = 0; i < rateHistoryMobileNetworkIDs.Count; i++)
            {
                if (rateHistoryMobileNetworkIDs[i] == mobileNetworkID)
                {
                    indexToOut = i;
                    return true;
                }
            }

            return false;
        }

        private DateTime GetEffectiveDate(int customerID)
        {
            CustomerSMSRateDraft customerSMSRateChanges = GetCustomerSMSRateChanges(customerID);
            if (customerSMSRateChanges != null && customerSMSRateChanges.EffectiveDate != null)
                return customerSMSRateChanges.EffectiveDate;

            return DateTime.Today;
        }

        private CustomerSMSRateChangesDetail CustomerSMSRateDetailMapper(CustomerSMSRateChange customerSMSRate)
        {
            if (customerSMSRate == null)
                return null;

            return new CustomerSMSRateChangesDetail()
            {
                MobileCountryName = GetMobileCountryName(customerSMSRate.MobileNetworkID),
                MobileNetworkName = GetMobileNetworkName(customerSMSRate.MobileNetworkID),
                CurrentRate = customerSMSRate.CurrentRate,
                NewRate = customerSMSRate.NewRate,
                MobileNetworkID = customerSMSRate.MobileNetworkID,
                HasFutureRate = customerSMSRate.HasFutureRate,
                FutureRate = customerSMSRate.FutureRate
            };
        }

        private CustomerSMSRateDraft GetCustomerSMSRateChanges(int customerID)
        {
            ProcessDraft processDraft = _processDraftDataManager.GetChanges(ProcessEntityType.Customer, customerID.ToString());
            if (processDraft == null || processDraft.Changes == null)
                return null;

            return !string.IsNullOrEmpty(processDraft.Changes) ? Serializer.Deserialize<CustomerSMSRateDraft>(processDraft.Changes) : null;
        }

        private string GetMobileCountryName(int id)
        {
            return id.ToString() + "_CountryName";
        }

        private string GetMobileNetworkName(int id)
        {
            return id.ToString() + "_NetworkName";
        }

        #endregion

        #region Private Classes

        private class CustomerSMSRateChangesRequestHandler : BigDataRequestHandler<CustomerSMSRateChangesQuery, CustomerSMSRateChange, CustomerSMSRateChangesDetail>
        {
            CustomerSMSRateDraftManager _manager = new CustomerSMSRateDraftManager();
            public override CustomerSMSRateChangesDetail EntityDetailMapper(CustomerSMSRateChange entity)
            {
                return _manager.CustomerSMSRateDetailMapper(entity);
            }

            public override IEnumerable<CustomerSMSRateChange> RetrieveAllData(DataRetrievalInput<CustomerSMSRateChangesQuery> input)
            {
                if (input != null && input.Query != null)
                {
                    List<CustomerSMSRate> mobileNetworkRates = new CustomerSMSRateManager().GetMobileNetworkRates(input.Query.CustomerID); // Rates
                    Dictionary<string, List<MobileNetwork>> mobileNetworksByCountryNames = GetMobileNetworksByCountryNames(input); // MN
                    CustomerSMSRateDraft customerSMSRateChanges = _manager.GetCustomerSMSRateChanges(input.Query.CustomerID); // Draft

                    //List<CustomerSMSRateChangesDetail> customerSMSRateChangeDetails = mobileNetworkRates.Select(item => ConvertMobileNetworkToSMSRateChangeDetail(item)).ToList();



                   // return MergeWithFilterExportedDraft(customerSMSRateChanges, input.Query);
                }

                return null;
            }

            private Dictionary<string, List<MobileNetwork>> GetMobileNetworksByCountryNames(DataRetrievalInput<CustomerSMSRateChangesQuery> input)
            {
                string countryCharToString = string.Empty;
                if (input.Query.CountryChar.HasValue)
                {
                    countryCharToString = input.Query.CountryChar.Value.ToString().ToUpper();
                    countryCharToString = countryCharToString.Replace("\0", string.Empty);
                }
                Dictionary<string, List<MobileNetwork>> mobileNetworksByCountryNames = null;// new MobileNetworkManager().GetAllMobileNetworksByMobileCountryNames(countryCharToString);
                if (mobileNetworksByCountryNames == null)
                    return null;

                return (Dictionary<string, List<Vanrise.MobileNetwork.Entities.MobileNetwork>>)mobileNetworksByCountryNames.VROrderList(input);
            }

            //private List<CustomerSMSRateChangesDetail> GetCustomerSMSRateChangesDetails(DataRetrievalInput<CustomerSMSRateChangesQuery> input)
            //{

            //}

            //private List<CustomerSMSRateChange> MergeWithFilterExportedDraft(CustomerSMSRateDraft draftCustomerSMSChanges, CustomerSMSRateChangesQuery customerSMSRateChangesQuery)
            //{

            //    if (draftCustomerSMSChanges == null || draftCustomerSMSChanges.SMSRates == null)
            //        return mobileNetworkRates;

            //    if (mobileNetworkRates != null)
            //    {
            //        List<CustomerSMSRateChange> draftCustomerSMSRates = draftCustomerSMSChanges.SMSRates;

            //        foreach (var customerSMSRate in mobileNetworkRates)
            //        {
            //            CustomerSMSRateChange foundedDraftCustomerSMS = draftCustomerSMSRates.Find(item => item.MobileNetworkID == customerSMSRate.MobileNetworkID);
            //            if (foundedDraftCustomerSMS != null)
            //                customerSMSRate.NewRate = foundedDraftCustomerSMS.NewRate;
            //        }

            //        return mobileNetworkRates;
            //    }

            //    return null;
            //}

            private CustomerSMSRateChangesDetail ConvertMobileNetworkToSMSRateChangeDetail(CustomerSMSRate customerSMSRate)
            {
                return new CustomerSMSRateChangesDetail()
                {
                    CurrentRate = customerSMSRate.Rate,
                    FutureRate = customerSMSRate.FutureRate,
                    MobileNetworkID = customerSMSRate.MobileNetworkID
                };
            }

        }
    }

    #endregion
}
