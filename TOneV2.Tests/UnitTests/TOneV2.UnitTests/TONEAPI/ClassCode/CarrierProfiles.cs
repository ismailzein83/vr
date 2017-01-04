using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace TONEAPI.ClassCode
{
    public class CarrierProfiles
    {

        public class Contact
        {
            [JsonProperty("Type")]
            public int Type { get; set; }
            [JsonProperty("Description")]
            public string Description { get; set; }
        }

        public class Settings
        {
            [JsonProperty("Company")]
            public string Company { get; set; }
            [JsonProperty("CountryId")]
            public int? CountryId { get; set; }
            [JsonProperty("CityId")]
            public int? CityId { get; set; }
            [JsonProperty("RegistrationNumber")]
            public string RegistrationNumber { get; set; }
            [JsonProperty("PhoneNumbers")]
            public IList<string> PhoneNumbers { get; set; }
            [JsonProperty("Faxes")]
            public IList<string> Faxes { get; set; }
            [JsonProperty("Website")]
            public string Website { get; set; }
            [JsonProperty("Address")]
            public string Address { get; set; }
            [JsonProperty("PostalCode")]
            public string PostalCode { get; set; }
            [JsonProperty("Town")]
            public string Town { get; set; }
            [JsonProperty("CompanyLogo")]
            public int CompanyLogo { get; set; }
            [JsonProperty("Contacts")]
            public IList<Contact> Contacts { get; set; }
        }

        public class Entity
        {
            [JsonProperty("CarrierProfileId")]
            public int CarrierProfileId { get; set; }
            [JsonProperty("Name")]
            public string Name { get; set; }
            [JsonProperty("Settings")]
            public Settings Settings { get; set; }
            [JsonProperty("SourceId")]
            public object SourceId { get; set; }
        }

        public class Datum
        {
            [JsonProperty("Entity")]
            public Entity Entity { get; set; }
            [JsonProperty("CountryName")]
            public string CountryName { get; set; }
        }

        public class ProfileData
        {
            [JsonProperty("ResultKey")]
            public string ResultKey { get; set; }
            [JsonProperty("Data")]
            public IList<Datum> Data { get; set; }
            [JsonProperty("TotalCount")]
            public int TotalCount { get; set; }
        }
        public string getprofiles(RestClient rs, Uri ur, string token)
        {
            connect con = new connect();

            string endPoint = "http://192.168.110.195:8103" + "/api/WhS_BE/CarrierProfile/GetFilteredCarrierProfiles ";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{},\"SortByColumnName\":\"Entity.CarrierProfileId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}");
            string parameters = "{\"Query\":{},\"SortByColumnName\":\"Entity.CarrierProfileId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}";
            string profiles = client.MakeRequested(parameters, token);
            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<ProfileData>(profiles);

                Settings CPsettings = objResponse1.Data[0].Entity.Settings;
          //      List<Entity> entity = (List<Entity>)CPsettings.

                string name = CPsettings.Company.ToString();
                result = result + "Success: get Carrier Profiles  \n|";

              
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Profile','Get carrier profile','Success','get Carrier Profiles',getdate(),'API'");

                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Profile','Carrier profile count/validation','Success','Success: Carrier Profile Data Validation',getdate(),'API'");

                return "Success :  Carrier Profile count correct  \n  Success: Carrier Profile Data Validation \n|";
            }
            catch
            {

            }
            return "Failed :  Carrier Profile count wrong  \n  Failed: Carrier Profile Data Validation \n|";
        }

        public string createprofile(RestClient rs, Uri ur, string token, string data)
        {
            connect con = new connect();
            string EndPoint = @"http://192.168.110.195:8103/api/WhS_BE/CarrierProfile/AddCarrierProfile";
            var client = new RestClient(endpoint: EndPoint,
                              method: HttpVerb.POST);
                client.PostData = data;
                client.ContentType = "application/json;charset=UTF-8";

         
            //string paramter = "{\"CarrierProfileId\":0,\"Name\":\"Test\",\"Settings\":{\"CountryId\":1,\"CityId\":9,\"Company\":\"Test\",\"Website\":\"123\",\"RegistrationNumber\":\"123\",\"Address\":\"123\",\"PostalCode\":\"123\",\"Town\":\"123\",\"CompanyLogo\":0,\"Contacts\":[{\"Type\":1,\"Description\":\"nab\"},{\"Type\":2,\"Description\":\"nab@vanrise.com\"},{\"Type\":4,\"Description\":\"nab\"},{\"Type\":5,\"Description\":\"nab@vanrise.com\"},{\"Type\":6,\"Description\":\"nab\"},{\"Type\":7,\"Description\":\"nab@vanrise.com\"},{\"Type\":8,\"Description\":\"nab\"},{\"Type\":9,\"Description\":\"nab@vanrise.com\"},{\"Type\":10,\"Description\":\"nab\"},{\"Type\":11,\"Description\":\"nab@vanrise.com\"},{\"Type\":12,\"Description\":\"nab\"},{\"Type\":13,\"Description\":\"nab@vanrise.com\"},{\"Type\":14,\"Description\":\"234233\"},{\"Type\":3,\"Description\":\"nab@vanrise.com\"}]}}";
            string paramter = data;
            string result = client.MakeRequested(paramter, token);


            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Profile','Create Carrier profile','Success','Success:Create Carrier Profile',getdate(),'API'");
            return result;
   
        }
    }
}




            

   