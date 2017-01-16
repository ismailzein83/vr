using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class DIDManager : IBusinessEntityManager
    {
        #region ctor/Local Variables

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

        public InsertOperationOutput<DIDDetail> AddDID(DID dID)
        {
            InsertOperationOutput<DIDDetail> insertOperationOutput = new InsertOperationOutput<DIDDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dIDId = -1;

            IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
            bool insertActionSucc = dataManager.Insert(dID, out dIDId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dID.DIDId = dIDId;
                insertOperationOutput.InsertedObject = DIDDetailMapper(dID);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<DIDDetail> UpdateDID(DID dID)
        {
            IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();

            bool updateActionSucc = dataManager.Update(dID);
            UpdateOperationOutput<DIDDetail> updateOperationOutput = new UpdateOperationOutput<DIDDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DIDDetailMapper(dID);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<DIDInfo> GetDIDsInfo(DIDFilter didFilter)
        {
            var DIDs = GetCachedDIDs();
            return DIDs.MapRecords(DIDInfoMapper);
        }


        #endregion

        #region Private Methods

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

        private DIDInfo DIDInfoMapper(DID dID)
        {
            return new DIDInfo()
            {
                DIDId = dID.DIDId,
                Number = dID.Number,
            };
        }

        private DIDDetail DIDDetailMapper(DID dID)
        {
            DIDDetail dIDDetail = new DIDDetail();
            dIDDetail.Entity = dID;
            return dIDDetail;
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
