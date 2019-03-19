using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebApplication2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AudiWCFService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AudiWCFService.svc or AudiWCFService.svc.cs at the Solution Explorer and start debugging.
    public class AudiWCFService : CallServiceClient.AudiServices.ppratesevsFXRate
    {
        public CallServiceClient.AudiServices.getCounterValueAmountResponse getCounterValueAmount(CallServiceClient.AudiServices.getCounterValueAmountRequest request)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<CallServiceClient.AudiServices.getCounterValueAmountResponse> getCounterValueAmountAsync(CallServiceClient.AudiServices.getCounterValueAmountRequest request)
        {
            throw new NotImplementedException();
        }

        public CallServiceClient.AudiServices.getFXRatesResponse getFXRates(CallServiceClient.AudiServices.getFXRatesRequest request)
        {
            return new CallServiceClient.AudiServices.getFXRatesResponse
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
        }

        public System.Threading.Tasks.Task<CallServiceClient.AudiServices.getFXRatesResponse> getFXRatesAsync(CallServiceClient.AudiServices.getFXRatesRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
