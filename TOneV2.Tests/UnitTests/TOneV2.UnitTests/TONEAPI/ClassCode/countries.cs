using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode


{
    public class countries
    {
        public string getcountires(RestClient rs, Uri ur, string token,string connections)
        {
            connect con = new connect();
            string country = rs.Getresposnse(ur, token);
            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<List<Countryclass>>(country);
                result = result + "Success: get Countries  \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Get Countries','Success','Get Countries',getdate(),'API'");

                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='country' and httpmethod='GET'", connections);
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query, connections);


                List<Countryclass> LC = ds1.Tables[0].AsEnumerable().Select(row => new Countryclass
                {

                    CountryId = row.Field<int>("countryid"),
                    Name = row.Field<string>("Name")

                }).ToList();



                List<Countryclass> ff = (List<Countryclass>)objResponse1;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  Countries count correct  \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Countries count','Success','countries count correct',getdate(),'API'");
                }


                bool correctcountry = false;
                foreach (Countryclass c in LC)
                {
                    if (ff.Any(countr => countr.CountryId == c.CountryId && countr.Name == c.Name))
                    {
                        correctcountry = true;
                    }
                    else
                        correctcountry = false;
                }
                if (correctcountry)
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Countries count','Success','countries count = DB Count',getdate(),'API'");
                    result = result + " Success : Countries equal countries in DB  \n|";

            }

            catch
            {

                result = result + "Failed: get countries  \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Get Countries','Fail','Fail Get Countries',getdate(),'API'");
            }

            return result;

        }


        public string createcountry(RestClient rs, Uri ur, string token, string data)
        {
            connect con = new connect();
            //   string endPoint = @"http://192.168.110.195:8103/api/VRCommon/Country/AddCountry";
            //RestClient  client = new RestClient(endpoint: endPoint,
            //                          method: HttpVerb.POST,
            //                          contenttype: "application/json;charset=UTF-8",
            //                          Auth_token:token,
            //                          postData: "{\"CountryId\":\"0\",  \"Name\":\"Batatas\"}");

            rs.EndPoint = ur.ToString();
            //RestClient  client = new RestClient(endpoint: endPoint,

            rs.PostData = data;
            string result = rs.MakeRequests("", token);


           
            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','create Country','success','" + result + "',getdate(),'API'");
            return result;

        }

    }
}