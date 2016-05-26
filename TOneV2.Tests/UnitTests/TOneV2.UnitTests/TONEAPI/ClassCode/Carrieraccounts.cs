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


                connect con = new connect();
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
                    result = result + "Success: get Carrier Accounts  \n";
                    result = result + "Success: Carrier Accounts count correct \n";
                }
                else
                {
                    result = result + "Success:  Carrier Accounts count wrong   \n";
                }




            }
            catch
            {
                return "Failed :  Carrier Profile count wrong  \n  Failed: Carrier Profile Data Validation \n";
            }
            return result; 
        }
    }
}