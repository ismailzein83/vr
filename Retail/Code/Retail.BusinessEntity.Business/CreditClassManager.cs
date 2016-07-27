using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;


namespace Retail.BusinessEntity.Business
{
    public class CreditClassManager
    {
        #region Public Methods

        public IDataRetrievalResult<CreditClassDetail> GetFilteredCreditClasss(DataRetrievalInput<CreditClassQuery> input)
        {
            var allCreditClasss = this.GetCachedCreditClasss();
            Func<CreditClass, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCreditClasss.ToBigResult(input, filterExpression, CreditClassDetailMapper));
        }

        public CreditClass GetCreditClass(int creditClassId)
        {
            Dictionary<int, CreditClass> cachedCreditClasss = this.GetCachedCreditClasss();
            return cachedCreditClasss.GetRecord(creditClassId);
        }

        public Vanrise.Entities.InsertOperationOutput<CreditClassDetail> AddCreditClass(CreditClass creditClassItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CreditClassDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int reprocessDefintionId = -1;

            ICreditClassDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICreditClassDataManager>();

            if (_dataManager.Insert(creditClassItem, out reprocessDefintionId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                creditClassItem.CreditClassId = reprocessDefintionId;
                insertOperationOutput.InsertedObject = CreditClassDetailMapper(creditClassItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<CreditClassDetail> UpdateCreditClass(CreditClass creditClassItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CreditClassDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ICreditClassDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICreditClassDataManager>();

            if (_dataManager.Update(creditClassItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CreditClassDetailMapper(this.GetCreditClass(creditClassItem.CreditClassId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICreditClassDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICreditClassDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCreditClassUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<int, CreditClass> GetCachedCreditClasss()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCreditClasss",
               () =>
               {
                   ICreditClassDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<ICreditClassDataManager>();
                   return dataManager.GetCreditClass().ToDictionary(x => x.CreditClassId, x => x);
               });
        }

        #endregion


        #region Mappers

        public CreditClassDetail CreditClassDetailMapper(CreditClass creditClass)
        {
            CreditClassDetail creditClassDetail = new CreditClassDetail()
            {
                Entity = creditClass
            };
            return creditClassDetail;
        }

        #endregion
    }
}
