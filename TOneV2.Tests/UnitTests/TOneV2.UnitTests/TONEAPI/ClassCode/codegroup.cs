using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class codegroup
    {

        public string getcodegroup(string token, string connections, Uri ur)
        {
            connect con = new connect();

            string endPoint = ur.ToString() + "api/WhS_BE/CodeGroup/GetFilteredCodeGroups";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{},\"SortByColumnName\":\"Entity.CodeGroupId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}");
            string parameters = "{\"Query\":{},\"SortByColumnName\":\"Entity.CodeGroupId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":30}";


            string codegroups = client.MakeRequested(parameters, token);

            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<DataRetrievalObject<TOne.WhS.BusinessEntity.Entities.CodeGroupDetail>>(codegroups);
                result = result + "Success: get codegroup  \n";

                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','Get CodeGroup','success','Success getting code group',getdate(),'API'");
                DataSet ds = con.getdata("SELECT   [validatequery]  FROM [TONEV2testing].[dbo].[testtable]  where unittype='codeGroup' and httpmethod='GET'", connections);
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query, connections);


                List<EntityCG> LC = ds1.Tables[0].AsEnumerable().Select(row => new EntityCG
                {

                    Code = row.Field<string>("Code"),
                    Id = row.Field<int>("ID"),
                    CountryId = row.Field<int>("CountryID"),

                }).ToList();



                var ff = objResponse1.Data;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  codegroup count correct  \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','Code group count','success',' code group count is correct',getdate(),'API'");
                }


                //bool correctcodegroup = false;
                //foreach (EntityCG c in LC)
                //{
                //    if (ff.Any(countr => countr.Entity.Code == c.Code && countr.Entity.Id == c.Id && countr.Entity.CountryId == c.CountryId))
                //    {
                //        correctcodegroup = true;
                //    }
                //    else
                //        correctcodegroup = false;
                //}
                //if (correctcodegroup)
                //{
                //    result = result + " Success : CodeGroup equal CodeGroup in DB  \n|";
                //    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','Code group compare to DB','success',' code group equal to DB ',getdate(),'API'");
                //}


            }

            catch
            {

                result = result + "Failed: get CodeGroup  \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','Code group count','Fail','Fail getting code groups',getdate(),'API'");
            }

            return result;

        }


        // Add codegroup To database 


        public String Addcodegroup(String _PostData, String _Token, Uri ur)
        {
            connect con = new connect();
            string endPoint = ur.ToString() + "/api/WhS_BE/CodeGroup/AddCodeGroup";
            string raddcountry;

            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST);
            client.PostData = _PostData;
            client.ContentType = "application/json;charset=UTF-8";
            raddcountry = client.MakeRequested(_PostData, _Token);
            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'CodeGroup','Code group create','success','success:code group create',getdate(),'API'");
            return raddcountry;



        }



    }
}



























