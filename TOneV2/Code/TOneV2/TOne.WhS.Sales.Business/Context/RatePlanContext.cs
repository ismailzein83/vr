using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanContext : IRatePlanContext
    {
        #region Fields / Constructors

        private DateTime _retroactiveDate;

        private object _object = new object();
        private bool _processHasChanges = false;
        private int _priceListCurrencyId;
        private int _systemCurrencyId;
        private Dictionary<int, decimal> _maximumRateConvertedByCurrency = new Dictionary<int, decimal>();
        public RatePlanContext()
        {
            int retroactiveDayOffset = new TOne.WhS.BusinessEntity.Business.ConfigManager().GetSaleAreaRetroactiveDayOffset();
            _retroactiveDate = DateTime.Now.Date.AddDays(-retroactiveDayOffset);

            CurrencyManager currencyManager = new CurrencyManager();
            var systemCurrency = currencyManager.GetSystemCurrency();
            if (systemCurrency == null)
                throw new DataIntegrityValidationException("System Currency was not found");
            _systemCurrencyId = systemCurrency.CurrencyId;

            MaximumRate = new BusinessEntity.Business.ConfigManager().GetSaleAreaMaximumRate();
        }

        #endregion

        #region Properties

        public Dictionary<int, decimal> MaximumRateConvertedByCurrency { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public int OwnerSellingNumberPlanId { get; set; }
        public int CurrencyId { get; set; }
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
        public int PriceListCurrencyId
        {
            get
            {
                return _priceListCurrencyId;
            }
            set { _priceListCurrencyId = value; }
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
        public decimal MaximumRate { get; set; }

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
        public decimal GetMaximumRateConverted(int currencyId)
        {
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            decimal convertedRate;
            if (!_maximumRateConvertedByCurrency.TryGetValue(currencyId, out convertedRate))
            {
                decimal convertedmaximumRate = currencyExchangeRateManager.ConvertValueToCurrency(MaximumRate, _systemCurrencyId, currencyId, DateTime.Now);
                _maximumRateConvertedByCurrency.Add(currencyId, convertedmaximumRate);
                return convertedmaximumRate;
            }
            return convertedRate;
        }

        public int GetRateToChangeCurrencyId(RateToChange rateToChange)
        {
            return _priceListCurrencyId;
        }
        #endregion

    }

    public interface IRatePlanContext
    {
        #region Properties

        SalePriceListOwnerType OwnerType { get; }
        int OwnerId { get; }
        int OwnerSellingNumberPlanId { get; }
        int CurrencyId { get; }
        DateTime EffectiveDate { get; }
        SaleEntityZoneRateLocator RateLocator { get; }
        SaleEntityZoneRateLocator FutureRateLocator { get; }
        DateTime RetroactiveDate { get; }
        EffectiveAfterCustomerZoneRatesByZone EffectiveAfterCustomerZoneRatesByZone { get; }
        IntersectedSellingProductZoneRatesByZone IntersectedSellingProductZoneRatesByZone { get; }
        Dictionary<int, List<ExistingZone>> ExistingZonesByCountry { get; }
        bool ProcessHasChanges { get; }
        decimal MaximumRate { get; }
        Dictionary<int, decimal> MaximumRateConvertedByCurrency { get; }
        #endregion

        #region Methods

        void SetProcessHasChangesToTrueWithLock();
        decimal GetMaximumRateConverted(int currencyId);
        int GetRateToChangeCurrencyId(RateToChange rateToChange);

        #endregion
    }
}
