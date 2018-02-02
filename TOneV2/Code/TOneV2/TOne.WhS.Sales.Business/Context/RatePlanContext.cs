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
            CurrencyManager currencyManager = new CurrencyManager();
            var systemCurrency = currencyManager.GetSystemCurrency();
            if (systemCurrency == null)
                throw new DataIntegrityValidationException("System Currency was not found");
            _systemCurrencyId = systemCurrency.CurrencyId;
            LongPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            DateFormat = new Vanrise.Common.Business.GeneralSettingsManager().GetDateFormat();
            PriceListCreationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public Dictionary<int, decimal> MaximumRateConvertedByCurrency { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public int OwnerSellingNumberPlanId { get; set; }
        public DateTime MinimumZoneBED { get; set; }
        public int CurrencyId { get; set; }
        public int SellingProductCurrencyId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public SaleEntityZoneRateLocator RateLocator { get; set; }
        public SaleEntityZoneRateLocator FutureRateLocator { get; set; }
        public SaleEntityZoneRateLocator ActionRateLocator { get; set; }
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
        public InheritedRatesByZoneId InheritedRatesByZoneId { get; set; }
        public Dictionary<int, List<ExistingZone>> ExistingZonesByCountry { get; set; } // Should be removed; EffectiveAndFutureExistingZonesByCountry should be used instead
        public Dictionary<int, List<ExistingZone>> EffectiveAndFutureExistingZonesByCountry { get; set; }
        public bool ProcessHasChanges
        {
            get
            {
                return _processHasChanges;
            }
        }
        public decimal MaximumRate { get; set; }
        public int LongPrecision { get; set; }
        public bool? IsFirstSellingProductOffer { get; set; }
        public IEnumerable<DataByZone> DataByZoneList { get; set; }
        public string DateFormat { get; set; }
        public DateTime PriceListCreationDate { get; set; }

        #endregion

        #region Methods

        public void setRatePlanContextPricingSettings(PricingSettings pricingSettings)
        {
            MaximumRate = pricingSettings.MaximumRate.Value;
            _retroactiveDate = DateTime.Now.Date.AddDays(-pricingSettings.RetroactiveDayOffset.Value);
        }

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

        public bool IsSubscriberOwner { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public interface IRatePlanContext
    {
        #region Properties

        SalePriceListOwnerType OwnerType { get; }
        int OwnerId { get; }
        int OwnerSellingNumberPlanId { get; }
        DateTime MinimumZoneBED { get; }
        int CurrencyId { get; }
        int SellingProductCurrencyId { get; }
        DateTime EffectiveDate { get; }
        SaleEntityZoneRateLocator RateLocator { get; }
        SaleEntityZoneRateLocator FutureRateLocator { get; }
        SaleEntityZoneRateLocator ActionRateLocator { get; }
        DateTime RetroactiveDate { get; }
        EffectiveAfterCustomerZoneRatesByZone EffectiveAfterCustomerZoneRatesByZone { get; }
        InheritedRatesByZoneId InheritedRatesByZoneId { get; }
        Dictionary<int, List<ExistingZone>> ExistingZonesByCountry { get; }
        Dictionary<int, List<ExistingZone>> EffectiveAndFutureExistingZonesByCountry { get; }
        bool ProcessHasChanges { get; }
        decimal MaximumRate { get; }
        Dictionary<int, decimal> MaximumRateConvertedByCurrency { get; }
        int LongPrecision { get; }
        bool? IsFirstSellingProductOffer { get; }
        IEnumerable<DataByZone> DataByZoneList { get; }
        string DateFormat { get; }
        DateTime PriceListCreationDate { get; }
        bool IsSubscriberOwner { get; }
        long RootProcessInstanceId { get; }

        #endregion

        #region Methods

        void SetProcessHasChangesToTrueWithLock();
        decimal GetMaximumRateConverted(int currencyId);
        int GetRateToChangeCurrencyId(RateToChange rateToChange);

        #endregion
    }
}
