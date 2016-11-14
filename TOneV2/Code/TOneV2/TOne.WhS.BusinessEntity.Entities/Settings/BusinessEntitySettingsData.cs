using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BusinessEntitySettingsData : SettingData
    {
        public CachingExpirationIntervals CachingExpirationIntervals { get; set; }
    }

    public class CachingExpirationIntervals
    {
        public int? TodayEntities { get; set; }

        public int? PreviousEntities { get; set; }
    }
}
