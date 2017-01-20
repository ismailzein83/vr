using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using System.Collections.Concurrent;

namespace Vanrise.GenericData.Business
{
    public class BEParentChildRelationManager
    {
        #region Public Methods

        public IDataRetrievalResult<BEParentChildRelationDetail> GetFilteredBEParentChildRelations(DataRetrievalInput<BEParentChildRelationQuery> input)
        {
            var allBEParentChildRelation = this.GetCachedBEParentChildRelations(input.Query.RelationDefinitionId);

            Func<BEParentChildRelation, bool> filterExpression = null;
            if (input.Query != null)
            {
                filterExpression = (beParentChildRelation) =>
                 {
                     if (input.Query.RelationDefinitionId != null && input.Query.RelationDefinitionId != beParentChildRelation.RelationDefinitionId)
                         return false;

                     if (input.Query.ParentBEId != null && input.Query.ParentBEId != beParentChildRelation.ParentBEId)
                         return false;

                     if (input.Query.ChildBEId != null && input.Query.ChildBEId != beParentChildRelation.ChildBEId)
                         return false;

                     return true;
                 };
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBEParentChildRelation.ToBigResult(input, filterExpression, BEParentChildRelationDetailMapper));
        }

        public BEParentChildRelation GetBEParentChildRelation(Guid beParentChildRelationDefinitionId, long beParentChildRelationId)
        {
            Dictionary<long, BEParentChildRelation> cachedBEParentChildRelation = this.GetCachedBEParentChildRelations(beParentChildRelationDefinitionId);
            return cachedBEParentChildRelation.GetRecord(beParentChildRelationId);
        }

        public IEnumerable<BEParentChildRelation> GetBEParentChildRelationsByDefinitionId(Guid beParentChildRelationDefinitionId)
        {
            Dictionary<long, BEParentChildRelation> cachedBEParentChildRelations = this.GetCachedBEParentChildRelations(beParentChildRelationDefinitionId);
            return cachedBEParentChildRelations.FindAllRecords(itm => itm.RelationDefinitionId == beParentChildRelationDefinitionId);
        }

        public InsertOperationOutput<BEParentChildRelationDetail> AddBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            var insertOperationOutput = new InsertOperationOutput<BEParentChildRelationDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long beParentChildRelationId = -1;

            IBEParentChildRelationDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();

