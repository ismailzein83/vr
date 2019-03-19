using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApplication2
{
    /// <summary>
    /// Summary description for AudiWebService2
    /// </summary>
    [WebService(Namespace = "http://www.banqueaudi.com/evs/pp.rates.FXRate")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AudiWebService2 : System.Web.Services.WebService
    {
        static Dictionary<string, CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType> s_exchRatesByCurrency = new Dictionary<string, CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType>
            {
                {"USD", new CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType
                    {
                         standardRate = 1507.5000000M,
                         standardRateSpecified = true,
                         buyingRate = 1506.0000000M,
                         buyingRateSpecified = true,
                         sellingRate = 1514.0000000M,
                         sellingRateSpecified = true,
                         rateDateTime = DateTime.Parse("2019-03-18"),
                         rateDateTimeSpecified = true
                    }
                },
                {"EUR", new CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType
                    {
                         standardRate = 1994.9000000M,
                         standardRateSpecified = true,
                         buyingRate = 1890.0000000M,
                         buyingRateSpecified = true,
                         sellingRate = 2094.7000000M,
                         sellingRateSpecified = true,
                         rateDateTime = DateTime.Parse("2019-03-17"),
                         rateDateTimeSpecified = true
                    }
                },
                {"XAU", new CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType
                    {
                         standardRate = 52815.5000000M,
                         standardRateSpecified = true,
                         buyingRate = 30315.6000000M,
                         buyingRateSpecified = true,
                         sellingRate = 45037.3200000M,
                         sellingRateSpecified = true,
                         rateDateTime = DateTime.Parse("2019-03-16"),
                         rateDateTimeSpecified = true
                    }
                },
                {"TRY", new CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType
                    {
                         standardRate = 706.1900000M,
                         standardRateSpecified = true,
                         buyingRate = 701.6000000M,
                         buyingRateSpecified = true,
                         sellingRate = 707.8200000M,
                         sellingRateSpecified = true,
                         rateDateTime = DateTime.Parse("2019-03-19"),
                         rateDateTimeSpecified = true
                    }
                },
                {"GBP", new CallServiceClient.AudiWSDLWebServiceProxy.FXRateFullDetailsType
                    {
                         standardRate = 2053.1100000M,
                         standardRateSpecified = true,
                         buyingRate = 2010.0000000M,
                         buyingRateSpecified = true,
                         sellingRate = 707.8200000M,
                         sellingRateSpecified = true,
                         rateDateTime = DateTime.Parse("2019-03-16"),
                         rateDateTimeSpecified = true
                    }
                }
            };
        [WebMethod]
        public CallServiceClient.AudiWSDLWebServiceProxy.GetFXRatesResponseType getFXRates(
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.banqueaudi.com/ebo/util.cmn.EBMHeader")]CallServiceClient.AudiWSDLWebServiceProxy.HeaderType header
            , CallServiceClient.AudiWSDLWebServiceProxy.GetFXRatesTypeBody body)
        {
            var response = new CallServiceClient.AudiWSDLWebServiceProxy.GetFXRatesResponseType
                {
                    body = new CallServiceClient.AudiWSDLWebServiceProxy.GetFXRatesResponseTypeBody
                    {
                        FXRateFullDetailsList = s_exchRatesByCurrency.Where(fxRateEntry => body.fromCurrencyList.Any(cur => cur.code == fxRateEntry.Key)).Select(fxRateEntry => fxRateEntry.Value).ToArray()
                    }
                };

            return response;
        }
    }
}
