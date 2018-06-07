using Demo.BestPractices.Data;
using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.BestPractices.Business
{
   public  class ChildManager
    {
       ParentManager _parentManager = new ParentManager();
       #region Public Methods
        public IDataRetrievalResult<ChildDetails> GetFilteredChilds(DataRetrievalInput<ChildQuery> input)
        {
            var allChilds = GetCachedChilds();
            Func<Child, bool> filterExpression = (child) =>
            {
                if (input.Query.Name != null && !child.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.ParentIds != null && !input.Query.ParentIds.Contains(child.ParentId))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allChilds.ToBigResult(input, filterExpression, ChildDetailMapper));

        }
        public IEnumerable<ChildShapeConfig> GetChildShapeConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ChildShapeConfig>(ChildShapeConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<ChildDetails> AddChild(Child child)
        {
            IChildDataManager childDataManager = BestPracticesFactory.GetDataManager<IChildDataManager>();
            InsertOperationOutput<ChildDetails> insertOperationOutput = new InsertOperationOutput<ChildDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long childId = -1;

            bool insertActionSuccess = childDataManager.Insert(child, out childId);
            if (insertActionSuccess)
            {
                child.ChildId = childId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ChildDetailMapper(child);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Child GetChildById(long childId)
        {
            var allChilds = GetCachedChilds();
            return allChilds.GetRecord(childId);
        }

        public UpdateOperationOutput<ChildDetails> UpdateChild(Child child)
        {
            IChildDataManager childDataManager = BestPracticesFactory.GetDataManager<IChildDataManager>();
            UpdateOperationOutput<ChildDetails> updateOperationOutput = new UpdateOperationOutput<ChildDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = childDataManager.Update(child);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ChildDetailMapper(child);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IChildDataManager childDataManager = BestPracticesFactory.GetDataManager<IChildDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return childDataManager.AreChildsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Child> GetCachedChilds()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedChilds", () =>
               {
                   IChildDataManager childDataManager = BestPracticesFactory.GetDataManager<IChildDataManager>();
                   List<Child> childs = childDataManager.GetChilds();
                   return childs.ToDictionary(child => child.ChildId, child => child);
               });
        }
        #endregion

        #region Mappers
        public ChildDetails ChildDetailMapper(Child child)
        {
            var childDetails = new ChildDetails
            {
                Name = child.Name,
                ChildId = child.ChildId,
                ParentName = _parentManager.GetParentName(child.ParentId),
            };

            if (child.Settings != null && child.Settings.ChildShape != null)
            {
                var context = new ChildShapeDescriptionContext
                {
                    Child = child
                };
                childDetails.AreaDescription = child.Settings.ChildShape.GetChildAreaDescription(context);
            }

            return childDetails;
        }
        #endregion 
    }
}
