using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class GeneralSettingData :SettingData
    {
        public UISettingData UIData { get; set; }

        public CacheSettingData CacheData { get; set; }
    }
    public class UISettingData
    {
        public Guid? DefaultViewId { get; set; }

        public int? NormalPrecision { get; set; }

        public int? LongPrecision { get; set; }

        public int? GridPageSize { get; set; }
    }

    public class CacheSettingData
    {
        public long ClientCacheNumber { get; set; }
    }
}
