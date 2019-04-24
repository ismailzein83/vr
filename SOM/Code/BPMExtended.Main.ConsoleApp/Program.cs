using BPMExtended.Main.Business;
using BPMExtended.Main.Common;
using System;
using System.Collections.Generic;
using System.Web;
using Terrasoft.Core;
using Vanrise.Security.Entities;

namespace BPMExtended.Main.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Runtime().CallFunc();
            Console.ReadLine();
        }

        public class Runtime
        {
            public void CallAction()
            {
                MainProxy manager = new MainProxy();

                int x = 3;
                int y = 0;

                Action action = delegate ()
                {
                    manager.Divide(x, y);
                };
                
                manager.InvokeMethod(action);
            }

            public int CallFunc()
            {
                MainProxy manager = new MainProxy();

                Func<int> getTemprature = delegate ()
                {
                    return manager.GetTemprature("3");
                };

                return manager.InvokeMehtod(getTemprature);
                //Console.WriteLine("Temprature is {0}", temprature);
            }
        }

        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }
        }
        
    //    #region testX
    //    [System.Xml.Serialization.XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //    public class BSCSRequest<T> where T : IBSCSRequestBody
    //    {
    //        [System.Xml.Serialization.XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //        public BSCSRequestHeader Header { get; set; }

    //        [System.Xml.Serialization.XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //        public T Body { get; set; }

    //        [System.Xml.Serialization.XmlIgnore]
    //        public Dictionary<string, string> NameSpaces
    //        {
    //            get
    //            {
    //                var namespaces = new Dictionary<string, string>() { { "soapenv", "http://schemas.xmlsoap.org/soap/envelope/" } };
    //                if (this.Body != null)
    //                {
    //                    var bodyNS = this.Body.NameSpaces;
    //                    if (bodyNS != null && bodyNS.Count > 0)
    //                    {
    //                        foreach (var namespaceKVP in bodyNS)
    //                        {
    //                            if (!namespaces.ContainsKey(namespaceKVP.Key))
    //                            {
    //                                namespaces.Add(namespaceKVP.Key, namespaceKVP.Value);
    //                            }
    //                        }
    //                    }
    //                }
    //                return namespaces;
    //            }
    //        }

    //        public BSCSRequest(T body)
    //        {
    //            Header = new BSCSRequestHeader();
    //            Body = body;
    //        }

    //        public BSCSRequest()
    //        {
    //            Header = new BSCSRequestHeader();
    //        }
    //    }
    //    public class BSCSRequestHeader
    //    { }
    //    public interface IBSCSRequestBody
    //    {
    //        Dictionary<string, string> NameSpaces { get; }
    //    }
    //    public class RatePlanReadRequestBody : IBSCSRequestBody
    //    {
    //        [System.Xml.Serialization.XmlElement("allowedRateplansReadRequest", Namespace = "http://ericsson.com/services/ws_CIL_7/allowedrateplansread")]
    //        public RatePlanReadRequest allowedRateplansReadRequest { get; set; }

    //        [System.Xml.Serialization.XmlIgnore]
    //        Dictionary<string, string> IBSCSRequestBody.NameSpaces => new Dictionary<string, string>() {
    //        { "ses", "http://ericsson.com/services/ws_CIL_7/sessionchange" },
    //        { "all","http://ericsson.com/services/ws_CIL_7/allowedrateplansread"}
    //  };
    //    }
    //    public class RatePlanReadRequest
    //    {
    //        [System.Xml.Serialization.XmlElement(Namespace = "http://ericsson.com/services/ws_CIL_7/allowedrateplansread")]
    //        public RatePlanReadInputAttributes inputAttributes { get; set; }

    //        [System.Xml.Serialization.XmlElement(Namespace = "http://ericsson.com/services/ws_CIL_7/allowedrateplansread")]
    //        public RatePlanReadSessionChangeRequest sessionChangeRequest { get; set; }
    //    }
    //    public class RatePlanReadInputAttributes
    //    {
    //        [System.Xml.Serialization.XmlElement(Namespace = "http://ericsson.com/services/ws_CIL_7/allowedrateplansread")]
    //        public string contractTypeIdPub { get; set; }

    //        [System.Xml.Serialization.XmlElement(Namespace = "http://ericsson.com/services/ws_CIL_7/allowedrateplansread")]
    //        public string prgCode { get; set; }
    //    }

    //    [System.Xml.Serialization.XmlType(Namespace = "http://ericsson.com/services/ws_CIL_7/sessionchange")]
    //    public class RatePlanReadSessionChangeRequest
    //    {
    //        //[System.Xml.Serialization.XmlArray(Namespace = "http://ericsson.com/services/ws_CIL_7/sessionchange")]
    //        [System.Xml.Serialization.XmlArrayItem("Item")]
    //        public List<RatePlanReadSessionChangeRequestValue> values { get; set; }
    //    }
    //    public class RatePlanReadSessionChangeRequestValue
    //    {
    //        public string Key { get; set; }

    //        public string Value { get; set; }
    //    }
    //    //Response
    //    public class allowedRateplansReadResponse
    //    {
    //        [System.Xml.Serialization.XmlArrayItem("item")]
    //        public List<RatePlan> numRp { get; set; }
    //    }
    //    public class RatePlan
    //    {
    //        public string rpcode { get; set; }
    //        public string rpcodePub { get; set; }
    //        public string rpVscode { get; set; }
    //        public string rpShdes { get; set; }
    //        public string rpDes { get; set; }
    //    }

    //    public static class SerializingManager
    //    {
    //        public static string SerializeFunction<T>(BSCSRequest<T> request) where T : IBSCSRequestBody
    //        {
    //            return new Vanrise.Common.VRXmlSerializer().Serialize(request, request.NameSpaces);
    //        }
    //    }

    //    #endregion
    //    static void Main(string[] args)
    //    {

    //        RatePlanReadRequestBody ratePlanReadRequestBody = new RatePlanReadRequestBody
    //        {
    //            allowedRateplansReadRequest = new RatePlanReadRequest
    //            {
    //                inputAttributes = new RatePlanReadInputAttributes
    //                {
    //                    contractTypeIdPub = "",
    //                    prgCode = ""
    //                },
    //                sessionChangeRequest = new RatePlanReadSessionChangeRequest
    //                {
    //                    values = new List<RatePlanReadSessionChangeRequestValue> {
    //                        new RatePlanReadSessionChangeRequestValue {
    //                        Key = "BU_ID",
    //                        Value = "2"
    //                        }
    //                    }
    //                }
    //            }
    //        };

    //        BSCSRequest<RatePlanReadRequestBody> request = new BSCSRequest<RatePlanReadRequestBody>(ratePlanReadRequestBody);

    //        string serialized = SerializingManager.SerializeFunction(request);
    //        Console.WriteLine(serialized);

    //        //using (SOMTestClient client = new SOMTestClient())
    //        //{
    //        //    while (0 == 0)
    //        //    {
    //        //        var resp = client.Post<CreateCustomerInput, WFResponse>("/api/DynamicBusinessProcess_BP/TestBSCS/StartProcess",
    //        //        new CreateCustomerInput()
    //        //        {
    //        //            InputArguments = new CreateCustomerInputArgument
    //        //            {
    //        //                CommonArgument = new BillingCommonInputArgument
    //        //                {
    //        //                },
    //        //                CustomerCategoryId = 12,
    //        //                CountryId = 206,
    //        //                CityName = "Qmishlo",
    //        //                ContactInput = new CreateCustomerContactInput
    //        //                {
    //        //                    FirstName = "Rodi",
    //        //                    LastName = "Hasan"
    //        //                }
    //        //            }
    //        //        }
    //        //        );

    //        //        if (resp != null)
    //        //        {
    //        //            Console.WriteLine(resp.ProcessId);
    //        //        }
    //        //        Console.ReadKey();
    //        //    }
    //        //}
    //    }
    //}


    //public class WFResponse
    //{
    //    public int ProcessId { get; set; }
    //}


    //public class SOMTestClient : IDisposable
    //{
    //    public UserConnection BPM_UserConnection
    //    {
    //        get
    //        {
    //            return (UserConnection)HttpContext.Current.Session["UserConnection"];
    //        }
    //    }

    //    AuthenticationToken _authToken;
    //    public SOMTestClient()
    //    {
    //        object email = "support@vanrise.com";
    //        object password = "S@pp0r!";
    //        //SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_Username", out email);
    //        //SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_Password", out password);

    //        CredentialsInput credentialsInput = new CredentialsInput() { Email = email as string, Password = password as string };
    //        var output = LocalWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(GetSOMBaseURL(),
    //             "/api/VR_Sec/Security/Authenticate", credentialsInput, null);
    //        if (output.Result != AuthenticateOperationResult.Succeeded)
    //        {
    //            throw new Exception(String.Format("Authentication to SOM failed. Result: {0}. Message: {1}", output.Result.ToString(), output.Message));
    //        }
    //        _authToken = output.AuthenticationObject;
    //    }

    //    public Q Post<T, Q>(string actionPath, T request)
    //    {
    //        Dictionary<string, string> headers = new Dictionary<string, string>();
    //        AddAuthenticationTokenToHeader(headers);
    //        return LocalWebAPIClient.Post<T, Q>(GetSOMBaseURL(), actionPath, request, headers);
    //    }

    //    public T Get<T>(string actionPath, Dictionary<string, string> headers)
    //    {
    //        return LocalWebAPIClient.Get<T>(GetSOMBaseURL(), actionPath, headers);
    //    }

    //    private string GetSOMBaseURL()
    //    {
    //        //object sombaseurl;
    //        //SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_Address", out sombaseurl);
    //        //return sombaseurl as string;
    //        return "http://127.0.0.1:5557";
    //    }

    //    private void AddAuthenticationTokenToHeader(Dictionary<string, string> headers)
    //    {
    //        headers.Add(_authToken.TokenName, _authToken.Token);
    //    }

    //    public void Dispose()
    //    {

    //    }
    }
}
