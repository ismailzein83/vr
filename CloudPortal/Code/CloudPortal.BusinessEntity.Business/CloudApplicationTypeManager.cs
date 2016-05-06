using CloudPortal.BusinessEntity.Data;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudApplicationTypeManager
    {
        #region public methods
        public Vanrise.Entities.IDataRetrievalResult<CloudApplicationTypeDetail> GetFilteredCloudApplicationTypes(Vanrise.Entities.DataRetrievalInput<CloudApplicationTypeQuery> input)
        {
            var allcloudApplicationTypes = GetCachedCloudApplicationTypes();

            Func<CloudApplicationType, bool> filterExpression = (prod) =>
                (input.Query == null || string.IsNullOrEmpty(input.Query.Name) || string.Compare(input.Query.Name, prod.Name, true) == 0);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allcloudApplicationTypes.ToBigResult(input, filterExpression, CloudApplicationTypeDetailMapper));
        }

        public CloudApplicationType GetCloudApplicationType(int cloudApplicationTypeId)
        {
            return GetCachedCloudApplicationTypes().First(itm => itm.CloudApplicationTypeId == cloudApplicationTypeId);
        }

        public Vanrise.Entities.InsertOperationOutput<CloudApplicationType> AddCloudApplicationType(CloudApplicationType cloudApplicationType)
        {
            InsertOperationOutput<CloudApplicationType> insertOperationOutput = new InsertOperationOutput<CloudApplicationType>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int cloudApplicationTypeId = -1;

            ICloudApplicationTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTypeDataManager>();
            bool insertActionSucc = dataManager.AddCloudApplicationType(cloudApplicationType, out cloudApplicationTypeId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                cloudApplicationType.CloudApplicationTypeId = cloudApplicationTypeId;
                insertOperationOutput.InsertedObject = cloudApplicationType;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<CloudApplicationType> UpdateCloudApplicationType(CloudApplicationType cloudApplicationType)
        {
            ICloudApplicationTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTypeDataManager>();
            bool updateActionSucc = dataManager.UpdateCloudApplicationType(cloudApplicationType);
            UpdateOperationOutput<CloudApplicationType> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CloudApplicationType>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = cloudApplicationType;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<CloudApplicationType> GetCloudApplicationTypesInfo(CloudApplicationTypeFilter filter)
        {
            var cloudApplicationTypes = GetCachedCloudApplicationTypes();

            if (filter != null)
            {
                Func<CloudApplicationType, bool> filterExpression = (x) => (true);
                return cloudApplicationTypes.FindAllRecords(filterExpression);
            }
            else
            {
                return cloudApplicationTypes;
            }
        }
        #endregion

        #region private methods

        private List<CloudApplicationType> GetCachedCloudApplicationTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCloudApplicationTypes",
            () =>
            {
                ICloudApplicationTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTypeDataManager>();
                return dataManager.GetAllCloudApplicationTypes();
            });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICloudApplicationTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreCloudApplicationTypesUpdated(ref _updateHandle);
            }
        }

        private CloudApplicationTypeDetail CloudApplicationTypeDetailMapper(CloudApplicationType cloudApplicationType)
        {
            return new CloudApplicationTypeDetail()
            {
                Entity = cloudApplicationType
            };
        }
        #endregion
    }
}
