<%@ WebHandler Language="C#" Class="HandlerGetTestOperator" %>

using System;
using System.Collections.Generic;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;

public class HandlerGetTestOperator :  IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    private class RequestTestCall
    {
        public string [] id { get; set; }
    }

    private class ResponseTestOperator
    {
        public ResponseTestOperator()
        {
            needRedirect = false;
        }
        
        public string Id { get; set; }
        public string OperatorId { get; set; }
        public string Prefix { get; set; }
        public string CreationDate { get; set; }
        public string EndDate { get; set; }
        public string ReceivedCli { get; set; }
        public string Status { get; set; }
        public string TestCli { get; set; }
        public string ErrorMessage { get; set; }
        public string DisplayMessage { get; set; }
        public string DisplayMessageStatus { get; set; }
        public string progressNbr { get; set; }
        public string Redirect { get; set; }
        public bool needRedirect { get; set; }
    }
    
    public void ProcessRequest (HttpContext context) {

        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonString = String.Empty;

        if (!Current.getCurrentUser(context).IsAuthenticated)
        {
            ResponseTestOperator responseRed = new ResponseTestOperator();
            responseRed.Redirect = "Login.aspx";
            responseRed.needRedirect = true;

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(jsonSerializer.Serialize(responseRed));
            return;
        }

        try
        {
            context.Request.InputStream.Position = 0;
            using (var inputStream = new System.IO.StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }


            var emplList = jsonSerializer.Deserialize<System.Collections.Generic.List<RequestTestCall>>(jsonString);

            RequestTestCall resp = emplList[0];


            String s = resp.id[0];

            // List<RequestTestCall> resp = emplList;
            List<ResponseTestOperator> LstresponseTestOp = new List<ResponseTestOperator>();

            List<TestOperator> Lsttest = new List<TestOperator>();
            Lsttest = TestOperatorRepository.GetTestCalls(Current.getCurrentUser(context).Id);

            foreach (String id in resp.id)
            {
                ResponseTestOperator responseTestOp = new ResponseTestOperator();
                int TestOperatorId = 0;
                int.TryParse(id, out TestOperatorId);

                TestOperator testOp = TestOperatorRepository.Load(TestOperatorId);

                if (testOp != null)
                {
                    responseTestOp.Id = testOp.Id.ToString();
                    responseTestOp.CreationDate = testOp.CreationDate.ToString();
                    responseTestOp.EndDate = "";
                    responseTestOp.OperatorId = OperatorRepository.Load(testOp.OperatorId.Value).FullName;
                    responseTestOp.Prefix = testOp.CarrierPrefix;
                    responseTestOp.ReceivedCli = "";
                    responseTestOp.Status = "";
                    responseTestOp.TestCli = "";
                    responseTestOp.ErrorMessage = "";
                    //Expiry seconds
                    int exptime = 0;
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["ExpiryTimeRequestCall"], out exptime);

                    MontyCall m = MontyCallRepository.LoadbyTestOperatorId(testOp.Id);

                    if(testOp.EndDate != null)
                    if ((DateTime.Now.Second - testOp.CreationDate.Value.Second) > exptime)
                    {
                        if (m == null)
                        {
                            testOp.EndDate = DateTime.Now;
                            testOp.ErrorMessage = "Expired - more then " + exptime.ToString() + " seconds without free number";
                            TestOperatorRepository.Save(testOp);
                        }
                    }

                    if (m != null)
                        if (m.CreationDate != null)
                            responseTestOp.progressNbr = "0";

                    if (testOp.EndDate != null)
                    {
                        responseTestOp.EndDate = testOp.EndDate.ToString();

                        if (testOp.TestCli != null)
                            responseTestOp.TestCli = testOp.TestCli.ToString();
                        else
                            responseTestOp.TestCli = "";

                        if (testOp.ReceivedCli != null)
                            responseTestOp.ReceivedCli = testOp.ReceivedCli.ToString();
                        else
                            responseTestOp.ReceivedCli = "";

                        if (testOp.Status.ToString() == ((int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid).ToString())
                            responseTestOp.Status = "CLI DELIVERED";
                        else if (testOp.Status.ToString() == ((int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid).ToString())
                            responseTestOp.Status = "CLI NOT DELIVERED";
                        else if (testOp.Status.ToString() == ((int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Expired).ToString())
                            responseTestOp.Status = "FAILED";
                        else if (testOp.Status.ToString() == ((int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Waiting).ToString())
                            responseTestOp.Status = "WAITING";
                        else if (testOp.Status.ToString() == ((int)CallGeneratorLibrary.Utilities.Enums.CallStatus.ErrorMessage).ToString())
                            responseTestOp.Status = "ERROR";
                        else if (testOp.Status.ToString() == ((int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Phase).ToString())
                            responseTestOp.Status = "FAS";
                        else
                            responseTestOp.Status = "NO STATUS";

                        if (testOp.ErrorMessage != null)
                            responseTestOp.ErrorMessage = testOp.ErrorMessage;
                        responseTestOp.progressNbr = "1";

                        // responseTestOp.DisplayMessage = " testt";
                        // responseTestOp.DisplayMessageStatus = "0";
                    }
                    LstresponseTestOp.Add(responseTestOp);
                }
            }

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(jsonSerializer.Serialize(LstresponseTestOp.ToArray()));
        }
        catch (System.Exception ex)
        {
            Logger.LogException(ex);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}   