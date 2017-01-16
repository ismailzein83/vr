﻿using System;
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

        public abstract IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType);

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

            if(customer == null)
                throw new DataIntegrityValidationException(String.Format("Customer with id {0} does not exist", ownerId));

            if (customer.CarrierAccountSettings == null)
                throw new DataIntegrityValidationException(String.Format("Carrier Account Settings for customer with id {0} does not exist", ownerId));

            int currencyId = customer.CarrierAccountSettings.CurrencyId;
            if (currencyId == 0)
                throw new DataIntegrityValidationException(String.Format("No currency set for customer with id {0}", ownerId));

            return currencyId;
        }

        protected List<NewZoneRateEntity> GetHighestRatesFromZoneMatchesSaleEntities(IEnumerable<ExistingZone> matchedZones)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            ExistingRatesByOwner existingRatesByOwner = new ExistingRatesByOwner();

            foreach (ExistingZone existingZone in matchedZones)
            {
                IEnumerable<ExistingRate> normalExistingRates = existingZone.ExistingRates.FindAllRecords(itm => !itm.RateEntity.RateTypeId.HasValue);
                if (normalExistingRates != null && normalExistingRates.Count() > 0)
                {
                    ExistingRate lastNormalExistingRate = normalExistingRates.OrderBy(itm => itm.BED).Last();
                    SalePriceList salePriceList = salePriceListManager.GetPriceList(lastNormalExistingRate.RateEntity.PriceListId);
                    existingRatesByOwner.TryAddValue((int)salePriceList.OwnerType, salePriceList.OwnerId, lastNormalExistingRate);
                }
            }

            List<NewZoneRateEntity> ratesEntities = new List<NewZoneRateEntity>();

            var e = existingRatesByOwner.GetEnumerator();
            while (e.MoveNext())
            {
                Owner owner = existingRatesByOwner.GetOwner(e.Current.Key);
                int ownerCurrencyId = GetCurrencyForNewRate(owner.OwnerId, owner.OwnerType);

                NewZoneRateEntity zoneRate = new NewZoneRateEntity()
                {
                    OwnerId = owner.OwnerId,
                    OwnerType = owner.OwnerType,
                    CurrencyId = ownerCurrencyId,
                    Rate = this.GetHighestRate(e.Current.Value, ownerCurrencyId)
                };

                ratesEntities.Add(zoneRate);
            }

            return ratesEntities;
        }

        private Decimal GetHighestRate(IEnumerable<ExistingRate> existingRates, int targetCurrencyId)
        {
            if(existingRates != null && existingRates.Count() > 0)
            {
                CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
                SaleRateManager saleRateManager = new SaleRateManager();
                List<decimal> convertedRates = new List<decimal>();
                
                foreach (ExistingRate item in existingRates)
                {
                    int rateCurrencyId = saleRateManager.GetCurrencyId(item.RateEntity);
                    decimal convertedRate = currencyExchangeRateManager.ConvertValueToCurrency(item.RateEntity.Rate, rateCurrencyId, targetCurrencyId, DateTime.Now);
                    convertedRates.Add(convertedRate);
                }

                return convertedRates.Max();
            }

            return 0;
        }
        
    }
}
