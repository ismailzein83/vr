using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CLINumberLibrary;

namespace PhoneNumbersService
{
    /// <summary>
    /// Summary description for PhoneNumberService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PhoneNumberReturn
    {
        public int RequestCallId { get; set; }
        public string Number { get; set; }
        public string Prefix { get; set; }
        public string ErrorStatus { get; set; }
    }
    public class CallInfo
    {
        public int RequestCallId { get; set; }
        public string ReceivedCLI { get; set; }
        public string ErrorStatus { get; set; }
    }

    public class PhoneNumberService : System.Web.Services.WebService
    {
        
        [WebMethod]
        public PhoneNumberReturn RequestForCall(string clientName, string password, string mcc, string mnc)
        {
            Client client = ClientRepository.Load(clientName, password);

            PhoneNumberReturn phoneNumberReturn = new PhoneNumberReturn();
            if (client != null)
            {
                PhoneNumberRepository.FreeAllLockedPhone();

                PhoneNumber phoneNumber = new PhoneNumber();
                Operator operatorObj = new Operator();
                operatorObj = OperatorRepository.Load(mcc, mnc);
                if (operatorObj != null)
                {
                    RequestCall requestCall = new RequestCall();
                    requestCall.CreationDate = DateTime.Now;
                    requestCall.OperatorId = operatorObj.Id;
                    requestCall.ClientId = client.Id;

                    phoneNumber = PhoneNumberRepository.GetFreePhoneNumber(operatorObj.Id);

                    if (phoneNumber == null)
                    {
                        phoneNumberReturn.ErrorStatus = "-1";
                        RequestCallRepository.Save(requestCall);
                    }
                    else
                    {
                        requestCall.PhoneNumberId = phoneNumber.Id;
                        RequestCallRepository.Save(requestCall);

                        phoneNumberReturn.RequestCallId = requestCall.Id;
                        phoneNumberReturn.Number = phoneNumber.Number;
                        phoneNumberReturn.Prefix = phoneNumber.Prefix;
                        phoneNumberReturn.ErrorStatus = "1";
                    }
                }
                else
                    phoneNumberReturn.ErrorStatus = "-1";
            }
            else
                phoneNumberReturn.ErrorStatus = "-1";
            
            return phoneNumberReturn;
        }


        [WebMethod]
        public CallInfo ReleaseCall(string clientName, string password, int requestId)
        {
            CallInfo callInfo = new CallInfo();
            Client client = ClientRepository.Load(clientName, password);
            callInfo.ErrorStatus = "-1";
            if (client != null)
            {
                RequestCall requestCall = RequestCallRepository.Load(requestId);
                if (requestCall != null)
                {
                    callInfo.RequestCallId = requestCall.Id;
                    PhoneNumber phoneNumber = PhoneNumberRepository.Load(requestCall.PhoneNumberId.Value);
                    if (phoneNumber != null)
                    {
                        PhoneNumberRepository.FreeThisPhoneNumber(phoneNumber.Id);
                    }
                    requestCall.ReleaseDate = DateTime.Now;
                    RequestCallRepository.Save(requestCall);

                    new MySQLDataManager().GetData(phoneNumber.Number, requestCall.CreationDate.Value, requestCall.ReleaseDate.Value, (reader) =>
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                callInfo.ReceivedCLI = reader[3].ToString();
                                callInfo.ErrorStatus = "1";
                            }
                        }
                    });
                }
            }

            return callInfo;
        }
    }
}
