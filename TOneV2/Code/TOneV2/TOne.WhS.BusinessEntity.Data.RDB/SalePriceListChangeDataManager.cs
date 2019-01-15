﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListChangeDataManager : ISalePriceListChangeDataManager
    {

        #region ISalePriceListChangeDataManager Members
        public List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds)
        {
            SalePricelistCodeChangeDataManager salePricelistCodeChangeDataManager = new SalePricelistCodeChangeDataManager();
            return salePricelistCodeChangeDataManager.GetSalePricelistCodeChanges(pricelistId, countryIds);
        }

        public List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds)
        {
            SalePricelistRateChangeDataManager salePricelistRateChangeDataManager = new SalePricelistRateChangeDataManager();
            return salePricelistRateChangeDataManager.GetSalePricelistRateChanges(pricelistId, countryIds);
        }

        public List<SalePricelistRPChange> GetFilteredSalePriceListRPChanges(int pricelistId, List<int> countryIds)
        {
            SalePricelistRPChangeDataManager salePricelistRpChangeDataManager = new SalePricelistRPChangeDataManager();
            return salePricelistRpChangeDataManager.GetSalePriceListRPChanges(pricelistId, countryIds);
        }

        public SalePriceListSnapShot GetSalePriceListSnapShot(int priceListId)
        {
            SalePriceListSnapShotDataManager salePriceListSnapShotDataManager = new SalePriceListSnapShotDataManager();
            return salePriceListSnapShotDataManager.GetSalePriceListSnapShot(priceListId);
        }

        public void SaveCustomerChangesToDb(IEnumerable<SalePriceListCustomerChange> salePriceLists)
        {
            throw new NotImplementedException();
        }

        public void SaveCustomerCodeChangesToDb(IEnumerable<SalePricelistCodeChange> codeChanges)
        {
            throw new NotImplementedException();
        }

        public void SaveCustomerRoutingProductChangesToDb(IEnumerable<SalePricelistRPChange> routingProductChanges, long processInstanceId)
        {
            throw new NotImplementedException();
        }

        public void SaveCustomerRateChangesToDb(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId)
        {
            throw new NotImplementedException();
        }

        public void SaveSalePriceListSnapshotToDb(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots)
        {
            throw new NotImplementedException();
        }

        public List<SalePriceListNew> GetTemporaryPriceLists(TemporarySalePriceListQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerRatePreview> GetCustomerRatePreviews(CustomerRatePreviewQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ZoneCustomerPair> GetCustomerRatePreviewZonePairs(CustomerRatePreviewQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RoutingProductPreview> GetRoutingProductPreviews(RoutingProductPreviewQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetAffectedCustomerIdsRPChangesByProcessInstanceId(long processInstanceId)
        {
            SalePricelistRPChangeNewDataManager salePricelistRpChangeNewDataManager = new SalePricelistRPChangeNewDataManager();
            return salePricelistRpChangeNewDataManager.GetAffectedCustomerIds(processInstanceId);
        }

        public IEnumerable<int> GetAffectedCustomerIdsNewCountryChangesByProcessInstanceId(long processInstanceId)
        {
            //need to implement it in Sales - RP
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetAffectedCustomerIdsRateChangesByProcessInstanceId(long processInstanceId)
        {
            SalePricelistRateChangeNewDataManager salePricelistRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            return salePricelistRateChangeNewDataManager.GetDistinctAffectedCustomerIds(processInstanceId);
        }

        public bool AreSalePriceListCodeSnapShotUpdated(ref object updateHandle)
        {
            SalePriceListSnapShotDataManager salePriceListSnapShotDataManager = new SalePriceListSnapShotDataManager();
            return salePriceListSnapShotDataManager.AreSalePriceListCodeSnapShotUpdated(ref updateHandle);
        }

        public bool DoCustomerTemporaryPricelistsExists(long processInstanceId)
        {
            SalePriceListNewDataManager salePriceListNewDataManager = new SalePriceListNewDataManager();
            var salePriceListNew = salePriceListNewDataManager.GetSalePriceListNew(processInstanceId);
            return salePriceListNew != null;
        }
        #endregion

        #region Not Used Functions

        public List<SalePricelistRateChange> GetNotSentRatechanges(IEnumerable<int> customerIds)
        {
            throw new NotImplementedException();
        }
        public List<SalePricelistCodeChange> GetNotSentCodechanges(IEnumerable<int> customerIds)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<CustomerRatePreview> GetCustomerOtherRatePreviews(CustomerRatePreviewQuery query)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<int> GetAffectedCustomerIdsChangedCountryChangesByProcessInstanceId(long ProcessInstanceId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
