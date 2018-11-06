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

public class ColorManager
{
    #region Public Methods
    public IEnumerable<ColorInfo> GetColorsInfo(ColorInfoFilter colorInfoFilter)
    {

        var allColors = GetCachedColors();
        Func<Color, bool> filterFunc = (color) =>
        {
            if(colorInfoFilter!=null)
            {
                var ColorId = color.DesksizeId;
                if(ColorId!=colorInfoFilter.DesksizeId)
                { return false; }


            }
            return true;
        };
        return allColors.MapRecords(ColorInfoMapper, filterFunc).OrderBy(color => color.Name);

    }

    public string GetColorName(int colorId)
    {
        var parent = GetColorById(colorId);
        if (parent == null)
            return null;
        return parent.Name;
    }

    public Color GetColorById(int colorId)
    {
        var allColors = GetCachedColors();
        return allColors.GetRecord(colorId);
    }

    #endregion 
    #region Private Classes
    private class CacheManager : Vanrise.Caching.BaseCacheManager
    {
        IColorDataManager colorDataManager = DemoModuleFactory.GetDataManager<IColorDataManager>();
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return false;
        }
    }
    #endregion

    #region Private Methods

    private Dictionary<int, Color> GetCachedColors()
    {
        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCachedColors", () =>
           {
               IColorDataManager colorDataManager = DemoModuleFactory.GetDataManager<IColorDataManager>();
               List<Color> colors = colorDataManager.GetColors();
               return colors.ToDictionary(color => color.ColorId, color => color);
           });
    }
    #endregion

    #region Mappers

    public ColorDetails ColorDetailMapper(Color color)
    {
        var colorDetails=new ColorDetails{

            Name=color.Name,
            ColorId=color.ColorId,
           

        };
        return colorDetails;
    }

    public ColorInfo ColorInfoMapper(Color color)
    {
        return new ColorInfo
        {
            Name = color.Name,
            ColorId = color.ColorId,
            DesksizeId = color.DesksizeId
        };

    }
    #endregion

}

