using Demo.Module.Data;
using Demo.Module.Entities;
using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class CollegeManager
    {
        public IDataRetrievalResult<CollegeDetails> GetFilteredColleges(DataRetrievalInput<CollegeQuery> input)
        {
            var allColleges = GetCachedColleges();
            Func<College, bool> filterExpression = (college) =>
            {
                if (input.Query.Name != null && !college.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.UniversityIds != null && !input.Query.UniversityIds.Contains(college.UniversityId))
                    return false;

                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allColleges.ToBigResult(input, filterExpression, CollegeDetailMapper));
        }

        public IEnumerable<CollegeInfo> GetCollegesInfo()
        {
            var allColleges = GetCachedColleges();
            Func<College, bool> filterFunc = null;

            filterFunc = (college) =>
            {
                return true;
            };

            IEnumerable<College> filteredColleges = (filterFunc != null) ? allColleges.FindAllRecords(filterFunc) : allColleges.MapRecords(u => u.Value);
            return filteredColleges.MapRecords(CollegeInfoMapper).OrderBy(college => college.Name);
        }

        public College GetCollegeById(int collegeId)
        {
            var allColleges = GetCachedColleges();
            return allColleges.GetRecord(collegeId);
        }

        public InsertOperationOutput<CollegeDetails> AddCollege(College college)
        {
            ICollegeDataManager collegeDataManager = DemoModuleFactory.GetDataManager<ICollegeDataManager>();
            InsertOperationOutput<CollegeDetails> insertOperationOutput = new InsertOperationOutput<CollegeDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int collegeId = -1;
            bool insertActionSuccess = collegeDataManager.Insert(college, out collegeId);
            if (insertActionSuccess)
            {
                college.CollegeId = collegeId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CollegeDetailMapper(college);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        
        public UpdateOperationOutput<CollegeDetails> UpdateCollege(College college)
        {
            ICollegeDataManager collegeDataManager = DemoModuleFactory.GetDataManager<ICollegeDataManager>();
            UpdateOperationOutput<CollegeDetails> updateOperationOutput = new UpdateOperationOutput<CollegeDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = collegeDataManager.Update(college);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CollegeDetailMapper(college);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        
        public DeleteOperationOutput<CollegeDetails> Delete(int Id)
        {
            ICollegeDataManager collegeDataManager = DemoModuleFactory.GetDataManager<ICollegeDataManager>();
            DeleteOperationOutput<CollegeDetails> deleteOperationOutput = new DeleteOperationOutput<CollegeDetails>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleteActionSuccess = collegeDataManager.Delete(Id);
            if (deleteActionSuccess)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }

        private Dictionary<int, College> GetCachedColleges()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCachedColleges", () =>
                {
                    ICollegeDataManager collegeDataManager = DemoModuleFactory.GetDataManager<ICollegeDataManager>();
                    List<College> colleges = collegeDataManager.GetColleges();
                    return colleges.ToDictionary(college => college.CollegeId, college => college);
                });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICollegeDataManager collegeDataManager = DemoModuleFactory.GetDataManager<ICollegeDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return collegeDataManager.AreCollegesUpdated(ref _updateHandle);
            }
        }

        public CollegeDetails CollegeDetailMapper(College college)
        {
            UniversityManager univerityManager = new UniversityManager();
            return new CollegeDetails
            {
                CollegeId = college.CollegeId,
                CollegeName = college.Name,
                UniversityId = college.UniversityId,
                UniversityName = univerityManager.GetUniversityName(college.UniversityId)
            };
        }

        CollegeInfo CollegeInfoMapper(College college)
        {
            return new CollegeInfo
            {
                CollegeId = college.CollegeId,
                Name = college.Name
            };
        }
    }

    public class CollegeInfoConfigsManager
    {
        public IEnumerable<CollegeInfoConfig> GetCollegeInfoTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<CollegeInfoConfig>(CollegeInfoConfig.EXTENSION_TYPE);
        }
    }
}
