using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class SupplierCodes
    {
        public class Carrierfiltered
        {
            public int CarrierAccountId { get; set; }
            public string Name { get; set; }
        }
        public class Entity
        {
            public long SupplierCodeId { get; set; }
            public string Code { get; set; }
            public long ZoneId { get; set; }
            public DateTime BED { get; set; }
            public object EED { get; set; }
            public object SourceId { get; set; }
            public object CodeGroupId { get; set; }
        }

        public class Datum
        {
            public Entity Entity { get; set; }
            public string SupplierZoneName { get; set; }
        }

        public class code
        {
            public string ResultKey { get; set; }
            public IList<Datum> Data { get; set; }
            public int TotalCount { get; set; }
        }

        public string getfiltercarriers(RestClient rs, Uri ur, string token, string param, string connections)
        {
            connect con = new connect();
            string results = "";

            string endPoint = ur.ToString() + "/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "");
            string parameters = "{\"Query\":{},\"SortByColumnName\":\"Entity.CarrierProfileId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}";
            string account = client.Getresposnse(ur, token);
            try
            {
                var objResponse1 =
                            JsonConvert.DeserializeObject<List<Carrierfiltered>>(account);
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','success','Supplier code page : get countries',getdate(),'API'");
                results = results + "Success: get Countries for page supplier zone \n|";



                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SupplierCodes' and httpmethod='GET'", connections);
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query, connections);


                List<Carrierfiltered> LC = ds1.Tables[0].AsEnumerable().Select(row => new Carrierfiltered
                {

                    CarrierAccountId = row.Field<int>("ID")

                }).ToList();



                List<Carrierfiltered> ff = (List<Carrierfiltered>)objResponse1;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','success','Supplier code page :Carrier count correct',getdate(),'API'");
                    results = results + " Success :  Supplier Codes - Carrier count correct  \n|";
                }
                else
                {
                    
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','Fail','Supplier code page :Carrier count incorrect',getdate(),'API'");
                    results = results + " Success :  Supplier Codes - Carrier count in correct  \n|";
                }
                bool correctcountry = false;
                foreach (Carrierfiltered c in LC)
                {
                    if (ff.Any(countr => countr.CarrierAccountId == c.CarrierAccountId))
                    {
                        correctcountry = true;
                    }
                    else
                        correctcountry = false;
                }
                if (correctcountry)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','success','Supplier code page :Carrier equal carriers in DB ',getdate(),'API'");
                    results = results + " Success : Supplier Codes - Carrier equal carriers in DB  \n|";
                }

                //   results = results + "Success: get carrieraccounts for page Supplier zones  \n|";

            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','Fail','get carrieraccounts for page Supplier Codes  ',getdate(),'API'");
                results = results + "Failed: get carrieraccounts for page Supplier Codes  \n|";
            }
            return results;
        }

        public string getsupplierCodes(RestClient rs, Uri ur, string token, string param, string connections)
        {
            connect con = new connect();
            string endPoint = ur.ToString() + "/api/WhS_BE/SupplierCode/GetFilteredSupplierCodes";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{\"SupplierId\":2,\"EffectiveOn\":\"2016-05-30T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierCodeId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40000}");
            string parameters = "{\"Query\":{\"SupplierId\":2,\"EffectiveOn\":\"2016-05-30T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierCodeId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40000}";
            string ZONES = client.MakeRequested(parameters, token);
            string result = "";
            try
            {


                var objResponse1 =
                            JsonConvert.DeserializeObject<code>(ZONES);



                //  var dictionary = JsonConvert.DeserializeObject<zone>(ZONES).Data.ToDictionary(x => x.zoneEntity.SupplierZoneId, x => x.zoneEntity);


                List<Entity> ct = objResponse1.Data.AsEnumerable().Select(row => new Entity
                {
                    Code = row.Entity.Code.ToString(),
                    BED = row.Entity.BED,
                    ZoneId = row.Entity.ZoneId,
                    CodeGroupId = row.Entity.CodeGroupId != DBNull.Value ? row.Entity.CodeGroupId : 0,
                    EED = row.Entity.EED != DBNull.Value ? row.Entity.EED : DateTime.Parse("2020-02-02"),
                    SupplierCodeId = row.Entity.SupplierCodeId


                }).ToList();



                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SupplierCodes' and httpmethod='POST'", connections);
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query, connections);


                //List<Entity> LC = ds1.Tables[0].AsEnumerable().Select(row => new Entity
                //{
                //    Code = row.Field<string>("Code"),
                //    ZoneId = row.Field<long>("ZoneId"),
                //    BED = row.Field<DateTime>("BED"),
                //    EED = row["EED"] != DBNull.Value ? row.Field<DateTime>("EED") : DateTime.Parse("2020-01-01"),
                //    SupplierCodeId = row.Field<long>("SupplierCodeId"),
                //    CodeGroupId = row.Field<long>("CodeGroupId")


                //}).ToList();
                List<Entity> LC = new List<Entity>();
                foreach (DataRow row in ds1.Tables[0].Rows)
                {
                    try
                    {
                        Entity entity = new Entity();
                        entity.Code = row.Field<string>("Code");
                        entity.ZoneId = row.Field<long>("ZoneId");
                        entity.BED = row.Field<DateTime>("BED");
                        entity.CodeGroupId = row["CodeGroupid"] != DBNull.Value ? row.Field<int>("CodeGroupId") :0;
                        LC.Add(entity);
                     
                       
                    }
                    catch (Exception exc)
                    {

                    }
                }

                if (LC.Count == ct.Count)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','success','Supplier code page :browsing ',getdate(),'API'");
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','success','Supplier code page :Code count is correct ',getdate(),'API'");
                    result = result + "Success: Supplier Code Page   \n";
                    result = result + "Success:Supplier Code Page Zone count correct \n|";
                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','Fail','Supplier code page :Code count in correct',getdate(),'API'");
                    result = result + "Success:Supplier Code Page Zone count in correct    \n|";
                }

                bool correctaccount = false;
                foreach (Entity c in LC)
                {
                    if (ct.Any(countr => countr.Code == c.Code && countr.ZoneId == c.ZoneId && countr.BED == c.BED))// && countr.EED == c.EED))
                    {
                        correctaccount = true;
                    }
                    else
                        correctaccount = false;
                }
                if (correctaccount)
                {
                    result = result + " Success : Supplier Code Page codes are retrieved correctly \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','success','Supplier code page :codes are retrieved correctly ',getdate(),'API'");


                }


            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierCode','Supplier Code','Fail','Supplier code page :Code Data Validation \',getdate(),'API'");
                return "Failed : Supplier Zone Code Zones count wrong  \n  Failed: Supplier Code Page Zones Data Validation \n|";
            }
            return result;


        }


    }
}