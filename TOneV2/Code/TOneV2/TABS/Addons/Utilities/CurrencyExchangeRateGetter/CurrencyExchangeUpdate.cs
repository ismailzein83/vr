using System;
using System.Collections.Generic;
using System.Data;

namespace TABS.Addons.Utilities.CurrencyExchangeRateGetter
{
    public class CurrencyExchangeUpdate
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.Addons.Utilities.CurrencyExchangeRateGetter");

        public static DataTable UpdateCurrencyExchangeRates()
        {
            string SupplementaryExchangeRatesActualSiteUrl = "http://fx.sauder.ubc.ca/today.html";// "http://pacific.commerce.ubc.ca/xr/rates.html";
            CurrencyReader CurrencyReader = new CurrencyReader();
            CurrencyReader.ExchangeRatesURL = SupplementaryExchangeRatesActualSiteUrl;
            DataTable DT = new DataTable();
            DT = CurrencyReader.Build();

            // The list of changed rates
            List<TABS.CurrencyExchangeRate> changedRates = new List<TABS.CurrencyExchangeRate>();
            List<TABS.Currency> changedCurrencies = new List<TABS.Currency>();

            if (DT != null)
            {
                for (int i = 0; i < DT.Rows.Count - 1; i++)
                {
                    TABS.Currency item = null;
                    if (TABS.Currency.All.TryGetValue(DT.Rows[i][0].ToString(), out item))
                    {
                        float newRate = float.Parse(DT.Rows[i][2].ToString());
                        if (newRate != item.LastRate)
                        {
                            changedCurrencies.Add(item);
                            item.LastRate = newRate;
                            item.LastUpdated = DateTime.Now;
                            TABS.CurrencyExchangeRate exchangeRate = new TABS.CurrencyExchangeRate();
                            exchangeRate.Currency = item;
                            exchangeRate.ExchangeDate = DateTime.Now;
                            exchangeRate.Rate = newRate;
                            changedRates.Add(exchangeRate);
                        }
                    }
                }

                Exception ex;
                if (!ObjectAssembler.SaveOrUpdate(changedCurrencies, out ex) || !ObjectAssembler.SaveOrUpdate(changedRates, out ex))
                    log.Error(string.Format("There was an error Updating Currencies Rates <br>" + ex.ToString()));
                else
                    log.Info(string.Format("Rates were Updated Successfully ({0} Changed)", changedRates.Count));
            }

            return DT;
        }
    }
}
