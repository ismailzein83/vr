using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

namespace CallServiceClient
{
    public partial class Form1 : Form
    {
        public class MyPolicy : System.Net.ICertificatePolicy
        {
            public bool CheckValidationResult(System.Net.ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Net.WebRequest request, int certificateProblem)
            {
                return true;
            }
        }
        public Form1()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Execute();
            //CallWebAPITestMethodFromRestSharp();
            CallASMXMyTestMethodFromRestSharp();
            //CallWebAPIFromRestSharp();
            //CallSVCTestMethodFromRestSharp();
            //CallASMXTestMethodFromRestSharp();
            CallWebAPIGetObjectFromRestSharp();
            RestSharp.RestRequest request = new RestSharp.RestRequest();
            //request.AddXmlBody("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>");

            //var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><DoWork xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>";

            //request.AddBody(new System.Net.Http.StringContent(soapXml, Encoding.UTF8, "text/xml"));

            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>", RestSharp.ParameterType.RequestBody);

            request.AddHeader("SOAPAction", "http://tempuri.org/HelloWorld");
            //request.AddHeader("Content-Type", "text/xml; charset=utf-8");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.POST;
            
            // request.AddParameter("application/xml", "");
            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/WebService1.asmx");
                var response = client.Execute(request);
            
            //if(response.ResponseStatus == RestSharp.ResponseStatus.Completed && response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)

            RestSharp.RestRequest request2 = new RestSharp.RestRequest();
            request2.AddBody(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
              <soap:Body>
                <DoWork xmlns=""http://tempuri.org/IService1/"">
                  
                </DoWork>
              </soap:Body>
            </soap:Envelope>");
            request2.AddHeader("SOAPAction", "http://tempuri.org/IService1/DoWork");
            request2.AddHeader("Content-Type", "text/xml;charset=\"utf-8\"");
            request2.AddHeader("Accept", "text/xml");
            request2.Method = RestSharp.Method.POST;
            // request.AddParameter("application/xml", "");
            RestSharp.RestClient client2 = new RestSharp.RestClient("http://localhost:1206/Service1.svc");
            var response2 = client2.Execute(request2);
            
        }

        private void CallSVCTestMethodFromRestSharp()
        {
            RestSharp.RestRequest request = new RestSharp.RestRequest();

            var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><TestMethod2 xmlns=\"http://vanrise.bp.services\" xmlns:a=\"http://schemas.datacontract.org/2004/07/WebApplication2\" ><input><a:Text>Text Method Input Value 1</a:Text></input><input2><a:Text>Text Method Input 2</a:Text></input2></TestMethod2></soap:Body></soap:Envelope>";

            request.AddParameter("text/xml", soapXml, RestSharp.ParameterType.RequestBody);

            request.AddHeader("SOAPAction", "http://vanrise.bp.services/IService1/TestMethod2");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.POST;


            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/Service1.svc");
            //var response = client.Execute(request);
            var response = client.Execute<TestMethod2Result>(request);
        }

        private void CallASMXTestMethodFromRestSharp()
        {
            RestSharp.RestRequest request = new RestSharp.RestRequest();

            var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><TestMethod2 xmlns=\"http://tempuri.org/\"  ><input><Text>Text Method Input Value 1</Text></input><input2><Text>Text Method Input 2</Text></input2></TestMethod2></soap:Body></soap:Envelope>";

            request.AddParameter("text/xml", soapXml, RestSharp.ParameterType.RequestBody);

            request.AddHeader("SOAPAction", "http://tempuri.org/TestMethod2");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.POST;


            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/webservice1.asmx");
            //var response = client.Execute(request);
            var response = client.Execute<TestMethod2Result>(request);
        }

        public class TestMethodOutput
        {
            //[System.Runtime.Serialization.DataMember]
            public string Text1 { get; set; }

            //[System.Runtime.Serialization.DataMember]
            public string Text2 { get; set; }
        }

        public class TestMethod2Result
        {
            //[System.Runtime.Serialization.DataMember]
            public string Text1 { get; set; }

            //[System.Runtime.Serialization.DataMember]
            public string Text2 { get; set; }
        }

        public void Execute()
                        {
                            var request = new RestSharp.RestRequest();
                            request.Method = RestSharp.Method.GET;
                            request.AddParameter("input", " input sent from workflow", RestSharp.ParameterType.QueryString);

                            
                            
                            var client = new RestSharp.RestClient(String.Concat("http://localhost:1206/", "api/MyTest/TestCall"));
                            var response = client.Execute(request);
                            OnResponseReceived(response);
                        }



