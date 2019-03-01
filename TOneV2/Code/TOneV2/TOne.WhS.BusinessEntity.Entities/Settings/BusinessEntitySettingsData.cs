using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BusinessEntitySettingsData : SettingData
    {
        public CachingExpirationIntervals CachingExpirationIntervals { get; set; }

        private TechnicalNumberPlanSettings _technicalNumberPlanSettings;

        public TechnicalNumberPlanSettings TechnicalNumberPlanSettings
        {
            get
            {
                if (_technicalNumberPlanSettings == null)
                    _technicalNumberPlanSettings = new TechnicalNumberPlanSettings() { MaxTechnicalZoneCount = 4000 };

                return _technicalNumberPlanSettings;
            }
            set
            {
                _technicalNumberPlanSettings = value;
            }
        }
    }

    public class CachingExpirationIntervals
    {
        public int? TodayEntitiesIntervalInMinutes { get; set; }

        public int PreviousEntitiesIntervalInMinutes { get; set; }

        public int? SupplierZonesIntevalInMinutes { get; set; }
    }
}