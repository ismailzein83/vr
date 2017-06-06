using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public abstract class NewZoneRateLocator
    {
        protected int SellingNumberPlanId { get; set; }

        protected SaleAreaSettingsData SaleAreaSettings { get; set; }

        public int DefaultCurrencyId { get; set; }

        public NewZoneRateLocator(int sellingNumberPlanId)
        {
            this.SellingNumberPlanId = sellingNumberPlanId;

            SettingManager settingManager = new SettingManager();
            this.SaleAreaSettings = settingManager.GetSetting<SaleAreaSettingsData>(TOne.WhS.BusinessEntity.Business.Constants.SaleAreaSettings);

            CurrencyManager currencyManager = new CurrencyManager();
            this.DefaultCurrencyId = currencyManager.GetSystemCurrency().CurrencyId;
        }

        public abstract IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType, ExistingRatesByZoneName effectiveExistingRatesByZoneName);

        protected List<ExistingZone> GetMatchedExistingZones(IEnumerable<CodeToAdd> codes, IEnumerable<ExistingZone> existingZones)
        {
            List<SaleCode> saleCodes = new List<SaleCode>();

            foreach (ExistingZone existingZone in existingZones)
            {
                saleCodes.AddRange(existingZone.ExistingCodes.Select(item => item.CodeEntity));
            }

            List<ExistingZone> matchedExistingZones = new List<ExistingZone>();

            CodeIterator<SaleCode> codeIterator = new CodeIterator<SaleCode>(saleCodes);
            foreach (CodeToAdd codeToAdd in codes)
            {
                SaleCode matchedCode = codeIterator.GetLongestMatch(codeToAdd.Code);
                if (matchedCode != null)
                {
                    ExistingZone existingZone = existingZones.FindRecord(item => item.ZoneId == matchedCode.ZoneId);
                    if (!matchedExistingZones.Contains(existingZone))
                        matchedExistingZones.Add(existingZone);
                }
            }

            return matchedExistingZones;
        }

        protected int GetCurrencyForNewRate(int ownerId, SalePriceListOwnerType ownerType)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
                return this.DefaultCurrencyId;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount customer = carrierAccountManager.GetCarrierAccount(ownerId);

            if (customer == null)
                throw new DataIntegrityValidationException(String.Format("Customer with id {0} does not exist", ownerId));

            if (customer.CarrierAccountSettings == null)
                throw new DataIntegrityValidationException(String.Format("Carrier Account Settings for customer with id {0} does not exist", ownerId));

            int currencyId = customer.CarrierAccountSettings.CurrencyId;
            if (currencyId == 0)
                throw new DataIntegrityValidationException(String.Format("No currency set for customer with id {0}", ownerId));

            return currencyId;
        }

        protected List<NewZoneRateEntity> GetHighestRatesFromZoneMatchesSaleEntities(IEnumerable<ExistingZone> matchedZones, ExistingRatesByZoneName existingRatesByZoneName)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            ExistingRatesByOwner existingRatesByOwner = new ExistingRatesByOwner();

            List<ExistingRate> effectiveExistingRates;
            foreach (ExistingZone existingZone in matchedZones)
            {
                if (existingRatesByZoneName.TryGetValue(existingZone.Name, out effectiveExistingRates))
                {
                    foreach (ExistingRate existingRate in effectiveExistingRates)
                    {
                        //Comparison will only occur with normal rates
                        if (existingRate.RateEntity.RateTypeId != null)
                            continue;

                        SalePriceList salePriceList = salePriceListManager.GetPriceList(existingRate.RateEntity.PriceListId);
                        if (salePriceList.OwnerType == SalePriceListOwnerType.Customer)
                        {
                            CarrierAccount customer = carrierAccountManager.GetCarrierAccount(salePriceList.OwnerId);
                            if (customer.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
                                continue;
                        }
                        existingRatesByOwner.TryAddValue((int)salePriceList.OwnerType, salePriceList.OwnerId, existingRate);
                    }
                }
            }

            List<NewZoneRateEntity> ratesEntities = new List<NewZoneRateEntity>();

            var e = existingRatesByOwner.GetEnumerator();
            while (e.MoveNext())
            {
                Owner owner = existingRatesByOwner.GetOwner(e.Current.Key);
                int? currencyId;
                Decimal zoneRateValue = GetHighestRate(e.Current.Value, out currencyId);
                NewZoneRateEntity zoneRate = new NewZoneRateEntity
                {
                    OwnerId = owner.OwnerId,
                    OwnerType = owner.OwnerType,
                    CurrencyId = currencyId,
                    Rate = zoneRateValue
                };
                ratesEntities.Add(zoneRate);
            }
            return ratesEntities;
        }

        private Decimal GetHighestRate(IEnumerable<ExistingRate> existingRates, out int? currencyId)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            currencyId = null;
            if (existingRates == null || !existingRates.Any()) return 0;

            var highestRate = existingRates.Max();
            currencyId = saleRateManager.GetCurrencyId(highestRate.RateEntity);
            return highestRate.RateEntity.Rate;
        }

    }
}
