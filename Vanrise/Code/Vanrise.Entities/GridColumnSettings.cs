using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class GridColumnSettings
    {
        public string Width { get; set; }
        public int? FixedWidth { get; set; }
        public int? ListViewWidth { get; set; }
    }
    public static class GridColumnWidthFactorConstants
    {
        public static GridColumnWidthFactor XSmall = new GridColumnWidthFactor { Value = "XSmall", Description = "XSmall", WidthFactor = 3 };
        public static GridColumnWidthFactor Small = new GridColumnWidthFactor { Value = "Small", Description = "Small", WidthFactor = 7 };
        public static GridColumnWidthFactor Normal = new GridColumnWidthFactor { Value = "Normal", Description = "Normal", WidthFactor = 10 };
        public static GridColumnWidthFactor Large = new GridColumnWidthFactor { Value = "Large", Description = "Large", WidthFactor = 13 };
        public static GridColumnWidthFactor XLarge = new GridColumnWidthFactor { Value = "XLarge", Description = "XLarge", WidthFactor = 15 };
        public static GridColumnWidthFactor FixedWidth = new GridColumnWidthFactor { Value = "FixedWidth", Description = "FixedWidth"};
        static Dictionary<string, GridColumnWidthFactor> s_ridColumnWidthFactorByValue = new Dictionary<string, GridColumnWidthFactor>();
        static GridColumnWidthFactorConstants()
        {
            s_ridColumnWidthFactorByValue.Add(XSmall.Value, XSmall);
            s_ridColumnWidthFactorByValue.Add(Small.Value, Small);
            s_ridColumnWidthFactorByValue.Add(Normal.Value, Normal);
            s_ridColumnWidthFactorByValue.Add(Large.Value, Large);
            s_ridColumnWidthFactorByValue.Add(XLarge.Value, XLarge);
            s_ridColumnWidthFactorByValue.Add(FixedWidth.Value, FixedWidth);
        }
        public static int? GetColumnWidthFactor(GridColumnSettings gridColumnSettings)
        {
            if (gridColumnSettings == null)
                throw new NullReferenceException("gridColumnSettings");
            GridColumnWidthFactor gridColumnWidthFactor;
            if (s_ridColumnWidthFactorByValue.TryGetValue(gridColumnSettings.Width, out gridColumnWidthFactor))
            {
                return gridColumnWidthFactor.WidthFactor;
            }
            throw new Exception(string.Format("gridColumnSettings.Width {0} don't have any value match", gridColumnSettings.Width));
        }

    }
    public class GridColumnWidthFactor
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public int? WidthFactor { get; set; }
    }
}
