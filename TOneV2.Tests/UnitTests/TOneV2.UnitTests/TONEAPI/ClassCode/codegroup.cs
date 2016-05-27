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

        public string getcodegroup( string token)
        {

            string endPoint = "http://192.168.110.195:8585" + "/api/WhS_BE/CodeGroup/GetFilteredCodeGroups";


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
                            JsonConvert.DeserializeObject < jasonrespCG > (codegroups);
                result = result + "Success: get codegroup  \n";

                connect con = new connect();
                DataSet ds = con.getdata("SELECT   [validatequery]  FROM [TONEV2TESTAPI].[dbo].[testtable]  where unittype='codeGroup' and httpmethod='GET'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<EntityCG> LC = ds1.Tables[0].AsEnumerable().Select(row => new EntityCG
                {

                     Code = row.Field<string>("Code"),
                     CodeGroupId = row.Field<int>("CodeGroupID"),
                     CountryId= row.Field<int>("CountryID"),

                }).ToList();


                
                               List<DatumCG > ff = (List<DatumCG >)objResponse1.Data ;

                               // check 1 
                               if (LC.Count == ff.Count)
                               {
                                   result = result + " Success :  codegroup count correct  \n|";
                               }


                               bool correctcodegroup = false;
                               foreach (EntityCG c in LC)
                               {
                                   if (ff.Any(countr => countr.Entity.Code == c.Code &&  countr.Entity.CodeGroupId == c.CodeGroupId &&  countr.Entity.CountryId == c.CountryId))
                                   {
                                       correctcodegroup = true;
                                   }
                                   else
                                       correctcodegroup  = false;
                               }
                               if (correctcodegroup)
                                   result = result + " Success : CodeGroup equal CodeGroup in DB  \n|";
   
                 

            }

            catch
            {

                result = result + "Failed: get CodeGroup  \n|";
            }

            return result;

        }


        // Add codegroup To database 


        public String Addcodegroup(String _PostData, String _Token)
        {
            string endPoint = "http://192.168.110.195:8585" + "/api/WhS_BE/CodeGroup/AddCodeGroup";
            string raddcountry;

            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST);
            client.PostData = _PostData;
            client.ContentType = "application/json;charset=UTF-8";
            raddcountry =  client.MakeRequested(_PostData, _Token);
            return raddcountry; 
        
         
        
        }
  
         

    }
}



     