        void OnResponseReceived(RestSharp.IRestResponse response)
        {
            Console.WriteLine("responseContent: ", response.Content);
        }

        private void CallWebAPIFromRestSharp()
        {
            RestSharp.RestRequest request = new RestSharp.RestRequest();
            
            //request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>", RestSharp.ParameterType.RequestBody);

            //request.AddHeader("Content-Type", "text/xml; charset=utf-8");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.GET;
            request.AddParameter("input", "input value", RestSharp.ParameterType.QueryString);
            // request.AddParameter("application/xml", "");
            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/api/MyTest/TestCall");
            var response = client.Execute(request);
            MessageBox.Show(response.Content);
        }

        private void CallWebAPITestMethodFromRestSharp()
        {
            RestSharp.RestRequest request = new RestSharp.RestRequest();

            //request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>", RestSharp.ParameterType.RequestBody);

            //request.AddHeader("Content-Type", "text/xml; charset=utf-8");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.POST;
            request.AddParameter("input", "input value", RestSharp.ParameterType.QueryString);
            request.AddParameter("application/json", "{\"Text\":\"test input1 to Test Method\"}", RestSharp.ParameterType.RequestBody);
            //request.AddParameter("application/json", "{\"Text\":\"test input2 to Test Method\"}", RestSharp.ParameterType.RequestBody);
            // request.AddParameter("application/xml", "");
            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/api/MyTest/TestMethod");
            var response = client.Execute<dynamic>(request);
        }

        private void CallWebAPIGetObjectFromRestSharp()
        {
            RestSharp.RestRequest request = new RestSharp.RestRequest();

            //request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>", RestSharp.ParameterType.RequestBody);

            //request.AddHeader("Content-Type", "text/xml; charset=utf-8");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.GET;
            request.AddParameter("input", "input value", RestSharp.ParameterType.QueryString);
            // request.AddParameter("application/xml", "");
            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/api/MyTest/TestGetObject");
            var response = client.Execute(request);
            var responseStrongTyped = client.Deserialize<MyTestMethodOutput>(response);
            var responseDynamic = client.Deserialize<dynamic>(response);
            var responseFault = client.Deserialize<JSONFault>(response);
        }

        public class JSONFault
        {
            public string Message { get; set; }

            public string ExceptionMessage { get; set; }

            public string StackTrace { get; set; }
        }

        private void CallASMXMyTestMethodFromRestSharp()
        {
            RestSharp.RestRequest request = new RestSharp.RestRequest();

            //request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>", RestSharp.ParameterType.RequestBody);

            //request.AddHeader("Content-Type", "text/xml; charset=utf-8");
            //request.RequestFormat = RestSharp.DataFormat.Xml;
            //request.AddHeader("Accept", "text/xml");
            //request.Accept = "text/xml";
            request.Method = RestSharp.Method.POST;

            var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><TestMethod xmlns=\"http://tempuri.org/\"  ><input1><Text>Text Method Input Value 1</Text></input1></TestMethod></soap:Body></soap:Envelope>";

            request.AddParameter("text/xml", soapXml, RestSharp.ParameterType.RequestBody);

            request.AddHeader("SOAPAction", "http://tempuri.org/TestMethod");

            //request.AddParameter("application/json", "{\"Text\":\"test input2 to Test Method\"}", RestSharp.ParameterType.RequestBody);
            // request.AddParameter("application/xml", "");
            RestSharp.RestClient client = new RestSharp.RestClient("http://localhost:1206/webservice1.asmx");
            var response = client.Execute(request);
            var strongTypeResponse = client.Deserialize<MyTestMethodOutput>(response);
            var faultResponse = client.Deserialize<Fault>(response);
            //var strongTypeResponse2 = new VRXmlSerializer().Deserialize<MyTestMethodOutput>(response.Content);
        }

        public class Fault
        {
            public string faultcode { get; set; }

            public string faultstring { get; set; }
        }

        public class MyTestMethodOutput
        {
            public string Text1 { get; set; }

            public string Text2 { get; set; }

            public List<string> List { get; set; }

