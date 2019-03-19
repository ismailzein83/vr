using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApplication2
{
    /// <summary>
    /// Summary description for AudiWebService
    /// </summary>
    [WebService(Namespace = "http://www.banqueaudi.com/evs/pp.rates.FXRate")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AudiWebService : System.Web.Services.WebService, CallServiceClient.AudiServices.ppratesevsFXRate
    {

        [WebMethod]
        public string HelloWorld()
        {
            
            return "Hello World";
        }

        public CallServiceClient.AudiServices.getCounterValueAmountResponse getCounterValueAmount(CallServiceClient.AudiServices.getCounterValueAmountRequest request)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<CallServiceClient.AudiServices.getCounterValueAmountResponse> getCounterValueAmountAsync(CallServiceClient.AudiServices.getCounterValueAmountRequest request)
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public CallServiceClient.AudiServices.getFXRatesResponse getFXRates(CallServiceClient.AudiServices.getFXRatesRequest request)
        {
            var response = new CallServiceClient.AudiServices.getFXRatesResponse
            {
                getFXRatesResponse1 = new CallServiceClient.AudiServices.GetFXRatesResponseType
                {
                    body = new CallServiceClient.AudiServices.GetFXRatesResponseTypeBody
                    {
                        FXRateFullDetailsList = new CallServiceClient.AudiServices.FXRateFullDetailsType[]
                        {
                            new CallServiceClient.AudiServices.FXRateFullDetailsType
                            {
                                standardRate = 4.3M
                            }
                        }
                    }
                }
            };

            return response;
        }

        public System.Threading.Tasks.Task<CallServiceClient.AudiServices.getFXRatesResponse> getFXRatesAsync(CallServiceClient.AudiServices.getFXRatesRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
