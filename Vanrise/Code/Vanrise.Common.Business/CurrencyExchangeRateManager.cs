using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class CurrencyExchangeRateManager
    {
        public CurrencyExchangeRate GetEffectiveExchangeRate(int currencyId, DateTime effectiveOn)
        {
            var allCurrenciesExchangeRates = GetOrderedCachedCurrenciesExchangeRates();
            IOrderedEnumerable<CurrencyExchangeRate> exchangeRates = allCurrenciesExchangeRates.GetRecord(currencyId);
            if (exchangeRates != null && exchangeRates.Count() > 0)
                return exchangeRates.FindRecord(itm => itm.ExchangeDate <= effectiveOn);
            return null;
        }
        public Decimal ConvertValueToCurrency(decimal value, int currencyId, DateTime effectiveOn)
        {
            if (currencyId == new ConfigManager().GetSystemCurrencyId())
                return value;
            else
            {
                var exchangeRate = GetEffectiveExchangeRate(currencyId, effectiveOn);
                if (exchangeRate == null)
                    throw new NullReferenceException(string.Format("exchangeRate: currency Id:{0}, Effective On: {1}", currencyId, effectiveOn.ToString()));

                return value * exchangeRate.Rate;
            }
        }

        public Decimal ConvertValueToCurrency(decimal value, int fromCurrencyId, int toCurrencyId, DateTime effectiveOn)
        {
            if (fromCurrencyId == toCurrencyId)
                return value;
            else
            {
                CurrencyExchangeRate fromExchangeRate;
                CurrencyExchangeRate toExchangeRate;

                if (fromCurrencyId == new ConfigManager().GetSystemCurrencyId())
                    fromExchangeRate = new CurrencyExchangeRate() { Rate = 1 };
                else
                    fromExchangeRate = GetEffectiveExchangeRate(fromCurrencyId, effectiveOn);

                if (fromExchangeRate == null)
                    throw new NullReferenceException(string.Format("fromExchangeRate: currency Id:{0}, Effective On: {1}", fromCurrencyId, effectiveOn.ToString()));


                if (toCurrencyId == new ConfigManager().GetSystemCurrencyId())
                    toExchangeRate = new CurrencyExchangeRate() { Rate = 1 };
                else
                    toExchangeRate = GetEffectiveExchangeRate(toCurrencyId, effectiveOn);

                if (toExchangeRate == null)
                    throw new NullReferenceException(string.Format("toExchangeRate: currency Id:{0}, Effective On: {1}", toCurrencyId, effectiveOn.ToString()));

                if (fromExchangeRate.Rate == 0)
                    throw new ArgumentException(string.Format("fromExchangeRate is 0: currency Id:{0}, Effective On: {1}", fromCurrencyId, effectiveOn.ToString()));

                return value * toExchangeRate.Rate / fromExchangeRate.Rate;
            }
        }

        public Vanrise.Entities.IDataRetrievalResult<CurrencyExchangeRateDetail> GetFilteredCurrenciesExchangeRates(Vanrise.Entities.DataRetrievalInput<CurrencyExchangeRateQuery> input)
        {
            var allCurrenciesExchangeRates = GetCachedCurrenciesExchangeRates();



            var filteredExchangeRates = allCurrenciesExchangeRates.FindAllRecords((prod) =>
                 (input.Query.Currencies == null || input.Query.Currencies.Contains(prod.CurrencyId))
                  && (!input.Query.ExchangeDate.HasValue || input.Query.ExchangeDate >= prod.ExchangeDate));

            //if exchange rate is specified, retrieve only latest exchange rate for each currency
            if (filteredExchangeRates != null && input.Query.ExchangeDate.HasValue)
            {
                List<CurrencyExchangeRate> filteredExchangeRatesWithDistinctCurrencies = new List<CurrencyExchangeRate>();
                HashSet<int> addedCurrencies = new HashSet<int>();
                foreach (var ex in filteredExchangeRates.OrderByDescending(itm => itm.ExchangeDate))
                {
                    if (!addedCurrencies.Contains(ex.CurrencyId))
                    {
                        filteredExchangeRatesWithDistinctCurrencies.Add(ex);
                        addedCurrencies.Add(ex.CurrencyId);
                    }
                }
                filteredExchangeRates = filteredExchangeRatesWithDistinctCurrencies;
            }

            CurrencyExchangeRateExcelExportHandler currencyExchangeRateExcel = new CurrencyExchangeRateExcelExportHandler(input.Query);
            ResultProcessingHandler<CurrencyExchangeRateDetail> handler = new ResultProcessingHandler<CurrencyExchangeRateDetail>()
            {
                ExportExcelHandler = currencyExchangeRateExcel
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, filteredExchangeRates.ToBigResult(input, null, CurrencyExchangeRateDetailMapper), handler);
        }

        private class CurrencyExchangeRateExcelExportHandler : ExcelExportHandler<CurrencyExchangeRateDetail>
        {
            private CurrencyExchangeRateQuery _query;
            public CurrencyExchangeRateExcelExportHandler(CurrencyExchangeRateQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CurrencyExchangeRateDetail> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Exchange Date", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Main Currency" });


                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.CurrencyExchangeRateId });
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.ExchangeDate });
                    row.Cells.Add(new ExportExcelCell { Value = record.CurrencySymbol });
                    row.Cells.Add(new ExportExcelCell { Value = record.CurrencyName });
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.Rate });
                    row.Cells.Add(new ExportExcelCell { Value = record.IsMain });
                }
                context.MainSheet = sheet;
            }
        }
        public Dictionary<string, ExchangeRateInfo> GetLastExchangeRate()
        {
            var allCurrenciesExchangeRates = GetCachedCurrenciesExchangeRates();
            var filteredExchangeRates = allCurrenciesExchangeRates.FindAllRecords((prod) => (DateTime.Now >= prod.ExchangeDate));

            CurrencyManager manager = new CurrencyManager();
            var allCurrencies = manager.GetCachedCurrencies();

            Dictionary<string, ExchangeRateInfo> currencyExchangeRates = new Dictionary<string, ExchangeRateInfo>();

            foreach (var ex in filteredExchangeRates.OrderByDescending(itm => itm.ExchangeDate))
            {
                var currency = allCurrencies[ex.CurrencyId];

                if (!currencyExchangeRates.ContainsKey(currency.Symbol))
                {
                    currencyExchangeRates.Add(
                        currency.Symbol,
                        new ExchangeRateInfo()
                        {
                            Symbol = currency.Symbol,
                            CurrencyId = currency.CurrencyId,
                            ExchangeRate = ex
                        }
                    );
                }
            }

            foreach (var currency in allCurrencies.Values)
            {
                if (!currencyExchangeRates.ContainsKey(currency.Symbol))
                {
                    currencyExchangeRates.Add(
                        currency.Symbol,
                        new ExchangeRateInfo()
                        {
                            Symbol = currency.Symbol,
                            CurrencyId = currency.CurrencyId
                        }
                    );
                }
            }

            return currencyExchangeRates;


        }


        public void SwapNewExchangeRateWithEEDTable(List<ConnectionStringSetting> connectionStrings)
        {
            var ordredExchangeRates = GetCachedCurrenciesExchangeRates().ToList().OrderBy(r => r.Value.CurrencyId).ThenByDescending(r => r.Value.ExchangeDate);
            List<ExchangeRateWithEED> newrates = new List<ExchangeRateWithEED>();
            ExchangeRateWithEED last = new ExchangeRateWithEED();
            foreach (var nr in ordredExchangeRates)
            {
                if (nr.Value.CurrencyId == last.CurrencyId)
                {

                    ExchangeRateWithEED current = new ExchangeRateWithEED()
                    {
                        CurrencyId = nr.Value.CurrencyId,
                        Rate = nr.Value.Rate,
                        BED = nr.Value.ExchangeDate,
                        EED = last.BED
                    };
                    newrates.Add(current);
                    last = current;

                }
                else
                {
                    ExchangeRateWithEED current = new ExchangeRateWithEED()
                    {
                        CurrencyId = nr.Value.CurrencyId,
                        Rate = nr.Value.Rate,
                        BED = nr.Value.ExchangeDate
                    };
                    newrates.Add(current);
                    last = current;
                }
            }
            foreach (var c in connectionStrings)
            {
                ICurrencyExchangeRateWithEEDDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateWithEEDDataManager>();
                dataManager.ConnectionString = c.ConnectionString;
                dataManager.ConnectionStringName = c.ConnectionStringName;
                dataManager.ApplyExchangeRateWithEESInDB(newrates);
            }



        }

        public IEnumerable<CurrencyExchangeRate> GetAllCurrenciesExchangeRate()
        {
            var allExchangeRates = GetCachedCurrenciesExchangeRates();
            if (allExchangeRates == null)
                return null;

            return allExchangeRates.Values;
        }
        public Dictionary<long, CurrencyExchangeRate> GetCachedCurrenciesExchangeRates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCurrenciesExchangeRate",
               () =>
               {
                   ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
                   IEnumerable<CurrencyExchangeRate> exchangeRates = dataManager.GetCurrenciesExchangeRate();
                   return exchangeRates.ToDictionary(ex => ex.CurrencyExchangeRateId, ex => ex);
               });
        }

        public Dictionary<int, IOrderedEnumerable<CurrencyExchangeRate>> GetOrderedCachedCurrenciesExchangeRates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOrderedCachedCurrenciesExchangeRates",
               () =>
               {
                   Dictionary<int, IOrderedEnumerable<CurrencyExchangeRate>> exchangeRateList = new Dictionary<int, IOrderedEnumerable<CurrencyExchangeRate>>();

                   Dictionary<long, CurrencyExchangeRate> data = GetCachedCurrenciesExchangeRates();
                   if (data != null)
                   {
                       Dictionary<int, List<CurrencyExchangeRate>> result = new Dictionary<int, List<CurrencyExchangeRate>>();
                       foreach (KeyValuePair<long, CurrencyExchangeRate> item in data)
                       {
                           List<CurrencyExchangeRate> exchangeRates;
                           if (result.TryGetValue(item.Value.CurrencyId, out exchangeRates))
                           {
                               exchangeRates.Add(item.Value);
                           }
                           else
                           {
                               exchangeRates = new List<CurrencyExchangeRate>();
                               exchangeRates.Add(item.Value);
                               result.Add(item.Value.CurrencyId, exchangeRates);
                           }
                       }

                       foreach (KeyValuePair<int, List<CurrencyExchangeRate>> item in result)
                       {
                           exchangeRateList.Add(item.Key, item.Value.OrderByDescending(itm => itm.ExchangeDate));
                       }
                   }
                   return exchangeRateList;
               });
        }
        public CurrencyExchangeRate GetCurrencyExchangeRate(int currencyExchangeRateId)
        {
            var allCurrenciesExchangeRates = GetCachedCurrenciesExchangeRates();
            return allCurrenciesExchangeRates.GetRecord(currencyExchangeRateId);
        }

        public Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail> AddCurrencyExchangeRate(CurrencyExchangeRate currencyExchangeRate)
        {
            Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int currencyExchangeRateId = -1;

            ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
            bool insertActionSucc = dataManager.Insert(currencyExchangeRate, out currencyExchangeRateId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                currencyExchangeRate.CurrencyExchangeRateId = currencyExchangeRateId;
                insertOperationOutput.InsertedObject = CurrencyExchangeRateDetailMapper(currencyExchangeRate);
            }

            return insertOperationOutput;
        }

        public void InsertExchangeRates(List<CurrencyExchangeRate> exchangeRates)
        {
            ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
            foreach (var exchangeRate in exchangeRates)
            {
                int currencyExchangeRateId = -1;
                bool insertActionSucc = dataManager.Insert(exchangeRate, out currencyExchangeRateId);

            }
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

        }
        public Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail> UpdateCurrencyExchangeRate(CurrencyExchangeRate currencyExchangeRate)
        {
            ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();

            bool updateActionSucc = dataManager.Update(currencyExchangeRate);
            Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CurrencyExchangeRateDetailMapper(currencyExchangeRate);
            }

            return updateOperationOutput;
        }

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICurrencyExchangeRateDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCurrenciesExchangeRateUpdated(ref _updateHandle);
            }
        }

        private CurrencyExchangeRateDetail CurrencyExchangeRateDetailMapper(CurrencyExchangeRate currencyExchangeRate)
        {
            CurrencyExchangeRateDetail currencyExchangeRateDetail = new CurrencyExchangeRateDetail();

            currencyExchangeRateDetail.Entity = currencyExchangeRate;

            CurrencyManager manager = new CurrencyManager();
            if (currencyExchangeRate != null)
            {
                int currencyId = currencyExchangeRate.CurrencyId;
                var currentCurrency = manager.GetCurrency(currencyId);
                currencyExchangeRateDetail.CurrencyName = currentCurrency.Name;
                currencyExchangeRateDetail.CurrencySymbol = currentCurrency.Symbol;
                if (currencyExchangeRateDetail.CurrencySymbol == manager.GetSystemCurrency().Symbol)
                    currencyExchangeRateDetail.IsMain = true;
            }

            return currencyExchangeRateDetail;
        }
        //private CurrencyExchangeRateDetail CurrencyExchangeRateSymbolMapper(CurrencyExchangeRate currencyExchangeRate)
        //{
        //    CurrencyExchangeRateDetail currencyExchangeRateDetail = new CurrencyExchangeRateDetail();

        //    currencyExchangeRateDetail.Entity = currencyExchangeRate;

        //    CurrencyManager manager = new CurrencyManager();
        //    if (currencyExchangeRate != null)
        //    {
        //        int currencyId = currencyExchangeRate.CurrencyId;
        //        currencyExchangeRateDetail.CurrencyName = manager.GetCurrency(currencyId).Name;
        //    }

        //    return currencyExchangeRateDetail;
        //}
        #endregion
    }
}
