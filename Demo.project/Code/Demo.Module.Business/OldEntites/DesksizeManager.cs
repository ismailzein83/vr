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
using Vanrise.Common.Business;

public class DesksizeManager
{
    #region Public Methods
    public IEnumerable<DesksizeInfo> GetDesksizesInfo()
    {

        var allDesksizes = GetCachedDesksizes();
        Func<Desksize, bool> filterFunc = (Desksize) =>
        {
            return true;
        };
        return allDesksizes.MapRecords(DesksizeInfoMapper, filterFunc).OrderBy(desksize => desksize.Name);

    }

    public string GetDesksizeName(int desksizeId)
    {
        var parent = GetDesksizeById(desksizeId);
        if (parent == null)
            return null;
        return parent.Name;
    }

    public Desksize GetDesksizeById(int desksizeId)
    {
        var allDesksizes = GetCachedDesksizes();
        return allDesksizes.GetRecord(desksizeId);
    }

    #endregion
    #region Private Classes
    private class CacheManager : Vanrise.Caching.BaseCacheManager
    {
        IDesksizeDataManager desksizeDataManager = DemoModuleFactory.GetDataManager<IDesksizeDataManager>();
        object _updateHandle;
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return false;
        }
    }
    #endregion

    #region Private Methods

    private Dictionary<int, Desksize> GetCachedDesksizes()
    {
        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCachedDesksizes", () =>
           {
               IDesksizeDataManager desksizeDataManager = DemoModuleFactory.GetDataManager<IDesksizeDataManager>();
               List<Desksize> desksizes = desksizeDataManager.GetDesksizes();
               return desksizes.ToDictionary(desksize => desksize.DesksizeId, desksize => desksize);
           });
    }
    #endregion

    #region Mappers

    public DesksizeDetails DesksizeDetailMapper(Desksize desksize)
    {
        var desksizeDetails=new DesksizeDetails{

            Name=desksize.Name,
            DesksizeId=desksize.DesksizeId,

        };
        return desksizeDetails;
    }

    public DesksizeInfo DesksizeInfoMapper(Desksize desksize)
    {
        return new DesksizeInfo
        {
            Name = desksize.Name,
            DesksizeId = desksize.DesksizeId,
        };
        
    }
    #endregion

}

