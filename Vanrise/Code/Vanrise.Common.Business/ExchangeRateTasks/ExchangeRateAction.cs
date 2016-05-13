using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;
using Vanrise.Data;

namespace Vanrise.Common.Business.ExchangeRateTasks
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
            string baseUrl = resultTaskActionArgument.URL ;
            string token = resultTaskActionArgument.Token;


            XigniteCurrencyReader CurrencyReader = new XigniteCurrencyReader(baseUrl, token);
            List<XigniteCurrency> newRates = CurrencyReader.GetRealTimeRates();

            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

            var currentExchangeRate = currencyExchangeRateManager.GetLastExchangeRate();
            List<CurrencyExchangeRate> ratesToInsert = new List<CurrencyExchangeRate>();
            foreach (var ex in newRates)
            {
                var existingExchangeRate = currentExchangeRate.FindRecord(nex => nex.Symbol == ex.Symbol);
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
               
               
            }
            currencyExchangeRateManager.InsertExchangeRates(ratesToInsert);

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        
    }
}
