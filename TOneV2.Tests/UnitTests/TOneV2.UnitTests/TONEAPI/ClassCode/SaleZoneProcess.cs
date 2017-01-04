using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class SaleZoneProcess
    {

        public string GetSaleZones(string _Token, string _api, string _postData, string connections)
        {
            string endPoint = "http://192.168.110.195:8103" + _api;
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
                            JsonConvert.DeserializeObject<JasonSZ>(RgetNumberPlan);
                result = result + "Success: get  SellingNumberPlan   \n";



                connect con = new connect();
                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SaleZone' and httpmethod='Post'", connections);
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query, connections);


                List<DatumSZ> LC = ds1.Tables[0].AsEnumerable().Select(row => new DatumSZ
                {

                    SellingNumberPlanName = row.Field<string>("SellingNumberPlanName"),
                    CountryName = row.Field<string>("CountryName"),
                    Entity = new EntitySZ
                    {

                        SaleZoneId = row.Field<Int64>("SaleZoneId"),
                        SellingNumberPlanId = row.Field<int>("SellingNumberPlanID"),
                        CountryId = row.Field<int>("CountryId"),
                        Name = row.Field<string>("Name"),
                        BED = row.Field<DateTime>("BED"),
                        EED = row.Field<object>("EED")



                    }


                }).ToList();



                List<DatumSZ> ff = (List<DatumSZ>)objResponse1.Data;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  Sale Zones  count correct  \n";
                }


                bool correctcountry = false;
                foreach (DatumSZ c in LC)
                {
                    if (ff.Any(countr => countr.Entity.SellingNumberPlanId == c.Entity.SellingNumberPlanId
                        && countr.Entity.Name == c.Entity.Name
                        && countr.Entity.SaleZoneId == c.Entity.SaleZoneId
                        && countr.Entity.CountryId == c.Entity.CountryId
                        && countr.Entity.BED == c.Entity.BED
                        && countr.Entity.EED == c.Entity.EED
                       ))
                    {
                        correctcountry = true;
                    }
                    else
                        correctcountry = false;
                }
                if (correctcountry)
                    result = result + " Success : SaleZone equal Salezone in DB  \n";



            }

            catch
            {

                result = result + "Failed: get Sale Zones  \n";
            }


            return result;


        }




    }
}