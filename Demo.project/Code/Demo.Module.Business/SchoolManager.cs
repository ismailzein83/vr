using Demo.Module.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Demo.Module.Entities;
using Demo.Module.Entities.ProductInfo;

public class SchoolManager
    {
      
       #region Public Methods
    public IDataRetrievalResult<SchoolDetails> GetFilteredSchools(DataRetrievalInput<SchoolQuery> input)
        {
            var allSchools = GetCachedSchools();
            Func<School, bool> filterExpression = (school) =>
            {
                if (input.Query.Name != null && !school.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allSchools.ToBigResult(input, filterExpression, SchoolDetailMapper));

        }

    public string GetSchoolName(int schoolId)
    {
        var parent = GetSchoolById(schoolId);
        if (parent == null)
            return null;
        return parent.Name;
    }



    public InsertOperationOutput<SchoolDetails> AddSchool(School school)
        {
            ISchoolDataManager schoolDataManager = DemoModuleFactory.GetDataManager<ISchoolDataManager>();
            InsertOperationOutput<SchoolDetails> insertOperationOutput = new InsertOperationOutput<SchoolDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int schoolId = -1;

            bool insertActionSuccess = schoolDataManager.Insert(school, out schoolId);
            if (insertActionSuccess)
            {
                school.SchoolId = schoolId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SchoolDetailMapper(school);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
    public School GetSchoolById(int schoolId)
        {
            var allSchools = GetCachedSchools();
            return allSchools.GetRecord(schoolId);
        }

    public IEnumerable<SchoolInfo> GetSchoolsInfo(SchoolInfoFilter schoolInfoFilter)
    {
        var allSchools = GetCachedSchools();
        Func<School, bool> filterFunc = (school) =>
        {
            if (schoolInfoFilter != null)
            {
           
                var SchoolId = school.DemoCityId;
                    if (SchoolId!=schoolInfoFilter.DemoCityId)
                        {
                            return false;
                        }
                   

            }
            return true;
        };
        return allSchools.MapRecords(SchoolInfoMapper, filterFunc).OrderBy(school => school.Name);
    }

    public UpdateOperationOutput<SchoolDetails> UpdateSchool(School school)
        {
            ISchoolDataManager schoolDataManager = DemoModuleFactory.GetDataManager<ISchoolDataManager>();
            UpdateOperationOutput<SchoolDetails> updateOperationOutput = new UpdateOperationOutput<SchoolDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = schoolDataManager.Update(school);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SchoolDetailMapper(school);
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
            ISchoolDataManager schoolDataManager = DemoModuleFactory.GetDataManager<ISchoolDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return schoolDataManager.AreSchoolsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, School> GetCachedSchools()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedSchools", () =>
               {
                   ISchoolDataManager schoolDataManager = DemoModuleFactory.GetDataManager<ISchoolDataManager>();
                   List<School> schools = schoolDataManager.GetSchools();
                   return schools.ToDictionary(school => school.SchoolId, school => school);
               });
        }
        #endregion
    
        #region Mappers
        public SchoolDetails SchoolDetailMapper(School school)
        {
            var schoolDetails = new SchoolDetails
            {
                Name = school.Name,
                SchoolId = school.SchoolId,
              
            };

            return schoolDetails;
        }

        public SchoolInfo SchoolInfoMapper(School school)
        {
            return new SchoolInfo
            {
                Name = school.Name,
                SchoolId = school.SchoolId,
                DemoCityId=school.DemoCityId
            };
        }
        #endregion 
    
}
