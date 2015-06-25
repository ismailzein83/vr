<%@ WebHandler Language="C#" Class="HandlerGetCountryMap" %>

using System;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
using System.Collections.Generic;

public class HandlerGetCountryMap : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    private class RequestInput
    {
        public string code { get; set; }
    }
    
    private class ResponseOutput
    {
        public ResponseOutput()
        {
            needRedirect = false;
        }

        public string Operator { get; set; }
        public int Total { get; set; }
        public int Delivered { get; set; }
        public int NotDelivered { get; set; }
        public decimal percentage { get; set; }
        public decimal percentageW { get; set; }
        
        
        public string Redirect { get; set; }
        public bool needRedirect { get; set; }
    }
    
    public void ProcessRequest (HttpContext context) {
        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonString = String.Empty;
        
        if (!Current.getCurrentUser(context).IsAuthenticated)
        {
            ResponseOutput responseRed = new ResponseOutput();
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

            var emplList = jsonSerializer.Deserialize<System.Collections.Generic.List<RequestInput>>(jsonString);

            RequestInput resp = emplList[0];

            String code = resp.code;

            List<ResponseOutput> responseTestOpLs = new List<ResponseOutput>();
            List<Operator> LstOperators = new List<Operator>();
            LstOperators = OperatorRepository.GetOperatorMap(code);
            foreach (Operator op in LstOperators)
            {
                ResponseOutput OMap = new ResponseOutput();
                OMap.Operator = op.Name;
                OMap.Total = TestOperatorRepository.GetMapCountry(null, Current.getCurrentUser(context).Id, op.Id);
                OMap.Delivered = TestOperatorRepository.GetMapCountry(1, Current.getCurrentUser(context).Id, op.Id);
                OMap.NotDelivered = TestOperatorRepository.GetMapCountry(2, Current.getCurrentUser(context).Id, op.Id);

                if (OMap.Delivered != 0 && OMap.Delivered != null)
                    OMap.percentage = decimal.Round((((decimal)(OMap.Delivered) / (decimal)(OMap.Total)) * 100), 2);
                else
                    OMap.percentage = 0;

                if (OMap.NotDelivered != 0 && OMap.NotDelivered != null)
                    OMap.percentageW = decimal.Round((((decimal)(OMap.NotDelivered) / (decimal)(OMap.Total)) * 100), 2);
                else
                    OMap.percentageW = 0;
                
                responseTestOpLs.Add(OMap);
            }

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(jsonSerializer.Serialize(responseTestOpLs.ToArray()));

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