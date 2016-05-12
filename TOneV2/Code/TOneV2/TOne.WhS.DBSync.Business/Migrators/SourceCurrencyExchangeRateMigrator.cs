﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.EntityMigrator;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyExchangeRateMigrator : SourceItemMigrator<SourceCurrencyExchangeRate, CurrencyExchangeRate, SourceCurrencyExchangeRateMigratorReader>
    {
        public SourceCurrencyExchangeRateMigrator(SourceCurrencyExchangeRateMigratorReader sourceCurrencyExchangeRateMigratorReader)
            : base(sourceCurrencyExchangeRateMigratorReader)
        {

        }

        public override void Migrate()
        {
            base.Migrate();
        }

        protected override void AddItems(List<CurrencyExchangeRate> itemsToAdd)
        {
            CurrencyExchangeRateDBSyncManager CurrencyExchangeRateManager = new CurrencyExchangeRateDBSyncManager();
            CurrencyExchangeRateManager.ApplyCurrencyExchangeRatesToTemp(itemsToAdd);
        }

        protected override CurrencyExchangeRate BuildItemFromSource(SourceCurrencyExchangeRate sourceItem)
        {
            CurrencyDBSyncManager currencyManager = new CurrencyDBSyncManager();
            var currency = currencyManager.GetCurrencyBySourceId(sourceItem.CurrencyId);
            if (currency != null)
                return new CurrencyExchangeRate
                {
                    CurrencyId = currency.CurrencyId,
                    ExchangeDate = (sourceItem.ExchangeDate.HasValue ? sourceItem.ExchangeDate.Value : DateTime.Now),
                    Rate = (sourceItem.Rate.HasValue ? sourceItem.Rate.Value : Decimal.MinValue),
                    SourceId = sourceItem.SourceId
                };
            else
                return null;
        }
    }
}
