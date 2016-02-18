using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class OperatorDeclaredInformationManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<OperatorDeclaredInformationDetail> GetFilteredOperatorDeclaredInformations(Vanrise.Entities.DataRetrievalInput<OperatorDeclaredInformationQuery> input)
        {
            var allOperatorDeclaredInformations = GetCachedOperatorDeclaredInformations();

            Func<OperatorDeclaredInformation, bool> filterExpression = (prod) =>
                 (input.Query.FromDate == null || input.Query.FromDate < prod.FromDate)
                 &&

                 (input.Query.ToDate == null || input.Query.ToDate >= prod.ToDate)
                 &&

                 (input.Query.OperatorIds == null || input.Query.OperatorIds.Count == 0 || input.Query.OperatorIds.Contains(prod.OperatorId))
                 &&

                 (input.Query.ZoneIds == null || input.Query.ZoneIds.Count == 0 || (prod.ZoneId.HasValue && input.Query.ZoneIds.Contains(prod.ZoneId.Value)))
                 &&
                 (input.Query.OperatorDeclaredInformationIds == null || input.Query.OperatorDeclaredInformationIds.Contains(prod.OperatorDeclaredInformationId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorDeclaredInformations.ToBigResult(input, filterExpression, OperatorDeclaredInformationDetailMapper));
        }
        public OperatorDeclaredInformation GetOperatorDeclaredInformation(int operatorDeclaredInformationId)
        {
            var info = GetCachedOperatorDeclaredInformations();
            return info.GetRecord(operatorDeclaredInformationId);
        }
        public Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail> AddOperatorDeclaredInformation(OperatorDeclaredInformation operatorDeclaredInformation)
        {
            Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int infoId = -1;

            IOperatorDeclaredInformationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();
            bool insertActionSucc = dataManager.Insert(operatorDeclaredInformation, out infoId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                operatorDeclaredInformation.OperatorDeclaredInformationId = infoId;
                insertOperationOutput.InsertedObject = OperatorDeclaredInformationDetailMapper(operatorDeclaredInformation);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail> UpdateOperatorDeclaredInformation(OperatorDeclaredInformation operatorDeclaredInformation)
        {
            IOperatorDeclaredInformationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();

            bool updateActionSucc = dataManager.Update(operatorDeclaredInformation);
            Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OperatorDeclaredInformationDetailMapper(operatorDeclaredInformation);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, OperatorDeclaredInformation> GetCachedOperatorDeclaredInformations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorDeclaredInformations",
               () =>
               {
                   IOperatorDeclaredInformationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();
                   IEnumerable<OperatorDeclaredInformation> info = dataManager.GetOperatorDeclaredInformations();
                   return info.ToDictionary(cn => cn.OperatorDeclaredInformationId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorDeclaredInformationDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorDeclaredInformationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorDeclaredInformationsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers

        private OperatorDeclaredInformationDetail OperatorDeclaredInformationDetailMapper(OperatorDeclaredInformation info)
        {
            OperatorDeclaredInformationDetail infoDetail = new OperatorDeclaredInformationDetail();
            infoDetail.Entity = info;

            OperatorProfileManager operatorProfileManager = new OperatorProfileManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            ServiceTypeManager serviceTypeManager = new ServiceTypeManager();

            infoDetail.OperatorName = operatorProfileManager.GetOperatorProfile(info.OperatorId).Name;
            if (info.ZoneId.HasValue)
                infoDetail.ZoneName = saleZoneManager.GetSaleZoneName(info.ZoneId.Value);

            infoDetail.AmountTypeName = serviceTypeManager.GetServiceType(info.AmountType).Description;
            return infoDetail;
        }
        #endregion
    }

}
