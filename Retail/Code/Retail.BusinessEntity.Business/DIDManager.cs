using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class DIDManager 
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

        public DID GetDID(int dIDId)
        {
            var DIDs = GetCachedDIDs();
            return DIDs.GetRecord(dIDId);
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

       
        
        public IEnumerable<DIDInfo> GetDIDsInfo()
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

       
    }
}
