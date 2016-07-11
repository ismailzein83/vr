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
            List<ConnectionStringSetting> connstrings = resultTaskActionArgument.ConnectionStrings;
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            WriteInformation(String.Format("Currency Exchange Rates task is started"));

            if (resultTaskActionArgument.EnableRateUpdate)
            {
            WriteInformation("Updating Currency Exchange Rates...");

            if (resultTaskActionArgument.URL == null)
                throw new Exception("URL is empty ");

            if (resultTaskActionArgument.Token == null)
                throw new Exception("Token is empty ");
           
                string baseUrl = resultTaskActionArgument.URL;
                string token = resultTaskActionArgument.Token;
               

                XigniteCurrencyReader CurrencyReader = new XigniteCurrencyReader(baseUrl, token);
                List<string> notsupportedSymbols;
                List<string> changedSymbols =  new List<string>();
                List<string> notChangedSymbols = new List<string>();
                List<XigniteCurrency> newRates = CurrencyReader.GetRealTimeRates(out  notsupportedSymbols);
                
                var currentExchangeRate = currencyExchangeRateManager.GetLastExchangeRate();
                List<CurrencyExchangeRate> ratesToInsert = new List<CurrencyExchangeRate>();
                foreach (var ex in newRates)
                {
                    var existingExchangeRate = currentExchangeRate.FindRecord(nex => nex.Symbol.ToUpper() == ex.Symbol.ToUpper());
                    if (existingExchangeRate != null)
                    {

                        if (existingExchangeRate.ExchangeRate != null)
                        {
                            if (ex.Rate != existingExchangeRate.ExchangeRate.Rate && existingExchangeRate.ExchangeRate.ExchangeDate.Date != DateTime.Now.Date)
                            {
                                 ratesToInsert.Add(new CurrencyExchangeRate()
                                {
                                    CurrencyId = existingExchangeRate.ExchangeRate.CurrencyId,
                                    Rate = ex.Rate,
                                    ExchangeDate = DateTime.Now.Date
                                });
                                 changedSymbols.Add(ex.Symbol);
                            }
                            else
                            {
                                notChangedSymbols.Add(ex.Symbol);
                            }
                               
                        }

                        else
                        {
                            ratesToInsert.Add(new CurrencyExchangeRate()
                            {
                                CurrencyId = existingExchangeRate.CurrencyId,
                                Rate = ex.Rate,
                                ExchangeDate = DateTime.Now.Date
                            });
                            changedSymbols.Add(ex.Symbol);
                        }
                           
                    }
                    else
                    {
                        WriteWarning(String.Format("Currency with symbol {0} has no exchange rate found", existingExchangeRate.Symbol.ToUpper()));

                    }


                }


                if (changedSymbols.Count > 0)
                    WriteInformation(String.Format("{0} currency exchange rates updated ({1})", changedSymbols.Count.ToString(), String.Join(",", changedSymbols)));

                if (notChangedSymbols.Count > 0)
                    WriteInformation(String.Format("{0} currency exchange rates not changed ({1})", notChangedSymbols.Count.ToString(), String.Join(",", notChangedSymbols)));


                if (notsupportedSymbols.Count > 0)
                    WriteWarning(String.Format("{0} currency not supported ({1})", notsupportedSymbols.Count.ToString(), String.Join(",", notsupportedSymbols)));

               


                currencyExchangeRateManager.InsertExchangeRates(ratesToInsert);



                WriteInformation("Updating Currency Exchange Rates is done");

            }

            if (connstrings != null && connstrings.Count > 0)
            {
                WriteInformation(String.Format("synchornizing Currency Exchange Rates with other databases({0} database{1}).", connstrings.Count.ToString(), connstrings.Count>1?"s":"" ));

                currencyExchangeRateManager.SwapNewExchangeRateWithEEDTable(connstrings);

                WriteInformation("synchornizing Currency Exchange Rates with other databases is done");
            }
              

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput
            {
                Result = ExecuteOutputResult.Completed
            };
            WriteInformation("Currency Exchange Rates task is done");
            return output;
        }

        private void WriteInformation(string message)
        {
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation(message);
        }
        private void WriteWarning(string message)
        {
            Vanrise.Common.LoggerFactory.GetLogger().WriteWarning(message);
        }
    }
}
