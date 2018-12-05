using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;



namespace Vanrise.Common.MainExtensions
{
    public class XigniteCurrencyReader
    {
        string _baseUrl ;
        string _token;  
        

        private string GetApiObject(string urls)
        {
            string result = String.Empty;
            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(urls);
            requests.Method = "GET";
            var httpResponse = (HttpWebResponse)requests.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            httpResponse.Close();
            return result;
        }
        internal class XigniteFormatCurrency
        {
            public string QuoteCurrency { get; set; }
            public decimal Mid { get; set; }

            public string Outcome { get; set; }
        }
        public XigniteCurrencyReader(string baseUrl, string token)
        {
            this._baseUrl = baseUrl;
            this._token = token;
        }

        private string GetSymbols()
        {
            string result = "";
            CurrencyManager manager = new CurrencyManager();
            var allCurrencies = manager.GetCachedCurrencies();
            Currency mainCurrency = manager.GetSystemCurrency();
            foreach (var currency in allCurrencies.Values)
            {
                    result += String.Format("{0}{1},", mainCurrency.Symbol.ToUpper(), currency.Symbol.ToUpper());
               
            }
            return result.Remove(result.Length - 1, 1);
        }

        public List<XigniteCurrency> GetRealTimeRates(out List<string> notsupportedCount)
        {
            notsupportedCount = new List<string>();            
            string urlTemp = string.Format("{0}/xGlobalCurrencies.json/GetRealTimeRates?_token={1}&Symbols={2}", this._baseUrl, this._token, GetSymbols());
            string resultString = GetApiObject(urlTemp);
            List<XigniteFormatCurrency> realTimeRates = Serializer.Deserialize<List<XigniteFormatCurrency>>(resultString);
            List<XigniteCurrency> formatedCurrencies = new List<XigniteCurrency>();
            foreach (var rc in realTimeRates)
            {
                if (rc.QuoteCurrency != null && rc.Outcome.Equals("Success"))
                {
                    formatedCurrencies.Add(new XigniteCurrency()
                    {
                        Symbol = rc.QuoteCurrency,
                        Rate = rc.Mid
                    });
                }
                   
                else
                {
                    notsupportedCount.Add(rc.QuoteCurrency);
                }
            }
            return formatedCurrencies;
        }


    }
}
