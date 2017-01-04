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

        public string GetSellingNunmberPlan(string _Token, string _api, string _postData, string connections, Uri ur)
        {
            connect con = new connect();

            string endPoint = ur.ToString() + _api;
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

                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SellingNumberPlan','get SellingNumberPlan','success',' Success getting SellingNumberPlan',getdate(),'API'");


                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SellingNumberPlan' and httpmethod='Post'", connections);
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query, connections);


                List<EntityNP> LC = ds1.Tables[0].AsEnumerable().Select(row => new EntityNP
                {

                    SellingNumberPlanId = row.Field<int>("SellingNumberPlanId"),
                    Name = row.Field<string>("Name"),

                }).ToList();



                List<DatumNP> ff = (List<DatumNP>)objResponse1.Data;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  SellingNumberingPlanCount count correct  \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SellingNumberPlan','count SellingNumberPlan ','success',' Selling NumberPlan count is correct',getdate(),'API'");
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
                {
                    result = result + " Success : Selling NumberPlan equal SellingNumberPlan in DB  \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SellingNumberPlan','compare SellingNumberPlan to db','success',' Selling NumberPlan equal SellingNumberPlan in DB',getdate(),'API'");
                }


            }

            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SellingNumberPlan','get SellingNumberPlan','Fail','Failure getting SellingNumberPlan',getdate(),'API'");
                result = result + "Failed: get SellingNumberPlan  \n|";
            }


            return result;


        }

        // Add Selling Numbering Plan 
        public String AddSellingNumberPlan(String _PostData, String _Token, string _api,Uri ur)
        {
            string endPoint = ur.ToString() + _api;
            string RaddSellingNumberPlan;

            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST);
            client.PostData = _PostData;
            client.ContentType = "application/json;charset=UTF-8";
            RaddSellingNumberPlan = client.MakeRequested(_PostData, _Token);
            return RaddSellingNumberPlan;
        }
  
    


    }
}