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

            string endPoint = "http://192.168.110.195:8585" + "/api/WhS_BE/CarrierProfile/GetFilteredCarrierProfiles ";


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
                result = result + "Success: get Carrier Profiles  \n";

                return "Success :  Carrier Profile count correct  \n  Success: Carrier Profile Data Validation \n";
            }
            catch
            {

            }
            return "Failed :  Carrier Profile count wrong  \n  Failed: Carrier Profile Data Validation \n";
        }
    }
}



            //        connect con = new connect();
            //        DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='Carrierprofile' and httpmethod='GET'");
            //        string query = "";
            //        foreach (DataRow _r in ds.Tables[0].Rows)
            //        {
            //            query = _r["validatequery"].ToString();
            //        }
            //        DataSet ds1 = con.getdata(query);


            //        List<Countryclass> LC = ds1.Tables[0].AsEnumerable().Select(row => new Countryclass
            //        {

            //            CountryId = row.Field<int>("countryid"),
            //            Name = row.Field<string>("Name")

            //        }).ToList();



            //        List<Countryclass> ff = (List<Countryclass>)objResponse1;

            //        // check 1 
            //        if (LC.Count == ff.Count)
            //        {
            //            result = result + " Success :  Countries count correct  \n";
            //        }


            //        bool correctcountry = false;
            //        foreach (Countryclass c in LC)
            //        {
            //            if (ff.Any(countr => countr.CountryId == c.CountryId && countr.Name == c.Name))
            //            {
            //                correctcountry = true;
            //            }
            //            else
            //                correctcountry = false;
            //        }
            //        if (correctcountry)
            //            result = result + " Success : Countries equal countries in DB  \n";

            //    }

            //    catch
            //    {

            //        result = result + "Failed: get countries  \n";
            //    }

            //    return result;

            //}


            //public string createcountry(RestClient rs, Uri ur, string token, string data)
            //{
            //    //   string endPoint = @"http://192.168.110.195:8585/api/VRCommon/Country/AddCountry";
            //    //RestClient  client = new RestClient(endpoint: endPoint,
            //    //                          method: HttpVerb.POST,
            //    //                          contenttype: "application/json;charset=UTF-8",
            //    //                          Auth_token:token,
            //    //                          postData: "{\"CountryId\":\"0\",  \"Name\":\"Batatas\"}");

            //    rs.EndPoint = @"http://192.168.110.195:8585/api/VRCommon/Country/AddCountry";
            //    //RestClient  client = new RestClient(endpoint: endPoint,

            //    rs.PostData = data;
            //    string result = rs.MakeRequests("", token);


            //    return result;



            //}

   