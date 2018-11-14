using Demo.BestPractices.Data;
using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.BestPractices.Business
{
    public class ParentManager
    {
        #region Public Methods

        public IDataRetrievalResult<ParentDetail> GetFilteredParents(DataRetrievalInput<ParentQuery> input)
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

        public Parent GetParentById(long parentId)
        {
            return GetCachedParents().GetRecord(parentId);
        }

        public string GetParentName(long parentId)
        {
            var parent = GetParentById(parentId);
            return parent != null ? parent.Name : null;
        }

        public IEnumerable<ParentInfo> GetParentsInfo(ParentInfoFilter parentInfoFilter)
        {
            var allParents = GetCachedParents();
            Func<Parent, bool> filterFunc = (parent) =>
            {
                if (parentInfoFilter != null)
                {
                    if (parentInfoFilter.Filters != null)
                    {
                        var context = new ParentInfoFilterContext { ParentId = parent.ParentId };

                        foreach (var filter in parentInfoFilter.Filters)
                        {
                            if (!filter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return allParents.MapRecords(ParentInfoMapper, filterFunc).OrderBy(parent => parent.Name);
        }

        public InsertOperationOutput<ParentDetail> AddParent(Parent parent)
        {
            InsertOperationOutput<ParentDetail> insertOperationOutput = new InsertOperationOutput<ParentDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long parentId = -1;

            IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
            bool insertActionSuccess = parentDataManager.Insert(parent, out parentId);
            if (insertActionSuccess)
            {
                parent.ParentId = parentId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.ParentDetailMapper(parent);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<ParentDetail> UpdateParent(Parent parent)
        {
            UpdateOperationOutput<ParentDetail> updateOperationOutput = new UpdateOperationOutput<ParentDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
            bool updateActionSuccess = parentDataManager.Update(parent);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = this.ParentDetailMapper(parent);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<long, Parent> GetCachedParents()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedParents", () =>
                    {
                        IParentDataManager parentDataManager = BestPracticesFactory.GetDataManager<IParentDataManager>();
                        List<Parent> parents = parentDataManager.GetParents();
                        return parents.ToDictionary(parent => parent.ParentId, parent => parent);
                    });
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

        #region Mappers

        private ParentDetail ParentDetailMapper(Parent parent)
        {
            return new ParentDetail
            {
                Name = parent.Name,
                ParentId = parent.ParentId
            };
        }

        private ParentInfo ParentInfoMapper(Parent parent)
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