            public List<MyTestMethodOutput> ListOfObjects { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Fawry.ApplicationBusinessFacadeService serv = new Fawry.ApplicationBusinessFacadeService();

            MyClientProxy.WebService1 soapClient0 = new MyClientProxy.WebService1();
            soapClient0.TestMethod2(new MyClientProxy.TestMethodInput { Text = "input1" }, new MyClientProxy.TestMethodInput { Text = "input 2" });

            WebService1Soap.WebService1 soapClient1 = new WebService1Soap.WebService1();
            soapClient1.TestMethod2(new WebService1Soap.TestMethodInput { Text = "input1" }, new WebService1Soap.TestMethodInput { Text = "input 2" });

            WebService1.WebService1SoapClient client = new WebService1.WebService1SoapClient();
           // string response = client.TestMethod2("ertterw");

            Service1.Service1Client client2 = new Service1.Service1Client();
            var response2 = client2.DoWork();

            var response3 = client2.TestMethod2(new Service1.TestMethodInput { Text = "ttttt" }, new Service1.TestMethodInput { Text = "Input 2" });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //CallWebAPI();
            //CallASMXService();
            CallASMXTestMethodService();
            //CallSVCService();
            CallSVCTestMethodService();
        }

        private static void CallASMXService()
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:1206");
            //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
            httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/HelloWorld");

            var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><HelloWorld xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>";

            var response = httpClient.PostAsync("WebService1.asmx", new System.Net.Http.StringContent(soapXml, Encoding.UTF8, "text/xml")).Result;

            var content = response.Content.ReadAsStringAsync().Result;
        }

        private static void CallASMXTestMethodService()
        {

            using (var client = new System.Net.Http.HttpClient())
            {

                client.BaseAddress = new Uri("http://localhost:8881");
                //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "WebService1.asmx"))
                {
                    request.Headers.Add("SOAPAction", "http://tempuri.org/TestMethod2");

                    var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><TestMethod2 xmlns=\"http://tempuri.org/\" ><input><Text>Text Method Input Value</Text></input></TestMethod2></soap:Body></soap:Envelope>";
                    request.Content = new System.Net.Http.StringContent(soapXml, Encoding.UTF8, "text/xml");
                    
                    using (var response = client.SendAsync(request))
                    {
                        response.Wait();
                        string responseString = response.Result.Content.ReadAsStringAsync().Result;
                    }
                }
            }
        }

