using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Data;

namespace Vanrise.Common.MainExtensions
{
    public class ExchangeRateTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            ExchangeRateTaskActionArgument resultTaskActionArgument = taskActionArgument as ExchangeRateTaskActionArgument;
            if (resultTaskActionArgument == null)
                throw new Exception("taskActionArgument  is not of type ResultTaskActionArgument ");

            if (resultTaskActionArgument.URL == null)
                throw new Exception("URL is empty ");

            if (resultTaskActionArgument.Token == null)
                throw new Exception("Token is empty ");
            if (resultTaskActionArgument.ConnectionStrings.Count ==0)
                throw new Exception("Connection Strings list is empty");
            
            string baseUrl = resultTaskActionArgument.URL ;
            string token = resultTaskActionArgument.Token;
            List<ConnectionStringSetting> connstrings = resultTaskActionArgument.ConnectionStrings;

            XigniteCurrencyReader CurrencyReader = new XigniteCurrencyReader(baseUrl, token);
            List<XigniteCurrency> newRates = CurrencyReader.GetRealTimeRates();

            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

            var currentExchangeRate = currencyExchangeRateManager.GetLastExchangeRate();
            List<CurrencyExchangeRate> ratesToInsert = new List<CurrencyExchangeRate>();
            foreach (var ex in newRates)
            {
                var existingExchangeRate = currentExchangeRate.FindRecord(nex => nex.Symbol.ToUpper() == ex.Symbol.ToUpper());
                if (existingExchangeRate != null  )
                {

                    if (existingExchangeRate.ExchangeRate != null)
                    {
                        if (ex.Rate != existingExchangeRate.ExchangeRate.Rate && existingExchangeRate.ExchangeRate.ExchangeDate.Date != DateTime.Now.Date)
                            ratesToInsert.Add(new CurrencyExchangeRate()
                            {
                                CurrencyId = existingExchangeRate.ExchangeRate.CurrencyId,
                                Rate = ex.Rate,
                                ExchangeDate = DateTime.Now.Date
                            });
                    }
                       
                    else 
                        ratesToInsert.Add(new CurrencyExchangeRate()
                        {
                            CurrencyId = existingExchangeRate.CurrencyId,
                            Rate = ex.Rate,
                            ExchangeDate = DateTime.Now.Date
                        });
                }
                else
                {
                    Vanrise.Common.LoggerFactory.GetLogger().WriteInformation("existingExchangeRate is null");
                }

               
            }
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation("currentExchangeRate Count: '{0}'", currentExchangeRate.Count);
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation("newRates Count: '{0}'", newRates.Count);
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation("ratesToInsert Count: '{0}'", ratesToInsert.Count);

            currencyExchangeRateManager.InsertExchangeRates(ratesToInsert);

            currencyExchangeRateManager.SwapNewExchangeRateWithEEDTable(connstrings);

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        
    }
}
