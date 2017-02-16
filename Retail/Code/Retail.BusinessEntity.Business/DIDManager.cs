using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class DIDManager : IBusinessEntityManager
    {
        #region ctor/Local Variables

        Guid _accountDIDRelationDefinitionId;

        public DIDManager()
        {
            _accountDIDRelationDefinitionId = new ConfigManager().GetAccountDIDRelationDefinitionId();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DIDDetail> GetFilteredDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            var allDIDs = GetCachedDIDs();
            Func<DID, bool> filterExpression = (did) => (input.Query.Number == null || did.Number.ToLower().Contains(input.Query.Number.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDIDs.ToBigResult(input, filterExpression, DIDDetailMapper));
        }

        public DID GetDID(int didId)
        {
            var DIDs = GetCachedDIDs();
            return DIDs.GetRecord(didId);
        }

        public string GetDIDNumber(int didId)
        {
            var DIDs = GetCachedDIDs();
            DID did = DIDs.GetRecord(didId);
            if (did == null)
                throw new NullReferenceException(string.Format("DID ID {0}", didId));

            return did.Number;
        }

        public DID GetDIDByNumber(string number)
        {
            var dids = GetCachedDIDsGroupByNumber();

            if (dids == null)
                return null;

            return dids.GetRecord(number);
        }

        public InsertOperationOutput<DIDDetail> AddDID(DID did)
        {
            InsertOperationOutput<DIDDetail> insertOperationOutput = new InsertOperationOutput<DIDDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int didId = -1;

            IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
            bool insertActionSucc = dataManager.Insert(did, out didId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                did.DIDId = didId;
                insertOperationOutput.InsertedObject = DIDDetailMapper(did);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<DIDDetail> UpdateDID(DID did)
        {
            IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();

            bool updateActionSucc = dataManager.Update(did);
            UpdateOperationOutput<DIDDetail> updateOperationOutput = new UpdateOperationOutput<DIDDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DIDDetailMapper(did);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<DIDInfo> GetDIDsInfo(DIDFilter filter)
        {
            Func<DID, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (did) =>
                {
                    if (filter.Filters != null && !CheckIfFilterIsMatch(did, filter.Filters))
                        return false;

                    return true;
                };
            }

            return GetCachedDIDs().MapRecords(DIDInfoMapper, filterExpression);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, DID> GetCachedDIDsGroupByNumber()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDIDsGroupByNumber",
               () =>
               {
                   var dids = GetCachedDIDs();
                   return dids.ToDictionary(itm => itm.Value.Number, itm => itm.Value);
               });
        }

        private Dictionary<int, DID> GetCachedDIDs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllDIDs",
               () =>
               {
                   IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
                   IEnumerable<DID> DIDs = dataManager.GetAllDIDs();
                   return DIDs.ToDictionary(x => x.DIDId, x => x);
               });
        }

        private bool CheckIfFilterIsMatch(DID did, List<IDIDFilter> filters)
        {
            DIDFilterContext context = new DIDFilterContext { DID = did };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDIDDataManager _dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDIDsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers

        private DIDDetail DIDDetailMapper(DID did)
        {
            DIDDetail didDetail = new DIDDetail();
            didDetail.Entity = did;

            BEParentChildRelation beParentChildRelation = new BEParentChildRelationManager().GetParent(_accountDIDRelationDefinitionId, did.DIDId.ToString(), DateTime.Now);
            if (beParentChildRelation != null)
            {
                BEParentChildRelationDefinition beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
                string accountName = new BusinessEntityManager().GetEntityDescription(beParentChildRelationDefinition.Settings.ParentBEDefinitionId, long.Parse(beParentChildRelation.ParentBEId));
                didDetail.AccountName = accountName;
            }
            return didDetail;
        }

        private DIDInfo DIDInfoMapper(DID did)
        {
            return new DIDInfo()
            {
                DIDId = did.DIDId,
                Number = did.Number,
            };
        }

        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetCachedDIDs().Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetDID(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetDIDNumber(Int32.Parse(context.EntityId.ToString()));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var did = context.Entity as DID;
            return did.DIDId;
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
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
