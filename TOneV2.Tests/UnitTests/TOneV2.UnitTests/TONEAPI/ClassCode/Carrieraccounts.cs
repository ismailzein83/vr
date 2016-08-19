using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class Carrieraccounts
    {
        public class SupplierSettings
        {
            public int RoutingStatus { get; set; }
        }

        public class CustomerSettings
        {
            public object DefaultRoutingProductId { get; set; }
            public int RoutingStatus { get; set; }
        }

        public class carrieracc
        {
            public int CarrierAccountId { get; set; }
            public string NameSuffix { get; set; }
            public int CarrierProfileId { get; set; }
            public int? SellingNumberPlanId { get; set; }
            public int AccountType { get; set; }
            public CarrierAccountSettings CarrierAccountSettings { get; set; }
            public SupplierSettings SupplierSettings { get; set; }
            public CustomerSettings CustomerSettings { get; set; }
            public object SourceId { get; set; }
        }
        class carrieraccountdet
        {
            public int CarrierAccountId { get; set; }
            public int CarrierProfileId { get; set; }
            public int? SellingNumberPlanId { get; set; }
            public int AccountType { get; set; }
        }
        public string getaccounts(RestClient rs, Uri ur, string token)
        {

            string endPoint = "http://192.168.110.195:8585" + "/api/WhS_BE/CarrierAccount/GetFilteredCarrierAccounts";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: "{\"Query\":{},\"SortByColumnName\":\"Entity.CarrierAccountId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}");
            string parameters = "{\"Query\":{},\"SortByColumnName\":\"Entity.CarrierProfileId\",\"IsSortDescending\":false,\"ResultKey\":null,\"DataRetrievalResultType\":0,\"FromRow\":1,\"ToRow\":40}";
            string account = client.MakeRequested(parameters, token);
            string result = "";
            connect con = new connect();
            try
            {


                CarrierAccountClass ca = new CarrierAccountClass();
                var objResponse1 =
                            JsonConvert.DeserializeObject<CarrierAccountClass>(account);

                List<carrieracc> ct = objResponse1.Data.AsEnumerable()
                    .Select(row => new carrieracc
                    {
                        CarrierAccountId = row.Entity.CarrierAccountId,
                        CarrierProfileId = row.Entity.CarrierProfileId,
                        SellingNumberPlanId = row.Entity.SellingNumberPlanId,
                        AccountType = row.Entity.AccountType

                    }).ToList();

                //      List<Entity> entity = (List<Entity>)CPsettings.

                // string name = CPsettings.Company.ToString();


             
                DataSet ds = con.getdata("SELECT       [validatequery]  FROM [ToneV2testing].[dbo].[testtable]  where unittype='Carrieraccount' and httpmethod='POST'");
                string query = "";
                foreach (DataRow _r in ds.Tables[0].Rows)
                {
                    query = _r["validatequery"].ToString();
                }
                DataSet ds1 = con.getdata(query);


                List<carrieraccountdet> LC = ds1.Tables[0].AsEnumerable().Select(row => new carrieraccountdet
                       {

                           CarrierAccountId = row.Field<int>("id"),
                           CarrierProfileId = row.Field<int>("CarrierProfileID"),
                           SellingNumberPlanId = row["SellingNumberPlanID"] != DBNull.Value ? row.Field<int>("SellingNumberPlanID") : 0,
                           AccountType = row.Field<int>("AccountType")

                       }).ToList();

                if (LC.Count == ct.Count)
                {
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Account','Get Carrier Account','Success','Success:Get Carrier Account',getdate(),'API'");
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Account','Carrier Account count/validation','Success','Success: Carrier Account Data Validation',getdate(),'API'");
                    result = result + "Success: get Carrier Accounts  \n";
                    result = result + "Success: Carrier Accounts count correct \n|";
                }
                else
                {
                    result = result + "Success:  Carrier Accounts count wrong   \n|";
                    con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Account','Carrier Account count/validation','Fail','Fail: Carrier Account Data Validation',getdate(),'API'");
                }

                bool correctaccount = false;
                foreach (carrieraccountdet c in LC)
                {
                    if (ct.Any(countr => countr.CarrierAccountId == c.CarrierAccountId && countr.CarrierProfileId == c.CarrierProfileId  && countr.SellingNumberPlanId==c.SellingNumberPlanId && countr.AccountType ==c.AccountType ))
                    {
                        correctaccount = true;
                    }
                    else
                        correctaccount = false;
                }
                if (correctaccount)
                    result = result + " Success : CarrierAccount are retrieved correctly \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Account','Carrier Account Data','Success','Success: Carrier Account Data retrived correctly',getdate(),'API'");



            }
            catch
            {
                return "Failed :  Carrier Account count wrong  \n  Failed: Carrier Account Data Validation \n|";
                con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Account','Carrier Account Data','Fail','Fail getting Carrier Account Data ',getdate(),'API'");
            }
            return result; 
        }
          public string createaccount(RestClient rs, Uri ur, string token, string data)
        {
            connect con = new connect();
            string EndPoint = @"http://192.168.110.195:8585/api/WhS_BE/CarrierAccount/AddCarrierAccount";
            var client = new RestClient(endpoint: EndPoint,
                              method: HttpVerb.POST);
                client.PostData = data;
                client.ContentType = "application/json;charset=UTF-8";

         
            //string paramter = "{\"CarrierProfileId\":0,\"Name\":\"Test\",\"Settings\":{\"CountryId\":1,\"CityId\":9,\"Company\":\"Test\",\"Website\":\"123\",\"RegistrationNumber\":\"123\",\"Address\":\"123\",\"PostalCode\":\"123\",\"Town\":\"123\",\"CompanyLogo\":0,\"Contacts\":[{\"Type\":1,\"Description\":\"nab\"},{\"Type\":2,\"Description\":\"nab@vanrise.com\"},{\"Type\":4,\"Description\":\"nab\"},{\"Type\":5,\"Description\":\"nab@vanrise.com\"},{\"Type\":6,\"Description\":\"nab\"},{\"Type\":7,\"Description\":\"nab@vanrise.com\"},{\"Type\":8,\"Description\":\"nab\"},{\"Type\":9,\"Description\":\"nab@vanrise.com\"},{\"Type\":10,\"Description\":\"nab\"},{\"Type\":11,\"Description\":\"nab@vanrise.com\"},{\"Type\":12,\"Description\":\"nab\"},{\"Type\":13,\"Description\":\"nab@vanrise.com\"},{\"Type\":14,\"Description\":\"234233\"},{\"Type\":3,\"Description\":\"nab@vanrise.com\"}]}}";
            string paramter = data;
            string result = client.MakeRequested(paramter, token);

            con.updatedata("INSERT INTO [dbo].[logging]           ([module]           ,[Events]           ,[status]           ,[messages]           ,[eventdate],unit)    select 'Carrier Account','add  Carrier Account','Success','Success: Carrier Account created " + result + "',getdate(),'API'");

            return result;
   
        }
        
        
    }
}