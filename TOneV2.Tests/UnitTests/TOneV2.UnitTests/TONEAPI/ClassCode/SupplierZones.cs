using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace TONEAPI.ClassCode
{
    public class SupplierZones
    {

        public class  Carrierfiltered
    {
        public int CarrierAccountId { get; set; }
        public string Name { get; set; }
    }

        public class Entity
        {
            public int SupplierZoneId { get; set; }
            public int CountryId { get; set; }
            public int SupplierId { get; set; }
            public string Name { get; set; }
            public DateTime BED { get; set; }
            public object EED { get; set; }
            public object SourceId { get; set; }
        }

        public class Datum
        {
            public Entity Entity { get; set; }
            public string CountryName { get; set; }
            public string SupplierName { get; set; }
        }

        public class zone
        {
            public object ResultKey { get; set; }
            public IList<Datum> Data { get; set; }
            public int TotalCount { get; set; }
        }
       

        public string getfiltercarriers(RestClient rs, Uri ur, string token, string param)
        {
            connect con = new connect();
            string results = "";

            string endPoint = "http://192.168.110.195:8585" + "/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo";


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
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','success','get countries SupplierZone',getdate(),'API'");
                results = results + "Success: get Countries for page supplier zone \n|";

                


                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SupplierZones' and httpmethod='GET'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<Carrierfiltered> LC = ds1.Tables[0].AsEnumerable().Select(row => new Carrierfiltered
                {

                    CarrierAccountId = row.Field<int>("ID")

                }).ToList();



                List<Carrierfiltered> ff = (List<Carrierfiltered>)objResponse1;

                // check 1 
                if (LC.Count == ff.Count)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','success','Carrier count is correct',getdate(),'API'");
                    results = results + " Success :  Supplier Zone - Carrier count correct  \n|";
                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','Fail','Carrier count is incorrect',getdate(),'API'");
                    results = results + " Success :  Supplier Zone - Carrier count in correct  \n|";
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
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','success','Carriers = Carriers in DB',getdate(),'API'");
                    results = results + " Success : Supplier Zone - Carrier equal Carriers in DB  \n|";
                
                }

             //   results = results + "Success: get carrieraccounts for page Supplier zones  \n|";

            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','Fail','Failure getting carriers for Supplier zones',getdate(),'API'");
                results = results + "Failed: get carrieraccounts for page Supplier zones  \n|";
            }
            return results;
        }

        public string getsupplierzones(RestClient rs, Uri ur, string token, string param)
        {
            connect con = new connect();
              string endPoint = "http://192.168.110.195:8585" + "/api/WhS_BE/SupplierZone/GetFilteredSupplierZones";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{\"SupplierId\":43,\"EffectiveOn\":\"2016-05-29T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierZoneId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":10000}");
            string parameters = "{\"Query\":{\"SupplierId\":43,\"EffectiveOn\":\"2016-05-29T21:00:00.000Z\"},\"SortByColumnName\":\"Entity.SupplierZoneId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":10000}";
            string ZONES = client.MakeRequested(parameters, token);
            string result = "";
            try
            {


                var objResponse1 =
                            JsonConvert.DeserializeObject<zone>(ZONES);



              //  var dictionary = JsonConvert.DeserializeObject<zone>(ZONES).Data.ToDictionary(x => x.zoneEntity.SupplierZoneId, x => x.zoneEntity);


                List<Entity> ct = objResponse1.Data.AsEnumerable().Select(row => new Entity
                {
                    SupplierId = row.Entity.SupplierId,
                    BED = row.Entity.BED,
                    Name=row.Entity.Name,
                    CountryId = row.Entity.CountryId
                    
                }).ToList();
                   
               
               
                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='SupplierZones' and httpmethod='POST'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<Entity> LC = ds1.Tables[0].AsEnumerable().Select(row => new Entity
                {

                    SupplierId = row.Field<int>("supplierid")
                    ,Name = row.Field<string>("Name")
                    ,BED =row.Field<DateTime>("BED")

                    ,
                    EED = row["EED"] != DBNull.Value ? row.Field<DateTime>("EED") : DateTime.Parse("2000-01-01")
              
                    ,CountryId = row.Field<int>("Countryid")

                }).ToList();

                if (LC.Count == ct.Count)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','success','Supplier Zone Page browsing',getdate(),'API'");
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','success','Supplier Zone Page: Zone count correct',getdate(),'API'");
                    result = result + "Success: Supplier Zone Page   \n";
                    result = result + "Success:Supplier Zone Page Zone count correct \n|";
                }
                else
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','Fail','Supplier Zone Page: Zone count incorrect',getdate(),'API'");
                    result = result + "Success:Supplier Zone Page Zone count in correct    \n|";
                }

                bool correctaccount = false;
                foreach (Entity c in LC)
                {
                    if (ct.Any(countr => countr.CountryId == c.CountryId && countr.SupplierId == c.SupplierId && countr.BED == c.BED))// && countr.EED == c.EED))
                    {
                        correctaccount = true;
                    }
                    else
                        correctaccount = false;
                }
                if (correctaccount)
                {
                    result = result + " Success : Supplier Zone Page Zones are retrieved correctly \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','success','Supplier Zone Page Zones are retrieved correctly',getdate(),'API'");
                }


            }
            catch
            {
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','Fail','Supplier Zone Page Zones count wrong ',getdate(),'API'");
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'SupplierZone','SupplierZone','Fail','Supplier Zone Page Zones Data Validation ',getdate(),'API'");
                return "Failed : Supplier Zone Page Zones count wrong  \n  Failed: Supplier Zone Page Zones Data Validation \n|";
            }
            return result; 
             
        
        }
    }
}