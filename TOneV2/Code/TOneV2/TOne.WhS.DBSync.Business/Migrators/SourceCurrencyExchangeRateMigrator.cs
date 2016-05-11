using System;
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
            CurrencyExchangeRateManager CurrencyExchangeRateManager = new CurrencyExchangeRateManager();
            CurrencyExchangeRateManager.AddCurrencyExchangeRatesFromSource(itemsToAdd);
        }

        protected override CurrencyExchangeRate BuildItemFromSource(SourceCurrencyExchangeRate sourceItem)
        {
            Vanrise.Common.Business.CurrencyManager currencyManager = new Vanrise.Common.Business.CurrencyManager();
            var allCurrencies = currencyManager.GetAllCurrencies();
            var currency = allCurrencies.Where(x => x.Symbol == sourceItem.CurrencyId).FirstOrDefault();
            if (currency != null)
                return new CurrencyExchangeRate
                {
                    CurrencyId = currency.CurrencyId,
                    ExchangeDate = (sourceItem.ExchangeDate.HasValue ? sourceItem.ExchangeDate.Value : DateTime.Now),
                    Rate = (sourceItem.Rate.HasValue ? sourceItem.Rate.Value : Decimal.MinValue),
                    SourceID = sourceItem.SourceId
                };
            else
                return null;
        }
    }
}
