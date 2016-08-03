using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Data;
using Vanrise.GenericData.Entities;


namespace Retail.BusinessEntity.Business
{
    public class CreditClassManager : IBusinessEntityManager
    {
        #region Public Methods

        public IDataRetrievalResult<CreditClassDetail> GetFilteredCreditClasses(DataRetrievalInput<CreditClassQuery> input)
        {
            var allCreditClass = this.GetCachedCreditClasses();
            Func<CreditClass, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCreditClass.ToBigResult(input, filterExpression, CreditClassDetailMapper));
        }

        public CreditClass GetCreditClass(int creditClassId)
        {
            Dictionary<int, CreditClass> cachedCreditClass = this.GetCachedCreditClasses();
            return cachedCreditClass.GetRecord(creditClassId);
        }

        public Vanrise.Entities.InsertOperationOutput<CreditClassDetail> AddCreditClass(CreditClass creditClassItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CreditClassDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int creditClassId = -1;

            ICreditClassDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICreditClassDataManager>();

            if (_dataManager.Insert(creditClassItem, out creditClassId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                creditClassItem.CreditClassId = creditClassId;
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

        public IEnumerable<CreditClassInfo> GetCreditClassesInfo(CreditClassFilter filter)
        {
            Func<CreditClass, bool> filterExpression = null;

            return this.GetCachedCreditClasses().MapRecords(CreditClassInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public string GetCreditClassName(int creditClassId)
        {
            CreditClass creditClass = this.GetCreditClass(creditClassId);
            return (creditClass != null) ? creditClass.Name : null;
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

        Dictionary<int, CreditClass> GetCachedCreditClasses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCreditClasses",
               () =>
               {
                   ICreditClassDataManager dataManager = BEDataManagerFactory.GetDataManager<ICreditClassDataManager>();
                   return dataManager.GetCreditClasses().ToDictionary(x => x.CreditClassId, x => x);
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

        public CreditClassInfo CreditClassInfoMapper(CreditClass creditClass)
        {
            CreditClassInfo creditClassInfo = new CreditClassInfo()
            {
                CreditClassId = creditClass.CreditClassId,
                Name = creditClass.Name
           };
            return creditClassInfo;
        }

        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedCreditClasses = GetCachedCreditClasses();
            if (cachedCreditClasses != null)
                return cachedCreditClasses.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCreditClass(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCreditClassName(Convert.ToInt32(context.EntityId));
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
             throw new NotImplementedException();
        }

        #endregion

    }
}
