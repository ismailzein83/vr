using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BusinessEntitySettingsData : SettingData
    {
        public CachingExpirationIntervals CachingExpirationIntervals { get; set; } 
    }

    public class CachingExpirationIntervals
    {
        public int? TodayEntitiesIntervalInMinutes { get; set; }

        public int PreviousEntitiesIntervalInMinutes { get; set; }
    }
}