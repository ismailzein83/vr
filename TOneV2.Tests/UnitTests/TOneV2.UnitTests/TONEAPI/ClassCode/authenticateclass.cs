
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class authenticateclass
    {
         public string  Testauthentication(RestClient rs,string url,string data, string data2, string data3)
    {
           string x ="";
           connect con = new connect();
             
            // Authentication 
             string xx = " Started the Process +\n";
             string endPoint = url + @"/api/VR_Sec/Security/Authenticate";

             var client = rs;
             client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: data);
            try { 
            var json = client.MakeRequest();
            // AuthenticationObject obj = JsonConvert.SerializeObject(obj);
            RootObject obj = JsonConvert.DeserializeObject<RootObject>(json);
                  x= obj.AuthenticationObject.Token.ToString();
              
              xx = xx + "|" + "Success: Authentication process correct username \n";
              con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','Authentication process correct username',getdate(),'API'");
                }
            catch
            {
              xx = xx + "|"  + "Failed:Authentication process \n";
              con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','Authentication process correct username',getdate(),'API'");
            }
            client = new RestClient(endpoint: endPoint,
                                method: HttpVerb.POST,
                                contenttype: "application/json;charset=UTF-8",
                                postData: data2);
            try
            {
                var json = client.MakeRequest();
                // AuthenticationObject obj = JsonConvert.SerializeObject(obj);
                RootObject obj = JsonConvert.DeserializeObject<RootObject>(json);
                x = obj.AuthenticationObject.Token.ToString();

                xx = xx + "|" + "Failed: Authentication process wrong username ,correct password \n";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','Authentication process wrong username correct password',getdate(),'API'");
            }
            catch
            {
                xx = xx + "|" + "Success: Authentication process wrong username ,correct password \n";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','Authentication process wrong username correct password',getdate(),'API'");
            }
            client = new RestClient(endpoint: endPoint,
                                method: HttpVerb.POST,
                                contenttype: "application/json;charset=UTF-8",
                                postData: data3);
            try
            {
                var json = client.MakeRequest();
                // AuthenticationObject obj = JsonConvert.SerializeObject(obj);
                RootObject obj = JsonConvert.DeserializeObject<RootObject>(json);
                x = obj.AuthenticationObject.Token.ToString();

                xx = xx + "|" + "Failed: Authentication process correct username ,wrong password \n";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Fail','Authentication process correct username wrong password',getdate(),'API'");
            }
            catch
            {
                xx = xx + "|" + "Success: Authentication process correct username ,wrong password \n";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'security','login','Success','Authentication process correct username wrong password',getdate(),'API'");
            }

            return xx;
    }

    public string getauthenticationtoken(RestClient rt,string url, string data)
    {

         string x ="";

            // Authentication 
             string xx = " Started the Process +\n";
             string endPoint =url+ @"/api/VR_Sec/Security/Authenticate";

             var client = rt;
            try { 
            var json = client.MakeRequest();
            // AuthenticationObject obj = JsonConvert.SerializeObject(obj);
            RootObject obj = JsonConvert.DeserializeObject<RootObject>(json);
                  x= obj.AuthenticationObject.Token.ToString();
              
              xx = xx + "|" + "Success: Authentication process correct username \n";

                }
            catch
            {
              xx = xx + "|"  + "Failed:Authentication process \n";
            }
            return x;
    }
    public class AuthenticationObject
    {
        public string __invalid_name__type { get; set; }
        public string TokenName { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public int ExpirationIntervalInMinutes { get; set; }
        public string Token { get; set; }
    }

    public class RootObject
    {
        public string _invalid_name_type { get; set; }
        public int Result { get; set; }
        public object Message { get; set; }
        public AuthenticationObject AuthenticationObject { get; set; }
    }
    }

    public class DataRetrievalObject<T>
    {
        public List<T> Data { get; set; }
    }
    }
