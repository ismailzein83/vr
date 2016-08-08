using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;


namespace Vanrise.Common.Business
{
    public class VRMailMessageTypeManager
    {
        #region Public Methods

        public VRMailMessageType GetMailMessageType(Guid vrMailMessageTypeId)
        {
            Dictionary<Guid, VRMailMessageType> cachedVRMailMessageTypes = this.GetCachedVRMailMessageTypes();
            return cachedVRMailMessageTypes.GetRecord(vrMailMessageTypeId);
        }

        public IDataRetrievalResult<VRMailMessageTypeDetail> GetFilteredVRMailMessageTypes(DataRetrievalInput<VRMailMessageTypeQuery> input)
        {
            var allVRMailMessageTypes = GetCachedVRMailMessageTypes();
            Func<VRMailMessageType, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRMailMessageTypes.ToBigResult(input, filterExpression, VRMailMessageTypeDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRMailMessageTypeDetail> AddVRMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRMailMessageTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();

            vrMailMessageTypeItem.VRMailMessageTypeId = Guid.NewGuid();

            if (dataManager.Insert(vrMailMessageTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRMailMessageTypeDetailMapper(vrMailMessageTypeItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRMailMessageTypeDetail> UpdateVRMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRMailMessageTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();

            if (dataManager.Update(vrMailMessageTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRMailMessageTypeDetailMapper(this.GetMailMessageType(vrMailMessageTypeItem.VRMailMessageTypeId));
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
            IVRMailMessageTypeDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreMailMessageTypeUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, VRMailMessageType> GetCachedVRMailMessageTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRMailMessageTypes",
               () =>
               {
                   IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();
                   return dataManager.GetMailMessageTypes().ToDictionary(x => x.VRMailMessageTypeId, x => x);
               });
        }

        #endregion


        #region Mappers

        public VRMailMessageTypeDetail VRMailMessageTypeDetailMapper(VRMailMessageType vrMailMessageType)
        {
            VRMailMessageTypeDetail vrMailMessageTypeDetail = new VRMailMessageTypeDetail()
            {
                Entity = vrMailMessageType
            };
            return vrMailMessageTypeDetail;
        }

        #endregion
    }
}
