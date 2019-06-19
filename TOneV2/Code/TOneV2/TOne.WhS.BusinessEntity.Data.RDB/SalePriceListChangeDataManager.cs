﻿using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListChangeDataManager : ISalePriceListChangeDataManager
    {
        SalePricelistRateChangeDataManager _salePricelistRateChangeDataManager = new SalePricelistRateChangeDataManager();
        SalePricelistRPChangeDataManager _salePricelistRpChangeDataManager = new SalePricelistRPChangeDataManager();
        SalePriceListSnapShotDataManager _salePriceListSnapShotDataManager = new SalePriceListSnapShotDataManager();
        SalePricelistCustomerChangeNewDataManager _salePricelistCustomerChangeNewDataManager = new SalePricelistCustomerChangeNewDataManager();
        SalePricelistRateChangeNewDataManager _salePricelistRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
        SalePriceListNewDataManager _salePriceListNewDataManager = new SalePriceListNewDataManager();
        SalePricelistRPChangeNewDataManager _salePricelistRpChangeNewDataManager = new SalePricelistRPChangeNewDataManager();
        SalePricelistCodeChangeDataManager _salePricelistCodeChangeDataManager = new SalePricelistCodeChangeDataManager();

        #region ISalePriceListChangeDataManager Members
        public List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds)
        {
            return _salePricelistCodeChangeDataManager.GetSalePricelistCodeChanges(pricelistId, countryIds);
        }

        public List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds)
        {
            return _salePricelistRateChangeDataManager.GetSalePricelistRateChanges(pricelistId, countryIds);
        }

        public List<SalePricelistRPChange> GetFilteredSalePriceListRPChanges(int pricelistId, List<int> countryIds)
        {
            return _salePricelistRpChangeDataManager.GetSalePriceListRPChanges(pricelistId, countryIds);
        }

        public SalePriceListSnapShot GetSalePriceListSnapShot(int priceListId)
        {
            return _salePriceListSnapShotDataManager.GetSalePriceListSnapShot(priceListId);
        }

        public void SaveCustomerChangesToDb(IEnumerable<SalePriceListCustomerChange> salePriceListsCustomerChange)
        {
            if (salePriceListsCustomerChange == null || !salePriceListsCustomerChange.Any())
                return;

            _salePricelistCustomerChangeNewDataManager.Bulk(salePriceListsCustomerChange);
        }

        public void SaveCustomerCodeChangesToDb(IEnumerable<SalePricelistCodeChange> codeChanges)
        {
            SalePricelistCodeChangeNewDataManager salePricelistCodeChangeNewDataManager = new SalePricelistCodeChangeNewDataManager();
            salePricelistCodeChangeNewDataManager.Bulk(codeChanges);
        }

        public void SaveCustomerRoutingProductChangesToDb(IEnumerable<SalePricelistRPChange> routingProductChanges, long processInstanceId)
        {
            SalePricelistRPChangeNewDataManager salePricelistRpChangeNewDataManager = new SalePricelistRPChangeNewDataManager();
            salePricelistRpChangeNewDataManager.Bulk(routingProductChanges, processInstanceId);
        }

        public void SaveCustomerRateChangesToDb(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId)
        {
            _salePricelistRateChangeNewDataManager.Bulk(rateChanges, processInstanceId);
        }

        public void SaveSalePriceListSnapshotToDb(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots)
        {
            if (salePriceListSaleCodeSnapshots == null || !salePriceListSaleCodeSnapshots.Any())
                return;

            _salePriceListSnapShotDataManager.Bulk(salePriceListSaleCodeSnapshots);
        }

        public List<SalePriceListNew> GetTemporaryPriceLists(TemporarySalePriceListQuery query)
        {
            return _salePriceListNewDataManager.GeSalePriceListsNewByOwner((int)SalePriceListOwnerType.Customer, (int)SalePriceListType.None, query.ProcessInstanceId);
        }

        public IEnumerable<CustomerRatePreview> GetCustomerRatePreviews(CustomerRatePreviewQuery query)
        {
            SalePricelistRateChangeNewDataManager salePricelistRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            return salePricelistRateChangeNewDataManager.GetCustomerRatePreviews(query.ProcessInstanceId, query.CustomerIds);
        }

        public IEnumerable<ZoneCustomerPair> GetCustomerRatePreviewZonePairs(CustomerRatePreviewQuery query)
        {
            SalePricelistRateChangeNewDataManager salePricelistRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            return salePricelistRateChangeNewDataManager.GetCustomerRatePreviewZonePairs(query.ProcessInstanceId, query.CustomerIds);
        }

        public IEnumerable<RoutingProductPreview> GetRoutingProductPreviews(RoutingProductPreviewQuery query)
        {
            return _salePricelistRpChangeNewDataManager.GetSalePriceListRPChangeNewByCustomerId(query.ProcessInstanceId, query.CustomerIds);
        }

        public IEnumerable<int> GetAffectedCustomerIdsRPChangesByProcessInstanceId(long processInstanceId)
        {
            return _salePricelistRpChangeNewDataManager.GetAffectedCustomerIds(processInstanceId);
        }

        public IEnumerable<int> GetAffectedCustomerIdsRateChangesByProcessInstanceId(long processInstanceId)
        {
            return _salePricelistRateChangeNewDataManager.GetDistinctAffectedCustomerIds(processInstanceId);
        }

        public bool AreSalePriceListCodeSnapShotUpdated(ref object updateHandle)
        {
            return _salePriceListSnapShotDataManager.AreSalePriceListCodeSnapShotUpdated(ref updateHandle);
        }

        public bool DoCustomerTemporaryPricelistsExists(long processInstanceId)
        {
            return _salePriceListNewDataManager.HasPriceListByProcessInstanceId(processInstanceId);
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

        #endregion
    }
}
