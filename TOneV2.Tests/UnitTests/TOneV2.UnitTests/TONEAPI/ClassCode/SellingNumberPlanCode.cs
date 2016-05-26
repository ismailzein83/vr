using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class SellingNumberPlanCode
    {

        public string GetSellingNunmberPlan(string _Token, string _api, string _postData)
        {
            string endPoint = "http://192.168.110.195:8585" + _api;
            string RgetNumberPlan;

            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST);
            client.PostData = _postData;
            client.ContentType = "application/json;charset=UTF-8";
            RgetNumberPlan = client.MakeRequested(_postData, _Token);


            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<SellingNumberPlan>(RgetNumberPlan);
                result = result + "Success: get  SellingNumberPlan   \n";



                connect con = new connect();
                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SellingNumberPlan' and httpmethod='Post'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<EntityNP> LC = ds1.Tables[0].AsEnumerable().Select(row => new EntityNP
                {

                    SellingNumberPlanId = row.Field<int>("SellingNumberPlanId"),
                    Name = row.Field<string>("Name"),

                }).ToList();



                List<DatumNP> ff = (List<DatumNP>)objResponse1.Data;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  SellingNumberingPlanCount count correct  \n";
                }


                bool correctcountry = false;
                foreach (EntityNP c in LC)
                {
                    if (ff.Any(countr => countr.Entity.SellingNumberPlanId == c.SellingNumberPlanId && countr.Entity.Name == c.Name))
                    {
                        correctcountry = true;
                    }
                    else
                        correctcountry = false;
                }
                if (correctcountry)
                    result = result + " Success : Selling NumberPlan equal SellingNumberPlan in DB  \n";



            }

            catch
            {

                result = result + "Failed: get SellingNumberPlan  \n";
            }


            return result;


        }



    }
}