using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class currencies
    {

        public string getcurrencies(RestClient rs, Uri ur, string token, string connections)
        {
            connect con = new connect();
            string country = rs.MakeRequested("{\"Query\":{},\"SortByColumnName\":\"Entity.Name\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}", token);
            string result = "";
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<List<Currency>>(country);
                result = result + "Success: get Countries  \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'currencies','Get currencies','Success','Get currencies',getdate(),'API'");

                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='currency' and httpmethod='GET'", connections);
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



                List<Currency> ff = (List<Currency>)objResponse1;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    result = result + " Success :  Countries count correct  \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Countries','Countries count','Success','countries count correct',getdate(),'API'");
                }


                bool correctcountry = false;
                //foreach (Countryclass c in LC)
                //{
                //    if (ff.Any(countr => countr == c.CountryId && countr.Name == c.Name))
                //    {
                //        correctcountry = true;
                //    }
                //    else
                //        correctcountry = false;
                //}
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

    }
}