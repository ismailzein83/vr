using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BEParentChildRelationManager
    {
        #region Public Methods

        public IDataRetrievalResult<BEParentChildRelationDetail> GetFilteredBEParentChildRelations(DataRetrievalInput<BEParentChildRelationQuery> input)
        {
            var allBEParentChildRelation = this.GetCachedBEParentChildRelations();
            Func<BEParentChildRelation, bool> filterExpression = null; //(x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBEParentChildRelation.ToBigResult(input, filterExpression, BEParentChildRelationDetailMapper));
        }

        public BEParentChildRelation GetBEParentChildRelation(long beParentChildRelationId)
        {
            Dictionary<long, BEParentChildRelation> cachedBEParentChildRelation = this.GetCachedBEParentChildRelations();
            return cachedBEParentChildRelation.GetRecord(beParentChildRelationId);
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BEParentChildRelationDetailMapper(this.GetBEParentChildRelation(beParentChildRelationItem.BEParentChildRelationId));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        //public IEnumerable<BEParentChildRelationInfo> GetBEParentChildRelationsInfo(BEParentChildRelationFilter filter)
        //{
        //    Func<BEParentChildRelation, bool> filterExpression = null;

        //    return this.GetCachedBEParentChildRelationes().MapRecords(BEParentChildRelationInfoMapper, filterExpression).OrderBy(x => x.Name);
        //}

        public BEParentChildRelation GetParent(Guid beParentChildRelationDefinitionId, string childId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        public List<BEParentChildRelation> GetChildren(Guid beParentChildRelationDefinitionId, string parentId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBEParentChildRelationDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBEParentChildRelationUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<long, BEParentChildRelation> GetCachedBEParentChildRelations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBEParentChildRelationes",
               () =>
               {
                   IBEParentChildRelationDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBEParentChildRelationDataManager>();
                   return dataManager.GetBEParentChildRelationes().ToDictionary(x => x.BEParentChildRelationId, x => x);
               });
        }

        private IBusinessEntityManager GetBusinessEntityManager(Guid businessEntityDefinitionId)
        {
            var beDefinitionManager = new BusinessEntityDefinitionManager();
            var beManagerInstance = beDefinitionManager.GetBusinessEntityManager(businessEntityDefinitionId);
            if (beManagerInstance == null)
                throw new NullReferenceException(String.Format("beManagerInstance. BusinessEntityDefinitionId '{0}'", businessEntityDefinitionId));

            return beManagerInstance;
        }

        private BusinessEntityDefinition GetBusinessEntityDefinition(Guid businessEntityDefinitionId)
        {
            var beDefinitionManager = new BusinessEntityDefinitionManager();
            var beDefinition = beDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException(String.Format("beDefinition of BusinessEntityDefinitionId '{0}'", businessEntityDefinitionId));

            return beDefinition;
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

        //public BEParentChildRelationInfo BEParentChildRelationInfoMapper(BEParentChildRelation beParentChildRelation)
        //{
        //    BEParentChildRelationInfo beParentChildRelationInfo = new BEParentChildRelationInfo()
        //    {
        //        BEParentChildRelationId = beParentChildRelation.BEParentChildRelationId,
        //        Name = beParentChildRelation.Name
        //    };
        //    return beParentChildRelationInfo;
        //}

        #endregion
    }
}