        private static void CallSVCTestMethodService()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1206");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "Service1.svc"))
                {
                    request.Headers.Add("SOAPAction", "http://vanrise.bp.services/IService1/TestMethod2");
                    
                    var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><TestMethod2 xmlns=\"http://vanrise.bp.services\" xmlns:a=\"http://schemas.datacontract.org/2004/07/WebApplication2\" ><input><a:Text>Text Method Input Value 1</a:Text></input><input2><a:Text>Text Method Input 2</a:Text></input2></TestMethod2></soap:Body></soap:Envelope>";
                    request.Content = new System.Net.Http.StringContent(soapXml, Encoding.UTF8, "text/xml");
                    
                    using (var response = client.SendAsync(request))
                    {
                        response.Wait();
                         string responseString = response.Result.Content.ReadAsStringAsync().Result;
                        // byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);// response.Result.Content.ReadAsByteArrayAsync().Result;

                        // System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
                        //xdoc.LoadXml(responseString);

                        //var responseXml = xdoc.FirstChild.FirstChild.FirstChild.FirstChild;
                        //System.Runtime.Serialization.DataContractSerializer dcSerializer = new System.Runtime.Serialization.DataContractSerializer(typeof(CallServiceClient.Service1.TestMethodOutput));
                        //var testMethodOutput1 = dcSerializer.ReadObject(new MemoryStream(responseBytes));
                        //System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TestMethod2Result));

                        //var testMethodOutput = xmlSerializer.Deserialize(new System.Xml.XmlNodeReader(responseXml)) as TestMethod2Result;
                         //var responseObj = new VRXmlSerializer().Deserialize<TestMethodOutput>(responseString); // response.Result.Content.ReadAsAsync<TestMethodOutput>();
                        //var testMethodOutput2 = responseObj.Result;
                       
                    }
                }
            }
        }

        private static void CallSVCService()
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:1206");
            //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
            httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/IService1/DoWork");

            var soapXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><DoWork xmlns=\"http://tempuri.org/\" /></soap:Body></soap:Envelope>";

            var response = httpClient.PostAsync("Service1.svc", new System.Net.Http.StringContent(soapXml, Encoding.UTF8, "text/xml")).Result;

            var content = response.Content.ReadAsStringAsync().Result;
        }

        private static void CallWebAPI()
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:1206");
            //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
           // httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/IService1/DoWork");

            var json = "{\"Text\":\"test input to Test Method\"}";

            var response = httpClient.PostAsync("api/MyTest/TestMethod", new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json")).Result;

            var content = response.Content.ReadAsStringAsync().Result;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConnectionLib.WsProxy.GenerateCodeFromWSDL();
        }

        private void btnCallNoverca_Click(object sender, EventArgs e)
        {
            NovercaMngtWS.ManagementService service = new NovercaMngtWS.ManagementService();
            
        }

        private void btnCallAudiService_Click(object sender, EventArgs e)
        {
            //var audiWSClient = new AudiWebService.AudiWebService();
            ////audiWSClient.Url = "http://localhost:8881";

            //var response1 = audiWSClient.getFXRates(
            //    new AudiWebService.getFXRatesRequest
            //    {
            //        getFXRates =
            //            new AudiWebService.GetFXRatesType
            //            {
            //                body = new AudiWebService.GetFXRatesTypeBody
            //                {
            //                    fromCurrencyList = new AudiWebService.ISO4217CurrencyType[]
            //            {
            //                new AudiWebService.ISO4217CurrencyType { code = "USD" },
            //                new AudiWebService.ISO4217CurrencyType { code = "EUR" }
            //            }
            //                }
            //            }
            //    });

            using (var client = new System.Net.Http.HttpClient())
            {

                client.BaseAddress = new Uri("http://localhost:8881");
                //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "AudiWebService2.asmx"))
                {
                    request.Headers.Add("SOAPAction", "http://www.banqueaudi.com/evs/pp.rates.FXRate/getFXRates");

                    var soapXml = @"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body>
                        <getFXRates xmlns=""http://www.banqueaudi.com/evs/pp.rates.FXRate"">

<header xmlns=""http://www.banqueaudi.com/ebo/util.cmn.EBMHeader"">
<ebmCID>0880e9ac-db5d-44cb-8118-d0731b8a8d40</ebmCID>
<ebmMID>0880e9ac-db5d-44cb-8118-d0731b8a8d40</ebmMID>
<ebmRTID>0880e9ac-db5d-44cb-8118-d0731b8a8d40</ebmRTID>
<ebmSID>CRM</ebmSID>
<ebmTimestamp>2017-12-15T08:36:16.8692591+02:00</ebmTimestamp>
<bankEntityId>BASAL</bankEntityId>
</header>
<body>
<fromCurrencyList>
<fromCurrency>
<code xmlns=""http://www.banqueaudi.com/ebo/util.cmn.Basic"">USD</code>
</fromCurrency>
<fromCurrency>
<code xmlns=""http://www.banqueaudi.com/ebo/util.cmn.Basic"">EUR</code>
</fromCurrency>
<fromCurrency>
<code xmlns=""http://www.banqueaudi.com/ebo/util.cmn.Basic"">XAU</code>
</fromCurrency>
<fromCurrency>
<code xmlns=""http://www.banqueaudi.com/ebo/util.cmn.Basic"">TRY</code>
</fromCurrency>
<fromCurrency>
<code xmlns=""http://www.banqueaudi.com/ebo/util.cmn.Basic"">GBP</code>
</fromCurrency>
</fromCurrencyList>
</body>
</getFXRates>
</soap:Body></soap:Envelope>";
                    request.Content = new System.Net.Http.StringContent(soapXml, Encoding.UTF8, "text/xml");

                    using (var response = client.SendAsync(request))
                    {
                        response.Wait();
                        string responseString = response.Result.Content.ReadAsStringAsync().Result;
                    }
                }
            }


            //AudiServices.ppratesevsFXRateClient client = new AudiServices.ppratesevsFXRateClient();
            //client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://localhost:8881/AudiWebService");
            //var fxRatesResponse = client.getFXRates(new AudiServices.GetFXRatesType
            //{
            //    body = new AudiServices.GetFXRatesTypeBody
            //    {
            //        fromCurrencyList = new AudiServices.ISO4217CurrencyType[]
            //            {
            //                new AudiServices.ISO4217CurrencyType { code = "USD" },
            //                new AudiServices.ISO4217CurrencyType { code = "EUR" }
            //            }
            //    }
            //});

            ////response.body.FXRateFullDetailsList[0].

            //var counterValueResponse = client.getCounterValueAmount(new AudiServices.GetCounterValueAmountType
            //    {
            //        body = new AudiServices.GetCounterValueAmountTypeBody
            //        {
            //            amountToConvertList = new AudiServices.AmountToConvertType[]
            //            {
            //                new AudiServices.AmountToConvertType
            //                {
            //                    amount = new AudiServices.MoneyType
            //                    {
            //                        amount = 34.4M,
            //                        currency = new AudiServices.ISO4217CurrencyType
            //                        {
            //                            code = "USD"
            //                        }
            //                    },
            //                    rateType = AudiServices.FXRateTypeType.Daily,
            //                    toCurrency = new AudiServices.ISO4217CurrencyType
            //                    {
            //                        code = ""
            //                    }
            //                }
            //            }
            //        }
            //    });
        }
    }
}
