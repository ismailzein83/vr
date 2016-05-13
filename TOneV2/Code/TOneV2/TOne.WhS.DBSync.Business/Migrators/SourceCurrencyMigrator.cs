using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyMigrator 
    {
        private string _ConnectionString;
        private bool _UseTempTables;
        private DBSyncLogger _Logger;

        public SourceCurrencyMigrator(string connectionString, bool useTempTables, DBSyncLogger logger)
        {
            _UseTempTables = useTempTables;
            _Logger = logger;
            _ConnectionString = connectionString;
        }



        public  void Migrate(List<DBTable> context)
        {
            _Logger.WriteInformation("Migrating table 'Currency' started");
            var sourceItems = GetSourceItems();
            if (sourceItems != null)
            {
                List<Currency> itemsToAdd = new List<Currency>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem, context);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd, context);
            }
            _Logger.WriteInformation("Migrating table 'Currency' ended");
        }

        private  void AddItems(List<Currency> itemsToAdd, List<DBTable> context)
        {
            CurrencyDBSyncManager CurrencyManager = new CurrencyDBSyncManager(_UseTempTables);
            CurrencyManager.ApplyCurrenciesToTemp(itemsToAdd);
            DBTable dbTableCurrency = context.Where(x => x.Name == Constants.Table_Currency).FirstOrDefault();
            if (dbTableCurrency != null)
                dbTableCurrency.Records = CurrencyManager.GetCurrencies();
        }

        private IEnumerable<SourceCurrency> GetSourceItems()
        {
            SourceCurrencyDataManager dataManager = new SourceCurrencyDataManager(_ConnectionString);
            return dataManager.GetSourceCurrencies();
        }

        private  Currency BuildItemFromSource(SourceCurrency sourceItem, List<DBTable> context)
        {
            return new Currency
            {
                Name = sourceItem.Name,
                Symbol = sourceItem.Symbol,
                SourceId = sourceItem.SourceId
            };
        }
    }
}
