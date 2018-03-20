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

        public InsertOperationOutput<List<BEParentChildRelationDetail>> AddBEParentChildrenRelation(BEParentChildrenRelation beParentChildrenRelation)
        {
            var insertMultipleOperationOutput = new InsertOperationOutput<List<BEParentChildRelationDetail>>();
            insertMultipleOperationOutput.Result = InsertOperationResult.Failed;
            insertMultipleOperationOutput.InsertedObject = null;

            List<string> overlappedChildBEIds;
            List<BEParentChildRelation> beParentChildRelationsToAdd;

            if (!AreTargetChildrenOverlapping(beParentChildrenRelation, out overlappedChildBEIds, out beParentChildRelationsToAdd))
            {
                List<BEParentChildRelationDetail> beParentChildRelationDetails;

                if (TryAddBEParentChildRelations(beParentChildRelationsToAdd, out beParentChildRelationDetails))
                {
                    insertMultipleOperationOutput.Result = InsertOperationResult.Succeeded;
                    insertMultipleOperationOutput.InsertedObject = beParentChildRelationDetails;
                }
                else
                {
                    insertMultipleOperationOutput.Result = InsertOperationResult.SameExists;
                }

                if (beParentChildRelationDetails != null && beParentChildRelationDetails.Count > 0)
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(beParentChildrenRelation.RelationDefinitionId);
            }
            else
            {
                BusinessEntityManager businessEntityManager = new BusinessEntityManager();
                List<string> overlappedChildBEDescriptions = new List<string>();

                BEParentChildRelationDefinition beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(beParentChildrenRelation.RelationDefinitionId);
                beParentChildRelationDefinition.ThrowIfNull("beParentChildRelationDefinition", beParentChildrenRelation.RelationDefinitionId);
                beParentChildRelationDefinition.Settings.ThrowIfNull("beParentChildRelationDefinition.Settings", beParentChildrenRelation.RelationDefinitionId);

                foreach (string childBEId in overlappedChildBEIds)
                    overlappedChildBEDescriptions.Add(businessEntityManager.GetEntityDescription(beParentChildRelationDefinition.Settings.ChildBEDefinitionId, childBEId));

                insertMultipleOperationOutput.Message = string.Format("Specified Interval overlaps with other assignments of DIDs: {0}", string.Join(", ", overlappedChildBEDescriptions));
            }

            return insertMultipleOperationOutput;
        }

        public UpdateOperationOutput<BEParentChildRelationDetail> UpdateBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            var updateOperationOutput = new UpdateOperationOutput<BEParentChildRelationDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IOrderedEnumerable<BEParentChildRelation> beParentChildRelations = this.GetParents(beParentChildRelationItem.RelationDefinitionId, beParentChildRelationItem.ChildBEId);

            if (!IsOverlappedWith(beParentChildRelationItem, beParentChildRelations))
            {
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
            }
            else
            {
                updateOperationOutput.Message = "Specified Interval overlaps with another one";
            }

            return updateOperationOutput;
        }

        public bool TryAddBEParentChildRelation(BEParentChildRelation beParentChildRelationItem, out long insertedId)
        {
            IBEParentChildRelationDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
            return _dataManager.Insert(beParentChildRelationItem, out insertedId);
        }

        public IOrderedEnumerable<BEParentChildRelation> GetParents(Guid beParentChildRelationDefinitionId, string childId)
        {
            Dictionary<string, IOrderedEnumerable<BEParentChildRelation>> beParentChildRelationsByChildId = this.GetCachedBEParentChildRelationsByChildId(beParentChildRelationDefinitionId);
            return beParentChildRelationsByChildId.GetRecord(childId);
        }

        public BEParentChildRelation GetParent(Guid beParentChildRelationDefinitionId, string childId, DateTime effectiveOn)
        {
            IOrderedEnumerable<BEParentChildRelation> beParentChildRelations = GetParents(beParentChildRelationDefinitionId, childId);

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
            Dictionary<string, IOrderedEnumerable<BEParentChildRelation>> beParentChildRelationsByParentId = this.GetCachedBEParentChildRelationsByParentId(beParentChildRelationDefinitionId);
            IOrderedEnumerable<BEParentChildRelation> beParentChildRelations = beParentChildRelationsByParentId.GetRecord(parentId);

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

        public bool IsChildAssignedToParentWithoutEED(Guid beParentChildRelationDefinitionId, string childId)
        {
            IOrderedEnumerable<BEParentChildRelation> beParentChildRelations = this.GetParents(beParentChildRelationDefinitionId, childId);
            if (beParentChildRelations == null || beParentChildRelations.Count() == 0)
                return false;

            var item = beParentChildRelations.FirstOrDefault(itm => !itm.EED.HasValue || itm.EED.Value != itm.BED);
            if (item == null || item.EED.HasValue)
                return false;

            return true;
        }

        public DateTime? GetLastAssignedEED(Guid beParentChildRelationDefinitionId, string childId)
        {
            IOrderedEnumerable<BEParentChildRelation> beParentChildRelations = this.GetParents(beParentChildRelationDefinitionId, childId);
            if (beParentChildRelations != null && beParentChildRelations.Count() > 0)
                return beParentChildRelations.First().EED;
            return null;
        }

        #endregion

        #region Private Methods

        public Dictionary<long, BEParentChildRelation> GetCachedBEParentChildRelations(Guid beParentChildRelationDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBEParentChildRelations", beParentChildRelationDefinitionId,
               () =>
               {
                   IBEParentChildRelationDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
                   return dataManager.GetBEParentChildRelations(beParentChildRelationDefinitionId).ToDictionary(x => x.BEParentChildRelationId, x => x);
               });
        }

        private Dictionary<string, IOrderedEnumerable<BEParentChildRelation>> GetCachedBEParentChildRelationsByParentId(Guid beParentChildRelationDefinitionId)
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

                    return beParentChildRelationsByParentId.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(x => x.BED).ThenByDescending(x => x.EED.HasValue ? x.EED.Value : DateTime.MaxValue));
                });
        }

        private Dictionary<string, IOrderedEnumerable<BEParentChildRelation>> GetCachedBEParentChildRelationsByChildId(Guid beParentChildRelationDefinitionId)
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

                    return beParentChildRelationsByChildId.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(x => x.BED).ThenByDescending(x => x.EED.HasValue ? x.EED.Value : DateTime.MaxValue));
                });
        }

        private bool IsOverlappedWith(BEParentChildRelation target, IOrderedEnumerable<BEParentChildRelation> beParentChildRelations)
        {
            if (beParentChildRelations != null && beParentChildRelations.Count() > 0)
            {
                foreach (var beParentChildRelation in beParentChildRelations)
                {
                    if (target.BEParentChildRelationId == beParentChildRelation.BEParentChildRelationId)
                        continue;

                    if (target.IsOverlappedWith(beParentChildRelation))
                        return true;
                }
            }
            return false;
        }

        private bool AreTargetChildrenOverlapping(BEParentChildrenRelation target, out List<string> overlappedChildBEIds, out List<BEParentChildRelation> beParentChildRelationsToAdd)
        {
            target.ChildBEIds.ThrowIfNull("beParentChildrenRelation.ChildBEIds");

            overlappedChildBEIds = new List<string>();
            beParentChildRelationsToAdd = new List<BEParentChildRelation>();

            foreach (var childBEId in target.ChildBEIds)
            {
                BEParentChildRelation beParentChildRelation = new BEParentChildRelation()
                {
                    RelationDefinitionId = target.RelationDefinitionId,
                    ParentBEId = target.ParentBEId,
                    ChildBEId = childBEId,
                    BED = target.BED,
                    EED = target.EED
                };

                IOrderedEnumerable<BEParentChildRelation> beParentChildRelations = this.GetParents(target.RelationDefinitionId, childBEId);

                if (target.BED != target.EED && !IsOverlappedWith(beParentChildRelation, beParentChildRelations))
                    beParentChildRelationsToAdd.Add(beParentChildRelation);
                else
                    overlappedChildBEIds.Add(childBEId);
            }

            if (overlappedChildBEIds.Count > 0)
            {
                beParentChildRelationsToAdd = null;
                return true;
            }

            return false;
        }

        private bool TryAddBEParentChildRelations(List<BEParentChildRelation> beParentChildRelations, out List<BEParentChildRelationDetail> beParentChildRelationDetails)
        {
            beParentChildRelationDetails = new List<BEParentChildRelationDetail>();
            long insertedId;

            foreach (var beParentChildRelation in beParentChildRelations)
            {
                if (!TryAddBEParentChildRelation(beParentChildRelation, out insertedId))
                    return false;

                beParentChildRelation.BEParentChildRelationId = insertedId;
                beParentChildRelationDetails.Add(BEParentChildRelationDetailMapper(beParentChildRelation));
            }

            return true;
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
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