            if (_dataManager.Insert(beParentChildRelationItem, out beParentChildRelationId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(beParentChildRelationItem.RelationDefinitionId);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                beParentChildRelationItem.BEParentChildRelationId = beParentChildRelationId;
                insertOperationOutput.InsertedObject = BEParentChildRelationDetailMapper(beParentChildRelationItem);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<BEParentChildRelationDetail> UpdateBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            var updateOperationOutput = new UpdateOperationOutput<BEParentChildRelationDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IBEParentChildRelationDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();

            if (_dataManager.Update(beParentChildRelationItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(beParentChildRelationItem.RelationDefinitionId);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BEParentChildRelationDetailMapper(this.GetBEParentChildRelation(beParentChildRelationItem.RelationDefinitionId, beParentChildRelationItem.BEParentChildRelationId));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public BEParentChildRelation GetParent(Guid beParentChildRelationDefinitionId, string childId, DateTime effectiveOn)
        {
            Dictionary<string, List<BEParentChildRelation>> beParentChildRelationsByChildId = this.GetCachedBEParentChildRelationsByChildId(beParentChildRelationDefinitionId);
            List<BEParentChildRelation> beParentChildRelations = beParentChildRelationsByChildId.GetRecord(childId);

            if (beParentChildRelations == null)
                return null;

            Func<BEParentChildRelation, bool> predicate = (itm) =>
            {
                if (itm.BED > effectiveOn)
                    return false;

                if (itm.EED.HasValue && itm.EED.Value <= effectiveOn)
                    return false;

                return true;
            };

            return beParentChildRelations.FindRecord(predicate);
        }

        public List<BEParentChildRelation> GetChildren(Guid beParentChildRelationDefinitionId, string parentId, DateTime effectiveOn)
        {
            Dictionary<string, List<BEParentChildRelation>> beParentChildRelationsByParentId = this.GetCachedBEParentChildRelationsByParentId(beParentChildRelationDefinitionId);
            List<BEParentChildRelation> beParentChildRelations = beParentChildRelationsByParentId.GetRecord(parentId);

            if (beParentChildRelations == null)
                return null;

            Func<BEParentChildRelation, bool> predicate = (itm) =>
            {
                if (itm.BED > effectiveOn)
                    return false;

                if (itm.EED.HasValue && itm.EED.Value <= effectiveOn)
                    return false;

                return true;
            };

            var relatedChildren = beParentChildRelations.FindAllRecords(predicate);
            if (relatedChildren == null)
                return null;

            return relatedChildren.ToList();
        }

        #endregion

        #region Private Methods

        private Dictionary<long, BEParentChildRelation> GetCachedBEParentChildRelations(Guid beParentChildRelationDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBEParentChildRelations", beParentChildRelationDefinitionId,
               () =>
               {
                   IBEParentChildRelationDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
                   return dataManager.GetBEParentChildRelationes().ToDictionary(x => x.BEParentChildRelationId, x => x);
               });
        }

        private Dictionary<string, List<BEParentChildRelation>> GetCachedBEParentChildRelationsByParentId(Guid beParentChildRelationDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBEParentChildRelationsByParentId", beParentChildRelationDefinitionId,
                () =>
                {
                    Dictionary<string, List<BEParentChildRelation>> beParentChildRelationsByParentId = new Dictionary<string, List<BEParentChildRelation>>();
                    List<BEParentChildRelation> beParentChildRelations;

                    var allBEParentChildRelations = this.GetCachedBEParentChildRelations(beParentChildRelationDefinitionId);
                    foreach (var itm in allBEParentChildRelations.Values)
                    {
                        beParentChildRelations = beParentChildRelationsByParentId.GetOrCreateItem(itm.ParentBEId);
                        beParentChildRelations.Add(itm);
                    }
                    return beParentChildRelationsByParentId;
                });
        }

        private Dictionary<string, List<BEParentChildRelation>> GetCachedBEParentChildRelationsByChildId(Guid beParentChildRelationDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBEParentChildRelationsByChildId", beParentChildRelationDefinitionId,
                () =>
                {
                    Dictionary<string, List<BEParentChildRelation>> beParentChildRelationsByChildId = new Dictionary<string, List<BEParentChildRelation>>();
                    List<BEParentChildRelation> beParentChildRelations;

                    var allBEParentChildRelations = this.GetCachedBEParentChildRelations(beParentChildRelationDefinitionId);
                    foreach (var itm in allBEParentChildRelations.Values)
                    {
                        beParentChildRelations = beParentChildRelationsByChildId.GetOrCreateItem(itm.ChildBEId);
                        beParentChildRelations.Add(itm);
                    }
                    return beParentChildRelationsByChildId;
                });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IBEParentChildRelationDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
            ConcurrentDictionary<Guid, Object> _updateHandlesByRelationDefinitionId = new ConcurrentDictionary<Guid, Object>();

            protected override bool ShouldSetCacheExpired(Guid beParentChildRelationDefinitionId)
            {
                object _updateHandle;

                _updateHandlesByRelationDefinitionId.TryGetValue(beParentChildRelationDefinitionId, out _updateHandle);
                bool isCacheExpired = _dataManager.AreBEParentChildRelationUpdated(beParentChildRelationDefinitionId, ref _updateHandle);
                _updateHandlesByRelationDefinitionId.AddOrUpdate(beParentChildRelationDefinitionId, _updateHandle, (key, existingHandle) => _updateHandle);

                return isCacheExpired;
            }
        }

        #endregion

        #region Mappers

        public BEParentChildRelationDetail BEParentChildRelationDetailMapper(BEParentChildRelation beParentChildRelation)
        {
            BEParentChildRelationDefinition beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(beParentChildRelation.RelationDefinitionId);
            BusinessEntityManager businessEntityManager = new BusinessEntityManager();

            BEParentChildRelationDetail beParentChildRelationDetail = new BEParentChildRelationDetail()
            {
                Entity = beParentChildRelation,
                ParentBEName = businessEntityManager.GetEntityDescription(beParentChildRelationDefinition.Settings.ParentBEDefinitionId, beParentChildRelation.ParentBEId),
                ChildBEName = businessEntityManager.GetEntityDescription(beParentChildRelationDefinition.Settings.ChildBEDefinitionId, beParentChildRelation.ChildBEId),
            };
            return beParentChildRelationDetail;
        }

        #endregion
    }
}
