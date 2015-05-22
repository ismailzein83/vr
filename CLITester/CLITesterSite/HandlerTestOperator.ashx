<%@ WebHandler Language="C#" Class="HandlerTestOperator" %>

using System;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
public class HandlerTestOperator : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private class RequestTestCall
    {
        public string id1 { get; set; }
        public string id2 { get; set; }
    }
    
    private class ResponseTestOperator
    {
        public ResponseTestOperator()
        {
            needRedirect = false;
            ErrorMessage = "";
            isDone = true;
        }
        
        public string Id { get; set; }
        public string OperatorId { get; set; }
        public string CountryPic { get; set; }
        public string Prefix { get; set; }
        public string CreationDate { get; set; }
        public string EndDate { get; set; }
        public string ReceivedCli { get; set; }
        public string Status { get; set; }
        public string TestCli { get; set; }
        public string Redirect { get; set; }
        public bool needRedirect { get; set; }
        public string ErrorMessage { get; set; }
        public bool isDone { get; set; }
    }
    
    public void ProcessRequest(HttpContext context)
    {
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

            ResponseTestOperator responseTestOp = new ResponseTestOperator();
            
            int OperatorId = 0;
            int.TryParse(resp.id1, out OperatorId);

            int balance = 0;
            int Requested = 0;
            int? ParentId = null;
            ParentId = UserRepository.GetParentId(Current.getCurrentUser(context).Id);
            
            if(ParentId == null)
                balance = UserRepository.Load(Current.getCurrentUser(context).Id).Balance.Value;
            else
                balance = UserRepository.Load(ParentId.Value).Balance.Value;

            Requested = TestOperatorRepository.GetRequestedTestOperatorsByUser(ParentId.Value);

            if (balance - Requested > 0)
            {
                TestOperator testOp = new TestOperator();
                testOp.UserId = Current.getCurrentUser(context).Id;
                testOp.ParentUserId = ParentId;
                
                if (OperatorId != 0)
                    testOp.OperatorId = OperatorId;

                testOp.NumberOfCalls = 1;
                testOp.CreationDate = DateTime.Now;
                testOp.CarrierPrefix = resp.id2;
                //testOp.CarrierPrefix = "";
                testOp.CallerId = Current.getCurrentUser(context).CallerId;

                bool saveB = TestOperatorRepository.Save(testOp);

                int TestOperatorId = testOp.Id;

                responseTestOp.Id = testOp.Id.ToString();

                responseTestOp.CountryPic = OperatorRepository.Load(testOp.OperatorId.Value).CountryPicture;
                responseTestOp.CreationDate = testOp.CreationDate.ToString();
                responseTestOp.EndDate = "";
                responseTestOp.OperatorId = OperatorRepository.Load(testOp.OperatorId.Value).FullName;
                responseTestOp.Prefix = testOp.CarrierPrefix;
                responseTestOp.ReceivedCli = "";
                responseTestOp.Status = "";
                responseTestOp.TestCli = "";
                responseTestOp.ErrorMessage = "";

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                context.Response.Write(jsonSerializer.Serialize(responseTestOp));
            }
            else
            {
                responseTestOp.isDone = false;

                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                context.Response.Write(jsonSerializer.Serialize(responseTestOp));
            }
            
            //context.Response.ContentType = "text/plain";
            //context.Response.Write(TestOperatorId);
        }
        catch (System.Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}