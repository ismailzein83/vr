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
using Vanrise.Common.Business;

public class SpecialityManager
{
    #region Public Methods
    public IEnumerable<SpecialityInfo> GetSpecialitiesInfo()
    {

        var allSpecialities = GetCachedSpecialities();
        Func<Speciality, bool> filterFunc = (Speciality) =>
        {
            return true;
        };
        return allSpecialities.MapRecords(SpecialityInfoMapper, filterFunc).OrderBy(speciality => speciality.Name);

    }

    public string GetSpecialityName(int specialityId)
    {
        var parent = GetSpecialityById(specialityId);
        if (parent == null)
            return null;
        return parent.Name;
    }

    public Speciality GetSpecialityById(int specialityId)
    {
        var allSpecialities = GetCachedSpecialities();
        return allSpecialities.GetRecord(specialityId);
    }

    #endregion 
    #region Private Classes
    private class CacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISpecialityDataManager specialityDataManager = DemoModuleFactory.GetDataManager<ISpecialityDataManager>();
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return false;
        }
    }
    #endregion

    #region Private Methods

    private Dictionary<int, Speciality> GetCachedSpecialities()
    {
        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCachedSpecialities", () =>
           {
               ISpecialityDataManager specialityDataManager = DemoModuleFactory.GetDataManager<ISpecialityDataManager>();
               List<Speciality> specialities = specialityDataManager.GetSpecialities();
               return specialities.ToDictionary(speciality => speciality.SpecialityId, speciality => speciality);
           });
    }
    #endregion

    #region Mappers

    public SpecialityDetails SpecialityDetailMapper(Speciality speciality)
    {
        var specialityDetails=new SpecialityDetails{

            Name=speciality.Name,
            SpecialityId=speciality.SpecialityId,

        };
        return specialityDetails;
    }

    public SpecialityInfo SpecialityInfoMapper(Speciality speciality)
    {
        return new SpecialityInfo
        {
            Name = speciality.Name,
            SpecialityId = speciality.SpecialityId,
        };

    }
    #endregion

}

