using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using System.Linq;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Business
{
    public class CurrencyMigrator : Migrator<SourceCurrency, Currency>
    {
        CurrencyDBSyncDataManager dbSyncDataManager;
        SourceCurrencyDataManager dataManager;
        string _mainCurrencySymbol;

        public CurrencyMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CurrencyDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCurrencyDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<Currency> itemsToAdd)
        {
            dbSyncDataManager.ApplyCurrenciesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCurrency> GetSourceItems()
        {
            return dataManager.GetSourceCurrencies();
        }

        public override Currency BuildItemFromSource(SourceCurrency sourceItem)
        {
            if (sourceItem.IsMainCurrency)
                _mainCurrencySymbol = sourceItem.Symbol;

            return new Currency
            {
                Name = sourceItem.Name,
                Symbol = sourceItem.Symbol,
                SourceId = sourceItem.SourceId
            };
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            if (dbTableCurrency != null)
            {
                Dictionary<string, Currency> allCurrencies =  dbSyncDataManager.GetCurrencies(useTempTables);
                dbTableCurrency.Records = allCurrencies;
                SettingManager settingManager = new SettingManager();
                Currency currency = allCurrencies.Values.FindRecord(item => item.Symbol.Equals(_mainCurrencySymbol, System.StringComparison.InvariantCultureIgnoreCase));
                if (currency != null)
                {
                    var systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
                    (systemCurrencySetting.Data as CurrencySettingData).CurrencyId = currency.CurrencyId;
                    settingManager.UpdateSetting(systemCurrencySetting);
                }
            }
            
            
            
        }
    }
}
