﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode


{
    public class countries
    {
        public string getcountires(RestClient rs, Uri ur, string token)
        {
            string country = rs.Getresposnse(ur, token);
            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<List<Countryclass>>(country);
                result = result + "Success: get Countries  \n";

                connect con = new connect();
                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='country' and httpmethod='GET'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<Countryclass> LC = ds1.Tables[0].AsEnumerable().Select(row => new Countryclass
                {

                    CountryId = row.Field<int>("countryid"),
                    Name = row.Field<string>("Name")

                }).ToList();



                List<Countryclass> ff = (List<Countryclass>)objResponse1;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  Countries count correct  \n";
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
                    result = result + " Success : Countries equal countries in DB  \n";

            }

            catch
            {

                result = result + "Failed: get countries  \n";
            }

            return result;

        }


        public string createcountry(RestClient rs, Uri ur, string token, string data)
        {
            //   string endPoint = @"http://192.168.110.195:8585/api/VRCommon/Country/AddCountry";
            //RestClient  client = new RestClient(endpoint: endPoint,
            //                          method: HttpVerb.POST,
            //                          contenttype: "application/json;charset=UTF-8",
            //                          Auth_token:token,
            //                          postData: "{\"CountryId\":\"0\",  \"Name\":\"Batatas\"}");

            rs.EndPoint = @"http://192.168.110.195:8585/api/VRCommon/Country/AddCountry";
            //RestClient  client = new RestClient(endpoint: endPoint,

            rs.PostData = data;
            string result = rs.MakeRequests("", token);


            return result;



        }

    }
}