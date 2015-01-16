using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS.Addons.Utilities.CurrencyExchangeRateGetter
{
    public class XigniteExchangeUpdate
    {
        internal static log4net.ILog log = log4net.LogManager.GetLogger(typeof(XigniteExchangeUpdate));
        internal class XigniteHelper
        {
            string UserName;
            string Password;
            string XMLResult;
            #region SoapMessages

            string ConvertRealTimeValueSoapMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope 
      xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
      xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
      xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Header>
    <Header xmlns=""http://www.xignite.com/services/"">
      <Username>#username#</Username>
      <Password>#password#</Password>
      <TRACER>Tracer</TRACER>
    </Header>
  </soap:Header>
  <soap:Body>
    <ConvertRealTimeValue xmlns=""http://www.xignite.com/services/"">
      <From xsi:type=""xsd:string"">#from#</From>
      <To xsi:type=""xsd:string"">#to#</To>
      <Amount xsi:type=""xsd:double"">#amount#</Amount>
    </ConvertRealTimeValue>
  </soap:Body>
</soap:Envelope>
";
            string GetAllCrossRatesForACurrencySoapMessage = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope 
      xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
      xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
      xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Header>
    <Header xmlns=""http://www.xignite.com/services/"">
      <Username>#username#</Username>
      <Password>#password#</Password>
      <TRACER>Tracer</TRACER>
    </Header>
  </soap:Header>
  <soap:Body>
    <GetAllCrossRatesForACurrency xmlns=""http://www.xignite.com/services/"">
      <Symbol xsi:type=""xsd:string"">#symbol#</Symbol>
    </GetAllCrossRatesForACurrency>
  </soap:Body>
</soap:Envelope>


";
            #endregion

            public readonly string URL = "http://www.xignite.com/xCurrencies.asmx?WSDL";
            public XigniteHelper(string UserName, string Password)
            {
                this.UserName = UserName;
                this.Password = Password;
            }
            public double ConvertRealTimeValue(string from, string to, double ammount)
            {
                try
                {
                    this.ConvertRealTimeValueSoapMessage = this.ConvertRealTimeValueSoapMessage.Replace("#username#", this.UserName);
                    this.ConvertRealTimeValueSoapMessage = this.ConvertRealTimeValueSoapMessage.Replace("#password#", this.Password);
                    this.ConvertRealTimeValueSoapMessage = this.ConvertRealTimeValueSoapMessage.Replace("#from#", from);
                    this.ConvertRealTimeValueSoapMessage = this.ConvertRealTimeValueSoapMessage.Replace("#to#", to);
                    this.ConvertRealTimeValueSoapMessage = this.ConvertRealTimeValueSoapMessage.Replace("#amount#", ammount.ToString());
                    Connect(this.ConvertRealTimeValueSoapMessage);
                    CheckForErrors();
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("(?<=<Result>)(.)*(?=</Result>)");
                    var match = regex.Match(this.XMLResult);
                    double result = double.Parse(match.Value);
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            public List<XigniteCurrency> GetAllCrossRatesForACurrency(string symbol)
            {
                this.GetAllCrossRatesForACurrencySoapMessage = this.GetAllCrossRatesForACurrencySoapMessage.Replace("#username#", this.UserName);
                this.GetAllCrossRatesForACurrencySoapMessage = this.GetAllCrossRatesForACurrencySoapMessage.Replace("#password#", this.Password);
                this.GetAllCrossRatesForACurrencySoapMessage = this.GetAllCrossRatesForACurrencySoapMessage.Replace("#symbol#", symbol);
                Connect(this.GetAllCrossRatesForACurrencySoapMessage);

                CheckForErrors();
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("(?<=<ExchangeRates>)(.|\n)*(?=</ExchangeRates>)");
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
                sb.Append("<root>");
                string match = regex.Match(this.XMLResult).Value;
                sb.Append(match);
                sb.Append("</root>");

                System.Xml.Linq.XElement document = System.Xml.Linq.XElement.Parse(sb.ToString());
                List<XigniteCurrency> result = (from q in document.Elements("CrossRate")
                                                select new XigniteCurrency()
                                                {

                                                    Name = q.Element("To").Element("Name").Value,
                                                    Symbol = q.Element("To").Element("Symbol").Value,
                                                    Message = q.Element("Message") != null ? q.Element("Message").Value : null,
                                                    Rate = float.Parse(q.Element("Rate").Value)
                                                }).ToList<XigniteCurrency>();
                return result;
            }

            private void Connect(string SoapMessage)
            {
                TABS.Addons.Utilities.WebServiceHelper webHelper = new TABS.Addons.Utilities.WebServiceHelper(this.URL);
                webHelper.Connect(SoapMessage, TABS.Addons.Utilities.ContentType.XML);
                this.XMLResult = webHelper.XMLResult;

            }
            private void CheckForErrors()
            {
                if (this.XMLResult.Contains("RegistrationError"))
                {
                    System.Text.RegularExpressions.Regex errorPattern = new System.Text.RegularExpressions.Regex("(?<=<Message>)(.|\n)*(?=</Message>)");
                    string errorMessage = errorPattern.Match(this.XMLResult).Value;
                    throw new System.ApplicationException(errorMessage);

                }
            }
        }
        internal class XigniteCurrency
        {
            public string Symbol { get; set; }
            public float Rate { get; set; }
            public string Name { get; set; }
            public string Message { get; set; }
        }

        XigniteHelper _XigniteHelper;

        public XigniteExchangeUpdate(string Username, string Password)
        {
            this._XigniteHelper = new XigniteHelper(Username, Password);
        }

        public void UpdateCurrencyExchangeRates()
        {

            List<TABS.Currency> changedCurrencies = new List<TABS.Currency>();
            List<TABS.CurrencyExchangeRate> changedRates = new List<TABS.CurrencyExchangeRate>();
            List<XigniteCurrency> newRates = null;

            try
            {
                newRates = _XigniteHelper.GetAllCrossRatesForACurrency(TABS.Currency.Main.Symbol);

            }
            catch (Exception ex)
            {
                log.Error("Error while updating Currencies from Xignite", ex);
                return;
            }
            var visibleCurr = TABS.Currency.Visible.Values.Where(c => !c.IsMainCurrency);

            foreach (var currency in visibleCurr)
            {
                TABS.Currency item = null;
                float newRate = (float)this._XigniteHelper.ConvertRealTimeValue(TABS.Currency.Main.Symbol, currency.Symbol, 1.0);

                if (TABS.Currency.All.TryGetValue(currency.Symbol, out item))
                {
                    if (newRate != item.LastRate)
                    {
                        changedCurrencies.Add(item);
                        item.LastRate = newRate;
                        item.LastUpdated = DateTime.Now;

                        TABS.CurrencyExchangeRate exRate = new TABS.CurrencyExchangeRate();
                        exRate.Currency = item;
                        exRate.ExchangeDate = DateTime.Now;
                        exRate.Rate = newRate;
                        changedRates.Add(exRate);
                    }

                }

            }
            //foreach (var rate in newRates)
            //{
            //    TABS.Currency item = null;
            //    if (TABS.Currency.All.TryGetValue(rate.Symbol, out item))
            //    {

            //        if (item.LastRate != rate.Rate)
            //        {
            //            changedCurrencies.Add(item);
            //            item.LastRate = rate.Rate;
            //            item.LastUpdated = DateTime.Now;

            //            TABS.CurrencyExchangeRate exRate = new TABS.CurrencyExchangeRate();
            //            exRate.Currency = item;
            //            exRate.ExchangeDate = DateTime.Now;
            //            exRate.Rate = rate.Rate;
            //            changedRates.Add(exRate);
            //        }
            //    }

            //}
            Exception exception;
            if (!TABS.ObjectAssembler.SaveOrUpdate(changedCurrencies, out exception))
            {
                log.Error("Error while updating Currencies");
            }

            if (!TABS.ObjectAssembler.SaveOrUpdate(changedRates, out exception))
            {
                log.Error("Error while updating ExchangeRates");
            }

            if (null == exception)
            {
                log.Info("Currencies has been updated succesfully");

            }

        }


    }
}
