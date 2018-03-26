using Demo.Module.Data;
using Demo.Module.Entities;
using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class UniversityManager
    {
        public IDataRetrievalResult<UniversityDetails> GetFilteredUniversities(DataRetrievalInput<UniversityQuery> input)
        {
            var allUniversities = GetCachedUniversities();
            Func<University, bool> filterExpression = (university) =>
            {
                if (input.Query.Name != null && !university.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allUniversities.ToBigResult(input, filterExpression, UniversityDetailMapper));
        }

        public IEnumerable<UniversityInfo> GetUniversitiesInfo()
        {
            var allUniversities = GetCachedUniversities();
            Func<University, bool> filterFunc = null;

            filterFunc = (university) =>
            {
                return true;
            };

            IEnumerable<University> filteredUniversities = (filterFunc != null) ? allUniversities.FindAllRecords(filterFunc) : allUniversities.MapRecords(u => u.Value);
            return filteredUniversities.MapRecords(UniversityInfoMapper).OrderBy(university => university.Name);
        }
        
        public University GetUniversityById(int universityId)
        {
           var allUniversities = GetCachedUniversities();
           return allUniversities.GetRecord(universityId);
        }
        
        public string GetUniversityName(int universityId)
        {
            var university = GetUniversityById(universityId);
            return university != null ? university.Name : null;
        }

        public InsertOperationOutput<UniversityDetails> AddUniversity(University university)
        {
            IUniversityDataManager universityDataManager = DemoModuleFactory.GetDataManager<IUniversityDataManager>();
            InsertOperationOutput<UniversityDetails> insertOperationOutput = new InsertOperationOutput<UniversityDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int universityId = -1;
            bool insertActionSuccess = universityDataManager.Insert(university, out universityId);
            if (insertActionSuccess)
            {
                university.UniversityId = universityId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = UniversityDetailMapper(university);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        
        public UpdateOperationOutput<UniversityDetails> UpdateUniversity(University university)
        {
            IUniversityDataManager universityDataManager = DemoModuleFactory.GetDataManager<IUniversityDataManager>();
            UpdateOperationOutput<UniversityDetails> updateOperationOutput = new UpdateOperationOutput<UniversityDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = universityDataManager.Update(university);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UniversityDetailMapper(university);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        
        public DeleteOperationOutput<UniversityDetails> Delete(int Id)
        {
            IUniversityDataManager universityDataManager = DemoModuleFactory.GetDataManager<IUniversityDataManager>();
            DeleteOperationOutput<UniversityDetails> deleteOperationOutput = new DeleteOperationOutput<UniversityDetails>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleteActionSuccess = universityDataManager.Delete(Id);
            if (deleteActionSuccess)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }

        private Dictionary<int, University> GetCachedUniversities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCachedUniversities", () =>
                {
                    IUniversityDataManager universityDataManager = DemoModuleFactory.GetDataManager<IUniversityDataManager>();
                    List<University> universities = universityDataManager.GetUniversities();
                    return universities.ToDictionary(university => university.UniversityId, university => university);
                });
        }
       
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IUniversityDataManager universityDataManager = DemoModuleFactory.GetDataManager<IUniversityDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return universityDataManager.AreUniversitiesUpdated(ref _updateHandle);
            }
        }

        public UniversityDetails UniversityDetailMapper(University university)
        {
            return new UniversityDetails
            {
                Entity = university
            };
        }

        UniversityInfo UniversityInfoMapper(University university)
        {
            return new UniversityInfo
            {
                UniversityId = university.UniversityId,
                Name = university.Name
            };
        }
    }
}
