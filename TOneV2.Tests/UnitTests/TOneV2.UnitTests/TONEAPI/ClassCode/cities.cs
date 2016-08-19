using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class cities
    {

        public string getcities( string token)
        {
            connect con = new connect();

            string endPoint = "http://192.168.110.195:8585" + "/api/VRCommon/City/GetFilteredCities";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{},\"SortByColumnName\":\"Entity.CityId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}");
            string parameters = "{\"Query\":{},\"SortByColumnName\":\"Entity.CityId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}";


            string city = client.MakeRequested(parameters, token);
          
            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject < jasonresp > (city);
                result = result + "Success: get city   \n";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Get Cities','Success','Get cities',getdate(),'API'");
         
                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='city' and httpmethod='GET'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<City> LC = ds1.Tables[0].AsEnumerable().Select(row => new City
                {

                    CountryId = row.Field<int>("countryid"),
                    CityId = row.Field<int>("cityid"),
                    Name = row.Field<string>("Name"),

                }).ToList();


                
                               List<CityDetail > ff = (List<CityDetail >)objResponse1.Data ;

                               // check 1 
                               if (LC.Count == ff.Count)
                               {
                                   result = result + " Success :  City count correct  \n|";
                                   con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count','Success','cities count correct',getdate(),'API'");
                               }


                               bool correctcountry = false;
                               foreach (City c in LC)
                               {
                                   if (ff.Any(countr => countr.Entity.CountryId == c.CountryId &&  countr.Entity.Name == c.Name &&  countr.Entity.CityId == c.CityId))
                                   {
                                       correctcountry = true;
                                   }
                                   else
                                       correctcountry = false;
                               }
                               if (correctcountry)
                               {
                                   result = result + " Success : Cities equal Cities in DB  \n|";
                                   con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count','Success','cities count = DB count',getdate(),'API'");
                               }
                               else
                               {

                                   con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count','Fail','cities count != DB count',getdate(),'API'");
                               }
                 

            }

            catch
            {

                result = result + "Failed: get cities  \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities count','Fail','failure getting cities',getdate(),'API'");
            }

            return result;

        }


        // Add city To database 


        public String Addcity(String _PostData, String _Token)
        {
            connect con = new connect();
            string endPoint = "http://192.168.110.195:8585" + "/api/VRCommon/City/AddCity";
            string raddcountry;

            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST);
            client.PostData = _PostData;
            client.ContentType = "application/json;charset=UTF-8";
            raddcountry =  client.MakeRequested(_PostData, _Token);
            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Cities','Cities ADD','Success','Success adding new city',getdate(),'API'");
            return raddcountry; 
        
         
        
        }
  
         

    }
}



     