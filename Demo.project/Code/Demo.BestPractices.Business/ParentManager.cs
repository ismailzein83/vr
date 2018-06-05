using Demo.BestPractices.Data;
using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.BestPractices.Business
{
    public class ParentManager
    {
        
        #region Public Methods
        public IDataRetrievalResult<ParentDetails> GetFilteredParents(DataRetrievalInput<ParentQuery> input)
        {
            var allParents = GetCachedParents();
            Func<Parent, bool> filterExpression = (parent) =>
            {
                if (input.Query.Name != null && !parent.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
               
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allParents.ToBigResult(input, filterExpression, ParentDetailMapper));

        }
        public string GetParentName(long parentId)
        {
            var parent = GetParentById(parentId);
            if (parent == null)
                return null;
            return parent.Name;
        }
        public InsertOperationOutput<ParentDetails> AddParent(Parent parent)
        {
            IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
            InsertOperationOutput<ParentDetails> insertOperationOutput = new InsertOperationOutput<ParentDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long parentId = -1;

            bool insertActionSuccess = parentDataManager.Insert(parent, out parentId);
            if (insertActionSuccess)
            {
                parent.ParentId = parentId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ParentDetailMapper(parent);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Parent GetParentById(long parentId)
        {
            return GetCachedParents().GetRecord(parentId);
        }
        public IEnumerable<ParentInfo> GetParentsInfo(ParentInfoFilter parentInfoFilter)
        {
            var allParents = GetCachedParents();
            Func<Parent, bool> filterFunc = (parent) =>
            {
                if (parentInfoFilter != null)
                {
                    if(parentInfoFilter.Filters != null)
                    {
                        var context = new ParentInfoFilterContext{
                            ParentId = parent.ParentId
                        };
                        foreach(var filter in parentInfoFilter.Filters)
                        {
                            if(!filter.IsMatch(context))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            };
            return allParents.MapRecords(ParentInfoMapper, filterFunc).OrderBy(parent => parent.Name);
        }
        public UpdateOperationOutput<ParentDetails> UpdateParent(Parent parent)
        {
            IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
            UpdateOperationOutput<ParentDetails> updateOperationOutput = new UpdateOperationOutput<ParentDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = parentDataManager.Update(parent);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ParentDetailMapper(parent);
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
            IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return parentDataManager.AreParentsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Parent> GetCachedParents()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedParents", () =>
               {
                   IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
                   List<Parent> parents = parentDataManager.GetParents();
                   return parents.ToDictionary(parent => parent.ParentId, parent => parent);
               });
        }
        #endregion

        #region Mappers
        public ParentDetails ParentDetailMapper(Parent parent)
        {
            return new ParentDetails
            {
                Name = parent.Name,
                ParentId = parent.ParentId
            };
        }

        public ParentInfo ParentInfoMapper(Parent parent)
        {
            return new ParentInfo
            {
                Name = parent.Name,
                ParentId = parent.ParentId
            };
        }
        #endregion 

    }
}
