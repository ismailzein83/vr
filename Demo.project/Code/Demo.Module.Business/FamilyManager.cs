using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class FamilyManager
    {
        
        #region Public Methods
        public IDataRetrievalResult<FamilyDetails> GetFilteredFamilies(DataRetrievalInput<FamilyQuery> input)
        {
            var allFamilies = GetCachedFamilies();
            Func<Family, bool> filterExpression = (family) =>
            {
                if (input.Query.Name != null && !family.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allFamilies.ToBigResult(input, filterExpression, FamilyDetailMapper));

        }
        public string GetFamilyName(long familyId)
        {
            var family = GetFamilyById(familyId);
            if (family == null)
                return null;
            return family.Name;
        }

        public InsertOperationOutput<FamilyDetails> AddFamily(Family family)
        {
            IFamilyDataManager familyDataManager = DemoModuleFactory.GetDataManager<IFamilyDataManager>();
            InsertOperationOutput<FamilyDetails> insertOperationOutput = new InsertOperationOutput<FamilyDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long familyId = -1;

            bool insertActionSuccess = familyDataManager.Insert(family, out familyId);
            if (insertActionSuccess)
            {
                family.FamilyId = familyId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = FamilyDetailMapper(family);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Family GetFamilyById(long familyId)
        {
            var allFamilys = GetCachedFamilies();
            return allFamilys.GetRecord(familyId);
        }
        public IEnumerable<FamilyInfo> GetFamiliesInfo(FamilyInfoFilter familyInfoFilter)
        {
            var allFamilies = GetCachedFamilies();
            Func<Family, bool> filterFunc = (family) =>
            {
                if (familyInfoFilter != null)
                {
                    if (familyInfoFilter.Filters != null)
                    {
                        var context = new FamilyInfoFilterContext
                        {
                            FamilyId = family.FamilyId
                        };
                        foreach (var filter in familyInfoFilter.Filters)
                        {
                            if (!filter.IsMatch(context))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            };
            return allFamilies.MapRecords(FamilyInfoMapper, filterFunc).OrderBy(family => family.Name);
        }

        public UpdateOperationOutput<FamilyDetails> UpdateFamily(Family family)
        {
            IFamilyDataManager familyDataManager = DemoModuleFactory.GetDataManager<IFamilyDataManager>();
            UpdateOperationOutput<FamilyDetails> updateOperationOutput = new UpdateOperationOutput<FamilyDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = familyDataManager.Update(family);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = FamilyDetailMapper(family);
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
            IFamilyDataManager familyDataManager = DemoModuleFactory.GetDataManager<IFamilyDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return familyDataManager.AreCompaniesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Family> GetCachedFamilies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedFamilies", () =>
               {
                   IFamilyDataManager familyDataManager = DemoModuleFactory.GetDataManager<IFamilyDataManager>();
                   List<Family> familys = familyDataManager.GetFamilies();
                   return familys.ToDictionary(family => family.FamilyId, family => family);
               });
        }
        #endregion

        #region Mappers
        public FamilyDetails FamilyDetailMapper(Family family)
        {
            return new FamilyDetails
            {
                Name = family.Name,
                FamilyId = family.FamilyId
            };
        }



        public FamilyInfo FamilyInfoMapper(Family family)
        {
            return new FamilyInfo
            {
                Name = family.Name,
                FamilyId = family.FamilyId
            };
        }
        #endregion 

    }
}
