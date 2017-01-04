using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class Switches
    {
        public class Switchobj
        {
            public object ResultKey { get; set; }
            public List<Switchdatas> switchData { get; set; }
            public int TotalCount { get; set; }
        }
        public class Switchdatas
        {
            public SwitchEntity switchEntity { get; set; }
        }
        public class SwitchEntity
        {
            public int SwitchId { get; set; }
            public string Name { get; set; }
            public object SourceId { get; set; }
        }

      

      

        public string getswitches(RestClient rs, Uri ur, string token)
        {
            connect con = new connect();
            string endPoint = "http://192.168.110.195:8103" + "/api/WhS_BE/Switch/GetFilteredSwitches  ";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{},\"SortByColumnName\":\"Entity.SwitchId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}");
            string parameters = "{\"Query\":{},\"SortByColumnName\":\"Entity.SwitchId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}";
            string switchdata = client.MakeRequested(parameters, token);
            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<Switchobj>(switchdata);



                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Switches','Get Switches','success','Success: get Switches',getdate(),'API'");
         
                result = result + "Success: get Switches  \n|";

                return "Success :  Switches count correct  \n  Success: Switches Data Validation \n|";
            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Switches','Get Switches','Fail','fail: get Switches',getdate(),'API'");
                return "Failed :  Switches count wrong  \n  Failed: Switches Data Validation \n|";
             

            }
        
        }

        public string createswitch(RestClient rs, Uri ur, string token, string data)
        {
            connect con = new connect();
            string result = "";

            try
            {
                string EndPoint = @"http://192.168.110.195:8103/api/WhS_BE/Switch/AddSwitch";
                var client = new RestClient(endpoint: EndPoint,
                                  method: HttpVerb.POST);
                client.PostData = data;
                client.ContentType = "application/json;charset=UTF-8";


                //string paramter = "{\"CarrierProfileId\":0,\"Name\":\"Test\",\"Settings\":{\"CountryId\":1,\"CityId\":9,\"Company\":\"Test\",\"Website\":\"123\",\"RegistrationNumber\":\"123\",\"Address\":\"123\",\"PostalCode\":\"123\",\"Town\":\"123\",\"CompanyLogo\":0,\"Contacts\":[{\"Type\":1,\"Description\":\"nab\"},{\"Type\":2,\"Description\":\"nab@vanrise.com\"},{\"Type\":4,\"Description\":\"nab\"},{\"Type\":5,\"Description\":\"nab@vanrise.com\"},{\"Type\":6,\"Description\":\"nab\"},{\"Type\":7,\"Description\":\"nab@vanrise.com\"},{\"Type\":8,\"Description\":\"nab\"},{\"Type\":9,\"Description\":\"nab@vanrise.com\"},{\"Type\":10,\"Description\":\"nab\"},{\"Type\":11,\"Description\":\"nab@vanrise.com\"},{\"Type\":12,\"Description\":\"nab\"},{\"Type\":13,\"Description\":\"nab@vanrise.com\"},{\"Type\":14,\"Description\":\"234233\"},{\"Type\":3,\"Description\":\"nab@vanrise.com\"}]}}";
                string paramter = data;
                 result = client.MakeRequested(paramter, token);


                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Switches','Add Switches','Success','Success: add Switches',getdate(),'API'");
            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Switches','Add Switches','Fail','Failure: add Switches',getdate(),'API'");
                
            }
            return result;
        }
    }
}