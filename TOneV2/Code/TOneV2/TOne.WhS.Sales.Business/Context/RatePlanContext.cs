using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanContext : IRatePlanContext
    {
        #region Fields / Constructors

        private DateTime _retroactiveDate;

        private object _object = new object();
        private bool _processHasChanges = false;

        public RatePlanContext()
        {
            int retroactiveDayOffset = new TOne.WhS.BusinessEntity.Business.ConfigManager().GetSaleAreaRetroactiveDayOffset();
            _retroactiveDate = DateTime.Now.Date.AddDays(-retroactiveDayOffset);
        }

        #endregion

        #region Properties

        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public int OwnerSellingNumberPlanId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public SaleEntityZoneRateLocator RateLocator { get; set; }
        public SaleEntityZoneRateLocator FutureRateLocator { get; set; }
        public DateTime RetroactiveDate
        {
            get
            {
                return _retroactiveDate;
            }
        }
        public EffectiveAfterCustomerZoneRatesByZone EffectiveAfterCustomerZoneRatesByZone { get; set; }
        public IntersectedSellingProductZoneRatesByZone IntersectedSellingProductZoneRatesByZone { get; set; }
        public Dictionary<int, List<ExistingZone>> ExistingZonesByCountry { get; set; }
        public bool ProcessHasChanges
        {
            get
            {
                return _processHasChanges;
            }
        }

        #endregion

        #region Methods

        public void SetProcessHasChangesToTrueWithLock()
        {
            if (!_processHasChanges)
            {
                lock (_object)
                {
                    _processHasChanges = true;
                }
            }
        }

        #endregion
    }

    public interface IRatePlanContext
    {
        #region Properties

        SalePriceListOwnerType OwnerType { get; }
        int OwnerId { get; }
        int OwnerSellingNumberPlanId { get; }
        DateTime EffectiveDate { get; }
        SaleEntityZoneRateLocator RateLocator { get; }
        SaleEntityZoneRateLocator FutureRateLocator { get; }
        DateTime RetroactiveDate { get; }
        EffectiveAfterCustomerZoneRatesByZone EffectiveAfterCustomerZoneRatesByZone { get; }
        IntersectedSellingProductZoneRatesByZone IntersectedSellingProductZoneRatesByZone { get; }
        Dictionary<int, List<ExistingZone>> ExistingZonesByCountry { get; }
        bool ProcessHasChanges { get; }

        #endregion

        #region Methods

        void SetProcessHasChangesToTrueWithLock();

        #endregion
    }
}
